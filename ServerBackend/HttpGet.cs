using System;

namespace WPF_WebServerClient.ServerBackend;

internal class HttpGET : Attribute
{
    public string MethodURI;

    public HttpGET(string methodUri)
    {
        MethodURI = methodUri;
    }
}