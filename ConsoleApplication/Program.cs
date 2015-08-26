using Gitcomparer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using Gitcomparer.Core.Comparers;
using Gitcomparer.Core.Model;
using GitComparer.Core;
namespace GitComparer.ConsoleApplication
{
    /*
     * This class is a console application, which allows the user to select repository, which comparison to run and with which
     * branch(es).
     * It uses GitComparer.Core/Comparers/SelectionHelpers to run comparers with the selected branches.
     */
    public class Program
    {
        static Enum[] _compChoices = _compChoices = new Enum[] { ComparisonOptions.CheckAll, ComparisonOptions.CheckOne, ComparisonOptions.CompareTwo };
        static List<ICommitComparer> _branchComparers = new List<ICommitComparer> { new CommitCountComparer(), new CommitIdComparer(), new CommitDetailComparer() };


        static void Main(string[] args)
        {
            // Gets directories at basepath
            var basepath = ConfigurationManager.AppSettings["basePath"];
            var repoFolders = Directory.GetDirectories(basepath).Select(repo => new DirectoryInfo(repo).Name).ToArray();

            // Asks user to select repository from directories
            string selectedRepo;
            ConsoleTools.Select("Select repo", repoFolders, out selectedRepo);
            selectedRepo = basepath + "\\" + selectedRepo;

            // Sets selected repository
            try
            {
                // Gets selected repo (as LibGit2Sharp.Repository) with submitted path 
                var repo = new LibGit2Sharp.Repository(selectedRepo);

                // Gets selected repo's branches (as Model.Branch) 
                var branches = repo.Branches.Where(b => !b.IsRemote).Select(b => new Branch(b)).ToList();
                var branchNames = branches.Select(b => b.Name).ToArray();

                if (branches.Count >= 2)
                {
                    // Asks user to select which comparison to run
                    Enum compChoice;
                    ConsoleTools.SelectEnum("Select comparison", _compChoices, out compChoice);

                    // Starts comparison with list of branch comparers 
                    var comparisonHelper = new ComparisonSelector(new BranchComparer(_branchComparers));
                    // If CheckAll
                    if (compChoice.Equals(ComparisonOptions.CheckAll))
                    {
                        // Runs comparison and gets the deletable branches
                        var deletableBranches = comparisonHelper.CheckAll(branches).Where(b => b.Status.Equals(ResultStatus.Yes)).Select(b => b.MainBranch);
                        // Prints result message according to result status
                        if (deletableBranches.Any())
                        {
                            foreach (var branch in deletableBranches)
                            {
                                Console.WriteLine("It's safe to remove " + branch.Name + ".");
                            }
                        }
                        else
                        {
                            Console.WriteLine("It's not safe to remove any of this repository's branches.");
                        }
                    }
                    else
                    {
                        // Asks user to select and sets main branch
                        string mainBranchName;
                        ConsoleTools.Select("Select the branch of interest", branchNames, out mainBranchName);
                        Branch mainBranch = new Branch();
                        foreach (var branch in branches)
                        {
                            if (branch.Name.Equals(mainBranchName))
                            {
                                mainBranch = branch;
                            }
                        }
                        ComparerResult compResult;
                        // If CheckOne
                        if (compChoice.Equals(ComparisonOptions.CheckOne))
                        {
                            compResult = comparisonHelper.CheckBranch(mainBranch, branches);
                            // Prints result message according to result status
                            if (compResult.Status.Equals(ResultStatus.Yes))
                            {
                                Console.WriteLine("It's safe to delete the branch " + mainBranchName + ".");
                            }
                            else
                            {
                                Console.WriteLine("It's not safe to delete the branch " + mainBranchName + ".");
                            }
                        }
                        // If CompareTwo
                        else
                        {
                            // Asks user to select reference branch
                            string refBranchName;
                            ConsoleTools.Select("Compare " + mainBranchName + " with reference branch", branchNames.Where(b => b != mainBranch.Name).ToArray(), out refBranchName);
                            Branch refBranch = new Branch();
                            foreach (var branch in branches)
                            {
                                if (branch.Name.Equals(refBranchName))
                                {
                                    refBranch = branch;
                                }
                            }
                            compResult = comparisonHelper.CompareBranches(mainBranch, refBranch);
                            // Prints result message according to result status
                            if (compResult.Status.Equals(ResultStatus.Yes))
                            {
                                Console.WriteLine("It's safe to delete the branch " + mainBranchName + ".");
                            }
                            else
                            {
                                Console.WriteLine("It's not safe to delete the branch " + mainBranchName + ".");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("This repository only contains 1 local branch and thus no comparison can be made.");
                }
            }
            catch (LibGit2Sharp.RepositoryNotFoundException)
            {
                Console.WriteLine("The selected directory isn't a repository.");
            }
            // TEST
            Console.ReadLine();

        }
    }
}
