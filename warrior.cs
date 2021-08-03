using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace game_server
{
    class warrior
    {

        //solid armor 10
        public static async void solid_armor(string table_id, string me, float how_long)
        {
            Players Player = functions.GetPlayerData(table_id, me);
            float old_armor = Player.armor;
            Player.armor = 999;
            string id_condition = functions.get_symb_for_IDs();
            Player.add_stop_to_spec_conditions(0);

            for (float i = how_long; i > 0; i-=0.1f)
            {                
                Player.set_condition("co", 10, id_condition, i);

                await Task.Delay(100);
            }

            Player.remove_stop_to_spec_conditions(0);
            Player.armor = old_armor;
            spells.remove_condition_in_player(table_id, me, id_condition);            
        }



        public static void simple_hit(string table_id, string me)
        {
            
            functions.turn_to_enemy(me, table_id, 10f, 0, 0, 3);
            spells.make_direct_melee_damage(table_id, me, 1, 2, 1, 2, 0);
        }

        public static async void make_hp_boost_spell_3(float time_ticks, float boost_koef, float regen_koef, float distance, string table_id, string mee)
        {
            List<Players> allgamers = functions.GetAllPlayersInList(table_id);
            List<Players> all_needed_players = new List<Players>();
            Players me = functions.GetPlayerData(table_id, mee);

            me.animation_id = 23;
            spells.reset_animation_for_one(table_id, mee);

            for (int i = 0; i < allgamers.Count; i++)
            {
                if (allgamers[i].team_id == me.team_id && !allgamers[i].isDead && 
                    functions.vector3_distance_unity(me.position_x, 0, me.position_z, allgamers[i].position_x, 0, allgamers[i].position_z) < distance)
                {
                    all_needed_players.Add(allgamers[i]);
                }
            }

            for (int u = 0; u < all_needed_players.Count; u++)
            {
                //Players pl = all_needed_players[u];
                string[] my_health = all_needed_players[u].health_pool.Split('=');
                all_needed_players[u].health_pool = (float.Parse(my_health[0]) * boost_koef).ToString() + "=" + (float.Parse(my_health[1]) * boost_koef).ToString();
                all_needed_players[u].health_regen += regen_koef;
            }

            List<string> conds_ids = new List<string>();
            for (int ii = 0; ii < all_needed_players.Count; ii++)
            {
                conds_ids.Add(functions.get_symb_for_IDs());
            }

            for (float i = time_ticks; i > 0; i-=0.250f)
            {
                for (int u = 0; u < all_needed_players.Count; u++)
                {
                    all_needed_players[u].set_condition("co", 3, conds_ids[u], i);
                    if (all_needed_players[u].is_stop_all_condition_by_checking_index(3))
                    {
                        break;
                    }
                }
                await Task.Delay(250);
            }

            for (int u = 0; u < all_needed_players.Count; u++)
            {
                spells.remove_condition_in_player(table_id, all_needed_players[u].player_id, conds_ids[u]);
                //Players pl = all_needed_players[u];
                string[] my_health = all_needed_players[u].health_pool.Split('=');
                all_needed_players[u].health_pool = (float.Parse(my_health[0]) / boost_koef).ToString() + "=" + (float.Parse(my_health[1]) / boost_koef).ToString();
                all_needed_players[u].health_regen -= regen_koef;
            }

        }


        //spell 2 BLEEDING===================
        public static async void bleeding(float time_ticks, string table_id, string me, string all_players, int spell_number, float att_pow_koef)
        {
            Players player = functions.GetPlayerData(table_id, me);
            player.animation_id = 2;
            spells.reset_animation_for_one(table_id, me);
            functions.turn_face_to_face(me, all_players, table_id);

            //await Task.Delay(100);
            string id_condition = functions.get_symb_for_IDs();
            Players enemy = functions.GetPlayerData(table_id, all_players);
            enemy.conditions.TryAdd(id_condition, $":co-{spell_number}-{time_ticks},");
            string x;
            for (float i = time_ticks; i > 0; i-=0.250f)
            {
                //enemy.conditions.TryRemove(id_condition, out x);
                //enemy.conditions.TryAdd(id_condition, $":co-{spell_number}-{i},");
                enemy.set_condition("co", spell_number, id_condition, i);
                if (enemy.is_stop_all_condition_by_checking_index(2))
                {
                    break;
                }



                if (Math.Round(i, 2) == Math.Truncate(i) && Math.Round(i, 2) % 2 != 0)
                {
                    //float prev_armor = enemy.armor;
                    float prev_dodge = enemy.dodge;
                    float prev_shieldbl = enemy.shield_block;

                    //enemy.armor = 0;
                    enemy.dodge = 0;
                    enemy.shield_block = 0;

                    

                    spells.melee_damage(table_id, me, all_players, spell_number, att_pow_koef, 2);

                    //enemy.armor = prev_armor;
                    enemy.dodge = prev_dodge;
                    enemy.shield_block = prev_shieldbl;
                }
               

                await Task.Delay(250);
            }

            //enemy.conditions.TryRemove(id_condition, out x);
            spells.remove_condition_in_player(table_id, all_players, id_condition);
        }


        //shield ON
        public static async void shield_on(string me, string table_id, float shield_on_time)
        {
            functions.turn_to_enemy(me, table_id, 0.1f, 20, 15, 20);
            Players pl = functions.GetPlayerData(table_id, me);
            pl.animation_id = 10;
            float current_shield_block = pl.shield_block;
            pl.start_spell_in_process();
            pl.is_reset_movement_not_rotation = true;

            pl.shield_block = 100;
            //pl.speed *= 0.3f;            

            string check_cond_id = functions.get_symb_for_IDs();

            for (float i = shield_on_time; i > 0; i-=0.1f)
            {
                /*
                if (pl.vertical_touch>1)
                {
                    pl.animation_id = 101;
                }
                else if(pl.vertical_touch < -1)
                {
                    pl.animation_id = 102;
                } 
                else if (pl.vertical_touch == 0)
                {
                    pl.animation_id = 10;
                }
                */
                pl.animation_id = 10;
                pl.shield_block = 100;
                pl.set_condition("co", 5, check_cond_id, i);
                if (pl.is_stop_all_condition_by_checking_index(5))
                {
                    break;
                }
                await Task.Delay(100);
            }
            spells.remove_condition_in_player(table_id, me, check_cond_id);
            spells.reset_animation_for_one(table_id, me);
            pl.shield_block = current_shield_block;
            pl.stop_spell_in_process();
            pl.is_reset_movement_not_rotation = false;
            //pl.speed /= 0.3f;

        }



        public static async void shield_bash(string me, string table_id, float cast_time, float stun_time, float energy_cost)
        {
            Players player = functions.GetPlayerData(table_id, me);
            //player.set_hiddenconds("sip", "1111");
            //player.is_spell_in_process = true;
            string check_cond_id = functions.get_symb_for_IDs();

            player.start_spell_in_process();

            for (float i = cast_time; i > 0; i -= 0.1f)
            {
                if (!player.is_casting_failed())
                {
                    functions.turn_to_enemy(me, table_id, 0, 1, 20, 2);
                    player.animation_id = 3;
                    string x;
                    player.conditions.TryRemove(check_cond_id, out x);
                    player.conditions.TryAdd(check_cond_id, $":ca-4-{i.ToString("f1").Replace(',', '.')},");
                }
                else
                {
                    functions.inform_of_cancel_casting(me, table_id, 4);
                    spells.remove_condition_in_player(table_id, me, check_cond_id);
                    spells.reset_animation_for_one(table_id, me);
                    //player.hidden_conds.Remove("1111");
                    //player.is_spell_in_process = false;
                    player.stop_spell_in_process();
                    return;
                }
                await Task.Delay(100);
            }

            player.minus_energy(energy_cost);
            //player.hidden_conds.Remove("1111");
            //player.is_spell_in_process = false;
            player.stop_spell_in_process();
            spells.remove_condition_in_player(table_id, me, check_cond_id);

            player.animation_id = 9;
            spells.reset_animation_for_one(table_id, me);

            List<Players> all_needed_players = new List<Players>();
            all_needed_players = functions.get_all_nearest_enemy_inmelee(me, table_id, 1, 20);
            if (all_needed_players != null)
            {
                List<string> conds_ids = new List<string>();
                List<string> conds_ids_stun = new List<string>();
                for (int ii = 0; ii < all_needed_players.Count; ii++)
                {
                    conds_ids.Add(functions.get_symb_for_IDs());
                    conds_ids_stun.Add(functions.get_symb_for_IDs());
                    if (ii == 0)
                    {
                        functions.turn_face_to_face(me, all_needed_players[ii].player_id, table_id);
                    }
                    spells.make_splash_melee_damage(table_id, me, 4, 0, 3, 2, 0, 0, 20);
                }
                for (float i = stun_time; i > 0; i -= 0.1f)
                {
                    for (int u = 0; u < all_needed_players.Count; u++)
                    {
                        all_needed_players[u].make_stun(conds_ids_stun[u], i);
                        all_needed_players[u].set_condition("co", 4, conds_ids[u], i);
                    }
                    await Task.Delay(100);
                }
                for (int u = 0; u < all_needed_players.Count; u++)
                {
                    //Players enemyy = functions.GetPlayerData(table_id, all_needed_players[u].player_id);
                    all_needed_players[u].animation_id = 0;
                }
                for (int ii = 0; ii < all_needed_players.Count; ii++)
                {
                    spells.remove_condition_in_player(table_id, all_needed_players[ii].player_id, conds_ids[ii]);
                    spells.remove_condition_in_player(table_id, all_needed_players[ii].player_id, conds_ids_stun[ii]);
                }
            }
            else
            {
                spells.reset_animation_for_one(table_id, me);
            }

        }


        //spell6
        public static async void series_of_hits(string table_id, string me, float interval)
        {
            Players player = functions.GetPlayerData(table_id, me);
            string id = functions.get_symb_for_IDs();
            player.set_condition("co", 6, id, 0);

            player.speed *= 0.4f;

            spells.make_direct_melee_damage(table_id, me, 6, 2, 1, 2, 0);

            for (float i = 0; i < interval; i += 0.1f)
            {
                if (player.is_casting_stopped_by_spells())
                {
                    spells.remove_condition_in_player(table_id, me, id);
                    player.speed /= 0.4f;
                    return;
                }
                await Task.Delay(100);
            }

            spells.make_splash_melee_damage(table_id, me, 6, 11, 1.5f, 2, 0.1f, 0, 0);

            for (float i = 0; i < interval; i += 0.1f)
            {
                if (player.is_casting_stopped_by_spells())
                {
                    spells.remove_condition_in_player(table_id, me, id);
                    player.speed /= 0.4f;
                    return;
                }
                await Task.Delay(200);
            }

            spells.make_splash_melee_damage(table_id, me, 6, 12, 2, 2, 0.3f, 0, 0);

            spells.remove_condition_in_player(table_id, me, id);
            player.speed /= 0.4f;

        }


        //spell 8 BLEEDING and SLOW===================
        public static async void bleeding_spell8(float time_ticks, string table_id, string me, string all_players, int spell_number, float att_pow_koef)
        {
            Players player = functions.GetPlayerData(table_id, me);
            player.animation_id = 2;
            spells.reset_animation_for_one(table_id, me);
            functions.turn_face_to_face(me, all_players, table_id);

            //await Task.Delay(100);
            string id_condition = functions.get_symb_for_IDs();
            Players enemy = functions.GetPlayerData(table_id, all_players);
            enemy.speed *= 0.4f;
            enemy.conditions.TryAdd(id_condition, $":co-{spell_number}-{time_ticks},");
            string x;

            for (float i = time_ticks; i > 0; i-=0.25f)
            {
                //enemy.conditions.TryRemove(id_condition, out x);
                //enemy.conditions.TryAdd(id_condition, $":co-{spell_number}-{i},");
                enemy.set_condition("co", spell_number, id_condition, i);
                if (enemy.is_stop_all_condition_by_checking_index(2))
                {
                    break;
                }

                if (Math.Round(i, 2) == Math.Truncate(i) && Math.Round(i, 2) % 2 != 0)
                {
                    //float prev_armor = enemy.armor;
                    float prev_dodge = enemy.dodge;
                    float prev_shieldbl = enemy.shield_block;

                    //enemy.armor = 0;
                    enemy.dodge = 0;
                    enemy.shield_block = 0;                    

                    spells.melee_damage(table_id, me, all_players, spell_number, att_pow_koef, 2);

                    //enemy.armor = prev_armor;
                    enemy.dodge = prev_dodge;
                    enemy.shield_block = prev_shieldbl;
                }
                await Task.Delay(250);
            }
            enemy.speed /= 0.4f;
            enemy.conditions.TryRemove(id_condition, out x);
        }

        //spell 9 RAM charge
        public static async void ram(string table_id, string mee)
        {
            Players me = functions.GetPlayerData(table_id, mee);
                        
            float distance = 6.5f; //7 in real
            float start_x = me.position_x;
            float start_z = me.position_z;
            me.is_reset_any_button = true;
            string check_cond_id = functions.get_symb_for_IDs();

            functions.turn_to_enemy(mee, table_id, 0.1f, distance, -15, distance);

            for (float i = 0.5f; i > 0; i -= 0.1f)
            {
                if (!me.is_casting_stopped_by_spells())
                {
                    functions.turn_to_enemy(mee, table_id, 0.1f, distance, -15, distance);
                    float curr_dist = functions.vector3_distance_unity(start_x, 0, start_z, me.position_x, 0, me.position_z);

                    if (curr_dist >= distance)
                    {
                        spells.remove_condition_in_player(table_id, mee, check_cond_id);
                        me.is_reset_any_button = false;
                        spells.reset_animation_for_one(table_id, mee);
                        return;
                    }
                    else
                    {
                        float[] res = new float[] { me.position_x, 0, me.position_z, 0, me.rotation_y, 0 };
                        functions.mover(ref res, 0, 20, 1);
                        me.position_x = res[0];
                        me.position_z = res[2];
                        me.animation_id = 101;
                    }
                    string x;
                    me.conditions.TryRemove(check_cond_id, out x);
                    me.conditions.TryAdd(check_cond_id, $":co-9-{i.ToString("f1").Replace(',', '.')},");
                    Players enemy = functions.get_one_nearest_enemy_inmelee(mee, table_id, -1.5f, -10, true);
                    if (enemy != null)
                    {
                        spells.reset_animation_for_one(table_id, mee);

                        if (!spells.if_resisted_nonmagic(table_id, mee, enemy.player_id))
                        {
                            me.animation_id = 9;
                            await Task.Delay(200);
                            spells.make_direct_melee_damage(table_id, mee, 9, 0, 1, 2, 0);
                            spells.fall_down_get_app(table_id, enemy.player_id, 0.5f);                            
                            spells.remove_condition_in_player(table_id, mee, check_cond_id);
                            spells.reset_animation_for_one(table_id, mee);
                            await Task.Delay(700);
                            me.is_reset_any_button = false;
                            
                            return;
                        }
                        else
                        {
                            spells.remove_condition_in_player(table_id, mee, check_cond_id);
                            me.is_reset_any_button = false;
                            me.animation_id = 0;
                            //spells.reset_animation_for_one(table_id, mee);
                            functions.inform_of_cancel_casting(mee, table_id, 9);
                            break;
                        }
                    }

                }
                else
                {
                    functions.inform_of_cancel_casting(mee, table_id, 9);
                    spells.remove_condition_in_player(table_id, mee, check_cond_id);
                    me.is_reset_any_button = false;
                    spells.reset_animation_for_one(table_id, mee);
                    return;
                }

                await Task.Delay(100);
            }

            spells.remove_condition_in_player(table_id, mee, check_cond_id);
            
            spells.reset_animation_for_one(table_id, mee);
            
            me.is_reset_any_button = false;
        }
    }
}
