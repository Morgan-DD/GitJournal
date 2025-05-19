using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Forms;

namespace GitJournal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Controller _controller;

        public int _pageId {  get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += IntiMethodes;
        }

        private async void IntiMethodes(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("--------------------------- START ---------------------------");
            _controller = new Controller(this);
            initClasses();

            _controller.retrivePAT();

            Grid_JDT_Content.Children.Add(_controller._RepoList);
            Grid_JDT_Content.Children.Add(_controller._UserList);

            (Grid_JDT_Content.Children[0] as List_Items).HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            (Grid_JDT_Content.Children[0] as List_Items).Height = Grid_JDT_Content.Height - 10;

            (Grid_JDT_Content.Children[0] as List_Items).Width = Grid_JDT_Content.ActualWidth / 10 * 5;

            (Grid_JDT_Content.Children[0] as List_Items).Margin = new Thickness(50, 5, 0, 5);


            (Grid_JDT_Content.Children[1] as List_Items).HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            (Grid_JDT_Content.Children[1] as List_Items).Height = Grid_JDT_Content.Height - 10;

            (Grid_JDT_Content.Children[1] as List_Items).Width = Grid_JDT_Content.ActualWidth / 10 * 3;

            (Grid_JDT_Content.Children[1] as List_Items).Margin = new Thickness(0, 5, 50, 5);

            _controller._Gitjmanager.createFolderIfDontExist();

            Grid_JDT_Titles.Children.Add(_controller._TitleBar);
            Grid_JDT_Total.Children.Add(_controller._TotalBar);
            Grid_JDT_Content.Children.Add(_controller._JDT);

            changePage(1);
        }

        private void initClasses()
        {
            
        }

        private void PATBUTTON_Click(object sender, RoutedEventArgs e)
        {
            // Debug.WriteLine(PATTEXT.Text);
        }


        /// <summary>
        /// action for the top bar that is a custom one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomTitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        /// <summary>
        /// action for the top bar that is a custom one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// action for the top bar that is a custom one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// action for the top bar that is a custom one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void MenuItem_Click_PAT(object sender, RoutedEventArgs e)
        {
            // open a dialogue windows and ask for the GitHub token (PAT)
            _controller.Login();
            
        }

        private void MenuItem_Click_Import(object sender, RoutedEventArgs e)
        {
            ChooseGitJFileAndImport();
        }

        private void MenuItem_Click_Export_gitj(object sender, RoutedEventArgs e)
        {
            _controller._JDTmanager.exportToGitJ(choosAFolder());
        }

        private void MenuItem_Click_Export_pdf(object sender, RoutedEventArgs e)
        {

        }

        public void displayUserInfo(string userName, BitmapImage userIcon)
        {
            Label_UserName.Content = userName;
            Image_UserIcon.Source = userIcon;
        }

        public void changePage(int page)
        {
            switch(page)
            {
                case 1: // page to choose the repo
                    Button_Display.Visibility = Visibility.Visible;
                    StackPanel_JDT_Content.Visibility = Visibility.Hidden;

                    Grid_JDT_Titles.Visibility = Visibility.Hidden;
                    Grid_JDT_Total.Children[1].Visibility = Visibility.Hidden;

                    Grid_JDT_Content.Children[0].Visibility = Visibility.Visible;
                    Grid_JDT_Content.Children[1].Visibility = Visibility.Visible;
                    Grid_JDT_Content.Children[2].Visibility = Visibility.Hidden;
                    break;
                case 2: // page to display JDT
                    StackPanel_JDT_Content.Visibility = Visibility.Visible;
                    Button_Display.Visibility = Visibility.Hidden;

                    Grid_JDT_Titles.Visibility = Visibility.Visible;

                    Grid_JDT_Total.Children[0].Visibility = Visibility.Hidden;
                    Grid_JDT_Total.Children[1].Visibility = Visibility.Visible;

                    Grid_JDT_Content.Children[0].Visibility = Visibility.Hidden;
                    Grid_JDT_Content.Children[1].Visibility = Visibility.Hidden;
                    Grid_JDT_Content.Children[2].Visibility = Visibility.Visible;
                    break;
            }
            if (page <= 2 && page >= 0)
                _pageId = page;
        }

        private void changeVisibility_Click(object sender, RoutedEventArgs e)
        {
            _controller._JDT.displayJDT(_controller._UserList.getUsersChecked());
        }

        private void Button_Display_Click(object sender, RoutedEventArgs e)
        {
            _controller._isFromGitHub = true;
            displayJDT();
        }

        public void EnableDisplaying()
        {
            Button_Display.IsEnabled = true;
        }

        public async void displayJDT()
        {
            // need to await so the commtis are pulled before display on _controller._JDT.displayJDT(_controller._JDTmanager._commits);
            if(_controller._isFromGitHub)
                await _controller._APImanager.getAllCommits(_controller._RepoSelected);

            _controller._JDTmanager.sortByDate();

            _controller._JDT.displayJDT(_controller._UserList.getUsersChecked());

            changePage(2);
        }

        private void Button_ChangeRepo_Click(object sender, RoutedEventArgs e)
        {
            _controller._JDTmanager._commits.Clear();
            changePage(1);
        }

        public void ChooseGitJFileAndImport()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "GitJ files (*.gitj)|*.gitj";
            openFileDialog.Title = "Select a GitJ file";

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filePath = openFileDialog.FileName;
                _controller._ActualGitJPath = filePath;
                _controller._JDTmanager.importFromGitJ(filePath);
            }
        }

        public string choosAFolder()
        {
            string folderPath = "";
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder to export the .gitj file";
                folderDialog.UseDescriptionForTitle = true;

                DialogResult result = folderDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    folderPath = folderDialog.SelectedPath;
                }
            }
            return folderPath;
        }

        private void Button_Export_Click(object sender, RoutedEventArgs e)
        {
            _controller._JDTmanager.exportToGitJ(choosAFolder());
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            _controller._JDT.DeleteSelectedEntry();
        }
    }
}