using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitJournal
{
    public class JDT_manager
    {
        Controller _controller;
        public static List<Commit_Info> _commits { get; set; }

        public JDT_manager(Controller controller)
        {
            _controller = controller;
            _commits = new List<Commit_Info>();
        }

        public void addNewEntry(string CommitId, string title, string content, string user, string status, TimeSpan Duration, bool ExistingStatus, DateTime Date, string url)
        {
            _commits.Add(new Commit_Info
            {
                CommitId = CommitId,
                Title = title,
                Content = content,
                User = user,
                Status = status.ToUpper(),
                Duration = Duration,
                ExistingStatus = ExistingStatus,
                Date = Date,
                Url = url
            });
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
            return _commits
                .GroupBy(c => c.Date.Date)
                .OrderBy(g => g.Key) // optional: sort days chronologically
                .Select(g => g.ToArray())
                .ToList();
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
        
    }
}
