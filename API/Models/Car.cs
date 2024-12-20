using System.Security.Cryptography.X509Certificates;

namespace API.Models;

public class Car
{
    public string LicencePlate { get; set; } = string.Empty;
    public Car() { }
    public Car(string licenceNumber)
    {
        LicencePlate = licenceNumber;
    }
    public override string ToString()
    {
        return LicencePlate;
    }
}
