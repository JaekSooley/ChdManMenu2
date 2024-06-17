# ChdManMenu2

A console application to simplify batch processing with chdman. 

This is a re-write of [chdman-menu](https://github.com/JaekSooley/chdman-menu) in C#.

## Features:
- Pre-defined parameters for creating and extracting CHD files (ISO, CUE/BIN, GDI).
- Option to delete each file as it's compressed/extracted.
- Deletes CUE files _and associated BIN files_.
- Option to move output files into parent directory of source file, deleting source directory if desired. This is good if the source files are each in individual folders.
- Shows size of output files compared to input files after compression/extraction.
- Works with the latest version of chdman.

## Instructions:
- Run the application.
- If chdman.exe is not present in the same directory as ChdManMenu2.exe, you will be asked to input a valid path to chdman.exe.
- Drag and drop a folder containing the files you wish to compress/extract into the window when prompted and press ENTER.

![image](https://github.com/JaekSooley/ChdManMenu2/assets/117260365/568cd78b-e272-4ec7-98ea-60d445baee2c)

 
Get chdman here: https://www.mamedev.org/release.html
_chdman comes bundled with MAME._
