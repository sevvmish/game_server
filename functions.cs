using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;


namespace game_server
{
    public class functions
    {


        public static void MainProcess(string CurrentPacket)
        {            
            string[] RawDataArray = CurrentPacket.Split('~');
            
            Players CurrentPlayer = GetPlayerData(RawDataArray[3], RawDataArray[2]);

            string result = "";

           
            CurrentPlayer.ZeroInputs();

            try
            {
                CurrentPlayer.OrderNumber = int.Parse(RawDataArray[0]);
                CurrentPlayer.horizontal_touch = float.Parse(RawDataArray[4]);
                CurrentPlayer.vertical_touch = float.Parse(RawDataArray[5]);

                if (RawDataArray[1] == "1")
                {
                    if (RawDataArray[6] == "1")
                    {
                        CurrentPlayer.button1 = true;
                    }
                    else
                    if (RawDataArray[7] == "1")
                    {
                        CurrentPlayer.button2 = true;
                    }
                    else
                    if (RawDataArray[8] == "1")
                    {
                        CurrentPlayer.button3 = true;
                    }
                    else
                    if (RawDataArray[9] == "1")
                    {
                        CurrentPlayer.button4 = true;
                    }
                    else
                    if (RawDataArray[10] == "1")
                    {
                        CurrentPlayer.button5 = true;
                    }
                    else
                    if (RawDataArray[11] == "1")
                    {
                        CurrentPlayer.button6 = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("==============ERROR================\n" + ex + "\n" + CurrentPacket + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
            }

            /*
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
            */







            /*
            if (CurrentPlayer.vertical_touch > 10)
            {
                CurrentPlayer.vertical_touch = 10;
            }
            if (CurrentPlayer.vertical_touch < -10)
            {
                CurrentPlayer.vertical_touch = -10;
            }

            if (CurrentPlayer.horizontal_touch > 10)
            {
                CurrentPlayer.horizontal_touch = 10;
            }
            if (CurrentPlayer.horizontal_touch < -10)
            {
                CurrentPlayer.horizontal_touch = -10;
            }
            */
            
            

           


            //MAIN ROTATION PART
            if (CurrentPlayer.horizontal_touch != 0 || CurrentPlayer.vertical_touch != 0) 
                CurrentPlayer.rotation_y = GetRotationY(CurrentPlayer.horizontal_touch, CurrentPlayer.vertical_touch);


            //===================================0000000000000000000000======================================
            if (RawDataArray[1] == "0")
            {
                float[] cur_pos_n_rot = new float[6] { CurrentPlayer.position_x, CurrentPlayer.position_y, CurrentPlayer.position_z, CurrentPlayer.rotation_x, CurrentPlayer.rotation_y, CurrentPlayer.rotation_z };
                
                if (CurrentPlayer.horizontal_touch != 0 || CurrentPlayer.vertical_touch != 0)
                {

                    
                   


                    if (!CurrentPlayer.is_reset_movement_not_rotation)
                    {
                        new_pos_n_rot(ref cur_pos_n_rot, CurrentPlayer.horizontal_touch, CurrentPlayer.vertical_touch, CurrentPlayer.speed);
                    }
                 

                    CurrentPlayer.position_x = cur_pos_n_rot[0];
                    CurrentPlayer.position_y = cur_pos_n_rot[1];
                    CurrentPlayer.position_z = cur_pos_n_rot[2];
                    CurrentPlayer.rotation_x = cur_pos_n_rot[3];
                    CurrentPlayer.rotation_y = cur_pos_n_rot[4];
                    CurrentPlayer.rotation_z = cur_pos_n_rot[5];

                    

                    
                    if (CurrentPlayer.animation_id < 2)
                    {
                        CurrentPlayer.animation_id = 1;
                    }

                } else
                {
                    if (CurrentPlayer.animation_id < 2)
                    {
                        CurrentPlayer.animation_id = 0;
                    }

                }



                result = "|" + CurrentPlayer.OrderNumber.ToString() + "~0^" + CurrentPlayer.GetPacketForSending();

                foreach (Players ppp in starter.SessionsPool[RawDataArray[3]].LocalPlayersPool.Values)
                {
                    if (ppp.player_id != RawDataArray[2] && ppp.team_id>=0)
                    {
                        result = result + ppp.GetPacketForSending(RawDataArray[2]);
                    }
                }
                                
            }

            //===================================11111111111111111111======================================
            else if (RawDataArray[1] == "1" && CurrentPlayer.global_button_cooldown == 0)
            {
                float[] return_result = new float[] { 0, 0, 0 };

                if (CurrentPlayer.global_button_cooldown == 0 && (CurrentPlayer.button1 || CurrentPlayer.button2 || CurrentPlayer.button3 || CurrentPlayer.button4 || CurrentPlayer.button5 || CurrentPlayer.button6))
                {
                    //CurrentPlayer.is_spell_button_touched = false;

                    if (CurrentPlayer.button1)
                    {//spell1                        
                        return_result = spells.CastSpell(CurrentPlayer.spell1, RawDataArray[3], RawDataArray[2], CurrentPlayer);
                        CurrentPlayer.button1 = false;
                        CurrentPlayer.is_spell_button_touched = false;
                    }
                    if (CurrentPlayer.button2)
                    {//spell2                        
                        return_result = spells.CastSpell(CurrentPlayer.spell2, RawDataArray[3], RawDataArray[2], CurrentPlayer);
                        CurrentPlayer.button2 = false;
                        CurrentPlayer.is_spell_button_touched = false;
                    }
                    if (CurrentPlayer.button3)
                    {//spell3                        
                        return_result = spells.CastSpell(CurrentPlayer.spell3, RawDataArray[3], RawDataArray[2], CurrentPlayer);
                        CurrentPlayer.button3 = false;
                        CurrentPlayer.is_spell_button_touched = false;
                    }
                    if (CurrentPlayer.button4)
                    {//spell4                        
                        return_result = spells.CastSpell(CurrentPlayer.spell4, RawDataArray[3], RawDataArray[2], CurrentPlayer);
                        CurrentPlayer.button4 = false;
                        CurrentPlayer.is_spell_button_touched = false;
                    }
                    if (CurrentPlayer.button5)
                    {//spell5                        
                        return_result = spells.CastSpell(CurrentPlayer.spell5, RawDataArray[3], RawDataArray[2], CurrentPlayer);
                        CurrentPlayer.button5 = false;
                        CurrentPlayer.is_spell_button_touched = false;
                    }
                    if (CurrentPlayer.button6)
                    {//spell6

                        return_result = spells.CastSpell(CurrentPlayer.spell6, RawDataArray[3], RawDataArray[2], CurrentPlayer);
                        CurrentPlayer.button6 = false;
                        CurrentPlayer.is_spell_button_touched = false;
                    }

                    result = "|" + CurrentPlayer.OrderNumber.ToString() + "~1~" + return_result[0] + "~" + return_result[1] + "~" + return_result[2] + "~";
                }
                else if (CurrentPlayer.global_button_cooldown == 1 && (CurrentPlayer.button1 || CurrentPlayer.button2 || CurrentPlayer.button3 || CurrentPlayer.button4 || CurrentPlayer.button5 || CurrentPlayer.button6))
                {
                    result = "|" + CurrentPlayer.OrderNumber.ToString() + "~1~0~1~0~";
                }

                result = result + "|" + CurrentPlayer.OrderNumber.ToString() + "~0^" + CurrentPlayer.GetPacketForSending();

                foreach (Players ppp in starter.SessionsPool[RawDataArray[3]].LocalPlayersPool.Values)
                {
                    if (ppp.player_id != RawDataArray[2] && ppp.team_id >= 0)
                    {

                        result = result + ppp.GetPacketForSending(RawDataArray[2]);
                    }
                }
                
            }
            //Console.WriteLine(result);
            result = result + starter.SessionsPool[RawDataArray[3]].environment_packet + CurrentPlayer.AdditionalPacketData;
            byte[] b = Encoding.UTF8.GetBytes(result);
            encryption.Encode(ref b, CurrentPlayer.secret_key);
            server.SendDataUDP(CurrentPlayer.endPointUDP, b);
            

        }


        public static float GetRotationY(float hor, float ver)
        {
            float result = 0;
            float[] new_vec = new float[3] {hor, ver, 0 };            
            normalize_to_vector(ref new_vec);

            //Console.WriteLine(new_vec[0].ToString("f1") + " - " + new_vec[1].ToString("f1"));
            float brutto_angle = RadianToDegree(MathF.Atan(new_vec[0] / new_vec[1]));
            
            //Console.WriteLine(brutto_angle.ToString("f2"));

            if (new_vec[0]>=0 && new_vec[1] >= 0)
            {
                result = brutto_angle;
            } 
            else if (new_vec[0] >= 0 && new_vec[1] < 0)
            {
                result = brutto_angle+180;
            }
            else if (new_vec[0] < 0 && new_vec[1] < 0)
            {
                result = brutto_angle + 180;
            }
            else if (new_vec[0] < 0 && new_vec[1] >= 0)
            {
                result = brutto_angle + 360;
            }
                      
            return result;
        }

        public static void new_pos_n_rot(ref float[] pos_rot, float hor_touch, float vert_touch, float speed)
        {
            
            float[] new_vec = new float[3] { hor_touch, vert_touch, 0 };
            normalize_to_vector(ref new_vec);

            hor_touch = MathF.Abs(new_vec[0]*5f);
            vert_touch = MathF.Abs(new_vec[1]*5f);

            if (MathF.Abs(hor_touch)> MathF.Abs(vert_touch))
            {
                vert_touch = MathF.Abs(hor_touch);
            } 
            else if (MathF.Abs(hor_touch) == MathF.Abs(vert_touch))
            {
                vert_touch = MathF.Abs(hor_touch);
            }
            else if (MathF.Abs(hor_touch) < MathF.Abs(vert_touch))
            {
                vert_touch = MathF.Abs(vert_touch);
            }


            /*
            if (vert_touch < 0)
            {
                speed = speed * 0.7f;
            }
            */

            //hor_touch *= 1.7f; //1.2   1.7   2.2
            hor_touch = 0;

            float position_x = pos_rot[0];
            float position_y = pos_rot[1];
            float position_z = pos_rot[2];
            float rotation_x = pos_rot[3];
            float rotation_y = pos_rot[4];
            float rotation_z = pos_rot[5];

            const float degree_to_radian = 0.0174532924f;
            float delta_for_rotation = 1f;
            if (Math.Abs(hor_touch) <= 2)
            {
                delta_for_rotation = 0.3f;
            } else if (Math.Abs(hor_touch) > 2 && Math.Abs(hor_touch) <= 4)
            {
                delta_for_rotation = 0.6f;
            } else if (Math.Abs(hor_touch) > 4)
            {
                delta_for_rotation = 1.2f;
            }

            //float new_rotation_y = rotation_y + hor_touch * delta_for_rotation;
            float new_rotation_y = rotation_y;

            /*
            if (new_rotation_y >= 360)
            {
                new_rotation_y = new_rotation_y - 360;
            }
            if (new_rotation_y < 0)
            {
                new_rotation_y = 360 + new_rotation_y;
            }
            */

            float delta_for_position = 0.08f * speed;

            float new_position_x = position_x + MathF.Sin(rotation_y * degree_to_radian) * delta_for_position * vert_touch;
            float new_position_z = position_z + 1 * MathF.Cos(rotation_y * degree_to_radian) * delta_for_position * vert_touch;

            if (!borders(new_position_x, new_position_z))
            {
                pos_rot = new float[6] { new_position_x, 0, new_position_z, rotation_x, new_rotation_y, rotation_z };
            } else
            {
                pos_rot = new float[6] { position_x, position_y, position_z, rotation_x, new_rotation_y, rotation_z };
            }

        }

        public static void mover(ref float[] pos_rot, float hor_touch, float vert_touch, float speed)
        {
            float position_x = pos_rot[0];
            float position_y = pos_rot[1];
            float position_z = pos_rot[2];
            float rotation_x = pos_rot[3];
            float rotation_y = pos_rot[4];
            float rotation_z = pos_rot[5];

            const float degree_to_radian = 0.0174532924f;
            float delta_for_rotation = 1f;
        

            float new_rotation_y = rotation_y + hor_touch * delta_for_rotation;

            if (new_rotation_y >= 360)
            {
                new_rotation_y = new_rotation_y - 360;
            }
            if (new_rotation_y < 0)
            {
                new_rotation_y = 360 + new_rotation_y;
            }

            float delta_for_position = 0.06f * speed;

            float new_position_x = position_x + MathF.Sin(rotation_y * degree_to_radian) * delta_for_position * vert_touch;
            float new_position_z = position_z + 1 * MathF.Cos(rotation_y * degree_to_radian) * delta_for_position * vert_touch;

            if (!borders(new_position_x, new_position_z))
            {
                pos_rot = new float[] { new_position_x, 0, new_position_z, rotation_x, new_rotation_y, rotation_z, 0 };
            }
            else
            {
                pos_rot = new float[] { position_x, position_y, position_z, rotation_x, new_rotation_y, rotation_z, 1 };
            }

        }

        public static bool borders(float x, float y)
        {
            /*
            float[] border_arr = {2.5f, -22f, 15.5f, -18f,
                -15.5f, -22f, -2.5f, -18f,
                -3f, 21f, 3f, 25f,
                14f, -20f, 16f, 20f,
                -15.5f, 18f, -2.5f, 22f,
                2.5f, 18f, 15.5f, 22f,
                -3f, -25f, 3f, -21f,
                -16f, -20f, -14f, 20f};
            */

            float[] border_arr = {
                -17,-16,-15,16,
                15,-16,17,16,
                -16,15,16,17,
                -16,-17,16,-15
            };

            float correction = 0.35f;

            for (int i = 0; i < border_arr.Length; i += 4)
            {
                if (x >= (border_arr[i] - correction) && x <= (border_arr[i + 2] + correction) && y >= (border_arr[i + 1] - correction) && y <= (border_arr[i + 3] + correction))
                {
                    return true;
                }
            }

            return false;
        }

        public static string get_symb_for_IDs()
        {
            int nub_of_symb = 6;
            string[] arr_name = { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "a", "s", "d", "f", "g", "h", "j", "k", "l", "z", "x", "c", "v", "b", "n", "m", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "Z", "X", "C", "V", "B", "N", "M", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string result = "";
            Random rnd = new Random();
            for (int i = 0; i < nub_of_symb; i++)
            {
                result = result + arr_name[rnd.Next(0, arr_name.Length - 1)];
            }

            return result;
        }

        
        public static string get_random_set_of_symb(int nub_of_symb)
        {
            string[] arr_name = { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "a", "s", "d", "f", "g", "h", "j", "k", "l", "z", "x", "c", "v", "b", "n", "m", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "Z", "X", "C", "V", "B", "N", "M", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string result = "";
            Random rnd = new Random();
            for (int i = 0; i < nub_of_symb; i++)
            {
                result = result + arr_name[rnd.Next(0, arr_name.Length - 1)];
            }

            return result;
        }
        

        public static bool assess_chance(float chance)
        {
            Random rnd = new Random();
            float random = rnd.Next(1, 100);

            if (random <= chance)
            {
                return true;
            } else
            {
                return false;
            }
        }

        //returns array with pos:x,y,z and delta:x,y,z
        public static float player_angle_unity(float player_pos_x, float player_pos_y, float player_pos_z, float player_rot_x, float player_rot_y, float player_rot_z, float enemy_pos_x, float enemy_pos_y, float enemy_pos_z)
        {
            const float degree_to_radian = 0.0174532924f;

            //forward pass - first data vector	        
            float new_position_x = MathF.Sin(player_rot_y * degree_to_radian) * MathF.Cos(player_rot_x * degree_to_radian);
            float new_position_y = MathF.Sin(-player_rot_x * degree_to_radian);
            float new_position_z = MathF.Cos(player_rot_x * degree_to_radian) * MathF.Cos(player_rot_y * degree_to_radian);

            //enemy - player - second data vector
            float delta_pos_x = enemy_pos_x - player_pos_x;
            float delta_pos_y = enemy_pos_y - player_pos_y;
            float delta_pos_z = enemy_pos_z - player_pos_z;

            //float cc = vector3_angle_unity(new_position_x, new_position_y, new_position_z, delta_pos_x, delta_pos_y, delta_pos_z);
            //Console.WriteLine(cc);
            return vector3_angle_unity(new_position_x, new_position_y, new_position_z, delta_pos_x, delta_pos_y, delta_pos_z);
        }

        public static float clamp_to_one(float nub_to_clamp)
        {
            if (nub_to_clamp > 1) {
                return 1;
            } else if (nub_to_clamp < -1) {
                return -1;
            }
            return nub_to_clamp;
        }

        // DOT function
        public static float vector3_dot_unity(float x_axis_1, float y_axis_1, float z_axis_1, float x_axis_2, float y_axis_2, float z_axis_2)
        {
            return (x_axis_1 * x_axis_2 + y_axis_1 * y_axis_2 + z_axis_1 * z_axis_2);
        }

        // unity magnitude function
        public static float vector3_magnitude_unity(float x_axis_1, float y_axis_1, float z_axis_1)
        {
            return MathF.Sqrt(x_axis_1 * x_axis_1 + y_axis_1 * y_axis_1 + z_axis_1 * z_axis_1);
        }

        //returns array with x,y,z keys
        public static void normalize_to_vector(ref float[] vector_axis)
        {
            float find_max = vector3_magnitude_unity(vector_axis[0], vector_axis[1], vector_axis[2]);
            if (find_max > 0.00001f)
            {
                vector_axis = new float[] { (vector_axis[0] / find_max), (vector_axis[1] / find_max), (vector_axis[2] / find_max) };
            } else
            {
                vector_axis = new float[] { 0, 0, 0 };
            }
        }

        // unity distance cheking function
        public static float vector3_distance_unity(float x_axis_1, float y_axis_1, float z_axis_1, float x_axis_2, float y_axis_2, float z_axis_2)
        {
            float xxxxx = MathF.Abs(x_axis_1 - x_axis_2);
            float yyyyy = MathF.Abs(y_axis_1 - y_axis_2);
            float zzzzz = MathF.Abs(z_axis_1 - z_axis_2);
            return MathF.Sqrt(xxxxx * xxxxx + yyyyy * yyyyy + zzzzz * zzzzz);
        }


        // just Unity VECTOR.ANGLE
        public static float vector3_angle_unity(float x_axis_1, float y_axis_1, float z_axis_1, float x_axis_2, float y_axis_2, float z_axis_2)
        {
            float[] norm_1 = new float[] { x_axis_1, y_axis_1, z_axis_1 };
            normalize_to_vector(ref norm_1);
            float[] norm_2 = new float[] { x_axis_2, y_axis_2, z_axis_2 };
            normalize_to_vector(ref norm_2);
            float dot_clamp_result = clamp_to_one(vector3_dot_unity(norm_1[0], norm_1[1], norm_1[2], norm_2[0], norm_2[1], norm_2[2]));
            return MathF.Acos(dot_clamp_result) * 57.29578f;
        }


        public static Players GetPlayerByIDinConditions(string table_id, string ID_to_search)
        {
            Players result = null;

            foreach (Players player in starter.SessionsPool[table_id].LocalPlayersPool.Values)
            {
                if (player.conditions.ContainsKey(ID_to_search))
                {
                    return player;
                }
            }

            return result;
        }

        public static Players GetPlayerData(string session_id, string player_id)
        {
            if (!starter.SessionsPool.ContainsKey(session_id))
            {
                Console.WriteLine("no such session");
                return null;
            }

            return starter.SessionsPool[session_id].LocalPlayersPool[player_id];
        }


        public static void minus_energy(string table_id, string player, float amount)
        {
            Players p = GetPlayerData(table_id, player);
            p.energy = p.energy - amount;
        }

        public static List<Players> GetAllPlayersInList(string table_id)
        {
            List<Players> result = new List<Players>();
            foreach (Players ppp in starter.SessionsPool[table_id].LocalPlayersPool.Values)
            {
                result.Add(ppp);
            }

            return result;
        }

        //get one in radius of OBJECT
        public static Players get_one_nearest_enemy_inradius_ofObject(float coord_xx, float coord_zz, string my_name, string table_id, float max_radius, bool check_invis)
        {
            max_radius = max_radius + 0.25f;
            Players p = GetPlayerData(table_id, my_name);
            float coord_x = coord_xx;
            float coord_z = coord_zz;

            Players result = null;
            float min_distance_checked = max_radius;
            List<Players> AllPlayersList = GetAllPlayersInList(table_id);

            if (!check_invis)
            {
                for (int i = 0; i < AllPlayersList.Count; i++)
                {
                    float check_radius = vector3_distance_unity(coord_x, 0, coord_z, AllPlayersList[i].position_x, 0, AllPlayersList[i].position_z);
                    if (!AllPlayersList[i].isDead && AllPlayersList[i].player_id != my_name && !AllPlayersList[i].is_invisible && AllPlayersList[i].team_id != p.team_id && max_radius > check_radius)
                    {
                        if (check_radius < min_distance_checked)
                        {
                            min_distance_checked = check_radius;
                            result = AllPlayersList[i];
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < AllPlayersList.Count; i++)
                {
                    float check_radius = vector3_distance_unity(coord_x, 0, coord_z, AllPlayersList[i].position_x, 0, AllPlayersList[i].position_z);
                    if (AllPlayersList[i].player_id != my_name && AllPlayersList[i].team_id != p.team_id && max_radius > check_radius)
                    {
                        if (check_radius < min_distance_checked)
                        {
                            min_distance_checked = check_radius;
                            result = AllPlayersList[i];
                        }
                    }
                }
            }
            return result;
        }



        public static Players get_one_nearest_enemy_inradius(string my_name, string table_id, float max_radius, bool check_invis)
        {
            max_radius = max_radius + 0.25f;
            Players p = GetPlayerData(table_id, my_name);
            float coord_x = p.position_x;
            float coord_z = p.position_z;

            Players result = null;
            float min_distance_checked = max_radius;
            List<Players> AllPlayersList = GetAllPlayersInList(table_id);

            if (!check_invis)
            {
                for (int i = 0; i < AllPlayersList.Count; i++)
                {
                    float check_radius = vector3_distance_unity(coord_x, 0, coord_z, AllPlayersList[i].position_x, 0, AllPlayersList[i].position_z);
                    if (!AllPlayersList[i].isDead   && AllPlayersList[i].player_id != my_name && !AllPlayersList[i].is_invisible && AllPlayersList[i].team_id != p.team_id && max_radius > check_radius)
                    {
                        if (check_radius < min_distance_checked)
                        {
                            min_distance_checked = check_radius;
                            result = AllPlayersList[i];
                        }
                    }
                }
            } else
            {
                for (int i = 0; i < AllPlayersList.Count; i++)
                {
                    float check_radius = vector3_distance_unity(coord_x, 0, coord_z, AllPlayersList[i].position_x, 0, AllPlayersList[i].position_z);
                    if (AllPlayersList[i].player_id != my_name && AllPlayersList[i].team_id != p.team_id && max_radius > check_radius)
                    {
                        if (check_radius < min_distance_checked)
                        {
                            min_distance_checked = check_radius;
                            result = AllPlayersList[i];
                        }
                    }
                }
            }
            return result;
        }

        //take all player who in radius with coords
        public static List<Players> get_all_nearest_enemy_inradius(float coord_x, float coord_z, string my_name, string table_id, float radius)
        {
            List<Players> result = new List<Players>();
            radius = radius + 0.25f;
            Players p = GetPlayerData(table_id, my_name);
            List<Players> AllPlayersList = new List<Players>();
            AllPlayersList = GetAllPlayersInList(table_id);
            for (int i = 0; i < AllPlayersList.Count; i++)
            {
                if (!AllPlayersList[i].isDead && AllPlayersList[i].player_id != p.player_id && AllPlayersList[i].team_id != p.team_id && radius > vector3_distance_unity(coord_x, 0, coord_z, AllPlayersList[i].position_x, 0, AllPlayersList[i].position_z))
                {
                    
                    result.Add(AllPlayersList[i]);
                }
            }

            return result;
        }




        //check if there any enemies in range of hit melee
        public static List<Players> get_all_nearest_enemy_inmelee(string me, string table_id, float add_distance, float add_angle)
        {
            List<Players> result = new List<Players>();
            //result = null;
            Players p = GetPlayerData(table_id, me);
            List<Players> AllPlayersList = new List<Players>();
            AllPlayersList = GetAllPlayersInList(table_id);
            float default_distance = starter.def_hit_melee_dist + add_distance;
            float default_angle = starter.def_hit_melee_angle + add_angle;
            float default_small_distance = starter.def_hit_melee_small_dist + add_distance;
            float default_small_angle = starter.def_hit_melee_small_angle + add_angle;
            float default_min_radius = starter.def_hit_melee_min_radius;
            for (int i = 0; i < AllPlayersList.Count; i++)
            {
                float current_distance = vector3_distance_unity(p.position_x, 0, p.position_z, AllPlayersList[i].position_x, 0, AllPlayersList[i].position_z);
                float current_angle = player_angle_unity(p.position_x, 0, p.position_z, 0, p.rotation_y, 0, AllPlayersList[i].position_x, 0, AllPlayersList[i].position_z);
                if (AllPlayersList[i].player_id != p.player_id && !AllPlayersList[i].isDead && AllPlayersList[i].team_id != p.team_id &&
                    ((current_distance < default_distance && current_angle < default_angle) ||
                    (current_distance < default_small_distance && current_angle < default_small_angle) ||
                    (default_min_radius > vector3_distance_unity(p.position_x, 0, p.position_z, AllPlayersList[i].position_x, 0, AllPlayersList[i].position_z)))
                    )
                {
                    result.Add(AllPlayersList[i]);
                }
            }

            return result;
        }

        //check if there ONLY ONE enemie in range of hit melee
        public static Players get_one_nearest_enemy_inmelee(string me_name, string table_id, float add_distance, float add_angle, bool check_invis)
        {
            Players result = null;
            List<Players> preresult = new List<Players>();
            Players p = GetPlayerData(table_id, me_name);
            List<Players> AllPlayersList = GetAllPlayersInList(table_id);

            float default_distance = starter.def_hit_melee_dist + add_distance;
            float default_angle = starter.def_hit_melee_angle + add_angle;
            float default_small_distance = starter.def_hit_melee_small_dist + add_distance;
            float default_small_angle = starter.def_hit_melee_small_angle + add_angle;
            float default_min_radius = starter.def_hit_melee_min_radius;

            for (int i = 0; i < AllPlayersList.Count; i++)
            {
                float current_distance = vector3_distance_unity(p.position_x, 0, p.position_z, AllPlayersList[i].position_x, 0, AllPlayersList[i].position_z);
                float current_angle = player_angle_unity(p.position_x, 0, p.position_z, 0, p.rotation_y, 0, AllPlayersList[i].position_x, 0, AllPlayersList[i].position_z);
                if (AllPlayersList[i].player_id != p.player_id && !AllPlayersList[i].isDead && AllPlayersList[i].team_id != p.team_id &&
                    ((current_distance < default_distance && current_angle < default_angle) ||
                    (current_distance < default_small_distance && current_angle < default_small_angle) ||
                    (default_min_radius > vector3_distance_unity(p.position_x, 0, p.position_z, AllPlayersList[i].position_x, 0, AllPlayersList[i].position_z)))
                    )
                {

                    if ((!check_invis && !AllPlayersList[i].is_invisible) || check_invis) {
                        preresult.Add(AllPlayersList[i]);
                    }
                }
            }
            
            if (preresult == null)
            {
                return null;
            }

            float coord_x = p.position_x;
            float coord_z = p.position_z;
            float min_distance_checked = 1000;
            for (int i = 0; i < preresult.Count; i++)
            {
                float check_radius = vector3_distance_unity(coord_x, 0, coord_z, preresult[i].position_x, 0, preresult[i].position_z);
                if (preresult[i].player_id != me_name && preresult[i].team_id != p.team_id)
                {
                    if (check_radius < min_distance_checked)
                    {
                        min_distance_checked = check_radius;
                        result = preresult[i];
                    }
                }
            }

            return result;
        }

        //good way to turn to any enemies
        public static bool turn_to_enemy(string me_name, string table_id, float time_for_turn, float add_dist_to_check, float add_angle_to_check, float radius_to_check)
        {
            Players aim = get_one_nearest_enemy_inmelee(me_name, table_id, add_dist_to_check, add_angle_to_check, false);
            Players enemy;

            if (aim != null)
            {
                enemy = aim;
            } else
            {
                aim = get_one_nearest_enemy_inradius(me_name, table_id, radius_to_check, false);
                if (aim != null)
                {
                    enemy = aim;
                } else
                {
                    return false;
                }
            }

            if (enemy.isDead)
            {
                return false;
            }

            Players p = GetPlayerData(table_id, me_name);
            int sign = 0;
            float current_angle = player_angle_unity(p.position_x, 0, p.position_z, 0, p.rotation_y, 0, enemy.position_x, 0, enemy.position_z);
            float current_angle1 = player_angle_unity(p.position_x, 0, p.position_z, 0, p.rotation_y + 1f, 0, enemy.position_x, 0, enemy.position_z);

            if (current_angle > 1f)
            {
                if (current_angle1 < current_angle)
                {
                    sign = 1;
                } else if (current_angle1 > current_angle)
                {
                    sign = -1;
                } else if (current_angle1 == current_angle)
                {
                    sign = 0;
                }

                float delta = current_angle * sign;
                //float time_delta = time_for_turn / 1f;
                if ((p.rotation_y + delta) > 360)
                {
                    p.rotation_y += delta - 360;
                } else if ((p.rotation_y + delta) < 0)
                {
                    p.rotation_y += delta + 360;
                } else if ((p.rotation_y + delta) >= 0 && (p.rotation_y + delta) <= 360)
                {
                    p.rotation_y += delta;
                }


            }
            //Console.WriteLine(current_angle + " - current angle after");
            return true;
        }


        //way to turn object to specific ENEMY
        public static void turn_object_to_exact_player(string aim, string table_id, ref float[] object_transform)
        {
            
            Players enemy = GetPlayerData(table_id, aim);


            //Players p = GetPlayerData(table_id, me_name);
            int sign = 0;
            float current_angle = player_angle_unity(object_transform[0], 0, object_transform[2], 0, object_transform[4], 0, enemy.position_x, 0, enemy.position_z);
            float current_angle1 = player_angle_unity(object_transform[0], 0, object_transform[2], 0, object_transform[4] + 1f, 0, enemy.position_x, 0, enemy.position_z);

            if (current_angle > 1f)
            {
                if (current_angle1 < current_angle)
                {
                    sign = 1;
                }
                else if (current_angle1 > current_angle)
                {
                    sign = -1;
                }
                else if (current_angle1 == current_angle)
                {
                    sign = 0;
                }

                float delta = current_angle * sign;
                //float time_delta = time_for_turn / 1f;
                if ((object_transform[4] + delta) > 360)
                {
                    object_transform[4] += delta - 360;
                }
                else if ((object_transform[4] + delta) < 0)
                {
                    object_transform[4] += delta + 360;
                }
                else if ((object_transform[4] + delta) >= 0 && (object_transform[4] + delta) <= 360)
                {
                    object_transform[4] += delta;
                }
            }
            //Console.WriteLine(current_angle + " - current angle after");
            return;
        }




        //way to turn object to enemy
        public static void turn_object_to_enemy_indirect(string me_name, string table_id, ref float [] object_transform, float add_dist_to_check, float add_angle_to_check, float radius_to_check)
        {
            Players aim = get_one_nearest_enemy_inmelee(me_name, table_id, add_dist_to_check, add_angle_to_check, false);
            Players enemy;

            if (aim != null)
            {
                enemy = aim;
            }
            else
            {                
                return;               
            }

            if (enemy.isDead)
            {
                return;
            }

            //Players p = GetPlayerData(table_id, me_name);
            int sign = 0;
            float current_angle = player_angle_unity(object_transform[0], 0, object_transform[2], 0, object_transform[4], 0, enemy.position_x, 0, enemy.position_z);
            float current_angle1 = player_angle_unity(object_transform[0], 0, object_transform[2], 0, object_transform[4]+1f, 0, enemy.position_x, 0, enemy.position_z);

            if (current_angle > 1f)
            {
                if (current_angle1 < current_angle)
                {
                    sign = 1;
                }
                else if (current_angle1 > current_angle)
                {
                    sign = -1;
                }
                else if (current_angle1 == current_angle)
                {
                    sign = 0;
                }

                float delta = current_angle * sign;
                //float time_delta = time_for_turn / 1f;
                if ((object_transform[4] + delta) > 360)
                {
                    object_transform[4] += delta - 360;
                }
                else if ((object_transform[4] + delta) < 0)
                {
                    object_transform[4] += delta + 360;
                }
                else if ((object_transform[4] + delta) >= 0 && (object_transform[4] + delta) <= 360)
                {
                    object_transform[4] += delta;
                }
            }
            //Console.WriteLine(current_angle + " - current angle after");
            return;
        }



        public static void turn_face_to_face(string me_name, string enemy_name, string table_id)
        {
            Players player = GetPlayerData(table_id, me_name);
            Players enemy1 = GetPlayerData(table_id, enemy_name);

            if (enemy1.isDead)
            {
                return;
            }

            float current_angle = player_angle_unity(player.position_x, 0, player.position_z, 0, player.rotation_y, 0, enemy1.position_x, 0, enemy1.position_z);
            float current_angle1 = player_angle_unity(player.position_x, 0, player.position_z, 0, player.rotation_y + 1f, 0, enemy1.position_x, 0, enemy1.position_z);
            int sign = 0;

            if (current_angle > 1f)
            {
                if (current_angle1 < current_angle)
                {
                    sign = 1;
                }
                else if (current_angle1 > current_angle)
                {
                    sign = -1;
                }
                else if (current_angle1 == current_angle)
                {
                    sign = 0;
                }

                float delta = current_angle * sign;
                //float time_delta = time_for_turn / 1f;
                if ((player.rotation_y + delta) > 360)
                {
                    player.rotation_y += delta - 360;
                }
                else if ((player.rotation_y + delta) < 0)
                {
                    player.rotation_y += delta + 360;
                }
                else if ((player.rotation_y + delta) >= 0 && (player.rotation_y + delta) <= 360)
                {
                    player.rotation_y += delta;
                }


            }
        }


        //set condition
        public static void set_condition(string cond_type, int condition_number, string cond_id, string table_id, string player_to_cond_name, float timer)
        {
            Players player_to_cond = GetPlayerData(table_id, player_to_cond_name);
            string x;
            player_to_cond.conditions.TryRemove(cond_id, out x);
            player_to_cond.conditions.TryAdd(cond_id, $":{cond_type}-{condition_number}-{timer.ToString("f1")},");
        }

        /*
        //set hidden condition
        public static void set_hiddenconds(string cond_type, string cond_id, string table_id, string player_to_cond_name)
        {
            Players player_to_cond = GetPlayerData(table_id, player_to_cond_name);
            player_to_cond.hidden_conds.Remove(cond_id);
            player_to_cond.hidden_conds.Add(cond_id, $":{cond_type},");
        }
        */

        //cheking for 1002 and 1003 and 1005 plus MOVEMENET and SPELLS
        public static bool is_casting_failed(string table_id, string player_name)
        {

            Players p = GetPlayerData(table_id, player_name);
            if (Math.Abs(p.vertical_touch) > 1.5f)
            {
                return true;
            }


            if (p.is_spell_button_touched)
            {

                return true;
            }

            string[] array_of_spells = new string[] { "1002", "1003", "1005", "1006", "1007" };
            bool result = is_any_cond_inarray_of_spellnumber(table_id, player_name, "co", array_of_spells);
            if (result)
            {
                return true;
            }


            return false;
        }


        /*
        //find any HIDDEN COND like type by naming condition only
        public static bool is_hiddencond_here_by_type_only(string player_name, string table_id, string searched_cond)
        {
            Players player = GetPlayerData(table_id, player_name);
            foreach (string vall in player.conditions.Values.ToList())
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
        public static bool is_any_cond_inarray_of_spellnumber(string table_id, string player_name, string type_of_cond, string[] array_to_check)
        {
            Players player = GetPlayerData(table_id, player_name);
            if (player.conditions.Count == 0)
            {
                return false;
            }

            try
            {
                foreach (string vall in player.conditions.Values.ToList())
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

        // CANCEL casting informing
        public static async void inform_of_cancel_casting(string player_name, string table_id, int spell_number)
        {
            Players player = GetPlayerData(table_id, player_name);
            string check_cond_id = get_symb_for_IDs();
            
            player.conditions.TryAdd(check_cond_id, $":ca-cncld-{spell_number},");
         
            await Task.Delay(100);
            
            spells.remove_condition_in_player(table_id, player_name, check_cond_id);
        }

        // CANCEL casting informing
        public static async void inform_of_cancel_casting(Players current_player, int spell_number)
        {
            Players player = current_player;
            string check_cond_id = get_symb_for_IDs();

            player.conditions.TryAdd(check_cond_id, $":ca-cncld-{spell_number},");

            await Task.Delay(100);

            spells.remove_condition_in_player(player, check_cond_id);
        }

        // CANCEL casting informing
        public static async void inform_of_cancel_casting(Players current_player, string player_name, string table_id, int spell_number)
        {
            Players player = current_player;
            string check_cond_id = get_symb_for_IDs();

            player.conditions.TryAdd(check_cond_id, $":ca-cncld-{spell_number},");

            await Task.Delay(100);

            spells.remove_condition_in_player(player, check_cond_id);
        }

        //get ID by type and spell like "xx-45"
        public static string get_id_by_type_and_spell(string player_name, string table_id, string searched_cond)
        {
            Players player = GetPlayerData(table_id, player_name);

            try
            {
                foreach (string vall in player.conditions.Keys.ToList())
                {
                    if (player.conditions[vall].Contains(searched_cond))
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

        //STUN
        public static void make_stun(string player, string table_id, string conds_id, float tick_time_left)
        {
            Players enemyy = GetPlayerData(table_id, player);
            enemyy.animation_id = 8;
            string x;
            enemyy.conditions.TryRemove(conds_id, out x);
            enemyy.conditions.TryAdd(conds_id, $":co-1002-{tick_time_left.ToString("f1")},");

        }


        //cheking for 1002 and 1003 and 1005 stopping casting
        public static bool is_casting_stopped_by_spells(string table_id, string player_name)
        {
            string[] array_of_spells = new string[] { "1002", "1003", "1005", "1006", "1007" };
            return is_any_cond_inarray_of_spellnumber(table_id, player_name, "co", array_of_spells);
        }


        public static bool is_cond_here_by_type_and_spell(string player_name, string table_id, string searched_cond)
        {
            Players player = GetPlayerData(table_id, player_name);

            try
            {
                foreach (string items in player.conditions.Values.ToList())
                {
                    if (items.Contains(searched_cond))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }


        //turn virtual player
        public static float [] turn_virtual_face_to_face(string table_id, float center_x, float center_z, string rotating_player)
        {
            Players player = GetPlayerData(table_id, rotating_player);
            float current_angle = player_angle_unity(player.position_x, 0, player.position_z, 0, player.rotation_y, 0, center_x, 0, center_z);
            float current_angle1 = player_angle_unity(player.position_x, 0, player.position_z, 0, player.rotation_y + 1f, 0, center_x, 0, center_z);

            float rotation_y=0;

            if (current_angle>1)
            {
                float sign = 0;
                if (current_angle1 <current_angle) {
                    sign = 1;
                } else if (current_angle1 >current_angle) {
                    sign = -1;
                } else if (current_angle1 ==current_angle) {
                    sign = 0;
                }

                float delta = current_angle * sign;

                if ((player.rotation_y +delta) > 360) {
                rotation_y = player.rotation_y + delta - 360;
                } else if ((player.rotation_y +delta)< 0) {
                rotation_y = player.rotation_y +delta + 360;
                } else if ((player.rotation_y +delta)>= 0 && (player.rotation_y +delta)<= 360) {
                rotation_y +=delta;
                }
            }

            return new float[] { player.position_x, 0, player.position_z, 0, rotation_y, 0 };
        }

        public static float DegreeToRadian(float angle)
        {
            return (float)Math.PI * angle / 180;
        }
        public static float RadianToDegree(float angle)
        {
            return angle * (180 / (float)Math.PI);
        }

        public static void lerp(ref float [] result_of_two, float start_x, float start_z, float end_x, float end_z, float rot_y, float koef)
        {
            float distance = Math.Abs((end_x - start_x) / MathF.Sin(DegreeToRadian(rot_y)));
            float x = start_x + MathF.Sin(DegreeToRadian(rot_y)) * koef * distance;
            float z = start_z + MathF.Cos(DegreeToRadian(rot_y)) * koef * distance;
            result_of_two = new float[] {x,z};

        }

        public static void projection(ref float[] result_of_two, float pos_x, float pos_z, float rot_y, float distance)
        {
            float x = pos_x + MathF.Sin(DegreeToRadian(rot_y)) * distance;
            float z = pos_z + MathF.Cos(DegreeToRadian(rot_y)) * distance;
            result_of_two = new float[] { x, z };
        }


        public static float what_damage_or_heal_received_analysis(string table_id, string player_for_searching, string what_condition_type) {

            Players player = functions.GetPlayerData(table_id, player_for_searching);
            
            var _temp_dat = from r in player.conditions.Values select r;
            string _data = string.Join("", _temp_dat);

            if (_data.Contains(what_condition_type))
            {
                bool isDamageGiven = false;
                string[] to_search = _data.Split(',');
                for (int i = 0; i < to_search.Length; i++)
                {
                    if (to_search[i].Contains(what_condition_type))
                    {
                        int _where_starts = to_search[i].IndexOf(what_condition_type) + 3;
                        int _where_ends = to_search[i].IndexOf("-", _where_starts + 1);
                        float result = float.Parse(to_search[i].Substring(_where_starts, _where_ends - _where_starts));

                        if (result > 0)
                        {
                            return result;                            
                        } 
                        else
                        {
                            return 0;
                        }
                    }
                }

               
            } else
            {
                return 0;
            }

            return 0;
        }






    }
}
