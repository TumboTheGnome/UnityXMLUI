using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BrainBit.UI
{
    public class DefaultRuleBuilder : IEllementBuilder
    {
        public List<UIEllement> Build()
        {
            List<UIEllement> r = new List<UIEllement>();
            r.Add(
           new UIEllement("panel", new List<string>() { "id", "background", "renderMode", "material", "min", "max" }, (Dictionary<string, string> attributes, string text, Transform parent) =>
            {
                GameObject panel = new GameObject("panel");
                Image image = panel.AddComponent<Image>();
                RectTransform rec = panel.GetComponent<RectTransform>();
                rec.SetParent(parent);

                if (attributes.ContainsKey("background") && UIAssets.Self != null)
                {
                    Sprite s = UIAssets.Self.Images.Find(x => x.name == attributes["background"]);
                    if(s != null)
                    {
                        image.sprite = s;
                    }
                }

                if (attributes.ContainsKey("material") && UIAssets.Self != null)
                {
                    Material m = UIAssets.Self.Materials.Find(x => x.name == attributes["material"]);
                    if(m != null)
                    {
                        image.material = m;
                    }
                }

                if(attributes.ContainsKey("renderMode"))
                {
                    image.type = (Image.Type)Enum.Parse(typeof(Image.Type), attributes["renderMode"]);
                }

                if (attributes.ContainsKey("min") && attributes.ContainsKey("max"))
                {
                    rec.anchorMin = _readCord(attributes["min"]);
                    rec.anchorMax = _readCord(attributes["max"]);
                    rec.localScale = Vector3.one;
                    rec.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1);
                    rec.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1);
                    rec.offsetMin = Vector2.zero;
                    rec.offsetMax = Vector2.zero;
                }

                string tags = "panel";

                if (attributes.ContainsKey("id"))
                {
                    tags += " " + attributes["id"];
                }

                return new UIEllementMeta(tags, panel);
            }));

            r.Add(new UIEllement("text", new List<string>() { "id", "font-size", "font","color", "min", "max" }, (Dictionary<string, string> attributes, string text, Transform parent) =>
            {
                GameObject textEllement = new GameObject("text");
                Text Text = textEllement.AddComponent<Text>();
                RectTransform rec = textEllement.GetComponent<RectTransform>();
                rec.SetParent(parent);

                if (attributes.ContainsKey("font") && UIAssets.Self != null)
                {
                    Font font  = UIAssets.Self.Fonts.Find(x => x.name == attributes["font"]);
                    if(font != null)
                    {
                        Text.font = font;
                    }
                }

                if (attributes.ContainsKey("font-size"))
                {
                    Text.fontSize = Convert.ToInt32(attributes["font-size"]);
                }

                if(attributes.ContainsKey("color"))
                {
                    Text.color = _readColor(attributes["color"]);
                }

                if (attributes.ContainsKey("min") && attributes.ContainsKey("max"))
                {
                    rec.anchorMin = _readCord(attributes["min"]);
                    rec.anchorMax = _readCord(attributes["max"]);
                    rec.localScale = Vector3.one;
                    rec.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1);
                    rec.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1);
                    rec.offsetMin = Vector2.zero;
                    rec.offsetMax = Vector2.zero;
                }

                Text.text = text;

                string tags = "text";

                if (attributes.ContainsKey("id"))
                {
                    tags += " " + attributes["id"];
                }

                return new UIEllementMeta(tags, textEllement);
            }));
            return r;
        }

        private static Vector2 _readCord(string input)
        {
            int split = input.IndexOf(',');
            string strX = input.Substring(0, split);
            string strY = input.Substring(split, input.Length - split);

            float x = 0;
            float y = 0;

            split = strX.IndexOf('%');
            if(split != -1)
            {
                string value = strX.Substring(0, strX.Length - split);
                x = (float)Convert.ToDouble(value)/100;
            }
            else
            {
                x = (float)Convert.ToDouble(strX);
            }

            split = strY.IndexOf('%');
            if(split != -1)
            {
                string value = strY.Substring(0, strY.Length - split);
                y = (float)Convert.ToDouble(value) / 100;
            }
            else
            {
                y = (float)Convert.ToDouble(strY);
            }

            return new Vector2(x, y);
        }

        private static Color _readColor(string input)
        {
            return Color.red;
        }

    }
}
