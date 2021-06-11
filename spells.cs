using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace game_server
{
    public class spells
    {
        public static float[] CastSpell(int spell_id, string table_id, string player, Players p)
        {
            //Players p = functions.GetPlayerData(table_id, player);
            
            //================spell 0==================================
            if (spell_id == 0)
            {
                return new float[] { 0, 2, 0 };
            }

            //================spell 1 simple melee hit==================================
            if (spell_id == 1)
            {
                float energy_cost = 10;

                if (p.energy >= energy_cost)
                {
                    p.minus_energy(energy_cost);                                        
                    Task.Run(() => warrior.simple_hit(table_id, player));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 0.7f));                    
                    return new float[] { 1, 5, 0.7f };
                }
                else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 1==================================
            
            //================spell 2 melee dot like rend==================================
            if (spell_id == 2) {
                float energy_cost = 20;

                Players result = functions.get_one_nearest_enemy_inmelee(player, table_id, 0.7f, 50, false);
                if (result==null)
                {
                    return new float[] { 0, 3, 0 };
                }

                if (p.energy >= energy_cost)
                {
                    p.minus_energy(energy_cost);
                    Task.Run(() => warrior.bleeding(5, table_id, player, result.player_id, 2, 1));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 5));
                    return new float[] { 1, 6, 5 };
                }
                else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 2==================================

            //================spell 3 HP boost like commanding shout==================================
            if (spell_id == 3) {
                float energy_cost = 10;

                if (p.energy >= energy_cost)
                {
                    p.minus_energy(energy_cost);
                    Task.Run(() => warrior.make_hp_boost_spell_3(10, 1.2f, 5, table_id, player));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 20));
                    return new float[] { 1, 0, 20 };
                }
                else
                {
                    return new float[] { 0, 7, 0 };
                }

            }
            //================spell 3========================

            //================spell 4 shield bash==================================
            if (spell_id == 4) {
                float energy_cost = 25;


                if (p.energy >= energy_cost) {
                                    
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 10));
                    Task.Run(() => warrior.shield_bash(player, table_id, 0.6f, 3, energy_cost));

                    return new float[] { 1, 0, 10 };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }

            }
            //================spell 4==================================
            //================spell 5 shield ON==================================
            if (spell_id == 5) {
                float energy_cost = 20;


                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 20));
                    Task.Run(() => warrior.shield_on(player, table_id, 3));

                    return new float[] { 1, 0, 20 };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }

            }
            //================spell 5==================================

            //================spell 6 series of hits==================================
            if (spell_id == 6)
            {
                float energy_cost = 10;

                if (p.energy >= energy_cost)
                {
                    p.minus_energy(energy_cost);
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 3));
                    Task.Run(() => warrior.series_of_hits(table_id, player, 0.5f));

                    return new float[] { 1, 5, 3 };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 6==================================

            //================spell 7 melee dot like rend higher damage==================================
            if (spell_id == 7) {
                float energy_cost = 20;

                Players result = functions.get_one_nearest_enemy_inmelee(player, table_id, 0.7f, 50, false);
                if (result == null)
                {
                    return new float[] { 0, 3, 0 };
                }

                if (p.energy >= energy_cost)
                {
                    p.minus_energy(energy_cost);
                    Task.Run(() => warrior.bleeding(5, table_id, player, result.player_id, 7, 1.5f));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 5));
                    return new float[] { 1, 6, 5 };
                }
                else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 7==================================

            //================spell 8 melee dot like rend slow target==================================
            if (spell_id == 8) {
                float energy_cost = 20;

                Players result = functions.get_one_nearest_enemy_inmelee(player, table_id, 0.7f, 50, false);
                if (result == null)
                {
                    return new float[] { 0, 3, 0 };
                }
                
                if (p.energy >= energy_cost) { 
                    p.minus_energy(energy_cost);

                    Task.Run(() => warrior.bleeding_spell8(5, table_id, player, result.player_id, 8, 1));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 5));
                    return new float[] { 1, 6, 5 };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 8==================================


            //================spell 9 charge run and hit==================================
            if (spell_id == 9) {
                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);
                    Task.Run(() => warrior.ram(table_id, player));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 2));
                    return new float[] { 1, 6, 2 };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }


            }
            //================spell 9==================================



            //================spell 51 fire bolt==================================
            if (spell_id == 51) {
                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    
                    Task.Run(() => elementalist.firebolt51(table_id, player, energy_cost));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 1));
                    return new float[] { 1, 5, 1 };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 51==================================

            //================spell 52 METEOR==================================
            if (spell_id == 52) {
                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 5));
                    Task.Run(() => elementalist.meteor(table_id, player));
                    return new float[] { 1, 5, 5 };
                }
                else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 52==================================

            //================spell 53 fire hands==================================
            if (spell_id == 53) {
                float energy_cost = 20;

                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 5));
                    Task.Run(() => elementalist.firehands(table_id, player, 40));
                    return new float[] { 1, 5, 5 };
                }
                else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 53==================================

            //================spell 54 firewalk==================================
            if (spell_id == 54) {
                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 5));
                    Task.Run(() => elementalist.firewalk(table_id, player, 40));
                    return new float[] { 1, 5, 5 };
                }
                else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 54==================================


            //================spell 55 frost nova==================================
            if (spell_id == 55) {
                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);
                                            
                    string check_cond_strike_id = functions.get_symb_for_IDs();
                    p.conditions.TryAdd(check_cond_strike_id, $":cs=55={p.position_x.ToString("f1").Replace(',', '.')}={p.position_z.ToString("f1").Replace(',', '.')},");                    
                    remove_condition_in_player(table_id, player, check_cond_strike_id);
            
                    List<Players> result = functions.get_all_nearest_enemy_inradius(p.position_x, p.position_z, player, table_id, 3);

                    if (result.Count>0)
                    {
                        for (int i = 0; i < result.Count; i++) {
                            elementalist.freezed(table_id, player, result[i].player_id, 5);
                        }
                    }

                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 3));
                    return new float[] { 1, 5, 3 };
                }
                else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 55==================================



            //================spell 101 swing melee hit==================================
            if (spell_id == 101) {
                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);
                    Task.Run(() => barbarian.barbarian_melee_hit(table_id, player));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 1.5f));
                    return new float[] { 1, 5, 1.5f };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }


            }
            //================spell 101==================================

            //================spell 102 hurricane==================================
            if (spell_id == 102) {
                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);

                    Task.Run(() => barbarian.hurricane(table_id, player, 3));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 5f));
                    return new float[] { 1, 5, 5f };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }


            }
            //================spell 102==================================

            //================spell 103 heroic leap==================================
            if (spell_id == 103) {
                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);
                    Task.Run(() => barbarian.heroic_leap(table_id, player, 9));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 5f));
                    return new float[] { 1, 5, 5f};
                } else
                {
                    return new float[] { 0, 7, 0 };
                }


            }
            //================spell 103==================================

            //================spell 104 block==================================
            if (spell_id == 104) {
                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);

                    Task.Run(() => barbarian.block_prep(table_id, player, 1));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 2f));
                    return new float[] { 1, 5, 2f };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }

            }
            //================spell 104==================================

            //================spell 105 power strike==================================
            if (spell_id == 105) {
                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);
                    Task.Run(() => barbarian.power_attack(table_id, player));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 2f));
                    return new float[] { 1, 5, 2f };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }

            }
            //================spell 105==================================

            //================spell 151 fast strike==================================
            if (spell_id == 151) {
                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);

                    Task.Run(() => rogue.fast_strike(table_id, player));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 0.7f));
                    return new float[] { 1, 5, 0.7f };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }


            }
            //================spell 151==================================

            //================spell 152 backstab==================================
            if (spell_id == 152) {
                float energy_cost = 10;

                if (p.energy >= energy_cost) {

                    functions.turn_to_enemy(player, table_id, 0.1f, 0, 0, 3);
            
                    Players aim = functions.get_one_nearest_enemy_inmelee(player, table_id, 0, 0, false);
                    
                    if (aim==null) {
                        return new float[] { 0, 3, 0 };
                    } else
                    {
            
                        Players enemy = aim;
                        float current_angle_of_enemy = functions.player_angle_unity(enemy.position_x, enemy.position_y, enemy.position_z, enemy.rotation_x, enemy.rotation_y, enemy.rotation_z, p.position_x, p.position_y, p.position_z);

                        if (current_angle_of_enemy <= 90) {
                            return new float[] { 0, 3, 0 };
                        }
                    }

                    //backstab($table_id, $player, $energy_cost);
                    p.minus_energy(energy_cost);                    
                    make_direct_melee_damage(table_id, player, 152, 11, 2, 2, 0.2f);
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 5f));
                    return new float[] { 1, 5, 5f };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }


            }
            //================spell 152==================================

            //================spell 153 invizibility==================================
            if (spell_id == 153) {
                float energy_cost = 10;

                if (p.is_cond_here_by_type_and_spell("co-153"))
                {
                    return new float[] { 0, 13, 0 };
                }

                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);
                    Task.Run(() => rogue.invizibility(table_id, player));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 5f));
                    return new float[] { 1, 0, 5f };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 153==================================

            //================spell 154 butchery==================================
            if (spell_id == 154) {
                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);

                    Task.Run(() => rogue.butchery(table_id, player, 1));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 4f));
                    return new float[] { 1, 0, 4f };
                } 
                else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 154==================================

            //================spell 155 step==================================
            if (spell_id == 155) {
                float energy_cost = 10;

                if (p.energy >= energy_cost) {
            
                    Players aim = functions.get_one_nearest_enemy_inradius(player, table_id, 10, false);

                    if (aim != null) {
                        p.minus_energy(energy_cost);
                        Task.Run(() => rogue.step(table_id, player, aim));
                        Task.Run(() => button_cooldowns(table_id, player, spell_id, 2f));
                        return new float[] { 1, 0, 2f };
                    }

                } else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 155==================================

            //================spell 156 step==================================
            if (spell_id == 156)
            {
                float energy_cost = 10;

                if (p.energy >= energy_cost)
                {

                    if (p.energy >= energy_cost)
                    {
                        Task.Run(() => rogue.pistol_shot(table_id, player));
                        Task.Run(() => button_cooldowns(table_id, player, spell_id, 2f));
                        return new float[] { 1, 5, 2f };
                    }
                    else
                    {
                        return new float[] { 0, 7, 0 };
                    }

                }
                else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 155==================================


            //================spell 201 death beam==================================
            if (spell_id == 201) {

                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    Task.Run(() => wizard.death_beam(table_id, player, energy_cost, 2, 20));
                    return new float[] { 1, 5, 2f };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 201==================================

            //================spell 202 black hole==================================
            if (spell_id == 202) {

                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);
                    Task.Run(() => wizard.black_hole(table_id, player, 5));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 2f));
                    return new float[] { 1, 0, 2f };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 202==================================

            //================spell 203 reflect any attack shield==================================
            if (spell_id == 203) {

                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    p.minus_energy(energy_cost);
                    Task.Run(() => wizard.answer_attack_shield(table_id, player, 5, 2));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 2f));
                    return new float[] { 1, 0, 2f };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 203==================================

            //================spell 204 auto heal==================================
            if (spell_id == 204) {

                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    Task.Run(() => wizard.auto_heal(table_id, player, 5, 10, energy_cost)); // 1   1   long   heal  1
                    //Task.Run(() => button_cooldowns(table_id, player, spell_id, 4f));
                    Task.Run(() => button_cooldowns_global_only(table_id, player));
                    return new float[] { 1, 5, 0.7f };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 204==================================

            //================spell 205 curse of casting==================================
            if (spell_id == 205) {

                float energy_cost = 10;

                if (p.energy >= energy_cost) {
                    Task.Run(() => wizard.curse_of_casting(table_id, player, 5, energy_cost));
                    Task.Run(() => button_cooldowns(table_id, player, spell_id, 7f));
                    return new float[] { 1, 5, 7f };
                } else
                {
                    return new float[] { 0, 7, 0 };
                }
            }
            //================spell 205==================================

            //================STRAFE==================================
            if (spell_id == 997)
            {
                //Task.Run(() => strafe(table_id, player));
                Task.Run(() => button_cooldowns(table_id, player, spell_id, 2f));
                return new float[] { 1, 5, 2f };
            }
            //================STRAFE==================================


            return new float[] {0,0,0 };
        }




        //HEALING
        public static void healing(string table_id, string mee, string aim, int spell_number, float amount, float power_koef, float crit_add_damage_koef)
        {
            Players me = functions.GetPlayerData(table_id, mee);
            Players enemy = functions.GetPlayerData(table_id, aim);

            if (enemy.isDead)
            {
                return;
            }

            float summ_heal = amount + me.spell_power * power_koef;
            string critt = "s";

            if (functions.assess_chance(me.magic_crit))
            {
                summ_heal = summ_heal * crit_add_damage_koef;
                critt = "c";
            }

            if (summ_heal<0)
            {
                summ_heal = 0;
            }

            string [] enemy_health = enemy.health_pool.Split('=');
            if ( (float.Parse(enemy_health[0]) + summ_heal) > float.Parse(enemy_health[1]))
            {
                summ_heal = float.Parse(enemy_health[1]) - float.Parse(enemy_health[0]);
            }

            enemy.health_pool = (float.Parse(enemy_health[0]) + summ_heal).ToString("f0") + "=" + enemy_health[1];
            //==========================

            string id = functions.get_symb_for_IDs();
            me.conditions.TryAdd(id, $":hg-{summ_heal.ToString("f0")}-{critt}-{spell_number},");
            enemy.conditions.TryAdd(id, $":ht-{summ_heal.ToString("f0")}-{critt}-{spell_number},");
            remove_condition_in_player(table_id, mee, id);
            remove_condition_in_player(table_id, aim, id);
        }


   

        





        public static void direct_impose_damage(string table_id, string mee, string enemyy, int spell_number, float base_damage, float power_koef)
        {
            Players me = functions.GetPlayerData(table_id, mee);
            Players enemy = functions.GetPlayerData(table_id, enemyy);

            if (enemy.isDead)
            {
                return;
            }

            float end_damage = base_damage * power_koef;

            string[] enemy_healthenemy = enemy.health_pool.Split('=');
            enemy.health_pool = (float.Parse(enemy_healthenemy[0]) - end_damage).ToString("f1") + "=" + enemy_healthenemy[1];
            string id = functions.get_symb_for_IDs();
            set_animation_for_one(table_id, enemyy, 4,2,0.3f);
            
            me.conditions.TryAdd(id, $":dg-{end_damage.ToString("f1").Replace(',', '.')}-s-{spell_number},");
            enemy.conditions.TryAdd(id, $":dt-{end_damage.ToString("f1").Replace(',', '.')}-s-{spell_number},");

            remove_condition_in_player(table_id, mee, id);
            remove_condition_in_player(table_id, enemyy, id);
        }


            

     

        
    


        //knocking down
        public static async void fall_down_get_app(string table_id, string pl, float tick_time_left)
        {
            string id_knocked = functions.get_symb_for_IDs();
            reset_animation_for_one(table_id, pl);
            
            Players player = functions.GetPlayerData(table_id, pl);
            player.animation_id = 17;
            player.dodge *= 0.001f;
            player.magic_resistance *= 0.001f;
            player.shield_block *= 0.001f;

            player.is_reset_movement_button = true;
            player.make_knocked(id_knocked, tick_time_left);
            await Task.Delay(200);
            float u = 0;

            for (float i = (tick_time_left-0.2f); i > 0.2f; i-=0.2f)
            {
                if (u>2)
                {
                    player.animation_id = 18;
                } else
                {
                    u++;
                }

                player.make_knocked(id_knocked, i);
                await Task.Delay(200);
            }

            player.animation_id = 19;
            player.dodge /= 0.001f;
            player.magic_resistance /= 0.001f;
            player.shield_block /= 0.001f;

            await Task.Delay(200);

            reset_animation_for_one(table_id, pl);
            remove_condition_in_player(table_id, pl, id_knocked);
            player.is_reset_movement_button = false;
        }


      
      



        //pooling &&&& spell +$power - pull, -$power - push
        public static void pooling(string table_id, float center_x, float center_z, string enemy, float power, float speed)
        {
            Players player = functions.GetPlayerData(table_id, enemy);
            float[] res = functions.turn_virtual_face_to_face(table_id, center_x, center_z, enemy);

            //float[] res = new float[6];
            functions.mover(ref res, 0, power, speed);
            player.position_x = res[0];
            player.position_y = res[1];
            player.position_z = res[2];
        }




        //making DAMAGE with magic spells
        public static void make_direct_magic_damage_exact_enemy(string table_id, string mee, string enemyy, int spell_number, float base_magic_damage, float spell_power_koef, float crit_add_damage_koef)
        {
            Players me = functions.GetPlayerData(table_id, mee);
            Players enemy = functions.GetPlayerData(table_id, enemyy);

            if (enemy.isDead)
            {
                return;
            }

            float summ_attack = base_magic_damage + me.spell_power * spell_power_koef;
            float default_angle = 80;
            float current_angle_of_enemy = functions.player_angle_unity(enemy.position_x, enemy.position_y, enemy.position_z, enemy.rotation_x, enemy.rotation_y, enemy.rotation_z, me.position_x, me.position_y, me.position_z);
            bool isblocked = false;
            if (current_angle_of_enemy<=default_angle)
            {
                if (functions.assess_chance(enemy.shield_block))
                {
                    summ_attack = 0;
                    isblocked = true;
                }
            }
            float end_damage = summ_attack;
            string critt = "s";

            if (functions.assess_chance(me.magic_crit))
            {
                end_damage = summ_attack * crit_add_damage_koef;
                critt = "c";
            } 
            if (end_damage<0)
            {
                end_damage = 0;
            }

            bool is_resisted = false;
            if (functions.assess_chance(enemy.magic_resistance))
            {
                is_resisted = true;
                end_damage = 0;
            }

            float enemy_armor = enemy.armor;
            if (enemy.armor>starter.armor_max)
            {
                enemy_armor = starter.armor_max - 1;
            }
            end_damage = end_damage * ((starter.armor_max - enemy_armor) / starter.armor_max);

            //CHHHHHHEEEEEEEECCCCCKKKKKKIIIIIINNNNNNNGGGGGGGGGGGGGG OTHERS
            if (enemy.is_immune_to_magic)
            {
                enemy.data_when_immune_magic = end_damage + "=" + spell_number;
                end_damage = 0;

                string id_rr = functions.get_symb_for_IDs();
                enemy.conditions.TryAdd(id_rr, ":me-i,");
                remove_condition_in_player(table_id, enemyy, id_rr);

                me.conditions.TryAdd(id_rr, ":him-i,");
                remove_condition_in_player(table_id, mee, id_rr);
            }
            //=====================================


            string[] enemy_health = enemy.health_pool.Split('=');
            enemy.health_pool = (float.Parse(enemy_health[0]) - end_damage).ToString("f0") + "=" + enemy_health[1];

            string id = functions.get_symb_for_IDs();
            me.conditions.TryAdd(id, $":dg-{end_damage.ToString("f0").Replace(',','.')}-{critt}-{spell_number},");
            enemy.conditions.TryAdd(id, $":dt-{end_damage.ToString("f0").Replace(',', '.')}-{critt}-{spell_number},");
            remove_condition_in_player(table_id, mee, id);
            remove_condition_in_player(table_id, enemyy, id);

            if (is_resisted)
            {
                string id_r = functions.get_symb_for_IDs();
                enemy.conditions.TryAdd(id_r, ":me-r,");
                remove_condition_in_player(table_id, enemyy, id_r);
                me.conditions.TryAdd(id_r, ":him-r,");
                remove_condition_in_player(table_id, mee, id_r);
            } else if (!isblocked)
            {
                set_animation_for_one(table_id, enemyy, 4,2,0.1f);
            } else if (isblocked)
            {
                string id_3 = functions.get_symb_for_IDs();
                if (enemy.animation_id<2)
                {
                    enemy.animation_id = 7;
                    reset_animation_for_one(table_id, enemyy);
                }
                enemy.conditions.TryAdd(id_3, ":me-b,");
                remove_condition_in_player(table_id, enemyy, id_3);
                me.conditions.TryAdd(id_3, ":him-b,");
                remove_condition_in_player(table_id, mee, id_3);
            }
        }




        //base strafe from functions
        public static async void strafe(string table_id, string player_name, float vert, float horiz)
        {
            Players player = functions.GetPlayerData(table_id, player_name);
            player.is_strafe_on = true;
            player.is_spell_in_process = true;
            player.is_reset_any_button = true;
            float destination_x = horiz / 7.5f;
            float destination_z = vert / 6f;
            float koeff = Math.Abs(MathF.Atan(destination_z / destination_x));
            float angle = 0;
            //Console.WriteLine(koeff);
            //Console.WriteLine(CurrentPlayer.horizontal_touch + ":" + CurrentPlayer.vertical_touch + "      " + destination_x.ToString("f1") + " - horiz   " + destination_z.ToString("f1") + " -vert   = " + MathF.Atan(destination_z / destination_x).ToString("f5"));
            

            if (vert >= 0 && horiz >= 0)
            {
                angle = 90f * (1f - koeff / 1.6f);
                

            } else if (vert < 0 && horiz >= 0)
            {
                angle = 90f + 90f * (koeff / 1.6f);
                

            } else if (vert < 0 && horiz < 0)
            {
                angle = 180f + 90f * (1f - koeff / 1.6f);
                

            } else if (vert >= 0 && horiz < 0)
            {
                angle = 270f + 90f * (koeff / 1.6f);
                
            }

            float old_rot_y = 0;

            if ((player.rotation_y + angle) > 360)
            {
                old_rot_y = player.rotation_y + angle - 360;
            } else
            {
                old_rot_y = player.rotation_y + angle;
            }
            Console.WriteLine(player.horizontal_touch + "-" + player.vertical_touch + "       " + koeff + ": " + angle);

            float[] pos_rot = new float[] {player.position_x, 0, player.position_z, 0, old_rot_y, 0 };

            
            float old_dodge = player.dodge;
            player.dodge = 100;
            float old_mag_res = player.magic_resistance;
            player.magic_resistance = 100;
            float old_speed = player.speed;
            player.speed = 1;

            string id = functions.get_symb_for_IDs();
            player.conditions.TryAdd(id, ":co-997-0.3,");
            float[] delta = new float[] {45,15,-5,0,0 };

                        
            for (int i = 0; i < 5; i++)
            {
                functions.mover(ref pos_rot, 0, delta[i], 1f);

                player.position_x = pos_rot[0];
                player.position_z = pos_rot[2];
                await Task.Delay(60);
            }
                                    

            player.dodge = old_dodge;
            player.magic_resistance = old_mag_res;
            player.speed = old_speed;
            remove_condition_in_player(table_id, player_name, id);
            
            player.is_strafe_on = false;
            player.is_spell_in_process = false;
            player.is_reset_any_button = false;
        }

      



        //splash damage
        public static async void make_splash_melee_damage(string table_id, string me, int spell_number, int anim_clip, float hit_power_koef, float crit_add_damage_koef, float time_delay_anim, float add_distance, float add_angle)
        {
            if (anim_clip!=0)
            {
                set_animation_for_one(table_id, me, anim_clip, 5, 0.4f);
            }

            if (time_delay_anim>0)
            {
                await Task.Delay((int)(time_delay_anim*1000));
            }

            List<Players> all_players = new List<Players>();
            all_players = functions.get_all_nearest_enemy_inmelee(me, table_id, add_distance, add_angle);

            if (all_players==null)
            {
                return;
            }

            for (int i = 0; i < all_players.Count; i++)
            {
                melee_damage(table_id, me, all_players[i].player_id, spell_number, hit_power_koef, crit_add_damage_koef);
            }
        }




        



        public static async void button_cooldowns(string table_id, string player_id, int button_place, float cool_down)
        {
            Players p = functions.GetPlayerData(table_id, player_id);
            
            p.global_button_cooldown = 1;           
            await Task.Delay((int)(starter.GlobalButtonCooldown * 1000));
                
            p.global_button_cooldown = 0;
            
            int for_check = 0;
            
            if (p.spell1 == button_place)
            {
                for_check = 1;
                p.spell1 = 0;
            }
            if (p.spell2 == button_place)
            {
                for_check = 2;
                p.spell2 = 0;
            }
            if (p.spell3 == button_place)
            {
                for_check = 3;
                p.spell3 = 0;
            }
            if (p.spell4 == button_place)
            {
                for_check = 4;
                p.spell4 = 0;
            }
            if (p.spell5 == button_place)
            {
                for_check = 5;
                p.spell5 = 0;
            }
            if (p.spell6 == button_place)
            {
                for_check = 6;
                p.spell6 = 0;
            }

            //Thread.Sleep((int)(cool_down * 1000)-700);
            if (cool_down > 0.7f)
            {
                await Task.Delay((int)(cool_down * 1000) - 700);
            }

            switch (for_check)
            {
                case 1:
                    p.spell1 = button_place;
                    break;
                case 2:
                    p.spell2 = button_place;
                    break;
                case 3:
                    p.spell3 = button_place;
                    break;
                case 4:
                    p.spell4 = button_place;
                    break;
                case 5:
                    p.spell5 = button_place;
                    break;
                case 6:
                    p.spell6 = button_place;
                    break;
            }
           
        }

       

        public static async void button_cooldowns_cooldown_only(string table_id, string player_id, int button_place, float cool_down)
        {
            Players p = functions.GetPlayerData(table_id, player_id);

            int for_check = 0;

            if (p.spell1 == button_place)
            {
                for_check = 1;
                p.spell1 = 0;
            }
            if (p.spell2 == button_place)
            {
                for_check = 2;
                p.spell2 = 0;
            }
            if (p.spell3 == button_place)
            {
                for_check = 3;
                p.spell3 = 0;
            }
            if (p.spell4 == button_place)
            {
                for_check = 4;
                p.spell4 = 0;
            }
            if (p.spell5 == button_place)
            {
                for_check = 5;
                p.spell5 = 0;
            }
            if (p.spell6 == button_place)
            {
                for_check = 6;
                p.spell6 = 0;
            }

            string res = $"|1~2~{button_place}~0~{cool_down}~{functions.get_symb_for_IDs()}";
            p.AdditionalPacketData = res;
            await Task.Delay(100);
            p.AdditionalPacketData = "";

            await Task.Delay((int)(cool_down * 1000 - 100));
            

            switch (for_check)
            {
                case 1:
                    p.spell1 = button_place;
                    break;
                case 2:
                    p.spell2 = button_place;
                    break;
                case 3:
                    p.spell3 = button_place;
                    break;
                case 4:
                    p.spell4 = button_place;
                    break;
                case 5:
                    p.spell5 = button_place;
                    break;
                case 6:
                    p.spell6 = button_place;
                    break;
            }

        }

        public static async void button_cooldowns_global_only(string table_id, string player_id)
        {
            Players p = functions.GetPlayerData(table_id, player_id);

            p.global_button_cooldown = 1;
            await Task.Delay((int)(starter.GlobalButtonCooldown * 1000));

            p.global_button_cooldown = 0;
                       

        }


        public static async void reset_animation_for_one(string table_id, string me)
        {        
            await Task.Delay(110);
            Players p = functions.GetPlayerData(table_id, me);
            p.animation_id = 0;
            
        }

        //set animation
        public static async Task<bool> set_animation_for_one(string table_id, string me_name, int general_animation, int stop_animation, float time_for_playing)
        {
            
            Players p = functions.GetPlayerData(table_id, me_name);
            if (p.animation_id >= stop_animation || p.animation_id == general_animation)
            {
                return false;
                
            }
            else
            {
                for (float i = time_for_playing; i > 0; i -= (float)(starter.GlobalTick / 1000f))
                {
                    Players pp = functions.GetPlayerData(table_id, me_name);
                    if (pp.animation_id >= stop_animation && pp.animation_id != general_animation)
                    {
                        return false;
                    }
                    pp.animation_id = general_animation;
                    //Thread.Sleep(starter.GlobalTick);
                    await Task.Delay(starter.GlobalTick);

                }

                reset_animation_for_one(table_id, me_name);
                return true;
            }
         
        }

        //melee hit of one who closer==============================
        public static async void make_direct_melee_damage(string table_id, string me, int spell_number, int anim_clip, float hit_power_koef, float crit_add_damage_koef, float time_delay_anim)
        {
            if (time_delay_anim == 0)
            {
                time_delay_anim = 0.01f;
            }

            if (anim_clip != 0)
            {
                set_animation_for_one(table_id, me, anim_clip, 5, 0.4f);
            }
                        
            await Task.Delay((int)(time_delay_anim * 1000f));
            

            Players enemy = functions.get_one_nearest_enemy_inmelee(me, table_id, 0, 0, true);

            if (enemy == null)
            {

            }
            else
            {
                melee_damage(table_id, me, enemy.player_id, spell_number, hit_power_koef, crit_add_damage_koef);
            }

        }
               
        public static async void remove_condition_in_player(string table_id, string object_name, string cond_id)
        {
            
            string x;
            Players p = functions.GetPlayerData(table_id, object_name);
            
            p.conditions.TryGetValue(cond_id, out x);
            if (x != null)
            {
                string[] bulk = x.Split('-');
                if (bulk[0] == ":co")
                {
                    p.conditions.TryRemove(cond_id, out x);
                    p.conditions.TryAdd(cond_id, $"{bulk[0]}-{bulk[1]}-0,");
                }
            }

            await Task.Delay(120);

             
            p.conditions.TryRemove(cond_id, out x);
        }


        //melee hit of one who closer==============================
        public static async void make_direct_melee_damage_exact_enemy(string table_id, string me, string enemy, int spell_number, int anim_clip, float hit_power_koef, float crit_add_damage_koef, float time_delay_anim)
        {
            if (anim_clip !=0)
            {
                set_animation_for_one(table_id, me, anim_clip, 5, 0.6f);
            }
            if (time_delay_anim > 0)
            {
                await Task.Delay((int)(time_delay_anim * 1000f));
            }
            melee_damage(table_id, me, enemy, spell_number, hit_power_koef, crit_add_damage_koef);

        }


       
   

        //check if resisted magic
        public static bool if_resisted_magic(string table_id, string mee, string enemy)
        {
            Players player1 = functions.GetPlayerData(table_id, enemy);
            Players me = functions.GetPlayerData(table_id, mee);

            if (functions.assess_chance(player1.magic_resistance))
            {
                string id_r = functions.get_symb_for_IDs();
                player1.conditions.TryAdd(id_r, ":me-r,");
                remove_condition_in_player(table_id, enemy, id_r);
                me.conditions.TryAdd(id_r, ":him-r,");
                remove_condition_in_player(table_id, mee, id_r);
                return true;
            }

            return false;
        }


        //check if resisted non magic
        public static bool if_resisted_nonmagic(string table_id, string mee, string enemyy)
        {
            Players enemy = functions.GetPlayerData(table_id, enemyy);
            Players me = functions.GetPlayerData(table_id, mee);

            //for dodge
            if (functions.assess_chance(enemy.dodge))
            {
                string id_r = functions.get_symb_for_IDs();

                enemy.animation_id = 6; //6 - ID for DODGE
                reset_animation_for_one(table_id, enemyy);
                enemy.conditions.TryAdd(id_r, ":me-d,");
                remove_condition_in_player(table_id, enemyy, id_r);
        
                me.conditions.TryAdd(id_r, ":him-d,");
                remove_condition_in_player(table_id, mee, id_r);

                return true;
            }
            float default_angle = 80;
            float current_angle_of_enemy = functions.player_angle_unity(enemy.position_x, 0, enemy.position_z, 0, enemy.rotation_y, 0, me.position_x, 0, me.position_z);

            if (current_angle_of_enemy <= default_angle && functions.assess_chance(enemy.shield_block))
            {
                string id_r = functions.get_symb_for_IDs();

                enemy.animation_id = 7; 
                reset_animation_for_one(table_id, enemyy);
                enemy.conditions.TryAdd(id_r, ":me-b,");
                remove_condition_in_player(table_id, enemyy, id_r);

                me.conditions.TryAdd(id_r, ":him-b,");
                remove_condition_in_player(table_id, mee, id_r);

                return true;
            }

            return false;
        }



        //main part of MELEE DAMAGE HIT
        public static void melee_damage(string table_id, string me, string enemy1, int spell_number, float hit_power_koef, float crit_add_damage_koef)
        {
            Players p = functions.GetPlayerData(table_id, me);
            string[] main_hand = p.weapon_attack.Split('-');
            Random rnd = new Random();
            float weapon_attack = rnd.Next(int.Parse(main_hand[0]), int.Parse(main_hand[1]));
            float summ_attack = weapon_attack + p.hit_power * hit_power_koef;
            Players enemy = functions.GetPlayerData(table_id, enemy1);

            if (enemy.isDead)
            {
                return;
            }

            float default_angle = 80;
            float current_angle_of_enemy = functions.player_angle_unity(enemy.position_x, 0, enemy.position_z, 0, enemy.rotation_y, 0, p.position_x, 0, p.position_z);
            bool is_dodged = false;
            bool is_blocked = false;

            if (functions.assess_chance(enemy.dodge))
            {
                summ_attack = 0;
                is_dodged = true;
            }
            else if (current_angle_of_enemy <= default_angle)
            {
                if (functions.assess_chance(enemy.shield_block))
                {
                    summ_attack = 0;
                    is_blocked = true;
                }
            }

            float end_damage = summ_attack;
            string critt = "s";

            if (functions.assess_chance(p.melee_crit))
            {
                end_damage = summ_attack * crit_add_damage_koef;
                critt = "c";
            }
            else
            {
                end_damage = summ_attack;
                critt = "s";
            }
            if (end_damage < 0)
            {
                end_damage = 0;
            }

            //armor assess
            float enemy_armor = enemy.armor;
            if (enemy_armor > starter.armor_max)
            {
                enemy_armor = starter.armor_max - 1;
            }
            end_damage = end_damage * ((starter.armor_max - enemy_armor) / starter.armor_max);

            
            //CHHHHHHEEEEEEEECCCCCKKKKKKIIIIIINNNNNNNGGGGGGGGGGGGGG OTHERS
            if (enemy.is_immune_to_melee)
            {
                enemy.data_when_immune_melee = end_damage + "=" + spell_number;
                end_damage = 0;

                string id_rr = functions.get_symb_for_IDs();
                enemy.conditions.TryAdd(id_rr, ":me-i,");
                remove_condition_in_player(table_id, enemy1, id_rr);

                p.conditions.TryAdd(id_rr, ":him-i,");
                remove_condition_in_player(table_id, me, id_rr);
            }
            //=====================================
            

            string[] enemy_health = enemy.health_pool.Split('=');
            enemy.health_pool = (float.Parse(enemy_health[0]) - end_damage).ToString("f0") + "=" + enemy_health[1];

            //==========================
            string id = functions.get_symb_for_IDs();

            p.conditions.TryAdd(id, $":dg-{end_damage.ToString("f0")}-{critt}-{spell_number},");
           
            enemy.conditions.TryAdd(id, $":dt-{end_damage.ToString("f0")}-{critt}-{spell_number},");

            remove_condition_in_player(table_id, me, id);
            remove_condition_in_player(table_id, enemy1, id);

            if (!is_dodged && !is_blocked)
            {
                set_animation_for_one(table_id, enemy1, 4, 2, 0.1f);
            }
            if (is_dodged)
            {
                string id_2 = functions.get_symb_for_IDs();
                enemy.animation_id = 6; //6 - ID for DODGE
                reset_animation_for_one(table_id, enemy1);
               
                enemy.conditions.TryAdd(id_2, ":me-d,");
              
                p.conditions.TryAdd(id_2, ":him-d,");
                remove_condition_in_player(table_id, me, id_2);
                remove_condition_in_player(table_id, enemy1, id_2);
            }
            if (is_blocked)
            {
                string id_3 = functions.get_symb_for_IDs();
                set_animation_for_one(table_id, enemy1, 7, 2, 0.2f);
              
                enemy.conditions.TryAdd(id_3, ":me-b,");
                remove_condition_in_player(table_id, enemy1, id_3);
               
                p.conditions.TryAdd(id_3, ":him-b,");
                remove_condition_in_player(table_id, me, id_3);
            }

                        
        }
        

        public static SpellCharacteristics SpellID(int spell_number)
        {
            /*
            public enum GeneralChars        
            attacking = 0,   positive,    negative,  no_such_spell
       
            public enum TypeofDamaging       
            close_melee = 0,   magic,   ranged_melee,   only_self,   self_and_others,   side_effect    
            
            public enum ContinuityOfDamage   
            instant = 0, dot,  not_damage
       
            public enum TypeOfMagic        
            not_magic = 0,  fire,  frost,        
            */

            switch(spell_number)
            {
                case 0:
                    break;
                case 1:
                    new SpellCharacteristics(0,0,0,0);
                    break;
                case 2:

                    break;
                case 3:

                    break;
                case 4:

                    break;
                case 5:

                    break;
                case 6:

                    break;
                case 7:

                    break;
                case 8:

                    break;
                case 9:

                    break;
                
                //elementalist
                case 51:
                    new SpellCharacteristics(0, 1, 0, 1); //fire bolt
                    break;
                case 52:
                    new SpellCharacteristics(0, 1, 0, 1); //meteor
                    break;
                case 53:
                    new SpellCharacteristics(0, 1, 1, 1); //pillar of fire
                    break;
                case 54:
                    new SpellCharacteristics(0, 1, 1, 1); //fire steps
                    break;
                case 55:
                    new SpellCharacteristics(0, 1, 0, 2); //frost nova
                    break;
                case 56:

                    break;

            }

            return new SpellCharacteristics(3, 0, 0, 0);

        }


        public struct SpellCharacteristics
        {
            public GeneralChars general;
            public TypeofDamaging damage_type;
            public ContinuityOfDamage damage_timing;
            public TypeOfMagic magic_type;

            public SpellCharacteristics(int general_, int damage_type_, int damage_timing_, int magic_type_)
            {
                general = (GeneralChars)general_;
                damage_type = (TypeofDamaging)damage_type_;
                damage_timing = (ContinuityOfDamage)damage_timing_;
                magic_type = (TypeOfMagic)magic_type_;
            }
        }

        



    }


    public enum GeneralChars
    {
        attacking=0,
        positive=1,
        negative=2,
        no_such_spell=3
    }

    public enum TypeofDamaging
    {
        close_melee=0,
        magic=1,
        ranged_melee=2,
        only_self=3,
        self_and_others=4,
        side_effect=5
    }

    public enum ContinuityOfDamage
    {
        instant=0,
        dot=1,
        not_damage=2
    }

    public enum TypeOfMagic
    {
        not_magic=0,
        fire=1,
        frost=2,
    }


}
