using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GitJournal
{
    
    class Controller
    {
        public MainWindow _mainWindow { get; set; }
        public PAT_manager _PATmanager { get; set; }

        public string _PATToken { get; set; }
        public string _ClientName { get; set; }
        public BitmapImage _ClientAvatar { get; set; }

        public Controller(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public async void Login()
        {
            string PAT = await _PATmanager.AskForPAT();

            if (PAT.Length > 0)
            {
                _PATToken = PAT;
                _mainWindow.displayUserInfo(_ClientName, _ClientAvatar);
            }
        }

        public async void retrivePAT()
        {
            string PAT = await _PATmanager.TokenRetriving();

            if (PAT.Length > 0)
            {
                _PATToken = PAT;
                _mainWindow.displayUserInfo(_ClientName, _ClientAvatar);
            }
        }
    }
}
