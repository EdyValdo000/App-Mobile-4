using Microsoft.Maui.Controls;

namespace App_Mobile_4
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnLampControlClicked(object sender, EventArgs e)
        {

        }

        private void OnDisconnectClicked(object sender, EventArgs e)
        {

        }

        private void OnConnectClicked(object sender, EventArgs e)
        {

        }


/* Alteração não mesclada do projeto 'App Mobile 4 (net8.0-maccatalyst)'
Antes:
        private void OnLampTapped(object sender, EventArgs e)
        {
Após:
        private void OnLampTappedAsync(object sender, EventArgs e)
        {
*/
     
        private async void OnLampTapped(object sender, EventArgs e)
        {
            // Identifica qual lâmpada foi tocada
            var tappedCell = sender as ViewCell;
            var label = tappedCell.View as StackLayout;
            var lampLabel = label.Children[0] as Label;  // O nome da lâmpada é o primeiro Label

            // Exibe o alerta com o nome da lâmpada clicada
            await DisplayAlert("Lâmpada Selecionada", $"Você clicou na {lampLabel.Text}", "OK");

        }
    }

}
