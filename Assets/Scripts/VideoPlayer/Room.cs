using System;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

using WebSocketSharp;

namespace VmesteApp.Rooms
{
    public class Room
    {
        public static RoomInfo LastRoomInfo;
        private static TcpClient socketConnection;
        private static WebSocket ws;
        private static Thread clientReceiveThread;
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
            ConnectToServer();
            JoinServer(true);
            return ans;
        }
        public static async void JoinServer(bool isAdmin)
        {
            ServerJoinCommand sjc = new ServerJoinCommand();
            sjc.command = "join";
            sjc.user_name = LastRoomInfo.user.user_name;
            sjc.avatar_url = LastRoomInfo.user.avatarurl;
            sjc.room = LastRoomInfo.code;
            sjc.profile_id = LastRoomInfo.user.profileid;
            sjc.username_color = LastRoomInfo.user.usernamecolor;
            sjc.uuid = LastRoomInfo.user.uuid;
            sjc.is_author = isAdmin.ToString();

            string message = JsonUtility.ToJson(sjc);


            //Debug.Log(message);

            //try
            //{
            //    // Create a TcpClient.
            //    // Note, for this client to work you need to have a TcpServer 
            //    // connected to the same address as specified by the server, port
            //    // combination.
            //    Int32 port = 443;
            //    TcpClient client = new TcpClient(LastRoomInfo.server, port);

            //    // Translate the passed message into ASCII and store it as a Byte array.
            //    byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            //    string d = "";
            //    foreach (byte b in data)
            //        d += b + " ";
            //    Debug.Log(d);

            //    // Get a client stream for reading and writing.
            //    //  Stream stream = client.GetStream();

            //    NetworkStream stream = client.GetStream();

            //    // Send the message to the connected TcpServer. 
            //    stream.Write(data, 0, data.Length);

            //    Debug.Log(string.Format("Sent: {0}", message));

            //    // Receive the TcpServer.response.

            //    // Buffer to store the response bytes.
            //    data = new byte[256];

            //    // String to store the response ASCII representation.
            //    string responseData = string.Empty;

            //    // Read the first batch of the TcpServer response bytes.
            //    Int32 bytes = stream.Read(data, 0, data.Length);
            //    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            //    Debug.LogWarning(string.Format("Received: {0}", responseData));

            //    // Close everything.
            //    stream.Close();
            //    client.Close();
            //}
            //catch (ArgumentNullException e)
            //{
            //    Debug.LogError(string.Format("ArgumentNullException: {0}", e));
            //}
            //catch (SocketException e)
            //{
            //    Debug.LogError(string.Format("SocketException: {0}", e));
            //}
            //Debug.Log("END");

            string[] server = LastRoomInfo.server.Split('.');
            string host = "";           
            for (int i = 1;i<server.Length;i++)
            {
                host += server[i] + ".";
            }
            host = host.Substring(0, host.Length - 1);

            ws = new WebSocket("wss://" + host+"/chat/stream/");
            ws.Connect();
            ws.Log.Level = LogLevel.Trace;

            ws.OnOpen += (sender, e) =>
            {
                ws.Send(message);
            }; ;
            ws.OnMessage += (sender, e) =>
            {
                Debug.Log(e.Data);
            };
            ws.OnError += (sender, e) =>
            {
                Debug.Log(e.Message);
            };

            ws.OnClose += (sender, e) =>
            {
                Debug.Log(e.Reason);
            };

            Debug.Log(LastRoomInfo.server);

            //while (socketConnection == null)
            //    await Task.Yield();
            //Debug.Log(host);
            //NetworkStream stream = socketConnection.GetStream();
            //string head = string.Format("POST /chat/stream/ HTTP/1.1\n" +
            //    "Upgrade: WebSocket\n" +
            //    "Connection: Upgrade\n" +
            //    "Host: {0}\n" +
            //    "Origin: wss://{0}\n" +
            //    "WebSocket-Protokol: TCP\n"
            //    , host);
            //Debug.Log(head);
            //if (stream.CanWrite)
            //{
            //    byte[] clientMessageAsByteArray = System.Text.Encoding.ASCII.GetBytes(head);

            //    //byte[] headByte = System.Text.Encoding.ASCII.GetBytes(head);

            //    stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
            //    Debug.Log("Client sent his message - should be received by server");
            //}
        }


        public static void ConnectToServer()
        {
            //clientReceiveThread = new Thread(new ThreadStart(ListenForJoinServerData));
            //clientReceiveThread.IsBackground = true;
            //clientReceiveThread.Start();
        }
        private static void ListenForJoinServerData()
        {
            socketConnection = new TcpClient(LastRoomInfo.server, 443);
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                try
                {
                    // Get a stream object for reading 				
                    using (NetworkStream stream = socketConnection.GetStream())
                    {
                        int length;
                        // Read incomming stream into byte arrary. 					
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);
                            // Convert byte array to string message. 						
                            string serverMessage = System.Text.Encoding.ASCII.GetString(incommingData);
                            Debug.Log("server message received as: " + serverMessage);
                        }
                    }
                }
                catch { }
            }
        }
        public static async void SendActionCommand(string command)
        {

        }
        [System.Serializable]
        public struct RoomInfo
        {
            public string status;
            public string roomurl;
            public string code;
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
            string command;
            string room;
            string progress;
            string is_pause;
            string uuid;
        }
        [System.Serializable]
        public struct ServerJoinCommand
        {
            public string command;
            public string user_name;
            public string avatar_url;
            public string room;
            public string profile_id;
            public string username_color;
            public string uuid;
            public string is_author;
        }
        [System.Serializable]
        public struct ServerJoinStatus
        {

        }
        [System.Serializable]
        public struct UserInfo
        {
            public string user_name;
            public string avatarurl;
            public string profileid;
            public string usernamecolor;
            public string uuid;
        }
    }
}
