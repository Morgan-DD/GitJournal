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
        }

        private void initClasses()
        {
            _controller._PATmanager = new PAT_manager(_controller);
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
    }
}