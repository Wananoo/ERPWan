using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
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
using System.Windows.Threading;

namespace ERPWan
{
    /// <summary>
    /// Lógica de interacción para VentanaPrincipal.xaml
    /// </summary>
    public partial class VentanaPrincipal : Window
    {
        public struct productoVenta//struct para crear objetos de cada elemento de la dgventa
        {
            public string codigo { set; get; }
            public string descripcion { set; get; }
            public string precio { set; get; }
        }
        public struct productoInventario//struct para crear objetos de cada elemento de la dginv
        {
            public string codigoInv { set; get; }
            public string nombreInv { set; get; }
            public string precioCInv { set; get; }
            public string precioVInv { set; get; }
            public string stockInv { set; get; }
        }
        public struct ventaReg//struct para crear objetos de cada elemento de la dgreg
        {
            public string iDReg { set; get; }
            public string fechaReg { set; get; }
            public string descReg { set; get; }
            public string tVReg { set; get; }
            public string fPReg { set; get; }
        }
        int totalVenta = 0;
        int CostoVenta = 0;
        int iVAVenta = 0;
        List<List<String>> ListaProductos = new List<List<String>>();
        DataConn dc = new DataConn();
        List<Control> controlesVenta = new List<Control>();
        public VentanaPrincipal()
        {
            InitializeComponent();
            dGInventario.RowHeight = double.NaN;
            dGProductos.RowHeight = double.NaN;
            //timer
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += timer_Tick;
            timer.Start();
            lblStock.Content = "0";
        }
        void timer_Tick(object sender, EventArgs e)
        {
             GetServiceStatus();
             CheckIngresar();
        }
        void GetServiceStatus()
        {
            ServiceController sc = new ServiceController("mysql");

            switch (sc.Status)
            {
                case ServiceControllerStatus.Running:
                    {
                        lblDB.Content = "MySQL OK";
                        lblDB.Foreground = Brushes.DarkGreen; 
                        break;
                    }
                case ServiceControllerStatus.Stopped:
                    {
                        lblDB.Content = "MySQL Detenido"; 
                        lblDB.Foreground = Brushes.DarkRed; 
                        break;
                    }
                case ServiceControllerStatus.Paused:
                    {
                        lblDB.Content = "MySQL Pausado";
                        lblDB.Foreground = Brushes.YellowGreen; 
                        break;
                    }
                case ServiceControllerStatus.StopPending:
                    {
                        lblDB.Content = "MySQL Deteniendose"; 
                        lblDB.Foreground = Brushes.Yellow; 
                        break;
                    }
                case ServiceControllerStatus.StartPending:
                    { 
                        lblDB.Content = "MySQL Iniciando";
                        lblDB.Foreground = Brushes.Yellow; 
                        break;
                    }
                default:
                    {
                        lblDB.Content = "MySQL Status Changing";
                        lblDB.Foreground = Brushes.Yellow;
                        break;
                    }
                    
            }

        }
        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            int index = dGProductos.SelectedIndex;
            ListaProductos.RemoveAt(index);
            UpdateProductos();
        }

        private void btnCarrito_Click(object sender, RoutedEventArgs e)
        {
            GlobalTools.goToTab(MainTabControl,1);
        }

        private void btnVenta_Click(object sender, RoutedEventArgs e)
        {
            GlobalTools.goToTab(MainTabControl,1);
        }

        private void btnStats_Click(object sender, RoutedEventArgs e)
        {
            GlobalTools.goToTab(MainTabControl, 3);
        }

        private void btnConsultar_Click(object sender, RoutedEventArgs e)
        {
            GlobalTools.goToTab(MainTabControl, 3);
        }

        private void btnCaja_Click(object sender, RoutedEventArgs e)
        {
            GlobalTools.goToTab(MainTabControl, 2);
        }

