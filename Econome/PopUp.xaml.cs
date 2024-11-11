using System;
using System.Collections.Generic;
using System.Windows;
using MySqlConnector;
using LiveCharts.Defaults;
using LiveCharts;
using System.Threading.Tasks;
using System.Windows.Media;
using LiveCharts.Wpf;
using Separator = LiveCharts.Wpf.Separator;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Econome
{
    /// <summary>
    /// Logique d'interaction pour PopUp.xaml
    /// </summary>
    public partial class PopUp : Window
    {
        string NumSalle;
        private DispatcherTimer refreshTimer;
        private static DateTime today = DateTime.Now.Date;
        private static DateTime yesterday = today.AddDays(-1);

        private readonly ChartValues<ObservablePoint> temperatureValues = new ChartValues<ObservablePoint>();
        private readonly ChartValues<ObservablePoint> humidityValues = new ChartValues<ObservablePoint>();
        private readonly string connectionString = "Server=100froid.exp.esiea.fr;Port=3306;Database=Economitiens;User ID=economitiens;Password=Password3;";
        private readonly DispatcherTimer timer = new DispatcherTimer();

        public PopUp(string _NumSalle)
        {
            InitializeComponent();
            NumSalle = _NumSalle;

            roomName.Text = _NumSalle;

            PrintDataForDay(NumSalle, today, Temp1_1, Temp1_2, Temp1_3, Humidity1_1, Humidity1_2, Humidity1_3);

            PrintDataForDay(NumSalle, yesterday, Temp2_1, Temp2_2, Temp2_3, Humidity2_1, Humidity2_2, Humidity2_3);


            date1.Text = today.ToString("dd/MM/yyyy");
            date2.Text = yesterday.ToString("dd/MM/yyyy");
            date3.Text = today.ToString("dd/MM/yyyy");

            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = TimeSpan.FromSeconds(10);
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();

            SetupChart();
            StartTimer();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            PrintDataForDay(NumSalle, today, Temp1_1, Temp1_2, Temp1_3, Humidity1_1, Humidity1_2, Humidity1_3);
        }

        private void PrintDataForDay(string roomName, DateTime date, TextBlock temp1, TextBlock temp2, TextBlock temp3, TextBlock humidity1, TextBlock humidity2, TextBlock humidity3)
        {
            string[] tempTab = GetTemperatureData(roomName, date);
            temp1.Text = tempTab[0];
            temp2.Text = tempTab[1];
            temp3.Text = tempTab[2];

            string[] humidityTab = GetHumidityData(roomName, date);
            humidity1.Text = humidityTab[0];
            humidity2.Text = humidityTab[1];
            humidity3.Text = humidityTab[2];
        }

        internal static string[] GetTemperatureData(string roomName, DateTime fromDate)
        {
            string connectionString = "Server=100froid.exp.esiea.fr;Port=3306;Database=Economitiens;User ID=economitiens;Password=Password3;";
            string[] temperatureData = new string[3];

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                WITH RankedHeat AS (
                    SELECT h.ROOM_NAME, h.DATE_TIME_COLLECTED, h.TEMPERATURE,
                           ROW_NUMBER() OVER (PARTITION BY h.ROOM_NAME ORDER BY h.DATE_TIME_COLLECTED DESC) AS rn
                    FROM HEAT h
                    WHERE h.ROOM_NAME = @RoomName AND DATE(h.DATE_TIME_COLLECTED) = @FromDate AND h.TEMPERATURE IS NOT NULL
                )
                SELECT ROOM_NAME, DATE_TIME_COLLECTED, TEMPERATURE
                FROM RankedHeat
                WHERE rn <= 3
                ORDER BY DATE_TIME_COLLECTED DESC;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomName", roomName);
                    command.Parameters.AddWithValue("@FromDate", fromDate);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        int index = 0;
                        while (reader.Read() && index < 3)
                        {
                            DateTime dateTimeCollected = (DateTime)reader["DATE_TIME_COLLECTED"];
                            string formattedDateTime = dateTimeCollected.ToString("HH:mm");
                            float temperature = Convert.ToSingle(reader["TEMPERATURE"]);

                            temperatureData[index] = $"{formattedDateTime}                                           {temperature:0.0}°C";
                            index++;
                        }

                        for (; index < 3; index++)
                        {
                            temperatureData[index] = "Moins de 3 relevés ce jour.";
                        }
                    }
                }
            }

            return temperatureData;
        }

        internal static string[] GetHumidityData(string roomName, DateTime fromDate)
        {
            string[] humidityData = new string[3];
            string connectionString = "Server=100froid.exp.esiea.fr;Port=3306;Database=Economitiens;User ID=economitiens;Password=Password3;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                WITH RankedHumidity AS (
                    SELECT h.ROOM_NAME, h.DATE_TIME_COLLECTED, h.HUMIDITY,
                           ROW_NUMBER() OVER (PARTITION BY h.ROOM_NAME ORDER BY h.DATE_TIME_COLLECTED DESC) AS rn
                    FROM HUMIDITY h
                    WHERE h.ROOM_NAME = @RoomName AND DATE(h.DATE_TIME_COLLECTED) = @FromDate AND h.HUMIDITY IS NOT NULL
                )
                SELECT ROOM_NAME, DATE_TIME_COLLECTED, HUMIDITY
                FROM RankedHumidity
                WHERE rn <= 3
                ORDER BY DATE_TIME_COLLECTED DESC;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomName", roomName);
                    command.Parameters.AddWithValue("@FromDate", fromDate);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        int index = 0;
                        while (reader.Read() && index < 3)
                        {
                            DateTime dateTimeCollected = (DateTime)reader["DATE_TIME_COLLECTED"];
                            string formattedDateTime = dateTimeCollected.ToString("HH:mm");
                            float humidity = Convert.ToSingle(reader["HUMIDITY"]);

                            humidityData[index] = $"{formattedDateTime}                                           {humidity:0.0}%";
                            index++;
                        }

                        for (; index < 3; index++)
                        {
                            humidityData[index] = "Moins de 3 relevés ce jour.";
                        }
                    }
                }
            }

            return humidityData;
        }

        private void SetupChart()
        {
            Chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Température",
                    Values = temperatureValues,
                    Fill = Brushes.Transparent,
                    PointGeometry = null // Optional: Hide data points markers
                },
                new LineSeries
                {
                    Title = "Humidité",
                    Values = humidityValues,
                    Fill = Brushes.Transparent,
                    PointGeometry = null // Optional: Hide data points markers
                }
            };

            Chart.AxisX.Add(new Axis
            {
                Title = "Temps",
                LabelFormatter = value => new DateTime((long)value).ToString("HH:mm"),
                Separator = new Separator
                {
                    Step = TimeSpan.FromSeconds(10).Ticks, // Set tick step interval to 5 seconds
                    IsEnabled = true
                },
                MinValue = DateTime.Now.Ticks,
                MaxValue = DateTime.Now.AddMinutes(2).Ticks // Adjust this range as needed
            });

            Chart.AxisY.Add(new Axis
            {
                Title = "Température et Humidité"
            });
        }

        private void StartTimer()
        {
            timer.Interval = TimeSpan.FromSeconds(1); // Update every 5 seconds
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            await UpdateChartData();
        }

        private async Task UpdateChartData()
        {
            var temperatureQuery = $"SELECT TEMPERATURE FROM HEAT WHERE ROOM_NAME = '{NumSalle}' ORDER BY DATE_TIME_COLLECTED DESC LIMIT 1";
            var humidityQuery = $"SELECT HUMIDITY FROM HUMIDITY WHERE ROOM_NAME = '{NumSalle}' ORDER BY DATE_TIME_COLLECTED DESC LIMIT 1";

            var temperatureData = await GetDataFromDatabaseAsync<int>(temperatureQuery);
            var humidityData = await GetDataFromDatabaseAsync<int>(humidityQuery);

            var currentTime = DateTime.Now;

            if (temperatureData != null)
            {
                temperatureValues.Add(new ObservablePoint(currentTime.Ticks, temperatureData));
                if (temperatureValues.Count > 50) // Limit the number of points on the chart
                {
                    temperatureValues.RemoveAt(0);
                }
            }

            if (humidityData != null)
            {
                humidityValues.Add(new ObservablePoint(currentTime.Ticks, humidityData));
                if (humidityValues.Count > 50) // Limit the number of points on the chart
                {
                    humidityValues.RemoveAt(0);
                }
            }

            // Update X-axis range to keep the most recent values visible
            var minValue = currentTime.AddMinutes(-1).Ticks; // Display last 1 minute of data
            var maxValue = currentTime.Ticks;
            Chart.AxisX[0].MinValue = minValue;
            Chart.AxisX[0].MaxValue = maxValue;
        }

        private async Task<T> GetDataFromDatabaseAsync<T>(string query)
        {
            T result = default(T);
            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (typeof(T) == typeof(float))
                            {
                                result = (T)(object)reader.GetInt32(0);
                            }
                            // Add other data types as needed
                        }
                    }
                }
            }
            return result;
        }
    }
}

