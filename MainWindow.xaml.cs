using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPF_WebServerClient.Helpers;
using WPF_WebServerClient.Pages;
using WPF_WebServerClient.ServerBackend;

namespace WPF_WebServerClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch sw = new Stopwatch();
        string currentTime = string.Empty;
        
        public MainWindow()
        {
            InitializeComponent();
            ControlPageFrame.NavigationService.Navigate(new Uri(PagesHelper.ControlPage, UriKind.Relative));

            App.httpServer.ServerStatusChanged += UpdateServerStatus;
           
            ResetTimer();
        }
        void dt_Tick(object sender, EventArgs e)
        {
            if (sw.IsRunning)
            {
                TimeSpan ts = sw.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours,
                ts.Minutes, ts.Seconds);
                ServerStatusTimer_Label.Text = currentTime;
            }
        }
        private void ResetTimer()
        {
            ServerStatusTimer_Label.Text = "00:00:00";
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, 1);
            sw.Reset();
            
        }
        private void UpdateServerStatus(object sender, EventArgs e)
        {
            var server = (HttpServer)sender; 

            if(server.Status == ServerStatus.Stop)
            {
                ServerStatus_Label.Text = "Stopped";
                ServerStatus_Icon.Fill = Brushes.Red;
                ControlPage.PrintToDebug($"Server works for {sw.Elapsed.Days} days, {sw.Elapsed.Minutes} hours, {sw.Elapsed.Minutes} minutes, {sw.Elapsed.Seconds} seconds");
                ResetTimer();
            }
            if (server.Status == ServerStatus.Start)
            {
                ServerStatus_Label.Text = "Started";
                ServerStatus_Icon.Fill = Brushes.Green;
                sw.Start();
                dt.Start();

            }
            ControlPage.PrintToDebug("Server " + ServerStatus_Label.Text);
        }

        public static void DisplayText(string text)
        {
           
                string caption = "Stop server";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Question;
                MessageBoxResult result;

                result = System.Windows.MessageBox.Show(text, caption, button, icon, MessageBoxResult.OK);
              
        }
        
    }
}