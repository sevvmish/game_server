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
        
        public int player_order;
        public string player_id;
        public string player_name;
        public int player_class;
        public string connection_number;
        public int team_id;
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
        //public Queue<string> CurrentPacket = new Queue<string>();
        public string CurrentPacket;
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
        

        public Players(params string [] data)
        {
            this.player_order = int.Parse(data[0]);
            this.player_id = data[1];
            this.player_name = data[2];
            this.player_class = int.Parse(data[3]);
            this.connection_number = data[4];
            this.team_id = int.Parse(data[5]);
            this.zone_type = int.Parse(data[6]);
            this.position_x = float.Parse(data[7]);
            this.position_y = float.Parse(data[8]);
            this.position_z = float.Parse(data[9]);
            this.rotation_x = float.Parse(data[10]);
            this.rotation_y = float.Parse(data[11]);
            this.rotation_z = float.Parse(data[12]);
            this.speed = float.Parse(data[13]);
            this.animation_id = int.Parse(data[14]);
            //this.conditions = data[15];
            this.health_pool = data[16];
            this.energy = float.Parse(data[17]);
            this.health_regen = float.Parse(data[18]);
            this.energy_regen = float.Parse(data[19]);
            this.weapon_attack = data[20];
            this.hit_power = float.Parse(data[21]);
            this.armor = float.Parse(data[22]);
            this.shield_block = float.Parse(data[23]);
            this.magic_resistance = float.Parse(data[24]);
            this.dodge = float.Parse(data[25]);
            this.cast_speed = float.Parse(data[26]);
            this.melee_crit = float.Parse(data[27]);
            this.magic_crit = float.Parse(data[28]);
            this.spell_power = float.Parse(data[29]);
            this.spell1 = int.Parse(data[30]);
            this.spell2 = int.Parse(data[31]);
            this.spell3 = int.Parse(data[32]);
            this.spell4 = int.Parse(data[33]);
            this.spell5 = int.Parse(data[34]);
            this.spell6 = 997;
            this.hidden_conds = data[35];
            this.global_button_cooldown = int.Parse(data[36]);

            this.OrderNumber = 0;
            //starter.PlayersPool.Add(this.player_id, this);
        }


        public string GetPacketForSending(string current_player_id)
        {
            string[] health_parts = this.health_pool.Split('=');
            this.health_pool = $"{Math.Round(float.Parse(health_parts[0]), 1).ToString("f0")}={Math.Round(float.Parse(health_parts[1]), 1).ToString("f0")}";

            StringBuilder sb = new StringBuilder(110);


            string conditions_refactor = null;
            try
            {
                foreach (string key in this.conditions.Keys.ToList())
                {
                    conditions_refactor = conditions_refactor + key + this.conditions[key];  //Console.WriteLine(key + " - " + t[key]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            if (player_id != current_player_id && is_invisible)
            {
                sb.Append("100~100~" +
                    this.rotation_y.ToString("f1") + "~" + 
                    this.animation_id + "~" + conditions_refactor + "~" + this.health_pool + "~" + this.energy.ToString("f0") + "^");
            } else
            {
                sb.Append(this.position_x.ToString("f1") + "~" + this.position_z.ToString("f1") + "~" +
                this.rotation_y.ToString("f1") + "~" + 
                this.animation_id + "~" + conditions_refactor + "~" + this.health_pool + "~" + this.energy.ToString("f0") + "^");
            }

            return sb.ToString();

        }

        public string GetPacketForSending_nonPlayer()
        {
            string[] health_parts = this.health_pool.Split('=');
            this.health_pool = $"{Math.Round(float.Parse(health_parts[0]), 1).ToString("f0")}={Math.Round(float.Parse(health_parts[1]), 1).ToString("f0")}";

            StringBuilder sb = new StringBuilder(110);


            string conditions_refactor = null;
            try
            {
                foreach (string key in this.conditions.Keys.ToList())
                {
                    conditions_refactor = conditions_refactor + key + this.conditions[key];  //Console.WriteLine(key + " - " + t[key]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
                       
            
            sb.Append(player_id + "~" + player_class + "~" + player_name + "~" + this.position_x.ToString("f1") + "~" + this.position_z.ToString("f1") + "~" +
            this.rotation_y.ToString("f1") + "~" +
            this.animation_id + "~" + conditions_refactor + "~" + this.health_pool + "~" + this.energy.ToString("f0") + "^");
            
            return sb.ToString();

        }

        public string GetPacketForSending()
        {
            string[] health_parts = this.health_pool.Split('=');
            this.health_pool = $"{Math.Round(float.Parse(health_parts[0]), 1).ToString("f0")}={Math.Round(float.Parse(health_parts[1]), 1).ToString("f0")}";

            StringBuilder sb = new StringBuilder(110);

            
            string conditions_refactor = null;
            try
            {
                foreach (string key in this.conditions.Keys.ToList())
                {
                    conditions_refactor = conditions_refactor + key + this.conditions[key];  //Console.WriteLine(key + " - " + t[key]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            sb.Append(this.position_x.ToString("f1") +"~" + this.position_z.ToString("f1") + "~" +
               this.rotation_y.ToString("f1") + "~" +
                this.animation_id + "~" + conditions_refactor + "~" + this.health_pool + "~" + this.energy.ToString("f0") + "^");

            return sb.ToString();

        }

        public void Dispose()
        {
            this.Dispose();
        }
                
        public void ZeroInputs()
        {
            this.horizontal_touch = 0;
            this.vertical_touch = 0;
            this.button1 = false;
            this.button2 = false;
            this.button3 = false;
            this.button4 = false;
            this.button5 = false;
            this.button6 = false;
            this.is_strafe_on = false;
    }

        //set condition
        public void set_condition(string cond_type, int condition_number, string cond_id, float timer)
        {
            string x;
            this.conditions.Remove(cond_id, out x);
            this.conditions.TryAdd(cond_id, $":{cond_type}-{condition_number}-{timer.ToString("f1")},");
        }

        /*
        //set hidden condition
        public void set_hiddenconds(string cond_type, string cond_id)
        {            
            this.hidden_conds.Remove(cond_id);
            this.hidden_conds.Add(cond_id, $":{cond_type},");
        }
        */

        /*
        //find any HIDDEN COND like type by naming condition only
        public bool is_hiddencond_here_by_type(string searched_cond)
        {            
            foreach (string vall in this.conditions.Values.ToList())
            {
                if (vall.Contains(searched_cond))
                {
                    return true;
                }
            }

            return false;
        }
        */

        //find in certain cond type any of spells in array
        public bool is_any_cond_inarray_of_spellnumber(string type_of_cond, string[] array_to_check)
        {
            
            if (this.conditions.Count == 0)
            {
                return false;
            }

            try
            {
                foreach (string vall in this.conditions.Values.ToList())
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
                foreach (string vall in this.conditions.Keys.ToList())
                {
                    if (this.conditions[vall].Contains(searched_cond))
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

        //get bulk data in condition by any cond data
        public string get_bulkconditiondata_by_anything_in_bulk(string searched_cond)
        {
            try
            {
                foreach (string vall in this.conditions.Keys.ToList())
                {
                    if (this.conditions[vall].Contains(searched_cond))
                    {
                        return this.conditions[vall];
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
                foreach (string vall in this.conditions.Keys.ToList())
                {
                    if (this.conditions[vall].Contains(searched_cond))
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
            string x;
            conditions.TryRemove(conds_id, out x);
            conditions.TryAdd(conds_id, $":co-1006-{tick_time_left.ToString("f1").Replace(',', '.')},");
        }
        


        //IMMOBILIZE
        public void make_immob(string conds_id, float tick_time_left)
        {
            //spells.set_animation_for_one($table_id, $player->player_id, 0, 2, 0.01);
            animation_id = 0;
            string x;
            conditions.TryRemove(conds_id, out x);
            conditions.TryAdd(conds_id, $":co-1001-{tick_time_left.ToString("f1").Replace(',', '.')},");

        }
      


        //STUN
        public void make_stun(string conds_id, float tick_time_left)
        {            
            this.animation_id = 8;
            string x;
            this.conditions.TryRemove(conds_id, out x);
            this.conditions.TryAdd(conds_id, $":co-1002-{tick_time_left.ToString("f1")},");
        }

        //cheking for 1002 and 1003 and 1005 stopping casting
        public bool is_casting_stopped_by_spells()
        {
            string[] array_of_spells = new string[] { "1002", "1003", "1005", "1006" };
            return is_any_cond_inarray_of_spellnumber("co", array_of_spells);
        }


        public bool is_cond_here_by_type_and_spell(string searched_cond)
        {
            try
            {
                foreach (string items in this.conditions.Values.ToList())
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

        /*
        public bool is_hiddencond_here_by_type_and_spell(string searched_cond)
        {
            
            foreach (string items in this.hidden_conds.Values.ToList())
            {
                if (items.Contains(searched_cond))
                {
                    return true;
                }
            }
            return false;
        }
        */

        //cheking for 1002 and 1003 and 1005 plus MOVEMENET and SPELLS
        public bool is_casting_failed()
        {

            
            if (Math.Abs(this.vertical_touch) > 1.5f)
            {
                return true;
            }

            if (is_spell_touched_for_casting_failing)
            {
                return true;
            }


            /*
            if (this.is_spell_button_touched)
            {
                
                return true;
            }*/

            string[] array_of_spells = new string[] { "1002", "1003", "1005", "1006" };
            bool result = is_any_cond_inarray_of_spellnumber("co", array_of_spells);
            if (result)
            {
                return true;
            }


            return false;
        }


        /*
        //find any HIDDEN COND like type by naming condition only
        public bool is_hiddencond_here_by_type_only(string searched_cond)
        {
            
            foreach (string vall in this.conditions.Values.ToList())
            {
                if (vall.Contains(searched_cond))
                {
                    return true;
                }
            }

            return false;
        }
        */


        public void minus_energy(float amount)
        {            
            this.energy = this.energy - amount;
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
}
