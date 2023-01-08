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
using System.Windows.Threading;

namespace Cliente_ModbusTCP
{
    public partial class MainWindow : Window
    {
        Cliente cliente = null;
        ClienteTLS clienteTLS = null;

        bool conectado = false;
        bool seguro = false;
        bool enter = false;
        bool valores = false;

        ushort num_mensaje = 0;
        ushort num_Salidas = 0;

        int cont = 0;
        int nBytesEnterosSalidas = 0;

        byte[] parte_func16;

        private DispatcherTimer Temp_Func16;

        public MainWindow()
        {
            InitializeComponent();

            Temp_Func16 = new DispatcherTimer();
            Temp_Func16.Interval = new TimeSpan(0, 0, 0, 0, 50);
            Temp_Func16.Tick += new EventHandler(Tick_Func16);
            Temp_Func16.Start();
        }

        /*-- Menú Servidores --*/
        private void Yo_Click(object sender, RoutedEventArgs e)
        {
            tb_DireccionIP.Text = "127.0.0.1";
            tb_DireccionIP.IsEnabled = false;

            tb_Puerto.Text = "502";
            tb_Puerto.IsEnabled = false;

            Yo.IsChecked = true;
            PC_13.IsChecked = false;

            return;
        }

        private void PC_13_Click(object sender, RoutedEventArgs e)
        {
            tb_DireccionIP.Text = "10.172.19.13";
            tb_DireccionIP.IsEnabled = false;

            tb_Puerto.Text = "502";
            tb_Puerto.IsEnabled = false;
            
            Yo.IsChecked = false;
            PC_13.IsChecked = true;

            return;
        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            tb_DireccionIP.Text = "";
            tb_DireccionIP.IsEnabled = true;

            tb_Puerto.Text = "";
            tb_Puerto.IsEnabled = true;
            
            Yo.IsChecked = false;
            PC_13.IsChecked = false;
            
            return;
        }
        /*---------------------*/

        /*-- Menú Seguridad --*/
        private void TLS_Click(object sender, RoutedEventArgs e)
        {
            if (TLS.IsChecked)
            {
                lb_SAN.Foreground = new SolidColorBrush(Colors.Black);
                tb_CertifServ.IsEnabled = true;

                lb_DN.Foreground = new SolidColorBrush(Colors.Black);
                tb_CertifClie.IsEnabled = true;
            }
            else
            {
                lb_SAN.Foreground = new SolidColorBrush(Colors.Gray);
                tb_CertifServ.IsEnabled = false;

                lb_DN.Foreground = new SolidColorBrush(Colors.Gray);
                tb_CertifClie.IsEnabled = false;
            }

            if (conectado)
                btn_Conectar_Click(sender, e);

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
            btn_Valores.Visibility = Visibility.Hidden;
            lb_parte_func16.Visibility = Visibility.Hidden;
            tb_parte_func16.Visibility = Visibility.Hidden;
            tb_parte_func16.Width = 35;
            lb_AyudaParteFunc16.Content = "";
            rbtn_Hex.Visibility = Visibility.Hidden;
            rbtn_Dec.Visibility = Visibility.Hidden;
            rbtn_Hex.IsChecked = true;
            rbtn_Dec.IsChecked = false;

            tb_PrimeraSalida.Text = "";
            tb_NumElementos.Text = "";
            cb_Valor.IsChecked = false;
            rtb_Func.Visibility = Visibility.Hidden;
            rtb_Func.Document.Blocks.Clear();

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
            btn_Valores.Visibility = Visibility.Hidden;
            lb_parte_func16.Visibility = Visibility.Hidden;
            tb_parte_func16.Visibility = Visibility.Hidden;
            tb_parte_func16.Width = 35;
            lb_AyudaParteFunc16.Content = "";
            rbtn_Hex.Visibility = Visibility.Hidden;
            rbtn_Dec.Visibility = Visibility.Hidden;
            rbtn_Hex.IsChecked = true;
            rbtn_Dec.IsChecked = false;

            tb_PrimeraSalida.Text = "";
            tb_NumElementos.Text = "";
            cb_Valor.IsChecked = false;
            rtb_Func.Visibility = Visibility.Hidden;
            rtb_Func.Document.Blocks.Clear();

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
            btn_Valores.Visibility = Visibility.Hidden;
            lb_parte_func16.Visibility = Visibility.Hidden;
            tb_parte_func16.Visibility = Visibility.Hidden;
            tb_parte_func16.Width = 35;
            lb_AyudaParteFunc16.Content = "";
            rbtn_Hex.Visibility = Visibility.Hidden;
            rbtn_Dec.Visibility = Visibility.Hidden;
            rbtn_Hex.IsChecked = true;
            rbtn_Dec.IsChecked = false;

            tb_PrimeraSalida.Text = "";
            tb_NumElementos.Text = "";
            cb_Valor.IsChecked = false;
            rtb_Func.Visibility = Visibility.Hidden;
            rtb_Func.Document.Blocks.Clear();

            Func_1.IsChecked = false;
            Func_3.IsChecked = false;
            Func_5.IsChecked = true;
            Func_16.IsChecked = false;
        }

