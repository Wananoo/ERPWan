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
    /// Lógica de interacción para CambiarPrecio.xaml
    /// </summary>
    public partial class CambiarPrecio : Window
    {
        public string precioreturn
        {
            get { return preciofinal; }
        }
        string preciofinal = "";
        public CambiarPrecio(string precio)
        {
            InitializeComponent();
            txbPrecio.Text = precio;
            preciofinal = precio;
            txbPrecio.Focus();
            txbPrecio.SelectAll();
        }

        private void txbPrecio_TextChanged(object sender, TextChangedEventArgs e)
        {
            txbPrecio.Text = GlobalTools.OnlyNum(txbPrecio.Text);
            txbPrecio.CaretIndex = txbPrecio.Text.Length;
            CheckIngresar();
        }
        void CheckIngresar()
        {
            btnOK.IsEnabled = true;
            if (txbPrecio.Text.Equals(""))
            {
                btnOK.IsEnabled = false;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            preciofinal = txbPrecio.Text;
            Window.GetWindow(this).DialogResult = true;
            Window.GetWindow(this).Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).DialogResult = false;
            Window.GetWindow(this).Close();
        }
    }
}
