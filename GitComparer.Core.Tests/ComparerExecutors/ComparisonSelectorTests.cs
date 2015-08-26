using Gitcomparer.Core.Comparers;
using Gitcomparer.Core.Model;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitComparer.Core.Tests.ComparerExecutors
{
    [TestFixture]
    public class ComparisonSelectorTests
    {
        private Mock<IBranchComparer> _mockBranchComparer;
        private Branch _mainBranch;
        private Branch _refBranch;

        [SetUp]
        public void createBranchComparer()
        {
            _mockBranchComparer = new Mock<IBranchComparer>();
        }

        [SetUp]
        public void createBranches()
        {
            _mainBranch = new Mock<Branch>().Object;
            _refBranch = new Mock<Branch>().Object;
            _mainBranch.Name = "Main";
            _refBranch.Name = "Ref";
        }

        [TestCase(ResultStatus.Yes, ResultStatus.Yes)]
        [TestCase(ResultStatus.No, ResultStatus.No)]
        [TestCase(ResultStatus.Unknown, ResultStatus.Unknown)]
        public void When_Compare_Branches_Is_Called_The_Status_Of_The_Result_Should_Be_The_Same_As_That_Of_The_Comparison_Between_The_Main_Branch_And_The_Ref_Branch(ResultStatus status, ResultStatus expectedStatus)
        {
            /* Setup */
            _mockBranchComparer
                .Setup(m => m.Compare(_mainBranch, _refBranch))
                .Returns(new ComparerResult { MainBranch = _mainBranch, RefBranch = _refBranch, Status = status });

            var selector = new ComparisonSelector(_mockBranchComparer.Object);

            /* Test */
            var result = selector.CompareBranches(_mainBranch, _refBranch);

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(expectedStatus));
        }

        [TestCase(ResultStatus.Yes, ResultStatus.Yes, ResultStatus.Yes)]
        [TestCase(ResultStatus.Yes, ResultStatus.No, ResultStatus.Yes)]
        [TestCase(ResultStatus.Yes, ResultStatus.Unknown, ResultStatus.Yes)]
        [TestCase(ResultStatus.No, ResultStatus.Yes, ResultStatus.Yes)]
        [TestCase(ResultStatus.No, ResultStatus.No, ResultStatus.No)]
        [TestCase(ResultStatus.No, ResultStatus.Unknown, ResultStatus.Unknown)]
        [TestCase(ResultStatus.Unknown, ResultStatus.Yes, ResultStatus.Yes)]
        [TestCase(ResultStatus.Unknown, ResultStatus.No, ResultStatus.No)]
        [TestCase(ResultStatus.Unknown, ResultStatus.Unknown, ResultStatus.Unknown)] 
        public void When_Check_Branch_Is_Called_A_Comparison_With_Result_Yes_Always_Returns_Yes_Otherwise_The_Result_Status_Is_That_Of_The_Last_Comparison(ResultStatus status1, ResultStatus status2, ResultStatus expectedStatus)
        {
            /* Setup */
            _mockBranchComparer
                .Setup(m => m.Compare(_mainBranch, _mainBranch))
                .Returns(new ComparerResult { Status = status1 });
            _mockBranchComparer
                .Setup(m => m.Compare(_mainBranch, _refBranch))
                .Returns(new ComparerResult { Status = status2 });
            var selector = new ComparisonSelector(_mockBranchComparer.Object);

            /* Test */
            var result = selector.CheckBranch(_mainBranch, new List<Branch> { _mainBranch, _refBranch });

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(expectedStatus));
        }
       
        [Test]
        public void When_Compare_All_Possible_Branch_Pairs_Is_Called_It_Should_Return_A_List_Of_All_Results_With_Status_Yes_And_The_Correct_Combination_Of_Main_And_Ref_Branches()
        {
            /* Setup */
            var mockBranch1 = new Mock<Branch>().Object;
            var mockBranch2 = new Mock<Branch>().Object;
            var mockBranch3 = new Mock<Branch>().Object;
            var branches = new List<Branch> { mockBranch1, mockBranch2, mockBranch3 };

            _mockBranchComparer
                .Setup(m => m.Compare(mockBranch1, mockBranch1))
                .Returns(new ComparerResult { MainBranch = mockBranch1, RefBranch = mockBranch1, Status = ResultStatus.No });
            _mockBranchComparer
                .Setup(m => m.Compare(mockBranch1, mockBranch2))
                .Returns(new ComparerResult { MainBranch = mockBranch1, RefBranch = mockBranch2, Status = ResultStatus.Yes });
            _mockBranchComparer
                .Setup(m => m.Compare(mockBranch1, mockBranch3))
                .Returns(new ComparerResult { MainBranch = mockBranch1, RefBranch = mockBranch3, Status = ResultStatus.Yes });
            _mockBranchComparer
                .Setup(m => m.Compare(mockBranch2, mockBranch1))
                .Returns(new ComparerResult { MainBranch = mockBranch2, RefBranch = mockBranch1, Status = ResultStatus.Yes });
            _mockBranchComparer
                .Setup(m => m.Compare(mockBranch2, mockBranch2))
                .Returns(new ComparerResult { MainBranch = mockBranch2, RefBranch = mockBranch2, Status = ResultStatus.No });
            _mockBranchComparer
                .Setup(m => m.Compare(mockBranch2, mockBranch3))
                .Returns(new ComparerResult { MainBranch = mockBranch2, RefBranch = mockBranch3, Status = ResultStatus.Yes });
            _mockBranchComparer
                .Setup(m => m.Compare(mockBranch3, mockBranch1))
                .Returns(new ComparerResult { MainBranch = mockBranch3, RefBranch = mockBranch1, Status = ResultStatus.No });
            _mockBranchComparer
                .Setup(m => m.Compare(mockBranch3, mockBranch2))
                .Returns(new ComparerResult { MainBranch = mockBranch3, RefBranch = mockBranch2, Status = ResultStatus.No });
            _mockBranchComparer
                .Setup(m => m.Compare(mockBranch3, mockBranch3))
                .Returns(new ComparerResult { MainBranch = mockBranch3, RefBranch = mockBranch3, Status = ResultStatus.No });

            var selector = new ComparisonSelector(_mockBranchComparer.Object);
            var expectedResults = new List<ComparerResult> 
            {
                new ComparerResult { MainBranch = mockBranch1, RefBranch = mockBranch2 },
                new ComparerResult { MainBranch = mockBranch1, RefBranch = mockBranch3 },
                new ComparerResult { MainBranch = mockBranch2, RefBranch = mockBranch1 },
                new ComparerResult { MainBranch = mockBranch2, RefBranch = mockBranch3 }
            };

            /* Test */
            var results = selector.CompareAllPossibleBranchPairs(branches);

            /* Assert */
            Assert.That(results.Select(r => r.MainBranch), Is.EqualTo(expectedResults.Select(r => r.MainBranch)));
            Assert.That(results.Select(r => r.RefBranch), Is.EqualTo(expectedResults.Select(r => r.RefBranch)));
        }

        [Test]
        public void When_Remove_Duplicates_Is_Called_Only_The_First_Instance_Of_All_Results_That_Have_The_Same_Main_Branch_Should_Be_Kept()
        {
            /* Setup */
            var branch1 = new Branch { Name = "Branch 1"};
            var branch2 = new Branch { Name = "Branch 2"};
            var branch3 = new Branch { Name = "Branch 3"};
            var results = new List<ComparerResult>
            {
                new ComparerResult { MainBranch = branch1, RefBranch = branch2 },
                new ComparerResult { MainBranch = branch1, RefBranch = branch3 },
                new ComparerResult { MainBranch = branch2, RefBranch = branch1 },
                new ComparerResult { MainBranch = branch2, RefBranch = branch3 },
                new ComparerResult { MainBranch = branch3, RefBranch = branch2 }
            };
            var expectedResults = new List<ComparerResult> 
            {
                new ComparerResult { MainBranch = branch1, RefBranch = branch2 },
                new ComparerResult { MainBranch = branch2, RefBranch = branch1 },
                new ComparerResult { MainBranch = branch3, RefBranch = branch2 }
            };
            var mockComparer = new Mock<IBranchComparer>().Object;
            var selector = new ComparisonSelector(mockComparer);

            /* Test */
            var finalResults = selector.RemoveDuplicates(results);
            
            /* Assert */
            Assert.That(finalResults.Select(r => r.MainBranch), Is.EqualTo(expectedResults.Select(r => r.MainBranch)));
            Assert.That(finalResults.Select(r => r.RefBranch), Is.EqualTo(expectedResults.Select(r => r.RefBranch)));
        }

        [Test]
        public void When_Remove_Invalid_Results_Is_Called_All_Results_That_Have_The_Same_Main_And_Ref_Branches_As_An_Earlier_Result_Should_Be_Removed()
        {
            /* SetUp */
            var branch1 = new Branch { Name = "Branch 1" };
            var branch2 = new Branch { Name = "Branch 2" };
            var branch3 = new Branch { Name = "Branch 3" };
            var mockComp = new Mock<IBranchComparer>().Object;
            var selector = new ComparisonSelector(mockComp);
            var results = new List<ComparerResult>
            {
                new ComparerResult { MainBranch = branch1, RefBranch = branch2 },
                new ComparerResult { MainBranch = branch1, RefBranch = branch3 },
                new ComparerResult { MainBranch = branch2, RefBranch = branch1 },
                new ComparerResult { MainBranch = branch2, RefBranch = branch3 },
                new ComparerResult { MainBranch = branch3, RefBranch = branch2 }
            };
            var expectedResults = new List<ComparerResult>
            {
                new ComparerResult { MainBranch = branch1, RefBranch = branch2 },
                new ComparerResult { MainBranch = branch1, RefBranch = branch3 },
                new ComparerResult { MainBranch = branch2, RefBranch = branch3 }
            };

            /* Test */
            var finalResults = selector.RemoveInvalidResults(results);

            /* Assert */
            Assert.That(finalResults.Select(r => r.MainBranch), Is.EqualTo(expectedResults.Select(r => r.MainBranch)));
            Assert.That(finalResults.Select(r => r.RefBranch), Is.EqualTo(expectedResults.Select(r => r.RefBranch)));
        }

        [Test]
        public void Test_Check_All_Branches()
        {
            /* Setup */
            /* Test */
            /* Assert */
        }
    }
}
