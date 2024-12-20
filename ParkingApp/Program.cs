using ParkingApp;
using System.Collections.Concurrent;
// TestingProgram();
// textTwo();

InitializeJson();

void InitializeJson()
{
    //string _parkingUsersFile = "ParkingUsers.json";
    //string _activeParkingPeriodsFile = "ActiveParkingPeriods.json";

    //ParkingUser newUser = new ParkingUser(1, "Jenny", "password", "email");
    //newUser.AddCar("123ABD");
    //newUser.AddCar("MAC123");

    //List<ParkingUser> users = new List<ParkingUser>();

    //users.Add(newUser);

    //List<ParkingPeriod> periods = new List<ParkingPeriod>();
    //// Serialize and write the list to the file
    //ParkingServices.WriteToFile(users, _parkingUsersFile);
    //ParkingServices.WriteToFile(periods, _activeParkingPeriodsFile);

    //Console.WriteLine("Data written to file successfully.");

    ParkingServices myParkingLot = new ParkingServices();
    //myParkingLot.RegisterUser("George", "password", "email", "PPP111");
    //myParkingLot.RegisterUser("John", "password", "email", "");
    //myParkingLot.RegisterUser("Paul", "password", "email", "");

    Console.WriteLine($"I currently keep track of {myParkingLot.ParkingUsers.Count} users and {myParkingLot.ActiveParkingPeriods.Count} parking periods");

    List<ParkingUser> parkingUsers = myParkingLot.ParkingUsers;

    foreach ( var user in parkingUsers)
    {
        string cars = "";
        foreach (var car in user.Cars)
        {
            cars += car.ToString() + ", ";
        }
        Console.WriteLine($"{user.UserName} has the following cars: {cars}");
    }

    //myParkingLot.StartParkingPeriod(1, "MAC123");
    myParkingLot.StartParkingPeriod(1, "PPP111");
    Console.WriteLine($"I currently keep track of {myParkingLot.ParkingUsers.Count} users and {myParkingLot.ActiveParkingPeriods.Count} parking periods");

    Console.WriteLine("Enter to continue and stop parking ");

    Console.ReadLine();

    myParkingLot.StopParkingPeriod(1, "MAC123");
    myParkingLot.StopParkingPeriod(1, "PPP111");

    Console.WriteLine($"I currently keep track of {myParkingLot.ParkingUsers.Count} users and {myParkingLot.ActiveParkingPeriods.Count} parking periods");
    

}
//void textTwo()
//{

//    bool wrongContent = true;
//    while (wrongContent)
//    {
//        try
//        {
//            Console.WriteLine("Type in how many hours to add from present: ");
//            int hoursToAdd = int.Parse(Console.ReadLine());
//            DateTime startTime = DateTime.Now.AddHours(hoursToAdd);
//            double fee = ParkingServices.FeeUntilMidnight(startTime);
//            double fee2 = ParkingServices.FeeFromMidnight(startTime);
//            Console.WriteLine($"Time to start: {startTime} with a fee of \n{fee:F2} until midnight and \n{fee2:F2} from midnight. Total fee = \n{(fee + fee2):F2}");
//            if (hoursToAdd == 0) wrongContent = false;
//        }
//        catch (Exception) { }
//    }
//}
//void TestingProgram() {
//    double fee;

//    Console.WriteLine("Type in how many hours to subtract: ");

//    int hoursToAdd = int.Parse(Console.ReadLine());
//    DateTime startTime = DateTime.Now.AddHours(-hoursToAdd);


//    bool wrongContent = true;
//    while (wrongContent)
//    {
//        try
//        {
//            Console.WriteLine("Type in how many hours to add: ");
//            int adding = int.Parse(Console.ReadLine());
//            if (adding == 0) wrongContent = false;
//            DateTime endTime = DateTime.Now.AddHours(adding);

//            Console.WriteLine($"Start time is: {startTime} and endtime is {endTime}. " +
//                $"It is a total of {(endTime - startTime).TotalHours}:F2");

//            fee = new ParkingServices().CalculateFee(startTime, endTime);


//            Console.WriteLine($"{fee:F2}");
//        }
//        catch (Exception)
//        {
//        }
//    }
//}