using System.Configuration;
using System.Data;
using System.Windows;

namespace E_conomitiens
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void startApp(object sender, StartupEventArgs e) { 
            MainWindow window = new MainWindow();   
            window.Show();

        }
    }

}
