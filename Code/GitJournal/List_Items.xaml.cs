using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
    /// Interaction logic for List_Items.xaml
    /// </summary>
    public partial class List_Items : UserControl
    {

        public bool _AddCheckboxes { get; set; }

        Controller _controller;

        public List_Items(bool addCheckboxes, Controller controller)
        {
            InitializeComponent();
            _AddCheckboxes = addCheckboxes;
            _controller = controller;
        }

        public void DisplayItems(List<string> ItemsList, string controlToCheck = "")
        {
            StackPanel_Main.Children.Clear();
            foreach (var item in ItemsList) 
            {
                Border border = new Border();
                border.BorderThickness = new Thickness(0,0,0,1);
                border.BorderBrush = new SolidColorBrush(Colors.Black);
                border.HorizontalAlignment = HorizontalAlignment.Stretch;
                border.Background = StackPanel_Main.Background;

                StackPanel panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;

                if (_AddCheckboxes)
                {
                    CheckBox chk = new CheckBox();
                    chk.VerticalAlignment = VerticalAlignment.Center;
                    if(item == controlToCheck)
                    {
                        chk.IsChecked = true;
                    }

                    panel.Children.Add(chk);
                    border.MouseUp += ItemClick_Checkbox;
                }
                else
                {
                    border.MouseUp += ItemClick_NoCheckbox;
                }

                Label lbl = new Label();
                lbl.Content = item;
                lbl.Foreground = new SolidColorBrush(Colors.White);
                lbl.HorizontalAlignment = HorizontalAlignment.Left;
                lbl.VerticalAlignment = VerticalAlignment.Center;
                panel.Children.Add(lbl);
                border.Child = panel;
                StackPanel_Main.Children.Add(border);
            }
        }

        private void ItemClick_NoCheckbox(object sender, RoutedEventArgs e)
        {
            foreach(Border border in StackPanel_Main.Children)
            {
                if(border.BorderThickness != new Thickness(0, 0, 0, 1))
                {
                    border.BorderThickness = new Thickness(0,0,0,1);
                    border.BorderBrush = new SolidColorBrush(Colors.Black);
                    break;
                }
            }
            (sender as Border).BorderThickness = new Thickness(2);
            (sender as Border).BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#0366D6"));
            _controller.ChangeSelectedRepo((((sender as Border).Child as StackPanel).Children[0] as Label).Content.ToString());
        }
        private void ItemClick_Checkbox(object sender, RoutedEventArgs e)
        {
            (((sender as Border).Child as StackPanel).Children[0] as CheckBox).IsChecked = !(((sender as Border).Child as StackPanel).Children[0] as CheckBox).IsChecked;
        }
    }
}
