using System;
using UnityEngine;

namespace BrainBit.UI
{
    public class UIEllementMeta
    {
        private string _tags;
        private GameObject _object;

        public UIEllementMeta(string Tags, GameObject Object)
        {
            this._tags = Tags;
            this._object = Object;
        }

        public string Tags
        {
            get
            {
                return this._tags;
            }
        }

        public GameObject Object
        {
            get
            {
                return this._object;
            }
        }
    }
}
