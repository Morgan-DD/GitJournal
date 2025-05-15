using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitJournal
{
    public class Commit_Info
    {
        public string CommitId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string User { get; set; }
        public string Status { get; set; }
        public TimeSpan Duration { get; set; }
        public bool ExistingStatus { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }
        public string Origin { get; set; }

        public bool IsTitleModifed { get; set; }
        public bool IsContentModifed { get; set; }
        public bool IsStatusModifed { get; set; }
        public bool IsTDurationModifed { get; set; }
    }
}