        private void Func_16_Click(object sender, RoutedEventArgs e)
        {
            lb_Funcion.Content = "(16 - Modificar el valor de registros internos)";
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
            btn_Valores.Visibility = Visibility.Visible;
            lb_parte_func16.Visibility = Visibility.Hidden;
            tb_parte_func16.Visibility = Visibility.Hidden;
            tb_parte_func16.Width = 35;
            lb_AyudaParteFunc16.Content = "";
            rbtn_Hex.Visibility = Visibility.Hidden;
            rbtn_Dec.Visibility = Visibility.Hidden;
            rbtn_Hex.IsChecked = true;
            rbtn_Dec.IsChecked = false;
            tb_parte_func16.Margin = new Thickness(500, 214, 0, 0);
            lb_AyudaParteFunc16.Margin = new Thickness(535, 211, 0, 0);

            tb_PrimeraSalida.Text = "";
            tb_NumElementos.Text = "";
            cb_Valor.IsChecked = false;
            rtb_Func.Visibility = Visibility.Hidden;
            rtb_Func.Document.Blocks.Clear();

            Func_1.IsChecked = false;
            Func_3.IsChecked = false;
            Func_5.IsChecked = false;
            Func_16.IsChecked = true;
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
                if (TLS.IsChecked == true)
                {
                    seguro = true;
                    clienteTLS = new ClienteTLS(tb_DireccionIP.Text, Convert.ToInt32(tb_Puerto.Text));

                    if (clienteTLS.conectarServidor(tb_CertifServ.Text, tb_CertifClie.Text == "" ? null : tb_CertifClie.Text))
                    {
                        btn_Conectar.Content = "Desconectar";
                        btn_Conectar.Background = new SolidColorBrush(Colors.Red);
                        btn_Peticion.IsEnabled = true;
                        btn_Valores.IsEnabled = true;
                        Title = "Cliente Modbus/TCP (TLS) - Conectado con seguridad";

                        if (tb_DireccionIP.Text == "127.0.0.1")
                            lb_Nombre.Content = "Conectado a: Yo";
                        else if (tb_DireccionIP.Text == "10.172.19.13")
                            lb_Nombre.Content = "Conectado a: PC 13 - Lab. 019";

                        conectado = true;
                    }
                    else
                        clienteTLS = null;
                }
                else
                {
                    seguro = false;
                    cliente = new Cliente(tb_DireccionIP.Text, Convert.ToInt32(tb_Puerto.Text));

                    if (cliente.conectarServidor())
                    {
                        btn_Conectar.Content = "Desconectar";
                        btn_Conectar.Background = new SolidColorBrush(Colors.Red);
                        btn_Peticion.IsEnabled = true;
                        btn_Valores.IsEnabled = true;
                        Title = "Cliente Modbus/TCP (TLS) - Conectado sin seguridad";

                        if (tb_DireccionIP.Text == "127.0.0.1")
                            lb_Nombre.Content = "Conectado a: Yo";
                        else if (tb_DireccionIP.Text == "10.172.19.13")
                            lb_Nombre.Content = "Conectado a: PC 13 - Lab. 019";

                        conectado = true;
                    }
                    else
                        cliente = null;
                }
            }
            else
            {
                if (seguro)
                {
                    if (clienteTLS != null)
                        clienteTLS.cierraCliente();

                    clienteTLS = null;
                    conectado = false;
                    seguro = false;
                    btn_Conectar.Content = "Conectar";
                    btn_Conectar.Background = new SolidColorBrush(Colors.Lime);
                    Title = "Cliente Modbus/TCP (TLS)";
                    lb_Nombre.Content = "";
                    btn_Peticion.IsEnabled = false;
                    btn_Valores.IsEnabled = false;
                }
                else
                {
                    if (cliente != null)
                        cliente.cierraCliente();

                    cliente = null;
                    conectado = false;
                    seguro = false;
                    btn_Conectar.Content = "Conectar";
                    btn_Conectar.Background = new SolidColorBrush(Colors.Lime);
                    Title = "Cliente Modbus/TCP (TLS)";
                    lb_Nombre.Content = "";
                    btn_Peticion.IsEnabled = false;
                    btn_Valores.IsEnabled = false;
                }
            }

