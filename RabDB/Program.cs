using System;
using Microsoft.Data.SqlClient;

namespace RabDB
{
    class Program
    {
        static void Main(string[] args)
        {
            MyDatabase db = new MyDatabase();
            db.ConnectDB();
            Console.WriteLine("Вы хотите создать или удалить пользователя БД?");
            string answer = Console.ReadLine();

            if (answer.ToLower() == "да")
            {
                int idPers = int.Parse(AskUser("Введите id: "));
                string namePers = AskUser("Введите name: ");
                db.Create(idPers, namePers);
            }
            else if (answer.ToLower() == "нет")
            {
                int idPers = int.Parse(AskUser("Введите id: "));
                db.Delete(idPers);
            }


            db.ShowUsers();
        }

        static string AskUser(string text)
        {
            Console.Write(text);
            return Console.ReadLine();
        }
    }

    class MyDatabase
    {
        private SqlConnection connection;
        private String connectionString = "Server=localhost;Database=My_Database;Trusted_Connection=True;Encrypt=False;";
        public void ConnectDB() 
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
            Console.WriteLine("Connection opened successful");
        }

        public void ShowUsers()
        {
            string sql = "SELECT * FROM users ORDER BY id";
            SqlCommand command = new SqlCommand(sql, connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine("{0} \t {1}", reader[0], reader[1]);
                }
            }
        }

        public void Create(int id, string name)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("insert into users values (@id,@name)", conn); 
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", name);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("DELETE FROM users WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
