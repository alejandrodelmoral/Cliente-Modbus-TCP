using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.ComponentModel;

namespace Cliente_ModbusTCP
{
    public class ClienteTLS
    {
        private Socket cliente = null;
        private NetworkStream streamTCP = null;
        private SslStream streamTLS = null;
        private IPEndPoint ipep = null;
        private bool conectado = false;

        public ClienteTLS(string direccionIP, Int32 puerto)
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

        public static bool validarCertificadoServidor(object sender, X509Certificate certificadoServidor, X509Chain cadenaCertificacion, SslPolicyErrors errores)
        {
            if (errores == SslPolicyErrors.None)
                return true;
            else
                MessageBox.Show("Error en el certificado del servidor: " + errores.ToString(), "Error en la conexión al servidor seguro", MessageBoxButton.OK, MessageBoxImage.Error);

            return false;
        }

        public bool conectarServidor(string nombreCertificadoServidor, string nombreCertificadoCliente)
        {
            try
            {
                cliente.Connect(ipep);
                streamTCP = new NetworkStream(cliente, true);
                streamTLS = new SslStream(streamTCP, false, new RemoteCertificateValidationCallback(validarCertificadoServidor), null);
                streamTLS.WriteTimeout = 500;
                streamTLS.ReadTimeout = 100000;
                X509Certificate2Collection coleccionCertificadosCliente = null;

                if (nombreCertificadoCliente != null)
                {
                    X509Store repositorioMY = new X509Store(StoreLocation.CurrentUser);
                    repositorioMY.Open(OpenFlags.ReadOnly);
                    X509Certificate2Collection todosCertificados = repositorioMY.Certificates;
                    coleccionCertificadosCliente = todosCertificados.Find(X509FindType.FindBySubjectDistinguishedName, nombreCertificadoCliente, false);
                    repositorioMY.Close();
                }

                streamTLS.AuthenticateAsClient(nombreCertificadoServidor, coleccionCertificadosCliente, SslProtocols.Tls12 | SslProtocols.Tls11, false);
                streamTLS.ReadTimeout = 1000;
            }
            catch (AuthenticationException ex_auth)
            {
                MessageBox.Show("Error en la autenticación: " + ex_auth.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                cierraCliente();
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
                if (streamTLS != null && streamTLS.CanRead && streamTLS.CanWrite)
                    conectado = true;
                else
                    conectado = false;
            }

            return conectado;
        }

        public void cierraCliente()
        {
            if (streamTLS != null)
                streamTLS.Close();
            else if (streamTCP != null)
                streamTCP.Close();
            else if (cliente != null)
                cliente.Close();

            streamTLS = null;
            streamTCP = null;
            cliente = null;
            conectado = false;

            return;
        }

        public int enviaDatos(byte[] datos, int dim)
        {
            try
            {
                if (conectado)
                {
                    streamTLS.Write(datos, 0, dim);
                    streamTLS.Flush();
                    return dim;
                }
                else
                    return -1;
            }
            catch (Exception ex) when (ex.InnerException is Win32Exception)
            {
                if (((Win32Exception)ex.InnerException).ErrorCode == 10053 || ((Win32Exception)ex.InnerException).ErrorCode == 10054)
                    return -1;
                else
                    return -2;
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
            int leidos = 0;
            int timeoutRead = streamTLS.ReadTimeout;

            try
            {
                if (conectado)
                {
                    int res;

                    while (leidos < dimMax)
                    {
                        res = streamTLS.Read(datos, leidos, dimMax - leidos);

                        if (res > 0)
                        {
                            leidos += res;
                            streamTLS.ReadTimeout = 100;
                        }
                        else if (res == 0)
                            break;
                    }

                    return leidos;
                }
                else
                    return -1;
            }
            catch (ObjectDisposedException ex_Ssl)
            {
                return -1;
            }
            catch (Exception ex) when (ex.InnerException is Win32Exception)
            {
                if (((Win32Exception)ex.InnerException).ErrorCode == 10060)
                    return leidos;
                else if (((Win32Exception)ex.InnerException).ErrorCode == 10053 || ((Win32Exception)ex.InnerException).ErrorCode == 10054)
                    return -1;
                else
                    return -2;
            }
            catch (SocketException ex_sock)
            {
                if (ex_sock.SocketErrorCode == SocketError.TimedOut)
                    return leidos;
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
            finally
            {
                streamTLS.ReadTimeout = timeoutRead;
            }
        }

    }
}
