using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ERPWan
{
    public class DataConn
    {
        public String getConnString()
        {
            string ConnectionString =
            "server=" + Properties.Settings.Default.DBIP +
            ";user=" + Properties.Settings.Default.DBUser +
            ";password=" + Properties.Settings.Default.DBPass +
            ";database=" + Properties.Settings.Default.DBName + ";";
            return ConnectionString;
        }
        public List<List<String>> getFullProductos(string condicion)
        {
            List<List<String>> ProdList = new List<List<string>>();
            try
            {
                String query = "select Codigo,Descripcion,PrecioCompra,Precio,Stock from Productos " + condicion;
                using (MySqlConnection c = new MySqlConnection(getConnString()))
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, c))
                    {
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                List<String> Prod = new List<String>
                                {
                                    dr.GetString(0),//cod
                                    dr.GetString(1),//desc
                                    dr.GetInt32(2).ToString(),//precio
                                    dr.GetInt32(3).ToString(),//stock
                                    dr.GetInt32(4).ToString(),//pcompra
                                    dr.GetInt32(5).ToString()//instalacion
                                };
                                ProdList.Add(Prod);
                            }
                        }
                    }
                    c.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Exception",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
            return ProdList;
        }
        public List<String> getProductos(String extra = "")
        {
            List<String> ProdList = new List<string>();
            try
            {
                String query = "select Descripcion from Productos "+extra+" order by Descripcion ASC";
                using (MySqlConnection c = new MySqlConnection(getConnString()))
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, c))
                    {
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                ProdList.Add(dr.GetString(0));
                            }
                        }
                    }
                    c.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Exception",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
            return ProdList;
        }
        public String getStock(String desc)
        {
            String Stock = "";
            try
            {
                String query = "select Stock from Productos where Descripcion = '" + desc+"'";
                using (MySqlConnection c = new MySqlConnection(getConnString()))
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, c))
                    {
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            dr.Read();
                            Stock = (dr.GetString(0));
                        }
                    }
                    c.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Exception",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
            return Stock;
        }
        public List<String> getProducto(String NombreProd)
        {
            List<String> NewProd = new List<string>();
            try
            {
                String query = "select Codigo,Descripcion,Precio,Instalacion,PrecioCompra from Productos where Descripcion = '" + NombreProd + "' or Codigo = '" + NombreProd + "'" +
                    "order by Codigo ASC limit 1";
                using (MySqlConnection c = new MySqlConnection(getConnString()))
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, c))
                    {
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            dr.Read();
                            NewProd.Add(dr.GetString(0));
                            NewProd.Add(dr.GetString(1));
                            NewProd.Add(dr.GetInt32(2).ToString());
                            NewProd.Add(dr.GetInt32(3).ToString());
                            NewProd.Add(dr.GetInt32(4).ToString());
                        }
                    }
                    c.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Item '" + NombreProd + "' no encontrado \n Error: " + e.GetType().ToString(), "Error",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
            return NewProd;
        }
        public void IngresarVenta(List<String> DatosVenta, List<List<String>> Productos, int Total,int Costo, int Iva, int ID)
        {
            /*
            Datosventa:
            0:TipoPago
            1:Desc
            */
            try
            {
                String query = @"insert into Ventas(Fecha, Descripcion, Costo, Iva, Total, TipoPago)"
                                        + " values(?fecha,?desc,?costo,?iva,?total,?tp)";
                using (MySqlConnection c = new MySqlConnection(getConnString()))
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, c))
                    {
                        cmd.Parameters.Add("?fecha", MySqlDbType.Date).Value = DateTime.Now;
                        cmd.Parameters.Add("?desc", MySqlDbType.String).Value = DatosVenta[1];
                        cmd.Parameters.Add("?costo", MySqlDbType.Int32).Value = Costo;
                        cmd.Parameters.Add("?iva", MySqlDbType.Int32).Value = Iva;
                        cmd.Parameters.Add("?total", MySqlDbType.Int32).Value = Total;
                        cmd.Parameters.Add("?tp", MySqlDbType.String).Value = DatosVenta[0];
                        cmd.ExecuteNonQuery();
                    }
                    c.Close();
                }
                foreach (List<String> codigoprecio in Productos)
                {
                    query = @"insert into VentaProd(IDVenta, CodigoProducto, Precio) values"
                                + "(?idventa,?codigo,?precio)";
                    using (MySqlConnection c = new MySqlConnection(getConnString()))
                    {
                        c.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, c))
                        {
                            cmd.Parameters.Add("?idventa", MySqlDbType.Int32).Value = ID;
                            cmd.Parameters.Add("?codigo", MySqlDbType.String).Value = codigoprecio[0];
                            cmd.Parameters.Add("?precio", MySqlDbType.Int32).Value = codigoprecio[1];
                            cmd.ExecuteNonQuery();
                        }
                        c.Close();
                    }
                    query = "UPDATE Productos SET Stock = Stock-1 WHERE Codigo = ?codigo";
                    using (MySqlConnection c = new MySqlConnection(getConnString()))
                    {
                        c.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, c))
                        {
                            cmd.Parameters.Add("?codigo", MySqlDbType.String).Value = codigoprecio[0];
                            cmd.ExecuteNonQuery();
                        }
                        c.Close();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
        }
        public String GetNextVentaID()
        {
            String NextVentaID = "";
            try
            {
                String query = "select MAX(ID) from Ventas";
                using (MySqlConnection c = new MySqlConnection(getConnString()))
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, c))
                    {
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            dr.Read();
                            NextVentaID = (dr.GetInt32(0) + 1).ToString();

                        }
                    }
                    c.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
            return NextVentaID;
        }
        public bool AddItem(List<String> NuevoItem)
        {
            /*NuevoItem  
             * Codigo = 0
             * Descripcion = 1
             * Stock = 2
             * Precio = 3
             * PrecioCompra = 4
             * Familia = 5
             */
            try
            {
                String query = @"insert into Productos(Codigo, Descripcion,Stock,Precio,PrecioCompra,Familia) values"
                                + "(?cod,?desc,?stock,?precio,?preciocompra,?familia)";
                using (MySqlConnection c = new MySqlConnection(getConnString()))
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, c))
                    {
                        cmd.Parameters.Add("?cod", MySqlDbType.String).Value = NuevoItem[0];
                        cmd.Parameters.Add("?desc", MySqlDbType.String).Value = NuevoItem[1];
                        cmd.Parameters.Add("?stock", MySqlDbType.Int32).Value = NuevoItem[2];
                        cmd.Parameters.Add("?precio", MySqlDbType.Int32).Value = NuevoItem[3];
                        cmd.Parameters.Add("?preciocompra", MySqlDbType.Int32).Value = NuevoItem[4];
                        cmd.Parameters.Add("?familia", MySqlDbType.String).Value = NuevoItem[5];
                        cmd.ExecuteNonQuery();
                    }
                    c.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error agregando Item. \nRevise sus datos y compruebe que el codigo no esté duplicado\n" 
                    + e.ToString(),
                    "Error agregando Item",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        public List<List<String>> GetFullProductos(string condicion="")
        {
            List<List<String>> ProdList = new List<List<string>>();
            try
            {
                String query = "select Codigo,Descripcion,PrecioCompra,Precio,Stock from Productos " + condicion;
                using (MySqlConnection c = new MySqlConnection(getConnString()))
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, c))
                    {
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                List<String> Prod = new List<String>();
                                Prod.Add(dr.GetString(0));//cod
                                Prod.Add(dr.GetString(1));//desc
                                Prod.Add(dr.GetInt32(2).ToString());//precio
                                Prod.Add(dr.GetInt32(3).ToString());//pcompra
                                Prod.Add(dr.GetInt32(4).ToString());//stock
                                ProdList.Add(Prod);
                            }
                        }
                    }
                    c.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
            return ProdList;
        }
        public List<String> GetFullProductosStats(string condicion)
        {
            List<String> ProdStats = new List<String>();
            try
            {
                String query = "select SUM(PrecioCompra * Stock) as SumaCompra, SUM(Precio * Stock) as SumaPrecio from Productos " + condicion;
                using (MySqlConnection c = new MySqlConnection(getConnString()))
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, c))
                    {
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            List<String> Prod = new List<String>();
                            dr.Read();
                            ProdStats.Add(dr.GetInt32(0).ToString("#,##0"));//SUMCompra
                            ProdStats.Add(dr.GetInt32(1).ToString("#,##0"));//SUMVenta
                        }
                    }
                    c.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
            return ProdStats;
        }
        public List<String> GetItem(String NombreProd)
        {
            List<String> NewProd = new List<string>();
            try
            {
                String query = "select PrecioCompra, Precio, Stock from Productos where Descripcion = '" + NombreProd + "' or Codigo = '" + NombreProd + "'" +
                    "order by Codigo ASC limit 1";
                using (MySqlConnection c = new MySqlConnection(getConnString()))
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, c))
                    {
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            dr.Read();
                            NewProd.Add(dr.GetInt32(0).ToString());
                            NewProd.Add(dr.GetInt32(1).ToString());
                            NewProd.Add(dr.GetInt32(2).ToString());
                        }
                    }
                    c.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Item '" + NombreProd + "' no encontrado \n Error: " + e.GetType().ToString(), "Error",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
            return NewProd;
        }
        public void EditInventarioData(String Item, List<int> Datos)
        {
            try
            {
                String query = "UPDATE Productos SET  PrecioCompra = ?pc, Precio = ?pv, Stock = ?stock WHERE Codigo = ?cod";
                using (MySqlConnection c = new MySqlConnection(getConnString()))
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, c))
                    {
                        cmd.Parameters.Add("?pc", MySqlDbType.Int32).Value = Datos[0];
                        cmd.Parameters.Add("?pv", MySqlDbType.Int32).Value = Datos[1];
                        cmd.Parameters.Add("?stock", MySqlDbType.Int32).Value = Datos[2];
                        cmd.Parameters.Add("?cod", MySqlDbType.VarString).Value = Item;
                        cmd.ExecuteNonQuery();
                    }
                    c.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Exception",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
        }
        public List<List<String>> getRegistros(DateTime fechaInicio, DateTime fechaFin)
        {
            List<List<String>> Registros = new List<List<String>>();
            try
            {
                String query = "select ID, Fecha, Descripcion, Total, TipoPago,Costo from Ventas where Fecha BETWEEN '" +
                    fechaInicio.ToString("yyyy-MM-dd") + "' and '" + fechaFin.AddDays(1).ToString("yyyy-MM-dd") + "'";
                using (MySqlConnection c = new MySqlConnection(getConnString()))
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, c))
                    {
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                List<String> Registro = new List<String>(new string[]
                                {
                                    dr.GetInt32(0).ToString(), //ID
                                    dr.GetDateTime(1).ToString("dd/MM/yyyy"), //Fecha
                                    dr.GetString(2), //Desc
                                    "$ " + dr.GetInt32(3).ToString(), //Total
                                    dr.GetString(4),//TipoPago
                            });
                                Registros.Add(Registro);

                            }
                        }
                        c.Close();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Exception",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
            return Registros;
        }
        public List<String> GetFullVentasStats(DateTime fechaInicio, DateTime fechaFin)
        {
            List<String> ProdStats = new List<String>();
            try
            {
                String query = "select SUM(Costo) as SumaCostos, SUM(Total) as SumaTotal from Ventas where Fecha BETWEEN '" +
                    fechaInicio.ToString("yyyy-MM-dd") + "' and '" + fechaFin.AddDays(1).ToString("yyyy-MM-dd") + "'";
                using (MySqlConnection c = new MySqlConnection(getConnString()))
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, c))
                    {
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            List<String> Prod = new List<String>();
                            dr.Read();
                            ProdStats.Add(dr.GetInt32(0).ToString("#,##0"));//SumaCostos
                            ProdStats.Add(dr.GetInt32(1).ToString("#,##0"));//SumaTotal
                        }
                    }
                    c.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
            return ProdStats;
        }
    }
}
