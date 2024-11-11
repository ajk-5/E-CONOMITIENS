using E_conomitiens.model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_conomitiens.viewModel
{
    public static class ParseAndUpload
    {
        public static void parseAndUpload()
        {
            string filePath = "../model/sample.txt";
            string connectionString = "Server=localhost;Database=economitiens;User=root;Password=;";
            List<Room> rooms = new List<Room>();

            Room currentRoom = null;
            string currentSection = null;

            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    if (line.StartsWith("ROOM_ID"))
                    {
                        if (currentRoom != null)
                        {
                            rooms.Add(currentRoom);
                        }
                        currentRoom = new Room
                        {
                            RoomId = GetValue(line, "ROOM_ID"),
                            Batiment = GetValue(line, "BATIMENT")
                        };
                    }
                    else if (line.EndsWith(":"))
                    {
                        currentSection = line.Replace(":", "").Trim();
                    }
                    else
                    {
                        ParseDataLine(currentRoom, currentSection, line);
                    }
                }

                if (currentRoom != null)
                {
                    rooms.Add(currentRoom);
                }
            }

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                foreach (var room in rooms)
                {
                    InsertRoom(conn, room);
                    foreach (var electricity in room.Electricities)
                    {
                        InsertElectricity(conn, electricity, room.RoomId);
                    }
                    foreach (var heat in room.Heats)
                    {
                        InsertHeat(conn, heat, room.RoomId);
                    }
                    foreach (var light in room.Lights)
                    {
                        InsertLight(conn, light, room.RoomId);
                    }
                    foreach (var humidity in room.Humidities)
                    {
                        InsertHumidity(conn, humidity, room.RoomId);
                    }
                }
            }
        }

        private static string GetValue(string line, string key)
        {
            int startIndex = line.IndexOf(key) + key.Length + 1;
            int endIndex = line.IndexOf(",", startIndex);
            if (endIndex == -1) endIndex = line.Length;
            return line.Substring(startIndex, endIndex - startIndex).Trim();
        }

        private static void ParseDataLine(Room room, string section, string line)
        {
            var parts = line.Split(',');

            switch (section)
            {
                case "ELECTRICITY":
                    var electricity = new Electricity
                    {
                        Id = int.Parse(GetValue(parts[0], "ID")),
                        DateTimeCollected = DateTime.Parse(GetValue(parts[1], "DATE_TIME_COLLECTED")),
                        Consumption = int.Parse(GetValue(parts[2], "CONSUMPTION"))
                    };
                    room.Electricities.Add(electricity);
                    break;

                case "HEAT":
                    var heat = new Heat
                    {
                        Id = int.Parse(GetValue(parts[0], "ID")),
                        DateTimeCollected = DateTime.Parse(GetValue(parts[1], "DATE_TIME_COLLECTED")),
                        Temperature = int.Parse(GetValue(parts[2], "TEMPERATURE"))
                    };
                    room.Heats.Add(heat);
                    break;

                case "LIGHT":
                    var light = new Light
                    {
                        Id = int.Parse(GetValue(parts[0], "ID")),
                        Lumen = int.Parse(GetValue(parts[1], "LUMEN")),
                        DateTimeCollected = DateTime.Parse(GetValue(parts[2], "DATE_TIME_COLLECTED"))
                    };
                    room.Lights.Add(light);
                    break;

                case "HUMIDITY":
                    var humidity = new Humidity
                    {
                        Id = int.Parse(GetValue(parts[0], "ID")),
                        GPM3 = int.Parse(GetValue(parts[1], "GPM3")),
                        DateTimeCollected = DateTime.Parse(GetValue(parts[2], "DATE_TIME_COLLECTED"))
                    };
                    room.Humidities.Add(humidity);
                    break;
            }
        }

        private static void InsertRoom(MySqlConnection conn, Room room)
        {
            using (var cmd = new MySqlCommand("INSERT INTO ROOM (ROOM_ID, BATIMENT) VALUES (@ROOM_ID, @BATIMENT)", conn))
            {
                cmd.Parameters.AddWithValue("@ROOM_ID", room.RoomId);
                cmd.Parameters.AddWithValue("@BATIMENT", room.Batiment);
                cmd.ExecuteNonQuery();
            }
        }

        private static void InsertElectricity(MySqlConnection conn, Electricity electricity, string roomId)
        {
            using (var cmd = new MySqlCommand("INSERT INTO ELECTRICITY (ID, DATE_TIME_COLLECTED, CONSUMPTION, ROOM_ID) VALUES (@ID, @DATE_TIME_COLLECTED, @CONSUMPTION, @ROOM_ID)", conn))
            {
                cmd.Parameters.AddWithValue("@ID", electricity.Id);
                cmd.Parameters.AddWithValue("@DATE_TIME_COLLECTED", electricity.DateTimeCollected);
                cmd.Parameters.AddWithValue("@CONSUMPTION", electricity.Consumption);
                cmd.Parameters.AddWithValue("@ROOM_ID", roomId);
                cmd.ExecuteNonQuery();
            }
        }

        private static void InsertHeat(MySqlConnection conn, Heat heat, string roomId)
        {
            using (var cmd = new MySqlCommand("INSERT INTO HEAT (ID, DATE_TIME_COLLECTED, TEMPERATURE, ROOM_ID) VALUES (@ID, @DATE_TIME_COLLECTED, @TEMPERATURE, @ROOM_ID)", conn))
            {
                cmd.Parameters.AddWithValue("@ID", heat.Id);
                cmd.Parameters.AddWithValue("@DATE_TIME_COLLECTED", heat.DateTimeCollected);
                cmd.Parameters.AddWithValue("@TEMPERATURE", heat.Temperature);
                cmd.Parameters.AddWithValue("@ROOM_ID", roomId);
                cmd.ExecuteNonQuery();
            }
        }

        private static void InsertLight(MySqlConnection conn, Light light, string roomId)
        {
            using (var cmd = new MySqlCommand("INSERT INTO LIGHT (ID, LUMEN, DATE_TIME_COLLECTED, ROOM_ID) VALUES (@ID, @LUMEN, @DATE_TIME_COLLECTED, @ROOM_ID)", conn))
            {
                cmd.Parameters.AddWithValue("@ID", light.Id);
                cmd.Parameters.AddWithValue("@LUMEN", light.Lumen);
                cmd.Parameters.AddWithValue("@DATE_TIME_COLLECTED", light.DateTimeCollected);
                cmd.Parameters.AddWithValue("@ROOM_ID", roomId);
                cmd.ExecuteNonQuery();
            }
        }

        private static void InsertHumidity(MySqlConnection conn, Humidity humidity, string roomId)
        {
            using (var cmd = new MySqlCommand("INSERT INTO HUMIDITY (ID, GPM3, DATE_TIME_COLLECTED, ROOM_ID) VALUES (@ID, @GPM3, @DATE_TIME_COLLECTED, @ROOM_ID)", conn))
            {
                cmd.Parameters.AddWithValue("@ID", humidity.Id);
                cmd.Parameters.AddWithValue("@GPM3", humidity.GPM3);
                cmd.Parameters.AddWithValue("@DATE_TIME_COLLECTED", humidity.DateTimeCollected);
                cmd.Parameters.AddWithValue("@ROOM_ID", roomId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
