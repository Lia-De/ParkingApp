namespace API.DTOs;

public class StopParkingUserFeeDTO
{
    public required UserDTO User { get; set; }
    public double Fee { get; set; }
}
