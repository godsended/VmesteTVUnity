using System.Net;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace VmesteApp.Catalog
{
    public class FilmInfo : MonoBehaviour
    {
        public static async Task<FilmResult> GetInfo(string id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Project.URLs.mainDomain + Project.URLs.filmInfoRequest + id + "?token=" + Auth.User.Instance.token);

            request.Method = "GET";
            request.UserAgent = Project.UserAgent;

            WebResponse response = await request.GetResponseAsync();

            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Debug.Log(responseString);

            FilmResult res = JsonUtility.FromJson<FilmResult>(responseString);

            return res;
        }
        [System.Serializable]
        public struct FilmResult
        {
            [System.Serializable]
            public struct FilmInfo
            {                
                public string is_serial;
                public Genre[] genres;
                public string kp_id;
                public string year;
                public string country;
                public string[] producer;
                public string[] actors;
                public string timing;
                public string kp_raiting;
                public string title;
                public string vmeste_url;
                public string thumb;
                public string description;
                public Comment[] comments;
                [System.Serializable]
                public struct Genre
                {
                    public string name;
                    public string name2;
                    public string name3;
                    public string slug;
                }
                [System.Serializable]
                public struct Comment
                {
                    public string username;
                    public string comment;
                    public string mark;
                    public string avatar;
                }
            }
            public string status;
            public FilmInfo data;
        }
    }
}
