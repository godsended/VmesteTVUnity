using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VmesteApp.UI
{
    public class Item : MonoBehaviour
    {
        public ItemContainer Container;
        public bool isSelected;
        [SerializeField] private Animator animator;
        public void Initialize(ItemContainer container)
        {
            if(container!=null)
            {
                Container = container;
                Container.onItemSelected.AddListener(Deselect);
            }
        }
        public void Select()
        {
            if (!isSelected)
            {
                Container.onItemSelected.Invoke();
                isSelected = true;
                animator.SetTrigger("Select");
            }
        }
        public void Deselect()
        {
            if (isSelected)
            {
                isSelected = false;
                animator.SetTrigger("Deselect");
            }
        }
    }
}
