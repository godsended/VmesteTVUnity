using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public static string GetData(string name)
    {
        if (PlayerPrefs.HasKey(name))
            return PlayerPrefs.GetString(name);
        else
        {
            return "";
        }
    }
    public static void SaveData(string name, string value)
    {
        PlayerPrefs.SetString(name, value);
    }
}