            return;
        }

        private void btn_Peticion_Click(object sender, RoutedEventArgs e)
        {
            if ((cliente != null) || (clienteTLS != null))
            {
                if (Func_1.IsChecked == true)
                    Funcion1();

                if (Func_3.IsChecked == true)
                    Funcion3();

                if (Func_5.IsChecked == true)
                    Funcion5();

                if (Func_16.IsChecked == true)
                {
                    tb_parte_func16.Text = "";
                    lb_parte_func16.Visibility = Visibility.Hidden;
                    tb_parte_func16.Visibility = Visibility.Hidden;
                    Funcion16();
                }
            }

            return;
        }

        private void btn_Valores_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                num_Salidas = Convert.ToUInt16(tb_NumElementos.Text);
                nBytesEnterosSalidas = num_Salidas * 2;
                parte_func16 = new byte[nBytesEnterosSalidas];

                lb_parte_func16.Visibility = Visibility.Visible;
                tb_parte_func16.Visibility = Visibility.Visible;
                tb_parte_func16.Width = 35;

                rbtn_Hex.Visibility = Visibility.Visible;
                rbtn_Dec.Visibility = Visibility.Visible;

                valores = true;

                return;
            }
            catch (Exception ex)
            {
                valores = false;
                MessageBox.Show("Debe indicar el número de registros que desea modificar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Salir_Click(object sender, RoutedEventArgs e)
        {
            if ((cliente != null) || clienteTLS != null)
            {
                if (seguro)
                {
                    clienteTLS.cierraCliente();
                    clienteTLS = null;
                }
                else
                {
                    cliente.cierraCliente();
                    cliente = null;
                }
            }

            Close();
            return;
        }

        private void tb_parte_func16_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                enter = true;
            else
                enter = false;

            return;
        }
        /*-------------*/

        /*-- Timers --*/
        private void Tick_Func16(object sender, EventArgs e)
        {
            if (valores)
            {
                byte[] parcial;
                int registro;

                registro = Convert.ToUInt16(tb_PrimeraSalida.Text) + cont;

                lb_AyudaParteFunc16.Content = "(Pulse Intro para avanzar)";
                tb_parte_func16.IsEnabled = true;

                if (rbtn_Dec.IsChecked == true)
                {
                    tb_parte_func16.MaxLength = 5;
                    lb_parte_func16.Content = "Registro " + registro + ": 10x";
                    tb_parte_func16.Margin = new Thickness(505, 214, 0, 0);
                    lb_AyudaParteFunc16.Margin = new Thickness(540, 211, 0, 0);
                }
                else
                {
                    tb_parte_func16.MaxLength = 4;
                    lb_parte_func16.Content = "Registro " + registro + ": 0x";
                    tb_parte_func16.Margin = new Thickness(500, 214, 0, 0);
                    lb_AyudaParteFunc16.Margin = new Thickness(535, 211, 0, 0);
                }

                try
                {
                    if (enter && cont < num_Salidas)
                    {
                        if (rbtn_Dec.IsChecked == true)
                        {
                            parcial = BitConverter.GetBytes(int.Parse(tb_parte_func16.Text));
                            Array.Reverse(parcial, 0, 2);
                            Array.Copy(parcial, 0, parte_func16, cont * 2, 2);
                        }
                        else
                        {
                            parcial = BitConverter.GetBytes(int.Parse(tb_parte_func16.Text, System.Globalization.NumberStyles.HexNumber));
                            Array.Reverse(parcial, 0, 2);
                            Array.Copy(parcial, 0, parte_func16, cont * 2, 2);
                        }

                        cont++;
                        tb_parte_func16.Text = "";
                        enter = false;
                    }
                }
                catch (Exception ex)
                {
                    enter = false;
                    MessageBox.Show("Debe indicar el valor que desea.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                

                if (cont == num_Salidas)
                {
                    valores = false;
                    cont = 0;
                    tb_parte_func16.IsEnabled = false;
                    tb_parte_func16.Width = 130;
                    lb_AyudaParteFunc16.Content = "";
                    tb_parte_func16.Text = "Pulse el botón Petición";
                }
            }
        }
        /*------------*/

        /*-- Funciones --*/
        private void Funcion1()
        {
            ushort primera_Salida = (ushort)(Convert.ToUInt16(tb_PrimeraSalida.Text) - 1);
            ushort num_Salidas = Convert.ToUInt16(tb_NumElementos.Text);
            int nBytesEnterosSalidas = num_Salidas / 8;
            int nBytesIncompletosSalidas = num_Salidas % 8 > 0 ? 1 : 0;
            int nBytesSalidas = nBytesEnterosSalidas + nBytesIncompletosSalidas;

            byte[] peticion = new byte[12];
            byte[] respuesta = new byte[256];
            byte[] parcial;
            int res;

            // peticion[0 - 1]
            parcial = BitConverter.GetBytes(num_mensaje);
            Array.Copy(parcial, 0, peticion, 0, 2);

            // peticion[2 - 3]
            peticion[2] = peticion[3] = 0;

            // peticion[4 - 5]
            parcial = BitConverter.GetBytes((ushort)6);
            Array.Reverse(parcial, 0, 2);
            Array.Copy(parcial, 0, peticion, 4, 2);

            // peticion[6 - 7]
            peticion[6] = 2;
            peticion[7] = 1;

            // peticion[8 - 9]
            parcial = BitConverter.GetBytes(primera_Salida);
            Array.Reverse(parcial, 0, 2);
            Array.Copy(parcial, 0, peticion, 8, 2);

            // peticion[10 - 11]
            parcial = BitConverter.GetBytes(num_Salidas);
            Array.Reverse(parcial, 0, 2);
            Array.Copy(parcial, 0, peticion, 10, 2);

            // Enviar petición
            if (seguro)
                res = clienteTLS.enviaDatos(peticion, peticion.Length);
            else
                res = cliente.enviaDatos(peticion, peticion.Length);

            // Recibir respuesta
            if (res == 12)
            {
                if (seguro)
                    res = clienteTLS.recibeDatos(respuesta, respuesta.Length);
                else
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

            return;
        }

        private void Funcion3()
        {
            ushort primera_Salida = (ushort)(Convert.ToUInt16(tb_PrimeraSalida.Text) - 40001);
            ushort num_Salidas = Convert.ToUInt16(tb_NumElementos.Text);
            int nBytesEnterosSalidas = num_Salidas * 2;
            int nBytesSalidas = nBytesEnterosSalidas;

            byte[] peticion = new byte[12];
            byte[] respuesta = new byte[256];
            byte[] parcial;
            int res;

            // peticion[0 - 1]
            parcial = BitConverter.GetBytes(num_mensaje);
            Array.Copy(parcial, 0, peticion, 0, 2);

            // peticion[2 - 3]
            peticion[2] = peticion[3] = 0;

            // peticion[4 - 5]
            parcial = BitConverter.GetBytes((ushort)6);
            Array.Reverse(parcial, 0, 2);
            Array.Copy(parcial, 0, peticion, 4, 2);

            // peticion[6 - 7]
            peticion[6] = 2;
            peticion[7] = 3;

            // peticion[8 - 9]
            parcial = BitConverter.GetBytes(primera_Salida);
            Array.Reverse(parcial, 0, 2);
            Array.Copy(parcial, 0, peticion, 8, 2);

            // peticion[10 - 11]
            parcial = BitConverter.GetBytes(num_Salidas);
            Array.Reverse(parcial, 0, 2);
            Array.Copy(parcial, 0, peticion, 10, 2);

            // Enviar petición
            if (seguro)
                res = clienteTLS.enviaDatos(peticion, peticion.Length);
            else
                res = cliente.enviaDatos(peticion, peticion.Length);

            // Recibir respuesta
            if (res == 12)
            {
                if (seguro)
                    res = clienteTLS.recibeDatos(respuesta, respuesta.Length);
                else
                    res = cliente.recibeDatos(respuesta, respuesta.Length);

                if (res == nBytesSalidas + 9)
                {
                    List<datosGrid> lista = new List<datosGrid>();
                    datosGrid elemento;
                    bool[] temp;

                    int k = 0;

                    for (int i = 0; i < primera_Salida + 1; i++)
                    {
                        if (primera_Salida == i)
                            k = i * 16 + 1;
                    }

                    int maxBits;
                    byte[] aux = new byte[2];

                    for (int i = 0; i < respuesta[8] / 2; i++)
                    {
                        aux[0] = respuesta[(i * 2) + 9];
                        aux[1] = respuesta[(i * 2) + 10];

                        Array.Reverse(aux, 0, 2);
                        Array.Copy(aux, 0, respuesta, 9 + (i * 2), 2);
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
                    rtb_Func.Document.Blocks.Clear();
                    rtb_Func.Visibility = Visibility.Visible;

                    for (int i = 0; i < respuesta[8] / 2; i++)
                    {
                        int registro = 0;
                        registro = Convert.ToUInt16(tb_PrimeraSalida.Text) + i;
                        rtb_Func.AppendText("Registro " + registro + ": 0x" + respuesta[(i * 2) + 10].ToString("X2") + respuesta[(i * 2) + 9].ToString("X2") + "\r");
                    }
                }
            }

            return;
        }

        private void Funcion5()
        {
            ushort primera_Salida = (ushort)(Convert.ToUInt16(tb_PrimeraSalida.Text) - 1);

            byte[] peticion = new byte[12];
            byte[] respuesta = new byte[256];
            byte[] parcial;
            int res;

            // peticion[0 - 1]
            parcial = BitConverter.GetBytes(num_mensaje);
            Array.Copy(parcial, 0, peticion, 0, 2);

            // peticion[2 - 3]
            peticion[2] = peticion[3] = 0;

            // peticion[4 - 5]
            parcial = BitConverter.GetBytes((ushort)6);
            Array.Reverse(parcial, 0, 2);
            Array.Copy(parcial, 0, peticion, 4, 2);

            // peticion[6 - 7]
            peticion[6] = 2;
            peticion[7] = 5;

            // peticion[8 - 9]
            parcial = BitConverter.GetBytes(primera_Salida);
            Array.Reverse(parcial, 0, 2);
            Array.Copy(parcial, 0, peticion, 8, 2);

            // peticion[10]
            if (cb_Valor.IsChecked == true)
                peticion[10] = 0xFF;
            else
                peticion[10] = 0;

            // peticion[11]
            peticion[11] = 0;

            // Enviar petición
            if (seguro)
                res = clienteTLS.enviaDatos(peticion, peticion.Length);
            else
                res = cliente.enviaDatos(peticion, peticion.Length);

            // Recibir respuesta
            if (res == 12)
            {
                if (seguro)
                    res = clienteTLS.recibeDatos(respuesta, respuesta.Length);
                else
                    res = cliente.recibeDatos(respuesta, respuesta.Length);

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

            return;
        }

        private void Funcion16()
        {
            ushort primera_Salida = (ushort)(Convert.ToUInt16(tb_PrimeraSalida.Text) - 40001);
            ushort num_Salidas = Convert.ToUInt16(tb_NumElementos.Text);
            int nBytesEnterosSalidas = num_Salidas * 2;
            int nBytesSalidas = nBytesEnterosSalidas;

            byte[] peticion = new byte[13 + nBytesSalidas];
            byte[] respuesta = new byte[256];
            byte[] parcial;
            int res;

            // peticion[0 - 1]
            parcial = BitConverter.GetBytes(num_mensaje);
            Array.Copy(parcial, 0, peticion, 0, 2);

            // peticion[2 - 3]
            peticion[2] = peticion[3] = 0;

            // peticion[4 - 5]
            parcial = BitConverter.GetBytes((ushort)(7 + nBytesSalidas));
            Array.Reverse(parcial, 0, 2);
            Array.Copy(parcial, 0, peticion, 4, 2);

            // peticion[6 - 7]
            peticion[6] = 2;
            peticion[7] = 16;

            // peticion[8 - 9]
            parcial = BitConverter.GetBytes(primera_Salida);
            Array.Reverse(parcial, 0, 2);
            Array.Copy(parcial, 0, peticion, 8, 2);

            // peticion[10 - 11]
            parcial = BitConverter.GetBytes(num_Salidas);
            Array.Reverse(parcial, 0, 2);
            Array.Copy(parcial, 0, peticion, 10, 2);

            // peticion[12]
            peticion[12] = (byte)nBytesSalidas;

            // peticion[13...]
            Array.Copy(parte_func16, 0, peticion, 13, nBytesSalidas);

            // Enviar petición
            if (seguro)
                res = clienteTLS.enviaDatos(peticion, peticion.Length);
            else
                res = cliente.enviaDatos(peticion, peticion.Length);

            // Recibir respuesta
            if (res == 13 + nBytesSalidas)
            {
                if (seguro)
                    res = clienteTLS.recibeDatos(respuesta, respuesta.Length);
                else
                    res = cliente.recibeDatos(respuesta, respuesta.Length);
            }

            return;
        }
        /*---------------*/

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

    /*-- Clase datosGrid --*/
    public class datosGrid
    {
        public int Elemento { get; set; }
        public bool Estado { get; set; }
    }
    /*---------------------*/

}