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
        bool conectado = false;
        ushort num_Mensaje = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        /*-- Menú Dispositivos --*/
        private void Yo_Click(object sender, RoutedEventArgs e)
        {
            lb_Dispositivo.Visibility = Visibility.Visible;
            lb_Nombre.Content = "Yo";
            tb_DireccionIP.Text = "127.0.0.1";
            tb_DireccionIP.IsEnabled = false;
            tb_Puerto.Text = "502";
            tb_Puerto.IsEnabled = false;
            return;
        }

        private void PC_13_Click(object sender, RoutedEventArgs e)
        {
            lb_Dispositivo.Visibility = Visibility.Visible;
            lb_Nombre.Content = "PC 13 - Lab. 019";
            tb_DireccionIP.Text = "10.172.19.13";
            tb_DireccionIP.IsEnabled = false;
            tb_Puerto.Text = "502";
            tb_Puerto.IsEnabled = false;
            return;
        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            lb_Dispositivo.Visibility = Visibility.Hidden;
            lb_Nombre.Content = "";
            tb_DireccionIP.Text = "";
            tb_DireccionIP.IsEnabled = true;
            tb_Puerto.Text = "";
            tb_Puerto.IsEnabled = true;
            return;
        }
        /*-----------------------*/

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

                if (cliente.conectarServidor())
                {
                    btn_Conectar.Content = "Desconectar";
                    btn_Peticion.IsEnabled = true;
                    Title = "Cliente Modbus/TCP (TLS) - Conectado";
                    conectado = true;
                }
                else
                    cliente = null;
            }
            else
            {
                if (cliente != null)
                    cliente.cierraCliente();

                cliente = null;
                conectado = false;
                btn_Conectar.Content = "Conectar";
                Title = "Cliente Modbus/TCP (TLS)";
                btn_Peticion.IsEnabled = false;
            }

            return;
        }

        private void btn_Peticion_Click(object sender, RoutedEventArgs e)
        {
            if (cliente != null)
            {
                ushort primera_Salida = (ushort)(Convert.ToUInt16(tb_PrimeraSalida.Text) - 1);
                ushort num_Salidas = Convert.ToUInt16(tb_NumElementos.Text);
                int nBytesEnterosSalidas = num_Salidas / 8;
                int nBytesIncompletosSalidas = num_Salidas % 8 > 0 ? 1 : 0;
                int nBytesSalidas = nBytesEnterosSalidas + nBytesIncompletosSalidas;

                byte[] peticion = new byte[12];
                byte[] respuesta = new byte[256];
                byte[] parcial;

                parcial = BitConverter.GetBytes(num_Mensaje);
                Array.Copy(parcial, 0, peticion, 0, 2);
                peticion[2] = peticion[3] = 0;
                parcial = BitConverter.GetBytes((ushort)6);
                Array.Reverse(parcial, 0, 2);
                Array.Copy(parcial, 0, peticion, 4, 2);

                peticion[6] = 22;
                peticion[7] = 1;
                parcial = BitConverter.GetBytes(primera_Salida);
                Array.Reverse(parcial, 0, 2);
                Array.Copy(parcial, 0, peticion, 8, 2);
                parcial = BitConverter.GetBytes(num_Salidas);
                Array.Reverse(parcial, 0, 2);
                Array.Copy(parcial, 0, peticion, 10, 2);

                int res = cliente.enviaDatos(peticion, 12);

                if (res == 12)
                {
                    res = cliente.recibeDatos(respuesta, respuesta.Length);

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

            return;
        }

        private void Salir_Click(object sender, RoutedEventArgs e)
        {
            if (cliente != null)
                cliente.cierraCliente();

            cliente = null;
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
