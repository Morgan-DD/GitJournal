using System;
using System.Collections.Generic;
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
    /// Interaction logic for UserControl_TotalBar.xaml
    /// </summary>
    public partial class UserControl_TotalBar : UserControl
    {
        public UserControl_TotalBar()
        {
            InitializeComponent();
        }

        public void updateTotal(TimeSpan newTotal)
        {
            Label_TotalHours.Content = $"{newTotal}";
        }
    }
}
