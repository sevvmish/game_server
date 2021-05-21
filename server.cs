using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using NetCoreServer;

namespace game_server
{
    class server
    {
        public static Dictionary<string, byte[]> Sessions = new Dictionary<string, byte[]>();
        //UDP
        private static Socket socket_udp;
        private const int port_udp = 2325;
        private static IPEndPoint ipendpoint_udp;
        private static EndPoint endpoint_udp;
        public static UDPServerConnector ServerUDP;
        //НЕ ЗАБУДЬ ПРИДУМАТЬ, КАК ЧИСТИТЬ ХЕШ НА УЖЕ ОТРАБОТАННЫЕ ЭНДПОИНТЫ
        //public static HashSet<EndPoint> UDPClientsENDPoints = new HashSet<EndPoint>();

        //TCP
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private static IPAddress ipaddress_tcp;
        private static IPEndPoint localendpoint_tcp;
        private static Socket socket_tcp;
        private const int port_tcp = 2323;
        private const int max_connections = 1000;

        //General
        public static HashSet<EndPoint> UDPClientsENDPoints = new HashSet<EndPoint>();
        //private static StringBuilder raw_data_received_tcp = new StringBuilder(1024);
        //private static StringBuilder raw_data_received_udp = new StringBuilder(128);
        //private static StringBuilder raw_data_received_udp = new StringBuilder(1024);
        //private static byte[] buffer_received_udp = new byte[256];
        //private static byte[] buffer_send_udp = new byte[1024];
        //private static byte[] buffer_received_tcp = new byte[256];
        private static byte[] buffer_send_tcp = new byte[1024];

        //INITIAL STARTER FOR TCP AND UDP
        public static void Server_init()
        {
            //Task.Run(() => Server_init_TCP());
            Task.Run(() =>
            {
                ServerUDP = new UDPServerConnector(IPAddress.Any, port_udp);
                ServerUDP.Start();
                //Server_init_UDP();


            });
            Server_init_TCP();
            //Task.Run(() => Server_init_UDP());            
        }

