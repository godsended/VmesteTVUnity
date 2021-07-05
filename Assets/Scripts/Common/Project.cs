using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Project : MonoBehaviour
{
    public static string UserAgent = "VmesteTV Android Application 0.0.1";
    public static class URLs
    {
        public static string mainDomain = "https://posmotrim.tv";
        public static string loginRequest = "/api/m/login/";
        public static string registrationRequest = "/api/m/register/";
        public static string searchRequest = "/api/search/?text=";
        public static string searchPopularRequest = "/api/m/catalog/?pop=1";
        public static string searchNewRequest = "/api/m/catalog/?new=1";
        public static string genresRequest = "/api/m/genres";
        public static string filmInfoRequest = "/api/m/kp/";
        public static string getRoomInfoRequest = "/api/m/room/";
    }
}
public static class Device
{
    public static Vector2 GetUIScreenSize()
    {
        return new Vector2(1500, 1500 * Screen.height / Screen.width);
    }
}
