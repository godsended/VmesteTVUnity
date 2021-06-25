using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VmesteApp.UI;

namespace VmesteApp.Catalog
{
    public class FilmBlock : MonoBehaviour
    {
        public bool isPermanent;
        public string id;
        public RawImage Image;
        public Text Title, Describtion, Year, Genre;
        public CatalogPanel CatalogPanel;
        public void OnSearch()
        {
            if(isPermanent)
            {
                if (CatalogPanel.SearchBar.text != "")
                    gameObject.SetActive(false);
                else
                    gameObject.SetActive(true);
            }
            else
            {
                CatalogPanel.onSearch.RemoveListener(OnSearch);
                Destroy(gameObject);
            }
        }
        public void OpenFilmInfo()
        {
            UI.UI.Instance.FilmInfoPanel.Open(id);
        }
    }
}
