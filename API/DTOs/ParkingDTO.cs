namespace API.DTOs;

public class ParkingDTO
{
    public int UserID { get; set; }
    public string? UserName { get; set; }
    public required string RegPlate { get; set; }
    public string? Name { get; set; }
}
