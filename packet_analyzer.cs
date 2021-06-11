using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

using System.Net.Sockets;

namespace game_server
{
    class packet_analyzer
    {
        private static Dictionary<string, encryption> TemporarySessionCreator = new Dictionary<string, encryption>();
                
        public static void StartSessionTCPInput(string data, Socket player_socket)
        {
            try
            {
                string[] packet_data = data.Split('~');

                if (packet_data.Length >= 3 && (packet_data[0] + packet_data[1] + packet_data[2]) == "060")
                {
                    string code = functions.get_random_set_of_symb(5);
                    encryption session_encryption = new encryption();
                    Console.WriteLine(DateTime.Now + ": user requested encryption from - " + player_socket.RemoteEndPoint.ToString());
                    server.SendDataTCP(player_socket, $"0~6~0~{code}~{session_encryption.publicKeyInString}");

                    if (!TemporarySessionCreator.ContainsKey(code))
                        TemporarySessionCreator.Add(code, session_encryption);
                    Task.Run(() => CleanTempSession(code));
                    
                    return;
                    //return $"0~6~0~{code}~{encoded secret key}";
                }

                if (packet_data.Length >= 3 && (packet_data[0] + packet_data[1] + packet_data[2]) == "061" && TemporarySessionCreator.ContainsKey(packet_data[3]))
                {
                    byte[] secret_key = TemporarySessionCreator[packet_data[3]].GetSecretKey(packet_data[4]);
                    server.Sessions.Add(packet_data[3], secret_key);
                    TemporarySessionCreator[packet_data[3]].Dispose();
                    TemporarySessionCreator.Remove(packet_data[3]);
                    Console.WriteLine(DateTime.Now + ": user received encryption and accepted - " + player_socket.RemoteEndPoint.ToString());
                    server.SendDataTCP(player_socket, $"0~6~1~ok");
                    
                    return;
                    //return $"0~6~1~OK";
                }

                if (packet_data.Length == 4 && (packet_data[0] + packet_data[1] + packet_data[2]) == "062" && server.Sessions.ContainsKey(packet_data[3]))
                {
                    server.Sessions.Remove(packet_data[3]);
                    Console.WriteLine(DateTime.Now + ": user removed from current encryption - " + player_socket.RemoteEndPoint.ToString());
                    server.SendDataTCP(player_socket, $"0~6~2~ok");
                    
                    return;
                }


                if ((packet_data[0] + packet_data[1]) == "05")
                {

                    if (packet_data[2] != starter.InnerServerConnectionPassword)
                    {
                        Console.WriteLine(DateTime.Now + ": error 0~5~wp for another server from " + player_socket.RemoteEndPoint.ToString());
                        server.SendDataTCP(player_socket, $"0~5~wp");
                        return;
                    }

                    //Console.Write("command: " + packet_data[3]);

                    bool ins_char_name = mysql.ExecuteSQLInstruction(packet_data[3]).Result;
                    if (!ins_char_name)
                    {
                        Console.WriteLine(DateTime.Now + ": error in exec instruction for another server from " + player_socket.RemoteEndPoint.ToString());
                        server.SendDataTCP(player_socket, $"0~5~er");
                        return;
                    }
                    else
                    {
                        Console.WriteLine(DateTime.Now + ": sql successfully executed for another server from " + player_socket.RemoteEndPoint.ToString());
                        server.SendDataTCP(player_socket, $"0~5~ok");
                        return;
                    }


                }

                //ping command 0~7~pass
                if ((packet_data[0] + packet_data[1]) == "07")
                {
                    Console.WriteLine(DateTime.Now + ": received ping from " + player_socket.RemoteEndPoint.ToString());
                    server.SendDataTCP(player_socket, $"0~7~ok~{starter.SessionsPool.Count}");
                    return;
                }
                //return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine("==============ERROR================\n" + ex + "\n" + data + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
            }            

        }


