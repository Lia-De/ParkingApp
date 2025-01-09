using API.Models;
using System.Text.Json;

namespace API.Services;

public class PersistDataServices
{

    public static bool ReadFromFile(out List<ParkingUser>? users, string filePath)
    {

        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
            string jsonUsers = File.ReadAllText(filePath);
            users = JsonSerializer.Deserialize<List<ParkingUser>>(jsonUsers, options);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            users = null;
            return false;
        }
    }

    public static bool ReadFromFile(out List<ParkingPeriod>? periods, string filePath)
    {

        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
            string jsonPeriods = File.ReadAllText(filePath);
            periods = JsonSerializer.Deserialize<List<ParkingPeriod>>(jsonPeriods, options);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            periods = null;
            return false;
        }
    }



    public static void WriteToFile(List<ParkingUser> users, string filePath)
    {
        try
        {
            // Use JSON serialization to convert list to string
            var options = new JsonSerializerOptions { WriteIndented = true }; // Pretty print
            string jsonString = JsonSerializer.Serialize(users, options);

            // Write the JSON string to file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(jsonString);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to file: {ex.Message}");
        }
    }

    public static void WriteToFile(List<ParkingPeriod> activeParkingPeriods, string filePath)
    {
        try
        {
            // Use JSON serialization to convert list to string
            var options = new JsonSerializerOptions { WriteIndented = true }; // Pretty print
            string jsonString = JsonSerializer.Serialize(activeParkingPeriods, options);

            // Write the JSON string to file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(jsonString);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to file: {ex.Message}");
        }
    }

}
