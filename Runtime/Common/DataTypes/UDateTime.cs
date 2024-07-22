using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmberToolkit.Common.DataTypes
{
    public class UDateTime
    {
        [OdinSerialize][ValueDropdown("GetYears")]
        public int Year;
        [OdinSerialize]
        [ValueDropdown("GetMonths")]
        public int Month;
        [OdinSerialize]
        [ValueDropdown("GetDays")]
        public int Day;
        [OdinSerialize]
        [ValueDropdown("GetHours")]
        public int Hour;
        [OdinSerialize]
        [ValueDropdown("GetMinutes")]
        public int Minute;

        public UDateTime() {             
            Year = 1;
            Month = 1;
            Day = 1;
            Hour = 0;
            Minute = 0;
        }

        // Constructor to initialize the values
        [JsonConstructor]
        public UDateTime(int year, int month, int day, int hour, int minute)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
        }

        public UDateTime(DateTime dateTime)
        {
            Year = dateTime.Year;
            Month = dateTime.Month;
            Day = dateTime.Day;
            Hour = dateTime.Hour;
            Minute = dateTime.Minute;
        }

        #region Operator Overloads

        // Overload the == operator
        public static bool operator ==(UDateTime uDateTime, DateTime dateTime)
        {
            return uDateTime.DateTime == dateTime;
        }

        // Overload the != operator
        public static bool operator !=(UDateTime uDateTime, DateTime dateTime)
        {
            return !(uDateTime == dateTime);
        }

        // Overload the < operator
        public static bool operator <(UDateTime uDateTime, DateTime dateTime)
        {
            return uDateTime.DateTime < dateTime;
        }

        // Overload the > operator
        public static bool operator >(UDateTime uDateTime, DateTime dateTime)
        {
            return uDateTime.DateTime > dateTime;
        }

        // Overload the <= operator
        public static bool operator <=(UDateTime uDateTime, DateTime dateTime)
        {
            return uDateTime.DateTime <= dateTime;
        }

        // Overload the >= operator
        public static bool operator >=(UDateTime uDateTime, DateTime dateTime)
        {
            return uDateTime.DateTime >= dateTime;
        }

        // Remember to also override Equals and GetHashCode when overloading operators
        public override bool Equals(object obj)
        {
            if (obj is UDateTime uDateTime)
            {
                return this.DateTime == uDateTime.DateTime;
            }
            if (obj is DateTime dateTime)
            {
                return this.DateTime == dateTime;
            }
            return false;
        }

        #endregion

        public override int GetHashCode()
        {
            return this.DateTime.GetHashCode();
        }


        // Property to translate the UDateTime values into a DateTime
        [HideInEditorMode]
        public DateTime DateTime
        {
            get
            {
                // Construct a DateTime based on UDateTime values
                return new DateTime(Year, Month, Day, Hour, Minute, 0);
            }
        }

        public void SetTime(DateTime inputTime)
        {
            Year = inputTime.Year;
            Month = inputTime.Month;
            Day = inputTime.Day;
            Hour = inputTime.Hour;
            Minute = inputTime.Minute;
        }

        private static int[] GetYears()
        {
            int startYear = 2030;
            int endYear = 2077;
            int[] years = new int[endYear - startYear + 1];

            for (int i = 0; i < years.Length; i++)
            {
                years[i] = startYear + i;
            }

            return years;
        }
        private static int[] GetMonths()
        {
            int[] months = new int[12];
            for (int i = 0; i < 12; i++)
            {
                months[i] = i + 1;
            }
            return months;
        }

        private static int[] GetDays()
        {
            int[] days = new int[31];
            for (int i = 0; i < 31; i++)
            {
                days[i] = i + 1;
            }
            return days;
        }

        private static int[] GetHours()
        {
            int[] hours = new int[24];
            for (int i = 0; i < 24; i++)
            {
                hours[i] = i;
            }
            return hours;
        }

        private static int[] GetMinutes()
        {
            int[] minutes = new int[60];
            for (int i = 0; i < 60; i++)
            {
                minutes[i] = i;
            }
            return minutes;
        }
    }
}
