﻿namespace API.DTOs;

public class ParkingDTO
{
    public int UserID { get; set; }
    public string? UserName { get; set; }
    public required string LicensePlate { get; set; }
}
