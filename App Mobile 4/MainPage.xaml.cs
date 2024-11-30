using App_Mobile_4.Conected;
using Microsoft.Maui;

namespace App_Mobile_4;

public partial class MainPage : ContentPage
{
    Ethernet ethernet = new Ethernet();
    bool[] lampStatus = new bool[4]; // Array para controlar o estado das lâmpadas
    bool poolStatus = false; // Array para controlar o estado das lâmpadas

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnConnectClicked(object sender, EventArgs e)
    {
        try
        {
            string ip = IpTxt.Text;
            string porta = PortTxt.Text;

            // Verifica se os campos estão vazios
            if (string.IsNullOrEmpty(ip))
            {
                IpTxt.Focus(); // Foca no campo email
                return;
            }

            if (string.IsNullOrEmpty(porta))
            {
                PortTxt.Focus(); // Foca no campo senha
                return;
            }

            ethernet.conected(ip, Int16.Parse(porta));
            ethernet.send("s");

            btnConeted.IsVisible = false;
            btnDesconeted.IsVisible = true;
            IpTxt.IsEnabled = false;
            PortTxt.IsEnabled = false;


            // Inicia a leitura dos dados após a conexão
            _ = Task.Run(ReadArduinoDataAsync);
        }
        catch (Exception)
        {
            return;
        }
    }

    // Função assíncrona para ler os dados do Arduino em segundo plano
    private async Task ReadArduinoDataAsync()
    {
        while (ethernet.isConected())
        {
            try
            {
                string receber = ethernet.read();

                if (!string.IsNullOrEmpty(receber))
                {
                    string[] data = receber.Split('*');

                    // Atualiza o estado das lâmpadas na UI thread
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        UpdateLampStatus(data);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler dados: {ex.Message}");
                break; // Termina o loop em caso de erro crítico
            }

            await Task.Delay(500); // Aguarda um pouco antes da próxima leitura
        }
    }

    // Atualiza o status das lâmpadas na interface do usuário
    private void UpdateLampStatus(string[] data)
    {
        if (data.Length >= 0)
        {
            Lamp1Status.Detail = $"{data[1]} - {DateTime.Now:HH:mm dd/MM/yyyy}";
            Lamp2Status.Detail = $"{data[2]} - {DateTime.Now:HH:mm dd/MM/yyyy}";
            Lamp3Status.Detail = $"{data[3]} - {DateTime.Now:HH:mm dd/MM/yyyy}";
            Lamp4Status.Detail = $"{data[4]} - {DateTime.Now:HH:mm dd/MM/yyyy}";

            if (data[1] == "Lamp1 ON") { Lamp1Btn.Text = "Lâmpada 1 ON"; }
            if (data[1] == "Lamp1 OFF") { Lamp1Btn.Text = "Lâmpada 1 OFF"; }

            if (data[2] == "Lamp2 ON") { Lamp2Btn.Text = "Lâmpada 2 ON"; }
            if (data[2] == "Lamp2 OFF") { Lamp2Btn.Text = "Lâmpada 2 OFF"; }

            if (data[3] == "Lamp3 ON") { Lamp3Btn.Text = "Lâmpada 3 ON"; }
            if (data[3] == "Lamp3 OFF") { Lamp3Btn.Text = "Lâmpada 3 OFF"; }

            if (data[4] == "Lamp4 ON") { Lamp4Btn.Text = "Lâmpada 4 ON"; }
            if (data[4] == "Lamp4 OFF") { Lamp4Btn.Text = "Lâmpada 4 OFF"; }
        }
    }
    

    private void OnDisconnectClicked(object sender, EventArgs e)
    {
        try
        {
            ethernet.desconected();
            btnConeted.IsVisible = true;
            btnDesconeted.IsVisible = false;
            IpTxt.IsEnabled = true;
            PortTxt.IsEnabled = true;
        }
        catch
        {

        }
    }

    private void LampClicked(object sender, EventArgs e)
    {
        try
        {
            int lampIndex = int.Parse(((Button)sender).CommandParameter.ToString()) - 1;
            ToggleLamp(lampIndex);
        }
        catch
        {

        }
    }

    private void ToggleLamp(int lampIndex)
    {
        if (!ethernet.isConected())
        {
            Console.WriteLine("Não está conectado ao Arduino.");
            return;
        }

        string sendCommand = lampStatus[lampIndex] ? $"{2 * lampIndex + 2}s" : $"{2 * lampIndex + 1}s";
        ethernet.send(sendCommand);
        lampStatus[lampIndex] = !lampStatus[lampIndex]; // Alterna o estado da lâmpada
    }

    private void PoolClicked(object sender, EventArgs e)
    {
        try
        {
            TogglePool();
        }
        catch
        {

        }
    }

    private void TogglePool()
    {
        if (!ethernet.isConected())
        {
            Console.WriteLine("Não está conectado ao Arduino.");
            return;
        }


        // Lógica para quando o poolIndex for igual a 5
        if (poolStatus == false) // poolIndex será 4 porque o CommandParameter é 5 e estamos subtraindo 1
        {
            ethernet.send("9");
            poolStatus = true;
        }
        else
        {
            ethernet.send("0");
            poolStatus = false;
        }
    }
}