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
            List<string> userRepo = new List<string>();
            string url = "https://api.github.com/user/repos?per_page=100";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("C#App");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _controller._PATToken);

                    while (!string.IsNullOrEmpty(url))
                    {
                        HttpResponseMessage response = await client.GetAsync(url);

                        if (!response.IsSuccessStatusCode)
                            throw new Exception($"GitHub API request failed: {response.StatusCode}");

                        string json = await response.Content.ReadAsStringAsync();
                        JArray repos = JArray.Parse(json);

                        foreach (JObject repo in repos)
                        {
                            userRepo.Add(repo["full_name"].ToString());
                        }

                        // Handle pagination
                        if (response.Headers.TryGetValues("Link", out IEnumerable<string> linkHeaders))
                        {
                            string linkHeader = linkHeaders.FirstOrDefault();
                            url = null; // Reset

                            if (linkHeader != null)
                            {
                                string[] links = linkHeader.Split(',');

                                foreach (string link in links)
                                {
                                    if (link.Contains("rel=\"next\""))
                                    {
                                        int start = link.IndexOf('<') + 1;
                                        int end = link.IndexOf('>');
                                        url = link.Substring(start, end - start);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            url = null;
                        }
                    }
                }
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

        public async Task<bool> getAllCommits(string repoName)
        {
            _controller._JDTmanager.importFromGitJ();

            string regexForHourStatus = @"\[(.*?)\]";
            string baseUrl = $"https://api.github.com/repos/{repoName}/commits?per_page=100";
            string url = baseUrl;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("C#App");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _controller._PATToken);

                while (!string.IsNullOrEmpty(url))
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"GitHub API request failed: {response.StatusCode}");

                    string json = await response.Content.ReadAsStringAsync();
                    JArray CommitsList = JArray.Parse(json);

                    foreach (JObject commit in CommitsList)
                    {
                        string message = commit["commit"]["message"].ToString();
                        string title = "";
                        string content = "";
                        string status = "";
                        TimeSpan duration = new TimeSpan(0, 0, 0);
                        DateTime date = DateTime.Parse(commit["commit"]["committer"]["date"].ToString(), null, DateTimeStyles.AdjustToUniversal);

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

                                content = string.Join(Environment.NewLine, content.Split(Environment.NewLine.ToCharArray()).Skip(1).ToArray());
                            }
                        }
                        else
                            title = message;

                        _controller._JDTmanager.addNewEntry(commit["sha"].ToString(), title, content, commit["commit"]["author"]["name"].ToString(), status, duration, true, date, commit["html_url"].ToString().ToLowerInvariant(), "GitHub", false);
                    }

                    // Check the Link header for next page
                    if (response.Headers.TryGetValues("Link", out IEnumerable<string> linkHeaders))
                    {
                        string linkHeader = linkHeaders.FirstOrDefault();
                        url = null; // default to null unless found

                        if (linkHeader != null)
                        {
                            string[] links = linkHeader.Split(',');

                            foreach (string link in links)
                            {
                                if (link.Contains("rel=\"next\""))
                                {
                                    int start = link.IndexOf('<') + 1;
                                    int end = link.IndexOf('>');
                                    url = link.Substring(start, end - start);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        url = null;
                    }
                }
            }

            return true;
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