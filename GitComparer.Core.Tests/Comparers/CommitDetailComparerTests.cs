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
    public class CommitDetailComparerTests
    {
        private CommitDetailComparer _comparer;
        private LibGit2Sharp.ObjectId _sha1;
        private LibGit2Sharp.ObjectId _sha2;
        private LibGit2Sharp.ObjectId _sha3;
        private LibGit2Sharp.ObjectId _sha4;
        private string _msg1;
        private string _msg2;
        private string _msg3;
        private string _msg4;
        private string _name1;
        private string _name2;
        private string _name3;
        private string _name4;
        private List<string> _files1;
        private List<string> _files2;
        private List<string> _files3;
        private List<string> _files4;

        [SetUp]
        public void CreateComparer()
        {
            _comparer = new CommitDetailComparer();
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

        [SetUp]
        public void CreateCommitMessages()
        {
            _msg1 = "Message 1";
            _msg2 = "Message 2";
            _msg3 = "Message 3";
            _msg4 = "Message 4";
        }

        [SetUp]
        public void CreateCommitNames()
        {
            _name1 = "Name 1";
            _name2 = "Name 2";
            _name3 = "Name 3";
            _name4 = "Name 4";
        }

        [SetUp]
        public void CreateCommitFiles()
        {
            _files1 = new List<string> { "File 1", "File 2" };
            _files2 = new List<string> { "File 2", "File 3" };
            _files3 = new List<string> { "File 3", "File 4" };
            _files4 = new List<string> { "File 4", "File 5" };
        }

        [Test]
        public void When_All_Of_The_Main_Branchs_Commits_Share_Commit_Id_Or_Message_And_Author_Name_And_Files_With_All_Of_The_Ref_Branchs_Commits_Then_The_Main_Branch_Should_Be_Deletable() 
        {
            /* Setup */
            var mainBranch = new Branch
            {
                Commits = new List<Commit> 
                { 
                    new Commit { Id = _sha1 }, 
                    new Commit { Id = _sha2, Message = _msg2, Name = _name2, Files = _files2 }
                }
            };
            var refBranch = new Branch
            {
                Commits = new List<Commit>
                {
                    new Commit { Id = _sha1 },
                    new Commit { Id = _sha3, Message = _msg2, Name = _name2, Files = _files2}
                }
            };

            /* Test */
            var result = _comparer.Compare(mainBranch, refBranch);

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Yes));
            Assert.That(result.MainBranch, Is.EqualTo(mainBranch));
            Assert.That(result.RefBranch, Is.EqualTo(refBranch));
        }

        [Test]
        public void When_All_Of_The_Main_Branchs_Commits_Share_Commit_Id_Or_Message_And_Author_Name_And_Files_With_Some_Of_The_Ref_Branchs_Commits_Then_The_Main_Branch_Should_Be_Deletable()
        {
            /* Setup */
            var mainBranch = new Branch
            {
                Commits = new List<Commit> 
                { 
                    new Commit { Id = _sha1 }, 
                    new Commit { Id = _sha2, Message = _msg2, Name = _name2, Files = _files2 }
                }
            };
            var refBranch = new Branch
            {
                Commits = new List<Commit>
                {
                    new Commit { Id = _sha1 },
                    new Commit { Id = _sha3, Message = _msg2, Name = _name2, Files = _files2},
                    new Commit { Id = _sha4, Message = _msg4, Name = _name4, Files = _files4}
                }
            };

            /* Test */
            var result = _comparer.Compare(mainBranch, refBranch);

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Yes));
            Assert.That(result.MainBranch, Is.EqualTo(mainBranch));
            Assert.That(result.RefBranch, Is.EqualTo(refBranch));
        }

        [Test]
        public void When_None_Of_The_Main_Branchs_Commits_Share_Id_Or_Message_And_Author_Name_And_Files_With_The_Any_Ref_Branchs_Commits_Then_The_Main_Branch_Should_Not_Be_Deletable()
        {
            /* Setup */
            var mainBranch = new Branch
            {
                Commits = new List<Commit> 
                { 
                    new Commit { Id = _sha1, Message = _msg1, Name = _name1, Files = _files1 }, 
                    new Commit { Id = _sha2, Message = _msg2, Name = _name2, Files = _files2 }
                }
            };
            var refBranch = new Branch
            {
                Commits = new List<Commit>
                {
                    new Commit { Id = _sha3, Message = _msg3, Name = _name3, Files = _files3},
                    new Commit { Id = _sha4, Message = _msg4, Name = _name4, Files = _files4}
                }
            };

            /* Test */
            var result = _comparer.Compare(mainBranch, refBranch);

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.No));
            Assert.That(result.MainBranch, Is.EqualTo(mainBranch));
            Assert.That(result.RefBranch, Is.EqualTo(refBranch));
        }

        [Test]
        public void When_Some_But_Not_All_Of_The_Main_Branchs_Commits_Share_Id_Or_Message_And_Author_Name_And_Files_With_The_Any_Ref_Branchs_Commits_Then_The_Main_Branch_Should_Not_Be_Deletable()
        {
            /* Setup */
            var mainBranch = new Branch
            {
                Commits = new List<Commit> 
                { 
                    new Commit { Id = _sha1, Message = _msg1, Name = _name1, Files = _files1 }, 
                    new Commit { Id = _sha2, Message = _msg2, Name = _name2, Files = _files2 }
                }
            };
            var refBranch = new Branch
            {
                Commits = new List<Commit>
                {
                    new Commit { Id = _sha3, Message = _msg1, Name = _name1, Files = _files1},
                    new Commit { Id = _sha4, Message = _msg4, Name = _name4, Files = _files4}
                }
            };

            /* Test */
            var result = _comparer.Compare(mainBranch, refBranch);

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.No));
            Assert.That(result.MainBranch, Is.EqualTo(mainBranch));
            Assert.That(result.RefBranch, Is.EqualTo(refBranch));
        }
    }
}
