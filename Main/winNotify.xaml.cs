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
using System.Windows.Shapes;

namespace Main
{
    /// <summary>
    /// Interaction logic for winNotify.xaml
    /// </summary>
    public partial class winNotify : Window
    {
       

        public winNotify()
        {
            InitializeComponent();
        }

        private void NotifyMain_Activated(object sender, EventArgs e)
        {
            double w = SystemParameters.PrimaryScreenWidth;
            double h = SystemParameters.PrimaryScreenHeight;

            this.Left = w - this.Width - 20;
            this.Top = h - this.Height - 100;
            
        }

        private void lblClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
        }
    }
}
