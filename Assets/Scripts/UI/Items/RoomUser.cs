using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace VmesteApp.UI
{
    public class RoomUser : MonoBehaviour
    {
        public RawImage Avatar;
        public Text Nick, Time;
        public string uuid;

        public IEnumerator LoadAvatar(string url)
        {
            Texture2D tex = new Texture2D(2, 2);
            byte[] data;
            UnityWebRequest client = UnityWebRequest.Get(url);
            yield return client.SendWebRequest();
            data = client.downloadHandler.data;
            tex.LoadImage(data);
            Debug.Log(data.Length);
            Avatar.texture = tex;
        }
    }
}
