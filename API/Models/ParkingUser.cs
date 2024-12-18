using System.ComponentModel.DataAnnotations;

namespace API.Models;
public class ParkingUser
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public List<Car> Cars { get; set; }
    public List<ParkingPeriod> ParkingHistory { get; set; }
    public double ParkingFeesOwed { get; set; }

    public ParkingUser(int id, string userName, string password, string email)
    {
        Id = id;
        UserName = userName;
        Password = password;
        Email = email;
        Cars = new List<Car>();
        ParkingHistory = new List<ParkingPeriod>();
        ParkingFeesOwed = 0;
    }
    public int AddCar(string licenceNumber)
    {
        Car newCar = new Car(licenceNumber);
        if (!Cars.Contains(newCar))
        {
            Cars.Add(newCar);
        }
        return Cars.Count();
    }
    public double LeaveParkingAndChargeAcconut(ParkingPeriod parkingPeriod)
    {
        if (parkingPeriod.IsRunning)
        {
            throw new Exception("Cannot add a running parking period to history.");
        }

        ParkingHistory.Add(parkingPeriod);
        ParkingFeesOwed += parkingPeriod.ParkingFee;
        return ParkingFeesOwed;
    }
}
