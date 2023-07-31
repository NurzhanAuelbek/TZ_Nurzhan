using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace TZ_Nurzhan
{
    internal class Program
    {
        // Подключения к базе данных
        private static string connectionString = @"Data Source=DESKTOP-DKM570K\MYDBSERVER;Initial Catalog=TZ_Nurzhan;Integrated Security=True";
        static void Main(string[] args)
        {
            Console.WriteLine("Zadanie_1");
            string query = "SELECT * FROM Containers";
            // Выполняем запрос и получаем результат в формате JSON
            string result = ExecuteSqlQuery(query);
            // Выводим результат на консоль
            Console.WriteLine(result + "\n");

            Console.WriteLine("Zadanie_2");
            // Получаем ID контейнера по его номеру
            string containerId = GetContainerIdByNumber(1);
            // Формируем запрос с параметром
            query = "SELECT * FROM Operations WHERE ContainerID = @ContainerID";
            // Выполняем запрос и получаем результат в формате JSON
            result = ExecuteSqlQuery(query, new List<SqlParameter> { new SqlParameter("@ContainerID", containerId) });
            // Выводим результат на консоль
            Console.WriteLine(result + "\n");
        }

        // Метод для получения ID контейнера по его номеру
        private static string GetContainerIdByNumber(int containerNumber)
        {
            string queryForContainerId = "SELECT ID FROM Containers WHERE Number = @ContainerNumber";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryForContainerId, connection);
                command.Parameters.AddWithValue("@ContainerNumber", containerNumber);
                connection.Open();
                return command.ExecuteScalar().ToString();
            }
        }

        // Метод для выполнения SQL запроса и получения результата в формате JSON
        private static string ExecuteSqlQuery(string query, List<SqlParameter> parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(reader);

                var data = new List<Dictionary<string, object>>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        dict[col.ColumnName] = row[col];
                    }
                    data.Add(dict);
                }
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                return JsonSerializer.Serialize(data, options);
            }
        }
    }
}
