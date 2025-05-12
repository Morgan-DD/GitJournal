using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Windows;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Net;
using System.Windows.Media.Imaging;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing.Imaging;

namespace GitJournal
{
    internal class PAT_manager
    {
        static Controller _controller;

        static string _FileName = "GitJournal.Debug";

        public PAT_manager(Controller controller)
        {
            _controller = controller;
        }

        /// <summary>
        /// retrive data from a file and set them into variables
        /// </summary>
        public async Task<string> TokenRetriving()
        {
            string returnValue = String.Empty;
            // file where the user token is stored
            // if not exist create it else take it and try

            // create the folder if not exist
            FileInfo file = new FileInfo(_FileName);
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            // if the file dont exist, create
            if (!file.Exists)
            {
                file.Create().Close();
            }
            else
            {
                // if the file exist, retrive the data
                returnValue = File.ReadAllText(file.FullName);
                // retrive the token and test it, if he is valid we save it, otherwise we dont save it and the user will have to give it again
                if (!await CheckTokenAsync(returnValue))
                {
                    returnValue = String.Empty;
                    File.WriteAllText(_FileName, returnValue);
                }
            }

            return returnValue;
        }

        public async Task<string> AskForPAT()
        {
            string returnValue = String.Empty;
            // answer of the user
            returnValue = Interaction.InputBox("Veuillez rensegnier votre clé PAT", "Clé PAT", "github_pat_jkasdkjasbd....");
            if (returnValue.Length > 0)
            {
                
                // check of the token
                if (await CheckTokenAsync(returnValue))
                {
                    // if ok we save it and display the name of the user and notify the user
                    MessageBox.Show("Clé valide");
                    File.WriteAllText(_FileName, returnValue);
                }
                else
                {
                    MessageBox.Show("Clé invalide");
                    returnValue = String.Empty;
                    File.WriteAllText(_FileName, returnValue);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// test the validity of the token
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckTokenAsync(string PAT)
        {
            if (await GetGitHubOwnerName(PAT) != "False Key")
            {
                // getUserIcon();
                return true;
            }
            return false;
        }


        /// <summary>
        /// get the name of the github user (used to test token)
        /// </summary>
        /// <returns>the name of the user if that worked, else return "false key"</returns>
        static async Task<string> GetGitHubOwnerName(string PAT)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("C#App");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", PAT);

                    HttpResponseMessage response = await client.GetAsync("https://api.github.com/user");

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        using JsonDocument doc = JsonDocument.Parse(json);
                        _controller._ClientName = doc.RootElement.GetProperty("login").GetString();
                        await getUserIcon(PAT);
                        return "Success";
                    }
                    else
                    {
                        throw new Exception($"GitHub API request failed: {response.StatusCode}");
                    }
                }
            }
            catch
            {
                return "False Key";
            }
        }

        /// <summary>
        /// get the icon of the client DONT WORK !!!!
        /// </summary>
        static async Task getUserIcon(string PAT)
        {
            // url of the icon
            string clientAvatarUrl = "";

            using (HttpClient client = new HttpClient())
            {
                // config of the client, we use the token cause the image might be on a private repo so we need the token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", PAT);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("C# App");

                // url of the image
                string url = $"https://api.github.com/users/{_controller._ClientName}";

                HttpResponseMessage response = await client.GetAsync(url);
                string ClientData = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(ClientData);
                clientAvatarUrl = json["avatar_url"].ToString();

                WebClient Webclient = new WebClient();
                Stream stream = Webclient.OpenRead(clientAvatarUrl);
                Bitmap tempBitmap = new Bitmap(stream);

                using (MemoryStream memory = new MemoryStream())
                {
                    // Save the image into the ram and save it into a variable as a bitmap
                    tempBitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze(); // Optional, makes it cross-thread usable

                    _controller._ClientAvatar = bitmapImage;
                }

                stream.Flush();
                stream.Close();

                client.Dispose();
            }
        }
    }
}
