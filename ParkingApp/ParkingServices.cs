namespace ParkingApp;

/*The practical circumstances for our client – one parking area with an hourly
charge 14 SEK between 8 and 18 and 6 SEK the rest of the day.Users have
an account that collects the cost.At some point the user is charged for
their costs, but that is outside of the app at this stage.*/
public class ParkingServices
{
    private static readonly double Fee = 14;
    private static readonly double ReducedFee = 6;
    private static readonly int StartTimeOfFullFee = 8;
    private static readonly int StartTimeOfReducedFee = 18;
    private static double MaxFullFare = (StartTimeOfReducedFee - StartTimeOfFullFee) * Fee;

    public static double CalculateFee(DateTime startTime, DateTime endTime)
    {
        double feeToPay = 0;
        TimeSpan timeSpan = endTime - startTime;
        // The parking time is all in the same day.
        if (startTime.Date == endTime.Date)
        {
            if (startTime.Hour >= StartTimeOfFullFee)
            {
                if (endTime.Hour < StartTimeOfReducedFee) // Parking time is all within normal fee hours.
                {
                    feeToPay = Fee * timeSpan.TotalHours;
                }
                else  // (endTime.Hour >= StartReducedFee) Start is during full fee, and end is after Reduced hours
                {
                    feeToPay = FeeUntilBreakpoint(startTime, StartTimeOfReducedFee);
                    feeToPay += FeeFromBreakpoint(endTime, StartTimeOfReducedFee);
                }
            }
            else if (startTime.Hour < StartTimeOfFullFee)
            {
                if (endTime.Hour < StartTimeOfFullFee) // We start and end within reduced fee time
                {
                    feeToPay = ReducedFee * timeSpan.TotalHours;
                }
                else
                {
                    feeToPay = FeeUntilBreakpoint(startTime, StartTimeOfFullFee);
                    if (endTime.Hour >= StartTimeOfReducedFee)
                    { // add hours to reduced fee

                        feeToPay += MaxFullFare;

                        feeToPay += FeeFromBreakpoint(endTime, StartTimeOfReducedFee);

                    }
                    else
                    {
                        feeToPay +=  FeeFromBreakpoint(endTime, StartTimeOfFullFee);
                    }
                }
            }
        }
        else // End time is one or more days after Start time
        {
            double fullDayFee = (StartTimeOfFullFee * ReducedFee) + ((StartTimeOfReducedFee - StartTimeOfFullFee) * Fee) + ((24 - StartTimeOfReducedFee) * ReducedFee);

            feeToPay = FeeUntilMidnight(startTime) + FeeFromMidnight(endTime);
            if (timeSpan.Days > 1) feeToPay += (int)(timeSpan.Days - 1) * fullDayFee;
        }
        return feeToPay;
    }
    public static double FeeUntilBreakpoint(DateTime startDate, int breakpointHour)
    {
        double fee = 0;
        DateTime breakpoint = startDate.Date.AddHours(breakpointHour);
        
        TimeSpan timeToCharge = breakpoint - startDate;
        if (breakpointHour == StartTimeOfFullFee)
        {
            fee += timeToCharge.TotalHours * ReducedFee;
        } else
        {
            fee += timeToCharge.TotalHours * Fee;
        }
        return fee;
    }
    public static double FeeFromBreakpoint (DateTime endDate, int breakpointHour)
    {
        double fee = 0;
        DateTime breakpoint = endDate.Date.AddHours(breakpointHour);
        TimeSpan timeToCharge = endDate - breakpoint;
        if (breakpointHour == StartTimeOfFullFee)
        {
            fee = timeToCharge.TotalHours * Fee;
        } else
        {
            fee = timeToCharge.TotalHours * ReducedFee;
        }

        return fee;
    }
    public static double FeeUntilMidnight(DateTime parkingDate)
    {
        double fee = 0;
        DateTime followingMidnight = parkingDate.AddDays(1).Date;
        if (parkingDate.Hour >=StartTimeOfReducedFee)
        {
            fee = ReducedFee * (followingMidnight - parkingDate).TotalHours;
        }
        else if (parkingDate.Hour < StartTimeOfFullFee)
        {
            fee = FeeUntilBreakpoint(parkingDate, StartTimeOfFullFee); //time before start of full fee
            fee += (StartTimeOfReducedFee - StartTimeOfFullFee)* Fee; // fee for the full Fee hours
            fee += (24 - StartTimeOfReducedFee) * ReducedFee; // fee for time after 18 until midnight

        }
        else if (parkingDate.Hour >= StartTimeOfFullFee)
        {
            fee = FeeUntilBreakpoint(parkingDate, StartTimeOfReducedFee); // fee for the time until the reduced 
            fee += (24 - StartTimeOfReducedFee) * ReducedFee; // fee for time after 18 until midnight

        }

        return fee;
    }
    public static double FeeFromMidnight(DateTime parkingDate)
    {
        double fee = 0;
        DateTime sameDayMidnight = parkingDate.Date;
        
        if (parkingDate.Hour < StartTimeOfFullFee)
        {
            fee = (parkingDate - sameDayMidnight).TotalHours * ReducedFee;
        }
        else if (parkingDate.Hour < StartTimeOfReducedFee)
        {
            fee = StartTimeOfFullFee * ReducedFee; // fee from midnight to start time of Full fee
            fee += FeeFromBreakpoint(parkingDate, StartTimeOfFullFee);
        }
        else
        {
            fee = StartTimeOfFullFee * ReducedFee; // fee from midnight to start time of Full fee
            fee += MaxFullFare; // fee for the full Fee hours
            fee += FeeFromBreakpoint(parkingDate, StartTimeOfReducedFee);
        }

        return fee;
    }
}
