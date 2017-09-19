using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// OSMTag.
/// </summary>
public class OSMTag
{    
    public string Key
    {
        get { return key; }
        set { key = value; }
    }
    public string Value
    {
        get { return this.value; }
        set { this.value = value; }
    }

    private string key;
    private string value;
}
