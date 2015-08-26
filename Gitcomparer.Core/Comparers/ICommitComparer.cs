using Gitcomparer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitcomparer.Core.Comparers
{
    public interface ICommitComparer
    {
        ComparerResult Compare(Branch mainBranch, Branch refBranch);
    }
}
