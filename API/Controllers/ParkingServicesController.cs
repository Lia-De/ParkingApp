using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using API.Services;
namespace API.Controllers;
[ApiController]
public class ParkingServicesController : Controller
{

    [HttpPost("addUser")]
    public IActionResult AddUser([FromForm] string username, [FromForm] string password, [FromForm] string email, [FromForm] string? licensePlate)
    {

        if (username.Length < 1 || password.Length < 1 || email.Length < 1)
        {
            return BadRequest("Please fill in all fields");
        }

        ParkingServices myParkingLot = new ParkingServices();

        string carPlate = string.IsNullOrWhiteSpace(licensePlate) ? "" : licensePlate.ToUpper();
        myParkingLot.RegisterUser(username, password, email, carPlate);

        string feedback = $"User {username} added to the system\n";
        feedback += myParkingLot.Report();
        return Ok($"{feedback}");
    }


}
