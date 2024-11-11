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

namespace Econome
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button _previousButton;
        

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainVM();

            // Initialiser le style du bouton par défaut au démarrage
            var defaultButton = this.DefaultClickedButton;
            if (defaultButton != null)
            {
                defaultButton.Style = (Style)FindResource("ClickedButtonStyle");
                _previousButton = defaultButton;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_previousButton != null)
            {
                _previousButton.Style = (Style)FindResource("NavButtons");
            }

            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                clickedButton.Style = (Style)FindResource("ClickedButtonStyle");
                _previousButton = clickedButton;
            }
        }
    }
}