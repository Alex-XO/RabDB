using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace RabDB
{
    class Program
    {
        enum Commands
        {
            Create = 1,
            FindByName = 2,
            Edit = 3,
            Delete = 4,
            DeleteAll = 5,
            Exit = 6
        }
        static void Main(string[] args)
        {
            MyDatabase db = new MyDatabase();
            db.ConnectDB();

            Commands? command = null;

            while (command != Commands.Exit)
            {
                Console.WriteLine("Введите команду, чтобы продолжить: ");
                var commands = Enum.GetValues(typeof(Commands));
                foreach (var currentCommand in commands)
                {
                    Console.WriteLine($"{(int)currentCommand}.{currentCommand}");
                }
                
                var answer = Console.ReadLine().ToLower();
                
                try
                {
                    command = Enum.Parse<Commands>(answer, true);
                    ExecutingCommands(command, db);
                } catch(Exception e)
                {
                    Console.WriteLine("Команда " + answer + " не существует");
                }

                
            }

            db.ConnectionClose();
        }

        static string AskUser(string text)
        {
            Console.Write(text);
            return Console.ReadLine();
        }

        static void ExecutingCommands(Commands? command, MyDatabase db)
        {
            switch (command)
            {
                case Commands.Create:
                    int idPers = int.Parse(AskUser("Введите id: "));
                    string namePers = AskUser("Введите name: ");
                    db.Create(idPers, namePers);
                    db.ShowUsers();
                    break;
                case Commands.Edit:
                    db.ShowUsers();
                    string name = AskUser("Введите name, кого хотите отредактировать: ");
                    string newName = AskUser("Введите новый name: ");
                    db.Edit(name, newName);
                    db.ShowUsers();
                    break;
                case Commands.Delete:
                    db.ShowUsers();
                    int idPerson = int.Parse(AskUser("Введите id: "));
                    db.Delete(idPerson);
                    db.ShowUsers();
                    break;
                case Commands.DeleteAll:
                    db.DeleteAll();
                    break;
                case Commands.FindByName:
                    string names = AskUser("Введите name: ");
                    db.FindByName(names);
                    break;
            }

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