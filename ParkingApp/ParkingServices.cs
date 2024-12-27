using System.Data.SqlTypes;
using System.Text.Json;

namespace ParkingApp;



public class ParkingServices
{
    private static readonly double Fee = 14;
    private static readonly double ReducedFee = 6;
    private static readonly int StartTimeOfFullFee = 8;
    private static readonly int StartTimeOfReducedFee = 18;
    private static readonly double MaxFullFare = (StartTimeOfReducedFee - StartTimeOfFullFee) * Fee;
    private string _parkingUsersFile = "ParkingUsers.json";
    private string _activeParkingPeriodsFile = "ActiveParkingPeriods.json";

    public List<ParkingUser> ParkingUsers { get; set; }
    public List<ParkingPeriod> ActiveParkingPeriods { get; set; }
    public ParkingServices()
    {
        ParkingUsers = new List<ParkingUser>();
        ActiveParkingPeriods = new List<ParkingPeriod>();

        // read in the lists from file here
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
            string jsonUsers = File.ReadAllText(_parkingUsersFile);
            List<ParkingUser>? users = JsonSerializer.Deserialize<List<ParkingUser>>(jsonUsers, options);
            if (users != null)
                ParkingUsers.AddRange(users);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        try
        {
            string jsonParkingPeriods = File.ReadAllText(_activeParkingPeriodsFile);
            List<ParkingPeriod>? activePeriodsFromFile = JsonSerializer.Deserialize<List<ParkingPeriod>>(jsonParkingPeriods);
            if (activePeriodsFromFile != null)
                ActiveParkingPeriods.AddRange(activePeriodsFromFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    // a method to report how many users and active parking periods are in the system.
    public string Report()
    {
        string? parkedCars = "";
        foreach (ParkingPeriod period in ActiveParkingPeriods)
        {
            parkedCars += " "+period.ParkedCar.LicencePlate + " ,";
        }
        string finish = parkedCars == "" ? "." : $" for the following cars: {parkedCars}.";
        return $"There are {ParkingUsers.Count} users and {ActiveParkingPeriods.Count} active parking periods{finish.Trim(',')}";
    }
    public string ReportUsers()
    {
        string report = "Users:\n";
        foreach (ParkingUser user in ParkingUsers)
        {
            report += $"User: {user.UserName}, Email: {user.Email}, Cars: {user.Cars.Count}\n";
            foreach (Car car in user.Cars)
            {
                report += $"  - Licence Plate: {car.LicencePlate}\n";
            }
        }
        return report;
    }
    public ParkingPeriod? CurrentlyParked(string licencePlate)
    {
        if (ActiveParkingPeriods.Count == 0)
        {
            Console.WriteLine("No active parking periods found.");
            return null;
        }
        if (licencePlate == null)
        {
            throw new Exception("You must enter a licence plate");
        }
        string parkedCar = licencePlate.ToUpper();
        ParkingPeriod? activePeriod = ActiveParkingPeriods.FirstOrDefault(p => p.ParkedCar.LicencePlate.Equals(parkedCar));
        return activePeriod;
    }
    public void StartParkingPeriod(int userID, string carLicencePlate)
    {
        if (CurrentlyParked(carLicencePlate) != null)
        {
            throw new Exception("Car is already parked.");
        }
        DateTime startTime = DateTime.Now;

        // Find the user, and start the parking period for the car
        ParkingUser? user = ParkingUsers.Find(u => u.Id == userID);
        if (user == null)
        {
            throw new Exception("User does not exist");
        }
        else
        {
            Car? parkedCar = user.Cars.Find(c => c.LicencePlate.Equals(carLicencePlate));
            if (parkedCar == null)
            {
                throw new Exception("Car not found for this user.");
            }
            else
            {
                ParkingPeriod newPeriod = new ParkingPeriod(startTime, userID, parkedCar);
                ActiveParkingPeriods.Add(newPeriod);
                WriteToFile(ActiveParkingPeriods, _activeParkingPeriodsFile);
            }
        }

    }
    public void StopParkingPeriod(string carLicencePlate)
    {
        ParkingPeriod? parkingPeriod = null;
        DateTime stopTime = DateTime.Now;
        if (ActiveParkingPeriods.Count == 0)
        {
            throw new Exception("No active parking periods found.");
        }
        try
        {
            parkingPeriod = CurrentlyParked(carLicencePlate);
            if (parkingPeriod == null)
            {
                throw new Exception("No active parking period found for this car.");
            }
            DateTime startTime = parkingPeriod.StartTime;
            ParkingUser? user = ParkingUsers.Find(u => u.Id == parkingPeriod.UserID);
            double feeToAdd = CalculateFee(startTime, stopTime);

            parkingPeriod.ParkingEnded(stopTime, feeToAdd);
            if (user == null) throw new Exception("No user found for this parking period.");
            user.LeaveParkingAndChargeAcconut(parkingPeriod);

            ActiveParkingPeriods.Remove(parkingPeriod);

            WriteToFile(ParkingUsers, _parkingUsersFile);
            WriteToFile(ActiveParkingPeriods, _activeParkingPeriodsFile);

        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }

    }

    public bool RegisterUser(string username, string password, string email, string licencePlate)
    {
        int highestID = ParkingUsers.Count == 0 ? 0 : ParkingUsers.Max(user => user.Id);

        ParkingUser newUser = new ParkingUser(++highestID, username, password, email);
        if (licencePlate != "") newUser.AddCar(licencePlate);
        ParkingUsers.Add(newUser);
        WriteToFile(ParkingUsers, _parkingUsersFile);
        return true;
    }
    public void RegisterCar(int userID, string licencePlate)
    {
        ParkingUser? user = ParkingUsers.Find(u => u.Id == userID);
        if (user == null)
        {
            throw new Exception("User does not exist");
        }
        if (user.Cars.Exists(c => c.LicencePlate.Equals(licencePlate)))
        {
            throw new Exception("Car already registered for this user.");
        }
        user.AddCar(licencePlate);
        WriteToFile(ParkingUsers, _parkingUsersFile);
    }
    public double CalculateFee(DateTime startTime, DateTime endTime)
    {
        double feeToPay = 0;
        TimeSpan timeSpan = endTime - startTime;
        // The parking time is all in the same day.
        if (startTime.Date == endTime.Date)
        {
            if (startTime.Hour >= StartTimeOfFullFee)
            {
                if (startTime.Hour < StartTimeOfReducedFee)
                {
                    if (endTime.Hour < StartTimeOfReducedFee) // Parking time is all within normal fee hours.
                    {
                        feeToPay = Fee * timeSpan.TotalHours;
                    }
                    else  // (endTime.Hour >= StartReducedFee) Start is during full fee, and end is after Reduced hours
                    {
                        feeToPay = FeeUntilBreakpoint(startTime, StartTimeOfReducedFee);
                        feeToPay += FeeFromBreakpoint(endTime, StartTimeOfReducedFee);
                    }
                } else // Start and end time is during reduced fee hours
                {
                    feeToPay = ReducedFee * timeSpan.TotalHours;
                }
            }
            else if (startTime.Hour < StartTimeOfFullFee)
            {
                if (endTime.Hour < StartTimeOfFullFee) // We start and end within reduced fee time
                {
                    feeToPay = ReducedFee * timeSpan.TotalHours;
                }
                else
                {
                    feeToPay = FeeUntilBreakpoint(startTime, StartTimeOfFullFee);
                    if (endTime.Hour >= StartTimeOfReducedFee)
                    { // add hours to reduced fee

                        feeToPay += MaxFullFare;
                        feeToPay += FeeFromBreakpoint(endTime, StartTimeOfReducedFee);
                    }
                    else
                    {
                        feeToPay += FeeFromBreakpoint(endTime, StartTimeOfFullFee);
                    }
                }
            }
        }
        else // End time is one or more days after Start time
        {
            double fullDayFee = (StartTimeOfFullFee * ReducedFee) + ((StartTimeOfReducedFee - StartTimeOfFullFee) * Fee) + ((24 - StartTimeOfReducedFee) * ReducedFee);

            feeToPay = FeeUntilMidnight(startTime) + FeeFromMidnight(endTime);
            if (timeSpan.Days > 1) feeToPay += (int)(timeSpan.Days - 1) * fullDayFee;
        }
        return feeToPay;
    }
    public static double FeeUntilBreakpoint(DateTime startDate, int breakpointHour)
    {
        double fee = 0;
        DateTime breakpoint = startDate.Date.AddHours(breakpointHour);
        Console.WriteLine($"Fee Until Breakpoint: Start time: {startDate} Breakpoint time: {breakpoint}");

        TimeSpan timeToCharge = breakpoint - startDate;
        if (breakpointHour == StartTimeOfFullFee)
        {
            fee += timeToCharge.TotalHours * ReducedFee;
        }
        else
        {
            fee += timeToCharge.TotalHours * Fee;
        }
        return fee;
    }
    public static double FeeFromBreakpoint(DateTime endDate, int breakpointHour)
    {
        double fee = 0;
        DateTime breakpoint = endDate.Date.AddHours(breakpointHour);
        TimeSpan timeToCharge = endDate - breakpoint;
        Console.WriteLine($"Fee From Breakpoint Endtime: {endDate} Breakpoint time: {breakpoint}");
        if (breakpointHour == StartTimeOfFullFee)
        {
            fee = timeToCharge.TotalHours * Fee;
        }
        else
        {
            fee = timeToCharge.TotalHours * ReducedFee;
        }

        return fee;
    }
    public static double FeeUntilMidnight(DateTime parkingDate)
    {
        double fee = 0;
        DateTime followingMidnight = parkingDate.AddDays(1).Date;
        if (parkingDate.Hour >= StartTimeOfReducedFee)
        {
            fee = ReducedFee * (followingMidnight - parkingDate).TotalHours;
        }
        else if (parkingDate.Hour < StartTimeOfFullFee)
        {
            fee = FeeUntilBreakpoint(parkingDate, StartTimeOfFullFee); //time before start of full fee
            fee += (StartTimeOfReducedFee - StartTimeOfFullFee) * Fee; // fee for the full Fee hours
            fee += (24 - StartTimeOfReducedFee) * ReducedFee; // fee for time after 18 until midnight

        }
        else if (parkingDate.Hour >= StartTimeOfFullFee)
        {
            fee = FeeUntilBreakpoint(parkingDate, StartTimeOfReducedFee); // fee for the time until the reduced 
            fee += (24 - StartTimeOfReducedFee) * ReducedFee; // fee for time after 18 until midnight

        }

        return fee;
    }
    public static double FeeFromMidnight(DateTime parkingDate)
    {
        double fee = 0;
        DateTime sameDayMidnight = parkingDate.Date;

        if (parkingDate.Hour < StartTimeOfFullFee)
        {
            fee = (parkingDate - sameDayMidnight).TotalHours * ReducedFee;
        }
        else if (parkingDate.Hour < StartTimeOfReducedFee)
        {
            fee = StartTimeOfFullFee * ReducedFee; // fee from midnight to start time of Full fee
            fee += FeeFromBreakpoint(parkingDate, StartTimeOfFullFee);
        }
        else
        {
            fee = StartTimeOfFullFee * ReducedFee; // fee from midnight to start time of Full fee
            fee += MaxFullFare; // fee for the full Fee hours
            fee += FeeFromBreakpoint(parkingDate, StartTimeOfReducedFee);
        }

        return fee;
    }

    public static void WriteToFile(List<ParkingUser> users, string filePath)
    {
        try
        {
            // Use JSON serialization to convert list to string
            var options = new JsonSerializerOptions { WriteIndented = true }; // Pretty print
            string jsonString = JsonSerializer.Serialize(users, options);

            // Write the JSON string to file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(jsonString);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to file: {ex.Message}");
        }
    }

    public static void WriteToFile(List<ParkingPeriod> activeParkingPeriods, string filePath)
    {
        try
        {
            // Use JSON serialization to convert list to string
            var options = new JsonSerializerOptions { WriteIndented = true }; // Pretty print
            string jsonString = JsonSerializer.Serialize(activeParkingPeriods, options);

            // Write the JSON string to file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(jsonString);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to file: {ex.Message}");
        }
    }
}