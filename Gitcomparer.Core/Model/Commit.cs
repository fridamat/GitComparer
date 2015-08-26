using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitcomparer.Core.Model
{
    public class Commit
    {
        public Commit()
        {

        }

        public Commit(LibGit2Sharp.Commit original)
        {
            Id = original.Id;
            Message = original.Message;
            Name = original.Author.Name;
            Files = original.Tree.Select(t => t.Name).ToList();
        }

        public LibGit2Sharp.ObjectId Id { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public List<string> Files { get; set; }
    }
}
