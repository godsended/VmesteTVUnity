using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VmesteApp.UI
{
    public class ItemContainer : MonoBehaviour
    {
        public List<Item> Items;
        public UnityEvent onItemSelected;
        public Transform ItemsContent;
        private void Awake()
        {
            onItemSelected = new UnityEvent();
        }
        private void Start()
        {
            for(int i = 0;i<Items.Count;i++)
            {
                Items[i].Initialize(this);
            }
        }
    }
}
