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

## Screenshots

![image](https://github.com/JaekSooley/ChdManMenu2/assets/117260365/d8e777ae-1442-4ac6-b421-7315ada7418c)
![image](https://github.com/JaekSooley/ChdManMenu2/assets/117260365/66c2143c-de0a-437b-847a-305c3b5c146a)
![image](https://github.com/JaekSooley/ChdManMenu2/assets/117260365/54b413a1-ef7f-43ff-afbb-a458f63c4aa7)
![image](https://github.com/JaekSooley/ChdManMenu2/assets/117260365/38981676-4b8c-4123-b4a5-5ed0319655d9)
![image](https://github.com/JaekSooley/ChdManMenu2/assets/117260365/ca194f52-0d0e-4119-a425-0b7a57506f7a)




Get chdman here: https://www.mamedev.org/release.html
_chdman comes bundled with MAME._
