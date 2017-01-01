using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TorchModder.Reader
{
    /// <summary>
    /// Handles creation modification of a TorchFiles list
    /// </summary>
    public class TorchFileListController
    {
        /// <summary>
        /// Directories to read Torchlight files from
        /// </summary>
        public List<string> Directories { get; private set; }
        /// <summary>
        /// Stored list of TorchFiles
        /// </summary>
        public List<TorchFile> TorchFiles { get; private set; }

        /// <summary>
        /// Constructor.  Initializes lists.
        /// </summary>
        public TorchFileListController()
        {
            this.TorchFiles = new List<TorchFile>(100000);
            this.Directories = new List<string>(100);
        }

        /// <summary>
        /// Adds a directory to Directories
        /// </summary>
        /// <param name="directory"></param>
        public void AddSearchDirectory(string directory)
        {
            if (!this.Directories.Contains(directory))
            {
                this.Directories.Add(directory);
            }
        }

        /// <summary>
        /// Reads a single directory and adds any matching files to the TorchFiles list
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileSearchPattern"></param>
        /// <param name="searchOption"></param>
        public void ReadDirectory(string directory, string fileSearchPattern, System.IO.SearchOption searchOption)
        {
            foreach (string file in System.IO.Directory.EnumerateFiles(directory, fileSearchPattern, searchOption))
            {
                addTorchFile(file, 0);
            }
        }

        /// <summary>
        /// Same as ReadDirectory, but does everything in Parallel (multi-threaded).
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileSearchPattern"></param>
        /// <param name="searchOption"></param>
        public void ReadDirectoryParallel(string directory, string fileSearchPattern, System.IO.SearchOption searchOption)
        {
            Parallel.ForEach(System.IO.Directory.EnumerateFiles(directory, fileSearchPattern, searchOption), file =>
            {
                addTorchFile(file, 0);
            });
        }

        /// <summary>
        /// Attempts to add the TorchFile to the TorchFiles list
        /// </summary>
        /// <param name="file"></param>
        /// <param name="tries"></param>
        private void addTorchFile(string file, int tries)
        {
            if (tries < 10)
            {
                try
                {
                    if (this.TorchFiles.Find(f => f.Path == file) == null)
                    {
                        this.TorchFiles.Add(new TorchFile(file));
                        if (tries > 0)
                        {
                            Console.WriteLine("Successfully added file on retry: " + file);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed on file: " + file);
                    Console.WriteLine("Trying again...");
                    addTorchFile(file, tries + 1);
                }
            }
            else
            {
                Console.WriteLine("Failed to add file after 10 tries: " + file);
            }
        }

        /// <summary>
        /// reads all specified directories and their sub directories!
        /// </summary>
        /// <param name="fileSearchPattern"></param>
        public void ReadAllDirectories(string fileSearchPattern)
        {
            foreach (string directory in this.Directories)
            {
                ReadDirectory(directory, fileSearchPattern, System.IO.SearchOption.AllDirectories);
            }
        }

        /// <summary>
        /// Same as ReadAllDirectories, but does it in Parallel. (multi-threaded)
        /// </summary>
        /// <param name="fileSearchPattern"></param>
        public void ReadAllDirectoriesParallel(string fileSearchPattern)
        {
            Parallel.ForEach(this.Directories, directory =>
              {
                  ReadDirectoryParallel(directory, fileSearchPattern, System.IO.SearchOption.AllDirectories);
              });
        }

        /// <summary>
        /// Finds a TorchFile by its file path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public TorchFile FindTorchFileByPath(string filePath)
        {
            return this.TorchFiles.Find(file => file.Path == filePath);
        }

        /// <summary>
        /// Finds a TorchFile by its file name (no extension)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public TorchFile FindTorchFile(string fileName)
        {
            return this.TorchFiles.Find(file => file.Name == fileName);
        }

        /// <summary>
        /// Finds a TorchFile by name that matches the regular expression
        /// </summary>
        /// <param name="fileNamePattern"></param>
        /// <returns></returns>
        public TorchFile FindTorchFile(Regex fileNamePattern)
        {
            return this.TorchFiles.Find(file => fileNamePattern.IsMatch(file.Name));
        }

        /// <summary>
        /// Finds all TorchFiles whose names match the regular expression
        /// </summary>
        /// <param name="fileNamePattern"></param>
        /// <returns></returns>
        public IEnumerable<TorchFile> FindTorchFiles(Regex fileNamePattern)
        {
            return this.TorchFiles.FindAll(file => fileNamePattern.IsMatch(file.Name));
        }
    }
}
