using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using VmesteApp.Rooms;
using System.Threading.Tasks;

namespace VmesteApp.UI
{
    public class VideoPlayerPanel : Panel
    {
        private List<Comment> comments;
        private Dictionary<string, RoomUser> roomUsers;
        public VideoPlayer VideoPlayer;
        public VideoPlayerLayout VideoPlayerLayout;
        public RawImage TargetImage;
        public RectTransform ChatRectTransform, RoomUsersRectTransform, UsersCountRectTransform, ChatContentRectTransform, RoomUsersContentRectTransform, TopBarRectTransform;
        public InputField MessageInput;
        public Comment commentPrefab;
        public RoomUser roomUserPrefab;
        public Text UsersCountText, RoomCodeText;
        private void Awake()
        {
            BestHTTP.HTTPManager.Setup();
            comments = new List<Comment>();
            roomUsers = new Dictionary<string, RoomUser>();
        }
        private void Start()
        {
            Init();
            onHide.AddListener(OnHide);
            onHide.AddListener(delegate { VideoPlayerLayout.isInit = false; });
        }

        public async void UpdateRoom()
        {
            Room.RoomStatus preRs = await Room.SendActionCommand("user_progress");
            if (preRs.is_pause)
            {
                VideoPlayerLayout.Pause();
                Debug.Log("PAUSED");
            }
            else
            {
                VideoPlayerLayout.Play();
                Debug.Log("RESUMED");
            }
            VideoPlayerLayout.TimeLine.value = preRs.progress;
            VideoPlayerLayout.OnTimeLineMooved();

            while (VideoPlayerLayout.isInit)
            {
                Room.RoomStatus rs;
                if (VideoPlayerLayout.isAdmin)
                {
                    rs = await Room.SendActionCommand("progress");
                }
                else
                {
                    rs = await Room.SendActionCommand("user_progress");
                }
                if (!string.IsNullOrEmpty(rs.user_count))
                    UsersCountText.text = rs.user_count;
                Dictionary<string, Room.RoomStatus.User> rsUsersList = new Dictionary<string, Room.RoomStatus.User>();
                if (rs.users != null)
                {
                    for (int i = 0; i < rs.users.Length; i++)
                    {
                        if (rs.users[i].uuid != null)
                        {
                            rsUsersList.Add(rs.users[i].uuid, rs.users[i]);
                            if (!roomUsers.ContainsKey(rs.users[i].uuid))
                            {
                                RoomUser rUser = Instantiate(roomUserPrefab, RoomUsersContentRectTransform);
                                rUser.uuid = rs.users[i].uuid;
                                rUser.Nick.text = rs.users[i].name;
                                rUser.StartCoroutine(rUser.LoadAvatar(rs.users[i].avatar));
                                Debug.LogWarning(rs.users[i].avatar);
                                roomUsers.Add(rUser.uuid, rUser);
                            }
                        }
                    }
                }
                if (roomUsers.Count > 0)
                {
                    foreach (RoomUser rUser in roomUsers.Values)
                    {
                        if (!rsUsersList.ContainsKey(rUser.uuid))
                        {
                            roomUsers.Remove(rUser.uuid);
                            Destroy(rUser.gameObject);
                        }
                        else
                        {
                            System.TimeSpan t = System.TimeSpan.FromSeconds(rsUsersList[rUser.uuid].progress);
                            string ts = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                                        t.Hours,
                                                        t.Minutes,
                                                        t.Seconds);
                            rUser.Time.text = ts;
                        }
                    }
                }
                await Task.Delay(500);
            }
        }
        public void CheckActions(BestHTTP.WebSocket.WebSocket webSocket, string msg)
        {
            Room.RoomStatus rs = JsonUtility.FromJson<Room.RoomStatus>(msg);
            Debug.Log(msg);
            switch (rs.msg_type)
            {
                case 0:
                    switch (rs.operation)
                    {
                        case null:
                            if (!string.IsNullOrEmpty(rs.message))
                            {
                                Comment c = Instantiate(commentPrefab, ChatContentRectTransform);
                                c.Name.text = rs.username;
                                c.Text.text = rs.message;
                                c.StartCoroutine(c.LoadAvatar(rs.avatar));
                                comments.Add(c);
                            }
                            break;
                        case "pause":
                            if (!VideoPlayer.isPaused && !VideoPlayerLayout.isAdmin)
                            {
                                VideoPlayerLayout.Pause();
                                VideoPlayerLayout.TimeLine.value = rs.progress;
                                VideoPlayerLayout.OnTimeLineMooved();
                                Debug.Log("PAUSED");
                            }
                            break;
                        case "resume":
                            if (VideoPlayer.isPaused && !VideoPlayerLayout.isAdmin)
                            {
                                VideoPlayerLayout.Play();
                                VideoPlayerLayout.TimeLine.value = rs.progress;
                                VideoPlayerLayout.OnTimeLineMooved();
                                Debug.Log("RESUMED");
                            }
                            break;
                        case "seek":
                            if (!VideoPlayerLayout.isAdmin)
                            {
                                VideoPlayerLayout.TimeLine.value = rs.progress;
                                VideoPlayerLayout.OnTimeLineMooved();
                            }
                            break;

                    }
                    break;
            }
        }
        private void Update()
        {
            float spacing = 20;

            UsersCountRectTransform.anchoredPosition = new Vector2(UsersCountRectTransform.anchoredPosition.x, -TopBarRectTransform.sizeDelta.y - VideoPlayerLayout.PlayerRectTransform.sizeDelta.y - spacing);
            RoomUsersRectTransform.anchoredPosition = new Vector2(RoomUsersRectTransform.anchoredPosition.x, UsersCountRectTransform.anchoredPosition.y - UsersCountRectTransform.sizeDelta.y - spacing);
            ChatRectTransform.offsetMin = new Vector2(0, 300); // 250 - высота инпута + 50 для расстояния
            ChatRectTransform.offsetMax = new Vector2(0, RoomUsersRectTransform.anchoredPosition.y - RoomUsersRectTransform.sizeDelta.y - spacing);
        }
        public async void Open(string filmID)
        {
            Room.RoomInfo roomInfo = await Room.GetInfo(filmID);
            Debug.LogWarning(roomInfo.mp4);
            string u = "";
            for (int i = 0; i < roomInfo.mp4.mp4urls.Length; i++)
            {
                if (roomInfo.mp4.mp4urls[0].quality.Contains("480"))
                {
                    u = roomInfo.mp4.mp4urls[i].url;
                }
            }
            if (u != "")
            {
                VideoPlayer.url = u;
            }
            else
            {
                VideoPlayer.url = roomInfo.mp4.mp4urls[roomInfo.mp4.mp4urls.Length - 1].url;
            }
            RoomCodeText.text = "Код: " + roomInfo.code;
            VideoPlayerLayout.isAdmin = true;
            VideoPlayer.enabled = true;
            VideoPlayer.Play();
            VideoPlayerLayout.LoadIndicator.gameObject.SetActive(true);
        }
        public async void Open(string token, string roomID)
        {
            Room.RoomInfo roomInfo = await Room.GetInfo(token, roomID);
            Debug.LogWarning(roomInfo.mp4);
            if (roomInfo.status == "success")
            {
                string u = "";
                for (int i = 0; i < roomInfo.mp4.mp4urls.Length; i++)
                {
                    if (roomInfo.mp4.mp4urls[0].quality.Contains("480"))
                    {
                        u = roomInfo.mp4.mp4urls[i].url;
                    }
                }
                if (u != "")
                {
                    VideoPlayer.url = u;
                }
                else
                {
                    VideoPlayer.url = roomInfo.mp4.mp4urls[roomInfo.mp4.mp4urls.Length - 1].url;
                }
                RoomCodeText.text = "Код: " + roomInfo.code;
                VideoPlayerLayout.isAdmin = false;
                VideoPlayer.enabled = true;
                VideoPlayer.Play();
                VideoPlayerLayout.LoadIndicator.gameObject.SetActive(true);
                Show();
            }
            else
            {
                Alert.Instance.Show("Ошибка", "Неверный код комнаты");
            }
        }
        public void OnHide()
        {
            VideoPlayer.Stop();
            VideoPlayerLayout.TargetImage.color = Color.black;
            VideoPlayerLayout.isInit = false;
            VideoPlayer.enabled = false;
            foreach (Comment com in comments)
            {
                Destroy(com.gameObject);
            }
            comments.Clear();
            foreach (RoomUser rs in roomUsers.Values)
            {
                Destroy(rs.gameObject);
            }
            roomUsers.Clear();
        }
        public void SendChatMessage()
        {
            Room.SendChatMessage(MessageInput.text);
            MessageInput.text = "";
        }
        public void InsertChatMessage(string nick, string message, string avatar)
        {

        }
    }
}
