using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using API.Services;
using API.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using API.DTOs;
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
            frontendUser.Cars = user.Cars.OrderBy(c=> c.RegPlate).ToList();
            frontendUser.ParkingHistory = user.ParkingHistory.OrderBy(hist => hist.ParkedCar.RegPlate).ThenByDescending(hist=> hist.EndTime).ToList();
            frontendUser.isParked = _myParkingLot.UserHasParkedCars(user.Id);
        }
        return frontendUser;
    }


    [HttpPost("addUser")]
    public IActionResult AddUser(CreateUserDTO newUser)
    {

        if (newUser.UserName is null || newUser.UserName.Length < 1)
        {
            return BadRequest("Please fill in all required fields");
        }

        // Hardcoded password and email - does not pull from frontend yet. Fix probably with a new DTO class
        string carPlate = string.IsNullOrWhiteSpace(newUser.RegPlate) ? "" : newUser.RegPlate.ToUpper();
        bool result = _myParkingLot.RegisterUser(newUser.UserName, newUser.Password, newUser.Email, carPlate, newUser.Name);
        
        if (result)
        {
            var user = _myParkingLot.ParkingUsers.FirstOrDefault(user => user.Email == newUser.Email);
            ParkingDTO returnUser = new ParkingDTO()
            {
                UserID = user.Id,
                UserName = user.UserName,
                RegPlate = carPlate,
                Name = newUser.Name
            };
            return Ok(returnUser);
        }
        return BadRequest("Email is already registered");
    }

    [HttpPost("addCar")]
    public IActionResult AddCar(ParkingDTO newCar)
    {

           string carPlate = carPlate = string.IsNullOrWhiteSpace(newCar.RegPlate) ? "" : newCar.RegPlate.ToUpper();
        try
        {
            ParkingUser? user = _myParkingLot.ParkingUsers.FirstOrDefault(u => u.Id == newCar.UserID);
            if (user == null) return BadRequest("User does not exist");
            if (string.IsNullOrEmpty(newCar.Name))
            {
                _myParkingLot.RegisterCar(newCar.UserID, carPlate);
            }
            else
            {
                _myParkingLot.RegisterCar(newCar.UserID, carPlate, newCar.Name);
            }

            return Ok(user.Cars);
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
        double feeAdded = _myParkingLot.StopParkingPeriod(carToPark);
        ParkingUser? user = _myParkingLot.ParkingUsers.FirstOrDefault(user => user.Id == userID);
        if (user == null) return BadRequest("User not recognized");

        UserDTO userDTO = GetSingleUser(userID);
        StopParkingUserFeeDTO response = new StopParkingUserFeeDTO() { User=userDTO, Fee=feeAdded };
        
        return Ok(response);
    }

    [HttpPost("startParking")]
    public IActionResult StartParking(ParkingDTO newParking)
    {

        string carToPark = newParking.RegPlate.ToUpper();
        try
        {
            _myParkingLot.StartParkingPeriod(newParking.UserID, carToPark);
            UserDTO userDTO = GetSingleUser(newParking.UserID);
            return Ok(userDTO);
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

  
    [HttpGet("currentlyParked/{RegPlate}")]
    public IActionResult CurrentlyParked(string RegPlate)
    {
        string? parkedCar = RegPlate.ToUpper();
        if (parkedCar == null) 
            return BadRequest("Not a car");
        try
        {
            ParkingPeriod? period = _myParkingLot.CurrentlyParked(parkedCar);
            if (period != null)
            {
                double currentFee = PaymentService.CalculateFee(period.StartTime, DateTime.Now);
                //var responseData = new (){ period = period, fee = currentFee };
                return Ok(currentFee);
            }
            else
            {
                return NoContent();
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }



    [HttpGet("allFees")]
    public IActionResult AllFees()
    {
        var result = new List<UserDTO>();

        foreach (ParkingUser user in _myParkingLot.ParkingUsers)
        {
            UserDTO userDTO = new UserDTO();
            userDTO.Id = user.Id;
            userDTO.UserName = user.UserName;
            userDTO.ParkingFeesOwed = user.ParkingFeesOwed;
            result.Add(userDTO);

        }
        if (result.Count == 0)
            return NoContent();
        return Ok(result);
    }

}
