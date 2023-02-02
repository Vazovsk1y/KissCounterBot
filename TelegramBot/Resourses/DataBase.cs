using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using System.Data.SQLite;
using System.IO;

namespace TelegramBot.Resourses
{
    internal static class DataBase
    {
        private static string fullDBPath = @"D:\IDE\MyTelegramBot\TelegramBot\Resourses\KissCounterDB.db";
        private static string finalDatabasePath = Path.Combine(AppContext.BaseDirectory, fullDBPath);
        private static SQLiteConnectionStringBuilder сonnectionPath = new SQLiteConnectionStringBuilder()
        {
            DataSource = finalDatabasePath
        };
        
        internal static void AddUserWithCurrentGroup(Message message)
        {
            //try
            //{
            using (SQLiteConnection DB = new SQLiteConnection(сonnectionPath.ToString()))
            {
                DB.Open();
                var addUser = DB.CreateCommand();
                addUser.CommandText = $"INSERT INTO users_and_groups (user_id, user_first_name, group_id, group_title) "
                + $"VALUES ('{message.From.Id}', '{message.From.FirstName ?? string.Empty}', '{message.Chat.Id}', '{message.Chat.Title}')";
                addUser.ExecuteNonQuery();
                DB.Close();
            }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine($"Error: {e}");
            //}
        }

        internal static bool IsUserPlayingGame(Message message)
        {
            //try
            //{
            using (SQLiteConnection DB = new SQLiteConnection(сonnectionPath.ToString()))
            {
                DB.Open();
                var command = DB.CreateCommand();
                command.CommandText = $"SELECT 1 FROM users_and_groups WHERE user_id LIKE " +
                    $"'{message.From.Id}' AND group_id LIKE '{message.Chat.Id}'";
                return command.ExecuteScalar() != null;                                    // if user is playing method return true, else false.
            }
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine($"Error: {ex}");
            //    return false;
            //}
        }

        internal static void UpdateRecord(Message Originalmessage)
        {
            using (SQLiteConnection DB = new SQLiteConnection(сonnectionPath.ToString()))
            {
                //try
                //{
                DB.Open();
                var update = DB.CreateCommand();
                update.CommandText = $"UPDATE users_and_groups SET value = value + '{1}' WHERE user_id LIKE '{Originalmessage.From.Id}' " +
                    $"AND group_id LIKE '{Originalmessage.Chat.Id}'";
                update.ExecuteNonQuery();
                DB.Close();
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e.ToString());
                //}
            }
        }

        internal static bool IsInGroupAnybodyPlay(Message message)
        {
            using (SQLiteConnection DB = new SQLiteConnection(сonnectionPath.ToString()))
            {
                DB.Open();
                var command = DB.CreateCommand();
                command.CommandText = $"SELECT 1 FROM users_and_groups WHERE group_id LIKE '{message.Chat.Id}'";
                return command.ExecuteScalar() != null;
            }
        }

        internal static string GetTop(Message message)
        {
            Dictionary<string, int> usersValues = new Dictionary<string, int>();

            using (SQLiteConnection DB = new SQLiteConnection(сonnectionPath.ToString()))
            {
                DB.Open();
                SQLiteCommand command = DB.CreateCommand();
                command.CommandText = $"SELECT user_first_name,value FROM users_and_groups WHERE group_id LIKE '{message.Chat.Id}'";
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string userId = reader[0].ToString();
                    int value = Convert.ToInt32(reader[1]);
                    usersValues.Add(userId, value);
                }

                reader.Close();
                DB.Close();
            }

            var sortedTop = usersValues.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            return string.Join(Environment.NewLine, sortedTop);
        }
    }
}
