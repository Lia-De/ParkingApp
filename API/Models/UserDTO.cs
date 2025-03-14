namespace API.Models;

public class UserDTO
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<Car> Cars { get; set; }
    public List<ParkingPeriod> ParkingHistory { get; set; }
    public double ParkingFeesOwed { get; set; }
}
