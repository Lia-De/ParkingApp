using System.Security.Cryptography.X509Certificates;

namespace API.Models;

public class Car
{
    public string RegPlate { get; set; } = string.Empty;
    public Car() { }
    public Car(string licenceNumber)
    {
        RegPlate = licenceNumber;
    }
    public override string ToString()
    {
        return RegPlate;
    }
}
