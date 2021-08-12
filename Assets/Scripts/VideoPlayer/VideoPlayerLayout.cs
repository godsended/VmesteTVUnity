using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace VmesteApp.Rooms
{
    public class VideoPlayerLayout : MonoBehaviour
    {
        private bool isActive = false;
        private bool timeLineChangeFlag = false;
        private bool isQualityPanelOpen, isFullScreen;
        public bool isAdmin;
        public bool isInit;
        [SerializeField] private RectTransform fullScreenRect, windowRect;
        [SerializeField] private Animator Animator;
        [SerializeField] private Animator PlayButtonAnimator, QualityPanelAnimator;
        [SerializeField] public List<QualityItem> QualityItems;
        public RectTransform PlayerRectTransform;
        public Slider TimeLine, VolumeLine;
        public Text TimeText;
        public Image VolumeButtonImage, BlackBG, LoadIndicator;
        public RawImage TargetImage;
        private VideoPlayer VideoPlayer;
        [SerializeField] private AudioSource AudioSource;
        private void Awake()
        {
            UI.UI.Instance.VideoPlayerPanel.VideoPlayer.started += Init;
        }
        public async void Init(VideoPlayer source)
        {
            if (!isInit)
            {
                TargetImage.color = Color.white;
                VideoPlayer = source;
                VideoPlayer.enabled = true;
                ulong totalFrames = VideoPlayer.frameCount;
                TimeLine.maxValue = totalFrames / VideoPlayer.frameRate;
                PlayerRectTransform.sizeDelta = new Vector2(PlayerRectTransform.sizeDelta.x, ((float)VideoPlayer.height / (float)VideoPlayer.width) * Device.GetUIScreenSize().x);
                VolumeLine.value = 0.25f;
                VideoPlayer.GetTargetAudioSource(0).volume = 0.25f;
                for (int i = 0; i < QualityItems.Count; i++)
                {
                    QualityItems[i].gameObject.SetActive(false);
                }
                for (int i = 0; i < Room.LastRoomInfo.mp4.mp4urls.Length; i++)
                {
                    QualityItems[i].ID = i;
                    QualityItems[i].Name.text = Room.LastRoomInfo.mp4.mp4urls[i].quality;
                    QualityItems[i].URL = Room.LastRoomInfo.mp4.mp4urls[i].url;
                    QualityItems[i].gameObject.SetActive(true);
                }

                windowRect.pivot = PlayerRectTransform.pivot;
                windowRect.rotation = PlayerRectTransform.rotation;
                windowRect.anchorMin = PlayerRectTransform.anchorMin;
                windowRect.anchorMax = PlayerRectTransform.anchorMax;
                windowRect.sizeDelta = PlayerRectTransform.sizeDelta;
                windowRect.anchoredPosition = PlayerRectTransform.anchoredPosition;

                BlackBG.gameObject.SetActive(false);
                isFullScreen = false;
                isInit = true;

                await Room.JoinServer(isAdmin);

                if (isAdmin)
                    await Room.SendActionCommand("resume");

                Room.jws.OnMessage += UI.UI.Instance.VideoPlayerPanel.CheckActions;

                UI.UI.Instance.VideoPlayerPanel.UpdateRoom();

                LoadIndicator.gameObject.SetActive(false);
            }
        }
        private void Update()
        {
            if (VideoPlayer == null)
                return;

            AudioSource.time = (float)VideoPlayer.time;

            if ((VideoPlayer.isPaused || !VideoPlayer.isPlaying) && AudioSource.isPlaying)
            {
                AudioSource.Pause();
                return;
            }

            if ((!VideoPlayer.isPaused && VideoPlayer.isPlaying) && !AudioSource.isPlaying)
            {
                AudioSource.Play();
                return;
            }
        }
        private void LateUpdate()
        {
            if (VideoPlayer == null)
                return;

            if (VideoPlayer.isPlaying)
            {
                timeLineChangeFlag = true;
                TimeLine.value = (float)VideoPlayer.time;

                System.TimeSpan time = System.TimeSpan.FromSeconds(TimeLine.value);
                System.TimeSpan totalTime = System.TimeSpan.FromSeconds(TimeLine.maxValue);

                TimeText.text = "";
                if (time.Hours != 0)
                    TimeText.text += time.Hours.ToString() + ":";
                TimeText.text += time.Minutes + ":" + time.Seconds + " / ";

                if (totalTime.Hours != 0)
                    TimeText.text += totalTime.Hours.ToString() + ":";
                TimeText.text += totalTime.Minutes + ":" + totalTime.Seconds;
            }
            Room.progress = TimeLine.value;
            Room.isPause = !VideoPlayer.isPaused;
        }
        public async void Pause()
        {
            if (VideoPlayer.isPaused == false)
            {
                VideoPlayer.Pause();
                PlayButtonAnimator.SetTrigger("OnPause");
                if (isAdmin)
                    await Room.SendActionCommand("pause");
            }
        }
        public async void Play()
        {
            if (VideoPlayer.isPaused == true)
            {
                VideoPlayer.Play();
                PlayButtonAnimator.SetTrigger("OnPlay");
                //Hide(5000);
                if (isAdmin)
                    await Room.SendActionCommand("resume");
            }
        }
        public void Show()
        {
            if (!isActive && isInit)
            {
                isActive = true;
                Animator.StopPlayback();
                PlayButtonAnimator.StopPlayback();
                Animator.SetTrigger("Show");
            }
        }
        public void Hide()
        {
            if (isActive)
            {
                isActive = false;
                Animator.StopPlayback();
                PlayButtonAnimator.StopPlayback();
                Animator.SetTrigger("Hide");
                Room.AbortJWSThred();
            }
        }
        public async void Hide(int time)
        {
            await System.Threading.Tasks.Task.Delay(time);
            Hide();
        }
        public void OnClick()
        {
            if (isInit)
            {
                if (!isActive)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }
        public void OnPlayAndPauseClick()
        {
            if (VideoPlayer.isPaused)
            {
                Play();
            }
            else
            {
                Pause();
            }
        }
        public async void OnTimeLineMooved()
        {
            if (timeLineChangeFlag)
            {
                timeLineChangeFlag = false;
            }
            else
            {
                Debug.Log("Mooved");
                VideoPlayer.time = TimeLine.value;
                if (isAdmin)
                    await Room.SendActionCommand("resume");
            }
        }
        public void OnVolumeLineMooved()
        {
            VideoPlayer.GetTargetAudioSource(0).volume = VolumeLine.value;
            if (VolumeLine.value == 0)
            {

            }
            if (VolumeLine.value == 1)
            {

            }
        }
        public void SwitchQuality(int id)
        {
            double time = VideoPlayer.time;
            VideoPlayer.Stop();
            VideoPlayer.url = QualityItems[id].URL;
            VideoPlayer.Play();
            VideoPlayer.time = time;
        }
        public void OnSettingsButtonClick()
        {
            if (isQualityPanelOpen)
            {
                QualityPanelAnimator.SetTrigger("Hide");
            }
            else
            {
                QualityPanelAnimator.SetTrigger("Show");
            }
            isQualityPanelOpen = !isQualityPanelOpen;
        }
        public void OnFullScreen()
        {
            if (isFullScreen)
            {
                ToWindow();
            }
            else
            {
                ToFullScreen();
            }
        }
        public void ToFullScreen()
        {
            if (!isFullScreen)
            {
                PlayerRectTransform.pivot = fullScreenRect.pivot;
                PlayerRectTransform.rotation = fullScreenRect.rotation;
                PlayerRectTransform.anchorMin = fullScreenRect.anchorMin;
                PlayerRectTransform.anchorMax = fullScreenRect.anchorMax;
                if (Device.GetUIScreenSize().y * ((float)VideoPlayer.height / (float)VideoPlayer.width) <= Device.GetUIScreenSize().x)
                    PlayerRectTransform.sizeDelta = new Vector2(Device.GetUIScreenSize().y, Device.GetUIScreenSize().y * ((float)VideoPlayer.height / (float)VideoPlayer.width));
                else
                    PlayerRectTransform.sizeDelta = new Vector2(Device.GetUIScreenSize().x * ((float)VideoPlayer.width / (float)VideoPlayer.height), Device.GetUIScreenSize().x);
                PlayerRectTransform.anchoredPosition = fullScreenRect.anchoredPosition;
                isFullScreen = true;
                BlackBG.gameObject.SetActive(true);
            }
        }
        public void ToWindow()
        {
            if (isFullScreen)
            {
                PlayerRectTransform.pivot = windowRect.pivot;
                PlayerRectTransform.rotation = windowRect.rotation;
                PlayerRectTransform.anchorMin = windowRect.anchorMin;
                PlayerRectTransform.anchorMax = windowRect.anchorMax;
                PlayerRectTransform.sizeDelta = new Vector2(windowRect.sizeDelta.x, ((float)VideoPlayer.height / (float)VideoPlayer.width) * Device.GetUIScreenSize().x);
                PlayerRectTransform.anchoredPosition = windowRect.anchoredPosition;
                isFullScreen = false;
                BlackBG.gameObject.SetActive(false);
            }
        }
    }
}
