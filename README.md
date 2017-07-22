# TcxMerger
**TcxMerger** is a command line application for merging multiple .tcx files into a single .tcx file with multiple activities.

Some workout sites such as Endomondo allows only for selection of a single TCX file to import at once.
Importing many workouts spread into a single file per workout can be tedious. This application helps to mitigate the work
to be done by merging the files into a one big file which can be imported into Endomondo in a single click while still 
importing all the workouts.

# Features
- Merging of virtually unlimited number of files into a one big TCX file.
- Cross platform - should work on every platform with .NET / Mono implementation. Tested on Xubuntu 16.04.

# Requirements
To run the application you need:
- [Microsoft .NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=30653)  
 *or*
- Equivalent version of [Mono](http://www.mono-project.com/download/)  

Additionally, if you want to compile the application you need:
- Microsoft Visual Studio 2015 or later  
 *or*
- MonoDevelop 6.2 or later

# Download
The binary releases and their corresponding source code snapshots can be downloaded at the  [releases](https://github.com/Strachu/TcxMerger/releases) page.

If you would like to retrieve the most up to date source code and compile the application yourself, install git
and clone the repository by executing the command:
`git clone https://github.com/Strachu/TcxMerger.git` or alternatively, click the "Download ZIP" button at the side
panel of this page.

# Usage
* Help display:  
``mono TcxMerger.exe``
* Merge files file1.tcx and file2.tcx into a big.tcx:  
``mono TcxMerger.exe /home/janusz/file1.tcx /home/janusz/file2.tcx /home/janusz/big.tcx``
* Merge all files with extension .tcx from a directory workouts into a big.tcx:  
``mono TcxMerger.exe /home/janusz/workouts/*.tcx /home/janusz/big.tcx``

# Libraries
The application uses the following libraries:
- [CommandLineParser 2.1.1](https://github.com/gsscoder/commandline) command line parsing.

# Tools
During the creation of the application the following tools were used:
- MonoDevelop 6.2
- [Git](https://git-scm.com/)
- [Git Extensions](https://github.com/gitextensions/gitextensions)

# License
TcxMerger is a free software distributed under the GNU LGPL 3 or later license.

See LICENSE.txt and LICENSE_THIRD_PARTY.txt for more information.

The most important point is:  
You can use, modify and distribute the application freely without any charge but you have to make public free of charge any changes to the source code (on LGPL 3 license) of the application *if* you modify the application and distribute the modified version.
