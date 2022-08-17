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
    /// Lógica de interacción para CambiarDatosInv.xaml
    /// </summary>
    public partial class CambiarDatosInv : Window
    {
        DataConn dc = new DataConn();
        VentanaPrincipal.productoInventario original;
        public CambiarDatosInv(VentanaPrincipal.productoInventario o)
        {
            InitializeComponent();
            //List<String> datos = dc.GetItem(ID);
            txbPrecio.Text = o.precioCInv;
            txbPrecioVenta.Text = o.precioVInv;
            txbStock.Text = o.stockInv;
            original = o;
            txbPrecio.Focus();
            txbPrecio.SelectAll();
        }
        public VentanaPrincipal.productoInventario datosreturn
        {
            get { return new VentanaPrincipal.productoInventario
            {
                codigoInv = original.codigoInv,
                nombreInv = original.nombreInv,
                precioCInv = txbPrecio.Text,
                precioVInv = txbPrecioVenta.Text,
                stockInv = txbStock.Text 
            }; }
        }
        private void txbStock_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbStock.Text.Length == 0)
            {
                return;
            }
            else
            {
                txbStock.Text = Int32.Parse(GlobalTools.OnlyNum(txbStock.Text)).ToString();
                txbStock.CaretIndex = txbStock.Text.Length;
            }
            CheckIngresar();
        }
        void CheckIngresar()
        {
            btnOK.IsEnabled = true;
            if (txbPrecio.Text.Equals("") || txbStock.Text.Equals("") || txbPrecioVenta.Text.Equals(""))
            {
                btnOK.IsEnabled = false;
            }
        }

        private void txbPrecioVenta_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbPrecioVenta.Text.Length == 0)
            {
                return;
            }
            else
            {
                txbPrecioVenta.Text = Int32.Parse(GlobalTools.OnlyNum(txbPrecioVenta.Text)).ToString();
                txbPrecioVenta.CaretIndex = txbPrecioVenta.Text.Length;
            }
            CheckIngresar();
        }

        private void txbPrecio_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbPrecio.Text.Length == 0)
            {
                return;
            }
            else
            {
                txbPrecio.Text = Int32.Parse(GlobalTools.OnlyNum(txbPrecio.Text)).ToString();
                txbPrecio.CaretIndex = txbPrecio.Text.Length;
            }
            CheckIngresar();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            List<int> DatosNuevos = new List<int>
                {
                    Int32.Parse(txbPrecio.Text),
                    Int32.Parse(txbPrecioVenta.Text),
                    Int32.Parse(txbStock.Text),
                };
            dc.EditInventarioData(original.codigoInv, DatosNuevos);
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
