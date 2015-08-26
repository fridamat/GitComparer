using Gitcomparer.Core.Comparers;
using Gitcomparer.Core.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitComparer.Core.Tests.Comparers
{
    [TestFixture]
    public class CommitCountComparerTests
    {
        private CommitCountComparer _comparer; 

        [SetUp]
        public void CreateComparer() {
            _comparer = new CommitCountComparer();
        }

        [Test]
        public void When_The_Main_Branch_Has_No_Commits_Then_The_Main_Branch_Should_Be_Deletable()
        {
            var mainBranch = new Branch
            {
                Commits = new List<Commit>()
            };
            var refBranch = new Branch
            {
                Commits = new List<Commit>()
            };

            /* Test */
            var result = _comparer.Compare(mainBranch, refBranch);
            
            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Yes));
            Assert.That(result.MainBranch, Is.EqualTo(mainBranch));
            Assert.That(result.RefBranch, Is.EqualTo(refBranch));
        }

        [Test]
        public void When_The_Main_Branch_Has_More_Commits_Than_The_Ref_Branch_Then_The_Main_Branch_Should_Not_Be_Deletable()
        {
            /* Setup */
            var mainBranch = new Branch
            {
                Commits = new List<Commit> { new Commit(), new Commit() }
            };
            var refBranch = new Branch
            {
                Commits = new List<Commit> { new Commit() }
            };

            /* Test */
            var result = _comparer.Compare(mainBranch, refBranch);

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.No));
            Assert.That(result.MainBranch, Is.EqualTo(mainBranch));
            Assert.That(result.RefBranch, Is.EqualTo(refBranch));
        }

        [Test]
        public void When_The_Main_Branch_And_The_Ref_Branch_Has_The_Same_Number_Of_Commits_Then_Its_Unknown_Whether_The_Main_Branch_Should_Be_Deletable()
        {
            /* Setup */
            var mainBranch = new Branch
            {
                Commits = new List<Commit> { new Commit() }
            };
            var refBranch = new Branch
            {
                Commits = new List<Commit> { new Commit() }
            };

            /* Test */
            var result = _comparer.Compare(mainBranch, refBranch);

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Unknown));
            Assert.That(result.MainBranch, Is.EqualTo(mainBranch));
            Assert.That(result.RefBranch, Is.EqualTo(refBranch));
        }

        [Test]
        public void When_The_Ref_Branch_Has_More_Commits_Than_The_Main_Branch_Then_Its_Unkown_Whether_The_Main_Branch_Should_Be_Deletable()
        {
            /* Setup */
            var mainBranch = new Branch
            {
                Commits = new List<Commit> { new Commit() }
            };
            var refBranch = new Branch
            {
                Commits = new List<Commit> { new Commit(), new Commit() }
            };

            /* Test */
            var result = _comparer.Compare(mainBranch, refBranch);

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Unknown));
            Assert.That(result.MainBranch, Is.EqualTo(mainBranch));
            Assert.That(result.RefBranch, Is.EqualTo(refBranch));
        }
    }
}
