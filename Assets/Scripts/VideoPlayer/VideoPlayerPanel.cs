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
        }
        public async void Open(string filmID)
        {
            Room.RoomInfo roomInfo = await Room.GetInfo(filmID);
            Debug.LogWarning(roomInfo.mp4);
            VideoPlayer.url = roomInfo.mp4.mp4urls[0].url;
            VideoPlayer.Play();
            VideoPlayer.started += VideoPlayerLayout.Init;
        }
        public void OnHide()
        {
            VideoPlayer.Stop();
            VideoPlayerLayout.isInit = false;
        }
    }
}
