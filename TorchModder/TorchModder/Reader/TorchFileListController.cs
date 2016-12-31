using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TorchModder.Reader
{
    public class TorchFileListController
    {
        public List<string> Directories { get; private set; }
        public List<TorchFile> TorchFiles { get; private set; }

        public TorchFileListController()
        {
            this.TorchFiles = new List<TorchFile>(10000);
            this.Directories = new List<string>(100);
        }

        //methods
        public void AddSearchDirectory(string directory)
        {
            if (!this.Directories.Contains(directory))
            {
                this.Directories.Add(directory);
            }
        }

        public void ReadDirectory(string directory, string fileSearchPattern, System.IO.SearchOption searchOption)
        {
            //  Parallel.ForEach(file =>
            //  {
            foreach (string file in System.IO.Directory.EnumerateFiles(directory, fileSearchPattern, searchOption))
            {
                this.TorchFiles.Add(new TorchFile(file));

            }
            //  if(this.TorchFiles.Find(f => f.FilePath == file) == null)
            //  {
            //  this.TorchFiles.Add( new TorchFile(file));
            //   }
            //   });
        }

        /// <summary>
        /// reads all specified directories and their sub directories!
        /// </summary>
        /// <param name="fileSearchPattern"></param>
        public void ReadAllDirectories(string fileSearchPattern)
        {
            //  Parallel.ForEach(this.Directories, directory =>
            //    {
            foreach (string directory in this.Directories)
            {
                ReadDirectory(directory, fileSearchPattern, System.IO.SearchOption.AllDirectories);
            }
            //  });
        }

        public TorchFile FindTorchFileByPath(string filePath)
        {
            return this.TorchFiles.Find(file => file.FilePath == filePath);
        }

        public TorchFile FindTorchFile(string fileName)
        {
            return this.TorchFiles.Find(file => file.Name == fileName);
        }

        public TorchFile FindTorchFile(Regex fileNamePattern)
        {
            return this.TorchFiles.Find(file => fileNamePattern.IsMatch(file.Name));
        }

        public IEnumerable<TorchFile> FindTorchFiles(Regex fileNamePattern)
        {
            return this.TorchFiles.FindAll(file => fileNamePattern.IsMatch(file.Name));
        }

    }
}
