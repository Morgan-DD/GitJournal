using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;
using System.Windows.Shapes;
using System.Windows;
using System.Globalization;

namespace GitJournal
{

    public class API_manager
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

        public async void getAllCommits(string repoName)
        {
            string regexForHourStatus = @"\[(.*?)\]";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("C#App");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _controller._PATToken);

                HttpResponseMessage response = await client.GetAsync($"https://api.github.com/repos/{repoName}/commits");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JArray CommitsList = JArray.Parse(json); // Parse the JSON array

                    foreach (JObject commit in CommitsList)
                    {
                        string message = commit["commit"]["message"].ToString();
                        string title = "";
                        string content = "";
                        string status = "";
                        TimeSpan duration = new TimeSpan(0, 0, 0);
                        DateTime date = DateTime.ParseExact(commit["commit"]["committer"]["date"].ToString().Split(" ")[0], "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        if (message.Contains("\n\n"))
                        {
                            title = message.Split("\n\n")[0];
                            content = message.Replace((title + "\n\n"), "");

                            MatchCollection matches = Regex.Matches(content, regexForHourStatus);
                            if (matches.Count >= 2)
                            {
                                if (matches[0].ToString().Any(char.IsDigit))
                                {
                                    string durationString = matches[0].ToString().Replace("[", "").Replace("]", "").ToLower();
                                    duration = transformStringToDuration(durationString);

                                    status = matches[1].ToString().Replace("[", "").Replace("]", "");

                                }
                                else
                                {
                                    string durationString = matches[1].ToString().Replace("[", "").Replace("]", "").ToLower();
                                    duration = transformStringToDuration(durationString);

                                    status = matches[0].ToString().Replace("[", "").Replace("]", "");
                                }

                                content = string.Join(Environment.NewLine, content.Split(Environment.NewLine.ToCharArray()).Skip(1).ToArray()).ToUpper();
                            }
                        }
                        else
                            title = message;

                        _controller._JDTmanager.addNewEntry(commit["sha"].ToString(), title, content, commit["commit"]["author"]["name"].ToString(), status, duration, true, date);
                    }
                }
                else
                {
                    throw new Exception($"GitHub API request failed: {response.StatusCode}");
                }
            }
        }

        private TimeSpan transformStringToDuration(string durationSrting)
        {
            TimeSpan duration = new TimeSpan(0, 0, 0);

            if (durationSrting.Contains("h"))
            {
                if (durationSrting.Split("h").Count() > 1)
                {
                    if (durationSrting.Split("h")[1].Any(char.IsDigit))
                        duration = new TimeSpan(Convert.ToInt32(new string(durationSrting.Split("h")[0].Where(char.IsDigit).ToArray())), Convert.ToInt32(new string(durationSrting.Split("h")[1].Where(char.IsDigit).ToArray())), 0);
                    else
                        duration = new TimeSpan(Convert.ToInt32(new string(durationSrting.Split("h")[0].Where(char.IsDigit).ToArray())), 0, 0);
                }
            }
            else
            {
                duration = new TimeSpan(0, Convert.ToInt32(new string(durationSrting.Where(char.IsDigit).ToArray())), 0);
            }

            return duration;
        }
    }
}