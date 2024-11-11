using System.Windows;
using System.Windows.Controls;

namespace Econome.Pages
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : UserControl
    {
        AlertsCenter alertsCenter = new AlertsCenter();

        public Setting()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            AlertsCenter.temperatureSeuil = TemperatureSlider.Value;
            alertsCenter.UpdateAlerts();

            MessageBox.Show($"Les alertes seront maintenant déclanchées à partir de : {AlertsCenter.temperatureSeuil} °C");
        }
    }
}