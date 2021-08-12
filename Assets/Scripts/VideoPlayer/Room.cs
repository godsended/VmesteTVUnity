using System;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.WebSocket;

namespace VmesteApp.Rooms
{
    public class Room
    {
        private static Thread jwsThread;
        private static List<string> jwsMessages;

        private static bool isJoined, isPWSInit;
        public static WebSocket jws { get; private set; } //join web socket, commands web socket and progress web socket

        public static OnWebSocketMessageDelegate onWebSocketMessage;
        public static RoomInfo LastRoomInfo;
        public static float progress;
        public static bool isPause;
        public static async Task<RoomInfo> GetInfo(string filmID)
        {
            LastRoomInfo = new RoomInfo();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Project.URLs.mainDomain + Project.URLs.getRoomInfoRequest + filmID + "?token=" + Auth.User.Instance.token);
            request.UserAgent = Project.UserAgent;
            request.Method = "GET";

            WebResponse response = await request.GetResponseAsync();
            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Debug.Log(responseString);

            RoomInfo ans = JsonUtility.FromJson<RoomInfo>(responseString);
            LastRoomInfo = ans;
            return ans;
        }
        public static async Task<RoomInfo> GetInfo(string token, string roomID)
        {
            LastRoomInfo = new RoomInfo();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Project.URLs.mainDomain + Project.URLs.getRoomInfoRequest + "?token=" + Auth.User.Instance.token + "&code=" + roomID);
            request.UserAgent = Project.UserAgent;
            request.Method = "GET";

