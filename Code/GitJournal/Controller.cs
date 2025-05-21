using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace GitJournal
{

    public class Controller
    {
        public MainWindow _mainWindow { get; set; }
        public PAT_manager _PATmanager { get; set; }
        public API_manager _APImanager { get; set; }
        public Gitj_Manager _Gitjmanager { get; set; }
        public JDT_manager _JDTmanager { get; set; }

        public List_Items _RepoList { get; set; }
        public List_Items _UserList { get; set; }
        public UserControl_TitleBar _TitleBar { get; set; }
        public UserControl_TotalBar _TotalBar { get; set; }
        public UserControl_JDT _JDT { get; set; }
        public UserControl_InfosPopup _InfosPopup { get; set; }

        public Window_Popup _Window_Popup { get; set; }

        public string _PATToken { get; set; }
        public string _ClientName { get; set; }
        public BitmapImage _ClientAvatar { get; set; }

        public string _RepoSelected { get; set; }


        public string _GitJFileDir = "GitJ_Files";

        public bool _isFromGitHub = false;
        public string _ActualGitJPath = "";
        public SolidColorBrush _Yellow { get; set; } = (SolidColorBrush)(new BrushConverter().ConvertFrom("#C4BA7E"));
        public SolidColorBrush _Green { get; set; } = (SolidColorBrush)(new BrushConverter().ConvertFrom("#638764"));

        public string _TokenBase { get; set; } = "Created_Token";
        public int _LastTokenId { get; set; } = 0;

        public Controller(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            // logic objects
            _PATmanager = new PAT_manager(this);
            _APImanager = new API_manager(this);
            _Gitjmanager = new Gitj_Manager(this);
            _JDTmanager = new JDT_manager(this);

            // visual objects
            _RepoList = new List_Items(false, this);
            _UserList = new List_Items(true, this);
            _TitleBar = new UserControl_TitleBar();
            _TotalBar = new UserControl_TotalBar();
            _JDT = new UserControl_JDT(this);
            _InfosPopup = new UserControl_InfosPopup(this);
            _Window_Popup = new Window_Popup();
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

        public void changeRepo()
        {
            
        }

        public void changeEntryDate()
        {
            _InfosPopup.SetUpForDisplay(date: true);
            _Window_Popup.ChangeContent(_InfosPopup);
            _Window_Popup.Visibility = Visibility.Visible;

            _InfosPopup._commitsIds = _JDT.getSelectedEntry();
            _mainWindow.IsEnabled = false;
        }

        public void addNewEntry()
        {
            _InfosPopup.SetUpForDisplay(); 
            _Window_Popup.ChangeContent(_InfosPopup);
            _Window_Popup.Visibility = Visibility.Visible;

            _mainWindow.IsEnabled = false;
            _InfosPopup._commitsIds = null;
        }

        public async Task returnValueFromPopup(
            List<string> commitIds = null,
            string title = null,
            string content = null,
            string user = null,
            string status = null,
            string duration = null,
            DateTime? date = null,
            bool? existingStatus = null)
        {
            _Window_Popup.Visibility = Visibility.Hidden;

            if (commitIds != null)
            {
                var tasks = new List<Task>();

                foreach (var commitId in commitIds)
                {
                    string _title = title == "" ? null : title;
                    string _content = content == "" ? null : content;
                    string _user = user == "" ? null : user;
                    string _status = status == "" ? null : status;
                    string _duration = duration == "" ? null : duration;
                    /*
                    DateTime? _date = date == "" ? null : date;
                    bool? _existingStatus = existingStatus == "" ? null : existingStatus;
                    */

                    tasks.Add(_JDTmanager.modifyEntry(
                        commitId,
                        _title,
                        _content,
                        _user,
                        _status,
                        _duration,
                        date,
                        existingStatus));
                }

                await Task.WhenAll(tasks);
            }
            else if (date != null && duration != null && TimeSpan.TryParse(duration, out TimeSpan parsedDuration))
            {
                _JDTmanager.addNewEntry(
                    createToken(),
                    title,
                    content,
                    user,
                    status,
                    parsedDuration,           
                    true,
                    date.Value,                  
                    "",
                    "GitJ",
                    true);
            }

            _JDTmanager.exportToGitJ();
            /*
            foreach(Commit_Info commit in _JDTmanager._commits)
            {
                Debug.WriteLine($"{commit.CommitId}");
            }
            */
            _mainWindow.displayJDT();
            _mainWindow.IsEnabled = true;
        }

        public string createToken()
        {
            string newId;
            do
            {
                newId = $"{_TokenBase}-{_LastTokenId}";
                _LastTokenId++; // Increment after generating
            }
            while (_JDTmanager._commits.Any(c => c.CommitId == newId));

            return newId;
        }
    }
}