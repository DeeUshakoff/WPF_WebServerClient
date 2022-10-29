using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WPF_WebServerClient.ServerBackend
{

    public class HttpServer : IDisposable
    {
        public ServerStatus Status = ServerStatus.Stop;
        private ServerSetting _serverSetting = new ServerSetting();
        private readonly HttpListener _httpListener;

        public event EventHandler ServerStatusChanged;
        public event EventHandler PrefixAdded;

        public HttpListenerPrefixCollection Prefixes => _httpListener.Prefixes;
        public ServerSetting ServerSetting => _serverSetting;
        public HttpServer()
        {
            _httpListener = new HttpListener();
            
            
        }
        public void Initialize()
        {
            DisplayPrefix(_serverSetting.Link, _serverSetting.Port);
        }
        public void DisplayPrefix(string link, uint port)
        {
            _httpListener.Prefixes.Add(link + _serverSetting.Port + "/");
            PrefixAdded?.Invoke(link + _serverSetting.Port + "/", new EventArgs());
        }

        public void Start()
        {
            if (Status == ServerStatus.Start)
            {
                return;
            }

            
            _httpListener.Start();
            
            Status = ServerStatus.Start;
            Listening();
            ServerStatusChanged?.Invoke(this, new EventArgs());
        }

        public void Stop()
        {
            if (Status == ServerStatus.Stop) return;
         
            _httpListener.Stop();
            Status = ServerStatus.Stop;
            
            ServerStatusChanged?.Invoke(this, new EventArgs());
        }
       
        private void Listening()
        {
            _httpListener.BeginGetContext(ListenerCallBack, _httpListener);
        }

        private void ListenerCallBack(IAsyncResult result)
        {
            if (!_httpListener.IsListening) return;

            var _httpContext = _httpListener.EndGetContext(result);
            HttpListenerRequest request = _httpContext.Request;
            HttpListenerResponse response = _httpContext.Response;
            //Console.WriteLine($"Directory " + _serverSetting.Path);
            byte[] buffer;
            if (Directory.Exists(_serverSetting.Path))
            {
                buffer = FileManager.GetFile(_httpContext.Request.RawUrl.Replace("%20", " "), _serverSetting);

                if (buffer == null)
                {
                    response.Headers.Set("Content-Type", "text/plain");
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    string err = "404 - not found";
                    buffer = Encoding.UTF8.GetBytes(err);
                }
            }
            else
            {
                string err = $"Directory " + _serverSetting.Path + " not found";
                buffer = Encoding.UTF8.GetBytes(err);
            }

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            Listening();
        }

        public void Dispose()
        {
            Stop();
        }
    }

    
}
