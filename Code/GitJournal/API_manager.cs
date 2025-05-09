using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GitJournal
{

    internal class API_manager
    {
        Controller _controller;

        public API_manager(Controller controller)
        {
            _controller = controller;
        }

        /// <summary>
        /// get the list of repositories of the users and store them
        /// </summary>
        public async Task<List<string>> getUserRepo()
        {
            Debug.WriteLine(_controller._PATToken);
            List<string> userRepo = new List<string>();
            string json;
            try
            {
                // retrive all the repositories
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("C#App");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _controller._PATToken);

                    HttpResponseMessage response = await client.GetAsync("https://api.github.com/user/repos");

                    if (response.IsSuccessStatusCode)
                    {
                        json = await response.Content.ReadAsStringAsync();
                        JArray repos = JArray.Parse(json); // Parse the JSON array

                        foreach (JObject repo in repos)
                        {
                            userRepo.Add(repo["full_name"].ToString());
                        }
                    }
                    else
                    {
                        throw new Exception($"GitHub API request failed: {response.StatusCode}");
                    }
                }
                // add the repos to the list of repositories and display them into their menu
                // _RepoList.updateRepoList(_ClientRepositories);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            return userRepo;
        }

        public async Task<List<string>> loadUserFromRepo(string repoName)
        {
            List<string> RepoUsersList = new List<string>();
            try
            {
                // retrive all the repositories
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("C#App");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _controller._PATToken);

                    HttpResponseMessage response = await client.GetAsync($"https://api.github.com/repos/{repoName}/collaborators");

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        JArray usersList = JArray.Parse(json); // Parse the JSON array


                        foreach (JObject user in usersList)
                        {
                            RepoUsersList.Add(user["login"]?.ToString());
                        }
                        
                    }
                    else
                    {
                        throw new Exception($"GitHub API request failed: {response.StatusCode}");
                    }
                }
                // add the repos to the list of repositories and display them into their menu
                // _RepoList.updateRepoList(_ClientRepositories);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            return RepoUsersList;
        }
    }
}
