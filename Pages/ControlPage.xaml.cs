using System;
using System.Collections.Generic;
using System.Data;
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
using WPF_WebServerClient.ServerBackend;

namespace WPF_WebServerClient.Pages
{
    /// <summary>
    /// Логика взаимодействия для ControlPage.xaml
    /// </summary>
    public partial class ControlPage : Page
    {
        private readonly HttpServer server;
        public static ListBox OutputList;
        public ControlPage()
        {
            InitializeComponent();
            server = App.httpServer;

            server.ServerStatusChanged += ChangeStartStopButtonContent;
            server.PrefixAdded += AddPrefixToList;
            server.ServerSetting.SettingsChanged += DisplayLinkPort;
            ControlPage.OutputList = DebugList;

            

            server.Initialize();
            server.ServerSetting.Initialize();
        }

        private void StartStop_Button_Click(object sender, RoutedEventArgs e)
        {

            if(server.Status == ServerStatus.Start)
            {
                string messageBoxText = "Do you want to stop the server?";
                string caption = "Stop server";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Question;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                if (result != MessageBoxResult.Yes)
                    return;
                server.Stop();
            }
            else
            {
                server.Start();
            }
        }
        private void ChangeStartStopButtonContent(object sender, EventArgs e)
        {
            if(server.Status == ServerStatus.Start)
            {
                StartStop_Button.Content = "Stop";
            }
            else if(server.Status == ServerStatus.Stop)
            {
                StartStop_Button.Content = "Start";
            }
        }
        private void Restart_Button_Click(object sender, RoutedEventArgs e)
        {
            server.Stop();
            server.Start();
        }
        
        public static void PrintToDebug(string input)
        {
            OutputList.Items.Add($"[{DateTime.Now}] {input}");
            //(OutputList.Items[OutputList.Items.Count-1] as MenuItem).Command 
        }

        private void ClearDebugList_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Do you want to clear status list?";
            string caption = "Clear status list";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            MessageBoxResult result;

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            if (result != MessageBoxResult.Yes)
                return;
            DebugList.Items.Clear();
        }

        private void SiteDirectoryPick_Button_Click(object sender, RoutedEventArgs e)
        {
            var dp = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select .Site root folder"
            };

            dp.ShowDialog();
            if (string.IsNullOrWhiteSpace(dp.SelectedPath))
                return;
            SiteDirectory_TextBox.Text = dp.SelectedPath;
        }

        private void AddPrefixToList(object sender, EventArgs e)
        {
            PrefixesList.Items.Add((string)sender);
        }
        private void DisplayLinkPort(object sender, EventArgs e)
        {
            
            Link_TextBox.Text = (sender as ServerSetting).Link;
            Port_TextBox.Text = (sender as ServerSetting).Port.ToString();
        }
    }
}
