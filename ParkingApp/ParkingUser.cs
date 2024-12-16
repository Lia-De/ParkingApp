using System.ComponentModel.DataAnnotations;

namespace ParkingApp;
public class ParkingUser
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public List<Car> Cars { get; set; }
    public List<ParkingPeriod> ParkingPeriods { get; set; }
    public double ParkingFeesOwed { get; set; }

    public ParkingUser(string userName, string password, string email)
    {
        UserName = userName;
        Password = password;
        Email = email;
        Cars = new List<Car>();
        ParkingPeriods = new List<ParkingPeriod>();
    }
    public int AddCar(string licenceNumber)
    {
        Car newCar = new Car(licenceNumber);
        Cars.Add(newCar);
        return Cars.Count();
    }
}
