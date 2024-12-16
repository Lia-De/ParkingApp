using ParkingApp;
using System.Collections.Concurrent;
// TestingProgram();
 textTwo();
void textTwo()
{
    
    bool wrongContent = true;
    while (wrongContent)
    {
        try
        {
            Console.WriteLine("Type in how many hours to add from present: ");
            int hoursToAdd = int.Parse(Console.ReadLine());
            DateTime startTime = DateTime.Now.AddHours(hoursToAdd);
            double fee = ParkingServices.FeeUntilMidnight(startTime);
            double fee2 = ParkingServices.FeeFromMidnight(startTime);
            Console.WriteLine($"Time to start: {startTime} with a fee of \n{fee} until midnight and \n{fee2} from midnight. Total fee = \n{fee + fee2}");
            if (hoursToAdd == 0) wrongContent = false;
        }
        catch (Exception) { }
    }
}
void TestingProgram() {
    double fee;
    
    Console.WriteLine("Type in how many hours to subtract: ");

    int hoursToAdd = int.Parse(Console.ReadLine());
    DateTime startTime = DateTime.Now.AddHours(-hoursToAdd);


    bool wrongContent = true;
    while (wrongContent)
    {
        try
        {
            Console.WriteLine("Type in how many hours to add: ");
            int adding = int.Parse(Console.ReadLine());
            if (adding == 0) wrongContent = false;
            DateTime endTime = DateTime.Now.AddHours(adding);

            Console.WriteLine($"Start time is: {startTime} and endtime is {endTime}. " +
                $"It is a total of {(endTime - startTime).TotalHours}");

            fee = ParkingServices.CalculateFee(startTime, endTime);


            Console.WriteLine(fee);
        }
        catch (Exception)
        {
        }
    }
}