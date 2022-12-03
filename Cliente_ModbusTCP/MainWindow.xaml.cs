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

    }
}
