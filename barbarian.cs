using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_server
{
    class barbarian
    {
        //spell 107 make slower
        public static async void slow(string table_id, string aim, float speed_decrease, float how_long)
        {
            float koef = 1f - speed_decrease;
            if (koef<0)
            {
                return;
            }

            Players player = functions.GetPlayerData(table_id, aim);

            if (player.is_cond_here_by_type_and_spell("co-107"))
            {
                return;
            }

            player.speed *= koef;
            string ID = functions.get_symb_for_IDs();

            for (float i = how_long; i > 0; i-=0.25f)
            {
                player.set_condition("co", 107, ID, i);
                if (player.is_stop_all_condition_by_checking_index(107))
                {
                    break;
                }

                await Task.Delay(250);
            }

            player.speed /= koef;
            player.remove_condition_in_player(ID);

        }


        //spell 106 throw axe
        public static async void throw_axe(string table_id, string me, float distance)
        {
            Players player = functions.GetPlayerData(table_id, me);
            player.is_spell_in_process = true;
            player.is_reset_any_button = true;
            string x;
            player.animation_id = 11;
            player.reset_animation_for_one();
            float time_to_fly_there = 2f;
            float time_to_fly_back = 2f;

            functions.turn_to_enemy(me, table_id, 0.1f, distance, 0, distance);

            float[] axe_aim_coords = new float[2] { player.position_x, player.position_z };
            float [] axe_start_coords = new float[2] { player.position_x, player.position_z };
            float[] axe_current_coords = new float[2] { player.position_x, player.position_z };
            float rot_y = player.rotation_y;

            string ID_for_cs = functions.get_symb_for_IDs();
            player.conditions.TryAdd(ID_for_cs, $":cs=106={player.position_x.ToString("f1").Replace(',', '.')}={player.position_z.ToString("f1").Replace(',', '.')},");
            
            functions.projection(ref axe_aim_coords, player.position_x, player.position_z, player.rotation_y, distance);
            
            await Task.Delay(350);
            
            player.is_reset_any_button = false;
            functions.turn_to_enemy(me, table_id, 0.1f, distance, 0, distance);

            List<Players> victims = new List<Players>();
            List<Players> pre_victims = new List<Players>();
            
            float koef = 0;
            for (float i = time_to_fly_there; i > 0; i-=0.2f)
            {
                functions.lerp(ref axe_current_coords, axe_start_coords[0], axe_start_coords[1], axe_aim_coords[0], axe_aim_coords[1], rot_y, koef * (1f / (time_to_fly_there / 0.2f)));
                
                
                player.conditions.TryRemove(ID_for_cs, out x);
                player.conditions.TryAdd(ID_for_cs, $":cs=106={axe_current_coords[0].ToString("f1").Replace(',', '.')}={axe_current_coords[1].ToString("f1").Replace(',', '.')},");

                //Console.WriteLine($":cs=106={axe_current_coords[0].ToString("f1").Replace(',', '.')}={axe_current_coords[1].ToString("f1").Replace(',', '.')},");
                               
                if (koef > 0)
                {
                    pre_victims = functions.get_all_nearest_enemy_inradius(axe_current_coords[0], axe_current_coords[1], me, table_id, 0.5f);                    
                    if (pre_victims.Count > 0)
                    {
                        for (int iii = 0; iii < pre_victims.Count; iii++)
                        {
                            if (!victims.Contains(pre_victims[iii]))
                            {
                                victims.Add(pre_victims[iii]);
                                spells.make_direct_melee_damage_exact_enemy(table_id, me, pre_victims[iii].player_id, 106, 0, 0.5f, 1.5f, 0);
                                slow(table_id, pre_victims[iii].player_id, 0.3f, 3f);                                
                            }
                        }
                    }
                }


                await Task.Delay((int)(koef*100/(time_to_fly_there/0.2f)));
                koef++;
            }
            axe_start_coords[0] = axe_current_coords[0];
            axe_start_coords[1] = axe_current_coords[1];
            victims.Clear();
            pre_victims.Clear();

            //========
            koef = 0;
            float[] axe_y = new float[6] { axe_current_coords[0], 0, axe_current_coords[1], 0, rot_y, 0 };
            for (float i = time_to_fly_back; i > 0; i -= 0.2f)
            {
                functions.turn_object_to_exact_player(me, table_id, ref axe_y);
                functions.lerp(ref axe_current_coords, axe_start_coords[0], axe_start_coords[1], player.position_x, player.position_z, axe_y[4], koef * (1f / (time_to_fly_back / 0.2f)));

                axe_y[0] = axe_current_coords[0];                
                axe_y[2] = axe_current_coords[1];
                
                player.conditions.TryRemove(ID_for_cs, out x);
                player.conditions.TryAdd(ID_for_cs, $":cs=106={axe_current_coords[0].ToString("f1").Replace(',', '.')}={axe_current_coords[1].ToString("f1").Replace(',', '.')},");

                if (koef > 1f)
                {
                    pre_victims = functions.get_all_nearest_enemy_inradius(axe_current_coords[0], axe_current_coords[1], me, table_id, 0.5f);
                    if (pre_victims.Count > 0)
                    {
                        for (int iii = 0; iii < pre_victims.Count; iii++)
                        {
                            if (!victims.Contains(pre_victims[iii]))
                            {
                                victims.Add(pre_victims[iii]);
                                spells.make_direct_melee_damage_exact_enemy(table_id, me, pre_victims[iii].player_id, 106, 0, 0.5f, 1.5f, 0);
                                slow(table_id, pre_victims[iii].player_id, 0.3f, 3f);
                            }
                        }
                    }
                }

                await Task.Delay((int)(((time_to_fly_back/0.2f)-koef)*10));
                koef++;
            }

            player.is_spell_in_process = false;
            player.CastEndCS(player.position_x, player.position_z, ID_for_cs, 106);
            
        }



        //block prep 104
        public static async void block_prep(string table_id, string me, float block_time)
        {
            string check_cond_id = functions.get_symb_for_IDs();
            string check_immob_id = functions.get_symb_for_IDs();
            Players player = functions.GetPlayerData(table_id, me);
            player.shield_block += 50;
            
            player.start_spell_in_process();
            player.is_reset_movement_not_rotation = true;

            for (float i = block_time; i > 0; i -= 0.25f)
            {
                if (!player.is_casting_stopped_by_spells())
                {
                    //player.make_immob(check_immob_id, i);
                    string x;
                    player.conditions.TryRemove(check_cond_id, out x);
                    player.conditions.TryAdd(check_cond_id, $":co-104-{i.ToString("f1").Replace(',', '.')},");
                    player.animation_id = 10;
                    if (player.get_id_by_type_and_spell("me-b") != null)
                    {

                    }


                }
                else
                {
                    break;
                }

                await Task.Delay(250);
            }

            player.shield_block -= 50;
            player.reset_animation_for_one();
            player.remove_condition_in_player(check_cond_id);
            player.remove_condition_in_player(check_immob_id);
            player.stop_spell_in_process();
            player.is_reset_movement_not_rotation = false;
        }




        //spell 103 HEROIC LEAP
        public static async void heroic_leap(string table_id, string me, float max_distance)
        {
            string check_cond_id = functions.get_symb_for_IDs();
            functions.turn_to_enemy(me, table_id, 0.1f, max_distance, -10, max_distance);

            float time_for_slow = 2f;

            Players player1 = functions.GetPlayerData(table_id, me);
            Players enemy = functions.get_one_nearest_enemy_inmelee(me, table_id, max_distance - 2, -10, true);
            float start_x = player1.position_x;
            float start_z = player1.position_z;
            player1.animation_id = 16;
            player1.start_spell_in_process();
            player1.is_reset_any_button = true;
            string x;
            string check_cond_strike_id = functions.get_symb_for_IDs();

            for (float i = 0.6f; i > 0; i -= 0.1f)
            {
                if (player1.is_casting_stopped_by_spells() )
                {
                    player1.reset_animation_for_one();
                    player1.remove_condition_in_player(check_cond_id);
                    player1.stop_spell_in_process();
                    player1.is_reset_any_button = false;
                    return;
                }
                float distance = functions.vector3_distance_unity(player1.position_x, 0, player1.position_z, start_x, 0, start_z);
                if (distance >= max_distance)
                {
                    player1.reset_animation_for_one();
                    break;
                }
                
                player1.conditions.TryRemove(check_cond_id, out x);
                player1.conditions.TryAdd(check_cond_id, $":co-103-{i.ToString("f1").Replace(',', '.')},");
                enemy = functions.get_one_nearest_enemy_inmelee(me, table_id, -1.5f, -10, true);

                if (enemy != null)
                {

                    player1.animation_id = 0;
                    break;
                }

                float[] new_pos_rot = new float[] { player1.position_x, 0, player1.position_z, 0, player1.rotation_y, 0 };
                functions.mover(ref new_pos_rot, 0, 20, 1.2f);
                player1.position_x = new_pos_rot[0];
                player1.position_y = new_pos_rot[1];
                player1.position_z = new_pos_rot[2];


                await Task.Delay(100);
            }

                        
            player1.conditions.TryRemove(check_cond_strike_id, out x);
            player1.conditions.TryAdd(check_cond_strike_id, $":cs=103={player1.position_x.ToString("f1").Replace(',', '.')}={player1.position_z.ToString("f1").Replace(',', '.')},");
            List<Players> enemies = functions.get_all_nearest_enemy_inradius(player1.position_x, player1.position_z, me, table_id, 2);
            List<string> IDs_for_slow = new List<string>(enemies.Count);
            if (enemies.Count > 0)
            {
                for (int u = 0; u < enemies.Count; u++)
                {
                    IDs_for_slow.Add(functions.get_symb_for_IDs());
                    enemies[u].make_slow(IDs_for_slow[u], time_for_slow);
                    enemies[u].speed *= 0.6f;
                    enemies[u].make_broken_casting();
                    spells.make_direct_melee_damage_exact_enemy(table_id, me, enemies[u].player_id, 103, 0, 1, 2, 0);
                }
            }

            //await Task.Delay(500);

            player1.stop_spell_in_process();
            player1.reset_animation_for_one();
            player1.remove_condition_in_player(check_cond_id);
            player1.remove_condition_in_player(check_cond_strike_id);
            
            await Task.Delay(1000);
            player1.is_reset_any_button = false;
            for (int u = 0; u < enemies.Count; u++)
            {
                enemies[u].remove_condition_in_player(IDs_for_slow[u]);
                enemies[u].speed /= 0.6f;                
            }
        }


        //spell 102 hurricane
        public static async void hurricane(string table_id, string me, float how_long)
        {
            string check_cond_id = functions.get_symb_for_IDs();
            Players player = functions.GetPlayerData(table_id, me);

            player.speed *= 0.5f;
            player.start_spell_in_process();

            List<Players> hit_players = new List<Players>();
            int hit_counter = 0;
            player.is_immune_to_movement_imparing = true;

            for (float i = how_long; i > 0; i -= 0.25f)
            {
                if (!player.is_casting_stopped_by_spells())
                {
                    player.animation_id = 15;
                    string x;
                    player.conditions.TryRemove(check_cond_id, out x);
                    //check_cond_id = functions.get_random_set_of_symb(4);
                    player.conditions.TryAdd(check_cond_id, $":co-102-{i.ToString("f1").Replace(',', '.')},");

                    List<Players> enemies = functions.get_all_nearest_enemy_inradius(player.position_x, player.position_z, me, table_id, 3);
                    if (enemies.Count > 0)
                    {
                        for (int u = 0; u < enemies.Count; u++)
                        {
                            if (!hit_players.Contains(enemies[u]))
                            {
                                hit_players.Add(enemies[u]);
                            }
                        }
                        /*
                        for (int u = 0; u < enemies.Count; u++)
                        {
                            make_direct_melee_damage_exact_enemy(table_id, me, enemies[u].player_id, 102, 0, 0.5f, 2, 0);
                        }
                        */
                    }

                }
                else
                {
                    functions.inform_of_cancel_casting(me, table_id, 102);
                    break;
                }

                //damage case============
                hit_counter++;
                if (hit_counter == 2)
                {
                    for (int u = 0; u < hit_players.Count; u++)
                    {
                        spells.make_direct_melee_damage_exact_enemy(table_id, me, hit_players[u].player_id, 102, 0, 0.25f, 2, 0);
                    }
                    hit_counter = 0;
                    hit_players.Clear();
                }
                //========================
                
                await Task.Delay(250);
            }

            player.is_immune_to_movement_imparing = false;
            player.stop_spell_in_process();
            player.remove_condition_in_player(check_cond_id);
            player.speed /= 0.5f;
            player.reset_animation_for_one();
        }





        // spell 101
        public static void barbarian_melee_hit(string table_id, string player)
        {
            functions.turn_to_enemy(player, table_id, 0.1f, 0.5f, 30, 3);

            spells.make_splash_melee_damage(table_id, player, 101, 2, 1, 2, 0.4f, 0.5f, 30);
        }

        //spell 105 power attack
        public static async void power_attack(string table_id, string me)
        {
            float casting_time = 1f;
            string check_cond_id = functions.get_symb_for_IDs();
            Players player = functions.GetPlayerData(table_id, me);
            player.start_spell_in_process();
            functions.turn_to_enemy(me, table_id, 0.1f, 2, -15, 2);
            string x;

            for (float i = casting_time; i > 0; i -= 0.2f)
            {
                functions.turn_to_enemy(me, table_id, 0.1f, 2, -15, 2);

                if (!player.is_casting_failed())
                {
                    player.conditions.TryRemove(check_cond_id, out x);
                    player.conditions.TryAdd(check_cond_id, $":ca-105-{i.ToString("f1").Replace(',', '.')},");
                }
                else
                {
                    player.remove_condition_in_player( check_cond_id);
                    functions.inform_of_cancel_casting(me, table_id, 105);
                    player.stop_spell_in_process();
                    return;
                }
                await Task.Delay(200);
            }

            player.remove_condition_in_player(check_cond_id);
            player.stop_spell_in_process();

            float new_x = player.position_x;
            float new_z = player.position_z;

            spells.make_splash_melee_damage(table_id, me, 105, 11, 1, 2, 0.3f, 0.5f, 0);
            await Task.Delay(300);
            List<Players> all_players = functions.get_all_nearest_enemy_inmelee(me, table_id, 0.5f, 0);
            if (all_players.Count > 0)
            {
                for (int i = 0; i < all_players.Count; i++)
                {
                    if (!spells.if_resisted_nonmagic(table_id, me, all_players[i].player_id))
                    {
                        spells.fall_down_get_app(table_id, all_players[i].player_id, 0.1f);
                        //spells.pooling(table_id, new_x, new_z, all_players[i].player_id, -15, 2);
                        spells.pooling_ver2(table_id, me, all_players[i].player_id, 15, 2);
                    }
                }
            }
        }

    }
}
