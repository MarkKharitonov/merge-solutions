[![Build](https://github.com/scowalt/merge-solutions/actions/workflows/build.yml/badge.svg?branch=scowalt%2Fmaster)](https://github.com/scowalt/merge-solutions/actions/workflows/build.yml)

# What it does

Merges Visual Studio solutions into a single .sln file

# Usage

```
merge-solutions.exe [/nonstop] [/fix] [/config solutionlist.txt] [/out merged.sln] [solution1.sln solution2.sln ...]
  /fix              regenerates duplicate project guids and replaces them in corresponding project/solution files,
                    requires write-access to project and solution files
  /config list.txt  takes list of new-line separated solution paths from list.txt file
  /out merged.sln   path to output solution file. Default is 'merged.sln'
  /nonstop          do not prompt for keypress if there were errors/warnings
  solution?.sln     list of solutions to be merged
```

# Original project

https://code.google.com/archive/p/merge-solutions/
