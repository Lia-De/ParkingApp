using System.Text.Json;
using API.Models;
namespace API.Services;

public class ParkingServices
{
    public List<ParkingUser> ParkingUsers { get; set; }
    public List<ParkingPeriod> ActiveParkingPeriods { get; set; }
    public ParkingServices()
    {
        ParkingUsers = new List<ParkingUser>();
        ActiveParkingPeriods = new List<ParkingPeriod>();

        // read in the lists from file here
        
        PersistDataServices.ReadInPersistantData(out List<ParkingUser>? users);
        if (users != null)
            ParkingUsers.AddRange(users);
        
        PersistDataServices.ReadInPersistantData(out List<ParkingPeriod>? periods);
        if (periods != null)
            ActiveParkingPeriods.AddRange(periods);
    }

    // a method to report how many users and active parking periods are in the system.
    public string Report()
    {
        string? parkedCars = "";
        foreach (ParkingPeriod period in ActiveParkingPeriods)
        {
            parkedCars += " " +period.ParkedCar.RegPlate + ",";
        }
        string finish = parkedCars == "" ? "." : $" for the following cars:{parkedCars}";
        return $"There are {ParkingUsers.Count} users and {ActiveParkingPeriods.Count} active parking periods{finish.Trim(',')}";
    }
    public List<Car>? UserHasParkedCars(int UserId)
    {
        List<Car> parked = new List<Car>();
        foreach (ParkingPeriod period in ActiveParkingPeriods)
        {
            if (period.UserID == UserId)
                parked.Add(period.ParkedCar);
        }
        return parked;
    }
    public string ReportUsers()
    {
        string report = "Users:\n";
        foreach (ParkingUser user in ParkingUsers)
        {
            report += $"User: {user.UserName}, Email: {user.Email}, Cars: {user.Cars.Count}\n";
            foreach (Car car in user.Cars)
            {
                report += $"  - Licence Plate: {car.RegPlate}\n";
            }
        }
        return report;
    }
    public ParkingPeriod? CurrentlyParked(string RegPlate)
    {
        if (ActiveParkingPeriods.Count == 0)
        {
            return null;
        }
        if (RegPlate == null)
        {
            throw new Exception("You must enter a licence plate");
        }
        string parkedCar = RegPlate.ToUpper();
        ParkingPeriod? activePeriod = ActiveParkingPeriods.FirstOrDefault(p => p.ParkedCar.RegPlate.Equals(parkedCar));
 
        return activePeriod;
    }
    public void StartParkingPeriod(int userID, string carRegPlate)
    {
        if (CurrentlyParked(carRegPlate) != null)
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
            Car? parkedCar = user.Cars.Find(c => c.RegPlate.Equals(carRegPlate));
            if (parkedCar == null)
            {
                throw new Exception("Car not found for this user.");
            }
            else
            {
                ParkingPeriod newPeriod = new ParkingPeriod(startTime, userID, parkedCar);
                ActiveParkingPeriods.Add(newPeriod);
                PersistDataServices.PersistData(ActiveParkingPeriods);
            }
        }

    }
    public double StopParkingPeriod(string carRegPlate)
    {
        ParkingPeriod? parkingPeriod = null;
        DateTime stopTime = DateTime.Now;
        if (ActiveParkingPeriods.Count == 0)
        {
            throw new Exception("No active parking periods found.");
        }
        try
        {
            parkingPeriod = CurrentlyParked(carRegPlate);
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

            PersistDataServices.PersistData(ParkingUsers);
            PersistDataServices.PersistData(ActiveParkingPeriods);
            return feeToAdd;

        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
        return 0;
    }

    public bool RegisterUser(string username, string password, string email, string RegPlate, string? carName)
    {
        int highestID = ParkingUsers.Count == 0 ? 0 : ParkingUsers.Max(user => user.Id);

        if (ParkingUsers.Any(u=> u.Email == email)) return false;

        ParkingUser newUser = new ParkingUser(++highestID, username, password, email);
        if (RegPlate != "") newUser.AddCar(RegPlate, carName);
        ParkingUsers.Add(newUser);
        PersistDataServices.PersistData(ParkingUsers);
        return true;
    }
    public void RegisterCar(int userID, string RegPlate)
    {
        ParkingUser? user = ParkingUsers.Find(u => u.Id == userID);
        if (user == null)
        {
            throw new Exception("User does not exist");
        }
        if (user.Cars.Exists(c => c.RegPlate.Equals(RegPlate)))
        {
            throw new Exception("Car already registered for this user.");
        }
        user.AddCar(RegPlate);
        PersistDataServices.PersistData(ParkingUsers);
    }

    public void RegisterCar(int userID, string RegPlate, string carName)
    {
        ParkingUser? user = ParkingUsers.Find(u => u.Id == userID);
        if (user == null)
        {
            throw new Exception("User does not exist");
        }
        if (user.Cars.Exists(c => c.RegPlate.Equals(RegPlate)))
        {
            throw new Exception("Car already registered for this user.");
        }
        user.AddCar(RegPlate, carName);
        PersistDataServices.PersistData(ParkingUsers);
    }



}