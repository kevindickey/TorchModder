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
    /// Reads files in the TorchLight 2 file format
    /// </summary>
    public class TorchFile
    {
        /// <summary>
        /// File Location of file
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// Returns a string representation of the Torchlight 2 File that is in memory
        /// </summary>
        public string StringContents { get { return this.serialize(); } }
        /// <summary>
        /// The Root Element of the file
        /// </summary>
        public Element Root { get; private set; }
        /// <summary>
        /// Gets the File name
        /// </summary>
        public string Name { get { return System.IO.Path.GetFileNameWithoutExtension(this.Path); } }

        /// <summary>
        /// Returns the Root Element name.  Returns "" if the Root is null
        /// </summary>
        public string RootName
        {
            get
            {
                if(this.Root != null)
                {
                    return this.Root.Name;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Constructor.  Reads in the file at the designated file path.
        /// </summary>
        /// <param name="filePath"></param>
        public TorchFile(string filePath)
        {
            this.Path = filePath;
            read();
        }

        /// <summary>
        /// Constructor.  Creates a new TorchFile in memory and doesn't read at the designated file path.
        /// When written to file, the filepath will be used.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="filePath"></param>
        public TorchFile(Element root, string filePath)
        {
            this.Root = root;
            this.Path = Path;
        }

        /// <summary>
        /// Returns a string representation of the file that's in memory
        /// </summary>
        /// <returns></returns>
        public string serialize()
        {
            return Root.serialize();
        }

        /// <summary>
        /// Returns the path of the file
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Path;
        }

        /// <summary>
        /// Writes the serialization of the TorchFile in memory to disk at the stored file path.
        /// </summary>
        public void Write()
        {
            System.IO.File.WriteAllText(this.Path, this.StringContents);
        }

        /// <summary>
        /// Starts a new element when reading a file and a new tag is found
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
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

        /// <summary>
        /// gets the name of an end tag
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
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

        /// <summary>
        /// determines if the line starts with a tag
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static bool isTag(string line)
        {
            line = line.Trim();
            return (line.StartsWith("[") && !isEndTag(line)) && line.EndsWith("]");
        }

        /// <summary>
        /// determines if the line starts with an end tag
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static bool isEndTag(string line)
        {
            line = line.Trim();
            return line.StartsWith("[/") && line.EndsWith("]");
        }

        /// <summary>
        /// determines if the line is a property
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static bool isProperty(string line)
        {
            line = line.Trim();
            return line.StartsWith("<");
        }

        /// <summary>
        /// Gets a properties type
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
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

        /// <summary>
        /// gets a properties name
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
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

        /// <summary>
        /// gets a properties value
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Starts reading a torchlight file
        /// </summary>
        private void read()
        {
            if (this.Path != null && this.Path != String.Empty && System.IO.File.Exists(this.Path))
            {
                this.Root = readFile();
            }
        }

        /// <summary>
        /// Does the real reading of the torchlight 2 file.  Basically, we create a stack of elements, and 
        /// process the file line by line, creating new elements as they're found, adding properties to them
        /// and adding the elements to their parents as children. I have no idea if this is efficient, but it works.
        /// </summary>
        /// <returns></returns>
        private Element readFile()
        {
            Stack<Element> elementStack = new Stack<Element>();

            using (StreamReader reader = new StreamReader(this.Path))
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
                    else if (isEndTag(currentLine)) //end an element!
                    {
                        var endTagName = getEndTagName(currentLine);
                        bool found = false;
                        while (!found)
                        {
                            var topElement = elementStack.Pop(); //we pop off the elements until we reach the beginning tag
                            if (topElement.Name == endTagName)
                            {
                                if (elementStack.Count == 0)
                                {
                                    return topElement; //at this point we've read everything in the file
                                }

                                found = true;
                            }
                        }
                    }
                    else if (isProperty(currentLine)) //add a property to the current element
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
