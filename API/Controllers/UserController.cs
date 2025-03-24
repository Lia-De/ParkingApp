using API.DTOs;
using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Models;
using API.DTOs;
namespace API.Controllers;

public class UserController : ControllerBase
{
    private ParkingServices _myParkingLot;
    public UserController(ParkingServices myParkingLot)
    {
        _myParkingLot = myParkingLot;
    }

    [HttpPost("/user/login")]
    public IActionResult Login([FromBody] CreateUserDTO newUser)
    {

        if (newUser.UserName is null || newUser.UserName.Length < 1)
        {
            return BadRequest("BACKEND Please fill in all required fields");
        }
        ParkingUser? loggedInUser = _myParkingLot.ParkingUsers.FirstOrDefault(u => u.UserName == newUser.UserName);
        if (loggedInUser is null)
        {
            return BadRequest("User not found");
        }
        if (loggedInUser.Password != newUser.Password)
        {
            return BadRequest("Incorrect password");
        }
        return Ok(new ParkingDTO(){ UserID = loggedInUser.Id, UserName = loggedInUser.UserName, RegPlate="" });

    }
}
