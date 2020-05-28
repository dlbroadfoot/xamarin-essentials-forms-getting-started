using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstateApp.Services.Logging
{
    public interface ILogger
    {
        void Debug(object value);
    }
}
