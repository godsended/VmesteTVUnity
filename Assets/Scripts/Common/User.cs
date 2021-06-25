using System.Net;
using System.Text;  // For class Encoding
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VmesteApp.Auth
{
    public class User : MonoBehaviour
    {
        public static User Instance;
        public string token;
        public UnityEvent onAuthComplited;
        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            if(Data.GetData("Token")!="")
            {
                token = Data.GetData("Token");
            }
        }

        public async Task<ServerAnswer> Login(string mail, string password)
        {
            LoginData loginData = new LoginData();
            loginData.username = mail;
            loginData.password = password;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Project.URLs.mainDomain + Project.URLs.loginRequest);

            string postData = JsonUtility.ToJson(loginData);
            byte[] data = Encoding.UTF8.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
            request.UserAgent = Project.UserAgent;

            using (Stream stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }

            WebResponse response = await request.GetResponseAsync();

            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            ServerAnswer ans = JsonUtility.FromJson<ServerAnswer>(responseString);
            if (ans.status == "success")
            {
                Data.SaveData("Token", ans.token);
                token = ans.token;
                Debug.Log("Login complited");
            }
            Debug.Log(responseString);
            return ans;
        }
        public async Task<ServerAnswer> AutoLogin()
        {
            AutoLoginData autoLoginData = new AutoLoginData();
            autoLoginData.token = Data.GetData("Token");
            if (autoLoginData.token != "")
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Project.URLs.mainDomain + Project.URLs.loginRequest);

                string postData = JsonUtility.ToJson(autoLoginData);
                byte[] data = Encoding.UTF8.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                request.UserAgent = Project.UserAgent;

                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }

                WebResponse response = await request.GetResponseAsync();

                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                ServerAnswer ans = JsonUtility.FromJson<ServerAnswer>(responseString);

                Debug.Log(responseString);

                return ans;
            }
            else
            {
                return new ServerAnswer();
            }
        }
        public async Task<ServerAnswer> Register(string nick, string mail, string password)
        {
            RegisterData regData = new RegisterData();
            regData.name = nick;
            regData.username = mail;
            regData.password = password;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Project.URLs.mainDomain + Project.URLs.registrationRequest);

            string postData = JsonUtility.ToJson(regData);
            byte[] data = Encoding.UTF8.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
            request.UserAgent = Project.UserAgent;


            using (Stream stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }

            WebResponse response = await request.GetResponseAsync();

            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            ServerAnswer ans = JsonUtility.FromJson<ServerAnswer>(responseString);

            Debug.Log(responseString);

            if (ans.status == "success")
            {
                Data.SaveData("Token", ans.token);
                token = ans.token;
                Debug.Log("Login complited");
            }

            return ans;
        }
        public void ResetPassword()
        {
            Application.OpenURL("https://vmeste-tv.site/reset/password/");
        }
    }
    public struct LoginData
    {
        public string username;
        public string password;
    }
    public struct RegisterData
    {
        public string username;
        public string password;
        public string name;
    }
    public struct AutoLoginData
    {
        public string username;
        public string password;
        public string token;
    }
    public struct ServerAnswer
    {
        public string status;
        public string message;
        public string token;
    }
}
