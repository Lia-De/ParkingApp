using API.Models;
using API.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ParkingServices>();

var sitePolicy = "site-policy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(sitePolicy, builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod()
               .SetIsOriginAllowed(origin => true);
    });
});


var app = builder.Build();

app.UseCors(sitePolicy);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//app.MapGet("/currentlyParked/{input}", (string input) => {
//    string? licencePlate = input.ToUpper();
//    if (licencePlate != null)
//    {
//        ParkingServices myParkingLot = new ParkingServices();

//        try
//        {
//            ParkingPeriod? period = myParkingLot.CurrentlyParked(licencePlate);
//            if (period != null)
//            {
//                double currentFee = myParkingLot.CalculateFee(period.StartTime, DateTime.Now);
//                return $"Car {licencePlate} is currently parked by {myParkingLot.ParkingUsers.FirstOrDefault(user => user.Id == period.UserID).UserName} since {period.StartTime}. Currently owing: {currentFee:F2} SEK.";
//            } else
//            {
//                return $"Car with Licence plate: {licencePlate} is not currently parked here.";
//            }
//        }
//        catch (Exception e)
//        {
//            return e.Message;
//        }
//    }
//    else
//    {
//        return "You must enter a licence plate";
//    }

//});

//app.MapGet("/allfees", () => {
//    ParkingServices myParkingLot = new ParkingServices();
//    string feedback = "";
//    if (myParkingLot.ParkingUsers.Count == 0) feedback += $"There are no users registered.";
//    foreach (ParkingUser user in myParkingLot.ParkingUsers)
//    {
//        feedback += $"User ({user.Id}) {user.UserName} currently owes {user.ParkingFeesOwed:F2} SEK\n";
//    }

//    return feedback;
//});

//app.MapGet("/currentlyOwing/{userID}", (int userID) => {
//    ParkingServices myParkingLot = new ParkingServices();
//    ParkingUser? user = myParkingLot.ParkingUsers.FirstOrDefault(user => user.Id == userID);
//    if (user == null) return $"No such user exists";
//    return $"User {user.UserName} currently owes {user.ParkingFeesOwed:F2} SEK";

//});

app.Run();
