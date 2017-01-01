using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TorchModder.File
{
    /// <summary>
    /// A TorchFile Element.  Starts with [Name] tag and ends with a [/Name] tag.
    /// Contains properties. Contains other elements.  Keeps track of its parent.
    /// </summary>
    public class Element
    {
        /// <summary>
        /// Element Name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Element parent Element (has no parent if it is null)
        /// </summary>
        public Element Parent { get; set; }
        /// <summary>
        /// Properties belonging to this element
        /// </summary>
        private List<Property> Properties { get; set; }
        /// <summary>
        /// Elements belonging to this element
        /// </summary>
        private List<Element> ChildElements { get; set; }

        /// <summary>
        /// Creates a new elment
        /// </summary>
        /// <param name="name"></param>
        public Element(string name)
        {
            this.Name = name;
            this.Properties = new List<Property>();
            this.ChildElements = new List<Element>();
        }

        /// <summary>
        /// Creates a new Element with no name
        /// </summary>
        public Element()
        {
            this.Name = String.Empty;
            this.Properties = new List<Property>();
            this.ChildElements = new List<Element>();
        }

        /// <summary>
        /// Finds a child element by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Element FindChild(string name)
        {
            return this.ChildElements.Find(child => child.Name == name);
        }

        /// <summary>
        /// Finds child elements matching the regular expression
        /// </summary>
        /// <param name="nameToMatch"></param>
        /// <returns></returns>
        public IEnumerable<Element> FindChildren(Regex nameToMatch)
        {
            return this.ChildElements.FindAll(child => nameToMatch.IsMatch(child.Name));
        }

        /// <summary>
        /// Finds a property by name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public Property FindProperty(string propertyName)
        {
            return this.Properties.Find(property => property.Name == propertyName);
        }

        /// <summary>
        /// Finds properties by name that have a particular value
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Property FindPropertyWithValue(string propertyName, string value)
        {
            return this.Properties.Find(property => property.Name == propertyName && property.Value == value);
        }

        /// <summary>
        /// Returns true if this element has all the listed properties
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public bool HasProperties(List<Property> properties)
        {
            foreach(Property prop in properties)
            {
                if(this.Properties.Exists(p => p.Name == prop.Name && p.Value == prop.Value))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Finds all properties matching the regular expression (matches by name)
        /// </summary>
        /// <param name="nameToMatch"></param>
        /// <returns></returns>
        public IEnumerable<Property> FindProperties(Regex nameToMatch)
        {
            return this.Properties.FindAll(property => nameToMatch.IsMatch(property.Name));
        }

        /// <summary>
        /// Adds a parent element to this element
        /// </summary>
        /// <param name="element"></param>
        public void AddParent(Element element)
        {
            this.Parent = element;
        }

        /// <summary>
        /// Adds a new property to this element
        /// </summary>
        /// <param name="property"></param>
        public void AddProperty(Property property)
        {
            if (property != null)
            {
                this.Properties.Add(property);
                property.ModifyParent(this);
            }
            else
            {
                throw new Exception("Cannot add null property.");
            }
        }

        /// <summary>
        /// Adds a new child element to this element
        /// </summary>
        /// <param name="element"></param>
        public void AddElement(Element element)
        {
            if (element != null)
            {
                element.Parent = this;
                this.ChildElements.Add(element);
            }
            else
            {
                throw new Exception("Cannot add null element.");
            }
        }

        /// <summary>
        /// Adds an indentation to the end of a string
        /// </summary>
        /// <param name="indentation"></param>
        /// <param name="fileContents"></param>
        /// <returns></returns>
        private string addIndentation(int indentation, string fileContents)
        {
            for (int i = 0; i < indentation; i++)
            {
                fileContents += "\t";
            }

            return fileContents;
        }

        /// <summary>
        /// Determines how deep the indentation should be for this element
        /// </summary>
        /// <returns></returns>
        private int determineElementIndentation()
        {
            bool foundRoot = false;
            Element currentParent = this.Parent;
            int indentation = 0;
            while (!foundRoot)
            {
                if (currentParent == null)
                {
                    foundRoot = true;
                    return indentation;
                }
                else
                {
                    indentation += 1;
                    currentParent = currentParent.Parent;
                }
            }

            return indentation;
        }

        /// <summary>
        /// Determines how deep the indentation should be for properties in this element
        /// </summary>
        /// <returns></returns>
        private int determinePropertyIndentation()
        {
            return determineElementIndentation() + 1;
        }

        /// <summary>
        /// Serializes the Element and its properties
        /// </summary>
        /// <returns></returns>
        public string serialize()
        {
            string serializedElement = "";
            int elementIndentation = determineElementIndentation();
            int propertyIndentation = determinePropertyIndentation();

            //create the start tag
            serializedElement = addIndentation(elementIndentation, serializedElement); //add initial indentation
            serializedElement += "[" + this.Name + "]" + "\n";

            //add properties
            foreach (Property property in this.Properties)
            {
                serializedElement = addIndentation(propertyIndentation, serializedElement);
                serializedElement += property.ToString() + "\n";
            }

            //add child elements
            foreach (Element child in this.ChildElements)
            {
                serializedElement += child.serialize();
            }

            //add end tag
            serializedElement = addIndentation(elementIndentation, serializedElement);
            serializedElement += "[/" + this.Name + "]\n";

            return serializedElement;
        }

        /// <summary>
        /// Returns the name of the element
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
