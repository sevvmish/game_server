using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_server
{
    class wizard
    {

        //spell beacon 207
        public static async void beacon(string table_id, string mee)
        {
            Players player = functions.GetPlayerData(table_id, mee);
            string ID_for_beacon = functions.get_symb_for_IDs();

            if (!player.is_cond_here_by_type_and_spell("cs=207"))
            {
                player.conditions.TryAdd(ID_for_beacon, $":cs=207={player.position_x.ToString("f1").Replace(',', '.')}={player.position_z.ToString("f1").Replace(',', '.')},");
            } 
            else
            {
                string [] data = player.get_bulkconditiondata_by_anything_in_bulk("cs=207").Split('=');
                //player.position_x = float.Parse(data[2].Replace('.', ','));
                player.position_x = float.Parse(data[2], CultureInfo.InvariantCulture);

                data[3] = data[3].Substring(0, data[3].Length - 1);
                //player.position_z = float.Parse(data[3].Replace('.', ','));
                player.position_z = float.Parse(data[3], CultureInfo.InvariantCulture);

                player.CastEndCS(player.position_x, player.position_z, player.get_condID_by_anything_in_bulk("cs=207"), 207);                
            }

            //player.conditions.TryAdd(check_cond_id2, $":cs=51={magic_data[0].ToString("f1").Replace(',', '.')}={magic_data[2].ToString("f1").Replace(',', '.')},");
        }


        //spell 206 void zone
        public static async void void_zone(string table_id, string mee, float how_long, float groth_koeff, float radius)
        {
            float distance = 10f;
            Players player = functions.GetPlayerData(table_id, mee);
            functions.turn_to_enemy(mee, table_id, 0.1f, distance, 0, distance);
            Players aim = functions.get_one_nearest_enemy_inmelee(mee, table_id, distance, 0, false);
            float coord_x, coord_z;
            if (aim==null)
            {
                coord_x = player.position_x;
                coord_z = player.position_z;
            } else
            {
                coord_x = aim.position_x;
                coord_z = aim.position_z;
            }

            string ID_for_cs = functions.get_symb_for_IDs();
                        
            player.conditions.TryAdd(ID_for_cs, $":cs=206={coord_x.ToString("f1").Replace(',', '.')}={coord_z.ToString("f1").Replace(',', '.')},");

            float koef = 1;

            for (float u = how_long; u > 0; u-=0.25f)
            {
                List<Players> aims = functions.get_all_nearest_enemy_inradius(coord_x, coord_z, mee, table_id, radius);

                if (aims.Count>0)
                {
                    //Console.WriteLine(player.spell_power / (4f * how_long) * koef / player.spell_power);
                    for (int i = 0; i < aims.Count; i++)
                    {
                        spells.make_direct_magic_damage_exact_enemy(table_id, mee, aims[i].player_id, 206, 0, (player.spell_power/(4f*how_long) * koef / player.spell_power ), 1.5f, TypeOfMagic.other);
                    }
                }

                await Task.Delay(250);
                koef = koef + groth_koeff;

            }

            player.CastEndCS(coord_x, coord_z, ID_for_cs, 206);
        }



        //spell 205 curse of casting
        public static async void cast_curse_of_casting(string table_id, string mee, float how_long, float energy_cost)
        {
            float casting_time = 2;
            float distance = 5;
            
            string check_cond_id = functions.get_symb_for_IDs();
            

            int spell_id = 205;
            Players player = functions.GetPlayerData(table_id, mee);
            player.start_spell_in_process();
            float cast_time = casting_time * (100 - player.cast_speed) / 100;

            for (float i = cast_time; i > 0; i -= 0.2f)
            {
                functions.turn_to_enemy(mee, table_id, 0.1f, distance, 0, distance);
                if (!player.is_casting_failed())
                {
                    player.animation_id = 3;
                    //string x;
                    player.set_condition("ca", 205, check_cond_id, i);
                    //player.conditions.TryRemove(check_cond_id, out x);
                    //player.conditions.TryAdd(check_cond_id, $":ca-{spell_id}{i.ToString("f1").Replace(',', '.')},");
                }
                else
                {
                    player.remove_condition_in_player(check_cond_id);
                    functions.inform_of_cancel_casting(mee, table_id, 205);
                    player.reset_animation_for_one();
                    player.stop_spell_in_process();
                    return;
                }

                await Task.Delay(200);
            }

            player.remove_condition_in_player(check_cond_id);
            player.stop_spell_in_process();
            player.reset_animation_for_one();

            

            Players enemy = functions.get_one_nearest_enemy_inmelee(mee, table_id, distance, -10, false);
            
            if (enemy!=null)
            {
                player.minus_energy(energy_cost);
                curse_of_casting(table_id, mee, enemy.player_id, how_long);
            }
            

        }


        public static async void curse_of_casting(string table_id, string me, string aim, float how_long)
        {

            if (!spells.isOKforMagicConditionImposing(table_id, me, aim, 205)) return;
            await Task.Delay(250);

            float sensitivity = 1f;
            Players aim_player = functions.GetPlayerData(table_id, aim);
            float energy_level = aim_player.energy;
            int stacks = 0;
            string cond_id = functions.get_symb_for_IDs();

            for (float i = how_long; i > 0; i-=0.1f)
            {
                if (Math.Round(energy_level, 0) != Math.Round(aim_player.energy, 0)) {
                    if ((energy_level - aim_player.energy) > sensitivity)
                    {
                        stacks++;
                        aim_player.set_condition("co", 205, cond_id, i, stacks);
                        spells.make_direct_magic_damage_exact_enemy(table_id, me, aim, 205, 0, stacks/5f, 2, TypeOfMagic.other);
                    } 
                    

                    energy_level = aim_player.energy;
                } else
                {
                    aim_player.set_condition("co", 205, cond_id, i);
                }

                if (aim_player.is_stop_all_condition_by_checking_index(205))
                {
                    break;
                }
                await Task.Delay(100);
            }

            aim_player.remove_condition_in_player(cond_id);

        }


            //spell 204
        public static async void auto_heal(string table_id, string mee, float how_long, float heal_amount, float energy_cost)
        {
            float casting_time = 1;
            string check_cond_id = functions.get_symb_for_IDs();
            int spell_id = 204;
            Players player = functions.GetPlayerData(table_id, mee);
            float cast_time = casting_time * (100 - player.cast_speed) / 100;
            string x;

            for (float i = casting_time; i > 0; i -= 0.2f)
            {
                if (!player.is_casting_failed())
                {
                    player.animation_id = 3;

                    player.conditions.TryRemove(check_cond_id, out x);
                    player.conditions.TryAdd(check_cond_id, $":ca-{spell_id}-{i.ToString("f1").Replace(',', '.')},");

                }
                else
                {
                    player.remove_condition_in_player(check_cond_id);
                    functions.inform_of_cancel_casting(mee, table_id, 204);
                    player.reset_animation_for_one();
                    return;
                }

                await Task.Delay(200);
            }

            player.remove_condition_in_player(check_cond_id);
            player.reset_animation_for_one();

            spells.button_cooldowns_cooldown_only(table_id, mee, 204, 4f);

            player.minus_energy(energy_cost);
            string check_cond_id2 = functions.get_symb_for_IDs();
            List<string> ids = new List<string>();
            Dictionary<string, string> conds = new Dictionary<string, string>();

            for (float i = how_long; i > 0; i -= 0.05f)
            {
                player.conditions.TryRemove(check_cond_id2, out x);
                player.conditions.TryAdd(check_cond_id2, $":co-204-{i.ToString("f1").Replace(',', '.')},");

                if (player.is_cond_here_by_type_and_spell("dt") && !ids.Contains(player.get_condID_by_anything_in_bulk("dt")))
                {
                    ids.Add(player.get_condID_by_anything_in_bulk("dt"));
                    string[] arr = player.get_bulkconditiondata_by_anything_in_bulk("dt").Split('-');
                    float damage = float.Parse(arr[1]);
                    if (damage > 0)
                    {
                        spells.healing(table_id, player.get_player_by_id_in_conds(player.get_id_by_type_and_spell("dt"), table_id).player_id, mee, 204, heal_amount, 1, 2);
                    }

                }
                await Task.Delay(50);
            }

            player.remove_condition_in_player(check_cond_id2);

        }

        //spell 203
        public static async void answer_attack_shield(string table_id, string mee, float how_long_shield, float how_long_knock)
        {
            string check_cond_id2 = functions.get_symb_for_IDs();
            Players player = functions.GetPlayerData(table_id, mee);
            player.is_immune_to_magic = true;
            player.is_immune_to_melee = true;

            for (float i = how_long_shield; i > 0; i -= 0.1f)
            {
                if (player.is_cond_here_by_type_and_spell("dt"))
                {
                    string[] arr = player.get_bulkconditiondata_by_anything_in_bulk("dt").Split('-');
                    Players attacker = player.get_player_by_id_in_conds(player.get_condID_by_anything_in_bulk("dt"), table_id);
                    spells.fall_down_get_app(table_id, attacker.player_id, how_long_knock);
                    if (player.data_when_immune_magic != null && player.data_when_immune_magic != "")
                    {
                        string[] data_off = player.data_when_immune_magic.Split('=');
                        spells.direct_impose_damage(table_id, mee, attacker.player_id, 203, float.Parse(data_off[0]), 1);
                        player.is_immune_to_magic = false;
                        player.data_when_immune_magic = null;
                    }
                    else if (player.data_when_immune_melee != null && player.data_when_immune_melee != "")
                    {
                        string[] data_off = player.data_when_immune_melee.Split('=');
                        spells.direct_impose_damage(table_id, mee, attacker.player_id, 203, float.Parse(data_off[0]), 1);
                        player.is_immune_to_melee = false;
                        player.data_when_immune_melee = null;
                    }

                    player.remove_condition_in_player(check_cond_id2);
                    break;

                }
                else
                {
                    string x;
                    player.conditions.TryRemove(check_cond_id2, out x);
                    player.conditions.TryAdd(check_cond_id2, $":co-203-{i.ToString("f1").Replace(',', '.')},");
                }
                await Task.Delay(100);
            }

            player.is_immune_to_magic = false;
            player.is_immune_to_melee = false;
            player.remove_condition_in_player(check_cond_id2);
        }



        //spell 202
        public static async void black_hole(string table_id, string mee, float how_long)
        {
            float distance = 3;
            float init_force = 20;
            float init_speed = 1;
            string x;
            string check_cond_id = functions.get_symb_for_IDs();
            string check_cond_id2 = functions.get_symb_for_IDs();
            Players player = functions.GetPlayerData(table_id, mee);
            float new_x = player.position_x;
            float new_z = player.position_z;
            player.conditions.TryAdd(check_cond_id, $":cs=202={new_x.ToString("f1").Replace(',', '.')}={new_z.ToString("f1").Replace(',', '.')},");
            player.remove_condition_in_player(check_cond_id);
            List<Players> result = new List<Players>();

            for (float i = how_long; i > 0; i -= 0.1f)
            {
                result = functions.get_all_nearest_enemy_inradius(new_x, new_z, mee, table_id, distance);

                if (result.Count > 0)
                {

                    for (int u = 0; u < result.Count; u++)
                    {

                        result[u].position_x = new_x;
                        result[u].position_z = new_z;
                        
                        result[u].conditions.TryRemove(check_cond_id2, out x);
                        result[u].conditions.TryAdd(check_cond_id2, $":co-202-{i.ToString("f1").Replace(',', '.')},");
                        init_force = 20;
                        init_speed = 1;

                        for (int uu = 0; uu < 2; uu++)
                        {
                            float checker = functions.vector3_distance_unity(new_x, 0, new_z, result[u].position_x, 0, result[u].position_z);
                            if ((checker / distance) <= 0.4)
                            {
                                float koef = checker / (distance * 0.4f);
                                init_force = init_force * koef;
                                init_speed = init_speed * koef;
                            }
                            else
                            {
                                init_force = 20;
                                init_speed = 1;
                            }
                            spells.pooling(table_id, new_x, new_z, result[u].player_id, init_force, init_speed);
                        }
                    }
                }

                await Task.Delay(100);
            }

            player.conditions.TryRemove(check_cond_id, out x);
            player.conditions.TryAdd(check_cond_id, $":cs=202=999=999,");

            for (int i = 0; i < result.Count; i++)
            {
                result[i].remove_condition_in_player(check_cond_id2);
            }

            player.remove_condition_in_player(check_cond_id);
            
        }






        //spell 201
        public static async void death_beam(string table_id, string mee, float energy_cost, float how_long, float damage)
        {
            Players player = functions.GetPlayerData(table_id, mee);
            player.is_spell_in_process = true;
            float casting_time = 1;
            float dist = 7;
            string xxx;

            functions.turn_to_enemy(mee, table_id, 0.1f, dist, -15, dist);
            string check_cond_id = functions.get_symb_for_IDs();
            int spell_id = 201;
            float cast_time = casting_time * (100 - player.cast_speed) / 100;

            for (float i = cast_time; i > 0; i -= 0.2f)
            {
                functions.turn_to_enemy(mee, table_id, 0.1f, dist - 1, -15, dist + 1);
                if (!player.is_casting_failed())
                {
                    player.animation_id = 3;
                    
                    player.conditions.TryRemove(check_cond_id, out xxx);
                    player.conditions.TryAdd(check_cond_id, $":ca-{spell_id}-{i.ToString("f1").Replace(',', '.')},");

                }
                else
                {
                    player.remove_condition_in_player(check_cond_id);
                    functions.inform_of_cancel_casting(mee, table_id, 201);
                    player.reset_animation_for_one();
                    player.is_spell_in_process = false;
                    return;
                }
                await Task.Delay(200);
            }

            player.remove_condition_in_player(check_cond_id);
            player.is_spell_in_process = false;
            player.minus_energy(energy_cost);
            player.reset_animation_for_one();

            float dam = damage / (10f * how_long);
            float power = 1 / (10f * how_long);
            string check_cond_id1 = functions.get_symb_for_IDs();
            

            for (float u = how_long; u > 0; u-=0.1f)
            {
                if (player.is_casting_failed())
                {
                    player.conditions.TryRemove(check_cond_id1, out xxx);
                    player.conditions.TryAdd(check_cond_id1, $":co-201-0,");
                    functions.inform_of_cancel_casting(mee, table_id, 201);
                    player.reset_animation_for_one();
                    player.remove_condition_in_player( check_cond_id1);
                    //spells.remove_condition_in_player(table_id, mee, check_cond_id);
                    return;
                }
                functions.turn_to_enemy(mee, table_id, 0.1f, dist - 1, -15, dist + 1);
                player.animation_id = 13;
                float default_player_x = player.position_x;
                float default_player_z = player.position_z;
                float default_player_rot_y = player.rotation_y;
                float[] x = new float[] { 0, 0 };
                functions.projection(ref x, default_player_x, default_player_z, default_player_rot_y, dist);
                float new_x = x[0];
                float new_z = x[1];
                player.conditions.TryRemove(check_cond_id1, out xxx);                
                player.conditions.TryAdd(check_cond_id1, $":co-201-{u.ToString("f1").Replace(',', '.')},");

                //player.conditions.TryRemove(check_cond_id, out xxx);
                //player.conditions.TryAdd(check_cond_id, $":ca-201-{u.ToString("f1").Replace(',', '.')},");

                //spells.remove_condition_in_player(table_id, mee, check_cond_id1);
                List<Players> all_enemies = new List<Players>();

                for (float i = 0; i < (dist - 0.5f); i += 0.5f)
                {
                    float[] xx = new float[] { 0, 0 };
                    functions.lerp(ref xx, default_player_x, default_player_z, x[0], x[1], default_player_rot_y, (i / (dist - 1.01f)));
                    new_x = xx[0];
                    new_z = xx[1];

                    List<Players> result = functions.get_all_nearest_enemy_inradius(new_x, new_z, mee, table_id, 0.4f);
                    if (result.Count > 0)
                    {
                        for (int uu = 0; uu < result.Count; uu++)
                        {
                            
                            if (!all_enemies.Contains(result[uu]))
                            {
                                all_enemies.Add(result[uu]);
                            }

                        }
                    }

                }                

                if (all_enemies.Count > 0)
                {
                    for (int uu = 0; uu < all_enemies.Count; uu++)
                    {
                        spells.make_direct_magic_damage_exact_enemy(table_id, mee, all_enemies[uu].player_id, 201, dam, power, 2, TypeOfMagic.fire);
                    }
                }
                all_enemies.Clear();

                await Task.Delay(100);

            }

            player.reset_animation_for_one();
            player.remove_condition_in_player(check_cond_id1);
            //spells.remove_condition_in_player(table_id, mee, check_cond_id);

        }



    }
}
