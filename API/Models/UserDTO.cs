namespace API.Models;

public class UserDTO
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<Car> Cars { get; set; }
    public List<ParkingPeriod> ParkingHistory { get; set; }
    public double ParkingFeesOwed { get; set; } = 0;
    public List<Car> isParked { get; set; }
}
