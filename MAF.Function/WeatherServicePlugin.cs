using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MAF.Function;

internal class WeatherServicePlugin
{
    [Description("Get current weather for a location")]
    public static async Task<string> GetCurrentWeatherAsync(string location)
    {
        return $"The weather is {location} is cloudy with a hight of 15℃";
    }
}
