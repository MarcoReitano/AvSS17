using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using ProtoBuf;
using UnityEngine;

[Serializable]
[ProtoContract]
public class TimeStamp : IComparer
{
    [SerializeField]
    [ProtoMember(1)]
    public static readonly string DateFormatString = "dd'.'MM'.'yyyy' - 'HH':'mm':'ss'.'fffffff";

    [SerializeField]
    [ProtoMember(2)]
    private static readonly CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("de-DE");

    [SerializeField]
    [ProtoMember(3)]
    public string timeStamp;

    #region Constructors
    public TimeStamp()
    {
        this.timeStamp = GetTimeStamp();
    }

    public TimeStamp(DateTime dateTime)
    {
        this.timeStamp = dateTime.ToString(DateFormatString, cultureInfo);
    }
    #endregion // Constructors

    #region Methods
    public static string GetTimeStamp()
    {
        return DateTime.Now.ToString(DateFormatString, cultureInfo);
    }

    public static DateTime ParseTimeStamp(string dateTimeString)
    {
        return DateTime.ParseExact(dateTimeString, DateFormatString, cultureInfo);
    }
    #endregion // Methods

    #region Cast Operators
    public static implicit operator TimeStamp(DateTime dateTime)  // explicit byte to digit conversion operator
    {
        return new TimeStamp(dateTime);
    }

    public static implicit operator DateTime(TimeStamp timeStamp)  // explicit byte to digit conversion operator
    {
        return ParseTimeStamp(timeStamp.timeStamp);
    }

    public static implicit operator string(TimeStamp timeStamp)  // explicit byte to digit conversion operator
    {
        return timeStamp.timeStamp;
    }
    #endregion //Cast Operators

    #region Duration Methods
    public static double DurationInMillis(TimeStamp t1, TimeStamp t2)
    {
        return DurationInMillis(t1.timeStamp, t2.timeStamp);
    }

    public static double DurationInMillis(string dateString1, string dateString2)
    {
        return Duration(dateString1, dateString2).TotalMilliseconds;
    }

    public static double DurationInSeconds(string dateString1, string dateString2)
    {
        return Duration(dateString1, dateString2).TotalSeconds;
    }

    public TimeSpan Duration(string dateString)
    {
        if (this.timeStamp == null)
        {
            this.timeStamp = GetTimeStamp();
        }
        return Duration(this, dateString);
    }

    public static TimeStamp Now()
    {
        return new TimeStamp(DateTime.Now);
    }

    public TimeSpan DurationSince()
    {
        return Duration(this.timeStamp, Now());
    }

    public static TimeSpan Duration(string dateString1, string dateString2)
    {
        DateTime date1 = ParseTimeStamp(dateString1);
        DateTime date2 = ParseTimeStamp(dateString2);
        if (date1 > date2)
            return date1 - date2;

        return date2 - date1;
    }
    #endregion // Duration Methods

    public override string ToString()
    {
        return this.timeStamp;
    }

    public int CompareTo(object obj)
    {
        if (obj == null)
            return 1;

        TimeStamp other = obj as TimeStamp;
        DateTime otherDateTime = other;
        if (this > otherDateTime)
            return 1;
        if (this < otherDateTime)
            return -1;
        else
            return 0;
    }

    public int Compare(object x, object obj)
    {
        if (obj == null || x == null)
            return 1;

        return ((DateTime)x).CompareTo((DateTime)obj);
    }
}
