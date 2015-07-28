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
		protected Dictionary<string, UIElementBuilder> propertyBuilderConfig = new Dictionary<string, UIElementBuilder> ();
		protected Dictionary<string, List<string>> elements = new Dictionary<string, List<string>> ();



		public RulesBuilderBase ()
		{


			//Registering element components. 
			this.propertyBuilderConfig.Add ("position", this.position);
			this.propertyBuilderConfig.Add ("textMaterial", this.textMaterial);
			this.propertyBuilderConfig.Add ("textColor", this.textColor);
			this.propertyBuilderConfig.Add ("bindView", this.attachView);
			this.propertyBuilderConfig.Add ("bindText", this.bindText);
			this.propertyBuilderConfig.Add ("imgMaterial", this.imgMaterial);
			this.propertyBuilderConfig.Add ("imgColor", this.imageColor);
			this.propertyBuilderConfig.Add ("imgBackground",this.imgBackground);
			this.propertyBuilderConfig.Add ("imgRenderMode", this.imageRenderMode);
			this.propertyBuilderConfig.Add ("foreachChild", this.foreachChild);
			this.propertyBuilderConfig.Add ("textFont", this.textFont);
			this.propertyBuilderConfig.Add ("textFontSize", this.textFontSize);
			this.propertyBuilderConfig.Add ("click", this.bindClick);
			this.propertyBuilderConfig.Add ("show", this.show);
			this.propertyBuilderConfig.Add ("imgBind", this.bindImg);
			this.propertyBuilderConfig.Add ("txtDefault", this.txtDefaultValue);
			this.propertyBuilderConfig.Add ("sldrInit", this.sliderInit);
			this.propertyBuilderConfig.Add ("sldrBind", this.bindSlider);
			this.propertyBuilderConfig.Add ("dragable", this.dragInit);
			this.propertyBuilderConfig.Add ("dragRecieve", this.dragRecieveInit);
			this.propertyBuilderConfig.Add ("dragRecieveBind", this.dragRecieveBind);
			this.propertyBuilderConfig.Add ("divBind", this.bindDiv);


			//Regestering element config
			this.elements.Add ("div", new List<string> (){"position", "imgMaterial", "imgColor", "imgBackground", "imgRenderMode", "bindView", "divBind", "foreachChild", "show", "imgBind", "dragable", "dragRecieve", "dragRecieveBind"});
			this.elements.Add ("text", new List<string> (){"txtDefault","position", "textMaterial", "textColor", "bindText", "textFont", "textFontSize", "show"}); 
			this.elements.Add ("button", new List<string> (){"position", "imgMaterial", "imgColor", "imgRenderMode", "imgBackground", "click", "show", "imgBind"});
			this.elements.Add ("slider", new List<string> (){"position", "sldrInit", "imgMaterial", "imgColor", "imgBackground", "imgRenderMode", "imgBind", "sldrBind"});
		}

		public List<UIEllement> Build ()
		{

			List<UIEllement> r = new List<UIEllement> ();

			foreach (KeyValuePair<string,List<string>> config in this.elements) {
				UIEllement ellement = new UIEllement (config.Key);

				foreach (string property in config.Value) {
					if (this.propertyBuilderConfig.ContainsKey (property)) { 
						ellement.Builder += this.propertyBuilderConfig [property];
					}
				}
				r.Add (ellement);
			}

			return r;
		}

		/* Start Binding */ 
		protected void attachView (UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (context.GetType ().GetInterfaces ().Contains (typeof(IUIBindable)) && meta.Node.ParentNode == null) {
				IUIBindable bindable = (IUIBindable)context;
				bindable.View = meta;
			}
		}

		protected void bindDiv(UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (meta.Node.Attributes ["bind"] != null) {	

				UIElementBinding bind = ()=>{
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
						 if (info.PropertyType.GetInterfaces ().Contains (typeof(IList)) && propIndex != -1) { // Because I'm lazey, currently only implementing for lists
							IList value = (IList)info.GetValue (context, null);
							meta.Data = value [propIndex];
							return;
						}
					} 

					meta.Data = null;
				}
				};
				meta.UpdateBinding += bind;
			}
		}

		protected void bindText (UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (meta.Node.Attributes["bind"] != null) {

				string bind = meta.Node.Attributes ["bind"].Value;

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
							meta.Text.text = info.GetValue (context, null).ToString ();
						} else if (info.PropertyType.GetInterfaces ().Contains (typeof(IList))) { // Because I'm lazey, currently only implementing for lists
							IList value = (IList)info.GetValue (context, null);
							meta.Text.text = value [propIndex].ToString ();
						} else {
							meta.Text.text = meta.Node.Value;
						}
					} else {
						meta.Text.text = meta.Node.Value;
					}
					}
				};
		
	
				binding.Invoke ();
				meta.UpdateBinding += binding;
			}
		}

		protected void bindImg (UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (meta.Node.Attributes["bindImage"] != null) {
				meta.Data = context;
				UIElementBinding binding = ()=>{
				Type type = context.GetType();
				PropertyInfo info = type.GetProperty(meta.Node.Attributes["bindImage"].Value);

				if(info != null)
				{
					if(info.PropertyType == typeof(Sprite))
					{
						meta.Image.sprite = (Sprite)info.GetValue(context, null);

					}
				}
				};

				binding.Invoke();
				meta.UpdateBinding += binding;
			}
		}

		protected void bindSlider(UI render, GameObject self, UIElementMeta meta, object context)
		{

			if (meta.Node.Attributes ["bind"] != null) {
			
				meta.Data = context;
				UIElementBinding binding = ()=>{

					Type type = context.GetType();
					PropertyInfo info = type.GetProperty(meta.Node.Attributes["bind"].Value);
					if(info != null)
					{
						float value = 0;
						if(info.PropertyType == typeof(float))
						{
							value = (float)info.GetValue(context, null);
						}

						meta.Slider.normalizedValue = value;
					}

				};

				binding.Invoke();
				meta.UpdateBinding += binding;
			
			}
		}

		protected void bindClick(UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (meta.Node.Attributes["onClick"] != null) {
				meta.Data = context;
				meta.Button.onClick.AddListener (() => {
					Type type = context.GetType();
					MethodInfo info = type.GetMethod(meta.Node.Attributes["onClick"].Value);
					if(info != null)
					{
						info.Invoke(context, null);	
					}
				});
			}
		}
			
		/*End Binding*/

		/*Start Slider*/
		protected void sliderInit(UI render, GameObject self, UIElementMeta meta, object context){

			meta.Slider.fillRect.anchorMin = Vector2.zero;
			meta.Slider.fillRect.anchorMax = Vector2.one;
			
			meta.Slider.fillRect.localScale = Vector3.one;
			meta.Slider.fillRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, 1);
			meta.Slider.fillRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, 1);
			meta.Slider.fillRect.offsetMin = Vector2.zero;
			meta.Slider.fillRect.offsetMax = Vector2.zero;	
		}

		/*End Slider*/

		/*Start Text*/
		protected void txtDefaultValue(UI render, GameObject self, UIElementMeta meta, object context)
		{
			meta.Text.text = meta.Node.InnerText;
		}

		protected void textMaterial (UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (meta.Node.Attributes["material"] != null && UIAssets.Self != null) {
				meta.Text.material = UIAssets.Self.Materials.Find (x => x.name == meta.Node.Attributes["material"].Value);
			}
		}

		protected void textColor (UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (meta.Node.Attributes["color"] != null) {
				meta.Text.color = readColor (meta.Node.Attributes ["color"].Value);
			}
		}

		protected void textFont (UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (meta.Node.Attributes["font"] != null && UIAssets.Self != null) {
				Font font = UIAssets.Self.Fonts.Find (x => x.name == meta.Node.Attributes ["font"].Value);
				if (font != null) {
					meta.Text.font = font;
				}
			}
		}

		protected void textFontSize (UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (meta.Node.Attributes["size"] != null) {
				meta.Text.fontSize = Convert.ToInt32 (meta.Node.Attributes ["size"].Value);
			}
		}

		/*End Text*/

		/*Start Image*/

		protected void imageColor (UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (meta.Node.Attributes["color"] != null) {
				meta.Image.color = this.readColor (meta.Node.Attributes ["color"].Value);
			}
		}

		protected void imgBackground (UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (meta.Node.Attributes["image"] != null && UIAssets.Self != null) {
				if (meta.Node.Attributes ["image"].Value.IndexOf ("$") == 0) {
					
				} else {
					
					Sprite s = UIAssets.Self.Images.Find (x => x.name == meta.Node.Attributes ["image"].Value);
					if (s != null) {
						meta.Image.sprite = s;
					}
				}
			}
		}

		protected void imageRenderMode (UI render, GameObject self, UIElementMeta meta, object context)
		{

			if (meta.Node.Attributes["renderMode"] != null) {
				meta.Image.type = (Image.Type)Enum.Parse (typeof(Image.Type), meta.Node.Attributes["renderMode"].Value);
			}
		}

		protected void imgMaterial (UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (meta.Node.Attributes["material"] != null && UIAssets.Self != null) {
				Material m = UIAssets.Self.Materials.Find (x => x.name == meta.Node.Attributes ["material"].Value);
				if (m != null) {
					meta.Image.material = m;
				}
			}
		}
		/*End Image*/

		/*Start Drag*/

		protected void dragInit(UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (meta.Node.Attributes ["dragable"] != null) {
				Dragable drag = meta.Dragable;
				drag.Meta = meta;
			}
		}

		protected void dragRecieveInit(UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (meta.Node.Attributes ["recieveDrag"] != null) {
				DragableRecieve recieve = meta.DragableRecieve;
			}
		}

		protected void dragRecieveBind(UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (meta.Node.Attributes ["recieveDragBind"] != null) {
				string name = meta.Node.Attributes["recieveDragBind"].Value;

				UIElementBinding bind = () => {;
					DragRecieve del = (DragRecieve)Delegate.CreateDelegate(typeof(DragRecieve), context, name);
					if(del != null)
					{
						meta.DragableRecieve.Dragable += del;
					}else{
						Debug.LogError("Method "+name+" does not match delegate DragRecieve");
					}
				};

				meta.UpdateBinding += bind;
			}
		}
		/*End Drag*/

		/*Verbs*/

		protected void show(UI render, GameObject self, UIElementMeta meta, object context)
		{
			if (meta.Node.Attributes["show"] != null) {
				meta.UpdateBinding += () => {
					Type type = context.GetType();
					PropertyInfo info = type.GetProperty(meta.Node.Attributes["show"].Value);
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

		protected void foreachChild (UI render, GameObject self, UIElementMeta meta, object context)
		{

			if (meta.Node.Attributes["foreach"] != null) {

				UIElementBinding binding = () => {

					meta.ContinueRenderChildren = false;

					string propName = meta.Node.Attributes ["foreach"].Value;
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

					//Debug.Log(meta[i].BindPath);
				}
			}
		}


		//Calculate the positioning of an ellement
		protected void position (UI renderer, GameObject self, UIElementMeta meta, object context)
		{
			Vector2 min = Vector2.zero;
			Vector2 max = Vector2.one;

			//If the min and max fields have been set use them, otherwise default to fullscreen
			if (meta.Node.Attributes["min"] != null) {
				min = readCord (meta.Node.Attributes["min"].Value);
			}

			if (meta.Node.Attributes["max"] != null) {
				max = readCord (meta.Node.Attributes ["max"].Value);
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
			input = input.Replace(" ", "");
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
	}
}