        private void btnInventario_Click(object sender, RoutedEventArgs e)
        {
            GlobalTools.goToTab(MainTabControl, 2);
        }
        private void btnAnularVenta_Click(object sender, RoutedEventArgs e)
        {
            cleanVentaControls();
            GlobalTools.goToTab(MainTabControl, 0);
        }
        void cleanVentaControls()
        {
            cbxProductos.SelectedIndex = -1;
            cbxProductos.Text = "";
            txbCant.Text = "1";
            rTBDescripcion.Text = "";
            dGProductos.Items.Clear();
            controlesVenta.Clear();
            ListaProductos.Clear();
        }
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings s = new Settings();
            s.Show();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Salir();
        }

        private void btnSalirIco_Click(object sender, RoutedEventArgs e)
        {
            Salir();
        }
        void Salir()
        {
            this.Close();
        }

        private void windowVentanaPrincipal_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            const string message = "Desea cerrar la aplicacion?";
            const string caption = "Atencion";
            if (MessageBox.Show(message, caption,
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question,
                                         MessageBoxResult.No)== MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void dGProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dGInventario_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tabItemVenta_Selected(object sender, RoutedEventArgs e)
        {
            PopulateProductos();
            controlesVenta.Add(cbxProductos);
            controlesVenta.Add(txbCant);
            controlesVenta.Add(rTBDescripcion);
            lblTotal.Content = "$" + (totalVenta.ToString("##,##0"));
            lblCosto.Content = "$" + (CostoVenta.ToString("##,##0"));
            lblIVA.Content = "$" + (iVAVenta.ToString("##,##0"));
            lblFecha.Content = DateTime.Now.ToString("dd/MM/yyyy");
            cbxMetodo.SelectedIndex = -1;
            lblID.Content = dc.GetNextVentaID();
        }
        void PopulateProductos(String extra = "")
        {
            cbxProductos.ItemsSource = dc.getProductos(extra);
        }

        private void cbxProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxProductos.SelectedItem != null)
            {
                lblStock.Content = dc.getStock(cbxProductos.SelectedItem.ToString());
            }
        }

        private void cbxProductos_KeyDown(object sender, KeyEventArgs e)
        {
            cbxProductos.IsDropDownOpen = true;
        }

        private void cbxProductos_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void cbxProductos_GotFocus(object sender, RoutedEventArgs e)
        {
            cbxProductos.IsDropDownOpen = true;
        }

        private void cbxProductos_LostFocus(object sender, RoutedEventArgs e)
        {
            cbxProductos.IsDropDownOpen = false;
        }

        private void btnExplorar_Click(object sender, RoutedEventArgs e)
        {
            ExplorarProductos ep = new ExplorarProductos();
            var result = ep.ShowDialog();
            if (result == true)
            {
                cbxProductos.Text = ep.SelectedItem;
            };
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (cbxProductos.Text.Length > 0)
            {
                AddProducto();
                UpdateProductos();
                cbxProductos.Focus();
                cbxProductos.IsDropDownOpen = false;
            }
        }
        void AddProducto()
        {
            for (int i = 1; i <= Int32.Parse(txbCant.Text); i++)
            {
                List<String> NewProd = dc.getProducto(cbxProductos.Text);
                if (NewProd.Count > 0)
                {
                    ListaProductos.Add(NewProd);
                }
            }
        }
        void UpdateProductos()
        {
            dGProductos.Items.Clear();
            dGProductos.Items.Refresh();
            //dGProductos.RowTemplate.Height = 25;
            int itemcount = 0;
            foreach (List<String> Prod in ListaProductos)
            {
                productoVenta npv = new productoVenta
                {
                    codigo = Prod[0],
                    descripcion = Prod[1],
                    precio = Prod[2]
                };
                dGProductos.Items.Add(npv);
                itemcount++;
            }
            UpdatePrecio();
            CheckIngresar();
        }
        void UpdatePrecio()
        {
            int PrecioTotal = 0;
            int PrecioCosto = 0;
            foreach (productoVenta pv in dGProductos.Items)
            {
                PrecioTotal += Int32.Parse(pv.precio);
                PrecioCosto += Int32.Parse(GlobalTools.OnlyNum(dc.GetItem(pv.descripcion)[0]));
            }
            //PrecioCosto = PrecioTotal * 100 / 119;
            int PrecioIva = (int)(PrecioTotal*0.19f);
            totalVenta = PrecioTotal;
            iVAVenta = PrecioIva;
            CostoVenta = PrecioCosto;
            lblTotal.Content = "$" + (PrecioTotal.ToString("##,##0"));
            lblCosto.Content = "$" + (PrecioCosto.ToString("##,##0"));
            lblIVA.Content = "$" + (PrecioIva.ToString("##,##0"));

        }
        void CheckIngresar()
        {
            if (btnOKVenta is null)
            {
                return;
            }
            if (dGProductos.Items.Count >= 1)
            {
                btnOKVenta.IsEnabled = true;
                CheckFields();
            }
            else
            {
                btnOKVenta.IsEnabled = false;
            }
        }
        void CheckFields()
        {
            btnOKVenta.IsEnabled = true;
            foreach (Control c in controlesVenta)
            {
                if (c is TextBox && ((TextBox)c).Text.Equals(""))
                {
                    btnOKVenta.IsEnabled = false;
                }
                if (c is ComboBox && ((ComboBox)c).Text.Equals(""))
                {
                    btnOKVenta.IsEnabled = false;
                }
            }
            if (cbxMetodo.SelectedIndex == -1)
            {
                btnOKVenta.IsEnabled = false;
            }
        }
        private void txbCant_TextChanged(object sender, TextChangedEventArgs e)
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

        private void dGProductos_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
            CambiarPrecio cap = new CambiarPrecio(ListaProductos[dGProductos.SelectedIndex][2]);
            if ((bool)cap.ShowDialog() == true)
            {
                string result = cap.precioreturn;
                ListaProductos[dGProductos.SelectedIndex][2] = result;
            }
            UpdateProductos();
        }

        private void dGProductos_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

        }

        private void btnCantDown_Click(object sender, RoutedEventArgs e)
        {
            if (Int32.Parse(txbCant.Text) > 0)
            {
                txbCant.Text = (Int32.Parse(txbCant.Text) - 1).ToString();
            }
        }

        private void btnCantUp_Click(object sender, RoutedEventArgs e)
        {
            txbCant.Text = (Int32.Parse(txbCant.Text) + 1).ToString();
        }

        private void rTBDescripcion_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckIngresar();
        }

        private void btnOKVenta_Click(object sender, RoutedEventArgs e)
        {
            UpdatePrecio();
            List<List<String>> ListaIDProductos = new List<List<String>>();
            foreach (List<String> prod in ListaProductos)
            {
                ListaIDProductos.Add(new List<String> { prod[0], prod[2] });
            }
            List<String> ListaDatosVenta = new List<String>();
            ListaDatosVenta.Add(cbxMetodo.Text);
            ListaDatosVenta.Add(rTBDescripcion.Text);
            dc.IngresarVenta(ListaDatosVenta, ListaIDProductos, totalVenta, CostoVenta, iVAVenta, Int32.Parse(lblID.Content.ToString()));
            cleanVentaControls();
            UpdatePrecio();
            lblID.Content = dc.GetNextVentaID();
        }

        private void tabItemInventario_Selected(object sender, RoutedEventArgs e)
        {
            getInventarioData();
        }
        void getInventarioData()
        {
            List<List<String>> FullItemsList = dc.GetFullProductos(" where Familia != 'Servicios' order by Stock DESC");
            if (FullItemsList.Count == 0)
            {
                productoInventario npi = new productoInventario
                {
                    codigoInv = "No hay datos",
                    nombreInv = "No hay datos",
                    precioCInv = "No hay datos",
                    precioVInv = "No hay datos",
                    stockInv = "No hay datos",
                };
                dGInventario.Items.Add(npi);
                lblTotalInv.Content = "$0";
                lblTotalVentaInv.Content = "$0";
            }
            else
            {
                List<String> FullItemsStats = dc.GetFullProductosStats(" where Familia != 'Servicios'");
                foreach (List<String> Item in FullItemsList)
                {
                    productoInventario npi = new productoInventario
                    {
                        codigoInv = Item[0],
                        nombreInv = Item[1],
                        precioCInv = Item[2],
                        precioVInv = Item[3],
                        stockInv = Item[4],
                    };
                    dGInventario.Items.Add(npi);
                }
                lblTotalInv.Content = "$" + FullItemsStats[0];
                lblTotalVentaInv.Content = "$" + FullItemsStats[1];
            }
        }
        private void dGInventario_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
            if (dc.GetFullProductos(" order by Stock DESC").Count == 0)
            {
                return;
            }
            CambiarDatosInv cdi = new CambiarDatosInv((productoInventario)dGInventario.SelectedItem);
            if ((bool)cdi.ShowDialog() == true)
            {
                object result = cdi.datosreturn;
                dGInventario.SelectedItem = result;
            }
            dGInventario.Items.Clear();
            getInventarioData();
        }

        private void btnDetallesReg_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tabItemRegistros_Selected(object sender, RoutedEventArgs e)
        {
            dPDesde.SelectedDate = DateTime.Now.AddDays(-7);
            dPHasta.SelectedDate = DateTime.Now;
            getRegistroData(DateTime.Now.AddDays(-7), DateTime.Now);
        }
        void getRegistroData(DateTime desde, DateTime hasta)
        {
            dGRegistros.Items.Clear();
            int totalef = 0;
            int total = 0;
            List<List<String>> registros = dc.getRegistros(desde,hasta);
            if (registros.Count == 0)
            {
                ventaReg npi = new ventaReg
                {
                    iDReg = "No hay datos",
                    fechaReg = "No hay datos",
                    descReg = "No hay datos",
                    tVReg = "No hay datos",
                    fPReg = "No hay datos",
                };
                dGRegistros.Items.Add(npi);
                lblTotalCostosReg.Content = "$0";
                lblTotalVentasReg.Content = "$0";
                lblTotalVentasEfReg.Content = "$0";
            }
            else
            {
                List<String> FullVentasStats = dc.GetFullVentasStats(desde,hasta);
                foreach (List<String> Item in registros)
                {
                    ventaReg npi = new ventaReg
                    {
                        iDReg = Item[0],
                        fechaReg = Item[1],
                        descReg = Item[2],
                        tVReg = Item[3],
                        fPReg = Item[4],
                    };
                    total += Int32.Parse(GlobalTools.OnlyNum(Item[3]));
                    if (Item[4].Equals("Efectivo"))
                    {
                        totalef += Int32.Parse(GlobalTools.OnlyNum(Item[3]));
                    }
                    dGRegistros.Items.Add(npi);
                }
                lblTotalCostosReg.Content = "$"+FullVentasStats[0];
                lblTotalVentasReg.Content = "$" + total.ToString("#,##0");
                lblTotalVentasEfReg.Content = "$"+totalef.ToString("#,##0");
            }
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            fixdate();
            getRegistroData(dPDesde.SelectedDate.Value.Date, dPHasta.SelectedDate.Value.Date);
        }

        private void dPDesde_LostFocus(object sender, RoutedEventArgs e)
        {
            fixdate();
        }

        private void dPHasta_LostFocus(object sender, RoutedEventArgs e)
        {
            fixdate();
        }
        void fixdate()
        {
            if ((dPHasta.SelectedDate.Value.Date - dPDesde.SelectedDate.Value.Date).Days < 0)
            {
                dPHasta.SelectedDate = dPDesde.SelectedDate;
            }
        }
    }
}
