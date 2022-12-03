using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cliente_ModbusTCP
{
    public class Cliente
    {
        private Socket cliente = null;
        private IPEndPoint ipep = null;
        private bool conectado = false;

        public Cliente(string direccionIP, Int32 puerto)
        {
            try
            {
                IPAddress direccion = IPAddress.Parse(direccionIP);
                ipep = new IPEndPoint(direccion, puerto);
                cliente = new Socket(direccion.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                cliente.SendTimeout = 500;
                cliente.ReceiveTimeout = 1000;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general al preparar la conexión al servidor:\n" + ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public bool conectarServidor()
        {
            try
            {
                cliente.Connect(ipep);
            }
            catch (SocketException ex_sock)
            {
                if (ex_sock.SocketErrorCode == SocketError.ConnectionRefused)
                    MessageBox.Show("Error al conectarse al servidor:\nAntes de poner en marcha el cliente, se debe poner en funcionamiento el servidor en el nodo:\nIP: " + ipep.Address.ToString() + " - Puerto: " + ipep.Port.ToString() + "\nAtención al \"firewall\"", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show("Error al conectarse al servidor:\n" + ex_sock.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general al conectarse al servidor:\n" + ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            finally
            {
                if (cliente != null && cliente.Connected)
                    conectado = true;
                else
                    conectado = false;
            }

            return conectado;
        }

        public void cierraCliente()
        {
            if (cliente != null && conectado)
                cliente.Close();

            cliente = null;
            conectado = false;
        }

        public int enviaDatos(byte[] datos, int dim)
        {
            try
            {
                if (conectado)
                {
                    int res = cliente.Send(datos, dim, SocketFlags.None);

                    if (res == dim)
                        return res;
                    else if (res == 0)
                    {
                        cliente = null;
                        conectado = false;
                        return -1;
                    }
                    else
                        return -2;
                }
                else
                    return -1;
            }
            catch (SocketException ex_sock)
            {
                if (ex_sock.SocketErrorCode == SocketError.TimedOut)
                    return 0;
                else if (ex_sock.SocketErrorCode == SocketError.ConnectionReset)
                {
                    cliente = null;
                    conectado = false;
                    return -1;
                }
                else
                    return -2;
            }
            catch (Exception ex)
            {
                return -2;
            }
        }

        public int recibeDatos(byte[] datos, int dimMax)
        {
            try
            {
                if (conectado)
                {
                    int res = cliente.Receive(datos, dimMax, SocketFlags.None);

                    if (res > 0)
                        return res;
                    else if (res == 0)
                    {
                        cliente = null;
                        conectado = false;
                        return -1;
                    }
                    else
                        return -2;
                }
                else
                    return -1;
            }
            catch (SocketException ex_sock)
            {
                if (ex_sock.SocketErrorCode == SocketError.TimedOut)
                    return 0;
                else if (ex_sock.SocketErrorCode == SocketError.ConnectionReset)
                {
                    cliente = null;
                    conectado = false;
                    return -1;
                }
                else
                    return -2;
            }
            catch (Exception ex)
            {
                return -2;
            }
        }

    }
}
