using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace L7_Chat
{
    internal class Program
    {
        static int RemotePort;
        static int LocalPort;
        static IPAddress RemoteIP;
        static string UserName;

        static void Main(string[] args)
        {
            try
            {
                Console.SetWindowSize(40, 20);
                Console.Title = "Chat";
                Console.Write("Введите ваше имя: ");
                UserName = Console.ReadLine();
                Console.Write("Введите удалённый IP: ");
                RemoteIP = IPAddress.Parse(Console.ReadLine());
                Console.Write("Введите удалённый порт: ");
                RemotePort = Convert.ToInt32(Console.ReadLine());
                Console.Write("Введите локальный порт: ");
                LocalPort = Convert.ToInt32(Console.ReadLine());
                Thread thread = new Thread(new ThreadStart(ReceiveThread));
                thread.IsBackground = true;
                thread.Start();
                Console.ForegroundColor = ConsoleColor.Red;
                while (true)
                {
                    SendData(Console.ReadLine());
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Преобразование невозможно: " + ex.Message);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Ошибка соединения: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void ReceiveThread()
        {
            try
            {
                while (true)
                {
                    UdpClient uClient = new UdpClient(LocalPort);
                    IPEndPoint ipEnd = null;
                    byte[] response = uClient.Receive(ref ipEnd);
                    string res = Encoding.Unicode.GetString(response);

                   
                    string[] parts = res.Split(new char[] { ':' }, 2);
                    if (parts.Length == 2)
                    {
                        string username = parts[0].Trim();
                        string message = parts[1].Trim();

                       
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(username + ": ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(message);
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else
                    {
                       
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(res);
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    uClient.Close();
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Ошибка соединения: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        static void SendData(string datagram)
        {
            UdpClient uClient = new UdpClient();
            IPEndPoint ipEnd = new IPEndPoint(RemoteIP, RemotePort);
            try
            {
                string messageWithUserName = UserName + ": " + datagram;
                byte[] bytes = Encoding.Unicode.GetBytes(messageWithUserName);
                uClient.Send(bytes, bytes.Length, ipEnd);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Ошибка соединения: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                uClient.Close();
            }
        }

    }
}
