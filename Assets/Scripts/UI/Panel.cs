using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VmesteApp.UI
{
    public class Panel : MonoBehaviour
    {
        public UnityEvent onHide, onShow;
        public Animator Animator;
        public bool isMainPage;
        [SerializeField] private bool isActive;
        private bool isInitialized;
        private void Awake()
        {
            if(onShow == null)
                onShow = new UnityEvent();
            if(onHide == null)
                onHide = new UnityEvent();
        }
        private void Start()
        {
            Init();
        }
        public void Init()
        {
            isInitialized = true;
            UI.Instance.onHideAllPanels.AddListener(Hide);
            Debug.Log(gameObject.name + ": sbscribed");
        }
        public void Hide()
        {
            if (isActive)
            {
                Animator.SetTrigger("Hide");
                onHide.Invoke();
            }
            isActive = false;
        }
        public void Show()
        {
            if (!isActive)
            {
                UI.Instance.onHideAllPanels.Invoke();
                Animator.SetTrigger("Show");
                if(isMainPage)
                    UI.History.ClearHistory();
                UI.History.Add(this);
                onShow.Invoke();
            }
            isActive = true;
        }
    }
}
