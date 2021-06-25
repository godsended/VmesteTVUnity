using System.Net;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace VmesteApp.Catalog
{
    public class Search : MonoBehaviour
    {
        private static int taskNum = 0;
        public static async Task<FilmResult> Film(string query)
        {
            taskNum++;
            int t = taskNum;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Project.URLs.mainDomain + Project.URLs.searchRequest + query + "&token=" + Auth.User.Instance.token);

            request.Method = "GET";

            request.UserAgent = Project.UserAgent;

            WebResponse response = await request.GetResponseAsync();

            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            FilmResult ans = JsonUtility.FromJson<FilmResult>(responseString);
            Debug.Log(responseString);
            if (taskNum == t)
                return ans;
            else
            {
                ans.status = "CANCELED";
                return ans;
            }
        }
        public static async Task<FilmResult> PopularFilms(string genre)
        {
            HttpWebRequest request;
            if (genre == "")
                request = (HttpWebRequest)WebRequest.Create(Project.URLs.mainDomain + Project.URLs.searchPopularRequest + "&token=" + Auth.User.Instance.token);
            else
                request = (HttpWebRequest)WebRequest.Create(Project.URLs.mainDomain + Project.URLs.searchPopularRequest + "&genre=" + genre + "&token=" + Auth.User.Instance.token);

            request.Method = "GET";

            request.UserAgent = Project.UserAgent;

            WebResponse response = await request.GetResponseAsync();

            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            FilmResult ans = JsonUtility.FromJson<FilmResult>(responseString);
            Debug.Log(responseString);
            return ans;
        }
        public static async Task<FilmResult> NewFilms(string genre)
        {
            HttpWebRequest request;
            if(genre == "")
                request = (HttpWebRequest)WebRequest.Create(Project.URLs.mainDomain + Project.URLs.searchNewRequest);
            else
                request = (HttpWebRequest)WebRequest.Create(Project.URLs.mainDomain + Project.URLs.searchNewRequest + "&genre=" + genre + "&token=" + Auth.User.Instance.token);

            request.Method = "GET";

            request.UserAgent = Project.UserAgent;

            WebResponse response = await request.GetResponseAsync();

            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            FilmResult ans = JsonUtility.FromJson<FilmResult>(responseString);
            Debug.Log(responseString);
            return ans;
        }
        [System.Serializable]
        public struct FilmResult
        {
            [System.Serializable]
            public struct FilmInfo
            {
                public string pk;
                public string is_hidden;
                public string kinopoisk_id;
                public string kp_rating;
                public string title;
                public string year;
                public string description;
                public string is_serial;
                public string image_url;
                public string trailer_url;
                public string url;
                public string room_url;
                public string genre;
            }
            public string status;
            public FilmInfo[] data;
        }
    }
}
