using ConsoleUI;
using System.IO.Compression;

Console.Title = "ChdMan Menu";

string? rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
string? chdmanPath = null;

List<string> chdFiles = new();
List<string> isoFiles = new();
List<string> cueFiles = new();
List<string> binFiles = new();
List<string> gdiFiles = new();
List<string> zipFiles = new();

string fileSummary = "";

bool deleteFiles = false;
bool moveToParent = false;
bool moveToChild = false;


// MAIN
if (FindChdmanExe())
{
    MainMenu();
}

UI.Header("Goodbye", true);
UI.Pause();


void MainMenu()
{
    while (true)
    {
        Menu menu = new Menu();

        menu.description = PrintFiles();

        menu.Add("Import Files...", ImportFiles);

        if (isoFiles.Count > 0 || cueFiles.Count > 0 || binFiles.Count > 0 || gdiFiles.Count > 0)
        {
            menu.Add("Create CD CHD file(s)", CueGdiIsoToChd, "PSX, Dreamcast, NeoGeo CD, (some) PS2");
            menu.Add("Create DVD CHD file(s)", CueGdiIsoToChdDvd, "PS2 (default hunk size)");
            menu.Add("Create DVD CHD file(s)", CueGdiIsoToChdPsp, "PPSSPP (2048 hunk size)");
        }

        if (chdFiles.Count > 0)
        {
            menu.Add("Extract DVD CHD to ISO", ExtractDvdToIso);
            menu.Add("Extract CD CHD to CUE/BIN", ExtractCdChdToCueBin);
            menu.Add("Extract CD CHD to GDI", ExtractCdChdToGdi);
        }

        menu.Make();
    }
}


void ImportFiles()
{
    UI.Header("Import Files");
    UI.Write("Enter files or directories containing files to process.");
    UI.Write();
    UI.Write("Leave blank to use this application's directory.");
    UI.Write();

    List<string> fileList = Input.GetFiles();

    if (fileList.Count == 0)
    {
        fileList = Directory.GetFiles(rootDirectory, "*.*", SearchOption.AllDirectories).ToList();
    }

    PopulateFileLists(fileList);

    // Extract and populate file lists with zip files
    if (zipFiles.Count > 0)
    {
        foreach (string file in zipFiles)
        {
            ExtractZipFolder(file);
        }
    }
}


void AskDeleteFiles()
{
    Menu menu = new();

    menu.description = "Delete source files after compression/extraction?";

    menu.Add("Yes", DeleteFiles);
    menu.Add("No", menu.Null);

    menu.Make();
}


void DeleteFiles()
{
    deleteFiles = true;
}


void AskMoveToParent()
{
    Menu menu = new();

    menu.description = "Move output CHD files into the parent directory of their source file(s)?" +
        "\nE.g. Use this if CUE files are contained within their own subfolder with associated BIN files.";

    menu.Add("Yes", MoveToParent);
    menu.Add("No", menu.Null);

    menu.Make();
}


void MoveToParent()
{
    moveToParent = true;
}


void AskCreateChildDirectory()
{
    Menu menu = new();

    menu.description = "Move output CUE + BIN files to child directory?" +
        "\nThis will create a new child directory with the same name as the input (CUE) file.";

    menu.Add("Yes", MoveToChild);
    menu.Add("No", menu.Null);

    menu.Make();
}


void MoveToChild()
{
    moveToChild = true;
}


