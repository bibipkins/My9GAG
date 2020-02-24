using My9GAG.Logic.DownloadManager;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.IO;
using System.Net;
using Xamarin.Forms;

[assembly: Dependency(typeof(My9GAG.Droid.Logic.DownloadManager))]

namespace My9GAG.Droid.Logic
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

            using (var webClient = new WebClient())
            {
                webClient.DownloadDataCompleted += (s, e) =>
                {
                    var bytes = e.Result;
                    var directoryPath = Android.OS.Environment.GetExternalStoragePublicDirectory(
                        Android.OS.Environment.DirectoryPictures) + "/" + DEFAULT_FOLDER_NAME;

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    string path = Path.Combine(directoryPath, filename);
                    File.WriteAllBytes(path, bytes);
                };

                webClient.DownloadDataAsync(new Uri(url));
            }
        }

        private const string DEFAULT_FOLDER_NAME = "9GAG";
    }
}
