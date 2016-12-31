using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TorchModder.File
{
    public class Element
    {
        public string Name { get; private set; }
        public Element Parent { get; set; }
        private List<Property> Properties { get; set; }
        private List<Element> ChildElements { get; set; }

        public Element(string name)
        {
            this.Name = name;
            this.Properties = new List<Property>();
            this.ChildElements = new List<Element>();
        }

        public Element()
        {
            this.Name = String.Empty;
            this.Properties = new List<Property>();
            this.ChildElements = new List<Element>();
        }

        public Element FindChild(string name)
        {
            return this.ChildElements.Find(child => child.Name == name);
        }

        public IEnumerable<Element> FindChildren(Regex nameToMatch)
        {
            return this.ChildElements.FindAll(child => nameToMatch.IsMatch(child.Name));
        }

        public Property FindProperty(string propertyName)
        {
            return this.Properties.Find(property => property.Name == propertyName);
        }

        public IEnumerable<Property> FindProperties(Regex nameToMatch)
        {
            return this.Properties.FindAll(property => nameToMatch.IsMatch(property.Name));
        }

        public void AddParent(Element element)
        {
            this.Parent = element;
        }

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

        private string addIndentation(int indentation, string fileContents)
        {
            for (int i = 0; i < indentation; i++)
            {
                fileContents += "\t";
            }

            return fileContents;
        }

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

        public string serialize()
        {
            string serializedElement = "";
            int elementIndentation = determineElementIndentation();
            int propertyIndentation = elementIndentation + 1;

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

        public override string ToString()
        {
            return this.Name;
        }
    }
}
