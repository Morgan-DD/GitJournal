using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Markdig;

namespace GitJournal
{
    /// <summary>
    /// Interaction logic for UserControl_JDT.xaml
    /// </summary>
    public partial class UserControl_JDT : UserControl
    {
        Controller _controller;
        public UserControl_JDT(Controller controller)
        {
            InitializeComponent();
            _controller = controller;
        }

        public void displayJDT()
        {
            Debug.WriteLine("Displaying Commits !!");
            foreach (Commit_Info[] commitGroupByDay in _controller._JDTmanager.SplitCommitsByDay())
            {
                TimeSpan TotalDuration = new TimeSpan(0);
                StackPanel StackPanel_CommitDay = new StackPanel();
                StackPanel StackPanel_Date = new StackPanel();

                Label Label_Date = new Label();
                Label_Date.Content = commitGroupByDay[0].Date.ToString("dd MMMM yyyy");
                Label_Date.Margin = new Thickness(16,0, 20, 0);
                Label_Date.Height = 60;
                Label_Date.Foreground = Brushes.White;
                Label_Date.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#637687"));
                Label_Date.FontSize = 15;
                Label_Date.VerticalAlignment = VerticalAlignment.Bottom;
                Label_Date.VerticalContentAlignment = VerticalAlignment.Bottom;

                /*
                Grid Grid_backgorund = new Grid() { Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#1D2125")), Width=20 };
                StackPanel_CommitDay.Children.Add(StackPanel_Date);*/

                StackPanel_Date.Children.Add(Label_Date);
                StackPanel_CommitDay.Children.Add(StackPanel_Date);

                foreach (Commit_Info SingleCommit in commitGroupByDay) 
                {
                    TotalDuration += SingleCommit.Duration;

                    // Create a Border for the entry
                    Border StackPanel_Entry_Border = new Border();
                    StackPanel_Entry_Border.BorderThickness = new Thickness(0, 0, 0, 1);

                    // Create a StackPanel for the entry layout
                    Grid Grid_Entry = new Grid();
                    // Grid_Entry.ShowGridLines = true;
                    // Define the columns
                    Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });  // Checkbox
                    Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });  // Title
                    Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });  // Content
                    Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });  // User
                    Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });  // Status
                    Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });  // Duration
                    Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });  // Image

                    // Create the controls
                    CheckBox checkBox_select = new CheckBox();
                    checkBox_select.VerticalAlignment = VerticalAlignment.Center;

                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri("pack://application:,,,/Ressources/redirect.png"));
                    image.Height = 20;
                    image.Width = 20;
                    image.Tag = SingleCommit.Url;
                    image.MouseUp += aPicture_MouseUp;

                    TextBlock TextBlock_Title = new TextBlock();
                    TextBlock_Title.TextWrapping = TextWrapping.Wrap;  // Enable text wrapping for Title

                    TextBlock TextBlock_Content = new TextBlock();
                    TextBlock_Content.TextWrapping = TextWrapping.Wrap;  // Enable text wrapping for Content
                    TextBlock_Content.Text = SingleCommit.Content;  // Set Content content

                    // Set the Label content conditionally based on the title
                    if (SingleCommit.Title.Contains(")"))
                    {
                        TextBlock_Title.Text = Regex.Match(SingleCommit.Title, @":\s*(.*)").Groups[1].Value;
                    }
                    else
                    {
                        TextBlock_Title.Text = SingleCommit.Title;
                    }

                    // Create the labels for Title, Content, User, Status, and Duration
                    Label Label_Title = new Label() { Content = TextBlock_Title, HorizontalContentAlignment = HorizontalAlignment.Left, Foreground = Brushes.White, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#638764"))};
                    Label Label_Content = new Label() { Content = TextBlock_Content, HorizontalContentAlignment = HorizontalAlignment.Left, Foreground = Brushes.White, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#638764")), BorderThickness = new Thickness(1, 0, 0, 0) };
                    Label Label_User = new Label() { Content = SingleCommit.User, HorizontalContentAlignment = HorizontalAlignment.Left, Foreground = Brushes.White, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#638764")), BorderThickness = new Thickness(1, 0, 0, 0) };
                    Label Label_Status = new Label() { Content = SingleCommit.Status, HorizontalContentAlignment = HorizontalAlignment.Left, Foreground = Brushes.White, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#638764")), BorderThickness = new Thickness(1, 0, 0, 0) };
                    Label Label_Duration = new Label() { Content = SingleCommit.Duration, HorizontalContentAlignment = HorizontalAlignment.Left, Foreground = Brushes.White, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#638764")), BorderThickness = new Thickness(1, 0, 0, 0) };

                    // Set columns for each control
                    Grid.SetColumn(checkBox_select, 0);
                    Grid.SetColumn(Label_Title, 1);
                    Grid.SetColumn(Label_Content, 2);
                    Grid.SetColumn(Label_User, 3);
                    Grid.SetColumn(Label_Status, 4);
                    Grid.SetColumn(Label_Duration, 5);
                    Grid.SetColumn(image, 6);

                    // Add controls to the grid
                    Grid_Entry.Children.Add(checkBox_select);
                    Grid_Entry.Children.Add(Label_Title);
                    Grid_Entry.Children.Add(Label_Content);
                    Grid_Entry.Children.Add(Label_User);
                    Grid_Entry.Children.Add(Label_Status);
                    Grid_Entry.Children.Add(Label_Duration);
                    Grid_Entry.Children.Add(image);
                    

                    // Create the border and add the grid
                    StackPanel_Entry_Border.Child = Grid_Entry;


                    // Add the StackPanel with Border to the main container
                    StackPanel_CommitDay.Children.Add(StackPanel_Entry_Border);

                }

                //Debug.WriteLine((StackPanel_CommitDay.Children[1] as Border).Child.GetType());
                _controller._TitleBar.updateColumnSize((((StackPanel_CommitDay.Children[1] as Border).Child as Grid).Children[1] as Label).Width, (((StackPanel_CommitDay.Children[1] as Border).Child as Grid).Children[2] as Label).Width, (((StackPanel_CommitDay.Children[1] as Border).Child as Grid).Children[3] as Label).Width, (((StackPanel_CommitDay.Children[1] as Border).Child as Grid).Children[4] as Label).Width, (((StackPanel_CommitDay.Children[1] as Border).Child as Grid).Children[5] as Label).Width, true);

                StackPanel_Main.Children.Add(StackPanel_CommitDay);
                
            }
            Debug.WriteLine("End of Displaying Commits !!");
        }

        private void aPicture_MouseUp(object sender, MouseEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = (sender as Image).Tag.ToString(),
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
}
