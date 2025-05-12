using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace GitJournal
{
    public class Gitj_Manager
    {
        static Controller _controller;

        public string _folderName {  get; set; }

        public Gitj_Manager(Controller controller)
        {
            _controller = controller;

            _folderName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\gitj_files";
        }

        public void createFolderIfDontExist()
        {
            if (!Directory.Exists(_folderName))
            {
                Directory.CreateDirectory(_folderName);
                Debug.WriteLine($"{_folderName} created");
            }
        }
    }
}
