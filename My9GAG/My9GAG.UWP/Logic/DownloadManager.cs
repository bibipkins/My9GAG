using My9GAG.Logic.DownloadManager;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.IO;
using System.Net;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(My9GAG.UWP.Logic.DownloadManager))]

namespace My9GAG.UWP.Logic
{
    public class DownloadManager : IDownloadManager
    {
        public async void DownloadFile(string url, string filename)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (status != PermissionStatus.Granted)
            {
                await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage);
                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);

                if (results.ContainsKey(Permission.Storage))
                {
                    status = results[Permission.Storage];
                }
            }

            var picturesFolder = KnownFolders.PicturesLibrary;
            var gagFolder = await picturesFolder.CreateFolderAsync(DEFAULT_FOLDER_NAME, CreationCollisionOption.OpenIfExists);
            var file = await gagFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            var filestream = await file.OpenStreamForWriteAsync();

            using (var webClient = new WebClient())
            {
                webClient.DownloadDataCompleted += (s, e) =>
                {
                    var bytes = e.Result;
                    filestream.Write(bytes, 0, bytes.Length);
                    filestream.Close();
                };

                webClient.DownloadDataAsync(new Uri(url));
            }
        }

        private const string DEFAULT_FOLDER_NAME = "9GAG";
    }
}
