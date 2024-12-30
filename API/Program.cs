using API.Models;
using API.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Needs the method to be called with: GET /startParking?userID=1234&licensePlate=ABC123
app.MapGet("/startParking", (int userID, string licensePlate) =>
{
    string carToPark = licensePlate.ToUpper();
    ParkingServices myParkingLot = new ParkingServices();
    try
    {
        myParkingLot.StartParkingPeriod(userID, carToPark);
        return $"Parking started for {carToPark} belonging to {myParkingLot.ParkingUsers.FirstOrDefault(user => user.Id == userID).UserName}";
    }
    catch (Exception e)
    {
        return e.Message;
    }

});
app.MapGet("/endParking", (string licensePlate) =>
{
    string carToPark = licensePlate.ToUpper();
    ParkingServices myParkingLot = new ParkingServices();
    ParkingPeriod? period = myParkingLot.CurrentlyParked(carToPark);
    if (period == null) return $"Car {carToPark} is not currently parked";
    int userID = period.UserID;
    myParkingLot.StopParkingPeriod(carToPark);
    ParkingUser? user = myParkingLot.ParkingUsers.FirstOrDefault(user => user.Id == userID);
    return $"Parking stopped for {carToPark} belonging to {user.UserName}, they have a current debit: {user.ParkingFeesOwed:F2} SEK.";

});

app.MapGet("/user/{number}", (int number) => {
    ParkingServices myParkingLot = new ParkingServices();
    ParkingUser? user = null;
    user = myParkingLot.ParkingUsers.SingleOrDefault(u => u.Id == number);
    if (user == null) return $"No such user exists";
    string feedback = "";
    int carCount = user.Cars.Count;
    feedback += $"User {user.UserName} ({user.Email}) has {carCount} cars registered and currently owes {user.ParkingFeesOwed:F2} SEK\n";
    if (carCount > 0) {
        feedback += $"Cars belonging to {user.UserName} are: ";
        foreach (var car in user.Cars)
        {
            feedback += car.ToString() + ", ";
        }
    }
    return feedback.Trim(' ', ',');

});

app.MapGet("/currentlyParked/{input}", (string input) => {
    string? licencePlate = input.ToUpper();
    if (licencePlate != null)
    {
        ParkingServices myParkingLot = new ParkingServices();
        ParkingPeriod? period = myParkingLot.CurrentlyParked(licencePlate);
        if (period != null)
        {
            double currentFee = myParkingLot.CalculateFee(period.StartTime, DateTime.Now);
            return $"Car {licencePlate} is currently parked by {myParkingLot.ParkingUsers.FirstOrDefault(user => user.Id == period.UserID).UserName} since {period.StartTime}. Currently owing: {currentFee:F2} SEK.";
        }
        return $"Car {licencePlate} is not currently parked";
    }
    return "You must enter a licence plate";

});

app.MapGet("/allfees", () => {
    ParkingServices myParkingLot = new ParkingServices();
    string feedback = "";
    if (myParkingLot.ParkingUsers.Count == 0) feedback += $"There are no users registered.";
    foreach (ParkingUser user in myParkingLot.ParkingUsers)
    {
        feedback += $"User ({user.Id}) {user.UserName} currently owes {user.ParkingFeesOwed:F2} SEK\n";
    }

    return feedback;
});

app.MapGet("/currentlyOwing/{userID}", (int userID) => {
    ParkingServices myParkingLot = new ParkingServices();
    ParkingUser? user = myParkingLot.ParkingUsers.FirstOrDefault(user => user.Id == userID);
    if (user == null) return $"No such user exists";
    return $"User {user.UserName} currently owes {user.ParkingFeesOwed:F2} SEK";

});

app.MapGet("/", () => {
    ParkingServices myParkingLot = new ParkingServices();
    return myParkingLot.Report().Trim(',') + "\n" + myParkingLot.ReportUsers();
});

app.Run();
