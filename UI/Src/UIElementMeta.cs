using System;
using System.Xml;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BrainBit.UI
{
	public delegate void UIElementBinding();
    public class UIElementMeta:IEquatable<UIElementMeta>, IEnumerable<UIElementMeta>
    {
		private UIElementMeta _parent;
        private string _tags;
        private GameObject _object;
		private XmlNode _node;
		private RectTransform _rect;
		private string _bindPath = null;
		public event UIElementBinding UpdateBinding;
		private List<UIElementMeta> _children = new List<UIElementMeta>();
		public bool ContinueRenderChildren = true; //Wasn't sure how else to toggle the render pattern.


		public UIElementMeta(GameObject Object, XmlNode Node, string Tags){
			this._init (Object, Node, Tags);
		}

		public UIElementMeta(GameObject Object, XmlNode Node)
		{
			this._init (Object, Node, "");
		}

		private void _init(GameObject Object, XmlNode Node, string Tags)
		{
			this._tags = Tags;
			this._object = Object;
			this._node = Node;
			
			this._rect = this._object.GetComponent<RectTransform> ();
			if (this._rect == null) {
				this._rect = this._object.AddComponent<RectTransform> ();
			}
		}


		public RectTransform Rect{
			get{
				return this._rect;
			}
		}

        public string Tags
        {
            get
            {
                return this._tags;
            }

			set{
				this._tags = value;
			}
        }

		public string BindPath{
			get{
				return this._bindPath;
			}

			set{
				this._bindPath = value;
			}
		}

        public GameObject Object
        {
            get
            {
                return this._object;
            }
        }

		public XmlNode Node{
			get{
				return this._node;
			}
		}

		public UIElementMeta AddChild(UIElementMeta child)
		{
			if(child != null)
			{
			if (!this._children.Contains (child)) {

				this._children.Add(child);
			}
			}
			return this;
		}

		public UIElementMeta Parent{
			get{
				return this._parent;
			}

			set{
				if(this._parent != null)
				{
					if(this._parent != value)
					{
						this._parent.RemoveChild(this);
					}
				}

				this._parent = value;
				this._parent.AddChild(this);
			}
		}

		public UIElementMeta RemoveChild(UIElementMeta child)
		{
			this._children.Remove (child);
			return this;
		}

		public int Count{
			get{
				return this._children.Count;
			}
		}

		public UIElementMeta ClearChildren()
		{
			Debug.Log (this._object.transform.childCount);
			this._children.ForEach (x => x.Delete ());
			this._children = new List<UIElementMeta> ();
			return this;
		}

		public void Delete()
		{
			if (this._parent != null) {
				this._parent.RemoveChild(this);
			}

			this._children.ForEach (x => x.Delete ());
			this._children = null;
			GameObject.Destroy (this._object);
		}

		public void Update()
		{
			if (this.UpdateBinding != null) {
				this.UpdateBinding();
			}

			//Debug.Log (this._children.Count);
			foreach (UIElementMeta child in this._children) {
				//Debug.Log ((child == null)+" "+this.Tags);
			if(child != null)
				{
				child.Update ();
				}
			}

		}

		#region IEquatable implementation

		public bool Equals (UIElementMeta other)
		{
			return this._object.GetInstanceID() == other._object.GetInstanceID();
		}

		#endregion

		#region IEnumerable implementation
		
		public IEnumerator<UIElementMeta> GetEnumerator ()
		{
			for (int i = 0; i < this._children.Count; i++) {
				yield return this._children[i];
			}
		}
		
		#endregion

		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator ()
		{
			for (int i = 0; i < this._children.Count; i++) {
				yield return this._children[i];
			}
		}
		#endregion
    }
}