void CueGdiIsoToChd()
{
    AskDeleteFiles();
    AskMoveToParent();

    UI.Header("CD to CHD");

    List<string> files = isoFiles.Concat(cueFiles).Concat(gdiFiles).ToList();
    List<string> failList = new();

    UI.Write();
    UI.Write($"Processing {files.Count} file(s)...\n");

    for (int i = 0; i < files.Count; i++)
    {
        string inputFile = files[i];
        string outputFile = Path.ChangeExtension(inputFile, ".chd");

        if (moveToParent)
        {
            string parentDir = Path.GetFullPath(Path.Combine(inputFile, @"..\..\"));

            if (parentDir != null) outputFile = parentDir + Path.GetFileName(outputFile);
        }

        if (RunChdman($"createcd -i \"{inputFile}\" -o \"{outputFile}\""))
        {
            OnCompressionSuccess(inputFile, outputFile);
        }
        else
        {
            failList.Add(inputFile);
        }

        ShowProgress(i + 1 - failList.Count, files.Count);
    }

    FinishedScreen(failList);
}


void CueGdiIsoToChdDvd()
{
    AskDeleteFiles();
    AskMoveToParent();

    UI.Header("DVD to CHD");

    List<string> files = isoFiles.Concat(cueFiles).Concat(gdiFiles).ToList();
    List<string> failList = new();

    UI.Write($"Processing {files.Count} file(s)...\n");

    for (int i = 0; i < files.Count; i++)
    {
        string inputFile = files[i];
        string outputFile = Path.ChangeExtension(inputFile, ".chd");

        if (moveToParent)
        {
            string parentDir = Path.GetFullPath(Path.Combine(inputFile, @"..\..\"));

            if (parentDir != null) outputFile = parentDir + Path.GetFileName(outputFile);
        }

        if (RunChdman($"createdvd -i \"{inputFile}\" -o \"{outputFile}\""))
        {
            OnCompressionSuccess(inputFile, outputFile);
        }
        else
        {
            failList.Add(inputFile);
        }

        ShowProgress(i + 1 - failList.Count, files.Count);
    }

    FinishedScreen(failList);
}


void CueGdiIsoToChdPsp()
{
    AskDeleteFiles();
    AskMoveToParent();

    UI.Header("PSP DVD to CHD");

    List<string> files = isoFiles.Concat(cueFiles).Concat(gdiFiles).ToList();
    List<string> failList = new();

    UI.Write($"Processing {files.Count} file(s)...\n");

    for (int i = 0; i < files.Count; i++)
    {
        string inputFile = files[i];
        string outputFile = Path.ChangeExtension(inputFile, ".chd");

        if (moveToParent)
        {
            string parentDir = Path.GetFullPath(Path.Combine(inputFile, @"..\..\"));

            if (parentDir != null) outputFile = parentDir + Path.GetFileName(outputFile);
        }

        if (RunChdman($"createdvd -hs 2048 -i \"{inputFile}\" -o \"{outputFile}\""))
        {
            OnCompressionSuccess(inputFile, outputFile);
        }
        else
        {
            failList.Add(inputFile);
        }

        ShowProgress(i + 1 - failList.Count, files.Count);
    }

    FinishedScreen(failList);
}


void ExtractDvdToIso()
{
    AskDeleteFiles();

    UI.Header("CHD to ISO");

    List<string> files = chdFiles;
    List<string> failList = new();

    UI.Write($"Processing {files.Count} file(s)...\n");

    for (int i = 0; i < files.Count; i++)
    {
        string inputFile = files[i];
        string outputFile = Path.ChangeExtension(inputFile, ".iso");

        if (RunChdman($"extractdvd -i \"{inputFile}\" -o \"{outputFile}\""))
        {
            OnCompressionSuccess(inputFile, outputFile);
        }
        else
        {
            failList.Add(inputFile);
        }

        ShowProgress(i + 1 - failList.Count, files.Count);
    }

    FinishedScreen(failList);
}


void ExtractCdChdToCueBin()
{
    AskDeleteFiles();
    AskCreateChildDirectory();

    UI.Header("CHD to CUE/BIN");

    List<string> files = chdFiles;
    List<string> failList = new();

    UI.Write($"Processing {files.Count} file(s)...\n");

    for (int i = 0; i < files.Count; i++)
    {
        string inputFile = files[i];
        string outputFile = Path.ChangeExtension(inputFile, ".cue");

        if (moveToChild)
        {
            string? dir = Path.GetDirectoryName(inputFile);

            if (dir != null)
            {
                string outputChildDir = $"{dir}\\{Path.GetFileNameWithoutExtension(inputFile)}\\";
                string outputFileName = Path.GetFileName(outputFile);

                Directory.CreateDirectory(outputChildDir);

                outputFile = outputChildDir + outputFileName;
            }
            else
            {
                UI.Error("Somehow the directory, within which the input file, is located does not exist.");
            }
        }

        if (RunChdman($"extractcd -i \"{inputFile}\" -o \"{outputFile}\""))
        {
            OnCompressionSuccess(inputFile, outputFile);
        }
        else
        {
            failList.Add(inputFile);
        }

        ShowProgress(i + 1 - failList.Count, files.Count);
    }

    FinishedScreen(failList);
}


void ExtractCdChdToGdi()
{
    AskDeleteFiles();

    UI.Header("CHD to CUE/BIN");

    List<string> files = chdFiles;
    List<string> failList = new();

    UI.Write($"Processing {files.Count} file(s)...\n");

    for (int i = 0; i < files.Count; i++)
    {
        string inputFile = files[i];
        string outputFile = Path.ChangeExtension(inputFile, ".gdi");

        if (RunChdman($"extractcd -i \"{inputFile}\" -o \"{outputFile}\""))
        {
            OnCompressionSuccess(inputFile, outputFile);
        }
        else
        {
            failList.Add(inputFile);
        }

        ShowProgress(i + 1 - failList.Count, files.Count);
    }

    FinishedScreen(failList);
}


List<string> ExtractZipFolder(string file)
{
    List<string> extractedFiles = new();

    string? destination = Path.GetDirectoryName(file);

    if (destination != null)
    {
        if (!Directory.Exists(destination))
        {
            Directory.CreateDirectory(destination);
        }
        else
        {
            UI.Warning($"Directory \"{destination}\" already exists! " +
                $"(I'm extracting everything to this location anyway, because I'm too lazy" +
                $"to set up the logic to not to that.");
        }

        using (ZipArchive archive = ZipFile.OpenRead(file))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                entry.ExtractToFile(destination);
            }
        }
    }

    return extractedFiles;
}


void OnCompressionSuccess(string inputFile, string outputFile)
{
    // Show file size difference
    if (File.Exists(outputFile))
    {
        double inputSize = GetFileSizeMb(inputFile);
        double outputSize = GetFileSizeMb(outputFile);

        if (Path.GetExtension(inputFile).ToLower() == ".cue")
        {
            foreach (string binFile in GetBinFilesFromCue(inputFile))
            {
                inputSize += GetFileSizeMb(binFile);
            }
        }

        if (Path.GetExtension(outputFile).ToLower() == ".cue")
        {
            foreach (string binFile in GetBinFilesFromCue(outputFile))
            {
                outputSize += GetFileSizeMb(binFile);
            }
        }

        UI.Write();
        UI.Write($"Size difference: {inputSize} MB -> {outputSize} MB", ConsoleColor.DarkCyan);
    }

    // Delete input file
    if (deleteFiles) DeleteFile(inputFile);
    else UI.Write("No files deleted.", ConsoleColor.DarkCyan);
}


void ShowProgress(int current, int total)
{
    UI.Write($"\nProgress: {current} of {total} done\n", ConsoleColor.Cyan);
}


void FinishedScreen(List<string> failList)
{
    // Clear file lists
    chdFiles.Clear();
    isoFiles.Clear();
    cueFiles.Clear();
    binFiles.Clear();
    gdiFiles.Clear();

    UI.Header("Done!", false);

    if (failList.Count > 0)
    {
        UI.Write($"{failList.Count} file(s) failed to process:", ConsoleColor.DarkRed);
        foreach (string file in failList)
        {
            UI.Write($"\t{file}", ConsoleColor.DarkRed);
        }
        UI.Write();
    }

    UI.Pause();
}

void DeleteFile(string file)
{
    UI.Write();

    if (Path.GetExtension(file).ToLower() == ".cue")
    {
        foreach(string binFile in GetBinFilesFromCue(file))
        {
            File.Delete(binFile);
            UI.Write($"Deleted \"{binFile}\"", ConsoleColor.DarkYellow);
        }
        
        File.Delete(file);
        UI.Write($"Deleted \"{file}\"", ConsoleColor.DarkYellow);

        string? dir = Path.GetDirectoryName(file);
        if (dir != null && moveToParent)
        {
            Directory.Delete(dir);
            UI.Write($"Deleted directory \"{dir}\"", ConsoleColor.DarkYellow);
        }
    }
    else
    {
        File.Delete(file);
        UI.Write($"Deleted \"{file}\"", ConsoleColor.DarkYellow);
    }
}


List<string> GetBinFilesFromCue(string file)
{
    List<string> output = new();

    if (File.Exists(file) && Path.GetExtension(file).ToLower() == ".cue")
    {
        foreach (string line in File.ReadLines(file))
        {
            if (line.Contains("FILE "))
            {
                string[] list = line.Split('"');
                string binName = list[1];

                string? path = Path.GetDirectoryName(file);
                string binFile = $"{path}\\{binName}";

                if (File.Exists(binFile))
                {
                    output.Add(binFile);
                }
            }
        }
    }

    return output;
}


bool RunChdman(string arg = "")
{
    using (System.Diagnostics.Process p = new System.Diagnostics.Process())
    {
        p.StartInfo.FileName = chdmanPath;
        p.StartInfo.Arguments = arg;
        p.Start();
        p.WaitForExit();

        // Return true if chdman is successful
        // chdman gives exit code 0 if there are no problems (as far as I know lol)
        if (p.ExitCode == 0) return true;
        else return false;
    }
}


void PopulateFileLists(List<string> files)
{
    // Clear currently loaded files
    chdFiles.Clear();
    isoFiles.Clear();
    cueFiles.Clear();
    binFiles.Clear();
    gdiFiles.Clear();

    // Add valid files to lists
    foreach (string file in files)
    {
        if (File.Exists (file))
        {
            string ext = Path.GetExtension(file);

            if (ext.ToLower() == ".chd") chdFiles.Add(file);
            if (ext.ToLower() == ".iso") isoFiles.Add(file);
            if (ext.ToLower() == ".cue") cueFiles.Add(file);
            if (ext.ToLower() == ".bin") binFiles.Add(file);
            if (ext.ToLower() == ".gdi") gdiFiles.Add(file);

            if (ext.ToLower() == ".zip") zipFiles.Add(file);
        }
        else
        {
            UI.Warning($"\"{file}\" does not exist!");
        }
    }
}


double GetFileSizeMb(string file)
{
    return new FileInfo(file).Length / 1000000;
}


string PrintFiles()
{
    fileSummary = string.Empty;

    fileSummary += "Imported files:\n\n";
    fileSummary += $"\tFound {cueFiles.Count()} .CUE files\n";
    fileSummary += $"\tFound {binFiles.Count()} .BIN files\n";
    fileSummary += $"\tFound {isoFiles.Count()} .ISO files\n";
    fileSummary += $"\tFound {gdiFiles.Count()} .GDI files\n";
    fileSummary += $"\tFound {chdFiles.Count()} .CHD files\n";

    return fileSummary;
}


bool FindChdmanExe()
{
    string tempPath = $"{rootDirectory}\\chdman.exe";

    if (File.Exists(tempPath))
    {
        chdmanPath = tempPath;
        return true;
    }
    else
    {
        UI.Header("Chdman not found!");
        UI.Write($"Chdman was not found at the expected location \"{tempPath}\"");
        UI.Write();
        UI.Write("Please enter a valid path to chdman.exe");
        UI.Write();

        chdmanPath = null;

        if (GetChdmanExe())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}


bool GetChdmanExe()
{
    string? path = Input.GetFile();
    
    if (File.Exists(path))
    {
        if (Path.GetFileName(path) == "chdman.exe")
        {
            chdmanPath = path;
            return true;
        }
        else
        {
            chdmanPath = null;
            UI.Error("chdman.exe not found!");
            return false;
        }
    }
    else
    {
        chdmanPath = null;
        UI.Error("chdman.exe not valid!");
        return false;
    }
}