
using Newtonsoft.Json;
using System;

namespace RealEstateApp.Services.Logging
{
    public class DebugLogger : ILogger
    {
        public void Debug(object value)
        {
            string output = null;
            if (value is string)
            {
                output = (string)value;
            }
            else if (value is Exception)
            {
                output = value.ToString();
            }
            else
            {
                output = JsonConvert.SerializeObject(value);
            }

            System.Diagnostics.Debug.WriteLine(output);
        }
    }
}
