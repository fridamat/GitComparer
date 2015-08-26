using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Gitcomparer.Core.Comparers;
using GitComparer.Core;
using LibGit2Sharp;
using Branch = Gitcomparer.Core.Model.Branch;
using Serilog;

namespace GitComparer.ConsoleApp
{
    /*
     * This class is a console application, which allows the user to select repository, which comparison to run and with which
     * branch(es).
     * It uses GitComparer.Core/Comparers/SelectionHelpers to run comparers with the selected branches.
     */
    public class Program
    {
        static readonly Enum[] CompChoices = new Enum[] { ComparisonOptions.CheckAll, ComparisonOptions.CheckOne, ComparisonOptions.CompareTwo };
        
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.RollingFile(@"C:\\Documents\\{Date}.txt").CreateLogger();
            // Gets directories at basepath
            var basepath = ConfigurationManager.AppSettings["basePath"];
            var repoFolders = Directory.GetDirectories(basepath).Select(r => new DirectoryInfo(r).Name).ToArray();

            // Asks user to select repository from directories
            string selectedRepo;
            ConsoleTools.Select("Select repo", repoFolders, out selectedRepo);
            selectedRepo = basepath + "\\" + selectedRepo;

            // Sets selected repository
            Repository repo = null;

            try
            {
                // Gets selected repo (as LibGit2Sharp.Repository) with submitted path 
                repo = new Repository(selectedRepo);
            }
            catch (RepositoryNotFoundException ex)
            {
                Log.Information(ex, ex.ToString(), true);
                return;
            }

            // Gets selected repo's branches (as Model.Branch) 
                var branches = repo.Branches.Where(b => !b.IsRemote).Select(b => new Branch(b)).ToList();
                var branchNames = branches.Select(b => b.Name).ToArray();

                if (branches.Count >= 2)
                {
                    // Asks user to select which comparison to run
                    Enum compChoice;
                    ConsoleTools.SelectEnum("Select comparison", CompChoices, out compChoice);

                    // Starts comparison with list of branch comparers 
                    var branchComparer = new BranchComparer(BranchComparer.CommitComparers);
                    var comparisonHelper = new ComparisonSelector(branchComparer);
                    // If CheckAll
                    if (compChoice.Equals(ComparisonOptions.CheckAll))
                    {
                        // Runs comparison and gets the deletable branches
                        var deletableBranches = comparisonHelper.CheckAllBranches(branches).Where(b => b.Status.Equals(ResultStatus.Yes)).Select(b => b.MainBranch);
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
                        var mainBranch = new Branch();
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
            
            // TEST
            Console.ReadLine();

        }
    }
}
