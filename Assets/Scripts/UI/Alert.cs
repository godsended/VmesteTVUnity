using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VmesteApp.UI
{
    public class Alert : MonoBehaviour
    {
        public static Alert Instance;
        [SerializeField] private Animator animator;
        [SerializeField] private Text Label, Text;
        private List<AlertInfo> stack;

        private bool isWorkerActive;
        private void Awake()
        {
            Instance = this;
            stack = new List<AlertInfo>();
            isWorkerActive = false;
        }
        public void Show(string label, string text)
        {
            AlertInfo ai = new AlertInfo();
            ai.label = label;
            ai.text = text;
            ai.time = 5000;
            stack.Add(ai);
            if (!isWorkerActive)
                startWorker();
        }
        private async void startWorker()
        {
            isWorkerActive = true;
            int delay = 1500;
            while(stack.Count!=0)
            {
                AlertInfo ai = stack[0];
                Label.text = ai.label;
                Text.text = ai.text;
                animator.SetTrigger("Show");
                await System.Threading.Tasks.Task.Delay(ai.time);
                Hide();
                await System.Threading.Tasks.Task.Delay(delay);
                stack.RemoveAt(0);
            }
            isWorkerActive = false;
        }
        public void Hide()
        {
            animator.SetTrigger("Hide");
        }
        private struct AlertInfo
        {
            public string label;
            public string text;
            public int time;
        }
    }
}
