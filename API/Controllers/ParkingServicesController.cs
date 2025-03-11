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
    public IActionResult AddCar([FromForm] int userID, [FromForm] string? licensePlate)
    {

        if (userID <= 1 || licensePlate.Length < 1)
        {
            return BadRequest("Please fill in all required fields");
        }

        

        string carPlate = carPlate = string.IsNullOrWhiteSpace(licensePlate) ? "" : licensePlate.ToUpper();
        try
        {
            ParkingUser user = _myParkingLot.ParkingUsers.FirstOrDefault(u => u.Id == userID);
            _myParkingLot.RegisterCar(userID, carPlate);

            string feedback = $"User {user.UserName} added new car {carPlate} to the system\n";

            return Ok($"{feedback}");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("user/{userId}")]
    public ParkingUser? GetSingleUser(int userId)
    {
        ParkingUser? user = _myParkingLot.ParkingUsers.SingleOrDefault(u => u.Id == userId);
        return user;
        //    if (user == null) return $"No such user exists";
        //    string feedback = "";
        //    int carCount = user.Cars.Count;
        //    feedback += $"User {user.UserName} ({user.Email}) has {carCount} cars registered and currently owes {user.ParkingFeesOwed:F2} SEK\n";
        //    if (carCount > 0) {
        //        feedback += $"Cars belonging to {user.UserName} are: ";
        //        foreach (var car in user.Cars)
        //        {
        //            feedback += car.ToString() + ", ";
        //        }
        //    }
        //    return feedback.Trim(' ', ',');

    }

}