        //START FOR TCP
        public static void Server_init_TCP()
        {
            //TCP config===================================
            ipaddress_tcp = IPAddress.Any;
            localendpoint_tcp = new IPEndPoint(ipaddress_tcp, port_tcp);
            socket_tcp = new Socket(ipaddress_tcp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine(DateTime.Now + ": " + "game server TCP initiated");

            try
            {
                socket_tcp.Bind(localendpoint_tcp);
                socket_tcp.Listen(max_connections);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();
                    
                    socket_tcp.BeginAccept(new AsyncCallback(AcceptCallbackTCP), socket_tcp);

                    allDone.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("==============ERROR================\n" + ex + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
            }
            //TCP config===================================
        }


       

        public static Task SendDataUDP(EndPoint ipEnd, string data)
        {
            try
            {
                ServerUDP.SendAsync(ipEnd, data);
                //Console.WriteLine("out&" + data + "$" + starter.stopWatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine("==============ERROR================\n" + ex + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
            }
            return Task.CompletedTask;
        }

        public static Task SendDataUDP(EndPoint ipEnd, byte[] data)
        {
            try
            {
                ServerUDP.SendAsync(ipEnd, data);
                //Console.WriteLine("out&" + data + "$" + starter.stopWatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine("==============ERROR================\n" + ex + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
            }
            return Task.CompletedTask;
        }

       

        public static void AcceptCallbackTCP(IAsyncResult ar)
        {
            try
            {
                // Signal the main thread to continue.  
                allDone.Set();
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = handler;
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallbackTCP), state);
            }
            catch (Exception ex)
            {
                Console.WriteLine("==============ERROR================\n" + ex + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
            }

        }

        public static void ReadCallbackTCP(IAsyncResult ar)
        {
            try
            {
                //raw_data_received_tcp.Clear();

                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    //raw_data_received_tcp.Append(Encoding.UTF8.GetString(buffer_received_tcp, 0, bytesRead));
                    //Console.WriteLine(raw_data_received_tcp + " : " + handler.RemoteEndPoint.ToString());
                    state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                    //Console.WriteLine(state.sb + " : " + handler.RemoteEndPoint.ToString());


                    if (!Sessions.ContainsKey(Encoding.UTF8.GetString(state.buffer, 0, 5)))
                    {
                        if (Encoding.UTF8.GetString(state.buffer, 0, 4)=="0~5~")
                        {
                            byte[] d = new byte[bytesRead];
                            for (int i = 0; i < bytesRead; i++)
                            {
                                d[i] = state.buffer[i];
                            }

                            encryption.Decode(ref d, starter.secret_key_for_game_servers);

                            packet_analyzer.StartSessionTCPInput(Encoding.UTF8.GetString(d), handler);

                        } 
                        else if (Encoding.UTF8.GetString(state.buffer, 0, 4) == "0~2~")
                        {
                            byte[] d = new byte[bytesRead];
                            for (int i = 0; i < bytesRead; i++)
                            {
                                d[i] = state.buffer[i];
                            }

                            encryption.Decode(ref d, starter.secret_key_for_game_servers);

                            string res = packet_analyzer.ProcessTCPPacket(Encoding.UTF8.GetString(d), handler.RemoteEndPoint.ToString());
                            
                            SendDataTCP(handler, $"0~2~{res}");
                        }
                        else
                        {
                            packet_analyzer.StartSessionTCPInput(Encoding.UTF8.GetString(state.buffer, 0, bytesRead), handler);
                        }
                        
                    }
                    else
                    {
                        
                        byte[] d = new byte[bytesRead];
                        for (int i = 0; i < bytesRead; i++)
                        {
                            d[i] = state.buffer[i];
                        }
                        
                        encryption.Decode(ref d, Sessions[Encoding.UTF8.GetString(state.buffer, 0, 5)]);

                        string back_result = packet_analyzer.ProcessTCPPacket(Encoding.UTF8.GetString(d).Remove(0, 6), handler.RemoteEndPoint.ToString());
                        
                        //Console.WriteLine(back_result);
                        byte[] t = Encoding.UTF8.GetBytes(back_result);
                        encryption.Encode(ref t, Sessions[Encoding.UTF8.GetString(state.buffer, 0, 5)]);
                        
                        SendDataTCP(handler, t);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("==============ERROR================\n" + ex + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
            }

        }


        public static Task SendDataTCP(Socket handler, String data)
        {
            try
            {
                //Console.WriteLine("send: " + data);
                // Convert the string data to byte data using ASCII encoding.  
                buffer_send_tcp = Encoding.UTF8.GetBytes(data);

                // Begin sending the data to the remote device.  
                handler.BeginSend(buffer_send_tcp, 0, buffer_send_tcp.Length, 0, new AsyncCallback(SendCallback), handler);
            }
            catch (Exception ex)
            {
                Console.WriteLine("==============ERROR================\n" + ex + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
            }
            return Task.CompletedTask;
        }

        public static Task SendDataTCP(Socket handler, byte [] data)
        {
            try
            {
                //Console.WriteLine("send: " + Encoding.UTF8.GetString(data));
                // Convert the string data to byte data using ASCII encoding.  
                buffer_send_tcp = data;

                // Begin sending the data to the remote device.  
                handler.BeginSend(buffer_send_tcp, 0, buffer_send_tcp.Length, 0, new AsyncCallback(SendCallback), handler);
            }
            catch (Exception ex)
            {
                Console.WriteLine("==============ERROR================\n" + ex + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
            }
            return Task.CompletedTask;
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("==============ERROR================\n" + ex + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
            }
        }



    }

    // State object for reading client data asynchronously  
    public class StateObject
    {
        // Size of receive buffer.  
        public const int BufferSize = 1024;

        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];

        // Received data string.
        public StringBuilder sb = new StringBuilder();

        // Client socket.
        public Socket workSocket = null;
    }


    class UDPServerConnector : UdpServer
    {

        private StringBuilder raw_data_received_udp = new StringBuilder(1024);

        public UDPServerConnector(IPAddress address, int port) : base(address, port) { }


        protected override void OnStarted()
        {
            // Start receive datagrams
            try
            {
                Console.WriteLine(DateTime.Now + ": " + "game server UDP initiated");
                ReceiveAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("==============ERROR================\n" + ex + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
            }

        }

        protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {

            if (size == 0)
            {
                /*
                foreach (Sessions current_session in starter.SessionsPool.Values)
                {
                    foreach (Players current_player in current_session.LocalPlayersPool.Values)
                    {
                        if (current_player.endPointUDP!=null)
                        {
                            //Console.WriteLine("awaiting " + (starter.stopWatch.ElapsedMilliseconds - current_player.AveragePing[current_player.AveragePing.Count-1]).ToString());
                            if ((starter.stopWatch.ElapsedMilliseconds - current_player.AveragePing[current_player.AveragePing.Count - 1])>30000)
                            {
                                current_player.endPointUDP = null;
                            }
                        }
                    }
                }
                */
                // Important: Receive using thread pool is necessary here to avoid stack overflow with Socket.ReceiveFromAsync() method!
                ThreadPool.QueueUserWorkItem(o => { ReceiveAsync(); });
                //Console.Write("uuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuu");
            }
            else
            {

                //raw_data_received_udp.Clear();
                //raw_data_received_udp.Append(Encoding.UTF8.GetString(buffer, 0, (int)size));
                //Console.WriteLine("real_in&" + raw_data_received_udp + "$" + starter.stopWatch.ElapsedMilliseconds);
                //Console.WriteLine(raw_data_received_udp + " - just before");

                //List<byte> b = new List<byte>();
                byte[] t = new byte[(int)size];

                for (int i = 0; i < (int)size; i++)
                {
                    //b.Add(buffer[i]);
                    t[i] = buffer[i];
                }

                

                if (server.Sessions.ContainsKey(Encoding.UTF8.GetString(buffer, 0, 5)))
                {
                    
                    encryption.Decode(ref t, server.Sessions[Encoding.UTF8.GetString(buffer, 0, 5)]);
                    
                    raw_data_received_udp.Clear();
                    string data_result = raw_data_received_udp.Append(Encoding.UTF8.GetString(t, 0, t.Length)).ToString().Remove(0,6);

                    if (!server.UDPClientsENDPoints.Contains(endpoint))
                    {
                        packet_analyzer.ProcessUDPinitPlayer(endpoint, data_result, server.Sessions[Encoding.UTF8.GetString(buffer, 0, 5)], Encoding.UTF8.GetString(buffer, 0, 5));
                    }

                    packet_analyzer.ProcessUDPActivePacket(data_result);
                }



            }
            //ThreadPool.QueueUserWorkItem(o => { ReceiveAsync(); });


            try
            {
                ReceiveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Onreceived - " + ex);
            }

            // Echo the message back to the sender
            //SendAsync(endpoint, buffer, offset, size);
        }

        /*
        protected override void OnSent(EndPoint endpoint, long sent)
        {
            // Continue receive datagrams.
            // Important: Receive using thread pool is necessary here to avoid stack overflow with Socket.ReceiveFromAsync() method!

            
            try
            {
                ThreadPool.QueueUserWorkItem(o => { ReceiveAsync(); });
            }
            catch (Exception ex)
            {
                Console.WriteLine("OnSend reactor " + ex);
            }
            

            //ThreadPool.QueueUserWorkItem(o => { ReceiveAsync(); });

            Console.Write("uuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuu");
            
        }*/

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Server caught an error with code {error} ");

            if (error == SocketError.AddressNotAvailable)
            {

            }

            //server.ServerUDP.Restart();

        }

    }



}
