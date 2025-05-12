using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace GitJournal
{

    public class Controller
    {
        public MainWindow _mainWindow { get; set; }
        PAT_manager _PATmanager { get; set; }
        API_manager _APImanager { get; set; }
        public Gitj_Manager _Gitjmanager { get; set; }

        public List_Items _RepoList { get; set; }
        public List_Items _UserList { get; set; }

        public string _PATToken { get; set; }
        public string _ClientName { get; set; }
        public BitmapImage _ClientAvatar { get; set; }

        public string _RepoSelected { get; set; }

        public Controller(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            // logic objects
            _PATmanager = new PAT_manager(this);
            _APImanager = new API_manager(this);
            _Gitjmanager = new Gitj_Manager(this);

            // visual objects
            _RepoList = new List_Items(false, this);
            _UserList = new List_Items(true, this);
        }

        public async void Login()
        {
            string PAT = await _PATmanager.AskForPAT();

            if (PAT.Length > 0)
            {
                _PATToken = PAT;
                loadRepositories();
                _mainWindow.displayUserInfo(_ClientName, _ClientAvatar);
            }
        }

        public async void retrivePAT()
        {
            string PAT = await _PATmanager.TokenRetriving();

            if (PAT.Length > 0)
            {
                _PATToken = PAT;
                loadRepositories();
                _mainWindow.displayUserInfo(_ClientName, _ClientAvatar);
            }
        }

        public async void loadRepositories()
        {
            _RepoList.DisplayItems(await _APImanager.getUserRepo());

        }

        public void ChangeSelectedRepo(string newRepo)
        {
            _RepoSelected = newRepo;
            if (_mainWindow._pageId == 1)
            {
                loadRepoUser(newRepo);
                _mainWindow.EnableDisplaying();
            }
        }

        public async void loadRepoUser(string repoName)
        {
            _UserList.DisplayItems(await _APImanager.loadUserFromRepo(repoName), _ClientName);
        }
    }
}