using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Networking;
using VmesteApp.Catalog;

namespace VmesteApp.UI
{
    public class CatalogPanel : Panel
    {
        public UnityEvent onSearch, onGenreChanged;
        public InputField SearchBar;
        [SerializeField] private FilmBlock FilmBlockPrefab, SimpleFilmBlockPrefab;
        [SerializeField] private ItemContainer GenresContainer;
        [SerializeField] private Transform ContentTransform;
        [SerializeField] private FilmsBlock PopularFilms, NewFilms;
        [SerializeField] private GenreItem GenreItemPrefab;
        [SerializeField] private Text NewFilmsText;
        [SerializeField] private Button SiteSearchButton;
        private void Awake()
        {
            onSearch = new UnityEvent();
            onGenreChanged = new UnityEvent();
        }
        private void Start()
        {
            onSearch.AddListener(OnSearch);
            onGenreChanged.AddListener(PopularFilms.Clear);
            onGenreChanged.AddListener(NewFilms.Clear);
            Init();
        }
        private void OnSearch()
        {
            if (SearchBar.text != "")
            {
                PopularFilms.gameObject.SetActive(false);
                NewFilmsText.gameObject.SetActive(false);
            }
            else
            {
                PopularFilms.gameObject.SetActive(true);
                NewFilmsText.gameObject.SetActive(true);
                SiteSearchButton.gameObject.SetActive(false);
            }
        }
        public async void LoadGenres()
        {
            Genres.GenresResult ans = await Genres.Get();
            Debug.Log(ans.data);
            if (ans.status == "success")
            {
                for (int i = 0; i < ans.data.Length; i++)
                {
                    GenreItem gi = Instantiate(GenreItemPrefab, GenresContainer.ItemsContent);
                    gi.Title.text = ans.data[i].name;
                    gi.id = ans.data[i].id;
                    gi.Initialize(GenresContainer);
                    gi.Catalog = this;
                    GenresContainer.Items.Add(gi);
                }
            }
        }
        public async void LoadPopular(string genre)
        {
            PopularFilms.Clear();
            Search.FilmResult ans = await Search.PopularFilms(genre);
            Debug.Log(ans.data);
            if (ans.status == "success")
            {
                for (int i = 0; i < ans.data.Length; i++)
                {
                    Debug.Log(ans.data[i].title);
                    FilmBlock fb = PopularFilms.GetEmptyFilmBlock();
                    fb.Title.text = ans.data[i].title;
                    fb.isPermanent = true;
                    fb.CatalogPanel = this;
                    fb.id = ans.data[i].kinopoisk_id;
                    if (Cache.IsImageSaved(ans.data[i].image_url))
                    {
                        Texture2D tex = new Texture2D(1, 1);
                        tex.LoadImage(Cache.LoadImage(ans.data[i].kinopoisk_id));
                        fb.Image.texture = tex;
                    }
                    else
                    {
                        LoadAndApplyImage(fb.Image, ans.data[i].image_url, ans.data[i].kinopoisk_id);
                    }
                }
            }
        }

        public async void LoadNew(string genre)
        {
            NewFilms.Clear();
            Search.FilmResult ans = await Search.NewFilms(genre);
            Debug.Log(ans.data);
            if (ans.status == "success")
            {
                for (int i = 0; i < ans.data.Length; i++)
                {
                    Debug.Log(ans.data[i].title);
                    FilmBlock fb = NewFilms.GetEmptyFilmBlock();
                    fb.Title.text = ans.data[i].title;
                    fb.Describtion.text = ans.data[i].description;
                    fb.Year.text = ans.data[i].year;
                    fb.Genre.text = ans.data[i].genre;
                    fb.isPermanent = true;
                    fb.CatalogPanel = this;
                    fb.id = ans.data[i].kinopoisk_id;
                    if (Cache.IsImageSaved(ans.data[i].image_url))
                    {
                        Texture2D tex = new Texture2D(1, 1);
                        tex.LoadImage(Cache.LoadImage(ans.data[i].kinopoisk_id));
                        fb.Image.texture = tex;
                    }
                    else
                    {
                        LoadAndApplyImage(fb.Image, ans.data[i].image_url, ans.data[i].kinopoisk_id);
                    }
                    onSearch.AddListener(fb.OnSearch);
                }
            }
            NewFilmsText.transform.SetSiblingIndex(1);
            NewFilmsText.gameObject.SetActive(true);
        }
        public async void SearchFilm()
        {
            string q = SearchBar.text;
            Search.FilmResult ans = await Search.Film(q);
            Debug.Log(ans.data);
            if (ans.status == "success")
            {
                onSearch.Invoke();
                for (int i = 0; i < ans.data.Length; i++)
                {
                    Debug.Log(ans.data[i].title);
                    FilmBlock fb = Instantiate(FilmBlockPrefab, ContentTransform);
                    fb.Title.text = ans.data[i].title;
                    fb.Describtion.text = ans.data[i].description;
                    fb.Year.text = ans.data[i].year;
                    fb.Genre.text = ans.data[i].genre;
                    fb.isPermanent = false;
                    fb.CatalogPanel = this;
                    fb.id = ans.data[i].kinopoisk_id;
                    if (Cache.IsImageSaved(ans.data[i].kinopoisk_id))
                    {
                        Texture2D tex = new Texture2D(1, 1);
                        tex.LoadImage(Cache.LoadImage(ans.data[i].kinopoisk_id));
                        fb.Image.texture = tex;
                    }
                    else
                    {
                        LoadAndApplyImage(fb.Image, ans.data[i].image_url, ans.data[i].kinopoisk_id);
                    }
                    onSearch.AddListener(fb.OnSearch);
                }
                SiteSearchButton.gameObject.SetActive(true);
                SiteSearchButton.onClick.RemoveAllListeners();
                SiteSearchButton.onClick.AddListener(delegate { Application.OpenURL("https://vmeste-tv.site/search/?text=" + q); });
                SiteSearchButton.transform.SetAsLastSibling();
            }
        }
        private async void LoadAndApplyImage(RawImage img, string path, string name)
        {
            byte[] data;
            UnityWebRequest w = UnityWebRequest.Get(path);

            w.SendWebRequest();

            while (w.result == UnityWebRequest.Result.InProgress)
            {
                await Task.Yield();
            }

            data = w.downloadHandler.data;

            Cache.SaveImage(name, data);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(data);

            img.texture = tex;
        }
    }

}
