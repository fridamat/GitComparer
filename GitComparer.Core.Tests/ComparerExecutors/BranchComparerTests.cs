using Gitcomparer.Core.Comparers;
using Gitcomparer.Core.Model;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitComparer.Core.Tests.Comparers
{
    [TestFixture]
    public class BranchComparerTests
    {
        private Branch _mainBranch;
        private Branch _refBranch;
        private Mock<ICommitComparer> _mockComparer1;
        private Mock<ICommitComparer> _mockComparer2;

        [SetUp]
        public void createComparers()
        {
            _mockComparer1 = new Mock<ICommitComparer>();
            _mockComparer2 = new Mock<ICommitComparer>();
        }


        [SetUp]
        public void createBranches()
        {
            _mainBranch = new Mock<Branch>().Object;
            _refBranch = new Mock<Branch>().Object;
            _mainBranch.Name = "Main";
            _refBranch.Name = "Ref";
        }

        [Test]
        public void When_The_Main_Branch_And_The_Reference_Branch_Have_The_Same_Name_They_Are_The_Same_Branch_And_Thus_The_Main_Branch_Should_Not_Be_Deletable()
        {
            /* Setup */
            var mainBranch = _mainBranch;
            var refBranch = _refBranch;
            refBranch.Name = _mainBranch.Name;

            var branchComparer = new BranchComparer(new List<ICommitComparer> { _mockComparer1.Object, _mockComparer2.Object });

            /* Test */
            var result = branchComparer.Compare(_mainBranch, _refBranch);

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.No));
        }

        [TestCase(ResultStatus.Yes, ResultStatus.Yes, ResultStatus.Yes)]
        [TestCase(ResultStatus.Yes, ResultStatus.No, ResultStatus.Yes)]
        [TestCase(ResultStatus.Yes, ResultStatus.Unknown, ResultStatus.Yes)]
        [TestCase(ResultStatus.No, ResultStatus.Yes, ResultStatus.No)]
        [TestCase(ResultStatus.No, ResultStatus.No, ResultStatus.No)]
        [TestCase(ResultStatus.No, ResultStatus.Unknown, ResultStatus.No)]
        [TestCase(ResultStatus.Unknown, ResultStatus.Yes, ResultStatus.Yes)]
        [TestCase(ResultStatus.Unknown, ResultStatus.No, ResultStatus.No)]
        [TestCase(ResultStatus.Unknown, ResultStatus.Unknown, ResultStatus.Unknown)]
        public void When_A_Comparer_Returns_A_Result_With_Status_Yes_Or_No_This_Should_Be_The_Result_Status_Of_The_Compare_Otherwise_The_Result_Status_Should_Be_That_Of_The_Subsequent_Compare(ResultStatus status1, ResultStatus status2, ResultStatus expectedStatus)
        {
            /* Setup */
            _mockComparer1
                .Setup(m => m.Compare(_mainBranch, _refBranch))
                .Returns(new ComparerResult { Status = status1 });
            _mockComparer2
                .Setup(m => m.Compare(_mainBranch, _refBranch))
                .Returns(new ComparerResult { Status = status2 });
            var comparers = new List<ICommitComparer> { _mockComparer1.Object, _mockComparer2.Object };
            var branchComparer = new BranchComparer(comparers);

            /* Test */
            var result = branchComparer.Compare(_mainBranch, _refBranch);

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(expectedStatus));
        }

        [TestCase(ResultStatus.Yes, ResultStatus.Yes, 1, 0)]
        [TestCase(ResultStatus.Yes, ResultStatus.No, 1, 0)]
        [TestCase(ResultStatus.Yes, ResultStatus.Unknown, 1, 0)]
        [TestCase(ResultStatus.No, ResultStatus.Yes, 1, 0)]
        [TestCase(ResultStatus.No, ResultStatus.No, 1, 0)]
        [TestCase(ResultStatus.No, ResultStatus.Unknown, 1, 0)]
        [TestCase(ResultStatus.Unknown, ResultStatus.Yes, 1, 1)]
        [TestCase(ResultStatus.Unknown, ResultStatus.No, 1, 1)]
        [TestCase(ResultStatus.Unknown, ResultStatus.Unknown, 1, 1)]
        public void When_A_Comparer_Returns_A_Result_With_Status_Yes_Or_No_The_Subsequent_Comparer_Should_Never_Be_Called_Otherwise_Both_Comparers_Should_Be_Called_Once(ResultStatus status1, ResultStatus status2, int expectedNr1, int expectedNr2)
        {
            /* Setup */
            _mockComparer1
                .Setup(m => m.Compare(_mainBranch, _refBranch))
                .Returns(new ComparerResult { Status = status1 });
            _mockComparer2
                .Setup(m => m.Compare(_mainBranch, _refBranch))
                .Returns(new ComparerResult { Status = status2 });
            var comparers = new List<ICommitComparer> { _mockComparer1.Object, _mockComparer2.Object };
            var branchComparer = new BranchComparer(comparers);

            /* Test */
            var result = branchComparer.Compare(_mainBranch, _refBranch);

            /* Assert */
            if (expectedNr1 == 0)
            {
                _mockComparer1.Verify(m => m.Compare(_mainBranch, _refBranch), Times.Never);
            }
            else if (expectedNr1 == 1)
            {
                _mockComparer1.Verify(m => m.Compare(_mainBranch, _refBranch), Times.Once);
            }
            
            if (expectedNr2 == 0) {
                _mockComparer2.Verify(m => m.Compare(_mainBranch, _refBranch), Times.Never);
            } 
            else if (expectedNr2 == 1) 
            {
                _mockComparer2.Verify(m => m.Compare(_mainBranch, _refBranch), Times.Once);
            }
        }
    }
}
