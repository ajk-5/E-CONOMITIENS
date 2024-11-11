using System;
using System.Collections.Generic;
using System.Windows;
using MySql.Data.MySqlClient;

namespace Econome.Models
{
    public class InfoTime
    {
        public static Dictionary<string, double> GetMoyenneJour(string connectionString)
        {
            string query = @"
                SELECT 
                    ROOM.ROOM_NAME,
                    AVG(HEAT.TEMPERATURE) AS AVERAGE_TEMPERATURE
                FROM 
                    HEAT
                JOIN 
                    ROOM ON HEAT.ROOM_NAME = ROOM.ROOM_NAME
                WHERE 
                    DATE(HEAT.DATE_TIME_COLLECTED) = CURDATE()
                GROUP BY 
                    ROOM.ROOM_NAME;";

            return ExecuteQuery(connectionString, query);
        }

        public static Dictionary<string, double> GetMoyenneSemaine(string connectionString)
        {
            string query = @"
                SELECT 
                    ROOM.ROOM_NAME,
                    AVG(HEAT.TEMPERATURE) AS AVERAGE_TEMPERATURE
                FROM 
                    HEAT
                JOIN 
                    ROOM ON HEAT.ROOM_NAME = ROOM.ROOM_NAME
                WHERE 
                    DATE(HEAT.DATE_TIME_COLLECTED) >= CURDATE() - INTERVAL 1 WEEK
                    AND DATE(HEAT.DATE_TIME_COLLECTED) < CURDATE()
                GROUP BY 
                    ROOM.ROOM_NAME;";

            return ExecuteQuery(connectionString, query);
        }

        public static Dictionary<string, double> GetMoyenneMois(string connectionString)
        {
            string query = @"
                SELECT 
                    ROOM.ROOM_NAME,
                    AVG(HEAT.TEMPERATURE) AS AVERAGE_TEMPERATURE
                FROM 
                    HEAT
                JOIN 
                    ROOM ON HEAT.ROOM_NAME = ROOM.ROOM_NAME
                WHERE 
                    DATE(HEAT.DATE_TIME_COLLECTED) >= CURDATE() - INTERVAL 1 MONTH
                    AND DATE(HEAT.DATE_TIME_COLLECTED) < CURDATE()
                GROUP BY 
                    ROOM.ROOM_NAME;";

            return ExecuteQuery(connectionString, query);
        }

        public static Dictionary<string, double> GetMoyenneAnnee(string connectionString)
        {
            string query = @"
                SELECT 
                    ROOM.ROOM_NAME,
                    AVG(HEAT.TEMPERATURE) AS AVERAGE_TEMPERATURE
                FROM 
                    HEAT
                JOIN 
                    ROOM ON HEAT.ROOM_NAME = ROOM.ROOM_NAME
                WHERE 
                    DATE(HEAT.DATE_TIME_COLLECTED) >= CURDATE() - INTERVAL 1 YEAR
                    AND DATE(HEAT.DATE_TIME_COLLECTED) < CURDATE()
                GROUP BY 
                    ROOM.ROOM_NAME;";

            return ExecuteQuery(connectionString, query);
        }

        private static Dictionary<string, double> ExecuteQuery(string connectionString, string query)
        {
            var result = new Dictionary<string, double>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Database connection successful");

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string roomName = reader["ROOM_NAME"].ToString();
                                double averageTemperature = Convert.ToDouble(reader["AVERAGE_TEMPERATURE"]);
                                result[roomName] = averageTemperature;
                                Console.WriteLine($"Room: {roomName}, Average Temperature: {averageTemperature}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Afficher la fenêtre d'erreur personnalisée
                    Console.WriteLine("Database connection error: " + ex.Message);
                    ShowErrorWindow(ex.Message);
                }
            }

            return result;
        }

        private static void ShowErrorWindow(string errorMessage)
        {
            ErrorWindow errorWindow = new ErrorWindow(errorMessage);
            errorWindow.Owner = Application.Current.MainWindow; // Définir la fenêtre parente si nécessaire
            errorWindow.ShowDialog();
        }
    }
}
