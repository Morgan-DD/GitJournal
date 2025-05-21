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
using Microsoft.Web.WebView2;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Printing;
// using System.Windows.Forms;

namespace GitJournal
{
    /// <summary>
    /// Interaction logic for UserControl_JDT.xaml
    /// </summary>
    public partial class UserControl_JDT : UserControl
    {
        Controller _controller;

        Grid _GirdModified;

        List<string> _UserToDisplay;
        public UserControl_JDT(Controller controller)
        {
            InitializeComponent();
            _controller = controller;
        }

        public async void displayJDT(List<string> UsersToDisplay)
        {
            if (_controller._isFromGitHub)
                _controller._ActualGitJPath = System.IO.Path.Combine(_controller._GitJFileDir, $"{_controller._RepoSelected.Replace("/", "@")}.gitj");

            _UserToDisplay = UsersToDisplay;

            bool displayUserColumn = UsersToDisplay.Count > 1;
            string baseHtml = "<html><style>html{{background-color: #638764;}}</style></html>";

            StackPanel_Main.Children.Clear();
            ScrollViewer_Main.ScrollToTop();
            TimeSpan TotalDuration = new TimeSpan(0, 0, 0, 0);
            foreach (Commit_Info[] commitGroupByDay in _controller._JDTmanager.SplitCommitsByDay())
            {
                TimeSpan dayTotal = new TimeSpan(0, 0, 0, 0);
                StackPanel StackPanel_CommitDay = new StackPanel();
                StackPanel StackPanel_Date = new StackPanel();

                Label Label_Date = new Label();
                Label_Date.Content = commitGroupByDay[0].Date.ToString("dd MMMM yyyy");
                Label_Date.Margin = new Thickness(16, 0, 20, 0);
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

                    // Debug.WriteLine($"-------------------------------------\n{SingleCommit.CommitId}\n{SingleCommit.User}\n{!_controller._isFromGitHub}\n{SingleCommit.ExistingStatus}");
                    if ((UsersToDisplay.Contains(SingleCommit.User) || !_controller._isFromGitHub) && SingleCommit.ExistingStatus)
                    {
                        dayTotal += SingleCommit.Duration;
                        TotalDuration += SingleCommit.Duration;

                        // Create a Border for the entry
                        Border StackPanel_Entry_Border = new Border();
                        StackPanel_Entry_Border.BorderThickness = new Thickness(0, 0, 0, 1);

                        int columnIndex = 0;

                        Grid Grid_Entry = new Grid();

                        // Define columns
                        Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });  // Checkbox
                        int titleCol = ++columnIndex;
                        Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });  // Title

