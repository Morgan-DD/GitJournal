using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitJournal
{
    internal class JDT_manager
    {
        Controller _controller;
        List<Commit_Info> _commits = new List<Commit_Info>();

        public JDT_manager(Controller controller)
        {
            _controller = controller;
        }

        public void addNewEntry(string CommitId, string title, string content, string user, string status, TimeSpan Duration, bool ExistingStatus, DateTime Date)
        {
            _commits.Add(new Commit_Info
            {
                CommitId = CommitId,
                Title = title,
                Content = content,
                User = user,
                Status = status,
                Duration = Duration,
                ExistingStatus = ExistingStatus,
                Date = Date
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

        public static List<Commit_Info[]> SplitCommitsByDay(List<Commit_Info> commits)
        {
            return commits
                .GroupBy(c => c.Date.Date)
                .OrderBy(g => g.Key) // optional: sort days chronologically
                .Select(g => g.ToArray())
                .ToList();
        }

        private void test()
        {
            List<Commit_Info[]> dailyCommits = SplitCommitsByDay(_commits);

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
