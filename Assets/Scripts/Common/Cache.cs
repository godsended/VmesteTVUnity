using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Cache
{
    public static void SaveImage(string name, byte[] bytes)
    {
        File.WriteAllBytes(URL.Image + name, bytes);
    }
    public static byte[] LoadImage(string name)
    {
        if (IsImageSaved(name))
            return File.ReadAllBytes(URL.Image + name);
        else return null;
    }
    public static bool IsImageSaved(string name)
    {
        return File.Exists(URL.Image + name);
    }
    private static class URL
    {
        public static string Image = Application.persistentDataPath;
    }
}
