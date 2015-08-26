using Gitcomparer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitcomparer.Core.Comparers
{
    public class CommitIdComparer : ICommitComparer
    {
        public ComparerResult Compare(Branch mainBranch, Branch refBranch)
        {
            var containsAllIds = true;
            var result = new ComparerResult();
            result.MainBranch = mainBranch;
            result.RefBranch = refBranch;

            foreach (var mainCommit in mainBranch.Commits.Select(c => c.Id))
            {
                if (!refBranch.Commits.Select(c => c.Id).Contains(mainCommit))
                {
                    containsAllIds = false;
                    break;
                }
            }

            if (containsAllIds)
            {
                result.Status = ResultStatus.Yes;
            }
            else
            {
                result.Status = ResultStatus.Unknown;
            }

            return result;

        }
    }
}
