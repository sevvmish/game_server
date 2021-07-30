using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_server
{
    class rogue
    {
        //backstab 152
        public static async void backstab(string table_id, string pl, string enem)
        {
            Players player = functions.GetPlayerData(table_id, pl);
            Players enemy = functions.GetPlayerData(table_id, enem);
            
            player.start_spell_in_process();
            player.is_reset_any_button = true;

            player.animation_id = 11;
            spells.make_direct_melee_damage(table_id, pl, 152, 0, 5, 3, 0.2f); //0.2f
            string cond = functions.get_symb_for_IDs();
            functions.set_condition("co", 152, cond, table_id, pl, 0);

            
            
            spells.reset_animation_for_one(table_id, pl);
            //spells.pooling(table_id, player.position_x, player.position_y, pl, 10, 2);
            spells.pooling_ver2(table_id, pl, enem, 10, 2);

            for (float i = 0.8f; i > 0; i-=0.1f)
            {
                if (player.is_casting_failed())
                {
                    break;
                }

                await Task.Delay(100);
            }

            
            spells.remove_condition_in_player(table_id, pl, cond);
            player.stop_spell_in_process();
            player.is_reset_any_button = false;
        }


            //step to the back spell 155
        public static async void step(string table_id, string pl, Players enemy)
        {
            Players player = functions.GetPlayerData(table_id, pl);
            string check_cond_strike_id = functions.get_symb_for_IDs();
            player.conditions.TryAdd(check_cond_strike_id, $":cs=155={player.position_x.ToString("f1").Replace(',', '.')}={player.position_z.ToString("f1").Replace(',', '.')},");
            float[] res = new float[] { enemy.position_x, 0, enemy.position_z, 0, enemy.rotation_y, 0 };
            functions.mover(ref res, 0, -6, 2);
            player.position_x = res[0];
            player.position_z = res[2];
            spells.remove_condition_in_player(table_id, pl, check_cond_strike_id);
            await Task.Delay(100);

            check_cond_strike_id = functions.get_symb_for_IDs();
            player.conditions.TryAdd(check_cond_strike_id, $":cs=155={player.position_x.ToString("f1").Replace(',', '.')}={player.position_z.ToString("f1").Replace(',', '.')},");
            spells.remove_condition_in_player(table_id, pl, check_cond_strike_id);
            bool isSlowed = false;
            if (!enemy.is_immune_to_movement_imparing)
            {
                isSlowed = true;
            }
            
            if (isSlowed) enemy.speed *= 0.4f;

            player.rotation_y = enemy.rotation_y;
            float new_enemy_angle = enemy.rotation_y;
            check_cond_strike_id = functions.get_symb_for_IDs();

            for (float i = 2; i > 0; i -= 0.1f)
            {
                string x;
                enemy.conditions.TryRemove(check_cond_strike_id, out x);
                
                enemy.rotation_y = new_enemy_angle;
                enemy.conditions.TryAdd(check_cond_strike_id, $":co-155-{i.ToString("f1").Replace(',', '.')},");
                if (enemy.is_stop_all_condition_by_checking_index(155)) 
                {
                    break;
                }
                await Task.Delay(100);
            }
            spells.remove_condition_in_player(table_id, enemy.player_id, check_cond_strike_id);
            if (isSlowed) enemy.speed /= 0.4f;

        }




        //spell 154 butchery
        public static async void butchery(string table_id, string mee, float attack_time)
        {
            functions.turn_to_enemy(mee, table_id, 0.1f, 0, 0, 3);
            string check_cond_id = functions.get_symb_for_IDs();
            string check_immob_id = functions.get_symb_for_IDs();
            Players player = functions.GetPlayerData(table_id, mee);
            player.is_spell_in_process = true;

            for (float i = attack_time; i > 0; i -= 0.201f)
            {
                if (!player.is_casting_failed())
                {
                    player.make_immob(check_immob_id, i);
                    functions.turn_to_enemy(mee, table_id, 0.1f, 0, 0, 2.3f);
                    string x;
                    player.conditions.TryRemove(check_cond_id, out x);
                    //check_cond_id = functions.get_random_set_of_symb(4);
                    player.conditions.TryAdd(check_cond_id, $":co-154-{i.ToString("f1").Replace(',', '.')},");
                    player.animation_id = 15;
                    spells.make_splash_melee_damage(table_id, mee, 154, 0, 0.5f, 2, 0, 0, 0);
                }
                else
                {
                    functions.inform_of_cancel_casting(mee, table_id, 154);
                    break;
                }
                await Task.Delay(200);
            }
            spells.reset_animation_for_one(table_id, mee);
            spells.remove_condition_in_player(table_id, mee, check_cond_id);
            spells.remove_condition_in_player(table_id, mee, check_immob_id);
            player.is_spell_in_process = false;
        }




        //invizibility 157
        public static void from_inviz_to_viz(string table_id, string player_name)
        {
            Players player = functions.GetPlayerData(table_id, player_name);
            string check_cond_strike_id = functions.get_symb_for_IDs();
            //player.conditions.TryAdd(check_cond_strike_id, $":cs=157={player.position_x.ToString("f1").Replace(',', '.')}={player.position_z.ToString("f1").Replace(',', '.')},");
            player.conditions.TryAdd(check_cond_strike_id, $":co-157-0,");
            spells.remove_condition_in_player(table_id, player_name, player.get_id_by_type_and_spell("co-153"));
            spells.remove_condition_in_player(table_id, player_name, check_cond_strike_id);
            
        }

        //invizibility 153
        public static async void invizibility(string table_id, string me)
        {   
            Players player = functions.GetPlayerData(table_id, me);
            string check_cond_strike_id = functions.get_symb_for_IDs();
            string check_cond = functions.get_symb_for_IDs();
            player.conditions.TryAdd(check_cond_strike_id, $":cs=153={player.position_x.ToString("f1").Replace(',', '.')}={player.position_z.ToString("f1").Replace(',', '.')},");
            player.conditions.TryAdd(check_cond, $":co-153-0,");
            await Task.Delay(20);
            spells.remove_condition_in_player(table_id, me, check_cond_strike_id);
            player.is_invisible = true;

        }



        //fast strike 151
        public static async void fast_strike(string table_id, string me)
        {
            functions.turn_to_enemy(me, table_id, 0.1f, 0, 0, 3);
            await Task.Delay(20);
            spells.make_direct_melee_damage(table_id, me, 151, 2, 1, 2, 0.1f);
            await Task.Delay(200);
            spells.make_direct_melee_damage(table_id, me, 151, 0, 1, 2, 0.1f);
        }


        //pistol shot 156
        public static async void pistol_shot(string table_id, string me)
        {
            float shot_distance = 15f;
            float time_for_aiming = 2f;

            string check_cond_id = functions.get_symb_for_IDs();
            string check_immob_id = functions.get_symb_for_IDs();
            string check_cond_strike_id = functions.get_symb_for_IDs();
            Players player = functions.GetPlayerData(table_id, me);            
            player.start_spell_in_process();
            bool isShooted = false;
            float[] bullet_pos_for_assess = new float[2];
            string x;

            for (float i = time_for_aiming; i > 0; i -= 0.1f)
            {
                if (!player.is_casting_stopped_by_spells())
                {
                    player.make_immob(check_immob_id, i);
                    
                    player.conditions.TryRemove(check_cond_id, out x);
                    player.conditions.TryAdd(check_cond_id, $":co-156-{i.ToString("f1").Replace(',', '.')},");

                    if (i > 0.5f)
                    {
                        functions.turn_to_enemy(me, table_id, 0.1f, shot_distance, -10, shot_distance);
                        player.animation_id = 20; //raise gun
                    } 
                    else
                    {
                        player.animation_id = 21; //make shot

                        if (!isShooted)
                        {
                            isShooted = true;
                            float pos_x = player.position_x;
                            float pos_z = player.position_z;
                            float rot_y = player.rotation_y;

                            List<Players> candidates = functions.GetAllPlayersInList(table_id);

                            for (int c = 0; c < candidates.Count; c++)
                            {
                                if ( candidates[c].player_id==me || candidates[c].team_id==player.team_id) //candidates[c].is_invisible ||
                                {
                                    candidates.Remove(candidates[c]);
                                }
                            }
                                                        

                            bool isFound = false;
                            for (float d = 0; d < shot_distance; d+=0.2f)
                            {
                                functions.projection(ref bullet_pos_for_assess, pos_x, pos_z, rot_y, d);
                                                                
                                for (int c = 0; c < candidates.Count; c++)
                                {
                                    if (functions.vector3_distance_unity(bullet_pos_for_assess[0], 0, bullet_pos_for_assess[1], candidates[c].position_x, 0, candidates[c].position_z)<0.25f)
                                    {
                                        spells.melee_damage(table_id, me, candidates[c].player_id, 156, 5, 2);
                                        
                                        player.conditions.TryAdd(check_cond_strike_id, $":cs=156={candidates[c].position_x.ToString("f1").Replace(',', '.')}={candidates[c].position_z.ToString("f1").Replace(',', '.')},");
                                        spells.remove_condition_in_player(table_id, me, check_cond_strike_id);

                                        isFound = true;
                                        break;
                                    }
                                }

                                if (isFound)
                                {
                                    break;
                                }
                            }

                            if (!isFound)
                            {
                                player.conditions.TryAdd(check_cond_strike_id, $":cs=156={bullet_pos_for_assess[0].ToString("f1").Replace(',', '.')}={bullet_pos_for_assess[1].ToString("f1").Replace(',', '.')},");
                                spells.remove_condition_in_player(table_id, me, check_cond_strike_id);
                            } else
                            {
                                break;
                            }

                        }
                        
                    }
                }
                else
                {
                    functions.inform_of_cancel_casting(me, table_id, 156);
                    break;
                }

                await Task.Delay(100);
            }

            await Task.Delay(400);

            spells.remove_condition_in_player(table_id, me, check_cond_id);
            
            spells.remove_condition_in_player(table_id, me, check_immob_id);
            player.stop_spell_in_process();

            
            spells.reset_animation_for_one(table_id, me);
        }

    }
}
