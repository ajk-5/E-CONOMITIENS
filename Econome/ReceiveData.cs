    using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace Econome
{
    internal class ReceiveData
    {
        internal static string[] GetTemperatureData(string roomName)
        {
            string connectionString = "Server=localhost;Database=Economitiens;User ID=root;Password=;";
            string[] temperatureData = new string[3];

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
            WITH RankedHeat AS (
                SELECT h.ROOM_NAME, h.DATE_TIME_COLLECTED, h.TEMPERATURE,
                       ROW_NUMBER() OVER (PARTITION BY h.ROOM_NAME ORDER BY h.DATE_TIME_COLLECTED DESC) AS rn
                FROM HEAT h
                WHERE h.ROOM_NAME = @RoomName
            )
            SELECT ROOM_NAME, DATE_TIME_COLLECTED, TEMPERATURE
            FROM RankedHeat
            WHERE rn <= 3
            ORDER BY DATE_TIME_COLLECTED DESC;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Ajouter le paramètre pour le nom de la salle
                    command.Parameters.AddWithValue("@RoomName", roomName);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        int index = 0;
                        while (reader.Read() && index < 3)
                        {
                            DateTime dateTimeCollected = (DateTime)reader["DATE_TIME_COLLECTED"];
                            string formattedDateTime = dateTimeCollected.ToString("dd/MM/yyyy - HH:mm");
                            float temperature = Convert.ToSingle(reader["TEMPERATURE"]);

                            string originalString = formattedDateTime;
                            char oldChar = ':';
                            char newChar = 'h';

                            StringBuilder sb = new StringBuilder(originalString);

                            for (int i = 0; i < sb.Length; i++)
                            {
                                if (sb[i] == oldChar)
                                {
                                    sb[i] = newChar;
                                }
                            }

                            string modifiedDateTime = sb.ToString();

                            temperatureData[index] = $"{modifiedDateTime}   {temperature:0.0}°C";
                            index++;
                        }

                        // Fill remaining entries with "No data available." if less than 3 records found
                        for (; index < 3; index++)
                        {
                            temperatureData[index] = "Moins de 3 relevés pour cette salle";
                        }
                    }
                }
            }

            return temperatureData;
        }

        internal static string[] GetHumidityData(string roomName)
        {
            string connectionString = "Server=localhost;Database=Economitiens;User ID=root;Password=;";
            string[] humidityData = new string[3];

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
            WITH RankedHumidity AS (
                SELECT h.ROOM_NAME, h.DATE_TIME_COLLECTED, h.HUMIDITY,
                       ROW_NUMBER() OVER (PARTITION BY h.ROOM_NAME ORDER BY h.DATE_TIME_COLLECTED DESC) AS rn
                FROM HUMIDITY h
                WHERE h.ROOM_NAME = @RoomName
            )
            SELECT ROOM_NAME, DATE_TIME_COLLECTED, HUMIDITY
            FROM RankedHumidity
            WHERE rn <= 3
            ORDER BY DATE_TIME_COLLECTED DESC;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@RoomName", roomName);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        int index = 0;
                        while (reader.Read() && index < 3)
                        {
                            DateTime dateTimeCollected = (DateTime)reader["DATE_TIME_COLLECTED"];
                            string formattedDateTime = dateTimeCollected.ToString("dd/MM/yyyy - HH:mm");
                            float humidity = Convert.ToSingle(reader["HUMIDITY"]);

                            humidityData[index] = $"{formattedDateTime}     :     {humidity:0.0}%";
                            index++;
                        }

                        // Fill remaining entries with "No data available." if less than 3 records found
                        for (; index < 3; index++)
                        {
                            humidityData[index] = "Moins de trois relevés";
                        }
                    }
                }
            }

            return humidityData;
        }

        internal static string GetLightData(string roomName)
        {
            string connectionString = "Server=localhost;Database=Economitiens;User ID=root;Password=;";
            string lightStatus = "No data available.";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT ROOM_NAME, DATE_TIME_COLLECTED, LIGHT_STATE
            FROM LIGHT
            WHERE ROOM_NAME = @RoomName
            ORDER BY DATE_TIME_COLLECTED DESC
            LIMIT 1;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Ajouter le paramètre pour le nom de la salle
                    command.Parameters.AddWithValue("@RoomName", roomName);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bool lightState = Convert.ToBoolean(reader["LIGHT_STATE"]);
                            lightStatus = lightState ? "Allumées" : "Éteintes";
                        }
                    }
                }
            }

            return lightStatus;
        }
    }
}