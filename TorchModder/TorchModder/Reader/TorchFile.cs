using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorchModder.File;

namespace TorchModder.Reader
{
    /// <summary>
    /// Reads the .dat file
    /// </summary>
    public class TorchFile
    {
        public string FilePath { get; private set; }
        public string StringContents { get { return this.serialize(); } }
        public Element Root { get; private set; }
        public string Name { get { return System.IO.Path.GetFileNameWithoutExtension(this.FilePath); } }

        public TorchFile(string filePath)
        {
            this.FilePath = filePath;
            read();
        }

        public string serialize()
        {
            return Root.serialize();
        }

        public override string ToString()
        {
            return this.FilePath;
        }

        public void Write()
        {
            System.IO.File.WriteAllText(this.FilePath, this.StringContents);
        }

        private Element startElement(string line)
        {
            line = line.Trim(); //remove any whitespace
            if (isTag(line))
            {
                //extract the name
                return new Element(line.Substring(1, line.Length - 2));
            }
            else
            {
                throw new Exception("Not a valid tag: " + line);
            }
        }

        private string getEndTagName(string line)
        {
            line = line.Trim();
            if (isEndTag(line))
            {
                return line.Substring(2, line.Length - 3); //extract the name
            }
            else
            {
                throw new Exception("Not a valid end tag: " + line);
            }
        }

        private static bool isTag(string line)
        {
            line = line.Trim();
            return (line.StartsWith("[") && !isEndTag(line)) && line.EndsWith("]");
        }

        private static bool isEndTag(string line)
        {
            line = line.Trim();
            return line.StartsWith("[/") && line.EndsWith("]");
        }

        private static bool isProperty(string line)
        {
            line = line.Trim();
            return line.StartsWith("<");
        }

        private string getPropertyType(string line)
        {
            var chars = line.ToCharArray();
            var propertyType = "";
            foreach (char ch in chars)
            {
                if (ch == '<')
                {
                    continue;
                }
                else if (ch == '>')
                {
                    break;
                }
                else
                {
                    propertyType += ch;
                }
            }

            return propertyType;
        }

        private string getPropertyName(string line)
        {
            var chars = line.ToCharArray();
            bool inName = false;
            var propertyName = "";
            foreach (char ch in chars)
            {
                if (ch == '>')
                {
                    inName = true;
                    continue;
                }
                else if (ch == ':')
                {
                    inName = false;
                    break;
                }

                if (inName)
                {
                    propertyName += ch;
                }

            }

            return propertyName;
        }

        private string getPropertyValue(string line)
        {
            try
            {
                return line.Split(':')[1]; //if this fails, oops
            }
            catch (Exception)
            {
                throw new Exception("Failed to read property value on line: " + line);
            }
        }

        //Need to build a list of Element
        //Need to build a list of Property
        //Elements contain other Elements
        //Elements also contain Properties

        //if we are at the start of an Element, create a new Element
        //if we are at the start of an Element, and currently in an Element, add the new Element to the previous Elements child elements
        //if we are at a property, add it to the current Element
        private void read()
        {
            if (this.FilePath != null && this.FilePath != String.Empty && System.IO.File.Exists(this.FilePath))
            {
                StreamReader reader = new StreamReader(this.FilePath);
                this.Root = readFile();
                reader.Dispose();
            }
        }

        private Element readFile()
        {
            Stack<Element> elementStack = new Stack<Element>();

            using (StreamReader reader = new StreamReader(this.FilePath))
            {
                while (!reader.EndOfStream)
                {
                    var currentLine = reader.ReadLine().Trim(); //get the current line

                    if (isTag(currentLine)) //create a new element!
                    {
                        if (elementStack.Count != 0)
                        {
                            var currentElement = startElement(currentLine);
                            elementStack.Peek().AddElement(currentElement); //add current element as a child to the parent
                            elementStack.Push(currentElement);
                        }
                        else
                        {
                            elementStack.Push(startElement(currentLine));
                        }
                    }
                    else if (isEndTag(currentLine))
                    {
                        var endTagName = getEndTagName(currentLine);
                        bool found = false;
                        while (!found)
                        {
                            var topElement = elementStack.Pop();
                            if (topElement.Name == endTagName)
                            {
                                if (elementStack.Count == 0)
                                {
                                    return topElement;
                                }

                                found = true;
                            }
                        }
                    }
                    else if (isProperty(currentLine))
                    {
                        elementStack.Peek().AddProperty(createProperty(currentLine)); //add the current property to the top element
                    }
                }
            }

            //at this point we've created the stack?
            throw new Exception("Couldn't read file");
        }

        private Property createProperty(string line)
        {
            return new Property(getPropertyName(line), getPropertyType(line), getPropertyValue(line));
        }
    }
}
