using System;

namespace WPF_WebServerClient.ServerBackend;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class HttpController : Attribute
{
    public string ControllerName;

    public HttpController(string controllerName)
    {
        ControllerName = controllerName;
    }
}