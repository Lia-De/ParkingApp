using ParkingApp;
using System.Collections.Concurrent;

DemoParkingApp();

void DemoParkingApp()
{

    ParkingServices myParkingLot = new ParkingServices();
    //myParkingLot.RegisterUser("George", "password", "email", "PPP111");
    //myParkingLot.RegisterUser("John", "password", "email", "BBB981");
    //myParkingLot.RegisterUser("Paul", "password", "email", "AKL134");

    Console.WriteLine(myParkingLot.Report());
    Console.WriteLine(myParkingLot.ReportUsers());
    try
    {
        myParkingLot.StartParkingPeriod(1, "PPP111");
        myParkingLot.StartParkingPeriod(5, "PPP111");
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
    Console.WriteLine(myParkingLot.Report());

    Console.WriteLine("Enter to continue and stop parking ");

    Console.ReadLine();
    try
    {   
        myParkingLot.StopParkingPeriod("PPP111");
        myParkingLot.StopParkingPeriod("PPP111");
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }

    Console.WriteLine(myParkingLot.Report());


}