using System.Windows;

namespace Econome.Models
{
    public partial class ErrorWindow : Window
    {
        public ErrorWindow(string errorMessage)
        {
            InitializeComponent();
            ErrorMessageTextBlock.Text = errorMessage;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Ferme la fenêtre d'erreur
        }
    }
}
