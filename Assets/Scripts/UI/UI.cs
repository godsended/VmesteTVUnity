using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VmesteApp.Auth;

namespace VmesteApp.UI
{
    public class UI : MonoBehaviour
    {
        public static UI Instance;
        public UnityEvent onHideAllPanels;
        [SerializeField] private Animator LogoAnimator;
        public LoginPanel LoginPanel;
        public RegistrationPanel RegistrationPanel;
        public FilmInfoPanel FilmInfoPanel;
        public VideoPlayerPanel VideoPlayerPanel;
        private void Awake()
        {
            Instance = this;
            onHideAllPanels = new UnityEvent();
            Application.targetFrameRate = 60;
            History.Init();
        }
        private void Start()
        {
            if (Data.GetData("Token") == "")
            {
                LogoAnimator.SetTrigger("Upside");
                LoginPanel.Show();
            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                History.Back();
            }
        }
        public static class History
        {
            private static List<Panel> history;
            public static void Init()
            {
                history = new List<Panel>();
            }
            public static void ClearHistory()
            {
                history.Clear();
            }
            public static void Back()
            {
                if (history.Count > 1)
                {
                    Panel lastPage = history[history.Count - 1];
                    lastPage.Hide();
                    Panel prevPage = history[history.Count - 2];
                    prevPage.Show();
                }
            }
            public static void Add(Panel panel)
            {
                if(!history.Contains(panel))
                {
                    if ((history.Count != 0) || (history.Count == 0 && panel.isMainPage))
                    {
                        history.RemoveRange(0, history.IndexOf(panel) + 1);
                        history.Add(panel);
                    }
                }
                else
                {
                    MoveTo(panel);
                }
            }
            public static void MoveTo(Panel panel)
            {
                if (history.Contains(panel))
                {
                    if (history.IndexOf(panel) + 1 != history.Count)
                        history.RemoveRange(history.IndexOf(panel) + 1, history.Count - history.IndexOf(panel) - 1);
                }
            }
        }
    }
}
