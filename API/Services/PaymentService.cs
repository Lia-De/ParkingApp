using API.Models;
namespace API.Services;

public class PaymentService
{
    private static readonly double _fee = 14;
    private static readonly double _reducedFee = 6;
    private static readonly int _startTimeOfFullFee = 8;
    private static readonly int _startTimeOfReducedFee = 18;
    private static readonly double _maxFullFare = (_startTimeOfReducedFee - _startTimeOfFullFee) * _fee;

    public static double CalculateFee(DateTime startTime, DateTime endTime)
    {
        double feeToPay = 0;
        TimeSpan timeSpan = endTime - startTime;
        // The parking time is all in the same day.
        if (startTime.Date == endTime.Date)
        {
            if (startTime.Hour >= _startTimeOfFullFee)
            {
                if (startTime.Hour < _startTimeOfReducedFee)
                {
                    if (endTime.Hour < _startTimeOfReducedFee) // Parking time is all within normal fee hours.
                    {
                        feeToPay = _fee * timeSpan.TotalHours;
                    }
                    else  // (endTime.Hour >= StartReducedFee) Start is during full fee, and end is after Reduced hours
                    {
                        feeToPay = FeeUntilBreakpoint(startTime, _startTimeOfReducedFee);
                        feeToPay += FeeFromBreakpoint(endTime, _startTimeOfReducedFee);
                    }
                }
                else // Start and end time is during reduced fee hours
                {
                    feeToPay = _reducedFee * timeSpan.TotalHours;
                }
            }
            else if (startTime.Hour < _startTimeOfFullFee)
            {
                if (endTime.Hour < _startTimeOfFullFee) // We start and end within reduced fee time
                {
                    feeToPay = _reducedFee * timeSpan.TotalHours;
                }
                else
                {
                    feeToPay = FeeUntilBreakpoint(startTime, _startTimeOfFullFee);
                    if (endTime.Hour >= _startTimeOfReducedFee)
                    { // add hours to reduced fee

                        feeToPay += _maxFullFare;
                        feeToPay += FeeFromBreakpoint(endTime, _startTimeOfReducedFee);
                    }
                    else
                    {
                        feeToPay += FeeFromBreakpoint(endTime, _startTimeOfFullFee);
                    }
                }
            }
        }
        else // End time is one or more days after Start time
        {
            double fullDayFee = (_startTimeOfFullFee * _reducedFee) + ((_startTimeOfReducedFee - _startTimeOfFullFee) * _fee) + ((24 - _startTimeOfReducedFee) * _reducedFee);

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
        if (breakpointHour == _startTimeOfFullFee)
        {
            fee += timeToCharge.TotalHours * _reducedFee;
        }
        else
        {
            fee += timeToCharge.TotalHours * _fee;
        }
        return fee;
    }
    public static double FeeFromBreakpoint(DateTime endDate, int breakpointHour)
    {
        double fee = 0;
        DateTime breakpoint = endDate.Date.AddHours(breakpointHour);
        TimeSpan timeToCharge = endDate - breakpoint;
        if (breakpointHour == _startTimeOfFullFee)
        {
            fee = timeToCharge.TotalHours * _fee;
        }
        else
        {
            fee = timeToCharge.TotalHours * _reducedFee;
        }

        return fee;
    }
    public static double FeeUntilMidnight(DateTime parkingDate)
    {
        double fee = 0;
        DateTime followingMidnight = parkingDate.AddDays(1).Date;
        if (parkingDate.Hour >= _startTimeOfReducedFee)
        {
            fee = _reducedFee * (followingMidnight - parkingDate).TotalHours;
        }
        else if (parkingDate.Hour < _startTimeOfFullFee)
        {
            fee = FeeUntilBreakpoint(parkingDate, _startTimeOfFullFee); //time before start of full fee
            fee += (_startTimeOfReducedFee - _startTimeOfFullFee) * _fee; // fee for the full Fee hours
            fee += (24 - _startTimeOfReducedFee) * _reducedFee; // fee for time after 18 until midnight

        }
        else if (parkingDate.Hour >= _startTimeOfFullFee)
        {
            fee = FeeUntilBreakpoint(parkingDate, _startTimeOfReducedFee); // fee for the time until the reduced 
            fee += (24 - _startTimeOfReducedFee) * _reducedFee; // fee for time after 18 until midnight

        }

        return fee;
    }
    public static double FeeFromMidnight(DateTime parkingDate)
    {
        double fee = 0;
        DateTime sameDayMidnight = parkingDate.Date;

        if (parkingDate.Hour < _startTimeOfFullFee)
        {
            fee = (parkingDate - sameDayMidnight).TotalHours * _reducedFee;
        }
        else if (parkingDate.Hour < _startTimeOfReducedFee)
        {
            fee = _startTimeOfFullFee * _reducedFee; // fee from midnight to start time of Full fee
            fee += FeeFromBreakpoint(parkingDate, _startTimeOfFullFee);
        }
        else
        {
            fee = _startTimeOfFullFee * _reducedFee; // fee from midnight to start time of Full fee
            fee += _maxFullFare; // fee for the full Fee hours
            fee += FeeFromBreakpoint(parkingDate, _startTimeOfReducedFee);
        }

        return fee;
    }


}
