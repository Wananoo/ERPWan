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
using System.Windows.Threading;

namespace ERPWan
{
    /// <summary>
    /// Lógica de interacción para ExplorarProductos.xaml
    /// </summary>
    public partial class ExplorarProductos : Window
    {
        DataConn dc = new DataConn();
        int ticks=0;
        public ExplorarProductos()
        {
            InitializeComponent();
            //timer
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.5),
            };
            timer.Tick += timer_Tick;
            timer.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            ticks++;
            //GetServiceStatus();
            if (txbSearch.Text.Length == 0)
            {
                listBoxProductos.ItemsSource = dc.getProductos();
                if (listBoxProductos.Items.Count > 0)
                {
                    listBoxProductos.SelectedIndex = 0;
                }
            }
            else
            {
                if (ticks == 1)
                {
                    listBoxProductos.ItemsSource = dc.getProductos("where Descripcion like '%" + txbSearch.Text + "%' ");
                    if (listBoxProductos.Items.Count >0)
                    {
                        listBoxProductos.SelectedIndex = 0;
                    }
                }
            }
            
        }

        public string SelectedItem { get; set; }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            SelectedItem = listBoxProductos.SelectedItem.ToString();
            DialogResult = true;
            this.Close();
        }

        private void txbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ticks = 0;
        }

        private void btnNuevoProducto_Click(object sender, RoutedEventArgs e)
        {
            nuevoProducto np = new nuevoProducto();
            var result = np.ShowDialog();
            /*if (result == true)
            {
                cbxProductos.Text = np.SelectedItem;
            };*/
        }
    }
}
