using Gitcomparer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitcomparer.Core.Comparers
{
    public class CommitDetailComparer : ICommitComparer
    {
        public ComparerResult Compare(Branch mainBranch, Branch refBranch)
        {
            var allCommitsMatch = true;
            var result = new ComparerResult();
            result.MainBranch = mainBranch;
            result.RefBranch = refBranch;

            foreach (var mainCommit in mainBranch.Commits)
            {
                if (!refBranch.Commits.Select(c => c.Id).Contains(mainCommit.Id))
                {
                    var potentialByMsg = refBranch.Commits.Where(c => c.Message == mainCommit.Message);
                    var potentialByName = potentialByMsg.Where(c => c.Name == mainCommit.Name);
                    var potentialByFiles = potentialByName.Where(c => c.Files == mainCommit.Files);

                    if (!potentialByFiles.Any())
                    {
                        allCommitsMatch = false;
                        break;
                    }
                }
            }

            if (allCommitsMatch)
            {
                result.Status = ResultStatus.Yes;
            }
            else
            {
                result.Status = ResultStatus.No;
            }

            return result;

        }
    }
}
