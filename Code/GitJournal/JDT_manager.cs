using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace GitJournal
{
    public class JDT_manager
    {
        Controller _controller;
        public List<Commit_Info> _commits { get; set; }

        public JDT_manager(Controller controller)
        {
            _controller = controller;
            _commits = new List<Commit_Info>();
        }

        public void addNewEntry(string CommitId, string title, string content, string user, string status, TimeSpan Duration, bool ExistingStatus, DateTime Date, string url, string origin, bool IsTitlemodifed)
        {
            var existing = _commits.FirstOrDefault(c => c.CommitId == CommitId);

            var newCommit = new Commit_Info
            {
                CommitId = CommitId,
                Title = title,
                Content = content,
                User = user,
                Status = status.ToUpper(),
                Duration = Duration,
                ExistingStatus = ExistingStatus,
                Date = Date,
                Url = url,
                Origin = origin,
                IsTitleModifed = IsTitlemodifed,
                IsContentModifed = false,
                IsUserModified = false,
                IsStatusModifed = false,
                IsTDurationModifed = false
            };

            if (existing != null)
            {
                var existingOrigin = existing.Origin?.ToLower();
                var newOrigin = origin?.ToLower();

                if (existingOrigin == "gitj" && newOrigin != "gitj")
                {
                    // Keep existing, do not add new
                    return;
                }
                else
                {
                    // Replace existing
                    _commits.Remove(existing);
                }
            }

            _commits.Add(newCommit);
        }


        public void clearCommits()
        {
            _commits.Clear();
        }

        public void sortByDate()
        {
            _commits.Sort((a, b) => b.Date.CompareTo(a.Date));

        }

        
        public List<Commit_Info[]> SplitCommitsByDay()
        {
            List<Commit_Info[]> listOfCommits = _commits
                .GroupBy(c => c.Date.Date)
                .OrderBy(g => g.Key) // optional: sort days chronologically
                .Select(g => g.ToArray())
                .ToList();
            listOfCommits.Reverse();
            exportToGitJ();
            return listOfCommits;
        }

        private void test()
        {
            List<Commit_Info[]> dailyCommits = SplitCommitsByDay();

            foreach (var dayGroup in dailyCommits)
            {
                Console.WriteLine($"--- Commits for {dayGroup[0].Date.Date.ToShortDateString()} ---");
                foreach (var commit in dayGroup)
                {
                    Console.WriteLine($"  {commit.CommitId} by {commit.User}");
                }
            }

        }

        public void exportToGitJ(string fullPath = "")
        {
            foreach (Commit_Info commit in _commits)
            {
                commit.Origin = "GitJ";
            }

            // Ensure the directory exists
            if (!Directory.Exists(_controller._GitJFileDir))
            {
                Directory.CreateDirectory(_controller._GitJFileDir);
            }

            // Serialize _commits to JSON
            string jsonString = JsonSerializer.Serialize(_commits, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            if (string.IsNullOrWhiteSpace(fullPath))
                // Write to file (creates file if it doesn't exist; overwrites if it does)
                File.WriteAllText(_controller._ActualGitJPath, jsonString);
            else
            {
                if(_controller._RepoSelected != null)
                    File.WriteAllText(Path.Combine(fullPath, $"{_controller._RepoSelected.Replace("/", "@")}.gitj"), jsonString);
                else
                    File.WriteAllText(Path.Combine(fullPath, "GitJournalExport.gitj"), jsonString);
            }

            Debug.WriteLine($"Saving to : {_controller._ActualGitJPath}");
        }
        
        public bool checkIfGitJExist()
        {
            return File.Exists(_controller._ActualGitJPath);
        }

        public void importFromGitJ(string fullPath = "")
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                _controller._ActualGitJPath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), _controller._GitJFileDir.ToLower(), $"{_controller._RepoSelected.Replace("/", "@")}.gitj");
                if (checkIfGitJExist())
                {
                    string jsonString = File.ReadAllText(_controller._ActualGitJPath);
                    List<Commit_Info> importedCommits = JsonSerializer.Deserialize<List<Commit_Info>>(jsonString);
                    if (importedCommits != null)
                    {
                        _commits.AddRange(importedCommits);
                    }
                }
            }
            else
            {
                string jsonString = File.ReadAllText(fullPath);
                List<Commit_Info> importedCommits = JsonSerializer.Deserialize<List<Commit_Info>>(jsonString);
                if (importedCommits != null)
                {
                    _commits.AddRange(importedCommits);
                }

                _controller._isFromGitHub = false;
                _controller._mainWindow.displayJDT();
            }
        }

        public async Task modifyEntry(string commitId, string title = default, string content = default, string user = default, string status = default, string duration = default)
        {
            Debug.WriteLine("ModifyEnty");
            int index = _commits.FindIndex(c => c.CommitId == commitId);


            if (index != null)
            {
                if (title != default && title != _commits[index].Title)
                {
                    _commits[index].Title = title;
                    _commits[index].IsTitleModifed = true;
                }

                if (content != default && content != _commits[index].Content)
                {
                    _commits[index].Content = content;
                    _commits[index].IsContentModifed = true;
                }

                if (user != default && user != _commits[index].User)
                {
                    _commits[index].User = user;
                    _commits[index].IsUserModified = true;
                }

                if (status != default && status != _commits[index].Status)
                {
                    _commits[index].Status = status;
                    _commits[index].IsStatusModifed = true;
                }

                if (duration != default && TimeSpan.TryParse(duration, out TimeSpan parsedDuration) && parsedDuration != _commits[index].Duration)
                {
                    _commits[index].Duration = parsedDuration;
                    _commits[index].IsTDurationModifed = true;
                }
                exportToGitJ();
            }

            await Task.CompletedTask;
        }

    }
}
