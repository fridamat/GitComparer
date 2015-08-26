# GitComparer
Is your repository untidy? Then you've come to the right place!
The GitComparer compares branches from your selected repository in order to find deletable/duplicate branches.

You can compare a Main Branch against a Reference Branch, a Main Branch against all other branches in the selected repository or to simply check all of the selected repository's branches. The comparison is then performed with a selection of comparers.

# Available comparers
Let's say we want to compare a Main Branch against a Reference Branch.
- Commit Count Comparer<br>
If the Main Branch has zero commits, then the Main Branch is deletable.<br>
If the Main Branch has a larger number of commits than the Reference Branch, then the Main Branch isn't deletable.<br>
If the Main Branch has the same number of/fewer commits than the Reference Branch, then further comparers have to be run in order to determine whether or not the Main Branch is deletable.
- Commit Id Comparer<br>
If all of the Main Branch's commits' ids can be found among the Reference Branch's, then the Main Branch is deletable.<br>
If some/none of the Main Branch's commits' ids can be found among the Reference Branch's, then further comparers have to be run in order to determine whether or not the Main Branch is deletable.
- Commit Detail Comparer<br>
If all of the Main Branch's commits' ids or combination of author, message and files can be found among the Reference Branch's, then the Main Branch is deletable.<br>
If this is not the case, then the Main Branch isn't deletable.

# Requirements
The Core and Console projects use the Nuget package LibGit2Sharp to handle and create Git objects.<br>
The Test projects also use Nuget packages NUnit and Moq.

# Installation
To run the Console Application - simply fork and build the project and run the /GitComparer.ConsoleApplication/bin/Debug/GitComparer.ConsoleApplication.exe file.

# Configuration

# Author
Alfrida Mattisson, fridamat@kth.se

