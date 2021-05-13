using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace game_server
{
    class data_config
    {

        public static void Init_data_config()
        {
            FileStream file = null;
            try
            {
                file = new FileStream(starter.address_for_data_config, FileMode.Open);
            }
            catch (Exception ex)
            {
                Console.WriteLine("==============ERROR================\n" + ex + "\n" + "error with data file" + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
                return;
            }

            try
            {
                byte[] read_bytes = new byte[file.Length];
                int count_read_result = file.ReadAsync(read_bytes).Result;
                string string_data = Encoding.UTF8.GetString(read_bytes);

                starter.MysqlConnectionData_login = string_data.Substring(string_data.IndexOf("mysql_server_data: ") + "mysql_server_data: ".Length, string_data.IndexOf("secret_key_for_game_servers: ") - string_data.IndexOf("mysql_server_data: ") - "mysql_server_data: ".Length - 2);

                string[] data_arr = (string_data.Substring(string_data.IndexOf("secret_key_for_game_servers: ") + "secret_key_for_game_servers: ".Length, string_data.IndexOf("InnerServerConnectionPassword: ") - string_data.IndexOf("secret_key_for_game_servers: ") - "secret_key_for_game_servers: ".Length - 2)).Split(',');
                byte[] key = new byte[data_arr.Length];
                for (int i = 0; i < data_arr.Length; i++)
                {
                    key[i] = byte.Parse(data_arr[i]);
                }
                starter.secret_key_for_game_servers = key;

                starter.InnerServerConnectionPassword = string_data.Substring(string_data.IndexOf("InnerServerConnectionPassword: ") + "InnerServerConnectionPassword: ".Length, string_data.Length - string_data.IndexOf("InnerServerConnectionPassword: ") - "InnerServerConnectionPassword: ".Length);

            }
            catch (Exception ex)
            {
                file.Close();
                Console.WriteLine("==============ERROR================\n" + ex + "\n" + "error with data file" + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
                return;
            }

            file.Close();
            Console.WriteLine(DateTime.Now + ": data config OK");

        }

    }
}