        public static void ProcessUDPActivePacket(string data)
        {
            
            string RawPacket = data;
            string[] RawDataArray = RawPacket.Split('~');

            
            if (RawDataArray.Length>0)
            {
                
                try
                {
                    if (starter.SessionsPool.ContainsKey(RawDataArray[3]))
                    {                        
                        //encryption.Decode(ref data, starter.SessionsPool[starter.PacketIDPool[RawDataArray[0]].SessionID].LocalPlayersPool[starter.PacketIDPool[RawDataArray[0]].PlayerID].secret_key);
                        
                        functions.MainProcess(data);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("==============ERROR================\n" + ex + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
                }
            }        
        }


        public static void ProcessUDPinitPlayer(EndPoint endPoint, string RawPacket, byte [] secret_key, string temp)
        {
            //order~spec~player_id~session_id~horiz~vertic
            

            string[] RawDataArray = RawPacket.Split('~');
            Console.WriteLine(DateTime.Now + ": received request for UDP from "+ RawDataArray[2] + " - "  + endPoint.ToString());
            if (RawDataArray.Length > 0)
            {
                try
                {                    
                    if (starter.SessionsPool.ContainsKey(RawDataArray[3]))
                    {
                        starter.SessionsPool[RawDataArray[3]].LocalPlayersPool[RawDataArray[2]].endPointUDP = endPoint;
                        starter.SessionsPool[RawDataArray[3]].LocalPlayersPool[RawDataArray[2]].secret_key = secret_key;
                        server.UDPClientsENDPoints.Add(endPoint);
                        //Console.WriteLine("Added new endpoint to new player " + starter.SessionsPool[RawDataArray[3]].LocalPlayersPool[RawDataArray[2]].player_name + " - " + RawDataArray[2] + " - " + temp);
                        Console.WriteLine(DateTime.Now + ": add new player to UDP: " + RawDataArray[2] + ", session - " + RawDataArray[3] + " from " + endPoint.ToString());
                    }                   

                }
                catch (Exception ex)
                {
                    Console.WriteLine("==============ERROR================\n" + ex + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
                }
                
            }
                        
        }

        public static void ProcessUDPShutDownPlayer(EndPoint endPoint, string session_id)
        {            
            //Console.WriteLine("Shut down " + endPoint.ToString());

            foreach (Players p in starter.SessionsPool[session_id].LocalPlayersPool.Values)
            {
                if (p.endPointUDP== endPoint)
                {
                    p.endPointUDP = null;
                }
            }                        
        }



        public static string ProcessTCPPacket(string RawPacket, string endpoint_address)
        {
            // 0~4~player_id~session_id - data for each player about team and ur spells
            // 0~5~passpass~ SQL instruction - executes SQL in local database
            // 0~6~player_id~session_id~base part~ mod part~ open key1 ~ open key2~ open key 3


            string[] RawDataArray = RawPacket.Split('~');

            if (RawDataArray.Length<4)
            {
                Console.WriteLine(DateTime.Now + ": wrong packet lenth from" + endpoint_address);
                return "0";
            }
            
            
            switch(RawDataArray[1])
            {
                

                case "2":// 0~2~passpass~session_id - starts tick of current session and inits the players
                    

                    try
                    {
                        if (RawDataArray[2] != starter.InnerServerConnectionPassword)
                        {
                            Console.WriteLine(DateTime.Now + ": wrong inner connection password from " + endpoint_address);
                            return "0";
                        }

                        if (!StringChecker(RawDataArray[2]) || !StringChecker(RawDataArray[3]))
                        {
                            Console.WriteLine(DateTime.Now + ": wrong numerics from " + endpoint_address);
                            return "0";
                        }

                        string sessionname = RawDataArray[3];

                        if (starter.SessionsPool.ContainsKey(sessionname))
                        {
                            //Console.WriteLine("session " + sessionname + " allready exists");
                            Console.WriteLine(DateTime.Now + $": error creating session {sessionname}: such session allready exists");
                            return "0";
                        }


                        string sql1 = "SELECT `player_order`, `player_id`, `player_name`, `player_class`, `connection_number`, `team_id`, `game_type_id`, `zone_type`, `position_x`, `position_y`, `position_z`, `rotation_x`, `rotation_y`, `rotation_z`, `speed`, `animation_id`, `conditions`, `health_pool`, `energy`, `health_regen`, `energy_regen`, `weapon_attack`, `hit_power`, `armor`, `shield_block`, `magic_resistance`, `dodge`, `cast_speed`, `melee_crit`, `magic_crit`, `spell_power`, `spell1`, `spell2`, `spell3`, `spell4`, `spell5`, `hidden_conds`, `global_button_cooldown` FROM `" + sessionname + "`";
                        string[,] result1 = mysql.GetMysqlSelect(sql1).Result;


                        //get the list of arrays with data for each player to init player
                        List<string[]> datausers = new List<string[]>(result1.GetLength(0));
                        for (int u = 0; u < result1.GetLength(0); u++)
                        {
                            
                            datausers.Add(new string[result1.GetLength(1)]);
                            for (int i = 0; i < result1.GetLength(1); i++)
                            {
                                datausers[u].SetValue(result1[u, i], i);
                            }
                        }
                        //init all players with arrays
                        Sessions NewSession = new Sessions(datausers.Count, sessionname, int.Parse(result1[0,7]), int.Parse(result1[0, 6]));
                        string who_is_in_session = "";

                        

                        for (int i = 0; i < datausers.Count; i++)
                        {
                            
                            Players new_player = new Players(sessionname, datausers[i]);                            
                            NewSession.LocalPlayersPool.Add(new_player.player_id, new_player);
                            who_is_in_session = who_is_in_session + new_player.player_id + ", ";
                        }                        
                        Console.WriteLine(DateTime.Now +  ": session " + sessionname + " created with player IDs:" + who_is_in_session);
                        return "1";
                    } catch (Exception ex)
                    {
                        Console.WriteLine("==============ERROR================\n" + ex + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
                        Console.WriteLine(DateTime.Now + $": error creating session {RawDataArray[3]}");
                    }
                    
                    return "0";
                    break;


                case "4"://order~specification~player_id~session_id
                    

                    try
                    {
                        if (!starter.SessionsPool[RawDataArray[3]].LocalPlayersPool.ContainsKey(RawDataArray[2]))
                        {
                            Console.WriteLine(DateTime.Now + ": no such player ID " + RawDataArray[2] + " in session " + RawDataArray[3] + "... from " + endpoint_address);
                            return "0";
                        }

                        if (!StringChecker(RawDataArray[2]) || !StringChecker(RawDataArray[3]))
                        {
                            Console.WriteLine(DateTime.Now + ": wrong numerics from " + endpoint_address);
                            return "0";
                        }

                        string answer = RawDataArray[0] + "~" + RawDataArray[1] + "~" + starter.SessionsPool[RawDataArray[3]].PlayersCount.ToString() + "~";

                        string sql = "SELECT `player_order`, `zone_type`, `player_name`, `player_class`, `team_id`, `spell1`, `spell2`, `spell3`, `spell4`, `spell5` FROM `" + RawDataArray[3] + "` WHERE `player_id`='"+ RawDataArray[2] + "'";
                        string[,] result = mysql.GetMysqlSelect(sql).Result;

                        //$result = $result.$data_conn[0]['player_order']."~".$data_conn[0]['player_name']."~".$data_conn[0]['player_class']."~".$data_conn[0]['team_id']."~".$data_conn[0]['spell1']."~".$data_conn[0]['spell2']."~".$data_conn[0]['spell3']."~".$data_conn[0]['spell4']."~".$data_conn[0]['spell5']."~".$data_conn[0]['spell6']."~";
                        for (int i=0; i<result.GetLength(1); i++)
                        {
                            answer = answer + result[0, i] + "~";
                        }

                        sql = "SELECT `player_order`, `player_name`, `player_class`, `team_id` FROM `" + RawDataArray[3] + "` WHERE `player_id`!='" + RawDataArray[2] + "'";
                        result = mysql.GetMysqlSelect(sql).Result;

                        for (int i=0; i<result.GetLength(0); i++)
                        {
                            answer = answer + result[i, 0] + "~" + result[i, 1] + "~" + result[i, 2] + "~" + result[i, 3] + "~";
                        }
                        Console.WriteLine(DateTime.Now + $": data about session send to player " + RawDataArray[2] + " of session - " + RawDataArray[3] + "from " + endpoint_address);
                        return answer;

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("==============ERROR================\n" + ex + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
                        Console.WriteLine(DateTime.Now + $": ERROR sending data about session to player " + RawDataArray[2] + " of session - " + RawDataArray[3] + "from " + endpoint_address);
                    }
                    return "0";

                    break;


                case "5": //ord~spec~pass~data
                    

                    try
                    {
                        if (RawDataArray[2] != starter.InnerServerConnectionPassword)
                        {
                            return "0";
                        }

                        if (!StringChecker(RawDataArray[2]) || !StringChecker(RawDataArray[3]))
                        {
                            Console.WriteLine(DateTime.Now + ": wrong numerics from " + endpoint_address);
                            return "0";
                        }

                        if (mysql.ExecuteSQLInstruction(RawDataArray[3]).Result)
                        {
                            return "1";
                        }
                        else
                        {
                            return "0";
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("==============ERROR================\n" + ex + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");

                    }
                    return "0";
                    break;

               
            }

            return "";

        }


        private static async void CleanTempSession(string index)
        {
            await Task.Delay(60000);
            if (TemporarySessionCreator.ContainsKey(index))
            {
                TemporarySessionCreator.Remove(index);
            }
        }

        public static bool StringChecker(string data_to_check)
        {
            if (data_to_check.All(char.IsLetterOrDigit))
            {
                return true;
            }

            return false;
        }


    }
}
