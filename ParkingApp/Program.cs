using ParkingApp;
using System.Collections.Concurrent;

DemoParkingApp();

void DemoParkingApp()
{

    ParkingServices myParkingLot = new ParkingServices();
    //myParkingLot.RegisterUser("Jonas", "password", "email", "KAL222");
    //myParkingLot.RegisterUser("John", "password", "email", "BBB981");
    //myParkingLot.RegisterUser("Paul", "password", "email", "AKL134");

    Console.WriteLine(myParkingLot.Report());
    //Console.WriteLine(myParkingLot.ReportUsers());
    
    string input = "";
    bool wrongContent = true;

    while (wrongContent)
    {

        Console.WriteLine("type in the hour");
        input = Console.ReadLine();
        if (input == "0")
        {
            wrongContent = false;
        }
        else
        {
            //DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(input), 0, 0);
            //Console.WriteLine("Type in en end-time hour");
            //string input2 = Console.ReadLine();
            //DateTime stop = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(input2), 30, 0);

            //double fee = myParkingLot.CalculateFee(start, stop);
            //Console.WriteLine($"The fee was: {fee:F2}");

        }
    }
    //while (wrongContent)
    //{
    //    Console.WriteLine("Type 1 to start parking for user (1, PPP111)");
    //    Console.WriteLine("Type 2 to stop parking for user (PPP111)");

    //    input = Console.ReadLine();
    //    if (input == "1")
    //    {
    //        myParkingLot.StartParkingPeriod(1, "PPP111");

    //    }
    //    else if (input == "2")
    //    {
    //        myParkingLot.StopParkingPeriod("PPP111");
    //        Console.WriteLine(myParkingLot.Report());
    //    }
    //    else if (input  =="0")
    //    {
    //        wrongContent = false;
    //    }
    //    else
    //    {
    //        Console.WriteLine("Wrong input, try again");
    //    }

    //}

    //try
    //{
    //    myParkingLot.StartParkingPeriod(1, "PPP111");
    //}
    //catch (Exception e)
    //{
    //    Console.WriteLine(e.Message);
    //}
    //Console.WriteLine(myParkingLot.Report());

    //Console.WriteLine("Enter to continue and stop parking ");

    //Console.ReadLine();
    //try
    //{   
    //    myParkingLot.StopParkingPeriod("PPP111");
    //    myParkingLot.StopParkingPeriod("PPP111");
    //}
    //catch (Exception e)
    //{
    //    Console.WriteLine(e.Message);
    //}

    //Console.WriteLine(myParkingLot.Report());


}