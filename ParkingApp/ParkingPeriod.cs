namespace ParkingApp;

public class ParkingPeriod
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsRunning { get; set; }
    public ParkingUser User { get; set; }
    public Car ParkedCar { get; set; }

    public ParkingPeriod() { }
    public ParkingPeriod(DateTime startTime, ParkingUser user, Car parkedCar)
    {
        StartTime = startTime;
        User = user;
        ParkedCar = parkedCar;
        IsRunning = true;
    }

}