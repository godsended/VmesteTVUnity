using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using VmesteApp.Catalog;

namespace VmesteApp.UI
{
    public class FilmInfoPanel : Panel
    {
        private string filmID;
        public UnityEvent onInfoOpen;
        public RawImage Image;
        public Text Title, Info, Discription;
        public Slider Rating;
        public VerticalLayoutGroup ContentLayout;
        public Comment CommentPrefab;
        private void Awake()
        {
            onInfoOpen = new UnityEvent();
        }
        public async void Open(string id)
        {
            filmID = id;
            onInfoOpen.Invoke();
            onInfoOpen.RemoveAllListeners();
            Show();
            FilmInfo.FilmResult res = await FilmInfo.GetInfo(id);
            Rating.value = float.Parse(res.data.kp_raiting.Replace('.', ','));
            Debug.Log(res.data.kp_raiting);
            Title.text = res.data.title;
            Discription.text = res.data.description;
            Info.text = $"Год выхода: {res.data.year}\n\nСтрана: {res.data.country}\n\nЖанр: ";
            for (int i = 0; i < res.data.genres.Length; i++)
            {
                Info.text += res.data.genres[i].name + ", ";
            }
            Info.text = Info.text.Substring(0, Info.text.Length - 2);
            Info.text += $"\n\nРежисёр: ";
            for (int i = 0; i < res.data.producer.Length; i++)
            {
                Info.text += res.data.producer[i] + ", ";
            }
            Info.text = Info.text.Substring(0, Info.text.Length - 2);
            Info.text += $"\n\nАктёры: ";
            for (int i = 0; i < res.data.actors.Length; i++)
            {
                Info.text += res.data.actors[i] + ", ";
            }
            Info.text = Info.text.Substring(0, Info.text.Length - 2);
            Info.text += $"\n\nРейтинг Кинопоиска: <b>{res.data.kp_raiting}</b>";
            for(int i = 0;i<res.data.comments.Length;i++)
            {
                Comment c = Instantiate(CommentPrefab, ContentLayout.transform);
                c.Name.text = res.data.comments[i].username;
                c.Text.text = res.data.comments[i].comment;
                onInfoOpen.AddListener(delegate { Destroy(c.gameObject); });
            }
            for (int i = 0;i<10;i++)
            {
                if (Cache.IsImageSaved(id))
                {
                    Texture2D tex = new Texture2D(1, 1);
                    tex.LoadImage(Cache.LoadImage(id));
                    Image.texture = tex;
                    break;
                }
                else
                {
                    await System.Threading.Tasks.Task.Delay(1000);
                }
            }
            await System.Threading.Tasks.Task.Yield();
            ContentLayout.childForceExpandHeight = !ContentLayout.childForceExpandHeight;
           
        }
        public void OpenRoom()
        {
            UI.Instance.VideoPlayerPanel.Show();
            UI.Instance.VideoPlayerPanel.Open(filmID);
        }
    }
}
