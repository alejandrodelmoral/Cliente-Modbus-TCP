using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Cliente_ModbusTCP
{
    public partial class MainWindow : Window
    {
        Cliente cliente = null;
        ClienteTLS clienteTLS = null;
        bool conectado = false;
        ushort num_mensaje = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        /*-- Menú Servidores --*/
        private void Yo_Click(object sender, RoutedEventArgs e)
        {
            tb_DireccionIP.Text = "127.0.0.1";
            tb_DireccionIP.IsEnabled = false;
            tb_Puerto.Text = "502";
            tb_Puerto.IsEnabled = false;
            return;
        }

        private void PC_13_Click(object sender, RoutedEventArgs e)
        {
            tb_DireccionIP.Text = "10.172.19.13";
            tb_DireccionIP.IsEnabled = false;
            tb_Puerto.Text = "502";
            tb_Puerto.IsEnabled = false;
            return;
        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            tb_DireccionIP.Text = "";
            tb_DireccionIP.IsEnabled = true;
            tb_Puerto.Text = "";
            tb_Puerto.IsEnabled = true;
            return;
        }
        /*---------------------*/
        
        /*-- Menú Seguridad --*/
        private void TLS_Click(object sender, RoutedEventArgs e)
        {
            return;
        }
        /*--------------------*/

        /*-- Menú Funciones --*/
        private void Func_1_Click(object sender, RoutedEventArgs e)
        {
            lb_Funcion.Content = "(1 - Lectura de salidas discretas)";
            lb_Elemento1.Content = "Primer elemento:";
            lb_Elementos.Content = "Nº elementos:";
            lb_AyudaElemento1.Content = "(1 - 320)";
            lb_AyudaElementos.Content = "(1 - 320)";

            tb_PrimeraSalida.Margin = new Thickness(110, 214, 0, 0);
            lb_Elementos.Margin = new Thickness(165, 210, 0, 0);
            tb_NumElementos.Margin = new Thickness(250, 214, 0, 0);

            lb_Elementos.Visibility = Visibility.Visible;
            tb_NumElementos.Visibility = Visibility.Visible;
            lb_Valor.Visibility = Visibility.Hidden;
            cb_Valor.Visibility = Visibility.Hidden;

            Func_1.IsChecked = true;
            Func_3.IsChecked = false;
            Func_5.IsChecked = false;
            Func_16.IsChecked = false;
        }

        private void Func_3_Click(object sender, RoutedEventArgs e)
        {
            lb_Funcion.Content = "(3 - Lectura de registros internos)";
            lb_Elemento1.Content = "Primer registro:";
            lb_Elementos.Content = "Nº registros:";
            lb_AyudaElemento1.Content = "(40001 - 40008)";
            lb_AyudaElementos.Content = "(1 - 8)";

            tb_PrimeraSalida.Margin = new Thickness(101, 214, 0, 0);
            lb_Elementos.Margin = new Thickness(165, 210, 0, 0);
            tb_NumElementos.Margin = new Thickness(241, 214, 0, 0);

            lb_Elementos.Visibility = Visibility.Visible;
            tb_NumElementos.Visibility = Visibility.Visible;
            lb_Valor.Visibility = Visibility.Hidden;
            cb_Valor.Visibility = Visibility.Hidden;

            Func_1.IsChecked = false;
            Func_3.IsChecked = true;
            Func_5.IsChecked = false;
            Func_16.IsChecked = false;
        }

        private void Func_5_Click(object sender, RoutedEventArgs e)
        {
            lb_Funcion.Content = "(5 - Modificación del estado de una salida discreta)";
            lb_Elemento1.Content = "Salida a forzar:";
            lb_AyudaElemento1.Content = "(1 - 320)";
            lb_AyudaElementos.Content = "";

            tb_PrimeraSalida.Margin = new Thickness(97, 214, 0, 0);
            lb_Valor.Margin = new Thickness(165, 210, 0, 0);
            cb_Valor.Margin = new Thickness(205, 216, 0, 0);

            lb_Elementos.Visibility = Visibility.Hidden;
            tb_NumElementos.Visibility = Visibility.Hidden;
            lb_Valor.Visibility = Visibility.Visible;
            cb_Valor.Visibility = Visibility.Visible;

            Func_1.IsChecked = false;
            Func_3.IsChecked = false;
            Func_5.IsChecked = true;
            Func_16.IsChecked = false;
        }

        private void Func_16_Click(object sender, RoutedEventArgs e)
        {
            lb_Funcion.Content = "(16 - Modificar el valor de registros internos)";

            Func_1.IsChecked = false;
            Func_3.IsChecked = false;
            Func_5.IsChecked = false;
            Func_16.IsChecked = true;

            lb_Valor.Visibility = Visibility.Visible;
            cb_Valor.Visibility = Visibility.Visible;
        }
        /*--------------------*/

        /*-- Menú Ayuda --*/
        private void Sobre_Click(object sender, RoutedEventArgs e)
        {
            Sobre sobre = new Sobre();
            sobre.Show();
        }
        /*----------------*/
        
        /*-- Botones --*/
        private void btn_Conectar_Click(object sender, RoutedEventArgs e)
        {
            if (!conectado)
            {
                cliente = new Cliente(tb_DireccionIP.Text, Convert.ToInt32(tb_Puerto.Text));
                clienteTLS = new ClienteTLS(tb_DireccionIP.Text, Convert.ToInt32(tb_Puerto.Text));

                if (cliente.conectarServidor() || clienteTLS.conectarServidor(tb_CertifServ.Text, tb_CertifClie.Text == "" ? null : tb_CertifClie.Text))

                //if (cliente.conectarServidor())
                {
                    btn_Conectar.Content = "Desconectar";
                    btn_Conectar.Background = new SolidColorBrush(Colors.Red);
                    btn_Peticion.IsEnabled = true;
                    Title = "Cliente Modbus/TCP (TLS) - Conectado";
                    if (tb_DireccionIP.Text == "127.0.0.1")
                        lb_Nombre.Content = "Conectado a: Yo";
                    else if (tb_DireccionIP.Text == "10.172.19.13")
                        lb_Nombre.Content = "Conectado a: PC 13 - Lab. 019";
                    conectado = true;
                }
                else
                {
                    cliente = null;
                    clienteTLS = null;
                }
            }
            else
            {
                if ((cliente != null) || (clienteTLS != null))
                {
                    cliente.cierraCliente();
                    clienteTLS.cierraCliente();
                }

                cliente = null;
                clienteTLS = null;
                conectado = false;
                btn_Conectar.Content = "Conectar";
                btn_Conectar.Background = new SolidColorBrush(Colors.Lime);
                Title = "Cliente Modbus/TCP (TLS)";
                lb_Nombre.Content = "";
                btn_Peticion.IsEnabled = false;
            }

            return;
        }

        private void btn_Peticion_Click(object sender, RoutedEventArgs e)
        {
            if ((cliente != null) || (clienteTLS != null))
            {
                if (Func_1.IsChecked == true)
                {
                    ushort primera_Salida = (ushort)(Convert.ToUInt16(tb_PrimeraSalida.Text) - 1);
                    ushort num_Salidas = Convert.ToUInt16(tb_NumElementos.Text);
                    int nBytesEnterosSalidas = num_Salidas / 8;
                    int nBytesIncompletosSalidas = num_Salidas % 8 > 0 ? 1 : 0;
                    int nBytesSalidas = nBytesEnterosSalidas + nBytesIncompletosSalidas;

                    byte[] peticion = new byte[12];
                    byte[] respuesta = new byte[256];
                    byte[] parcial;

                    parcial = BitConverter.GetBytes(num_mensaje);
                    Array.Copy(parcial, 0, peticion, 0, 2);
                    peticion[2] = peticion[3] = 0;
                    parcial = BitConverter.GetBytes((ushort)6);
                    Array.Reverse(parcial, 0, 2);
                    Array.Copy(parcial, 0, peticion, 4, 2);

                    peticion[6] = 2;
                    peticion[7] = 1;
                    parcial = BitConverter.GetBytes(primera_Salida);
                    Array.Reverse(parcial, 0, 2);
                    Array.Copy(parcial, 0, peticion, 8, 2);
                    parcial = BitConverter.GetBytes(num_Salidas);
                    Array.Reverse(parcial, 0, 2);
                    Array.Copy(parcial, 0, peticion, 10, 2);

                    int res = cliente.enviaDatos(peticion, peticion.Length);
                    //int res = clienteTLS.enviaDatos(peticion, peticion.Length);

                    if (res == 12)
                    {
                        res = cliente.recibeDatos(respuesta, respuesta.Length);
                        //res = clienteTLS.recibeDatos(respuesta, respuesta.Length);

                        if (res == nBytesSalidas + 9)
                        {
                            List<datosGrid> lista = new List<datosGrid>();
                            datosGrid elemento;
                            bool[] temp;

                            int k = primera_Salida + 1;
                            int maxBits;

                            for (int i = 0; i < respuesta[8]; i++)
                            {
                                temp = extraeBits(respuesta[9 + i], 8);

                                if (i < nBytesEnterosSalidas)
                                    maxBits = 8;
                                else
                                    maxBits = num_Salidas % 8;

                                for (int j = 0; j < maxBits; j++)
                                {
                                    elemento = new datosGrid();
                                    elemento.Elemento = k++;
                                    elemento.Estado = temp[j];
                                    lista.Add(elemento);
                                }
                            }

                            dg_Salidas.ItemsSource = lista;
                        }
                    }
                }

                if (Func_3.IsChecked == true)
                {
                    ushort primera_Salida = (ushort)(Convert.ToUInt16(tb_PrimeraSalida.Text) - 40001);
                    ushort num_Salidas = Convert.ToUInt16(tb_NumElementos.Text);
                    int nBytesEnterosSalidas = num_Salidas * 2;
                    int nBytesSalidas = nBytesEnterosSalidas;

                    byte[] peticion = new byte[12];
                    byte[] respuesta = new byte[256];
                    byte[] parcial;

                    parcial = BitConverter.GetBytes(num_mensaje);
                    Array.Copy(parcial, 0, peticion, 0, 2);
                    peticion[2] = peticion[3] = 0;
                    parcial = BitConverter.GetBytes((ushort)6);
                    Array.Reverse(parcial, 0, 2);
                    Array.Copy(parcial, 0, peticion, 4, 2);

                    peticion[6] = 2;
                    peticion[7] = 3;
                    parcial = BitConverter.GetBytes(primera_Salida);
                    Array.Reverse(parcial, 0, 2);
                    Array.Copy(parcial, 0, peticion, 8, 2);
                    parcial = BitConverter.GetBytes(num_Salidas);
                    Array.Reverse(parcial, 0, 2);
                    Array.Copy(parcial, 0, peticion, 10, 2);

                    int res = cliente.enviaDatos(peticion, peticion.Length);
                    //int res = clienteTLS.enviaDatos(peticion, peticion.Length);

                    if (res == 12)
                    {
                        res = cliente.recibeDatos(respuesta, respuesta.Length);
                        //res = clienteTLS.recibeDatos(respuesta, respuesta.Length);

                        if (res == nBytesSalidas + 9)
                        {
                            List<datosGrid> lista = new List<datosGrid>();
                            datosGrid elemento;
                            bool[] temp;

                            int k = primera_Salida + 1;
                            int maxBits;
                            //int cont = 0;
                            byte[] aux = new byte[2];

                            for (int i = 0; i < respuesta[8] / 2; i++)
                            {
                                aux[0] = respuesta[(i * 2) + 9];
                                aux[1] = respuesta[(i * 2) + 10];
                                
                                Array.Reverse(aux, 0, 2);
                                Array.Copy(aux, 0, respuesta, 9 + (i * 2), 2);
                                //cont += 2;
                            }

                            for (int i = 0; i < respuesta[8]; i++)
                            {
                                temp = extraeBits(respuesta[9 + i], 8);

                                if (i < nBytesEnterosSalidas)
                                    maxBits = 8;
                                else
                                    maxBits = num_Salidas % 8;

                                for (int j = 0; j < maxBits; j++)
                                {
                                    elemento = new datosGrid();
                                    elemento.Elemento = k++;
                                    elemento.Estado = temp[j];
                                    lista.Add(elemento);
                                }
                            }

                            dg_Salidas.ItemsSource = lista;
                        }
                    }
                }

                if (Func_5.IsChecked == true)
                {
                    ushort primera_Salida = (ushort)(Convert.ToUInt16(tb_PrimeraSalida.Text) - 1);

                    byte[] peticion = new byte[12];
                    byte[] respuesta = new byte[256];
                    byte[] parcial;

                    parcial = BitConverter.GetBytes(num_mensaje);
                    Array.Copy(parcial, 0, peticion, 0, 2);
                    peticion[2] = peticion[3] = 0;
                    parcial = BitConverter.GetBytes((ushort)6);
                    Array.Reverse(parcial, 0, 2);
                    Array.Copy(parcial, 0, peticion, 4, 2);

                    peticion[6] = 2;
                    peticion[7] = 5;
                    parcial = BitConverter.GetBytes(primera_Salida);
                    Array.Reverse(parcial, 0, 2);
                    Array.Copy(parcial, 0, peticion, 8, 2);

                    if (cb_Valor.IsChecked == true)
                        peticion[10] = 0xFF;
                    else
                        peticion[10] = 0;

                    peticion[11] = 0;

                    int res = cliente.enviaDatos(peticion, peticion.Length);
                    //int res = clienteTLS.enviaDatos(peticion, peticion.Length);

                    if (res == 12)
                    {
                        res = cliente.recibeDatos(respuesta, respuesta.Length);
                        //res = clienteTLS.recibeDatos(respuesta, respuesta.Length);

                        if (res == 12)
                        {
                            List<datosGrid> lista = new List<datosGrid>();
                            datosGrid elemento;
                            bool[] temp;

                            elemento = new datosGrid();
                            elemento.Elemento = primera_Salida + 1;
                            elemento.Estado = BitConverter.ToBoolean(respuesta, 10);
                            lista.Add(elemento);

                            dg_Salidas.ItemsSource = lista;

                        }
                    }
                }
            }

            return;
        }

        private void Salir_Click(object sender, RoutedEventArgs e)
        {
            if ((cliente != null) || clienteTLS != null)
            {
                cliente.cierraCliente();
                clienteTLS.cierraCliente();
            }

            cliente = null;
            clienteTLS = null;
            Close();
            return;
        }
        /*-------------*/

        private bool[] extraeBits(byte valor, int n)
        {
            bool[] solucion = new bool[8];
            byte mascara = 0x01;

            for (int i = 0; i < n; i++)
            {
                if ((valor & mascara) != 0)
                    solucion[i] = true;
                else
                    solucion[i] = false;

                mascara = (byte)(mascara << 1);
            }

            for (int i = n; i < 8; i++)
                solucion[i] = false;

            return solucion;
        }

    }

    public class datosGrid
    {
        public int Elemento { get; set; }
        public bool Estado { get; set; }
    }

}
