# Setting up the project
## Cloning the repository
+ Open a new Git Bash terminal and `cd` into the desired directory:
+ `cd school-projects`
+ Once in your directory of choice clone the repo with SSH:
+ `git clone git@github.com:benman31/snow-removal-sim.git`
+ If you do not yet have an SSH key associated with your GitHub account, you can follow the instructions provided here:
+ https://docs.github.com/en/authentication/connecting-to-github-with-ssh/checking-for-existing-ssh-keys

## Install Git Large File Storage
+ We will likely be working with some large files and since git is limited to 100 Mb per file we will need to use the Git LFS tool to replace those files with pointers and store the files on a remote server
+ Download Git LFS from https://git-lfs.github.com/ and install the latest version
+ Open a new Git Bash terminal and navigate to the project folder
+ Type `git lfs install`
+ You should now be good to go. A `.gitattributes` file has already been created and filled with some of the most common large file formats used in Unity projects. If we need to add a new file format we can simply add it to `.gitattributes` and continue using git as normal to stage, commit, and push our work to the remote repository

## Opening the project in Unity
+ Open Unity Hub
+ Under the `Installs` section make sure you have editor version `2021.3.9f1` installed. If not, click `Install Editor` and select `2021.3.9f1` from the list
+ Now under the `Projects` section click `Open` and select `Add project from disk` from the dropdown menu and navigate to the directory containing the repository you just cloned
+ After loading for a bit you should be good to go!