                        int contentCol = ++columnIndex;
                        Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });  // Content

                        int userCol = -1;
                        if (displayUserColumn)
                        {
                            userCol = ++columnIndex;
                            Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });  // User
                        }

                        int statusCol = ++columnIndex;
                        Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });  // Status

                        int durationCol = ++columnIndex;
                        Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });  // Duration

                        int imageCol = ++columnIndex;
                        Grid_Entry.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });  // Image

                        // Create the controls
                        CheckBox checkBox_select = new CheckBox();
                        checkBox_select.VerticalAlignment = VerticalAlignment.Center;
                        checkBox_select.Tag = SingleCommit.CommitId;

                        Image image = new Image();
                        image.Source = new BitmapImage(new Uri("pack://application:,,,/Ressources/redirect.png"));
                        image.Height = 20;
                        image.Width = 20;
                        image.Tag = SingleCommit.Url;
                        image.MouseUp += aPicture_MouseUp;

                        TextBlock TextBlock_Title = new TextBlock();
                        TextBlock_Title.TextWrapping = TextWrapping.Wrap;  // Enable text wrapping for Title
                        TextBlock_Title.Text = SingleCommit.Title;

                        TextBlock TextBlock_Content = new TextBlock();
                        TextBlock_Content.TextWrapping = TextWrapping.Wrap;  // Enable text wrapping for Content
                        TextBlock_Content.Text = SingleCommit.Content;  // Set Content content

                        // Create the labels for Title, Content, User, Status, and Duration
                        Label Label_Title = new Label() { Content = TextBlock_Title, HorizontalContentAlignment = HorizontalAlignment.Left, Foreground = Brushes.White, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#638764")) };
                        Label Label_Content = new Label() { Content = TextBlock_Content, HorizontalContentAlignment = HorizontalAlignment.Left, Foreground = Brushes.White, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#638764")), BorderThickness = new Thickness(1, 0, 0, 0) };
                        Label Label_User = new Label() { Content = SingleCommit.User, HorizontalContentAlignment = HorizontalAlignment.Left, Foreground = Brushes.White, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#638764")), BorderThickness = new Thickness(1, 0, 0, 0) };
                        Label Label_Status = new Label() { Content = SingleCommit.Status, HorizontalContentAlignment = HorizontalAlignment.Left, Foreground = Brushes.White, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#638764")), BorderThickness = new Thickness(1, 0, 0, 0) };
                        Label Label_Duration = new Label() { Content = SingleCommit.Duration.ToString(@"hh\:mm"), HorizontalContentAlignment = HorizontalAlignment.Left, Foreground = Brushes.White, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#638764")), BorderThickness = new Thickness(1, 0, 0, 0) };

                        if (SingleCommit.IsTitleModifed)
                            Label_Title.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#C4BA7E"));

                        if (SingleCommit.IsContentModifed)
                            Label_Content.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#C4BA7E"));

                        if (SingleCommit.IsStatusModifed)
                            Label_Status.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#C4BA7E"));

                        if (SingleCommit.IsTDurationModifed)
                            Label_Duration.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#C4BA7E"));

                        var pipeline = new MarkdownPipelineBuilder()
                            .UseAdvancedExtensions()  // This enables advanced features, including task lists
                            .Build();
                        string htmlContent = Markdig.Markdown.ToHtml(SingleCommit.Content, pipeline);
                        /*
                        // Use the correct WebView2 control for WPF
                        WebView2 webView = new WebView2();

                        // Add event handlers for WebView2 initialization (optional logging)
                        webView.CoreWebView2InitializationCompleted += (sender, args) =>
                        {
                            if (args.IsSuccess)
                            {
                                Debug.WriteLine("WebView2 Initialized Successfully.");
                            }
                            else
                            {
                                Debug.WriteLine($"WebView2 Initialization Failed: {args}");
                            }
                        };

                        // Ensure WebView2 is initialized
                        try
                        {
                            await webView.EnsureCoreWebView2Async(null); // Ensure initialization
                            Debug.WriteLine("WebView2 Initialization started.");

                            // After initialization, navigate to the HTML content
                            webView.NavigateToString(htmlContent);
                            Debug.WriteLine("WebView2 navigating to content...");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"WebView2 Initialization Error: {ex.Message}");
                        }
                        */

                        var stackPanel_Title = new Grid();
                        stackPanel_Title.Tag = SingleCommit.CommitId;
                        var textBox_title = new TextBox { Margin = new Thickness(2), Text = $"Column 1", Visibility = Visibility.Collapsed, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#637687")), TextWrapping = TextWrapping.Wrap, AcceptsReturn = true, Foreground = Brushes.White, BorderThickness = new Thickness(0), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Padding = new Thickness(2, 3, 0, 3), Tag = "1" };
                        textBox_title.KeyDown += InputBox_KeyDown;
                        stackPanel_Title.Children.Add(Label_Title);
                        stackPanel_Title.Children.Add(textBox_title);
                        stackPanel_Title.MouseUp += StackPanel_JDT_MouseUp;


                        var stackPanel_Content = new Grid();
                        stackPanel_Content.Tag = SingleCommit.CommitId;
                        var textBox_content = new TextBox { Margin = new Thickness(2), Text = $"Column 1", Visibility = Visibility.Collapsed, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#637687")), TextWrapping = TextWrapping.Wrap, AcceptsReturn = false, Foreground = Brushes.White, BorderThickness = new Thickness(0), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Padding = new Thickness(2, 3, 1, 3), Tag = "2" };
                        textBox_content.KeyDown += InputBox_KeyDown;
                        stackPanel_Content.Children.Add(Label_Content);
                        stackPanel_Content.Children.Add(textBox_content);
                        stackPanel_Content.MouseUp += StackPanel_JDT_MouseUp;


                        var stackPanel_Status = new Grid();
                        stackPanel_Status.Tag = SingleCommit.CommitId;
                        var textBox_status = new TextBox { Margin = new Thickness(2), Text = $"Column 1", Visibility = Visibility.Collapsed, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#637687")), TextWrapping = TextWrapping.Wrap, AcceptsReturn = true, Foreground = Brushes.White, BorderThickness = new Thickness(0), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Padding = new Thickness(2, 3, 1, 3), Tag = "3" };
                        textBox_status.KeyDown += InputBox_KeyDown;
                        stackPanel_Status.Children.Add(Label_Status);
                        stackPanel_Status.Children.Add(textBox_status);
                        stackPanel_Status.MouseUp += StackPanel_JDT_MouseUp;


                        var stackPanel_User = new Grid();
                        stackPanel_User.Tag = SingleCommit.CommitId;
                        var textBox_user = new TextBox { Margin = new Thickness(2), Text = $"Column 1", Visibility = Visibility.Collapsed, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#637687")), TextWrapping = TextWrapping.Wrap, AcceptsReturn = true, Foreground = Brushes.White, BorderThickness = new Thickness(0), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Padding = new Thickness(2, 3, 1, 3), Tag = "4" };
                        textBox_user.KeyDown += InputBox_KeyDown;
                        stackPanel_User.Children.Add(Label_User);
                        stackPanel_User.Children.Add(textBox_user);
                        stackPanel_User.MouseUp += StackPanel_JDT_MouseUp;


                        var stackPanel_Duration = new Grid();
                        stackPanel_Duration.Tag = SingleCommit.CommitId;
                        var textBox_duration = new TextBox { Margin = new Thickness(2), Text = $"Column 1", Visibility = Visibility.Collapsed, Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#637687")), TextWrapping = TextWrapping.Wrap, AcceptsReturn = true, Foreground = Brushes.White, BorderThickness = new Thickness(0), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Padding = new Thickness(2, 3, 1, 3), Tag = "5" };
                        textBox_duration.KeyDown += InputBox_KeyDown;
                        stackPanel_Duration.Children.Add(Label_Duration);
                        stackPanel_Duration.Children.Add(textBox_duration);
                        stackPanel_Duration.MouseUp += StackPanel_JDT_MouseUp;

                        // Set columns for each control
                        Grid.SetColumn(checkBox_select, 0);
                        Grid.SetColumn(stackPanel_Title, titleCol);
                        Grid.SetColumn(stackPanel_Content, contentCol);
                        if (displayUserColumn)
                            Grid.SetColumn(stackPanel_User, userCol);
                        Grid.SetColumn(stackPanel_Status, statusCol);
                        Grid.SetColumn(stackPanel_Duration, durationCol);
                        Grid.SetColumn(image, imageCol);

                        // Add controls to the grid
                        Grid_Entry.Children.Add(checkBox_select);
                        Grid_Entry.Children.Add(stackPanel_Title);
                        Grid_Entry.Children.Add(stackPanel_Content);
                        if (displayUserColumn)
                            Grid_Entry.Children.Add(stackPanel_User);
                        Grid_Entry.Children.Add(stackPanel_Status);
                        Grid_Entry.Children.Add(stackPanel_Duration);
                        Grid_Entry.Children.Add(image);

                        // Create the border and add the grid
                        StackPanel_Entry_Border.Child = Grid_Entry;


                        // Debug.WriteLine((Grid_Entry.Children[statusCol] as Grid).Children[0].GetType()); //  as Label).Content.ToString().Length

                        if (((Grid_Entry.Children[statusCol] as Grid).Children[0] as Label).Content.ToString().Length > 0 ||
                            ((Grid_Entry.Children[titleCol] as Grid).Children[0] as Label).Content.ToString().Length > 0 ||
                            ((Grid_Entry.Children[contentCol] as Grid).Children[0] as Label).Content.ToString().Length > 0)
                        {
                            // Add the StackPanel with Border to the main container
                            StackPanel_CommitDay.Children.Add(StackPanel_Entry_Border);
                        }
                    }
                    else
                    {
                        // StackPanel_CommitDay.Children.Clear();
                    }
                }
                Label totalHour = new Label();
                totalHour.Content = dayTotal.ToString(@"hh\:mm");
                totalHour.Margin = new Thickness(16, 0, 20, 0);
                totalHour.Foreground = Brushes.White;
                totalHour.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#637687"));
                totalHour.HorizontalContentAlignment = HorizontalAlignment.Right;
                totalHour.BorderThickness = new Thickness(0, 0, 0, 1);

                StackPanel_CommitDay.Children.Add(totalHour);

                dayTotal = new TimeSpan(0, 0, 0, 0);

                if (displayUserColumn)
                {
                    _controller._TitleBar.DisplayHeader(false);
                }
                else
                {
                    _controller._TitleBar.DisplayHeader(true);
                }
                if (StackPanel_CommitDay.Children.Count > 2)
                    StackPanel_Main.Children.Add(StackPanel_CommitDay);
            }
            _controller._TotalBar.updateTotal(TotalDuration);
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

        private void StackPanel_JDT_MouseUp(object sender, MouseEventArgs e)
        {
            ToggleGridElementVisibility(_GirdModified);

            if ((sender as Grid).Children[0].Visibility == Visibility.Visible)
            {
                if ((sender as Grid).Children[0] is Label)
                {

                    if (((sender as Grid).Children[0] as Label).Content is TextBlock)

                        ((sender as Grid).Children[1] as TextBox).Text = (((sender as Grid).Children[0] as Label).Content as TextBlock).Text.ToString();
                    else
                        ((sender as Grid).Children[1] as TextBox).Text = ((sender as Grid).Children[0] as Label).Content.ToString();
                }
            }
            ToggleGridElementVisibility((sender as Grid));
            _GirdModified = (sender as Grid);
        }

        private void ToggleGridElementVisibility(Grid GridToModify, bool SkipLogic = false)
        {
            if (GridToModify != null && !SkipLogic)
            {
                if (GridToModify.Children[0].Visibility == Visibility.Visible)
                {
                    GridToModify.Children[0].Visibility = Visibility.Collapsed;
                    GridToModify.Children[1].Visibility = Visibility.Visible;
                    GridToModify.Children[1].Focus();
                }
                else
                {
                    switch ((GridToModify.Children[1] as TextBox).Tag)
                    {
                        case "1":
                            if ((GridToModify.Children[0] as Label).Content is TextBlock)
                            {
                                if ((GridToModify.Children[1] as TextBox).Text.ToString() != ((GridToModify.Children[0] as Label).Content as TextBlock).Text)
                                {
                                    _controller._JDTmanager.modifyEntry(GridToModify.Tag.ToString(), title: (GridToModify.Children[1] as TextBox).Text.ToString());
                                    (GridToModify.Children[0] as Label).Content = (GridToModify.Children[1] as TextBox).Text;
                                    (GridToModify.Children[0] as Label).Background = _controller._Yellow;
                                }
                            }
                            else
                            {
                                if ((GridToModify.Children[1] as TextBox).Text.ToString() != (GridToModify.Children[0] as Label).Content)
                                {
                                    _controller._JDTmanager.modifyEntry(GridToModify.Tag.ToString(), title: (GridToModify.Children[1] as TextBox).Text.ToString());
                                    (GridToModify.Children[0] as Label).Content = (GridToModify.Children[1] as TextBox).Text;
                                    (GridToModify.Children[0] as Label).Background = _controller._Yellow;
                                }
                            }
                            break;

                        case "2":
                            if ((GridToModify.Children[0] as Label).Content is TextBlock)
                            {
                                if ((GridToModify.Children[1] as TextBox).Text.ToString() != ((GridToModify.Children[0] as Label).Content as TextBlock).Text)
                                {
                                    _controller._JDTmanager.modifyEntry(GridToModify.Tag.ToString(), content: (GridToModify.Children[1] as TextBox).Text.ToString());
                                    (GridToModify.Children[0] as Label).Content = (GridToModify.Children[1] as TextBox).Text;
                                    (GridToModify.Children[0] as Label).Background = _controller._Yellow;
                                }
                            }
                            else
                            {
                                if ((GridToModify.Children[1] as TextBox).Text.ToString() != (GridToModify.Children[0] as Label).Content)
                                {
                                    _controller._JDTmanager.modifyEntry(GridToModify.Tag.ToString(), content: (GridToModify.Children[1] as TextBox).Text.ToString());
                                    (GridToModify.Children[0] as Label).Content = (GridToModify.Children[1] as TextBox).Text;
                                    (GridToModify.Children[0] as Label).Background = _controller._Yellow;
                                }
                            }
                            break;

                        case "3":
                            if ((GridToModify.Children[1] as TextBox).Text.ToString() != (GridToModify.Children[0] as Label).Content)
                            {
                                _controller._JDTmanager.modifyEntry(GridToModify.Tag.ToString(), user: (GridToModify.Children[1] as TextBox).Text.ToString());
                                (GridToModify.Children[0] as Label).Content = (GridToModify.Children[1] as TextBox).Text;
                                (GridToModify.Children[0] as Label).Background = _controller._Yellow;
                            }
                            break;

                        case "4":
                            if ((GridToModify.Children[1] as TextBox).Text.ToString() != (GridToModify.Children[0] as Label).Content)
                            {
                                _controller._JDTmanager.modifyEntry(GridToModify.Tag.ToString(), status: (GridToModify.Children[1] as TextBox).Text.ToString());
                                (GridToModify.Children[0] as Label).Content = (GridToModify.Children[1] as TextBox).Text;
                                (GridToModify.Children[0] as Label).Background = _controller._Yellow;
                            }
                            break;

                        case "5":
                            if ((GridToModify.Children[1] as TextBox).Text.ToString() != (GridToModify.Children[0] as Label).Content)
                            {
                                _controller._JDTmanager.modifyEntry(GridToModify.Tag.ToString(), duration: (GridToModify.Children[1] as TextBox).Text);
                                (GridToModify.Children[0] as Label).Content = (GridToModify.Children[1] as TextBox).Text;
                                (GridToModify.Children[0] as Label).Background = _controller._Yellow;
                            }
                            break;
                    }

                    GridToModify.Children[0].Visibility = Visibility.Visible;
                    GridToModify.Children[1].Visibility = Visibility.Collapsed;
                }
            }
            else if (GridToModify != null)
            {
                if (GridToModify.Children[0].Visibility == Visibility.Visible)
                {
                    GridToModify.Children[0].Visibility = Visibility.Collapsed;
                    GridToModify.Children[1].Visibility = Visibility.Visible;
                    GridToModify.Children[1].Focus();
                }
                else
                {
                    GridToModify.Children[0].Visibility = Visibility.Visible;
                    GridToModify.Children[1].Visibility = Visibility.Collapsed;
                    (GridToModify.Children[1] as TextBox).Text = "";
                }
            }
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    // Insert line break at current caret position
                    var textBox = (TextBox)sender;
                    int caretIndex = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Insert(caretIndex, Environment.NewLine);
                    textBox.CaretIndex = caretIndex + Environment.NewLine.Length;

                    e.Handled = true; // Prevent default behavior
                }
                else
                {
                    // Validate or submit
                    ToggleGridElementVisibility(_GirdModified);
                    _GirdModified = null;
                }
            }
            else if (e.Key == Key.Escape)
            {
                ToggleGridElementVisibility(_GirdModified, true);
                _GirdModified = null;
            }
        }

        public void DeleteSelectedEntry()
        {
            foreach (string idToDelet in getSelectedEntry())
            {
                Debug.WriteLine(idToDelet);
                _controller._JDTmanager.modifyEntry(idToDelet, existingStatus: false);
            }

            _controller._JDTmanager.exportToGitJ();
            displayJDT(_UserToDisplay);
        }

        public List<string> getSelectedEntry()
        {
            var checkedTags = new List<string>();

            foreach (var child in StackPanel_Main.Children)
            {
                if (child is StackPanel commitDayPanel)
                {
                    foreach (var entry in commitDayPanel.Children)
                    {
                        if (entry is Border entryBorder && entryBorder.Child is Grid entryGrid)
                        {
                            foreach (var element in entryGrid.Children)
                            {
                                if (element is CheckBox cb && cb.IsChecked == true && cb.Tag is string tag)
                                {
                                    checkedTags.Add(tag);
                                }
                            }
                        }
                    }
                }
            }

            return checkedTags;
        }
    }
}