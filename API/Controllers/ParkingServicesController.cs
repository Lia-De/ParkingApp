using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using API.Services;
using API.Models;
namespace API.Controllers;
[ApiController]
public class ParkingServicesController : Controller
{

    [HttpPost("addUser")]
    public IActionResult AddUser([FromForm] string username, [FromForm] string password, [FromForm] string email, [FromForm] string? licensePlate)
    {

        if (username.Length < 1 || password.Length < 1 || email.Length < 1)
        {
            return BadRequest("Please fill in all required fields");
        }

        ParkingServices myParkingLot = new ParkingServices();

        string carPlate = string.IsNullOrWhiteSpace(licensePlate) ? "" : licensePlate.ToUpper();
        myParkingLot.RegisterUser(username, password, email, carPlate);

        string feedback = $"User {username} added to the system\n";
        feedback += myParkingLot.Report();
        return Ok($"{feedback}");
    }

    [HttpPost("addCar")]
    public IActionResult AddCar([FromForm] int userID, [FromForm] string? licensePlate)
    {

        if (userID <= 1 || licensePlate.Length < 1)
        {
            return BadRequest("Please fill in all required fields");
        }

        ParkingServices myParkingLot = new ParkingServices();

        string carPlate = carPlate = string.IsNullOrWhiteSpace(licensePlate) ? "" : licensePlate.ToUpper();
        try
        {
            ParkingUser? user = myParkingLot.ParkingUsers.FirstOrDefault(u => u.Id == userID);
            myParkingLot.RegisterCar(userID, carPlate);

            string feedback = $"User {user.UserName} added new car {carPlate} to the system\n";

            return Ok($"{feedback}");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
