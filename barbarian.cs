using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_server
{
    class barbarian
    {
        //block prep 104
        public static async void block_prep(string table_id, string me, float block_time)
        {
            string check_cond_id = functions.get_symb_for_IDs();
            string check_immob_id = functions.get_symb_for_IDs();
            Players player = functions.GetPlayerData(table_id, me);
            player.shield_block += 50;
            player.start_spell_in_process();

            for (float i = block_time; i > 0; i -= 0.25f)
            {
                if (!player.is_casting_stopped_by_spells())
                {
                    player.make_immob(check_immob_id, i);
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
            spells.reset_animation_for_one(table_id, me);
            spells.remove_condition_in_player(table_id, me, check_cond_id);
            spells.remove_condition_in_player(table_id, me, check_immob_id);
            player.stop_spell_in_process();
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
                    spells.reset_animation_for_one(table_id, me);
                    spells.remove_condition_in_player(table_id, me, check_cond_id);
                    player1.stop_spell_in_process();
                    player1.is_reset_any_button = false;
                    return;
                }
                float distance = functions.vector3_distance_unity(player1.position_x, 0, player1.position_z, start_x, 0, start_z);
                if (distance >= max_distance)
                {
                    spells.reset_animation_for_one(table_id, me);
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
            spells.reset_animation_for_one(table_id, me);
            spells.remove_condition_in_player(table_id, me, check_cond_id);
            spells.remove_condition_in_player(table_id, me, check_cond_strike_id);
            
            await Task.Delay(1000);
            player1.is_reset_any_button = false;
            for (int u = 0; u < enemies.Count; u++)
            {                
                spells.remove_condition_in_player(table_id, enemies[u].player_id, IDs_for_slow[u]);
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
            spells.remove_condition_in_player(table_id, me, check_cond_id);
            player.speed /= 0.5f;
            spells.reset_animation_for_one(table_id, me);
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
                    spells.remove_condition_in_player(table_id, me, check_cond_id);
                    functions.inform_of_cancel_casting(me, table_id, 105);
                    player.stop_spell_in_process();
                    return;
                }
                await Task.Delay(200);
            }

            spells.remove_condition_in_player(table_id, me, check_cond_id);
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
