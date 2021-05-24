using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace game_server
{
    public class Sessions: IDisposable
    {
        public int PlayersCount;
        public int ZoneType;
        public Dictionary<string, Players> LocalPlayersPool;
        public string Session_id, environment_packet;

        private List<Players> LocalEnvironmentPool = new List<Players>();
        
        private delegate void Environment();
        private Environment CurrentEnvironment;

        public Sessions(int NumberOfPlayers, string sess_id, int zone)
        {
            Session_id = sess_id;
            ZoneType = zone;
            PlayersCount = NumberOfPlayers;
            
            //Console.WriteLine(ZoneType + " - zone");
            LocalPlayersPool = new Dictionary<string, Players>(NumberOfPlayers);
            //CurrentTimer = new Timer(sendpackets, 0, 0, starter.GlobalTick);
            //Thread t = new Thread(sendpackets);
            //t.Start();
            //sendpackets();
            //GetTwice();
            do_every_tick();
            starter.SessionsPool.Add(sess_id, this);
            Console.WriteLine("server session " + sess_id + " started to tick");

            switch (ZoneType)
            {
                case 1:
                    /*
                    Players env1 = new Players(10, functions.get_random_set_of_symb(5), "test_player", 1, "0", -1, 1, 0, 0, 0, 0, 45, 0, 1, 0, "200=200", 100, 1, 1, "10-15", 20, 200, 0, 10, 10, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0);
                    Players env2 = new Players(10, functions.get_random_set_of_symb(5), "test_player", 5, "0", -1, 1, 0.5f, 0.5f, 0, 0, 45, 0, 1, 0, "200=200", 100, 1, 1, "10-15", 20, 200, 0, 10, 10, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0);
                    LocalPlayersPool.Add(env1.player_id, env1);
                    LocalEnvironmentPool.Add(env1);
                    LocalPlayersPool.Add(env2.player_id, env2);
                    LocalEnvironmentPool.Add(env2);
                    */
                    CurrentEnvironment = Location1;
                    break;
                
                case 2:

                    break;
            }

        }

       

        private void Location1()
        {
            


        }

       
        private async void do_every_tick()
        {
            long CurrentTime = starter.stopWatch.ElapsedMilliseconds;
            

            while (true)
            {
                if (CurrentTime < starter.stopWatch.ElapsedMilliseconds)
                {
                    
                    foreach (Players CurrentPlayer in LocalPlayersPool.Values)
                    {
                        //free regen
                        CurrentPlayer.CheckForFreeRegeneration();

                        if (CurrentPlayer.energy < 100)
                        {
                            CurrentPlayer.energy = CurrentPlayer.energy + CurrentPlayer.energy_regen * 0.05f;
                        }

                        string[] curr_health = CurrentPlayer.health_pool.Split('=');
                        float health_curr = float.Parse(curr_health[0]);
                        if (health_curr < float.Parse(curr_health[1]))
                        {
                            health_curr = health_curr + CurrentPlayer.health_regen * 0.05f;
                        }
                        CurrentPlayer.health_pool = health_curr + "=" + curr_health[1];

                    }

                    if (CurrentEnvironment != null)
                    {
                        CurrentEnvironment();
                        
                    }

                    

                    if (LocalEnvironmentPool.Count>0)
                    {

                        environment_packet = "";
                        for (int i = 0; i < LocalEnvironmentPool.Count; i++)
                        {
                            environment_packet = environment_packet + LocalEnvironmentPool[i].GetPacketForSending_nonPlayer();
                        }
                    }

                    
                    //Console.WriteLine("tick" + starter.stopWatch.ElapsedMilliseconds);
                    CurrentTime += 50;
                    if (CurrentTime> starter.stopWatch.ElapsedMilliseconds)
                    {
                        //Console.WriteLine((CurrentTime - starter.stopWatch.ElapsedMilliseconds).ToString());
                        //Thread.Sleep( (int)(CurrentTime - starter.stopWatch.ElapsedMilliseconds));
                        await Task.Delay((int)(CurrentTime - starter.stopWatch.ElapsedMilliseconds));
                    }
                } 
                   
            }
        }



        public void Dispose()
        {
            foreach (Players p in LocalPlayersPool.Values)
            {
                //if (UDPServerConnector.UDPClientsENDPoints.Contains(p.endPointUDP))
                //{
                 //   UDPServerConnector.UDPClientsENDPoints.Remove(p.endPointUDP);
                //}
                if (starter.PacketIDPool.ContainsKey(p.PacketID))
                {
                    starter.PacketIDPool.Remove(p.PacketID);
                }

                p.Dispose();

            }


            this.Dispose();
        }
        
    }
}
