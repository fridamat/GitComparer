using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitcomparer.Core.Model
{
    public class Branch
    {
        public Branch()
        {

        }

        public Branch(LibGit2Sharp.Branch original)
        {
            Commits = original.Commits.Select(c => new Commit(c));
            Name = original.Name;
            Remote = original.IsRemote;
        }

        public IEnumerable<Commit> Commits { get; set; }
        public string Name { get; set; }
        public bool Remote { get; set; }
    }
}
