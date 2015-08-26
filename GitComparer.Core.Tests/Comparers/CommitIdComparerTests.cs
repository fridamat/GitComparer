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
    public class CommitIdComparerTests
    {
        private CommitIdComparer _comparer;
        private LibGit2Sharp.ObjectId _sha1;
        private LibGit2Sharp.ObjectId _sha2;
        private LibGit2Sharp.ObjectId _sha3;
        private LibGit2Sharp.ObjectId _sha4;

        [SetUp]
        public void CreateComparer()
        {
            _comparer = new CommitIdComparer();
        }

        [SetUp]
        public void CreateCommitIds()
        {
            // Shas (commit ids) used in tests
            _sha1 = (LibGit2Sharp.ObjectId)"0123456789101112131415161718192021222324";
            _sha2 = (LibGit2Sharp.ObjectId)"1234567891011121314151617181920212223242";
            _sha3 = (LibGit2Sharp.ObjectId)"2345678910111213141516171819202122232425";
            _sha4 = (LibGit2Sharp.ObjectId)"3456789101112131415161718192021222324256";
        }


        [Test]
        public void When_The_Main_Branch_And_Ref_Branch_Have_The_Same_Number_Of_Commits_With_Identical_Commit_Ids_Then_The_Main_Branch_Should_Be_Deletable()
        {
            /* Setup */
            var mainBranch = new Branch
            {
                Commits = new List<Commit> { new Commit { Id = _sha1 }, new Commit { Id = _sha2 } }
            };
            var refBranch = new Branch
            {
                Commits = new List<Commit> { new Commit { Id = _sha1 }, new Commit { Id = _sha2 } }
            };

            /* Test */
            var result = _comparer.Compare(mainBranch, refBranch);

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Yes));
        }

        [Test]
        public void When_The_Main_Branch_And_The_Ref_Branch_Have_The_Same_Number_Of_Commits_But_All_With_Different_Commit_Ids_Then_Its_Unknown_Wheter_The_Main_Branch_Should_Be_Deletable()
        {
            /* Setup */
            var mainBranch = new Branch
            {
                Commits = new List<Commit> { new Commit { Id = _sha1 }, new Commit { Id = _sha2 } }
            };
            var refBranch = new Branch
            {
                Commits = new List<Commit> { new Commit { Id = _sha3 }, new Commit { Id = _sha4 } }
            };

            /* Test */
            var result = _comparer.Compare(mainBranch, refBranch);

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Unknown));
        }

        [Test]
        public void When_The_Main_Branch_And_The_Ref_Branch_Have_The_Same_Number_Of_Commits_But_Some_With_Different_Commit_Ids_Then_Its_Unknown_Wheter_The_Main_Branch_Should_Be_Deletable()
        {
            /* Setup */
            var mainBranch = new Branch
            {
                Commits = new List<Commit> { new Commit { Id = _sha1 }, new Commit { Id = _sha2 } }
            };
            var refBranch = new Branch
            {
                Commits = new List<Commit> { new Commit { Id = _sha1 }, new Commit { Id = _sha3 } }
            };

            /* Test */
            var result = _comparer.Compare(mainBranch, refBranch);

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Unknown));
            Assert.That(result.MainBranch, Is.EqualTo(mainBranch));
            Assert.That(result.RefBranch, Is.EqualTo(refBranch));
        }

        [Test]
        public void When_The_Ref_Branch_Has_More_Commits_Than_The_Main_Branch_And_Contains_All_Of_Main_Branchs_Commit_Ids_Then_The_Main_Branch_Should_Be_Deletable()
        {
            /* Setup */
            var mainBranch = new Branch
            {
                Commits = new List<Commit> { new Commit { Id = _sha1 }, new Commit { Id = _sha2 } }
            };
            var refBranch = new Branch
            {
                Commits = new List<Commit> { new Commit { Id = _sha1 }, new Commit { Id = _sha2 }, new Commit { Id = _sha3 } }
            };

            /* Test */
            var result = _comparer.Compare(mainBranch, refBranch);

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Yes));
            Assert.That(result.MainBranch, Is.EqualTo(mainBranch));
            Assert.That(result.RefBranch, Is.EqualTo(refBranch));
        }

        [Test]
        public void When_The_Ref_Branch_Has_More_Commits_Than_The_Main_Branch_But_Contains_None_Of_Main_Branchs_Commit_Ids_Then_Its_Unknown_Whether_The_Main_Branch_Should_Be_Deletable()
        {
            /* Setup */
            var mainBranch = new Branch
            {
                Commits = new List<Commit> { new Commit { Id = _sha1 } }
            };
            var refBranch = new Branch
            {
                Commits = new List<Commit> { new Commit { Id = _sha2 }, new Commit { Id = _sha3 } }
            };

            /* Test */
            var result = _comparer.Compare(mainBranch, refBranch);

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Unknown));
            Assert.That(result.MainBranch, Is.EqualTo(mainBranch));
            Assert.That(result.RefBranch, Is.EqualTo(refBranch));
        }

        [Test]
        public void When_The_Ref_Branch_Has_More_Commits_Than_The_Main_Branch_But_Contains_Only_Some_Of_Main_Branchs_Commit_Ids_Then_Its_Unknown_Whether_The_Main_Branch_Should_Be_Deletable()
        {
            /* Setup */
            var mainBranch = new Branch
            {
                Commits = new List<Commit> { new Commit { Id = _sha1 }, new Commit { Id = _sha2 } }
            };
            var refBranch = new Branch
            {
                Commits = new List<Commit> { new Commit { Id = _sha1 }, new Commit { Id = _sha3 } }
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
