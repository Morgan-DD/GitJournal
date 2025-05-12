using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        List<int> _ratio = [15, 50, 10, 15, 10];
        public UserControl_TitleBar()
        {
            InitializeComponent();
        }

        public void DisplayHeader(bool ignorUser)
        {
            StackPanel_main.Children.Clear();
            int id = 0;
            foreach (string columnName in _columnName)
            {
                if (ignorUser && columnName == "Utilisateur")
                {

                }
                else
                {

                    Label lbl = new Label();

                    lbl.Content = columnName;
                    lbl.Foreground = Brushes.White;
                    lbl.BorderBrush = Brushes.Black;
                    if(id > 0)
                        lbl.BorderThickness = new Thickness(1, 0, 0, 0);
                    if (id == 1 && ignorUser)
                        lbl.Width = StackPanel_main.ActualWidth / 100 * (_ratio[id] + _ratio[2]);
                    else
                        lbl.Width = StackPanel_main.ActualWidth / 100 * _ratio[id];
                    lbl.FontSize = 14;
                    lbl.VerticalContentAlignment = VerticalAlignment.Center;
                    StackPanel_main.Children.Add(lbl);
                }
                id++;
            }
        }
    }
}