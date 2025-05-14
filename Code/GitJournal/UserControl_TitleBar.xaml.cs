using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
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
    /// Interaction logic for UserControl_TitleBar.xaml
    /// </summary>
    public partial class UserControl_TitleBar : UserControl
    {
        List<string> _columnName = ["Titre", "Contenu", "Utilisateur", "Status", "Durée"];
        public List<int> _ratio { get; set; }
        public UserControl_TitleBar()
        {
            InitializeComponent();
            _ratio = [25, 50, 10, 7, 8];
        }

        public void updateColumnSize(double title, double content, double user, double status, double duration, bool ignorUser)
        {
            if (StackPanel_main.Children.Count > 0)
            {
                try
                {

                ((StackPanel_main.Children[0] as Grid).Children[0] as Label).Width = title;
                ((StackPanel_main.Children[1] as Grid).Children[0] as Label).Width = content;
                if (!ignorUser)
                    ((StackPanel_main.Children[2] as Grid).Children[0] as Label).Width = user;
                ((StackPanel_main.Children[3] as Grid).Children[0] as Label).Width = status;
                ((StackPanel_main.Children[4] as Grid).Children[0] as Label).Width = duration;
                }
                catch { }
            }
        }

        public void DisplayHeader(bool ignorUser)
        {
            StackPanel_main.Children.Clear();
            int id = 0;

            Grid Grid_Entry = new Grid();
            Grid_Entry.Width = StackPanel_main.ActualWidth;
            int statusColumnIndex = ignorUser ? 2 : 3;
            int durationColumnIndex = ignorUser ? 3 : 4;

            // Column definitions
            Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });  // Title
            Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });  // Content
            if (!ignorUser)
            {
                Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });  // User
            }
            Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });  // Status
            Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });  // Duration

            Label Label_Title = new Label() { Content = _columnName[0], HorizontalContentAlignment = HorizontalAlignment.Left, Foreground = Brushes.White };
            Label Label_Content = new Label() { Content = _columnName[1], HorizontalContentAlignment = HorizontalAlignment.Left, Foreground = Brushes.White };
            Label Label_User = new Label() { Content = _columnName[2], HorizontalContentAlignment = HorizontalAlignment.Left, Foreground = Brushes.White };
            Label Label_Status = new Label() { Content = _columnName[3], HorizontalContentAlignment = HorizontalAlignment.Left, Foreground = Brushes.White };
            Label Label_Duration = new Label() { Content = _columnName[4], HorizontalContentAlignment = HorizontalAlignment.Left, Foreground = Brushes.White };

            // Set columns
            Grid.SetColumn(Label_Title, 0);
            Grid.SetColumn(Label_Content, 1);
            if (!ignorUser)
            {
                Grid.SetColumn(Label_User, 2);
            }
            Grid.SetColumn(Label_Status, statusColumnIndex);
            Grid.SetColumn(Label_Duration, durationColumnIndex);

            // Add controls to the grid
            Grid_Entry.Children.Add(Label_Title);
            Grid_Entry.Children.Add(Label_Content);
            if (!ignorUser)
            {
                Grid_Entry.Children.Add(Label_User);
            }
            Grid_Entry.Children.Add(Label_Status);
            Grid_Entry.Children.Add(Label_Duration);
            StackPanel_main.Children.Add(Grid_Entry);
        }
    }
}