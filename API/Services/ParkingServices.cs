using System.Text.Json;
using API.Models;
namespace API.Services;

public class ParkingServices
{
   
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
            parkedCars += " " +period.ParkedCar.LicencePlate + ",";
        }
        string finish = parkedCars == "" ? "." : $" for the following cars:{parkedCars}";
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
                PersistDataServices.WriteToFile(ActiveParkingPeriods, _activeParkingPeriodsFile);
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
            double feeToAdd = PaymentService.CalculateFee(startTime, stopTime);
            
            parkingPeriod.ParkingEnded(stopTime, feeToAdd);
            if (user == null) throw new Exception("No user found for this parking period.");
            user.LeaveParkingAndChargeAcconut(parkingPeriod);

            ActiveParkingPeriods.Remove(parkingPeriod);

            PersistDataServices.WriteToFile(ParkingUsers, _parkingUsersFile);
            PersistDataServices.WriteToFile(ActiveParkingPeriods, _activeParkingPeriodsFile);

        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }

    }

    public bool RegisterUser(string username, string password, string email, string licencePlate)
    {
        int highestID = ParkingUsers.Count == 0 ? 0 : ParkingUsers.Max(user => user.Id);

        ParkingUser newUser = new ParkingUser(++highestID, username, password, email);
        if (licencePlate != "") newUser.AddCar(licencePlate);
        ParkingUsers.Add(newUser);
        PersistDataServices.WriteToFile(ParkingUsers, _parkingUsersFile);
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
        PersistDataServices.WriteToFile(ParkingUsers, _parkingUsersFile);
    }

   
}