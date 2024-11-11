using MySql.Data;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.Generic;
using Econome.Models;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MySqlConnector;
namespace Econome.Pages
{ 
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {
        private DispatcherTimer timer;
        private DispatcherTimer _timerdirect;
        private Random random;
        private DispatcherTimer refreshTimer;

        public Map()
        {
            InitializeComponent();
            random = new Random();

            // Créer et démarrer le minuteur
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(2); // Mettre à jour toutes les deux minutes
            timer.Tick += Timer_Tick;
            timer.Start();

            // Mettre à jour les températures sur les boutons au démarrage de la fenêtre
            //UpdateButtonContent();

            average_E01.Text = GetAverageTemperature("E01");
            average_E03.Text = GetAverageTemperature("E03");
            average_E05.Text = GetAverageTemperature("E05");
            average_E06.Text = GetAverageTemperature("E06");
            average_E07.Text = GetAverageTemperature("E07");
            average_E08.Text = GetAverageTemperature("E08");
            average_E09.Text = GetAverageTemperature("E09");
            _timerdirect = new DispatcherTimer();
            _timerdirect.Interval = TimeSpan.FromSeconds(1); // Met à jour chaque seconde
            _timerdirect.Tick += Timer_Direct;
            _timerdirect.Start();

            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = TimeSpan.FromSeconds(5);
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            average_E01.Text = GetAverageTemperature("E01");
            average_E03.Text = GetAverageTemperature("E03");
            average_E05.Text = GetAverageTemperature("E05");
            average_E06.Text = GetAverageTemperature("E06");
            average_E07.Text = GetAverageTemperature("E07");
            average_E08.Text = GetAverageTemperature("E08");
            average_E09.Text = GetAverageTemperature("E09");
        }

        private void Timer_Direct(object sender, EventArgs e)
        {
            // Met à jour le TextBlock avec l'heure actuelle
            ClockTextBlock.Text = DateTime.Now.ToString("HH:mm");
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Mettre à jour les températures sous chaque numéro de salle
            //UpdateButtonContent();
        }

        private void Button_Salle(object sender, RoutedEventArgs e)
        {
            string NumSalle = "ça n'a pas marché";
            Button button = sender as Button;
            if (button != null)
            {
                NumSalle = button.Name;
            }
            PopUp popup = new PopUp(NumSalle);
            popup.Title = NumSalle;
            popup.Show();
        }

        private string GetAverageTemperature(string roomName)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("Server=100froid.exp.esiea.fr;port=3306;Database=Economitiens;User ID=economitiens;Password=Password3;"))
                {
                    connection.Open();

                    string query = @"
                SELECT TEMPERATURE
                FROM HEAT
                WHERE ROOM_NAME = @RoomName AND TEMPERATURE IS NOT NULL
                ORDER BY DATE_TIME_COLLECTED DESC
                LIMIT 1;
            ";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RoomName", roomName);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                float latestTemperature = reader.GetFloat("TEMPERATURE");
                                return $"{latestTemperature:F1}°C"; // Formater la température avec 1 chiffre après la virgule
                            }
                            else
                            {
                                return "No data";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return "Error";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
        }

        public Action<PageId> NavigateToPage { get; set; }

        private void RoomInfoButton_Click(object sender, RoutedEventArgs e)
        {
             
            NavigateToPage(PageId.E); // PageId.D is assumed to be RoomInfo page in this case
        }
    }
}
