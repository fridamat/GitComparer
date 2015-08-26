using Gitcomparer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitcomparer.Core.Comparers
{
    public class ComparerResult
    {
        public ResultStatus Status { get; set; }
        public Branch MainBranch { get; set; }
        public Branch RefBranch { get; set; }
    }
}
