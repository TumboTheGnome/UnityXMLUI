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
    public delegate UIEllementMeta EllementBuilder(Dictionary<string, string> attributes, string text, Transform parent);
    public class UI
    {
        private List<UIEllementMeta> _ellements = new List<UIEllementMeta>();
        private Dictionary<string, UIEllement> _handlers = new Dictionary<string, UIEllement>();
        private RectTransform _root;
        private Canvas _canvas;
        private EventSystem _events;
        private List<UIEllementMeta> _selection;
        
        public UI()
        {
            GameObject k = new GameObject("UI Root");
            this._canvas = k.AddComponent<Canvas>();
            this._root = k.GetComponent<RectTransform>();

            GameObject events = new GameObject("UI Events");
            this._events = events.AddComponent<EventSystem>();
        }

        public UI(IEllementBuilder builder)
        {
            GameObject k = new GameObject("UI Root");
            this._canvas = k.AddComponent<Canvas>();
            this._root = k.GetComponent<RectTransform>();

            GameObject events = new GameObject("UI Events");
            this._events = events.AddComponent<EventSystem>();

            if(builder != null)
            {
                foreach(UIEllement ellement in builder.Build())
                {
                    this.AddRule(ellement);
                }
            }
        }

        public Canvas Canvas
        {
            get
            {
                return this._canvas;
            }
        }

        public EventSystem Events
        {
            get
            {
                return this._events;
            }
        }

        public List<UIEllementMeta> Get
        {
            get
            {
                return this._selection;
            }
        }

        public UI LoadAdditive(string xml)
        {
            this._render(xml);
            return this;

        }

        public UI Load(string xml)
        {
            this.Clear();
            this._render(xml);
            return this;
        }

        public UI Clear()
        {
            if (this._root.childCount > 0)
            {
                foreach (Transform t in this._root)
                {
                    GameObject.Destroy(t.gameObject);
                }
            }
            return this;
        }

        public UI Select(string search)
        {
            this._selection = this._ellements.FindAll(x => x.Tags.Contains(search));
            return this;
        }

        public UI AddRule(UIEllement ellement)
        {
            if (this._handlers.ContainsKey(ellement.Name))
            {
                this._handlers[ellement.Name] = ellement;
            }
            else
            {
                this._handlers.Add(ellement.Name, ellement);
            }
            return this;
        }

        public void Test(string xml)
        {

            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XmlDocument document = new XmlDocument();

            while (reader.Read())
            {
                if (!String.IsNullOrEmpty(reader.Name))
                {
                    XmlNode node = document.ReadNode(reader);
                    Debug.Log("Node Name: " + node.Name);
                    Debug.Log("Has Children: " + node.HasChildNodes);
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        Debug.Log("Child Name: " + childNode.Name);
                        Debug.Log(childNode.InnerText);
                    }

                }
            }
        }

        private void _render(string xml)
        {
            if (!String.IsNullOrEmpty(xml))
            {
                this._selection = new List<UIEllementMeta>();

                XmlReader reader = XmlReader.Create(new StringReader(xml));
                XmlDocument document = new XmlDocument();
                while (reader.Read())
                {
                    if (!String.IsNullOrEmpty(reader.Name))
                    {
                        this._renderNode(document.ReadNode(reader), this._root);
                    }
                }

            }
        }

        private void _renderNode(XmlNode node, Transform parent)
        {
            if (this._handlers.ContainsKey(node.Name))
            {
                UIEllementMeta k = this._handlers[node.Name].Run(node, parent, this._ellements);
                this._selection.Add(k);

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (k.Object != null)
                    {
                        this._renderNode(childNode, k.Object.transform);
                    }
                }
            }
        }
    }

}
