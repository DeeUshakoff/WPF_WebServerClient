using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WPF_WebServerClient.Pages;

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

        private async void Listening()
        {
            
            while (_httpListener.IsListening)
            {
                if (Status == ServerStatus.Stop)
                    return;
               
                try
                {
                    var _httpContext = await _httpListener.GetContextAsync();

                    if (MethodHandler(_httpContext)) return;

                    StaticFiles(_httpContext.Request, _httpContext.Response);
                }
                catch (System.Net.HttpListenerException) { }
            }
        }
        private void StaticFiles(HttpListenerRequest request, HttpListenerResponse response)
        {
            byte[] buffer;
            if (Directory.Exists(_serverSetting.Path))
            {
                buffer = getFile(request.RawUrl.Replace("%20", " "));
                

                if (buffer == null)
                {
                    response.Headers.Set("Content-Type", "text/plain");
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    string err = "not found - 404";
                    buffer = Encoding.UTF8.GetBytes(err);
                }

            }
            else
            {
                string err = $"Directory '{_serverSetting.Path}' not found";
                buffer = Encoding.UTF8.GetBytes(err);
            }
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
        private void ListenerCallBack(IAsyncResult result)
        {
            if (_httpListener.IsListening)
            {
                var _httpContext = _httpListener.EndGetContext(result);
                HttpListenerRequest request = _httpContext.Request;
                HttpListenerResponse response = _httpContext.Response;
                byte[] buffer;
                if (Directory.Exists(_serverSetting.Path))
                {
                    buffer = FileManager.GetFile(_httpContext.Request.RawUrl.Replace("%20", " "), _serverSetting);

                    if (buffer != null)
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
        }
        private bool MethodHandler(HttpListenerContext _httpContext)
        {
            HttpListenerRequest request = _httpContext.Request;

            HttpListenerResponse response = _httpContext.Response;

            if (_httpContext.Request.Url.Segments.Length < 2) return false;

            string controllerName = _httpContext.Request.Url.Segments[1].Replace("/", "");

            string[] strParams = _httpContext.Request.Url
                                    .Segments
                                    .Skip(2)
                                    .Select(s => s.Replace("/", ""))
                                    .ToArray();

            var assembly = Assembly.GetExecutingAssembly();

            var controller = assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(HttpController))).FirstOrDefault(c => c.Name.ToLower() == controllerName.ToLower());

            if (controller == null) return false;

            var test = typeof(HttpController).Name;
            var method = controller.GetMethods().Where(t => t.GetCustomAttributes(true)
                                                              .Any(attr => attr.GetType().Name == $"Http{_httpContext.Request.HttpMethod}"))
                                                 .FirstOrDefault();

            if (method == null) return false;

            object[] queryParams = method.GetParameters()
                                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                .ToArray();

            var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);

            response.ContentType = "Application/json";

            byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();

            return true;
        }
        private byte[] getFile(string rawUrl)
        {
            byte[] buffer = null;
            var filePath = _serverSetting.Path + rawUrl.Replace('/','\\');

            if (Directory.Exists(filePath))
            {
                
                filePath += "\\index.html";

                if (File.Exists(filePath))
                {
                   
                    buffer = File.ReadAllBytes(filePath);
                }

            }
            else if (File.Exists(filePath))
                buffer = File.ReadAllBytes(filePath);

            
            return buffer;
        }
        public void Dispose()
        {
            Stop();
        }
    }

    
}
