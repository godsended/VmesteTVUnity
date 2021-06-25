using System.Net;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VmesteApp.Auth;

namespace VmesteApp.UI
{
    public class RegistrationPanel : Panel
    {
        [SerializeField] private InputField NickInput, EmailInput, PasswordInput, PasswordConfirmInput;
        [SerializeField] private Animator animator;

        public async void Register()
        {
            ServerAnswer ans;
            if (PasswordInput.text == PasswordConfirmInput.text)
            {
                ans = await User.Instance.Register(NickInput.text, EmailInput.text, PasswordInput.text);
                if (ans.status == "success")
                {
                    User.Instance.onAuthComplited.Invoke();
                    animator.SetTrigger("Hide");
                }
                switch (ans.message)
                {
                    case "email_exists":
                        Alert.Instance.Show("Ошибка", "Аккаунт с такой почтой уже зарегестрирован");
                        break;
                }
            }
            else
                Debug.LogError("Passwords not equals to each other");
        }

    }
}
