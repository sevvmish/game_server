using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using System.Globalization;

namespace game_server
{
    class mysql
    {
        
        public async static Task<string[,]> GetMysqlSelect(string sql)
        {


            string[,] ReceivedDataRows;

            try
            {
                MySqlConnection conn = new MySqlConnection(starter.MysqlConnectionData_login);
                await conn.OpenAsync();

                MySqlCommand command = new MySqlCommand(sql, conn);
                MySqlDataReader reader = await command.ExecuteReaderAsync();

                int ColumnNumber = reader.FieldCount;

                List<string> dat = new List<string>();

                int rows = 0;
                while (reader.Read())
                {
                    for (int i = 0; i < ColumnNumber; i++)
                    {
                        dat.Add(reader[i].ToString());
                    }
                    rows++;
                }

                ReceivedDataRows = new string[rows, ColumnNumber];

                int index = 0;
                for (int i = 0; i < rows; i++)
                {
                    for (int u = 0; u < ColumnNumber; u++)
                    {
                        ReceivedDataRows[i, u] = dat[index];
                        index++;
                    }
                }

                reader.Close();
                conn.Close();
                return ReceivedDataRows;
            }
            catch (Exception ex)
            {
                Console.WriteLine("==============ERROR================\n" + ex + "\n" + sql + "\n" + DateTime.Now + "\n" + "==================ERROR_END===========\n");
            }

            ReceivedDataRows = new string[1, 1];
            ReceivedDataRows[0, 0] = "error";
            return ReceivedDataRows;
        }


        public async static Task<bool> ExecuteSQLInstruction(string sql)
        {


            try
            {
                MySqlConnection conn = new MySqlConnection(starter.MysqlConnectionData_login);
                await conn.OpenAsync();

                MySqlCommand command = new MySqlCommand(sql, conn);
                int r = await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("==============ERROR================\n" + ex + "\n" + sql + "\n" +  DateTime.Now + "\n" + "==================ERROR_END===========\n");
            }

            return false;
        }
    }
}
