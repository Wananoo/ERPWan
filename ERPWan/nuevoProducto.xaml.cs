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
    /// Lógica de interacción para nuevoProducto.xaml
    /// </summary>
    public partial class nuevoProducto : Window
    {
        DataConn dc = new DataConn();
        List<TextBox> controlesNuevoProd = new List<TextBox>();
        public nuevoProducto()
        {
            InitializeComponent();
            controlesNuevoProd.Add(txbID);
            controlesNuevoProd.Add(txbNombre);
            controlesNuevoProd.Add(txbCant);
            controlesNuevoProd.Add(txbFamilia);
            controlesNuevoProd.Add(txbPrecioNeto);
            controlesNuevoProd.Add(txbPCIVA);
            controlesNuevoProd.Add(txbPrecioVenta);
            txbCant.Text = "1";
            txbPrecioNeto.Text = "0";
            CheckFields();
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            bool ok = false;
            List<String> nuevoItem = new List<String>();
            nuevoItem.Add(txbID.Text);//cod
            nuevoItem.Add(txbNombre.Text);//desc
            nuevoItem.Add(txbCant.Text);//stock
            nuevoItem.Add(GlobalTools.OnlyNum(txbPrecioVenta.Text));//precio
            nuevoItem.Add(GlobalTools.OnlyNum(txbPrecioNeto.Text));//pcompra
            nuevoItem.Add(txbFamilia.Text);//familia
            ok = dc.AddItem(nuevoItem);
            if (ok)
            {
                MessageBox.Show("Item ID " + txbID.Text + " añadido exitosamente", "Exito",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                ClearControls();
                InitializeComponent();
                txbID.Focus();
                txbCant.Text = "1";
                txbPrecioNeto.Text = "0";
            }
        }

        private void txbID_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckIngresar();
            txbID.Text = txbID.Text.ToUpper();
            txbID.CaretIndex = txbID.Text.Length;
        }
        void ClearControls()
        {
            foreach (Control c in controlesNuevoProd)
            {
                (c as TextBox).Text = "";
            }
        }
        private void txbPrecioNeto_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbPrecioNeto.Text.Length>0 && txbPrecioNeto.Text[0].Equals('$'))
            {
                if (txbPrecioNeto.Text.Substring(1).Length>0 && (txbPrecioNeto.Text.Substring(1) == GlobalTools.OnlyNum(txbPrecioNeto.Text.Substring(1))))
                {
                    txbPrecioNeto.CaretIndex = txbPrecioNeto.Text.Length;
                    if (txbPrecioNeto.Text.Substring(1).Length == 0)
                    {
                        txbPrecioNeto.Text = "0";
                    }
                    float valor = Int32.Parse(GlobalTools.OnlyNum(txbPrecioNeto.Text));
                    if (!(txbPCIVA is null))
                    {
                        txbPCIVA.Text = ((int)(valor * 1.19f)).ToString();
                    }
                }
            }
            txbPrecioNeto.Text = "$" + String.Format((GlobalTools.OnlyNum(txbPrecioNeto.Text)),"##,##0");
            CheckIngresar();
        }

        private void txbPCIVA_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbPCIVA.Text.Length > 0 && txbPCIVA.Text[0].Equals('$'))
            {
                if (txbPCIVA.Text.Substring(1).Length > 0 && (txbPCIVA.Text.Substring(1) == GlobalTools.OnlyNum(txbPCIVA.Text.Substring(1))))
                {
                    txbPCIVA.CaretIndex = txbPCIVA.Text.Length;
                    if (txbPCIVA.Text.Substring(1).Length == 0)
                    {
                        txbPCIVA.Text = "0";
                    }
                    float valor = Int32.Parse(GlobalTools.OnlyNum(txbPCIVA.Text));
                    if (!(txbPrecioVenta is null))
                    {
                        txbPrecioVenta.Text = ((int)(valor * 2f)).ToString();
                    }
                }
            }
            txbPCIVA.Text = "$" + String.Format((GlobalTools.OnlyNum(txbPCIVA.Text)), "##,##0");
            CheckIngresar();
        }

        private void txbPrecioVenta_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbPrecioVenta.Text.Length > 0 && txbPrecioVenta.Text[0].Equals('$'))
            {
                if (txbPrecioVenta.Text.Substring(1).Length > 0 && (txbPrecioVenta.Text.Substring(1) == GlobalTools.OnlyNum(txbPrecioVenta.Text.Substring(1))))
                {
                    txbPrecioVenta.CaretIndex = txbPrecioVenta.Text.Length;
                }
            }
            txbPrecioVenta.Text = "$" + String.Format((GlobalTools.OnlyNum(txbPrecioVenta.Text)), "##,##0");
            CheckIngresar();
        }

        private void txbCantidad_TextChanged(object sender, TextChangedEventArgs e)
        {
            txbCant.Text = GlobalTools.OnlyNum(txbCant.Text);
            txbCant.CaretIndex = txbCant.Text.Length;
            if (txbCant.Text.Length == 0)
            {
                txbCant.Text = "0";
            }
            txbCant.Text = Int32.Parse(txbCant.Text).ToString();
            CheckIngresar();
        }
        void CheckIngresar()
        {
            if (btnOK is null)
            {
                return;
            }
            else
            {
                btnOK.IsEnabled = false;
            }
            CheckFields();
        }
        void CheckFields()
        {
            btnOK.IsEnabled = true;
            foreach (TextBox c in controlesNuevoProd)
            {
                if (c.Text.Trim('\n').Length == 0)
                {
                    btnOK.IsEnabled = false;
                }
            }
        }

        private void txbNombre_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckIngresar();
        }

        private void txbFamilia_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckIngresar();
        }
    }
}
