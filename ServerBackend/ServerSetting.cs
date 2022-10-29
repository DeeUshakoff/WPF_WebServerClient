using System;
using System.Windows.Forms;

namespace WPF_WebServerClient.ServerBackend
{
    public class ServerSetting
    {
        private uint port = 7700;
        public uint Port
        {
            get => port;
            set
            {
                port = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }
        public readonly uint DefaultPort  = 7700;

        private string link = "http://localhost:";
        public string Link
        {
            get => link;
            set
            {
                link = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }
        public readonly string DefaultLink = "http://localhost:";

        public string Path { get; set; } = @"./Homework/2nd course/hw_3 http server v2/site";
        public readonly string DefaultPath  = @"./Homework/2nd course/hw_3 http server v2/site";

        public event EventHandler SettingsChanged;
        
        public void Initialize()
        {
            Port = DefaultPort;
            Link = DefaultLink;
        }
    }

    
}
