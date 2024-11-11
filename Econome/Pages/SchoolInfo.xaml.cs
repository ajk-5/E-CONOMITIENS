using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using LiveCharts.Defaults;
using System.Windows.Media;
using System.Windows.Threading;
using Econome.Models;

namespace Econome.Pages
{
    /// <summary>
    /// Interaction logic for SchoolInfo.xaml
    /// </summary>
    public partial class SchoolInfo : UserControl
    {
            private ColumnSeries JourSeries;
            private ColumnSeries SemaineSeries;
            private ColumnSeries MoisSeries;
            private ColumnSeries AnneeSeries;
            private DispatcherTimer refreshTimer;

            public ChartValues<ObservableValue> JourData { get; set; }
            public ChartValues<ObservableValue> SemaineData { get; set; }
            public ChartValues<ObservableValue> MoisData { get; set; }
            public ChartValues<ObservableValue> AnneeData { get; set; }

            private List<ColumnSeries> allSeries;

            private string connectionString = "server=10.13.0.36;database=Economitiens;user=economitiens;password=password!;";

            public SchoolInfo()
            {
                InitializeComponent();
                Loaded += MainWindow_Loaded;

                refreshTimer = new DispatcherTimer();
                refreshTimer.Interval = TimeSpan.FromSeconds(10);
                refreshTimer.Tick += RefreshTimer_Tick;
                refreshTimer.Start();
            }

            private void RefreshTimer_Tick(object sender, EventArgs e)
            {
                RefreshData();
            }

            private void RefreshData()
            {
                var jourData = InfoTime.GetMoyenneJour(connectionString);
                var semaineData = InfoTime.GetMoyenneSemaine(connectionString);
                var moisData = InfoTime.GetMoyenneMois(connectionString);
                var anneeData = InfoTime.GetMoyenneAnnee(connectionString);

                JourData.Clear();
                SemaineData.Clear();
                MoisData.Clear();
                AnneeData.Clear();

                foreach (var kvp in jourData)
                {
                    JourData.Add(new ObservableValue(kvp.Value));
                }

                foreach (var kvp in semaineData)
                {
                    SemaineData.Add(new ObservableValue(kvp.Value));
                }

                foreach (var kvp in moisData)
                {
                    MoisData.Add(new ObservableValue(kvp.Value));
                }

                foreach (var kvp in anneeData)
                {
                    AnneeData.Add(new ObservableValue(kvp.Value));
                }

                // Optionally update the chart to ensure it refreshes
                cartesianChart.Update(true, true);
            }

            private void MainWindow_Loaded(object sender, RoutedEventArgs e)
            {

                var jourData = InfoTime.GetMoyenneJour(connectionString);
                var semaineData = InfoTime.GetMoyenneSemaine(connectionString);
                var moisData = InfoTime.GetMoyenneMois(connectionString);
                var anneeData = InfoTime.GetMoyenneAnnee(connectionString);

                JourData = new ChartValues<ObservableValue>();
                SemaineData = new ChartValues<ObservableValue>();
                MoisData = new ChartValues<ObservableValue>();
                AnneeData = new ChartValues<ObservableValue>();

                foreach (var kvp in jourData)
                {
                    JourData.Add(new ObservableValue(kvp.Value));
                }

                foreach (var kvp in semaineData)
                {
                    SemaineData.Add(new ObservableValue(kvp.Value));
                }

                foreach (var kvp in moisData)
                {
                    MoisData.Add(new ObservableValue(kvp.Value));
                }

                foreach (var kvp in anneeData)
                {
                    AnneeData.Add(new ObservableValue(kvp.Value));
                }

                JourSeries = new ColumnSeries
                {
                    Title = "Jour",
                    Values = JourData,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34126F"))
                };

                SemaineSeries = new ColumnSeries
                {
                    Title = "Semaine",
                    Values = SemaineData,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4B0268"))
                };

                MoisSeries = new ColumnSeries
                {
                    Title = "Mois",
                    Values = MoisData,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#09076D"))
                };

                AnneeSeries = new ColumnSeries
                {
                    Title = "Année",
                    Values = AnneeData,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A056B"))
                };
                DataContext = this;
            }

            private void TogglePeriod(object sender, RoutedEventArgs e)
            {
                if (cartesianChart == null || cartesianChart.Series == null) return;

                // Réinitialiser la collection de séries
                cartesianChart.Series.Clear();

                // Ajouter les séries en fonction de l'état actuel des CheckBoxes
                if (JourCheckBox.IsChecked == true)
                {
                    cartesianChart.Series.Add(JourSeries);
                }
                if (SemaineCheckBox.IsChecked == true)
                {
                    cartesianChart.Series.Add(SemaineSeries);
                }
                if (MoisCheckBox.IsChecked == true)
                {
                    cartesianChart.Series.Add(MoisSeries);
                }
                if (AnneeCheckBox.IsChecked == true)
                {
                    cartesianChart.Series.Add(AnneeSeries);
                }

                // Mettre à jour le graphique
                cartesianChart.Update(true, true);
            }
        }
    }