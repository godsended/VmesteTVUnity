using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using VmesteApp.Rooms;

namespace VmesteApp.UI
{
    public class VideoPlayerPanel : Panel
    {
        public VideoPlayer VideoPlayer;
        public VideoPlayerLayout VideoPlayerLayout;
        public RawImage TargetImage;
        private void Start()
        {
            Init();
            onHide.AddListener(OnHide);
            VideoPlayer.started += VideoPlayerLayout.Init;
        }
        public async void Open(string filmID)
        {
            Room.RoomInfo roomInfo = await Room.GetInfo(filmID);
            Debug.LogWarning(roomInfo.mp4);
            string u = "";
            for(int i = 0;i<roomInfo.mp4.mp4urls.Length;i++)
            {
                if(roomInfo.mp4.mp4urls[0].quality.Contains("480"))
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
            VideoPlayer.enabled = true;
            VideoPlayer.Play();
            VideoPlayerLayout.LoadIndicator.gameObject.SetActive(true);
        }
        public void OnHide()
        {
            VideoPlayer.Stop();
            VideoPlayerLayout.TargetImage.color = Color.black;
            VideoPlayerLayout.isInit = false;
            VideoPlayer.enabled = false;
        }
    }
}
