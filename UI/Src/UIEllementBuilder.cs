using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

namespace BrainBit.UI
{
	public delegate void UIEllementBuilder (UI ui, GameObject self,UIEllementMeta meta,Dictionary<string, string> attributes,object context);

	public class UIEllement
	{
		private string _name;
		private List<string> _fields = new List<string> ();

		public event UIEllementBuilder Builder;

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
		public UIEllementMeta Make (UI ui, XmlNode node, Transform parent, object context)
		{
			//Filter Attributes
			string tags = this._name;
			Dictionary<string, string> attrs = new Dictionary<string, string> ();
			foreach (XmlAttribute field in node.Attributes) {

				if(field.Name.ToLower() == "id")
				{
					tags += " "+field.Value;
				}else if (this._fields.Contains (field.Name)) {
					if (!attrs.ContainsKey (field.Name)) {
						attrs.Add (field.Name, field.Value);
					}
				}
			}
            
			//Creating object.
			GameObject g = new GameObject (this._name);
			g.transform.parent = parent;

			UIEllementMeta meta  = new UIEllementMeta (g,node, tags);

			if (this.Builder != null) {
				this.Builder (ui, g, meta, attrs, context);
			}

			return meta;
		}
	}

}
