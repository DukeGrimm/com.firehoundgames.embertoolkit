using System;
using System.Collections.Generic;
using System.Text;

namespace EmberToolkit.Common.DataTypes
{
    [Serializable] // Mark the class as serializable for Unity
    public class UDateTime
    {
        public int Year;
        public int Month;
        public int Day;
        public int Hour;
        public int Minute;

        // Constructor to initialize the values
        public UDateTime(int year, int month, int day, int hour, int minute)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
        }

        // Property to translate the UDateTime values into a DateTime
        public DateTime DateTime
        {
            get
            {
                // Construct a DateTime based on UDateTime values
                return new DateTime(Year, Month, Day, Hour, Minute, 0);
            }
        }
    }
}
