using System.Net;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace VmesteApp.Catalog
{
    public class Genres : MonoBehaviour
    {
        public static async Task<GenresResult> Get()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Project.URLs.mainDomain + Project.URLs.genresRequest + "?token=" + Auth.User.Instance.token);

            request.Method = "GET";

            request.UserAgent = Project.UserAgent;

            WebResponse response = await request.GetResponseAsync();

            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            GenresResult ans = JsonUtility.FromJson<GenresResult>(responseString);
            Debug.Log(responseString);
            return ans;
        }
        [System.Serializable]
        public struct GenresResult
        {
            [System.Serializable]
            public struct GenreInfo
            {
                public string id;
                public string np;
                public string name;
            }
            public string status;
            public GenreInfo[] data;
        }
    }
}
