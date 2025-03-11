using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using API.Services;
using API.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace API.Controllers;
[ApiController]
public class ParkingServicesController : ControllerBase
{
    private ParkingServices _myParkingLot;
    public ParkingServicesController(ParkingServices myParkingLot)
    {
        _myParkingLot = myParkingLot;
    }
    [HttpPost("addUser")]
    public IActionResult AddUser([FromForm] string username, [FromForm] string password, [FromForm] string email, [FromForm] string? licensePlate)
    {

        if (username.Length < 1 || password.Length < 1 || email.Length < 1)
        {
            return BadRequest("Please fill in all required fields");
        }


        string carPlate = string.IsNullOrWhiteSpace(licensePlate) ? "" : licensePlate.ToUpper();
        _myParkingLot.RegisterUser(username, password, email, carPlate);

        string feedback = $"User {username} added to the system\n";
        feedback += _myParkingLot.Report();
        return Ok($"{feedback}");
    }

    [HttpPost("addCar")]
    public IActionResult AddCar(ParkingDTO newCar)
    {

        if (newCar.UserID <= 1 || newCar.LicensePlate.Length < 1)
        {
            return BadRequest("Please fill in all required fields");
        }

        

        string carPlate = carPlate = string.IsNullOrWhiteSpace(newCar.LicensePlate) ? "" : newCar.LicensePlate.ToUpper();
        try
        {
            ParkingUser? user = _myParkingLot.ParkingUsers.FirstOrDefault(u => u.Id == newCar.UserID);
            if (user == null) 
                return BadRequest("User does not exist");
            
            _myParkingLot.RegisterCar(newCar.UserID, carPlate);

            string feedback = $"User {user.UserName} added new car {carPlate} to the system\n";

            return Ok($"{feedback}");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("/")]
    public IActionResult ParkingLotReport()
    {
        var report = _myParkingLot.Report();
        return Ok(report);
    }

    [HttpGet("user/{userId}")]
    public ParkingUser? GetSingleUser(int userId)
    {
        ParkingUser? user = _myParkingLot.ParkingUsers.SingleOrDefault(u => u.Id == userId);
        return user;
    }

    [HttpGet("stopParking/{licensePlate}")]
    public IActionResult StopParking(string licensePlate)
    {
        string carToPark = licensePlate.ToUpper();
        ParkingPeriod? period = _myParkingLot.CurrentlyParked(carToPark);
        if (period == null) return BadRequest("Car not parked");
        int userID = period.UserID;
        _myParkingLot.StopParkingPeriod(carToPark);
        ParkingUser? user = _myParkingLot.ParkingUsers.FirstOrDefault(user => user.Id == userID);
        if (user == null) return BadRequest("User not recognized");
        return Ok(user.ParkingFeesOwed);
    }

    [HttpPost("startParking")]
    public IActionResult StartParking(ParkingDTO newParking)
    {

        string carToPark = newParking.LicensePlate.ToUpper();
        try
        {
            _myParkingLot.StartParkingPeriod(newParking.UserID, carToPark);
            return Ok($"Parking started for {carToPark} belonging to {_myParkingLot.ParkingUsers.FirstOrDefault(user => user.Id == newParking.UserID).UserName}");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }


    }

}
