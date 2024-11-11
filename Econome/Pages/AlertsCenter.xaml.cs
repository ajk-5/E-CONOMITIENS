using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Econome.Pages
{
    /// <summary>
    /// Interaction logic for AlertsCenter.xaml
    /// </summary>
    public partial class AlertsCenter : UserControl
    {
        public static double temperatureSeuil { get; set; } = 24.0;

        public AlertsCenter()
        {
            InitializeComponent();

            // Alertes de température
            UpdateAlerts();
        }

        public void UpdateAlerts()
        {
            List<(string Room, float Temperature)> sensorData = GetTempAlert();
            List<(string Room, int Humidity, bool LightState)> sensorData2 = GetHumidityAndLightState();

            for (int i = 0; i < sensorData.Count; i++)
            {
                var currentEntry = sensorData[i];
                var currentEntry2 = sensorData2[i];

                TextBlock tempTextBlock = (TextBlock)FindName($"Temp_{i}");
                TextBlock roomTextBlock = (TextBlock)FindName($"Room_{i}");
                TextBlock humidityTextBlock = (TextBlock)FindName($"Humidity_{i}");
                TextBlock alertReasonTextBlock = (TextBlock)FindName($"AlertReason_{i}");

                Border border = (Border)FindName($"Border_{i}");
                border.Visibility = Visibility.Visible;


                if (tempTextBlock != null && roomTextBlock != null && humidityTextBlock != null && alertReasonTextBlock != null)
                {
                    tempTextBlock.Text = $"Temp : {currentEntry.Temperature}°C";
                    roomTextBlock.Text = currentEntry.Room;
                    humidityTextBlock.Text = $"Humidité : {(currentEntry2.Humidity != -1 ? currentEntry2.Humidity.ToString() + "%" : "N/A")}";
                    alertReasonTextBlock.Text = "Température trop élevée";
                }
            }
        }

        public static List<(string RoomName, float Temperature)> GetTempAlert()
        {
            List<(string RoomName, float Temperature)> alertData = new List<(string, float)>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection("Server=localhost;Port=3306;Database=Economitiens;User ID=root;Password=;"))
                {
                    connection.Open();

                    string tempQuery = @"
                SELECT h.ROOM_NAME, h.TEMPERATURE
                FROM HEAT h
                WHERE h.DATE_TIME_COLLECTED = (SELECT MAX(DATE_TIME_COLLECTED) FROM HEAT WHERE ROOM_NAME = h.ROOM_NAME)
                AND h.TEMPERATURE > @temperatureSeuil;
            ";

                    using (MySqlCommand command = new MySqlCommand(tempQuery, connection))
                    {
                        command.Parameters.AddWithValue("@temperatureSeuil", temperatureSeuil);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string room = reader.GetString("ROOM_NAME");
                                float temperature = reader.GetFloat("TEMPERATURE");
                                alertData.Add((room, temperature)); // Placeholder values for humidity and light state
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return alertData;
        }

        public static List<(string RoomName, int Humidity, bool LightState)> GetHumidityAndLightState()
        {
            List<(string RoomName, int Humidity, bool LightState)> alertData = new List<(string, int, bool)>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection("Server=localhost;Port=3306;Database=Economitiens;User ID=root;Password=;"))
                {
                    connection.Open();

                    // Requête pour récupérer le dernier relevé d'humidité et d'état de la lumière pour chaque salle dont la température est supérieure à 24°C
                    string query = @"
                SELECT h.ROOM_NAME, hu.HUMIDITY, lt.LIGHT_STATE
                FROM HEAT h
                LEFT JOIN HUMIDITY hu ON h.ROOM_NAME = hu.ROOM_NAME AND h.DATE_TIME_COLLECTED = hu.DATE_TIME_COLLECTED
                LEFT JOIN LIGHT lt ON h.ROOM_NAME = lt.ROOM_NAME AND h.DATE_TIME_COLLECTED = lt.DATE_TIME_COLLECTED
                WHERE h.DATE_TIME_COLLECTED = (SELECT MAX(DATE_TIME_COLLECTED) FROM HEAT WHERE ROOM_NAME = h.ROOM_NAME)
                AND h.TEMPERATURE > @temperatureSeuil;
            ";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@temperatureSeuil", temperatureSeuil);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string room = reader.GetString("ROOM_NAME");
                                int humidity = reader.IsDBNull(reader.GetOrdinal("HUMIDITY")) ? -1 : reader.GetInt32("HUMIDITY");
                                bool lightState = reader.IsDBNull(reader.GetOrdinal("LIGHT_STATE")) ? false : reader.GetBoolean("LIGHT_STATE");
                                alertData.Add((room, humidity, lightState));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return alertData;
        }
    }
}
