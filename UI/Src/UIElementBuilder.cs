using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

namespace BrainBit.UI
{
	public delegate void UIElementBuilder (UI ui, GameObject self,UIElementMeta meta,object context);

	public class UIEllement
	{
		private string _name;
		private List<string> _fields = new List<string> ();

		public event UIElementBuilder Builder;

		public UIEllement (string Name)
		{
			this._name = Name;
		}


		//Name/Id of ellement builder
		public string Name {
			get {
				return this._name;
			}
		}

		public List<string> Fields {

			get {
				return this._fields;
			}
			set {
				this._fields = value;
			}
		}


		//Creates ellement
		public UIElementMeta Make (UI ui, XmlNode node, Transform parent, object context)
		{
			//Filter Attributes
			string tags = this._name;

			//Creating object.
			GameObject g = new GameObject (this._name);
			g.transform.parent = parent;

			UIElementMeta meta  = new UIElementMeta (g,node, tags);

			if (this.Builder != null) {
				this.Builder (ui, g, meta, context);
			}

			return meta;
		}
	}

}
