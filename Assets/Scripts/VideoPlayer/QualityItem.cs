using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VmesteApp.UI;

namespace VmesteApp.Rooms
{
    public class QualityItem : Item
    {
        public Text Name;
        public string URL;
        public int ID;
        public void ChangeQuaality()
        {
            UI.UI.Instance.VideoPlayerPanel.VideoPlayerLayout.SwitchQuality(ID);
        }
    }
}
