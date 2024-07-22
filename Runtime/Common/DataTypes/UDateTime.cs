﻿using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmberToolkit.Common.DataTypes
{
    public class UDateTime
    {
        [OdinSerialize]
        public int Year;
        [OdinSerialize]
        public int Month;
        [OdinSerialize]
        public int Day;
        [OdinSerialize]
        public int Hour;
        [OdinSerialize]
        public int Minute;

        public UDateTime() {             
            Year = 0;
            Month = 0;
            Day = 0;
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
    }
}
