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
            Console.WriteLine("Вы хотите создать, редактировать, найти или удалить пользователя/всех из БД?");
            string answer = Console.ReadLine();


            if (answer.ToLower() == "создать")
            {
                int idPers = int.Parse(AskUser("Введите id: "));
                string namePers = AskUser("Введите name: ");
                db.Create(idPers, namePers);
                db.ShowUsers();
            }
            else if (answer.ToLower() == "найти")
            {
                string name = AskUser("Введите name: ");
                db.FindByName(name);
            }
            else if (answer.ToLower() == "удалить")
            {
                db.ShowUsers();
                int idPers = int.Parse(AskUser("Введите id: "));
                db.Delete(idPers);
                db.ShowUsers();
            }
            else if (answer.ToLower() == "удалить всех")
            {
                db.DeleteAll();
            }
            else if (answer.ToLower() == "редактировать")
            {
                db.ShowUsers();
                string name = AskUser("Введите name, кого хотите отредактировать: ");
                string newName = AskUser("Введите новый name: ");
                db.Edit(name, newName);
                db.ShowUsers();
            }

            db.ConnectionClose();
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
            var cmd = new SqlCommand("insert into users values (@id,@name)", connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {

            var cmd = new SqlCommand("DELETE FROM users WHERE id = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

        }

        public void FindByName(string name)
        {
            var cmd = new SqlCommand("SELECT * FROM users WHERE name = @name", connection);
            cmd.Parameters.AddWithValue("@name", name + '%');

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine("{0} \t {1}", reader[0], reader[1]);
                }
            }
        }

        public void DeleteAll()
        {
            var cmd = new SqlCommand("DELETE FROM users", connection);
            cmd.ExecuteNonQuery();
        }

        public void Edit(string name, string newName)
        {
            var cmd = new SqlCommand("UPDATE users SET name = @newName WHERE name = @name", connection);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@newName", newName);
            cmd.ExecuteNonQuery();
        }

        public void ConnectionClose()
        {
            connection.Close();
        }
    }
}