using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VmesteApp.Catalog 
{
    public class FilmsBlock : MonoBehaviour
    {
        [SerializeField] private List<FilmBlock> Films;
        [SerializeField] private List<FilmBlock> EmptyFilmBlocks;
        public Transform ContentTransform;
        public FilmBlock SimpleFilmBlockPrefab;
        public FilmBlock GetEmptyFilmBlock()
        {
            if (EmptyFilmBlocks.Count > 0)
            {
                FilmBlock fb = EmptyFilmBlocks[0];
                Films.Add(fb);
                EmptyFilmBlocks.RemoveAt(0);
                fb.gameObject.SetActive(true);
                return fb;
            }
            else
            {
                FilmBlock fb = Instantiate(SimpleFilmBlockPrefab, ContentTransform);
                Films.Add(fb);
                return fb;
            }
        }
        public void Clear()
        {
            while(Films.Count>0)
            {
                FilmBlock fb = Films[0];
                fb.gameObject.SetActive(false);
                EmptyFilmBlocks.Add(fb);
                Films.RemoveAt(0);
            }
        }
    }
}
