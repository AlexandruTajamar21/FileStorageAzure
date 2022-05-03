using Azure.Storage.Files.Shares;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileStorageAzure.Services
{
    public class ServiceStorageFile
    {
        private ShareDirectoryClient Root;

        public ServiceStorageFile(string keys)
        {
            ShareClient client = new ShareClient(keys, "ficheros");
            this.Root = client.GetRootDirectoryClient();
        }

        public async Task<List<string>> GetFilesAsync()
        {
            List<string> ficheros = new List<string>();
            await foreach (var file in this.Root.GetFilesAndDirectoriesAsync())
            {
                ficheros.Add(file.Name);
            }
            return ficheros;
        }

        public async Task<string> ReadFileAsync(string fileName)
        {
            ShareFileClient file = this.Root.GetFileClient(fileName);
            var data = await file.DownloadAsync();
            Stream stream = data.Value.Content;
            using (StreamReader reader = new StreamReader(stream))
            {
                string content = await reader.ReadToEndAsync();
                return content;
            }
        }

        public async Task UploadFileAsync(string fileName, Stream stream)
        {
            ShareFileClient file = this.Root.GetFileClient(fileName);
            await file.CreateAsync(stream.Length);
            await file.UploadAsync(stream);
        }

        public async Task DeleteFileAsync(string fileName)
        {
            ShareFileClient file = this.Root.GetFileClient(fileName);
            await file.DeleteAsync();
        }
    }
}
