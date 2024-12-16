using System.Security.Cryptography.X509Certificates;

namespace ParkingApp;

public class Car
{
    public string Licence { get; set; } = string.Empty;
    
    public Car(string licenceNumber)
    {
        Licence = licenceNumber;
    }
}
