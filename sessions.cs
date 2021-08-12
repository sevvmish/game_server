using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;

namespace game_server
{
    public class Sessions: IDisposable
    {
        public int PlayersCount;
        public int ZoneType;
        public int GameType;
        public Dictionary<string, Players> LocalPlayersPool;
        public string Session_id, environment_packet;

        private List<Players> LocalEnvironmentPool = new List<Players>();
        
        private delegate void Environment();
        private Environment CurrentEnvironment;

        private int Round;
        private bool isRoundChecked;

        public Sessions(int NumberOfPlayers, string sess_id, int zone, int game_type)
        {
            Session_id = sess_id;
            ZoneType = zone;
            GameType = game_type;
            PlayersCount = NumberOfPlayers;
            Round++;

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

       
        private void ResetGameData()
        {
            foreach (Players CurrentPlayer in LocalPlayersPool.Values)
            {
                CurrentPlayer.isDead = false;                
                CurrentPlayer.is_reset_any_button = false;
                CurrentPlayer.conditions.Clear();

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
                if (CurrentTime < starter.stopWatch.ElapsedMilliseconds) //CurrentTime < starter.stopWatch.ElapsedMilliseconds
                {
                     
                    
                    foreach (Players CurrentPlayer in LocalPlayersPool.Values)
                    {
                        
                        CheckingRoundConditions();

                        if (float.Parse(CurrentPlayer.health_pool.Split('=')[0])<=0 && !CurrentPlayer.isDead && !isRoundChecked)
                        {
                            CurrentPlayer.isDead = true;
                            CurrentPlayer.animation_id = 22;
                            CurrentPlayer.is_reset_any_button = true;
                            CurrentPlayer.conditions.Clear();
                            CurrentPlayer.CurrentSpecial = null;
                            
                            CurrentPlayer.conditions.TryAdd(functions.get_symb_for_IDs(), $":co-1006-0,");
                            
                        } 
                        else if (float.Parse(CurrentPlayer.health_pool.Split('=')[0]) > 0 && !CurrentPlayer.isDead && isRoundChecked)
                        {
                          
                        }

                        
                        if (isRoundChecked)
                        {
                           
                        }
                        
                        
                        if (1==1 && !CurrentPlayer.isDead && !isRoundChecked) //CurrentPlayer.endPointUDP != null   ПОТОМ ИСПРАВЬ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        {
                            //specials
                            CurrentPlayer.CurrentSpecial?.Invoke();

                          
                            //free regen
                            CurrentPlayer.CheckForFreeRegeneration();

                            if (CurrentPlayer.energy < 100)
                            {
                                CurrentPlayer.energy = CurrentPlayer.energy + CurrentPlayer.energy_regen * 0.05f*2;
                            }

                           //health regen
                            string[] curr_health = CurrentPlayer.health_pool.Split('=');
                            float health_curr = float.Parse(curr_health[0]);
                            if (health_curr < float.Parse(curr_health[1]))
                            {
                                health_curr = health_curr + CurrentPlayer.health_regen * 0.05f;
                            }
                            CurrentPlayer.health_pool = health_curr + "=" + curr_health[1];

                           
                            //checker for BST 0000 state=========================
                            if ((CurrentPlayer.button1 || CurrentPlayer.button2 || CurrentPlayer.button3 || CurrentPlayer.button4 || CurrentPlayer.button5 || CurrentPlayer.button6) && !CurrentPlayer.is_spell_button_touched)
                            {
                                CurrentPlayer.is_spell_button_touched = true;
                                if (CurrentPlayer.start_cheking_if_spell_touched)
                                {
                                    CurrentPlayer.is_spell_touched_for_casting_failing = true;
                                }
                            }
                            else if ((!CurrentPlayer.button1 && !CurrentPlayer.button2 && !CurrentPlayer.button3 && !CurrentPlayer.button4 && !CurrentPlayer.button5 && !CurrentPlayer.button6) && CurrentPlayer.is_spell_button_touched)
                            {
                                CurrentPlayer.is_spell_button_touched = false;
                            }
                            //====================================================

                            //check RAB  reset any button touch=================================
                            if (CurrentPlayer.is_reset_any_button)
                            {
                                CurrentPlayer.ZeroInputs();
                            }

                            //spell in process - reset only spell buttons
                            if (CurrentPlayer.is_spell_in_process)
                            {
                                CurrentPlayer.button1 = false;
                                CurrentPlayer.button2 = false;
                                CurrentPlayer.button3 = false;
                                CurrentPlayer.button4 = false;
                                CurrentPlayer.button5 = false;
                                CurrentPlayer.button6 = false;
                            }

                            //reset only movement buttons
                            if (CurrentPlayer.is_reset_movement_button)
                            {
                                CurrentPlayer.horizontal_touch = 0;
                                CurrentPlayer.vertical_touch = 0;
                            }

                            //reset other buttons when its evade
                            if (CurrentPlayer.button6)
                            {
                                CurrentPlayer.button1 = false;
                                CurrentPlayer.button2 = false;
                                CurrentPlayer.button3 = false;
                                CurrentPlayer.button4 = false;
                                CurrentPlayer.button5 = false;

                            }

                            //CHECK for stun, fear, immobilize
                            if (CurrentPlayer.conditions.Count > 0)
                            {
                                //IMMOBILIZED co-1001
                                if (CurrentPlayer.is_cond_here_by_type_and_spell("co-1001"))
                                {
                                    CurrentPlayer.vertical_touch = 0;
                                    CurrentPlayer.horizontal_touch = 0;
                                }

                                //STUN co-1002 or FEAR co-1003
                                if (CurrentPlayer.is_cond_here_by_type_and_spell("co-1002") || CurrentPlayer.is_cond_here_by_type_and_spell("co-1003"))
                                {
                                    CurrentPlayer.ZeroInputs();
                                }
                            }


                            //working with invisibility
                            if (CurrentPlayer.is_invisible)
                            {
                                if (
                                    (!CurrentPlayer.is_cond_here_by_type_and_spell("dt-0") &&
                                    CurrentPlayer.is_cond_here_by_type_and_spell("dt")) ||
                                    CurrentPlayer.is_cond_here_by_type_and_spell("dg"))
                                {

                                    CurrentPlayer.is_invisible = false;
                                    rogue.from_inviz_to_viz(Session_id, CurrentPlayer.player_id);
                                }
                            }
                        }
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
                    
                    CurrentTime += 50;
                    if (CurrentTime> starter.stopWatch.ElapsedMilliseconds)
                    {                        
                        await Task.Delay((int)(CurrentTime - starter.stopWatch.ElapsedMilliseconds));
                    }
                    
                } 
                   
            }
        }


        private void CheckingRoundConditions()
        {

            if (!isRoundChecked)
            {
                //===================================test for GAME TYPE 0==============
                if (GameType == 0)
                {
                    int HowManyDead = 0;
                    List<int> _which_teamID_winner = new List<int>();
                    foreach (Players CurrentPlayer in LocalPlayersPool.Values)
                    {
                        if (CurrentPlayer.isDead)
                        {
                            HowManyDead++;
                        } else
                        {
                            _which_teamID_winner.Add(CurrentPlayer.team_id);
                        }
                    }

                    if (HowManyDead > 0)
                    {
                        foreach (Players CurrentPlayer in LocalPlayersPool.Values)
                        {
                            if (_which_teamID_winner.Contains(CurrentPlayer.team_id))
                            {
                                CurrentPlayer.AddPVPScoreByTeamID(CurrentPlayer.team_id);
                            }
                            
                        }

                        Task.Run(() => RestartRound(5000));
                        Console.WriteLine("started...");
                        isRoundChecked = true;
                    }

                }
                //==================================test for GAME TYPE 0==============



            }

        }

        public string SessionGamingStatistics()
        {
            string result = null;

            result = $"{GameType}~{PlayersCount}~";

            if (GameType == 0 || GameType == 1)
            {
                foreach (Players CurrentPlayer in LocalPlayersPool.Values)
                {
                    result = result + $"{CurrentPlayer.player_name}-{CurrentPlayer.player_class}-{CurrentPlayer.team_id}-{CurrentPlayer.PlayerScoreInPvP}~";
                }
            }

            return result;
        }

        private async void RestartRound(float _after_seconds)
        {
            //await Task.Delay(_after_seconds);
            string ID = functions.get_symb_for_IDs();
            string x;

            for (float i = 0; i < _after_seconds; i+=1000)
            {
                foreach (Players CurrentPlayer in LocalPlayersPool.Values)
                {
                    CurrentPlayer.conditions.TryRemove(ID, out x);
                    CurrentPlayer.conditions.TryAdd(ID, $":co-1007-{(_after_seconds-i)/1000f},");
                                        
                    if (!CurrentPlayer.isDead) {                        
                        CurrentPlayer.animation_id = 0;
                        CurrentPlayer.health_pool = $"{CurrentPlayer.health_pool.Split('=')[1]}={CurrentPlayer.health_pool.Split('=')[1]}";
                    } else
                    {
                        CurrentPlayer.animation_id = 22;
                    }

                    CurrentPlayer.is_reset_any_button = true;
                    //CurrentPlayer.conditions.Clear();
                    CurrentPlayer.CurrentSpecial = null;
                }

                await Task.Delay(1000);
            }

            foreach (Players CurrentPlayerCheck in LocalPlayersPool.Values)
            {
                CurrentPlayerCheck.ResetData();
            }
            Console.WriteLine("ended...");
            isRoundChecked = false;

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
