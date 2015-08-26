using Gitcomparer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitcomparer.Core.Comparers
{
    public class ComparisonSelector
    {
        private IBranchComparer _branchComparer;

        public ComparisonSelector(IBranchComparer branchComparer)
        {
            _branchComparer = branchComparer;
        }

        /*
         * Compares two submitted branches.
         */
        public ComparerResult CompareBranches(Branch mainBranch, Branch refBranch)
        {
            var result = _branchComparer.Compare(mainBranch, refBranch);
            return result;
        }

        /*
         * Compares a main branch against all branches in a list of branches.
         */
        public ComparerResult CheckBranch(Branch mainBranch, List<Branch> branches)
        {
            var result = new ComparerResult();
            foreach (var refBranch in branches)
            {
                result = _branchComparer.Compare(mainBranch, refBranch);
                if (result.Status == ResultStatus.Yes)
                {
                    break;
                }
            }
            return result;
        }

        /*
         * Compares all possible combinations of branch pairs in a list of branches and removes duplicate and invalid results.
         */
        public List<ComparerResult> CheckAllBranches(List<Branch> branches)
        {
            var succResults = CompareAllPossibleBranchPairs(branches);
            succResults = RemoveDuplicates(succResults);
            succResults = RemoveInvalidResults(succResults);

            return succResults;
        }

        internal List<ComparerResult> CompareAllPossibleBranchPairs(List<Branch> branches)
        {
            var succResults = new List<ComparerResult>();
            foreach (var mainBranch in branches)
            {
                foreach (var refBranch in branches)
                {
                    var result = _branchComparer.Compare(mainBranch, refBranch);
                    if (result.Status == ResultStatus.Yes)
                    {
                        succResults.Add(result);
                    }
                }
            }
            return succResults;
        }

        internal List<ComparerResult> RemoveInvalidResults(List<ComparerResult> results)
        {
            var succResults = new List<ComparerResult>();

            while (results.Any())
            {
                var result = results.First();
                results.Remove(result);
                succResults.Add(result);

                var invalids = results
                    .Where(r => r.RefBranch.Equals(result.MainBranch))
                    .Where(r => r.MainBranch.Equals(result.RefBranch))
                    .ToList();

                foreach (var invalid in invalids)
                {
                    results.Remove(invalid);
                }
            }

            return succResults;
        }

        internal List<ComparerResult> RemoveDuplicates(List<ComparerResult> results)
        {
            var succResults = new List<ComparerResult>();

            while (results.Any())
            {
                var result = results.First();
                results.Remove(result);
                succResults.Add(result);

                var duplicates = results
                    .Where(r => r.MainBranch.Equals(result.MainBranch))
                    .ToList();
                foreach (var duplicate in duplicates)
                {
                    results.Remove(duplicate);
                }
            }
            return succResults;
        }
    }
}
