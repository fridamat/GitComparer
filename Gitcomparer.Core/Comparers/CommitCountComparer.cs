using Gitcomparer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitcomparer.Core.Comparers
{
    public class CommitCountComparer : ICommitComparer
    {
        public ComparerResult Compare(Branch mainBranch, Branch refBranch)
        {
            var mainCommits = mainBranch.Commits.Count();
            var refCommits = refBranch.Commits.Count();

            var result = new ComparerResult();
            result.MainBranch = mainBranch;
            result.RefBranch = refBranch;

            // If there are no commits on the main branch, then the main branch
            // should be deleted
            if (mainCommits == 0)
            {
                result.Status = ResultStatus.Yes;
            }
            // If the main branch contains more commits than the reference branch, 
            // then the main branch should not be deleted
            else if (mainCommits > refCommits)
            {
                result.Status = ResultStatus.No;
            }
            // If the main branch contains the same number of commits/fewer commits than
            // the reference branch, then we cannot deduce whether the main branch should
            // be deleted
            else
            {
                result.Status = ResultStatus.Unknown;
            }

            return result;
        }
    }
}
