using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

namespace VmesteApp.UI
{
    public class Comment : MonoBehaviour
    {
        public RawImage Image;
        public Text Name;
        public Text Text;

        public IEnumerator LoadAvatar(string url)
        {
            Texture2D tex = new Texture2D(2, 2);
            byte[] data;

            UnityWebRequest client = UnityWebRequest.Get(url);
            yield return client.SendWebRequest();
            data = client.downloadHandler.data;

            tex.LoadImage(data);
            Debug.Log(data.Length);
            Image.texture = tex;
        }
    }
}
