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

namespace ERPWan
{
    /// <summary>
    /// Lógica de interacción para Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            txbDBIP.Text = Properties.Settings.Default.DBIP;
            txbDBName.Text = Properties.Settings.Default.DBName;
            txbDBUser.Text = Properties.Settings.Default.DBUser;
            txbDBPass.Text = Properties.Settings.Default.DBPass;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DBIP = txbDBIP.Text;
            Properties.Settings.Default.DBName = txbDBName.Text;
            Properties.Settings.Default.DBUser = txbDBUser.Text;
            Properties.Settings.Default.DBPass = txbDBPass.Text;
            this.Close();
        }
    }
}
