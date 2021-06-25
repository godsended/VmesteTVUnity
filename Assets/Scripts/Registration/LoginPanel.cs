using System.Net;
using System.Text;  // For class Encoding
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VmesteApp.Auth;

namespace VmesteApp.UI
{
    public class LoginPanel : Panel
    {
        [SerializeField] private InputField MainInput, PasswordInput;
        [SerializeField] private Animator animator;
        void Start()
        {
            Init();
            if (Data.GetData("Token") != "")
            {
                AutoLogin();
            }
        }
        public async void Login()
        {
            ServerAnswer ans;
            ans = await User.Instance.Login(MainInput.text, PasswordInput.text);
            if(ans.status == "success")
            {
                User.Instance.onAuthComplited.Invoke();
                Hide();
            }
            switch(ans.message)
            {
                case "invalid_username_or_password":
                    Alert.Instance.Show("Ошибка", "Неправильный логин или пароль");
                    break;
            }
        }
        public async void AutoLogin()
        {
            ServerAnswer ans;
            ans = await User.Instance.AutoLogin();
            if (ans.status == "success")
            {
                User.Instance.onAuthComplited.Invoke();
                //animator.SetTrigger("Hide");
            }
        }
        public void ResetPassword()
        {
            User.Instance.ResetPassword();
        }
    }
}
