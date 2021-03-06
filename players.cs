using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace game_server
{
    public class Players: IDisposable
    {
        public bool isDead = false;
        private string[] init_data;
        public int PlayerScoreInPvP;

        public string Session_ID;
        public int player_order;
        public string player_id;
        public string player_name;
        public int player_class;
        public string connection_number;
        public int team_id;
        public int game_type;
        public int zone_type;
        public float position_x;
        public float position_y;
        public float position_z;
        public float rotation_x;
        public float rotation_y;
        public float rotation_z;
        public float speed;
        public int animation_id;
        //public Dictionary<string, string> conditions = new Dictionary<string, string>();
        public ConcurrentDictionary<string, string> conditions = new ConcurrentDictionary<string, string>();
        //public string conditions;
        public string health_pool;
        public float energy;
        public float health_regen;
        public float energy_regen;
        public string weapon_attack;
        public float hit_power;
        public float armor;
        public float shield_block;
        public float magic_resistance;
        public float dodge;
        public float cast_speed;
        public float melee_crit;
        public float magic_crit;
        public float spell_power;
        public int spell1;
        public int spell2;
        public int spell3;
        public int spell4;
        public int spell5;
        public int spell6;
        //public Dictionary<string, string> hidden_conds = new Dictionary<string, string>();
        public string hidden_conds;
        public int global_button_cooldown;

        public EndPoint endPointUDP;
        public int OrderNumber;
        public float MaxHealth;
        //public Stack<string> CurrentPacket = new Stack<string>();
        public Dictionary<int, Packets> CurrentPacketToProcess = new Dictionary<int, Packets>();
        //public Dictionary<long, string> CurrentPacketToProcess = new Dictionary<long, string>();
        public long LastTimePacketSend;
        public int LastPacketNumberSend;
        public string LastPacketProcessed;
        //public string LastPacketIn;
        //public string CurrentPacket;
        public string AdditionalPacketData;
        //public Queue<string> CurrentPacketToSend = new Queue<string>();
        public ConcurrentQueue<string> CurrentPacketToSend = new ConcurrentQueue<string>();
                
        //public string CurrentPacketToSend = "";
        //public List<string> CurrentPacket = new List<string>();

        public List<long> AveragePing = new List<long>();
        public long LastTimePacketIn;
        public float LastNormalPing;

        //inner vars
        public float horizontal_touch = 0;
        public float vertical_touch = 0;
        public bool button1 = false;
        public bool button2 = false;
        public bool button3 = false;
        public bool button4 = false;
        public bool button5 = false;
        public bool button6 = false;
        public bool is_strafe_on = false; //0004 sia
        public bool is_invisible = false; //2222 inv
        public bool is_spell_in_process = false;//1111 sip
        public bool is_spell_button_touched = false;//0000 bst
        public bool is_reset_any_button = false; //0002 rab
        public bool is_reset_movement_button = false; //0003 rmb
        public bool is_movement_touched = false; //0001 bmt
        public bool is_immune_to_movement_imparing = false; //
        public bool is_reset_movement_not_rotation = false;
        public bool is_immune_to_melee = false; //0005 immune to melee received
        public bool is_immune_to_magic = false; //0006 immune to magic received
        public bool is_reflecting_melee = false; //0007 reflect damage melee
        public bool is_reflecting_magic = false; //0008 reflect damage magic
        
        //------------
        public bool start_cheking_if_spell_touched = false;
        public bool is_spell_touched_for_casting_failing = false;
        public string data_when_immune_melee=null;
        public string data_when_immune_magic=null;


        // encoding
        //public encryption PlayerEncryption = new encryption();
        public byte[] secret_key;
        public string PacketID;

        //free regeneration control
        public bool isFreeRegeneration = false;
        public DateTime TimeOfLastAction;
        public float FreeRegenerationDelta;
        public readonly float BaseHealthRegen;

        //special
        public delegate void Specials();
        public Specials CurrentSpecial;

        //GLOBALS FOR ELEMENTALIST
        public float PowerOfFireMulti = 1f;
        public float PowerOfIceMulti = 1f;
        public float PowerOfAirMulti = 1f;
        public float PowerOfEarthMulti = 1f;
        public float ChanceOfCastBurningOnFireSpell = 5f;

        //some globals
        public float ChanceOfSpellCastingBroken = 0;

        public Players(int player_order, string player_id, string player_name, int player_class, string connection_number, int team_id,
        int zone_type, float position_x, float position_y, float position_z, float rotation_x, float rotation_y, float rotation_z,
        float speed, int animation_id, string health_pool, float energy, float health_regen, float energy_regen,
        string weapon_attack, float hit_power, float armor, float shield_block, float magic_resistance, float dodge, float cast_speed,
        float melee_crit, float magic_crit, float spell_power, int spell1, int spell2, int spell3, int spell4, int spell5, 
        int global_button_cooldown)
        {

            this.player_order = player_order;
            this.player_id = player_id;
            this.player_name = player_name;
            this.player_class = player_class;
            this.connection_number = connection_number;
            this.team_id = team_id;
            this.zone_type = zone_type;
            this.position_x = position_x;
            this.position_y = position_y;
            this.position_z = position_z;
            this.rotation_x = rotation_x;
            this.rotation_y = rotation_y;
            this.rotation_z = rotation_z;
            this.speed = speed;
            this.animation_id = animation_id;
            //this.conditions = conditions;
            this.health_pool = health_pool;
            this.energy = energy;
            this.health_regen = health_regen;
            this.energy_regen = energy_regen;
            this.weapon_attack = weapon_attack;
            this.hit_power = hit_power;
            this.armor = armor;
            this.shield_block = shield_block;
            this.magic_resistance = magic_resistance;
            this.dodge = dodge;
            this.cast_speed = cast_speed;
            this.melee_crit = melee_crit;
            this.magic_crit = magic_crit;
            this.spell_power = spell_power;
            this.spell1 = spell1;
            this.spell2 = spell2;
            this.spell3 = spell3;
            this.spell4 = spell4;
            this.spell5 = spell5;
            this.spell6 = 997;
            //this.hidden_conds = hidden_conds;
            this.global_button_cooldown = global_button_cooldown;
        }
        

        

        public Players(string _session, params string [] data)
        {
            init_data = data;
            Session_ID = _session;
            player_order = int.Parse(data[0]);
            player_id = data[1];
            player_name = data[2];
            player_class = int.Parse(data[3]);
            connection_number = data[4];
            team_id = int.Parse(data[5]);
            game_type = int.Parse(data[6]);
            zone_type = int.Parse(data[7]);
            position_x = float.Parse(data[8]);
            position_y = float.Parse(data[9]);
            position_z = float.Parse(data[10]);
            rotation_x = float.Parse(data[11]);
            rotation_y = float.Parse(data[12]);
            rotation_z = float.Parse(data[13]);
            speed = float.Parse(data[14]);
            animation_id = int.Parse(data[15]);
            //conditions = data[16];
            health_pool = data[17];
            energy = float.Parse(data[18]);
            health_regen = float.Parse(data[19]);
            energy_regen = float.Parse(data[20]);
            weapon_attack = data[21];
            hit_power = float.Parse(data[22]);
            armor = float.Parse(data[23]);
            shield_block = float.Parse(data[24]);
            magic_resistance = float.Parse(data[25]);
            dodge = float.Parse(data[26]);
            cast_speed = float.Parse(data[27]);
            melee_crit = float.Parse(data[28]);
            magic_crit = float.Parse(data[29]);
            spell_power = float.Parse(data[30]);
            spell1 = int.Parse(data[31]);
            spell2 = int.Parse(data[32]);
            spell3 = int.Parse(data[33]);
            spell4 = int.Parse(data[34]);
            spell5 = int.Parse(data[35]);
            spell6 = 997;
            hidden_conds = data[36];
            global_button_cooldown = int.Parse(data[37]);

            OrderNumber = 0;
            
            //free regeneration
            TimeOfLastAction = DateTime.Now;
            MaxHealth = float.Parse(health_pool.Split('=')[1]);
            BaseHealthRegen = health_regen;

            SetSpecials();
        }

        private void SetSpecials()
        {
            
            CurrentSpecial = null;
            //specials
            switch (player_class)
            {
                case 1:
                    WarriorSpecial currwar = new WarriorSpecial(this);
                    CurrentSpecial += currwar.UpdateSpecial;
                    break;
                case 2:
                    ElementalistSpecial currelem = new ElementalistSpecial(this);
                    CurrentSpecial += currelem.UpdateSpecial;
                    break;
                case 3:
                    BarbarianSpecial currbarbar = new BarbarianSpecial(this);
                    CurrentSpecial += currbarbar.UpdateSpecial;
                    break;
                case 4:
                    RogueSpecial currrog = new RogueSpecial(this);
                    CurrentSpecial += currrog.UpdateSpecial;

                    RogueChecksWhenInvisible rogcheck = new RogueChecksWhenInvisible(this);
                    CurrentSpecial += rogcheck.UpdateSpecial;

                    break;
                case 5:
                    break;

            }
        }



        public void ResetData()
        {
            position_x = float.Parse(init_data[8]);
            position_y = float.Parse(init_data[9]);
            position_z = float.Parse(init_data[10]);
            rotation_x = float.Parse(init_data[11]);
            rotation_y = float.Parse(init_data[12]);
            rotation_z = float.Parse(init_data[13]);
            speed = float.Parse(init_data[14]);
            animation_id = int.Parse(init_data[15]);
            //conditions = data[16];
            health_pool = init_data[17];
            energy = float.Parse(init_data[18]);
            health_regen = float.Parse(init_data[19]);
            energy_regen = float.Parse(init_data[20]);
            weapon_attack = init_data[21];
            hit_power = float.Parse(init_data[22]);
            armor = float.Parse(init_data[23]);
            shield_block = float.Parse(init_data[24]);
            magic_resistance = float.Parse(init_data[25]);
            dodge = float.Parse(init_data[26]);
            cast_speed = float.Parse(init_data[27]);
            melee_crit = float.Parse(init_data[28]);
            magic_crit = float.Parse(init_data[29]);
            spell_power = float.Parse(init_data[30]);
            spell1 = int.Parse(init_data[31]);
            spell2 = int.Parse(init_data[32]);
            spell3 = int.Parse(init_data[33]);
            spell4 = int.Parse(init_data[34]);
            spell5 = int.Parse(init_data[35]);
            spell6 = 997;
            hidden_conds = init_data[36];
            global_button_cooldown = int.Parse(init_data[37]);

            SetSpecials();

            //inner vars
            horizontal_touch = 0;
            vertical_touch = 0;
            button1 = false;
            button2 = false;
            button3 = false;
            button4 = false;
            button5 = false;
            button6 = false;
            is_strafe_on = false; //0004 sia
            is_invisible = false; //2222 inv
            is_spell_in_process = false;//1111 sip
            is_spell_button_touched = false;//0000 bst
            is_reset_any_button = false; //0002 rab
            is_reset_movement_button = false; //0003 rmb
            is_movement_touched = false; //0001 bmt
            is_immune_to_melee = false; //0005 immune to melee received
            is_immune_to_magic = false; //0006 immune to magic received
            is_reset_movement_not_rotation = false;
            is_reflecting_melee = false; //0007 reflect damage melee
            is_reflecting_magic = false; //0008 reflect damage magic

            //------------
            start_cheking_if_spell_touched = false;
            is_spell_touched_for_casting_failing = false;
            data_when_immune_melee = null;
            data_when_immune_magic = null;

            isDead = false;

            conditions.Clear();
            Console.WriteLine("reseted...");
        }




        public string GetPacketForSending(string current_player_id)
        {
            string[] health_parts = health_pool.Split('=');
            health_pool = $"{Math.Round(float.Parse(health_parts[0]), 1).ToString("f0")}={Math.Round(float.Parse(health_parts[1]), 1).ToString("f0")}";

            StringBuilder sb = new StringBuilder(110);


            string conditions_refactor = null;
            /*
            try
            {
                foreach (string key in conditions.Keys.ToList())
                {
                    if (conditions[key] != ":,")
                    {
                        conditions_refactor = conditions_refactor + key + conditions[key];  //Console.WriteLine(key + " - " + t[key]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            */

            try
            {
                conditions_refactor = string.Join("", conditions.Select(kvp => kvp.Key + kvp.Value));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            

            if (player_id != current_player_id && is_invisible)
            {
                                
                sb.Append("100~100~" +
                    rotation_y.ToString("f1") + "~" + 
                    animation_id + "~" + conditions_refactor + "~" + health_pool + "~" + energy.ToString("f0") + "^");
            } else
            {
                sb.Append(position_x.ToString("f1") + "~" + position_z.ToString("f1") + "~" +
                rotation_y.ToString("f1") + "~" + 
                animation_id + "~" + conditions_refactor + "~" + health_pool + "~" + energy.ToString("f0") + "^");
            }

            return sb.ToString();

        }

        public string GetPacketForSending_nonPlayer()
        {
            string[] health_parts = health_pool.Split('=');
            health_pool = $"{Math.Round(float.Parse(health_parts[0]), 1).ToString("f0")}={Math.Round(float.Parse(health_parts[1]), 1).ToString("f0")}";

            StringBuilder sb = new StringBuilder(110);


            string conditions_refactor = null;
            try
            {
                foreach (string key in conditions.Keys.ToList())
                {
                    if (conditions[key] != ":,")
                    {
                        conditions_refactor = conditions_refactor + key + conditions[key];  //Console.WriteLine(key + " - " + t[key]);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            

            sb.Append(player_id + "~" + player_class + "~" + player_name + "~" + position_x.ToString("f1") + "~" + position_z.ToString("f1") + "~" +
            rotation_y.ToString("f1") + "~" +
            animation_id + "~" + conditions_refactor + "~" + health_pool + "~" + energy.ToString("f0") + "^");
            
            return sb.ToString();

        }

        public string GetPacketForSending()
        {
            string[] health_parts = health_pool.Split('=');
            health_pool = $"{Math.Round(float.Parse(health_parts[0]), 1).ToString("f0")}={Math.Round(float.Parse(health_parts[1]), 1).ToString("f0")}";

            StringBuilder sb = new StringBuilder(110);

            //var _temp_dat = from r in CurrentPlayer.conditions.Values select r;
            //string _data = string.Join("", _temp_dat);

            string conditions_refactor = null;
            /*
            try
            {
                foreach (string key in conditions.Keys.ToList())
                {
                    if (conditions[key] != ":,")
                    {
                        conditions_refactor = conditions_refactor + key + conditions[key];  //Console.WriteLine(key + " - " + t[key]);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            */

            try
            {
                conditions_refactor = string.Join("", conditions.Select(kvp => kvp.Key + kvp.Value));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            sb.Append(position_x.ToString("f1") +"~" + position_z.ToString("f1") + "~" +
               rotation_y.ToString("f1") + "~" +
                animation_id + "~" + conditions_refactor + "~" + health_pool + "~" + energy.ToString("f0") + "^");

            return sb.ToString();

        }

        public void CheckForFreeRegeneration()
        {
            var dat = from r in conditions.Values select r;

            string _data = string.Join("", dat);
            
            if (_data == null) return;
            
            if (isFreeRegeneration && (_data.Contains("dg") || _data.Contains("hg") || _data.Contains("dt") ))
            {
                TimeOfLastAction = DateTime.Now;
                isFreeRegeneration = false;
                health_regen = BaseHealthRegen;
                FreeRegenerationDelta = 0;
            } 
            
            if (isFreeRegeneration)
            {
                if (FreeRegenerationDelta<starter.max_free_regeneration)
                {
                    FreeRegenerationDelta += 0.07f;
                }
                health_regen += FreeRegenerationDelta;

            }

            
            if (!isFreeRegeneration && DateTime.Now.Subtract(TimeOfLastAction).TotalSeconds>5)
            {
                isFreeRegeneration = true;                
            }

        }

        public void Dispose()
        {
            Dispose();
        }

        public void AddPVPScoreByTeamID(int teamID)
        {
            if (team_id==teamID)
            {
                PlayerScoreInPvP++;
            }
        }
                
        public void ZeroInputs()
        {
            horizontal_touch = 0;
            vertical_touch = 0;
            button1 = false;
            button2 = false;
            button3 = false;
            button4 = false;
            button5 = false;
            button6 = false;
            is_strafe_on = false;
        }


        

        //stop all conditions
        public void add_stop_to_spec_conditions(int type)
        {                        
            string x;

            switch(type)
            {
                case 0:
                    conditions.Remove("c10001", out x);
                    conditions.TryAdd("c10001", $":,");
                    break;
                case 1:
                    conditions.Remove("c10002", out x);
                    conditions.TryAdd("c10002", $":,");
                    break;
                case 2:
                    conditions.Remove("c10003", out x);
                    conditions.TryAdd("c10003", $":,");
                    break;
            }            
        }

        public void remove_stop_to_spec_conditions(int type)
        {
            string x;

            switch (type)
            {
                case 0:
                    conditions.Remove("c10001", out x);                    
                    break;
                case 1:
                    conditions.Remove("c10002", out x);                    
                    break;
                case 2:
                    conditions.Remove("c10003", out x);                    
                    break;
            }
        }
        //======================================================


        public bool is_stop_all_condition_by_checking_index(int index)
        {
            if (conditions.ContainsKey("c10001"))
            {
                return true;
            }

            if (conditions.ContainsKey("c10002"))
            {
                /*
                foreach (string item in conditions.Values)
                {
                    if (item.Contains(":co"))
                    {
                        if (spells.SpellID(index).general != GeneralChars.negative)
                        {
                            return false;
                        }

                        int _where_starts = item.IndexOf("co") + 3;
                        int _where_ends = item.IndexOf("-", _where_starts + 1);
                        int result = int.Parse(item.Substring(_where_starts, _where_ends - _where_starts));

                        if (index == result)
                        {
                            return true;
                        }
                    }
                }
                */

                if (spells.SpellID(index).general == GeneralChars.negative)
                {
                    return true;
                }
            }

            if (conditions.ContainsKey("c10003"))
            {
                if (spells.SpellID(index).general == GeneralChars.positive)
                {
                    return true;
                }
            }

            if (is_cond_here_by_type_and_spell($"st-{index}"))
            {                
                string x;
                try
                {
                    conditions.TryRemove(get_id_by_type_and_spell($"st-{index}"), out x);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(DateTime.Now + ": no key to del in conditions when clearing one-condition-removal");
                }
                
                return true;
            }


            return false;
        }


        //set condition
        public void set_condition(string cond_type, int condition_number, string cond_id, float timer)
        {
            string x;
            conditions.Remove(cond_id, out x);
            conditions.TryAdd(cond_id, $":{cond_type}-{condition_number}-{timer.ToString("f1").Replace(',', '.')},"); //.Replace(',', '.')
        }

        public void set_condition(string cond_type, int condition_number, string cond_id, float timer, int stack)
        {
            string x;
            conditions.Remove(cond_id, out x);
            conditions.TryAdd(cond_id, $":{cond_type}-{condition_number}-{timer.ToString("f1").Replace(',', '.')}-{stack},"); //.Replace(',', '.')
        }


        //find in certain cond type any of spells in array
        public bool is_any_cond_inarray_of_spellnumber(string type_of_cond, string[] array_to_check)
        {
            
            if (conditions.Count == 0)
            {
                return false;
            }

            try
            {
                foreach (string vall in conditions.Values.ToList())
                {
                    for (int i = 0; i < array_to_check.Length; i++)
                    {
                        
                            if (vall.Contains(array_to_check[i]) && vall.Contains(type_of_cond))
                            {
                                return true;
                            }
                        


                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            

            return false;
        }


        //get ID by type and spell like "xx-45"
        public string get_id_by_type_and_spell(string searched_cond)
        {
            try
            {
                foreach (string vall in conditions.Keys.ToList())
                {
                    if (conditions[vall].Contains(searched_cond))
                    {
                        return vall;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            

            return null;
        }


        //check if DAMAGE is crit
        public bool isCrit_DamageAnalisys(string bulk_data)
        {
            if (!bulk_data.Contains("dg") || !bulk_data.Contains("dt"))
            {
                return false;
            }

            string[] data = bulk_data.Split('-');

            if (data[2]=="c")
            {
                return true;
            } else
            {
                return false;
            }
                        
        }

        //check spell index in DAMAGE
        public int GetSpellIndex_DamageAnalisys(string bulk_data)
        {
            if (!bulk_data.Contains("dg") || !bulk_data.Contains("dt"))
            {
                return 0;
            }

            string[] data = bulk_data.Split('-');

            return int.Parse(data[3].Replace(',', ' '));
            
        }



        //get bulk data in condition by any cond data
        public string get_bulkconditiondata_by_anything_in_bulk(string searched_cond)
        {
            try
            {
                foreach (string vall in conditions.Keys.ToList())
                {
                    if (conditions[vall].Contains(searched_cond))
                    {
                        return conditions[vall];                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            return null;
        }

        //get bulk data in condition by any cond data
        public string get_condID_by_anything_in_bulk(string searched_cond)
        {
            try
            {
                foreach (string vall in conditions.Keys.ToList())
                {
                    if (conditions[vall].Contains(searched_cond))
                    {
                        return vall;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            return null;
        }

        public Players get_player_by_id_in_conds(string cond_id, string table_id)
        {
            foreach (Players current_player in starter.SessionsPool[table_id].LocalPlayersPool.Values)
            {
                if (current_player.conditions.ContainsKey(cond_id))
                {
                    return current_player;
                }
            }

            return null;
        }



        //KNOCKED DOWN
        public void make_knocked(string conds_id, float tick_time_left)
        {
            if (is_stop_all_condition_by_checking_index(1006))
            {
                return;
            }

            string x;
            conditions.TryRemove(conds_id, out x);
            conditions.TryAdd(conds_id, $":co-1006-{tick_time_left.ToString("f1").Replace(',', '.')},");
        }
        


        //IMMOBILIZE
        public void make_immob(string conds_id, float tick_time_left)
        {
            if (is_stop_all_condition_by_checking_index(1001))
            {
                return;
            }

            //spells.set_animation_for_one($table_id, $player->player_id, 0, 2, 0.01);
            animation_id = 0;
            string x;
            conditions.TryRemove(conds_id, out x);
            conditions.TryAdd(conds_id, $":co-1001-{tick_time_left.ToString("f1").Replace(',', '.')},");

        }


        //slower
        public void make_slow(string conds_id, float tick_time_left)
        {
            if (is_stop_all_condition_by_checking_index(995))
            {
                return;
            }

            string x;
            conditions.TryRemove(conds_id, out x);
            conditions.TryAdd(conds_id, $":co-995-{tick_time_left.ToString("f1")},");
        }


        //STUN
        public void make_stun(string conds_id, float tick_time_left)
        {
            if (is_stop_all_condition_by_checking_index(1002))
            {
                return;
            }

            animation_id = 8;
            string x;
            
            conditions.TryRemove(conds_id, out x);
            conditions.TryAdd(conds_id, $":co-1002-{tick_time_left.ToString("f1")},");
        }


        //break casting 1005
        public async void make_broken_casting()
        {
            /*
            if (is_stop_all_condition_by_checking_index(1005))
            {
                return;
            }
            */

            string conds_id = functions.get_symb_for_IDs();
            string x;
            conditions.TryRemove(conds_id, out x);
            conditions.TryAdd(conds_id, $":co-1005-0,");
            await Task.Delay(250);
            remove_condition_in_player(conds_id);
        }

        //cheking for 1002 and 1003 and 1005 stopping casting
        public bool is_casting_stopped_by_spells()
        {
            string[] array_of_spells = new string[] { "1002", "1003", "1005", "1006", "1007" };
            return is_any_cond_inarray_of_spellnumber("co", array_of_spells);
        }


        public async void remove_condition_in_player(string cond_id)
        {

            string x;
            
            conditions.TryGetValue(cond_id, out x);
            if (x != null)
            {
                string[] bulk = x.Split('-');
                if (bulk[0] == ":co")
                {
                    conditions.TryRemove(cond_id, out x);
                    conditions.TryAdd(cond_id, $"{bulk[0]}-{bulk[1]}-0,");
                }
            }

            await Task.Delay(200);


            conditions.TryRemove(cond_id, out x);
        }



        public bool is_cond_here_by_type_and_spell(string searched_cond)
        {
            try
            {
                foreach (string items in conditions.Values.ToList())
                {
                    
                        if (items.Contains(searched_cond))
                        {
                            return true;
                        }
                    
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine (ex);
            }
            


            return false;
        }

        public async void reset_animation_for_one()
        {
            await Task.Delay(110);            
            animation_id = 0;
        }

        public async void CastEndCS(float last_x, float last_z, string ID, int spell_index)
        {
            string x;

            conditions.TryRemove(ID, out x);
            conditions.TryAdd(ID, $":cs={spell_index}={last_x.ToString("f1").Replace(',', '.')}={last_z.ToString("f1").Replace(',', '.')},");
            
            await Task.Delay(100);
            
            conditions.TryRemove(ID, out x);
            conditions.TryAdd(ID, $":cs={spell_index}=999=999,");

            remove_condition_in_player(ID);
        }
       

        //cheking for 1002 and 1003 and 1005 plus MOVEMENET and SPELLS
        public bool is_casting_failed()
        {

            
            if (Math.Abs(vertical_touch) > 1.5f || Math.Abs(horizontal_touch) > 1.5f)
            {
                return true;
            }

            if (is_spell_touched_for_casting_failing)
            {
                return true;
            }



            string[] array_of_spells = new string[] { "1002", "1003", "1005", "1006", "1007" };
            bool result = is_any_cond_inarray_of_spellnumber("co", array_of_spells);
            if (result)
            {
                return true;
            }


            return false;
        }


     
        public void minus_energy(float amount)
        {            
            energy = energy - amount;
        }

        public void start_spell_in_process()
        {
            start_cheking_if_spell_touched = true;
            is_spell_in_process = true;
        }

        public void stop_spell_in_process()
        {
            start_cheking_if_spell_touched = false;
            is_spell_in_process = false;
            is_spell_touched_for_casting_failing = false;
        }

    }

    public class RogueChecksWhenInvisible
    {
        private string id_condition = "";
        private Players CurrentPlayer;
        private bool isReady;
        private DateTime TimeOfLastEffect;

        private float CoolDownForChecks;

        private readonly int TimeForSpecialFrom = 5;
        private readonly int TimeForSpecialTo = 10;

        public RogueChecksWhenInvisible(Players _current_player)
        {
            CurrentPlayer = _current_player;            
            id_condition = functions.get_symb_for_IDs();
            isReady = true;
            CoolDownForChecks = GetRandom();
        }

        public void UpdateSpecial()
        {
            bool isOK = false;
            foreach (string item in CurrentPlayer.conditions.Values)
            {
                if (item.Contains("co-153"))
                {
                    isOK = true;                    
                    break;
                }
            }

            if (!isOK)
            {
                TimeOfLastEffect = DateTime.Now;
                return;
            }

            float _tick = (float)DateTime.Now.Subtract(TimeOfLastEffect).TotalSeconds;
            if (_tick > CoolDownForChecks)
            {
                if (functions.assess_chance(10))
                {
                    MakeChecks();
                    TimeOfLastEffect = DateTime.Now;
                    CoolDownForChecks = GetRandom();
                }
            }
        }

        private void MakeChecks()
        {            
            CurrentPlayer.conditions.TryAdd(id_condition, $":ad=153={CurrentPlayer.position_x.ToString("f1").Replace(',', '.')}={CurrentPlayer.position_z.ToString("f1").Replace(',', '.')}={CurrentPlayer.rotation_y.ToString("f1").Replace(',', '.')},");
            CurrentPlayer.remove_condition_in_player(id_condition);
            id_condition = functions.get_symb_for_IDs();
        }

        private float GetRandom()
        {
            Random rnd = new Random();
            return rnd.Next(TimeForSpecialFrom, TimeForSpecialTo+1);
        }

    }



    public class WarriorSpecial : IDisposable
    {
        private float CurrentArmorStack;
        private float DefaultTimeforNextSpecial = 5f;
        private int CurrentIterationStack;
        
        private readonly float DefaultArmorSingleStack = 50f;
        private readonly float DefaultTimeforStack = 5f;        
        private readonly int TimeForSpecialFrom = 5;
        private readonly int TimeForSpecialTo = 10;


        private DateTime TimeOfStackInitiated;
        private DateTime TimeOfEndForSpecial;
        private bool isReadyToUse;
        
        private string id_condition = "";
        private Players CurrentPlayer;
        
        public WarriorSpecial(Players _current_player)
        {
            CurrentPlayer = _current_player;
            isReadyToUse = true;
            id_condition = functions.get_symb_for_IDs();
            TimeOfEndForSpecial = DateTime.Now;
        }

        public void UpdateSpecial()
        {            
            if (CurrentIterationStack!=0)
            {
                float _tick = (float)DateTime.Now.Subtract(TimeOfStackInitiated).TotalSeconds;
                if (_tick<DefaultTimeforStack)
                {
                    UpdateTimer(DefaultTimeforStack-_tick);
                }
                else
                {
                    EndTickAndRemove();
                }
                
            }

            if (DateTime.Now.Subtract(TimeOfEndForSpecial).TotalSeconds > DefaultTimeforNextSpecial)
            {                
                isReadyToUse = true;
            }
            else
            {
                isReadyToUse = false;
            }

            if (!isReadyToUse || CurrentPlayer.conditions.Count==0) return;

            bool isDamageTaken = false;
            bool isDamageTakenCrit = false;

            var _temp_dat = from r in CurrentPlayer.conditions.Values select r;
            string _data = string.Join("", _temp_dat);
            
            if (_data == null) return;

            

            if (_data.Contains("dt"))
            {
                string[] to_search = _data.Split(',');
                for (int i = 0; i < to_search.Length; i++)
                {
                    if (to_search[i].Contains("dt"))
                    {
                        int _where_starts = to_search[i].IndexOf("dt") + 3;
                        int _where_ends = to_search[i].IndexOf("-", _where_starts + 1);
                        float result = float.Parse(to_search[i].Substring(_where_starts, _where_ends - _where_starts));

                        if (result > 0)
                        {
                            isDamageTaken = true;

                            if (to_search[i].Contains("-c-"))
                            {
                                isDamageTakenCrit = true;

                                break;
                            }
                        }
                    }
                }

            }



            if (isDamageTaken && CurrentIterationStack < 5)
            {
                //break before next stack is available
                if ((float)DateTime.Now.Subtract(TimeOfStackInitiated).TotalSeconds<1f)
                {
                    return;
                }

                if (isDamageTakenCrit)
                {
                    CurrentPlayer.armor += DefaultArmorSingleStack*2;
                    CurrentArmorStack += DefaultArmorSingleStack*2;
                } else
                {
                    CurrentPlayer.armor += DefaultArmorSingleStack;
                    CurrentArmorStack += DefaultArmorSingleStack;
                }

                
                CurrentIterationStack++;
                
                
                UpdateConditions();
            } 
            else if (isDamageTaken && CurrentIterationStack >= 5)
            {
                //EndTickAndRemove();
            } 
            else if (!isDamageTaken)
            {                
                return;
            }          
            
        }

        private float GetRandom()
        {
            Random rnd = new Random();
            return rnd.Next(TimeForSpecialFrom, TimeForSpecialTo+1);
        }

        private void UpdateConditions()
        {
            TimeOfStackInitiated = DateTime.Now;            
            CurrentPlayer.set_condition("co", 1010, id_condition, DefaultTimeforStack, CurrentIterationStack);            
        }

        private void EndTickAndRemove()
        {
            DefaultTimeforNextSpecial = GetRandom();
            CurrentPlayer.armor -= CurrentArmorStack;
            CurrentIterationStack = 0;
            CurrentArmorStack = 0;
            TimeOfEndForSpecial = DateTime.Now;
            CurrentPlayer.remove_condition_in_player(id_condition);
            id_condition = functions.get_symb_for_IDs();
        }

        private void UpdateTimer(float _time)
        {
            CurrentPlayer.set_condition("co", 1010, id_condition, _time, CurrentIterationStack);
        }

        public void Dispose()
        {
            this.Dispose();
        }
    }

    public class ElementalistSpecial
    {
        
        private DateTime TimeOfStartForSpecial;
        private DateTime TimeOfEndForSpecial;
        
        private bool isReadyToUse;
        private bool isSpecialActive = false;
        
        private float TimeTillNextSpecial;
        private float DefaultContinuityOfSpecial = 2f;
        
        private readonly int TimeForSpecialFrom = 5;
        private readonly int TimeForSpecialTo = 10;
                
        private string id_condition = "";
        
        private Players CurrentPlayer;

        private enum Powers
        {
            PowerOfFire = 1011,
            powerOfIce = 1012,
            PowerOfAir = 1013,
            PowerOfEarth = 1014
        }

        private Powers CurrentActivePower;

        public ElementalistSpecial(Players _current_player)
        {
            CurrentPlayer = _current_player;
            isReadyToUse = true;
            id_condition = functions.get_symb_for_IDs();
            TimeOfEndForSpecial = DateTime.Now;
            TimeTillNextSpecial = 10f;
        }

        public void UpdateSpecial()
        {
            if (!isReadyToUse) return;
            float _tick = (float)DateTime.Now.Subtract(TimeOfStartForSpecial).TotalSeconds;

            if (isSpecialActive && _tick > DefaultContinuityOfSpecial)
            {                
                EndTickAndRemove();
            } 
            else if (isSpecialActive && !(_tick > DefaultContinuityOfSpecial))
            {
                UpdateTimer(DefaultContinuityOfSpecial - _tick);
            }

            if (!isSpecialActive && (float)DateTime.Now.Subtract(TimeOfEndForSpecial).TotalSeconds>TimeTillNextSpecial)
            {
                TimeOfStartForSpecial = DateTime.Now;
                isSpecialActive = true;
                CurrentActivePower = GetSpecial();
                
                UpdateConditions();
            }


        }

        private Powers GetSpecial()
        {
            Random rnd = new Random();
            return (Powers)rnd.Next(1011, 1015);            
        }

        private float GetRandom()
        {
            Random rnd = new Random();
            return rnd.Next(TimeForSpecialFrom, TimeForSpecialTo+1);
        }

        private void UpdateConditions()
        {
            
            CurrentPlayer.set_condition("co", (int)CurrentActivePower, id_condition, DefaultContinuityOfSpecial);
            //Console.WriteLine(CurrentPlayer.armor + " - armoraaaaaaaaaaa");
        }

        private void EndTickAndRemove()
        {
            isSpecialActive = false;
            TimeOfEndForSpecial = DateTime.Now;
            TimeTillNextSpecial = GetRandom();

            CurrentPlayer.remove_condition_in_player(id_condition);
            id_condition = functions.get_symb_for_IDs();
        }

        private void UpdateTimer(float _time)
        {
            CurrentPlayer.set_condition("co", (int)CurrentActivePower, id_condition, _time);
        }
    }

    public class BarbarianSpecial
    {
        private float CurrentAttackPowerStack;
        private float DefaultTimeforNextSpecial = 5f;
        private int CurrentIterationStack;
        private readonly int DefaultSpellNumber = 1015;

        private readonly float DefaultAttackPowerSingleStack = 20f;
        private readonly float DefaultTimeforStack = 5f;
        private readonly int TimeForSpecialFrom = 5;
        private readonly int TimeForSpecialTo = 10;


        private DateTime TimeOfStackInitiated;
        private DateTime TimeOfEndForSpecial;
        private bool isReadyToUse;

        private string id_condition = "";
        private Players CurrentPlayer;

        public BarbarianSpecial(Players _current_player)
        {
            CurrentPlayer = _current_player;
            isReadyToUse = true;
            id_condition = functions.get_symb_for_IDs();
            TimeOfEndForSpecial = DateTime.Now;
        }

        public void UpdateSpecial()
        {            
            if (CurrentIterationStack != 0)
            {
                float _tick = (float)DateTime.Now.Subtract(TimeOfStackInitiated).TotalSeconds;
                if (_tick < DefaultTimeforStack)
                {
                    UpdateTimer(DefaultTimeforStack - _tick);
                }
                else
                {
                    EndTickAndRemove();
                }

            }

            if (DateTime.Now.Subtract(TimeOfEndForSpecial).TotalSeconds > DefaultTimeforNextSpecial)
            {
                isReadyToUse = true;
            }
            else
            {
                isReadyToUse = false;
            }

            if (!isReadyToUse || CurrentPlayer.conditions.Count == 0) return;

            bool isDamageGiven = false;
            bool isDamageGivenCrit = false;

            var _temp_dat = from r in CurrentPlayer.conditions.Values select r;
            string _data = string.Join("", _temp_dat);

            if (_data == null) return;

            if (_data.Contains("dg"))
            {                
                string[] to_search = _data.Split(',');                
                for (int i = 0; i < to_search.Length; i++)
                {
                    if (to_search[i].Contains("dg"))
                    {
                        int _where_starts = to_search[i].IndexOf("dg") + 3;
                        int _where_ends = to_search[i].IndexOf("-", _where_starts + 1);
                        float result = float.Parse(to_search[i].Substring(_where_starts, _where_ends - _where_starts));
                        
                        if (result>0)
                        {
                            isDamageGiven = true;
                            
                            if (to_search[i].Contains("-c-"))
                            {
                                isDamageGivenCrit = true;
                                
                                break;
                            }
                        }
                    }
                }

            }



            if (isDamageGiven && CurrentIterationStack < 5)
            {
                //break before next stack is available
                if ((float)DateTime.Now.Subtract(TimeOfStackInitiated).TotalSeconds < 1f)
                {
                    return;
                }

                if (isDamageGivenCrit)
                {
                    CurrentPlayer.hit_power += DefaultAttackPowerSingleStack * 2;
                    CurrentAttackPowerStack += DefaultAttackPowerSingleStack * 2;
                }
                else
                {
                    CurrentPlayer.hit_power += DefaultAttackPowerSingleStack;
                    CurrentAttackPowerStack += DefaultAttackPowerSingleStack;
                }

                CurrentIterationStack++;

                UpdateConditions();
            }
            else if (isDamageGiven && CurrentIterationStack >= 5)
            {
                //EndTickAndRemove();
            }
            else if (!isDamageGiven)
            {
                return;
            }

        }

        private float GetRandom()
        {
            Random rnd = new Random();
            return rnd.Next(TimeForSpecialFrom, TimeForSpecialTo+1);
        }

        private void UpdateConditions()
        {
            TimeOfStackInitiated = DateTime.Now;
            CurrentPlayer.set_condition("co", DefaultSpellNumber, id_condition, CurrentIterationStack);
        }

        private void EndTickAndRemove()
        {
            DefaultTimeforNextSpecial = GetRandom();
            CurrentPlayer.hit_power -= CurrentAttackPowerStack;
            CurrentIterationStack = 0;
            CurrentAttackPowerStack = 0;
            TimeOfEndForSpecial = DateTime.Now;
            CurrentPlayer.remove_condition_in_player(id_condition);
            id_condition = functions.get_symb_for_IDs();
        }

        private void UpdateTimer(float _time)
        {
            CurrentPlayer.set_condition("co", DefaultSpellNumber, id_condition, _time, CurrentIterationStack);
        }
    }

    public class RogueSpecial
    {
        private int CurrentStackAmount;

        private DateTime TimeOfLastStackAdded;

        private readonly float TimeTillNextStackAdding = 5f;
        private readonly float TimeForTickingOfCurrentStack = 5f;
        private readonly float DefaultStackForMeleeCrit = 1f;
        private readonly float DefaultStackForAttpower = 1f;

        private readonly int SpecializationSpellIndex = 1016;

        private bool isReadyToUse;
        private string id_condition = "";
        private Players CurrentPlayer;

        public RogueSpecial(Players _current_player)
        {
            CurrentPlayer = _current_player;
            isReadyToUse = true;
            id_condition = functions.get_symb_for_IDs();
            TimeOfLastStackAdded = DateTime.Now;
        }

        

        public void UpdateSpecial()
        {
            
            if (!isReadyToUse || CurrentPlayer.conditions.Count == 0) return;

            var _temp_dat = from r in CurrentPlayer.conditions.Values select r;
            string _data = string.Join("", _temp_dat);

            if (_data.Contains("153"))
            {
                if ((float)DateTime.Now.Subtract(TimeOfLastStackAdded).TotalSeconds > TimeTillNextStackAdding)
                {
                    AddBuffStack();
                }
            } 



            if (_data.Contains("dg") && CurrentStackAmount>0)
            {
                bool isDamageGiven = false;
                string[] to_search = _data.Split(',');
                for (int i = 0; i < to_search.Length; i++)
                {
                    if (to_search[i].Contains("dg"))
                    {
                        int _where_starts = to_search[i].IndexOf("dg") + 3;
                        int _where_ends = to_search[i].IndexOf("-", _where_starts + 1);
                        float result = float.Parse(to_search[i].Substring(_where_starts, _where_ends - _where_starts));

                        if (result > 0)
                        {
                            isDamageGiven = true;
                            break;
                        }
                    }
                }

                if (isDamageGiven)
                {
                    EndTickAndRemove();
                }

            }
            

        }

       
        private void AddBuffStack()
        {
            if (CurrentStackAmount<5)
            {
                CurrentStackAmount++;
                CurrentPlayer.melee_crit += DefaultStackForMeleeCrit;
                CurrentPlayer.hit_power += DefaultStackForAttpower;
                TimeOfLastStackAdded = DateTime.Now;
                UpdateTimer();
            }
        }

        
        private void EndTickAndRemove()
        {

            CurrentPlayer.melee_crit -= CurrentStackAmount * DefaultStackForMeleeCrit;
            CurrentPlayer.hit_power -= CurrentStackAmount * DefaultStackForAttpower;
            CurrentStackAmount = 0;
            TimeOfLastStackAdded = DateTime.Now;

            CurrentPlayer.remove_condition_in_player(id_condition);
            id_condition = functions.get_symb_for_IDs();
        }

        private void UpdateTimer()
        {
            //float _time_left = TimeForTickingOfCurrentStack - (float)DateTime.Now.Subtract(TimeOfLastStackAdded).TotalSeconds;
            
            CurrentPlayer.set_condition("co", SpecializationSpellIndex, id_condition, 99, CurrentStackAmount);
        }


    }

    public struct Packets
    {
        public long CurrentTimeStamp;
        //public int CurrentOrder;
        public string Packet;

        public Packets(long _time_stamp, /*int _order,*/ string _packet)
        {
            CurrentTimeStamp = _time_stamp;
            //CurrentOrder = _order;
            Packet = _packet;
        }
    }
            

}
