using Gitcomparer.Core.Comparers;
using Gitcomparer.Core.Model;
using LibGit2Sharp;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitComparer.Core.IntegrationTests
{
    [TestFixture]
    public class ComparerIntegrationTests
    {
        private LibGit2Sharp.Repository _repo;
        private string _repoPath;
        private ComparisonSelector _selector;

        [SetUp]
        public void CreateEmptyRepo() 
        {
            _repoPath = Environment.CurrentDirectory + "\\myRepo";
            var directory = Directory.CreateDirectory(_repoPath);
            LibGit2Sharp.Repository.Init(_repoPath);
            _repo = new LibGit2Sharp.Repository(_repoPath);
        }

        [SetUp]
        public void CreateComparisonSelector()
        {
            _selector = new ComparisonSelector(new BranchComparer(BranchComparer.CommitComparers));
        }

        [TearDown]
        public void DeleteRepo()
        {
            DeleteDirectories(_repoPath);
        }

        [Test]
        public void When_The_Main_Branch_Has_More_Commits_Than_A_Ref_Branch_It_Should_Not_Be_Deletable()
        {
            /* Setup */
            // First commit
            File.WriteAllText(Path.Combine(_repoPath, "file1.txt"), "File1's content...");
            _repo.Stage("file1.txt");
            _repo.Commit("First commit...");

            // Checkout new branch
            _repo.CreateBranch("newBranch");
            _repo.Checkout("newBranch");

            // Second commit
            File.WriteAllText(Path.Combine(_repoPath, "file2.txt"), "File2's content...");
            _repo.Stage("file2.txt");
            _repo.Commit("Second commit...");

            // Prepare for comparison
            var mainBranch = _repo.Branches["newBranch"];
            var refBranch = _repo.Branches["master"];
            
            /* Test */
            var result = _selector.CompareBranches(new Gitcomparer.Core.Model.Branch(mainBranch), new Gitcomparer.Core.Model.Branch(refBranch));

            /* Assert */
            Assert.That(result.MainBranch.Name, Is.EqualTo(mainBranch.Name));
            Assert.That(result.RefBranch.Name, Is.EqualTo(refBranch.Name));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.No));
        }

        [Test]
        public void When_The_Main_Branch_And_The_Ref_Branch_Has_The_Same_Number_Of_Commits_With_The_Same_Commit_Ids_Then_The_Main_Branch_Should_Be_Deletable()
        {
            /* Setup */
            // First commit
            File.WriteAllText(Path.Combine(_repoPath, "file1.txt"), "File1's content...");
            _repo.Stage("file1.txt");
            _repo.Commit("First commit...");

            // Checkout new branch
            _repo.CreateBranch("newBranch");
            _repo.Checkout("newBranch");

            // Prepare for comparison
            var mainBranch = _repo.Branches["newBranch"];
            var refBranch = _repo.Branches["master"];
            
            /* Test */
            var result = _selector.CompareBranches(new Gitcomparer.Core.Model.Branch(mainBranch), new Gitcomparer.Core.Model.Branch(refBranch));

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Yes));
        }

        [Test]
        public void When_Not_All_Of_The_Main_Branchs_Commits_Can_Be_Found_In_The_Ref_Branch_Then_The_Main_Branch_Should_Not_Be_Deletable()
        {
            /* Setup */
            // First commit
            File.WriteAllText(Path.Combine(_repoPath, "file1.txt"), "File1's content...");
            _repo.Stage("file1.txt");
            _repo.Commit("First commit...");

            // Checkout new branch
            _repo.CreateBranch("newBranch");
            _repo.Checkout("newBranch");

            // Second commit
            File.WriteAllText(Path.Combine(_repoPath, "file2.txt"), "File2's content...");
            _repo.Stage("file2.txt");
            _repo.Commit("Second commit...");

            // Prepare for comparison
            var mainBranch = _repo.Branches["newBranch"];
            var refBranch = _repo.Branches["master"];

            /* Test */
            var result = _selector.CompareBranches(new Gitcomparer.Core.Model.Branch(mainBranch), new Gitcomparer.Core.Model.Branch(refBranch));

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.No));
        }

        [Test]
        public void After_A_Merge_Between_Main_Branch_And_Ref_Branch_Then_The_Main_Branch_Should_Be_Deletable()
        {
            /* Setup */
            // First commit
            File.WriteAllText(Path.Combine(_repoPath, "file1.txt"), "File1's content...");
            _repo.Stage("file1.txt");
            _repo.Commit("First commit...");

            // Checkout new branch
            _repo.CreateBranch("newBranch");
            _repo.Checkout("newBranch");

            // Second commit
            File.WriteAllText(Path.Combine(_repoPath, "file2.txt"), "File2's content...");
            _repo.Stage("file2.txt");
            _repo.Commit("Second commit...");

            // Merge branches
            var merger = new LibGit2Sharp.Signature("Name", "@email", DateTime.Now);
            _repo.Checkout("master");
            _repo.Merge("newBranch", merger);

            // Prepare for comparison
            var mainBranch = _repo.Branches["newBranch"];
            var refBranch = _repo.Branches["master"];

            /* Test */
            var result = _selector.CompareBranches(new Gitcomparer.Core.Model.Branch(mainBranch), new Gitcomparer.Core.Model.Branch(refBranch));

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Yes));
        }

        [Test]
        public void When_Check_All_Branches_Is_Called_The_Two_Branches_With_Fewer_And_Corresponding_Commit_Ids_Should_Be_Deletable()
        {
            /* Setup */
            // First commit
            File.WriteAllText(Path.Combine(_repoPath, "file1.txt"), "File1's content...");
            _repo.Stage("file1.txt");
            _repo.Commit("First commit...");

            // Checkout new branch
            _repo.CreateBranch("newBranch1");
            _repo.Checkout("newBranch1");

            // Second commit
            File.WriteAllText(Path.Combine(_repoPath, "file2.txt"), "File2's content...");
            _repo.Stage("file2.txt");
            _repo.Commit("Second commit...");

            // Checkout new branch
            _repo.CreateBranch("newBranch2");
            _repo.Checkout("newBranch2");

            // Second commit
            File.WriteAllText(Path.Combine(_repoPath, "file3.txt"), "File3's content...");
            _repo.Stage("file3.txt");
            _repo.Commit("Third commit...");

            // Prepare for comparison
            var branches = _repo.Branches;
            
            /* Test */
            var results = _selector.CheckAllBranches(branches.Select(b => new Gitcomparer.Core.Model.Branch(b)).ToList());

            /* Assert */
            Assert.That(results.Select(r => r.MainBranch.Name).Contains("master"));
            Assert.That(results.Select(r => r.MainBranch.Name).Contains("newBranch1"));
            Assert.That(!results.Select(r => r.MainBranch.Name).Contains("newBranch2"));
        }

        [Test]
        public void When_Check_Branch_Is_Called_With_A_Branch_That_Has_Fewer_Commits_With_The_Same_Commit_Ids_As_Those_Of_Another_Branch_Then_The_Main_Branch_Should_Be_Deletable()
        {
            /* Setup */
            // First commit
            File.WriteAllText(Path.Combine(_repoPath, "file1.txt"), "File1's content...");
            _repo.Stage("file1.txt");
            _repo.Commit("First commit...");

            // Checkout new branch
            _repo.CreateBranch("newBranch1");
            _repo.Checkout("newBranch1");

            // Second commit
            File.WriteAllText(Path.Combine(_repoPath, "file2.txt"), "File2's content...");
            _repo.Stage("file2.txt");
            _repo.Commit("Second commit...");

            // Checkout new branch
            _repo.CreateBranch("newBranch2");
            _repo.Checkout("newBranch2");

            // Second commit
            File.WriteAllText(Path.Combine(_repoPath, "file3.txt"), "File3's content...");
            _repo.Stage("file3.txt");
            _repo.Commit("Third commit...");

            // Prepare for comparison
            var mainBranch = _repo.Branches["newBranch1"];
            var branches = _repo.Branches;
            
            /* Test */
            var result = _selector.CheckBranch(new Gitcomparer.Core.Model.Branch(mainBranch), branches.Select(b => new Gitcomparer.Core.Model.Branch(b)).ToList());

            /* Assert */
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Yes));
        }

        [Test]
        public void When_A_Branch_Is_Deletable_After_Calling_Check_All_Branches_With_Reference_To_Two_Different_Ref_Branches_Then_The_Same_Branch_Should_Not_Be_Suggested_As_Deletable_More_Than_Once()
        {
            /* Setup */
            // First commit
            File.WriteAllText(Path.Combine(_repoPath, "file1.txt"), "File1's content...");
            _repo.Stage("file1.txt");
            _repo.Commit("First commit...");

            // Checkout new branch
            _repo.CreateBranch("newBranch1");
            _repo.Checkout("newBranch1");

            // Second commit
            File.WriteAllText(Path.Combine(_repoPath, "file2.txt"), "File2's content...");
            _repo.Stage("file2.txt");
            _repo.Commit("Second commit...");

            // Checkout new branch
            _repo.Checkout("master");
            _repo.CreateBranch("newBranch2");
            _repo.Checkout("newBranch2");

            // Third commit
            File.WriteAllText(Path.Combine(_repoPath, "file3.txt"), "File3's content...");
            _repo.Stage("file3.txt");
            _repo.Commit("Third commit...");

            // Prepare for comparison
            var branches = _repo.Branches.Select(r => new Gitcomparer.Core.Model.Branch(r)).ToList();

            /* Test */
            var results = _selector.CheckAllBranches(branches);

            /* Assert */
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First().MainBranch.Name, Is.EqualTo("master"));
            Assert.That(results.First().RefBranch.Name, Is.EqualTo("newBranch1"));
        }

        [Test]
        public void When_Calling_Check_All_Branches_And_A_Ref_Branch_Allows_The_Main_Branch_To_Be_Returned_And_Vice_Versa_Then_Only_One_Of_Them_Should_Be_Suggested_As_Deletable()
        {
            /* Setup */
            // First commit
            File.WriteAllText(Path.Combine(_repoPath, "file1.txt"), "File1's content...");
            _repo.Stage("file1.txt");
            _repo.Commit("First commit...");

            // Checkout new branch
            _repo.CreateBranch("newBranch");
            _repo.Checkout("newBranch");

            // Prepare for comparison
            var branches = _repo.Branches.Select(r => new Gitcomparer.Core.Model.Branch(r)).ToList();

            /* Test */
            var results = _selector.CheckAllBranches(branches);

            /* Assert */
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First().MainBranch.Name, Is.EqualTo("master"));
            Assert.That(results.First().RefBranch.Name, Is.EqualTo("newBranch"));
        }

        private void DeleteDirectories(string directory)
        {
            // Delete subdirectories
            var subdirectories = Directory.EnumerateDirectories(directory);
            foreach (var subdir in subdirectories)
            {
                DeleteDirectories(subdir);
            }
            // Delete files
            var files = Directory.EnumerateFiles(directory);
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                fileInfo.Attributes = FileAttributes.Normal;
                fileInfo.Delete();
            }

            // Delete submitted directory
            Directory.Delete(directory);
        }

    }
}
