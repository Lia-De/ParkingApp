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


app.Run();
