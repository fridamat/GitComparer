using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitcomparer.Core.Comparers
{
    public enum ComparisonOptions
    {
        [Description("Compare a main branch against a reference branch.")]
        CompareTwo,
        [Description("Compare a main branch against all other branches.")]
        CheckOne,
        [Description("Check all branches to find all deletable branches.")]
        CheckAll
    }
}
