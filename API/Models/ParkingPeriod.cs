namespace API.Models;

public class ParkingPeriod
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsRunning { get; set; }
    public int UserID { get; set; }
    public Car ParkedCar { get; set; }
    public double ParkingFee { get; set; }

    public ParkingPeriod(DateTime startTime, int userID, Car parkedCar)
    {
        StartTime = startTime;
        UserID = userID;
        ParkedCar = parkedCar;
        IsRunning = true;
        ParkingFee = 0;
    }
    public void ParkingEnded(DateTime endTime, double feeToPay)
    {
        if (IsRunning)
        {
            EndTime = endTime;
            ParkingFee = feeToPay;
            IsRunning = false;
        }
        else
        {
            throw new Exception("The parking period was not running");
        }
    }

}