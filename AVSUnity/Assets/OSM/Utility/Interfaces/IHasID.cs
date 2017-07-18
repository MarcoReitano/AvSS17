using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using UnityEngine;

public interface IHasID
{
    string GetID();
    //void SetID(string id);
}




public abstract class IDUtils
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static string GetUniqueID<T>(string prefix, string suffix, ICollection<T> idCollection, int maxCount) where T : IHasID
    {
        string result = string.Empty;

        prefix = prefix ?? string.Empty;
        suffix = suffix ?? string.Empty;
        
        int numberOfdigits = maxCount.NumberOfDigits();
        string numberOfDigitsString = string.Empty;
        for (int i = 0; i < numberOfdigits; i++)
            numberOfDigitsString += '0';

        int count = 0;
        bool unique = true;
        do
        {
            count++;
            result = prefix + count.ToString(numberOfDigitsString, CultureInfo.CreateSpecificCulture("en-US")) + suffix;

            unique = true;
            foreach (IHasID obj in idCollection)
            {
                if (obj.GetID() == result)
                {
                    unique = false;
                    continue;
                }
            }

            if (count >= maxCount)
            {
                Debug.Log("EmergencyExit");
                return result;
            }

        } while (!unique);

        return result;
    }
        
}