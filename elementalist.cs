using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_server
{
    class elementalist
    {


        public static async void frostbolt(string table_id, string me, float energy_cost)
        {
            Players player = functions.GetPlayerData(table_id, me);
            float default_distance = 15;
          
            player.start_spell_in_process();
            functions.turn_to_enemy(me, table_id, 0.1f, default_distance, -15, default_distance);

            //casting part
            string check_cond_id = functions.get_symb_for_IDs();
            int spell_id = 56;

            float cast_time = 1.5f * (100 - player.cast_speed) / 100;

            for (float i = cast_time; i > 0; i -= 0.1f)
            {
                functions.turn_to_enemy(me, table_id, 0.1f, default_distance, -15, default_distance);
                if (!player.is_casting_failed())
                {
                    player.animation_id = 3;
                    string x;
                    player.conditions.TryRemove(check_cond_id, out x);
                    player.conditions.TryAdd(check_cond_id, $":ca-{spell_id}-{i.ToString("f1").Replace(',', '.')},");
                }
                else
                {
                    spells.remove_condition_in_player(table_id, me, check_cond_id);
                    functions.inform_of_cancel_casting(me, table_id, spell_id);
                    spells.reset_animation_for_one(table_id, me);                
                    player.stop_spell_in_process();
                    return;
                }
                await Task.Delay(100);
            }

            spells.remove_condition_in_player(table_id, me, check_cond_id);       
            player.stop_spell_in_process();
            player.minus_energy(energy_cost);
            //end casting

            string check_cond_id2 = functions.get_symb_for_IDs();
            player.animation_id = 5;
            spells.reset_animation_for_one(table_id, me);
            float[] magic_data = new float[] { player.position_x, player.position_y, player.position_z, player.rotation_x, player.rotation_y, player.rotation_z };
            float default_player_x = player.position_x;
            float default_player_z = player.position_z;
            List<Players> result = new List<Players>();

            for (float i = 0; i < 2; i += 0.05f)
            {
                float curr_dist = functions.vector3_distance_unity(default_player_x, 0, default_player_z, magic_data[0], 0, magic_data[2]);
                if (curr_dist > default_distance)
                {
                    break;
                }
                string x;
                player.conditions.TryRemove(check_cond_id2, out x);
                functions.mover(ref magic_data, 0, 10, 1.5f);
                //check_cond_id2 = functions.get_random_set_of_symb(4);
                player.conditions.TryAdd(check_cond_id2, $":cs={spell_id}={magic_data[0].ToString("f1").Replace(',', '.')}={magic_data[2].ToString("f1").Replace(',', '.')},");
                result = functions.get_all_nearest_enemy_inradius(magic_data[0], magic_data[2], me, table_id, 1);
                if (result.Count > 0)
                {
                    for (int u = 0; u < result.Count; u++)
                    {
                        if (result.Count > 0)
                        {
                            spells.make_direct_magic_damage_exact_enemy(table_id, me, result[u].player_id, spell_id, 0, 1, 2);
                            foreach (string item in result[u].conditions.Values)
                            {
                                if (!item.Contains("co-59"))
                                {
                                    freezing_slow(table_id, result[u].player_id, 3);
                                }
                            }
                        }
                    }
                    break;
                }

                if (magic_data[6] == 1)
                {
                    break;
                }
                await Task.Delay(50);
            }
            spells.remove_condition_in_player(table_id, me, check_cond_id2);
        }


        public static async void freezing_slow(string table_id, string aim, float time_for_slow)
        {
            Players aim_player = functions.GetPlayerData(table_id, aim);
            aim_player.speed *= 0.6f;
            string ID_cond = functions.get_symb_for_IDs();

            for (float i = time_for_slow; i > 0; i-=0.1f)
            {
                aim_player.set_condition("co", 59, ID_cond, i);

                await Task.Delay(100);
            }
            aim_player.speed /= 0.6f;
            spells.remove_condition_in_player(table_id, aim, ID_cond);
        }


        public static async void firebolt51(string table_id, string me, float energy_cost)
        {
            Players player = functions.GetPlayerData(table_id, me);
            float default_distance = 15;
            //player.set_hiddenconds("sip", "1111");
            //player.is_spell_in_process = true;
            player.start_spell_in_process();
            functions.turn_to_enemy(me, table_id, 0.1f, default_distance, -15, default_distance);

            //casting part
            string check_cond_id = functions.get_symb_for_IDs();
            int spell_id = 51;

            float cast_time = 2 * (100 - player.cast_speed) / 100;

            for (float i = cast_time; i > 0; i -= 0.1f)
            {
                functions.turn_to_enemy(me, table_id, 0.1f, default_distance, -15, default_distance);
                if (!player.is_casting_failed())
                {
                    player.animation_id = 3;
                    string x;
                    player.conditions.TryRemove(check_cond_id, out x);
                    player.conditions.TryAdd(check_cond_id, $":ca-{spell_id}-{i.ToString("f1").Replace(',', '.')},");
                }
                else
                {
                    spells.remove_condition_in_player(table_id, me, check_cond_id);
                    functions.inform_of_cancel_casting(me, table_id, 51);
                    spells.reset_animation_for_one(table_id, me);
                    //player.hidden_conds.Remove("1111");
                    //player.is_spell_in_process = false;
                    player.stop_spell_in_process();
                    return;
                }
                await Task.Delay(100);
            }

            spells.remove_condition_in_player(table_id, me, check_cond_id);
            //player.hidden_conds.Remove("1111");
            //player.is_spell_in_process = false;
            player.stop_spell_in_process();
            player.minus_energy(energy_cost);
            //end casting

            string check_cond_id2 = functions.get_symb_for_IDs();
            player.animation_id = 5;
            spells.reset_animation_for_one(table_id, me);
            float[] magic_data = new float[] { player.position_x, player.position_y, player.position_z, player.rotation_x, player.rotation_y, player.rotation_z };
            float default_player_x = player.position_x;
            float default_player_z = player.position_z;
            List<Players> result = new List<Players>();

            for (float i = 0; i < 2; i += 0.05f)
            {
                float curr_dist = functions.vector3_distance_unity(default_player_x, 0, default_player_z, magic_data[0], 0, magic_data[2]);
                if (curr_dist > default_distance)
                {
                    break;
                }
                string x;
                player.conditions.TryRemove(check_cond_id2, out x);
                functions.mover(ref magic_data, 0, 10, 1.5f);
                //check_cond_id2 = functions.get_random_set_of_symb(4);
                player.conditions.TryAdd(check_cond_id2, $":cs=51={magic_data[0].ToString("f1").Replace(',', '.')}={magic_data[2].ToString("f1").Replace(',', '.')},");
                result = functions.get_all_nearest_enemy_inradius(magic_data[0], magic_data[2], me, table_id, 1);
                if (result.Count > 0)
                {
                    for (int u = 0; u < result.Count; u++)
                    {
                        if (result.Count > 0)
                        {
                            spells.make_direct_magic_damage_exact_enemy(table_id, me, result[u].player_id, 51, 0, 1, 2);
                        }
                    }
                    break;
                }

                if (magic_data[6] == 1)
                {
                    break;
                }
                await Task.Delay(50);
            }
            spells.remove_condition_in_player(table_id, me, check_cond_id2);
        }


        //spell 54 firewalk===========================================
        public static async void firewalk(string table_id, string me, float base_damage)
        {
            string check_cond_id = "";
            float old_x = 0.01f;
            float old_z = 0.01f;
            Players player = functions.GetPlayerData(table_id, me);
            int stacks_on_place = 0;

            for (float i = 2; i > 0; i -= 0.1f)
            {
                if (Math.Abs(player.position_x - old_x) < 0.3f && Math.Abs(player.position_z - old_z) < 0.3f)
                {
                    stacks_on_place++;
                    if (stacks_on_place==10)
                    {
                        stacks_on_place = 0;
                        
                        float point_x = player.position_x;
                        float point_z = player.position_z;
                        string x;
                        player.conditions.TryRemove(check_cond_id, out x);
                        check_cond_id = functions.get_symb_for_IDs();
                        player.conditions.TryAdd(check_cond_id, $":cs=54={point_x.ToString("f1").Replace(',', '.')}={point_z.ToString("f1").Replace(',', '.')},");
                        firestep(table_id, me, base_damage, point_x, point_z, 0.75f);
                        old_x = point_x;
                        old_z = point_z;
                    }
                }
                else
                {
                    float point_x = player.position_x;
                    float point_z = player.position_z;
                    string x;
                    player.conditions.TryRemove(check_cond_id, out x);
                    check_cond_id = functions.get_symb_for_IDs();
                    player.conditions.TryAdd(check_cond_id, $":cs=54={point_x.ToString("f1").Replace(',', '.')}={point_z.ToString("f1").Replace(',', '.')},");
                    firestep(table_id, me, base_damage, point_x, point_z, 0.75f);
                    old_x = point_x;
                    old_z = point_z;
                }
                await Task.Delay(100);
            }

            spells.remove_condition_in_player(table_id, me, check_cond_id);
        }


        // addition to spell 54
        public static async void firestep(string table_id, string me, float base_damage, float pos_x, float pos_z, float radius)
        {

            int hit_counter = 0;
            List<Players> hit_players = new List<Players>();
            for (float i = 1; i > 0; i -= 0.25f)
            {
                List<Players> result = functions.get_all_nearest_enemy_inradius(pos_x, pos_z, me, table_id, radius);
                if (result.Count > 0)
                {
                    for (int u = 0; u < result.Count; u++)
                    {
                        if (!hit_players.Contains(result[u]))
                        {
                            hit_players.Add(result[u]);
                        }
                    }

                    /*
                    for (int u = 0; u < result.Count; u++)
                    {
                        make_direct_magic_damage_exact_enemy(table_id, me, result[u].player_id, 54, base_damage, 0.33f, 2);
                    }
                    */
                }

                //damage case============
                hit_counter++;
                if (hit_counter == 2)
                {
                    for (int u = 0; u < hit_players.Count; u++)
                    {
                        if (result.Count > 0)
                        {
                            spells.make_direct_magic_damage_exact_enemy(table_id, me, result[u].player_id, 54, base_damage / 2, 2, 2);
                        }
                    }
                    hit_counter = 0;
                    hit_players.Clear();
                }
                //========================

                await Task.Delay(250);
            }
        }





        //spell53 - fire hands blow
        public static async void firehands(string table_id, string mee, float base_damage)
        {
            float how_long = 2;
            float distance = 4.5f;

            functions.turn_to_enemy(mee, table_id, 0.1f, distance, -15, distance);
            string check_cond_id_for_casting = functions.get_symb_for_IDs();
            string check_cond_id_for_spellcha = functions.get_symb_for_IDs();

            Players player = functions.GetPlayerData(table_id, mee);
            player.animation_id = 13;
            player.conditions.TryAdd(check_cond_id_for_spellcha, $":co-53-{(how_long + 0.5f).ToString("f1").Replace(',', '.')},");
            await Task.Delay(500);

            player.start_spell_in_process();
            int hit_counter = 0;
            List<Players> hit_players = new List<Players>();
            for (float i = how_long; i > 0; i -= 0.250f)
            {
                functions.turn_to_enemy(mee, table_id, 0.1f, distance - 1, -15, distance + 1);
                if (!player.is_casting_failed())
                {
                    player.animation_id = 13;
                    string x;
                    player.conditions.TryRemove(check_cond_id_for_casting, out x);
                    player.conditions.TryRemove(check_cond_id_for_spellcha, out x);
                    player.conditions.TryAdd(check_cond_id_for_casting, $":ca-53-{i.ToString("f1").Replace(',', '.')},");
                    player.conditions.TryAdd(check_cond_id_for_spellcha, $":co-53-{i.ToString("f1").Replace(',', '.')},");
                    List<Players> all_players = functions.get_all_nearest_enemy_inmelee(mee, table_id, 2.5f, -10);
                    if (all_players.Count > 0)
                    {
                        for (int u = 0; u < all_players.Count; u++)
                        {
                            if (!hit_players.Contains(all_players[u]))
                            {
                                hit_players.Add(all_players[u]);
                            }
                        }
                        /*
                        for (int u = 0; u < all_players.Count; u++)
                        {
                            make_direct_magic_damage_exact_enemy(table_id, mee, all_players[u].player_id, 53, damage_per_enemy, power, 2);
                        }
                        */
                    }
                }
                else
                {
                    
                    spells.remove_condition_in_player(table_id, mee, check_cond_id_for_casting);
                    spells.remove_condition_in_player(table_id, mee, check_cond_id_for_spellcha);
                    player.stop_spell_in_process();
                    functions.inform_of_cancel_casting(mee, table_id, 53);
                    spells.reset_animation_for_one(table_id, mee);
                    return;
                }


                //damage case============
                hit_counter++;
                if (hit_counter == 2)
                {
                    for (int u = 0; u < hit_players.Count; u++)
                    {
                        spells.make_direct_magic_damage_exact_enemy(table_id, mee, hit_players[u].player_id, 53, base_damage / 2, 2, 2);
                    }
                    hit_counter = 0;
                    hit_players.Clear();
                }
                //========================


                await Task.Delay(250);
            }

            player.stop_spell_in_process();
            spells.remove_condition_in_player(table_id, mee, check_cond_id_for_casting);
            spells.remove_condition_in_player(table_id, mee, check_cond_id_for_spellcha);
            spells.reset_animation_for_one(table_id, mee);
        }



        //spell52
        public static async void meteor(string table_id, string mee)
        {
            Players player = functions.GetPlayerData(table_id, mee);
            float point_x = player.position_x;
            float point_z = player.position_z;
            string cond_id = functions.get_symb_for_IDs();

            for (float i = 0; i < 0.1f; i += 0.05f)
            {
                //remove_condition_in_player(table_id, mee, cond_id);
                string x;
                player.conditions.TryRemove(cond_id, out x);
                player.conditions.TryAdd(cond_id, $":cs=52={point_x.ToString("f1").Replace(',', '.')}={point_z.ToString("f1").Replace(',', '.')},");
                await Task.Delay(50);
            }
            await Task.Delay(1400);

            spells.remove_condition_in_player(table_id, mee, cond_id);
            //explosion============================================
            List<Players> result_p = functions.get_all_nearest_enemy_inradius(point_x, point_z, mee, table_id, 2);
            //List<string> conds_ids_for_stun = new List<string>();
            Dictionary<string, Players> result = new Dictionary<string, Players>();

            for (int i = 0; i < result_p.Count; i++)
            {
                result.Add(functions.get_symb_for_IDs(), result_p[i]);
            }
            

            foreach (string item in result.Keys)
            {
                spells.make_direct_magic_damage_exact_enemy(table_id, mee, result[item].player_id, 52, 20, 1, 2);                
            }

            
            for (float i = 1; i > 0; i-=0.1f)
            {
                foreach (string item in result.Keys)
                {
                    result[item].make_stun(item, i);
                    result[item].set_condition("co", 52, item, i);
                }
                await Task.Delay(100);
            }

            foreach (string item in result.Keys)
            {
                spells.remove_condition_in_player(table_id, result[item].player_id, item);
                spells.reset_animation_for_one(table_id, result[item].player_id);
            }

        }


        //freezed any spell
        public static async void freezed(string table_id, string me, string enemy, float time)
        {
            Players player = functions.GetPlayerData(table_id, enemy);

            if (spells.if_resisted_magic(table_id, me, enemy) || player.is_immune_to_movement_imparing)
            {
                return;
            }

            
            string check_cond_id = functions.get_symb_for_IDs();
            string conds_id = functions.get_symb_for_IDs();
            string x;
            for (float i = time; i > 0; i -= 0.1f)
            {
                player.conditions.TryRemove(check_cond_id, out x);
                player.conditions.TryAdd(check_cond_id, $":co-58-{i.ToString("f1").Replace(',', '.')},");
                player.make_immob(conds_id, i);
                await Task.Delay(100);

                if (player.is_immune_to_movement_imparing)
                {
                    break;
                }
            }

            spells.remove_condition_in_player(table_id, enemy, check_cond_id);
            spells.remove_condition_in_player(table_id, enemy, conds_id);
        }


    }
}
