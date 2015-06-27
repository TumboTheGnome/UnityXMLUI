using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

namespace BrainBit.UI
{
    public struct UIEllement
    {
        private string _name;
        private List<string> _fields;
        private EllementBuilder _builder;

        public UIEllement(string Name, List<string> Fields, EllementBuilder Builder)
        {
            if (!String.IsNullOrEmpty(Name) && Builder != null)
            {
                this._name = Name;
                this._fields = Fields;
                this._builder = Builder;
            }
            else
            {
                throw new Exception("UIEllement does not accept null Name or Builder fields.");
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public UIEllementMeta Run(XmlNode node, Transform parent, List<UIEllementMeta> meta)
        {
            Dictionary<string, string> attrs = new Dictionary<string, string>();

            if (this._fields != null)
            {

                foreach (XmlAttribute field in node.Attributes)
                {
                    if (this._fields.Contains(field.Name))
                    {
                        if (!attrs.ContainsKey(field.Name))
                        {
                            attrs.Add(field.Name, field.Value);
                        }
                    }
                }
            }

            UIEllementMeta m = this._builder(attrs, node.InnerText, parent);
            meta.Add(m);
            return m;
        }
    }

}
