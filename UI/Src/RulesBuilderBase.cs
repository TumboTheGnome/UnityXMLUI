using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BrainBit.UI
{
	public class RulesBuilderBase : IElementBuilder
	{
		protected Dictionary<string, builderRules> propertyBuilderConfig = new Dictionary<string, builderRules> ();
		protected Dictionary<string, List<string>> elements = new Dictionary<string, List<string>> ();

		protected class builderRules
		{
			private List<string> _reqFields;
			private UIElementBuilder _builder;

			public builderRules (List<string> RequiredFields, UIElementBuilder Builder)
			{
				this._builder = Builder;
				this._reqFields = RequiredFields;

			}

			public List<string> Fields {
				get {
					return this._reqFields;
				}
			}

			public UIElementBuilder PropertyBuilder {
				get {
					return this._builder;
				}
			}
		}

		public RulesBuilderBase ()
		{
			//Registering element components. 
			this.propertyBuilderConfig.Add ("position", new builderRules (new List<string> (){"min", "max"}, this.position));
			this.propertyBuilderConfig.Add ("textMaterial", new builderRules (new List<string> (){"material"}, this.textMaterial));
			this.propertyBuilderConfig.Add ("textColor", new builderRules (new List<string> (){"color"}, this.textColor));
			this.propertyBuilderConfig.Add ("bindView", new builderRules (new List<string> (), this.bindView));
			this.propertyBuilderConfig.Add ("bindText", new builderRules (new List<string> (){"bind"}, this.bindText));
			this.propertyBuilderConfig.Add ("imgMaterial", new builderRules (new List<string> (){"material"}, this.imgMaterial));
			this.propertyBuilderConfig.Add ("imgColor", new builderRules (new List<string> (){"color"}, this.imageColor));
			this.propertyBuilderConfig.Add ("imgBackground", new builderRules (new List<string> (){"background"}, this.imgBackground));
			this.propertyBuilderConfig.Add ("imgRenderMode", new builderRules (new List<string> (){"renderMode"}, this.imageRenderMode));
			this.propertyBuilderConfig.Add ("foreachChild", new builderRules (new List<string> (){"foreach"}, this.foreachChild));
			this.propertyBuilderConfig.Add ("textFont", new builderRules (new List<string> (){"font"}, this.textFont));
			this.propertyBuilderConfig.Add ("textFontSize", new builderRules (new List<string> (){"size"}, this.textFontSize));
			this.propertyBuilderConfig.Add ("click", new builderRules (new List<string> (){ "onClick" }, this.bindClick));
			this.propertyBuilderConfig.Add ("show", new builderRules (new List<string> (){ "show" }, this.show));

			//Regestering element config
			this.elements.Add ("div", new List<string> (){"position", "imgMaterial", "imgColor", "imgBackground", "imgRenderMode", "bindView", "foreachChild", "show"});
			this.elements.Add ("text", new List<string> (){"position", "textMaterial", "textColor", "bindText", "textFont", "textFontSize", "show"}); 
			this.elements.Add ("button", new List<string> (){"position", "imgMaterial", "imgColor", "imgRenderMode", "imgBackground", "click", "show"});
		}

		public List<UIEllement> Build ()
		{

			List<UIEllement> r = new List<UIEllement> ();

			foreach (KeyValuePair<string,List<string>> config in this.elements) {
				UIEllement ellement = new UIEllement (config.Key);

				foreach (string property in config.Value) {
					if (this.propertyBuilderConfig.ContainsKey (property)) { 
						ellement.Fields.AddRange (this.propertyBuilderConfig [property].Fields);
						ellement.Builder += this.propertyBuilderConfig [property].PropertyBuilder;
					}
				}
				r.Add (ellement);
			}

			return r;
		}

		/* Start Binding */ 
		protected void bindView (UI render, GameObject self, UIElementMeta meta, Dictionary<string, string> attributes, object context)
		{
			if (context.GetType ().GetInterfaces ().Contains (typeof(IUIBindable)) && meta.Node.ParentNode == null) {
				IUIBindable bindable = (IUIBindable)context;
				bindable.View = meta;
			}
		}
		protected void bindText (UI render, GameObject self, UIElementMeta meta, Dictionary<string, string> attributes, object context)
		{
			if (attributes.ContainsKey ("bind")) {
				Text txt = this.getText (self, meta.Node);

				string bind = attributes ["bind"];

				//If the bind is set to value, we presume the super structure contianing the ellement is handling the setting of the bindpath. Otherwise we pull the bind path.
				if (bind != "value") {
					meta.BindPath = bind;
				}

				UIElementBinding binding = () => {

					if(meta.BindPath != null)
					{
					string propName = null;
					int propIndex = -1;

					//Array indexed values are refed using #name:#index
					int pos = meta.BindPath.IndexOf (":");
					if (pos != -1) {
						propName = meta.BindPath.Substring (0, pos);
						propIndex = Convert.ToInt32 (meta.BindPath.Substring (pos+1, meta.BindPath.Length - (pos + 1)));
					} else {
						propName = meta.BindPath;
					}

					Type type = context.GetType ();
					PropertyInfo info = type.GetProperty (propName);
					if (info != null) {
						if (propIndex == -1) {
							txt.text = info.GetValue (context, null).ToString ();
						} else if (info.PropertyType.GetInterfaces ().Contains (typeof(IList))) { // Because I'm lazey, currently only implementing for lists
							IList value = (IList)info.GetValue (context, null);
							txt.text = value [propIndex].ToString ();
						} else {
							txt.text = meta.Node.Value;
						}
					} else {
						txt.text = meta.Node.Value;
					}
					}
				};
		
	
				binding.Invoke ();
				meta.UpdateBinding += binding;
			}
		}

		protected void bindImg (UI render, GameObject self, UIElementMeta meta, Dictionary<string, string> attributes, object context)
		{
			
		}

		protected void bindClick(UI render, GameObject self, UIElementMeta meta, Dictionary<string, string> attributes, object context)
		{
			if (attributes.ContainsKey ("onClick")) {
				Button btn = this.getButton (self, meta.Node);


				btn.onClick.AddListener (() => {
					Type type = context.GetType();
					MethodInfo info = type.GetMethod(attributes["onClick"]);
					if(info != null)
					{
						info.Invoke(context, null);	
					}
				});
			}
		}
			
		/*End Binding*/

		/*Start Text*/
		protected void textMaterial (UI render, GameObject self, UIElementMeta meta, Dictionary<string, string> attributes, object context)
		{
			if (attributes.ContainsKey ("material") && UIAssets.Self != null) {
				Text txt = this.getText (self, meta.Node);
				txt.material = UIAssets.Self.Materials.Find (x => x.name == attributes ["material"]);
			}
		}

		protected void textColor (UI render, GameObject self, UIElementMeta meta, Dictionary<string, string> attributes, object context)
		{
			if (attributes.ContainsKey ("color")) {
				Text txt = this.getText (self, meta.Node);
				txt.color = readColor (attributes ["color"]);
			}
		}

		protected void textFont (UI render, GameObject self, UIElementMeta meta, Dictionary<string, string> attributes, object context)
		{
			if (attributes.ContainsKey ("font") && UIAssets.Self != null) {
				Text txt = this.getText (self, meta.Node);

				Font font = UIAssets.Self.Fonts.Find (x => x.name == attributes ["font"]);
				if (font != null) {

					txt.font = font;
				}
			}
		}

		protected void textFontSize (UI render, GameObject self, UIElementMeta meta, Dictionary<string, string> attributes, object context)
		{
			if (attributes.ContainsKey ("size")) {
				Text txt = this.getText (self, meta.Node);
				txt.fontSize = Convert.ToInt32 (attributes ["size"]);
			}
		}

		/*End Text*/

		/*Start Image*/

		protected void imageColor (UI render, GameObject self, UIElementMeta meta, Dictionary<string, string> attributes, object context)
		{
			if (attributes.ContainsKey ("color")) {
				Image img = this.getImage (self, meta.Node);
				img.color = this.readColor (attributes ["color"]);
			}
		}

		protected void imgBackground (UI render, GameObject self, UIElementMeta meta, Dictionary<string, string> attributes, object context)
		{
			if (attributes.ContainsKey ("background") && UIAssets.Self != null) {
				Image img = this.getImage (self, meta.Node);
				
				if (attributes ["background"].IndexOf ("$") == 0) {
					
				} else {
					
					Sprite s = UIAssets.Self.Images.Find (x => x.name == attributes ["background"]);
					if (s != null) {
						img.sprite = s;
					}
				}
			}
		}

		protected void imageRenderMode (UI render, GameObject self, UIElementMeta meta, Dictionary<string, string> attributes, object context)
		{

			if (attributes.ContainsKey ("renderMode")) {
				Image img = this.getImage (self, meta.Node);
				img.type = (Image.Type)Enum.Parse (typeof(Image.Type), attributes ["renderMode"]);
			}
		}

		protected void imgMaterial (UI render, GameObject self, UIElementMeta meta, Dictionary<string, string> attributes, object context)
		{
			if (attributes.ContainsKey ("material") && UIAssets.Self != null) {
				Image img = this.getImage (self, meta.Node);
				Material m = UIAssets.Self.Materials.Find (x => x.name == attributes ["material"]);
				if (m != null) {
					img.material = m;
				}
			}
		}
		/*End Image*/

		/*Verbs*/

		protected void show(UI render, GameObject self, UIElementMeta meta, Dictionary<string, string> attributes, object context)
		{
			if (attributes.ContainsKey ("show")) {
				meta.UpdateBinding += () => {
					Type type = context.GetType();
					PropertyInfo info = type.GetProperty(attributes["show"]);
					if(info != null)
					{
						if(info.PropertyType == typeof(bool))
						{
							self.SetActive((bool)info.GetValue(context, null));
						}
					}
				};
			}
		}

		protected void foreachChild (UI render, GameObject self, UIElementMeta meta, Dictionary<string, string> attributes, object context)
		{

			if (attributes.ContainsKey ("foreach")) {

				UIElementBinding binding = () => {

					meta.ContinueRenderChildren = false;

					string propName = attributes ["foreach"];
					Type t = context.GetType ();
					PropertyInfo prop = t.GetProperty (propName);
			
					if (prop != null) {
						if (prop.PropertyType.GetInterfaces ().Contains (typeof(IList))) {
							IList data = (IList)prop.GetValue (context, null);

							/* Create needed ellements */
							if (meta.Object.transform.childCount < data.Count) {
								int needed = data.Count - meta.Object.transform.childCount;
								for(int i = 0; i< needed; i++){
									foreach (XmlNode child in meta.Node.ChildNodes) {
										meta.AddChild (render.Render (child, self.transform, context));
									}
								}
							}

							//Toggle on ellements that we'll be using, disable the ones we won't be using
							int ellementsNeeded = 0;
							List<UIElementMeta> active = new List<UIElementMeta> ();
							foreach (UIElementMeta child in meta) {
								//Enable ellements 
								if (ellementsNeeded < data.Count) {
									child.Object.SetActive (true);
									child.BindPath = propName;
									active.Add (child);
								} else { //Disable elements
									child.Object.SetActive (false);
								}
								ellementsNeeded++;
							}

							//Layout enabled ellements
							this.groupLayout (active);
						}
					}
			
			
				};

				binding.Invoke ();
				meta.UpdateBinding += binding;
			}
		}

		protected void groupLayout (List<UIElementMeta> meta)
		{
			if (meta != null) {
				for (int i = 0; i< meta.Count; i++) {

					Vector2 min = Vector2.zero;
					Vector2 max = Vector2.zero;

					string minRaw = meta [i].Node.Attributes ["min"].Value;
					if (minRaw != null) {
						min = this.readCord (minRaw);
					}

					string maxRaw = meta [i].Node.Attributes ["max"].Value;
					if (maxRaw != null) {
						max = this.readCord (maxRaw);
					}

					Vector2 dif = new Vector2 (min.x * i, max.y * i); 
				
					meta [i].Rect.anchorMin = min + dif;
					meta [i].Rect.anchorMax = max + dif; 

					meta [i].Rect.localScale = Vector3.one;
					meta [i].Rect.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, 1);
					meta [i].Rect.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, 1);
					meta [i].Rect.offsetMin = Vector2.zero;
					meta [i].Rect.offsetMax = Vector2.zero;
					meta [i].BindPath += ":" + i;
					meta [i].Update ();
				}
			}
		}


		//Calculate the positioning of an ellement
		protected void position (UI renderer, GameObject self, UIElementMeta meta, Dictionary<string, string> attributes, object context)
		{
			Vector2 min = Vector2.zero;
			Vector2 max = Vector2.one;

			//If the min and max fields have been set use them, otherwise default to fullscreen
			if (attributes.ContainsKey ("min")) {
				min = readCord (attributes ["min"]);
			}

			if (attributes.ContainsKey ("max")) {
				max = readCord (attributes ["max"]);
			}

			meta.Rect.anchorMin = min;
			meta.Rect.anchorMax = max;

			meta.Rect.localScale = Vector3.one;
			meta.Rect.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, 1);
			meta.Rect.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, 1);
			meta.Rect.offsetMin = Vector2.zero;
			meta.Rect.offsetMax = Vector2.zero;				
		}

		//Parse points, in decimal or fraction form to, vector2 
		protected Vector2 readCord (string input)
		{
			int split = input.IndexOf (',');
			string strX = input.Substring (0, split);
			string strY = input.Substring (split, input.Length - split);

			float x = 0;
			float y = 0;

			split = strX.IndexOf ('%');
			if (split != -1) {
				string value = strX.Substring (0, strX.Length - split);
				x = (float)Convert.ToDouble (value) / 100;
			} else {
				x = (float)Convert.ToDouble (strX);
			}

			split = strY.IndexOf ('%');
			if (split != -1) {
				string value = strY.Substring (0, strY.Length - split);
				y = (float)Convert.ToDouble (value) / 100;
			} else {
				y = (float)Convert.ToDouble (strY);
			}

			return new Vector2 (x, y);
		}

		protected Color readColor (string input)
		{
			if (input.IndexOf ('#') == 0 && input.Length == 7) {
				float r = (float)Convert.ToInt32 (input.Substring (1, 2), 16);
				float g = (float)Convert.ToInt32 (input.Substring (3, 2), 16);
				float b = (float)Convert.ToInt32 (input.Substring (5, 2), 16);
				Color c = new Color (r / 255, g / 255, b / 255);
				return c;
			} else {

				Color c = Color.black;

				switch (input.ToLower ()) {
				case "black":
					c = Color.black;
					break;
				case "blue":
					c = Color.blue;
					break;
				case "clear":
					c = Color.clear;
					break;
				case "cyan":
					c = Color.cyan;
					break;
				case "gray":
					c = Color.gray;
					break;
				case "green":
					c = Color.green;
					break;
				case "grey":
					c = Color.grey;
					break;
				case "magenta":
					c = Color.magenta;
					break;
				case "red":
					c = Color.red;
					break;
				case "white":
					c = Color.white;
					break;
				case "yellow":
					c = Color.yellow;
					break;
				}

				return c;
			}
		}

		protected Text getText (GameObject g, XmlNode node)
		{
			Text txt = g.GetComponent<Text> ();
			if (txt == null) {
				txt = g.AddComponent<Text> ();

				txt.text = node.InnerText;
			}
			return txt;
		}

		protected Image getImage (GameObject g, XmlNode node)
		{
			Image img = g.GetComponent<Image> ();
			if (img == null) {
				img = g.AddComponent<Image> ();
			}
			return img;
		}

		protected Button getButton(GameObject g, XmlNode node)
		{
			Button btn = g.GetComponent<Button> ();
			if (btn == null) {
				btn = g.AddComponent<Button> ();

				Navigation n = new Navigation ();
				n.mode = Navigation.Mode.None;
				btn.navigation = n;
			}

			return btn;

		}
	}
}