            try
            {
                WebResponse response = await request.GetResponseAsync();
                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                Debug.Log(responseString);

                RoomInfo ans = JsonUtility.FromJson<RoomInfo>(responseString);
                LastRoomInfo = ans;
                return ans;
            }
            catch
            {
                return new RoomInfo();
            }
        }
        #region JoinServer
        public static async Task<bool> JoinServer(bool isAdmin)
        {
            if (jwsThread != null)
                jwsThread.Abort();

            isJoined = false;
            progress = 0;
            isPause = false;
            isPWSInit = false;
            jwsMessages = new List<string>();

            //Thread.CurrentThread.Name = "main";

            jwsThread = new Thread(Thread_InitJWS);
            jwsThread.Start();

            while (!isJoined)
                await Task.Yield();

            return true;
        }
        private static void ApplyUUID(WebSocket w, string message)
        {
            //Debug.Log("JWS msg" + message);
            ServerJoinStatus sjs = JsonUtility.FromJson<ServerJoinStatus>(message);
            LastRoomInfo.uuid = sjs.join;
            isJoined = true;
            jws.OnMessage -= ApplyUUID;
        }
        #endregion
        public static async Task<RoomStatus> SendActionCommand(string command)
        {
            RoomStatus rs = new RoomStatus();
            Command com = new Command();
            com.command = command;
            com.room = LastRoomInfo.uuid;
            com.progress = progress;
            com.is_pause = isPause;
            com.uuid = LastRoomInfo.user.uuid;

            string message = JsonUtility.ToJson(com);

            bool ansRecived = false;

            if (!isPWSInit)
            {
                //jws.OnMessage += (sender, e) =>
                //{
                //    Debug.Log("PWS msg: " + e.Data);
                //};
                isPWSInit = true;
            }
            jws.OnMessage += (sender, msg) =>
            {
                rs = JsonUtility.FromJson<RoomStatus>(msg);
                ansRecived = true;
            };
            //jwsThread.Start(message);
            jwsMessages.Add(message);
            //Debug.Log("Awaiting for socket");

            while (!ansRecived && jws.IsOpen)
            {
                await Task.Yield();
            }

            //Debug.Log("Action command answer delivered");

            return rs;
        }
        public static void SendChatMessage(string chatMsg)
        {
            ChatCommand com = new ChatCommand();
            com.command = "send";
            com.room = LastRoomInfo.uuid;
            com.message = chatMsg;
            com.avatar = LastRoomInfo.user.avatar_url;
            com.profile_id = LastRoomInfo.user.profileid;
            com.color = LastRoomInfo.user.usernamecolor;
            com.author_uuid = LastRoomInfo.user.uuid;

            string message = JsonUtility.ToJson(com);

            jwsMessages.Add(message);
        }
        private static void Thread_InitJWS()
        {
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Name = "BG";

            ServerJoinCommand sjc = new ServerJoinCommand();
            sjc.command = "join";
            sjc.user_name = LastRoomInfo.user.user_name;
            sjc.avatar_url = LastRoomInfo.user.avatar_url;
            Debug.Log(sjc.avatar_url);
            
            sjc.room = LastRoomInfo.room;
            sjc.profile_id = LastRoomInfo.user.profileid;
            sjc.username_color = LastRoomInfo.user.usernamecolor;
            sjc.uuid = LastRoomInfo.user.uuid;
            sjc.is_author = true;

            string message = JsonUtility.ToJson(sjc);

            string host = "wss://" + LastRoomInfo.server + "/chat/stream/";

            jws = new WebSocket(new Uri(host));

            onWebSocketMessage = jws.OnMessage;

            jws.OnOpen += (sender) =>
            {
                jwsMessages.Add(message);
                //Debug.Log("MessageSended");
            };
            jws.OnMessage += ApplyUUID;
            //jws.OnMessage += (sender, msg) => Debug.Log(msg);
            jws.OnError += (sender, error) =>
            {
                Debug.Log("Error " + error);
                jws.Close();
            };
            jws.OnClosed += (sender, code, msg) =>
            {
                Debug.Log("JWS Closed with code " + code +"\n" + msg);
                jws = null;
            };

            jws.Open();

            while (true)
            {
                Thread.Sleep(200);
                if (jwsMessages.Count > 0)
                {
                    while(jwsMessages[0] == null)
                    {

                    }
                    jws.Send(jwsMessages[0]);
                    jwsMessages.RemoveAt(0);
                }
            }
        }

        public static void AbortJWSThred()
        {
            jwsThread.Abort();
        }
        [System.Serializable]
        public struct RoomInfo
        {
            public string status;
            public string roomurl;
            public string code;
            public string room;
            public string uuid;
            public VideoInfo mp4;
            public string server;
            public UserInfo user;
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
        [System.Serializable]
        public struct Command
        {
            public string command;
            public string room;
            public float progress;
            public bool is_pause;
            public string uuid;
        }
        [System.Serializable]
        public struct ChatCommand
        {
            public string command;
            public string room;
            public string message;
            public string avatar;
            public int profile_id;
            public string color;
            public string author_uuid;
            public string uuid;
        }
        [System.Serializable]
        public struct ServerJoinCommand
        {
            public string command;
            public string user_name;
            public string avatar_url;
            public string room;
            public int profile_id;
            public string username_color;
            public string uuid;
            public bool is_author;
        }
        [System.Serializable]
        public struct ServerJoinStatus
        {
            public string join;
        }
        [System.Serializable]
        public struct UserInfo
        {
            public string user_name;
            public string avatar_url;
            public int profileid;
            public string usernamecolor;
            public string uuid;
        }
        [System.Serializable]
        public struct RoomStatus
        {
            public int msg_type;
            public string operation;
            public bool is_pause;
            public float progress;
            public int dt;
            public string user_count;
            public string avatar;
            public string username;
            public string message;
            public User[] users;
            [System.Serializable]
            public struct User
            {
                public string name;
                public string avatar;
                public float progress;
                public string peer_id;
                public int profile_id;
                public bool is_author;
                public bool is_banned;
                public string ua;
                public string uuid;
                public bool is_vip;
                public string username_color;
                public bool is_pause;
                public int dt;
            }
        }
    }
}
