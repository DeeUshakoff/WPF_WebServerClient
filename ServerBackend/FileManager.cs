using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_WebServerClient.ServerBackend
{
    internal static class FileManager
    {
        public static byte[] GetFile(string rawUrl, ServerSetting serverSetting)
        {
            byte[] buffer = null;
            var filePath = serverSetting.Path + rawUrl;

            if (Directory.Exists(filePath))
            {
                filePath += "index.html";

                if (File.Exists(filePath))
                {
                    buffer = File.ReadAllBytes(filePath);
                }
            }
            else if (File.Exists(filePath))
                buffer = File.ReadAllBytes(filePath);

            return buffer;
        }


    }
}
