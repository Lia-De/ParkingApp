using API.Models;
using System.Text.Json;

namespace API.Services;

public class PersistDataServices
{
    private static readonly string _parkingUsersFile = "ParkingUsers.json";
    private static readonly string _activeParkingPeriodsFile = "ActiveParkingPeriods.json";
    public static bool ReadInPersistantData(out List<ParkingUser>? users)
    {

        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
            string jsonUsers = File.ReadAllText(_parkingUsersFile);
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

    public static bool ReadInPersistantData(out List<ParkingPeriod>? periods)
    {

        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
            string jsonPeriods = File.ReadAllText(_activeParkingPeriodsFile);
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



    public static void PersistData(List<ParkingUser> users)
    {
        try
        {
            // Use JSON serialization to convert list to string
            var options = new JsonSerializerOptions { WriteIndented = true }; // Pretty print
            string jsonString = JsonSerializer.Serialize(users, options);

            // Write the JSON string to file
            using (StreamWriter writer = new StreamWriter(_parkingUsersFile))
            {
                writer.Write(jsonString);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to file: {ex.Message}");
        }
    }

    public static void PersistData(List<ParkingPeriod> activeParkingPeriods)
    {
        try
        {
            // Use JSON serialization to convert list to string
            var options = new JsonSerializerOptions { WriteIndented = true }; // Pretty print
            string jsonString = JsonSerializer.Serialize(activeParkingPeriods, options);

            // Write the JSON string to file
            using (StreamWriter writer = new StreamWriter(_activeParkingPeriodsFile))
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
