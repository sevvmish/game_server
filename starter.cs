using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetCoreServer;
using System.Collections.Concurrent;
using System.Numerics;

namespace game_server
{
    
    public class starter
    {
        public static byte[] secret_key_for_game_servers;
        public static string InnerServerConnectionPassword;
        public static string MysqlConnectionData_login;
        public static string address_for_data_config = @"C:\android\data1";

        public const float GlobalButtonCooldown = 0.7f;
        public const float GlobalTickFloat = 0.05f;
        public const int GlobalTick = 50;
        public static Dictionary<string, Sessions> SessionsPool = new Dictionary<string, Sessions>();
        public static Dictionary<string, SessionData> PacketIDPool = new Dictionary<string, SessionData>();
        //public static Dictionary<string, Players> PlayersPool = new Dictionary<string, Players>();
        public const float def_hit_melee_dist = 2.3f;
        public const float def_hit_melee_angle = 35;
        public const float def_hit_melee_small_dist = 1.5f;
        public const float def_hit_melee_small_angle = 50;
        public const float def_hit_melee_min_radius = 1;
        public const float armor_max = 1000;
        public const float max_free_regeneration = 50;
        public static Stopwatch stopWatch = new Stopwatch();

        

        public static int Main(String[] args)
        {
            stopWatch.Start();
            data_config.Init_data_config();


            Console.WriteLine(packet_analyzer.ProcessTCPPacket("0~2~passpass~session", "testing"));
            //TimeSpan ts = stopWatch.Elapsed;
            
            

            server.Server_init();
            
            
            
            
            Console.WriteLine("ready to exit...");
            Console.ReadKey();            
            return 0;
        }




    }


    public struct SessionData
    {
        public string PlayerID;
        public string SessionID;

        public SessionData(string pl, string sess)
        {
            PlayerID = pl;
            SessionID = sess;
        }
    }
}