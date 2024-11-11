using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Econome
{
    internal class ArduinoToDatabase
    {
        internal static void Arduino()
        {
            string connectionString = "Server=localhost;Port=3306;Database=Economitiens;User ID=root;Password=;";
            SerialPort mySerialPort = new SerialPort("COM4");

            mySerialPort.BaudRate = 9600;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = 8;
            mySerialPort.Handshake = Handshake.None;
            mySerialPort.DataReceived += new SerialDataReceivedEventHandler((sender, e) => DataReceivedHandler(sender, e, connectionString));

            try
            {
                mySerialPort.Open();
                Console.WriteLine("Listening to serial port...");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                mySerialPort.Close();
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Access to the COM port is denied.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e, string connectionString)
        {
            SerialPort sp = (SerialPort)sender;
            string inData = sp.ReadLine();
            Console.WriteLine("Données récupérées:");
            Console.WriteLine(inData);

            // Extract room name and temperature from the received data
            if (inData.Contains("Temperature:"))
            {
                string[] parts = inData.Split(' ');
                string roomName = "E01"; // Default room name, can be updated based on actual data if required
                float temperature = 0;

                foreach (string part in parts)
                {
                    if (part.Contains("C"))
                    {
                        string tempStr = part.Replace("C", "").Trim();
                        Console.WriteLine("Temperature extraites du string: '" + tempStr + "'"); // Debug message
                        if (float.TryParse(tempStr, NumberStyles.Float, CultureInfo.InvariantCulture, out temperature))
                        {
                            Console.WriteLine("Temperature convertie: " + temperature); // Debug message
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Failed to convert temperature string to float.");
                        }
                    }
                }

                if (temperature != 0)
                {
                    InsertDataToDatabase(connectionString, temperature, roomName);
                }
                else
                {
                    Console.WriteLine("Temperature invalide reçue.");
                }
            }
        }

        private static void InsertDataToDatabase(string connectionString, float temperature, string roomId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO HEAT (DATE_TIME_COLLECTED, TEMPERATURE, ROOM_NAME) VALUES (@dateTimeCollected, @temperature, @roomId)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateTimeCollected", DateTime.Now);
                        cmd.Parameters.AddWithValue("@temperature", temperature);
                        cmd.Parameters.AddWithValue("@roomId", roomId);

                        cmd.ExecuteNonQuery();
                    }

                    Console.WriteLine($"Données insérées: Temperature = {temperature}, Room Name = {roomId}");
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Database insertion error: " + ex.Message);
                }
            }
        }
    }
}

