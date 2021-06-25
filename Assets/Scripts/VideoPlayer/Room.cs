using System.Net;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace VmesteApp.Rooms
{
    public class Room
    {
        public static RoomInfo LastRoomInfo;
        public static async Task<RoomInfo> GetInfo(string roomID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Project.URLs.mainDomain + Project.URLs.getRoomInfoRequest + roomID + "?token=" + Auth.User.Instance.token);
            Debug.LogWarning(Project.URLs.mainDomain + Project.URLs.getRoomInfoRequest + roomID + "?token=" + Auth.User.Instance.token);
            request.UserAgent = Project.UserAgent;
            request.Method = "GET";

            WebResponse response = await request.GetResponseAsync();
            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Debug.LogWarning(responseString);

            RoomInfo ans = JsonUtility.FromJson<RoomInfo>(responseString);
            LastRoomInfo = ans;
            return ans;
        }
        [System.Serializable]
        public struct RoomInfo
        {
            public string status;
            public string roomurl;
            public string code;
            public VideoInfo mp4;
            public string server;
            [System.Serializable]
            public struct VideoInfo
            {
                public Video[] mp4urls;
                public string dt_update;
                public string is_active;
                [System.Serializable] 
                public struct Video
                {
                    public string url;
                    public string quality;
                }
            }
        }
    }
}
