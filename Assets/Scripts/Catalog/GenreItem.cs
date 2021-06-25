using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VmesteApp.UI;

namespace VmesteApp.Catalog
{
    public class GenreItem : Item
    {
        public Text Title;
        public CatalogPanel Catalog;
        public string id;
        public void OnClick()
        {
            Catalog.LoadPopular(id);
            Catalog.LoadNew(id);
        }
    }
}