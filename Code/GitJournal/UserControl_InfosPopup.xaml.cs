using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for UserControl_InfosPopup.xaml
    /// </summary>
    public partial class UserControl_InfosPopup : UserControl
    {
        Controller _controller;
        public List<string> _commitsIds { get; set; } = new List<string>();
        public UserControl_InfosPopup(Controller controller)
        {
            InitializeComponent();
            _controller = controller;
        }

        public void SetUpForDisplay(bool title = false, bool text = false, bool user = false, bool status = false, bool Duration = false, bool Date = false)
        {
            if (!title)
                StackPanel_Title.Visibility = Visibility.Collapsed;
            else
            {
                StackPanel_Title.Visibility = Visibility.Visible;

            }

            if (!text)
                StackPanel_Text.Visibility = Visibility.Collapsed;
            else
            {
                StackPanel_Text.Visibility = Visibility.Visible;

            }

            if (!user)
                StackPanel_User.Visibility = Visibility.Collapsed;
            else
            {
                StackPanel_User.Visibility = Visibility.Visible;

            }

            if (!status)
                StackPanel_Status.Visibility = Visibility.Collapsed;
            else
            {
                StackPanel_Status.Visibility = Visibility.Visible;

            }

            if (!Duration)
                StackPanel_Duration.Visibility = Visibility.Collapsed;
            else
            {
                StackPanel_Duration.Visibility = Visibility.Visible;

            }

            if (!Date)
                StackPanel_Date.Visibility = Visibility.Collapsed;
            else
            {
                StackPanel_Date.Visibility = Visibility.Visible;

            }
        }

        public void UpgateTextBoxContent(string? title, string? text, string? user, string? status, TimeSpan? Duration, DateTime? Date)
        {
            if (title == null)
            {
                TextBox_Title.Text = title;
            }

            if (text == null)
            {
                TextBox_Content.Text = text;
            }

            if (user == null)
            {
                TextBox_User.Text = user;
            }

            if (status == null)
            {
                if(status.ToLower() == "done")
                    RadioButton_Done.IsChecked = true;
                else if (status.ToLower() == "wip")
                    RadioButton_WIP.IsChecked = true;
            }

            if (Duration == null)
            {
                TextBox_Duration.Text = Duration.ToString();
            }

            if (Date == null)
            {
                TextBox_Date.Text = Date.ToString();
            }

        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            _controller.returnValueFromPopup();
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            string status = null;

            if (RadioButton_Done.IsChecked == true)
                status = "Done";
            else if (RadioButton_WIP.IsChecked == true)
                status = "WIP";


            _controller.returnValueFromPopup(
                commitIds: _commitsIds, 
                title: TextBox_Title.Text, 
                content: TextBox_Content.Text, 
                user: TextBox_User.Text, 
                status: status, 
                duration: TextBox_Duration.Text,
                date: TextBox_Date.SelectedDate.Value, 
                existingStatus:true
                );
            
        }
    }
}
