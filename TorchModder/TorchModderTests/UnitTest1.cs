using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorchModder.File;
using TorchModder.Reader;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TorchReaderUnitTester
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ReadTorchFilesParallel()
        {
            List<TorchModder.Reader.TorchFile> torchFiles = new List<TorchModder.Reader.TorchFile>(2500);

            Parallel.ForEach(System.IO.Directory.EnumerateDirectories(@"C:\Program Files (x86)\Steam\steamapps\common\Torchlight II\MEDIA"), directory =>
            {
                Parallel.ForEach(System.IO.Directory.EnumerateFiles(directory), file =>
                {
                    if (file.EndsWith(".DAT") || file.EndsWith(".LAYOUT"))
                    {
                        torchFiles.Add(new TorchFile(file));
                        var content = torchFiles[0].Root.serialize();
                    }
                });
            });

            var readersCount = torchFiles.Count;

        }

        [TestMethod]
        public void ReadFilesNoParallel()
        {
            List<TorchModder.Reader.TorchFile> torchFiles = new List<TorchModder.Reader.TorchFile>(2500);

            foreach (string directory in System.IO.Directory.EnumerateDirectories(@"C:\Program Files (x86)\Steam\steamapps\common\Torchlight II\MEDIA"))
            {
                foreach (string file in System.IO.Directory.EnumerateFiles(directory))
                {
                    if (file.EndsWith(".DAT") || file.EndsWith(".LAYOUT"))
                    {
                        torchFiles.Add(new TorchFile(file));
                        var content = torchFiles[0].Root.serialize();
                    }
                }
            }

            var readersCount = torchFiles.Count;
        }

        [TestMethod]
        public void ReadDirectoriesWithController()
        {
            var controller = new TorchFileListController();

            controller.ReadAllDirectories("*.DAT");
            controller.ReadAllDirectories("*MISSLE*.LAYOUT");
            var files = controller.TorchFiles;
        }

        [TestMethod]
        public void ModifyFile()
        {
            var controller = new TorchFileListController();
            controller.AddSearchDirectory(@"C:\Program Files (x86)\Steam\steamapps\common\Torchlight II\MEDIA");
            controller.ReadAllDirectories("ALRICEMBERBALL.LAYOUT");

            var file = controller.FindTorchFile("ALRICEMBERBALL");

            var version = file.Root.FindProperty("VERSION");
            var element = file.Root.FindChild("OBJECTS").FindChild("BASEOBJECT").FindChild("PROPERTIES");

            Console.WriteLine(version.ToString());
            Console.WriteLine(element.serialize());

            element.FindProperty("DESCRIPTOR").ModifyValue("TABLE");
            element.FindProperty("RADIUS").ModifyValue("0.5");

            Console.WriteLine(element.serialize());


        }


    }
}
