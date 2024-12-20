using API.Services;

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
    myParkingLot.StartParkingPeriod(userID, carToPark);
    return $"Parking started for {carToPark} belonging to {myParkingLot.ParkingUsers.FirstOrDefault(user => user.Id == userID).UserName}";

});
app.MapGet("/endParking", (int userID, string licensePlate) =>
{
    string carToPark = licensePlate.ToUpper();
    ParkingServices myParkingLot = new ParkingServices();
    myParkingLot.StopParkingPeriod(userID, carToPark);
    return $"Parking stopped for {carToPark} belonging to {myParkingLot.ParkingUsers.FirstOrDefault(user => user.Id == userID)}";

});



app.MapGet("/", () => {
    ParkingServices myParkingLot = new ParkingServices();
    return myParkingLot.Report() + "\n" + myParkingLot.ReportUsers();
});

app.Run();
