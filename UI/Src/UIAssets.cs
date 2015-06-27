using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BrainBit.UI
{
   
    public class UIAssets:MonoBehaviour
    {
        public static UIAssets Self;
        public List<Sprite> Images;
        public List<Font> Fonts;
        public List<Material> Materials;

        void Awake()
        {
            if(Self == null)
            {
                Self = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}
