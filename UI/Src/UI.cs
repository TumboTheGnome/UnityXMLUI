using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace BrainBit.UI
{
	public class UI
	{
		private List<UIElementMeta> _ellements = new List<UIElementMeta> ();
		private Dictionary<string, UIEllement> _handlers = new Dictionary<string, UIEllement> ();
		private RectTransform _root;
		private Canvas _canvas;
		private EventSystem _events;
		private UIElementMeta _metaRoot;
		private List<UIElementMeta> _selection;
        
		public UI ()
		{
			this._init ();
		}

		public UI (IElementBuilder builder)
		{
			this._init ();
			if (builder != null) {
				foreach (UIEllement ellement in builder.Build()) {
					this.AddRule (ellement);
				}
			}
		}

		private void _init ()
		{
			GameObject k = new GameObject ("UI Root");
			this._canvas = k.AddComponent<Canvas> ();
			this._root = k.GetComponent<RectTransform> ();
			k.AddComponent<CanvasScaler> ();
			k.AddComponent<GraphicRaycaster> ();
			
			GameObject events = new GameObject ("UI Events");
			this._events = events.AddComponent<EventSystem> ();
			events.AddComponent<StandaloneInputModule> ();
			events.AddComponent<TouchInputModule> ();
		}

		public Canvas Canvas {
			get {
				return this._canvas;
			}
		}

		public EventSystem Events {
			get {
				return this._events;
			}
		}

		public List<UIElementMeta> Get {
			get {
				return this._selection;
			}
		}

		public UI LoadAdditive (string xml, IUIBindable context)
		{
			this._render (xml, context);
			return this;

		}

		public UI LoadAdditive (string xml)
		{
			this._render (xml, null);
			return this;
		}

		public UI Load (string xml, IUIBindable context)
		{
			this.Clear ();
			this._render (xml, context);
			return this;
		}

		public UI Load (string xml)
		{
			this.Clear ();
			this._render (xml, null);
			return this;
		}

		public UI Clear ()
		{
			if (this._root.childCount > 0) {
				foreach (Transform t in this._root) {
					GameObject.Destroy (t.gameObject);
				}
			}
			return this;
		}

		public UI Select (string search)
		{
			this._selection = this._ellements.FindAll (x => x.Tags.Contains (search));
			return this;
		}

		public UI AddRule (UIEllement ellement)
		{
			if (this._handlers.ContainsKey (ellement.Name)) {
				this._handlers [ellement.Name] = ellement;
			} else {
				this._handlers.Add (ellement.Name, ellement);
			}
			return this;
		}

		public UIElementMeta Render (XmlNode node, Transform parent, object context)
		{
			return this._renderNode (node, parent, context);
		}

		private void _render (string xml, object context)
		{
			if (!String.IsNullOrEmpty (xml)) {
				this._selection = new List<UIElementMeta> ();
				UIElementMeta root = null;

				XmlReader reader = XmlReader.Create (new StringReader (xml));
				XmlDocument document = new XmlDocument ();
				while (reader.Read()) {
					if (!String.IsNullOrEmpty (reader.Name)) {
						root = this._renderNode (document.ReadNode (reader), this._root, context);
					}
				}

				root.Update ();
			}
		}

		private UIElementMeta _renderNode (XmlNode node, Transform parent, object context)
		{
			if (this._handlers.ContainsKey (node.Name)) {
				UIElementMeta k = _handlers [node.Name].Make (this, node, parent, context);
				this._selection.Add (k);
				this._ellements.Add (k);

				if (k.ContinueRenderChildren) {
					foreach (XmlNode childNode in node.ChildNodes) {
						if (k.Object != null) {
							k.AddChild (_renderNode (childNode, k.Object.transform, context));
						}
					}
				}
				return k;
			} else {
				return null;
			}
		}
	}
}
