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

        public void SetUpForDisplay(bool title = false, bool text = false, bool user = false, bool status = false, bool duration = false, bool date = false)
        {
            // If all parameters are false, set everything to visible
            if (!title && !text && !user && !status && !duration && !date)
            {
                title = text = user = status = duration = date = true;
            }

            StackPanel_Title.Visibility = title ? Visibility.Visible : Visibility.Collapsed;
            StackPanel_Text.Visibility = text ? Visibility.Visible : Visibility.Collapsed;
            StackPanel_User.Visibility = user ? Visibility.Visible : Visibility.Collapsed;
            StackPanel_Status.Visibility = status ? Visibility.Visible : Visibility.Collapsed;
            StackPanel_Duration.Visibility = duration ? Visibility.Visible : Visibility.Collapsed;
            StackPanel_Date.Visibility = date ? Visibility.Visible : Visibility.Collapsed;
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
            {
                status = "Done";
                RadioButton_Done.IsChecked = false;
            }
            else if (RadioButton_WIP.IsChecked == true)
            {
                status = "WIP";
                RadioButton_WIP.IsChecked = false;
            }

            // Debug.WriteLine($"{_commitsIds}\n{TextBox_Title.Text}\n{TextBox_Content.Text}\n{TextBox_User.Text}\n{status}\n{TextBox_Duration.Text}\n{TextBox_Date.SelectedDate.Value}\n{true}\n");

            _controller.returnValueFromPopup(
                commitIds: _commitsIds,
                title: string.IsNullOrWhiteSpace(TextBox_Title.Text) ? "" : TextBox_Title.Text,
                content: string.IsNullOrWhiteSpace(TextBox_Content.Text) ? "" : TextBox_Content.Text,
                user: string.IsNullOrWhiteSpace(TextBox_User.Text) ? "" : TextBox_User.Text,
                status: string.IsNullOrWhiteSpace(status) ? "" : status,
                duration: string.IsNullOrWhiteSpace(TextBox_Duration.Text) ? "" : TextBox_Duration.Text,
                date: TextBox_Date.SelectedDate.HasValue ? TextBox_Date.SelectedDate.Value : null,
                existingStatus: true
            );

            TextBox_Title.Text = "";
            TextBox_Content.Text = "";
            TextBox_User.Text = "";
            TextBox_Title.Text = "";
            TextBox_Duration.Text = "";
            TextBox_Date.Text = "";
        }
    }
}
