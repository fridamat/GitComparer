using Gitcomparer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Gitcomparer.Core.Comparers
{
    public class BranchComparer : IBranchComparer
    {
        private readonly List<ICommitComparer> _comparers;

        public static List<ICommitComparer> CommitComparers = new List<ICommitComparer> {
            new CommitCountComparer(),
            new CommitIdComparer(),
            new CommitDetailComparer()
        };

        public BranchComparer(List<ICommitComparer> comparers)
        {
            _comparers = comparers;
        }

        public ComparerResult Compare(Branch mainBranch, Branch refBranch)
        {
            var result = new ComparerResult
            {
                MainBranch = mainBranch,
                RefBranch = refBranch
            };

            if (mainBranch.Name == refBranch.Name)
            {
                result.Status = ResultStatus.No;
            }
            else
            {
                foreach (var comparer in _comparers)
                {
                    result = comparer.Compare(mainBranch, refBranch);
                    if (result.Status == ResultStatus.Yes || result.Status == ResultStatus.No)
                    {
                        break;
                    }
                }
            }
            Log.Information("Branches compared: " + mainBranch.Name + " + " + refBranch.Name + ", Result: " + result.Status + ".");
            return result;
        }

        public List<string> GetComparers()
        {
            //var stringsToReturn = new List<string>();
            ////var list = typeof(ICommitComparer).Assembly.GetTypes().Where(t => t.(typeof(ICommitComparer)));
            //foreach (var l in list)
            //{
            //    stringsToReturn.Add(l.FullName);
            //}
            //return stringsToReturn;
            return null;
        }
    }
}
