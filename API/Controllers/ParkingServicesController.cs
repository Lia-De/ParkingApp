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

    [HttpGet("/")]
    public IActionResult ParkingLotReport()
    {
        var report = _myParkingLot.Report();
        return Ok(report);
    }

    [HttpGet("user/{userId}")]
    public UserDTO? GetSingleUser(int userId)
    {
        ParkingUser? user = _myParkingLot.ParkingUsers.SingleOrDefault(u => u.Id == userId);
        UserDTO frontendUser= new UserDTO();
        if (user != null)
        {
            frontendUser.Id = user.Id;
            frontendUser.UserName = user.UserName;
            frontendUser.ParkingFeesOwed = user.ParkingFeesOwed;
            frontendUser.Cars = user.Cars;
            frontendUser.ParkingHistory = user.ParkingHistory;
        }
        return frontendUser;
    }


    [HttpPost("addUser")]
    public IActionResult AddUser(ParkingDTO newUser)
    {

        if (newUser.UserName is null || newUser.UserName.Length < 1)
        {
            return BadRequest("Please fill in all required fields");
        }


        string carPlate = string.IsNullOrWhiteSpace(newUser.LicensePlate) ? "" : newUser.LicensePlate.ToUpper();
        _myParkingLot.RegisterUser(newUser.UserName, "password", "email", carPlate);


        return Ok();
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
            ParkingUser user = _myParkingLot.ParkingUsers.FirstOrDefault(u => u.Id == newCar.UserID);
            _myParkingLot.RegisterCar(newCar.UserID, carPlate);

            string feedback = $"User {user.UserName} added new car {carPlate} to the system\n";

            return Ok($"{feedback}");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
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

    [HttpGet("currentlyOwing/{userID}")]
    public IActionResult CurrentlyOwing(int userID)
    {
        ParkingUser? user = _myParkingLot.ParkingUsers.FirstOrDefault(user => user.Id == userID);
        if (user == null) 
            return BadRequest("No such user");
        return Ok(user.ParkingFeesOwed);
    }

  
    [HttpGet("currentlyParked/{licencePlate}")]
    public IActionResult CurrentlyParked(string licencePlate)
    {
        string? parkedCar = licencePlate.ToUpper();
        if (parkedCar == null) 
            return BadRequest("Not a car");
        try
        {
            ParkingPeriod? period = _myParkingLot.CurrentlyParked(parkedCar);
            if (period != null)
            {
                double currentFee = PaymentService.CalculateFee(period.StartTime, DateTime.Now);
                return Ok(currentFee);
            }
            else
            {
                return BadRequest($"Car with Licence plate: {parkedCar} is not currently parked here.");
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


}
