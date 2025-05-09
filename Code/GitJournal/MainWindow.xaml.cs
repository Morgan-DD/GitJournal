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

            Grid_UserRepoList.Children.Add(_controller._RepoList);
            Grid_UserRepoList.Children.Add(_controller._UserList);

            (Grid_UserRepoList.Children[0] as List_Items).HorizontalAlignment = HorizontalAlignment.Left;

            (Grid_UserRepoList.Children[0] as List_Items).Height = Grid_UserRepoList.Height - 10;

            (Grid_UserRepoList.Children[0] as List_Items).Width = Grid_UserRepoList.ActualWidth / 10 * 5;

            (Grid_UserRepoList.Children[0] as List_Items).Margin = new Thickness(50, 5, 0, 5);


            (Grid_UserRepoList.Children[1] as List_Items).HorizontalAlignment = HorizontalAlignment.Right;

            (Grid_UserRepoList.Children[1] as List_Items).Height = Grid_UserRepoList.Height - 10;

            (Grid_UserRepoList.Children[1] as List_Items).Width = Grid_UserRepoList.ActualWidth / 10 * 3;

            (Grid_UserRepoList.Children[1] as List_Items).Margin = new Thickness(0, 5, 50, 5);

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

        }

        private void MenuItem_Click_Export_gitj(object sender, RoutedEventArgs e)
        {

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
                case 0:
                    break;
                case 1:
                    Grid_UserRepoList.Visibility = Visibility.Visible;
                    Button_Display.Visibility = Visibility.Visible;
                    StackPanel_JDT_Content.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    StackPanel_JDT_Content.Visibility = Visibility.Visible;
                    Grid_UserRepoList.Visibility = Visibility.Hidden;
                    Button_Display.Visibility = Visibility.Hidden;
                    break;
            }
            if (page <= 2 && page >= 0)
                _pageId = page;
        }

        private void changeVisibility_Click(object sender, RoutedEventArgs e)
        {
            if(_pageId == 1)
                changePage(2);
            else if (_pageId == 2)
                changePage(1);
        }

        private void Button_Display_Click(object sender, RoutedEventArgs e)
        {

        }

        public void EnableDisplaying()
        {
            Button_Display.IsEnabled = true;
        }
    }
}