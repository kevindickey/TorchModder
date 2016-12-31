using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorchModder.File
{
    public class Property
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public string Value { get; private set; }
        public Element Parent { get; private set; }

        public Property(string name, string type, string value)
        {
            this.Name = name;
            this.Type = type;
            this.Value = value;
        }

        public void ModifyParent(Element newParent)
        {
            this.Parent = newParent;
        }

        public void ModifyValue(string newValue)
        {
            this.Value = newValue;
        }

        public void ModifyType(string newType)
        {
            this.Type = newType;
        }

        public void ModifyName(string newName)
        {
            this.Name = newName;
        }

        public override string ToString()
        {
            return "<" + this.Type + ">" + this.Name + ":" + this.Value;
        }
    }
}
