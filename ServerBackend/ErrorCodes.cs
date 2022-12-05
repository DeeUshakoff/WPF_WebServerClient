using System.Collections.Generic;

namespace WPF_WebServerClient.ServerBackend;

public static class ErrorCodes
{
    private static Dictionary<int, string> ErrorCodesDictionary = new()
    {
        { 404, "not found" }
    };

    public static string GetErrorExplanation(int key) =>
        ErrorCodesDictionary.ContainsKey(key) ? ErrorCodesDictionary[key] : "unnamed error";
}
