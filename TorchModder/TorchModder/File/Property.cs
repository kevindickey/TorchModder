using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorchModder.File
{
    /// <summary>
    /// TorchFile property.  Contained by an element.  Looks something like
    /// /<TYPE/>Name:Value
    /// And is contained on its own line. Keeps track of its parent.
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Property Name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Property Type (ie, INTEGER, STRING)
        /// </summary>
        public string Type { get; private set; }
        /// <summary>
        /// Property Value
        /// </summary>
        public string Value { get; private set; }
        /// <summary>
        /// This properties parent element
        /// </summary>
        public Element Parent { get; private set; }

        /// <summary>
        /// Creates a new property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public Property(string name, string type, string value)
        {
            this.Name = name;
            this.Type = type;
            this.Value = value;
        }

        /// <summary>
        /// Modifies the property parent element
        /// </summary>
        /// <param name="newParent"></param>
        public void ModifyParent(Element newParent)
        {
            this.Parent = newParent;
        }

        /// <summary>
        /// Modifies the properties value
        /// </summary>
        /// <param name="newValue"></param>
        public void ModifyValue(string newValue)
        {
            this.Value = newValue;
        }

        /// <summary>
        /// Modifies the properties type
        /// </summary>
        /// <param name="newType"></param>
        public void ModifyType(string newType)
        {
            this.Type = newType;
        }

        /// <summary>
        /// Modifies the properties name
        /// </summary>
        /// <param name="newName"></param>
        public void ModifyName(string newName)
        {
            this.Name = newName;
        }

        /// <summary>
        /// Returns a string representation of the property
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "<" + this.Type + ">" + this.Name + ":" + this.Value;
        }
    }
}
