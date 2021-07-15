using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

    public static byte[] StructureToByteArray(object obj)
    {
        int len = Marshal.SizeOf(obj);

        byte[] arr = new byte[len];

        System.IntPtr ptr = Marshal.AllocHGlobal(len);

        Marshal.StructureToPtr(obj, ptr, true);

        Marshal.Copy(ptr, arr, 0, len);

        Marshal.FreeHGlobal(ptr);

        return arr;
    }

    public static void ByteArrayToStructure(byte[] bytearray, ref object obj)
    {
        int len = Marshal.SizeOf(obj);

        System.IntPtr i = Marshal.AllocHGlobal(len);

        Marshal.Copy(bytearray, 0, i, len);

        obj = Marshal.PtrToStructure(i, obj.GetType());

        Marshal.FreeHGlobal(i);
    }
}
