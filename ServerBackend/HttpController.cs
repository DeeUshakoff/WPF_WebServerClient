using System;

namespace WPF_WebServerClient.ServerBackend;

public class HttpController : Attribute
{
    public string ControllerName;

    public HttpController(string controllerName)
    {
        ControllerName = controllerName;
    }
}