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

bool usingApplicationDirectory = false;

bool deleteFiles = false;
bool moveToParent = false;


// MAIN
if (FindChdmanExe())
{
    MainMenu();
}
UI.Header("Goodbye", true);
UI.Pause();


// HERE LIE METHODS
void MainMenu()
{
    while (true)
    {
        deleteFiles = false;
        moveToParent = false;

        UI.Header("Load Files");
        UI.Write($"Found chdman.exe at: \"{chdmanPath}\"");
        UI.Write();
        UI.Write("Enter files or directories containing files to process.");
        UI.Write();
        UI.Write("Leave blank to use this application's directory.");
        UI.Write();

        List<string> fileList = Input.GetFiles();

        if (fileList.Count == 0)
        {
            usingApplicationDirectory = true;
            fileList = Directory.GetFiles(rootDirectory, "*.*", SearchOption.AllDirectories).ToList();
        }

        PopulateFileLists(fileList);
        ProcessMenu();
    }
}


void ProcessMenu()
{
    int? processingOption;

    // Select processing option
    while (true)
    {
        UI.Header("Valid Files");
        if (usingApplicationDirectory)
        {
            UI.Write($"Using current application directory ({rootDirectory})");
        }

        PrintFiles();

        UI.Header("Main Menu", false);
        UI.Option("[1] Create CD CHD file(s) - PSX, Dreamcast, NeoGeo CD, (some) PS2");
        UI.Option("[2] Create DVD CHD file(s) - PS2 (default hunk size)");
        UI.Option("[3] Create DVD CHD file(s) - PPSSPP (2048 hunk size)");
        UI.Write();
        UI.Option("[4] Extract DVD CHD to ISO");
        UI.Option("[5] Extract CD CHD to CUE/BIN");
        UI.Option("[6] Extract CD CHD to GDI");
        UI.Write();
        UI.Option("[0] Cancel");
        UI.Write();

        int? input = Input.GetInteger();

        if (input >= 0 && input <= 6)
        {
            processingOption = input;
            break;
        }
    }

    // Select delete files
    while (true)
    {
        bool escape = false;

        UI.Header("Delete Files?");
        UI.Write("\nDelete source file(s) after compression/extraction?\n");
        UI.Option("[1] No");
        UI.Option("[2] Yes");

        int? inputDel = Input.GetInteger();

        switch (inputDel)
        {
            case 1:
                deleteFiles = false;
                escape = true;
                break;
            case 2:
                deleteFiles = true;
                escape = true;
                break;
            default:
                escape = false;
                break;
        }

        if (escape) break;
    }

    // Select move up folder
    while (true)
    {
        moveToParent = false;

        UI.Header("Move to Parent Directory?");
        UI.Write("Move output CHD files into source files' parent directory?");
        UI.Write();
        UI.Write("E.g. Use this if CUE files are contained within their own subfolder with associated BIN files.");
        UI.Write();
        UI.Option("[1] No");
        UI.Option("[2] Yes");

        int? inputMoveUp = Input.GetInteger();
        if (inputMoveUp == 1)
        {
            moveToParent = false;
            break;
        }
        if (inputMoveUp == 2)
        {
            moveToParent = true;
            break;
        }
    }

    // Start Processing
    if (processingOption != null)
    {
        switch (processingOption)
        {
            case 1:
                CueGdiIsoToChd();
                break;
            case 2:
                CueGdiIsoToChdDvd();
                break;
            case 3:
                CueGdiIsoToChdPsp();
                break;
            case 4:
                ExtractDvdToIso();
                break;
            case 5:
                ExtractCdChdToCueBin();
                break;
            case 6:
                ExtractCdChdToGdi();
                break;
            default:
                UI.Error($"Processing option \"{processingOption}\" not recognised!");
                break;
        }
    }
    else
    {
        UI.Error("No processing option selected!");
    }
}


void CueGdiIsoToChd()
{
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
            if (File.Exists(outputFile))
            {
                double oldSize = GetFileSizeMb(inputFile);
                double newSize = GetFileSizeMb(outputFile);

                if (Path.GetExtension(inputFile).ToLower() == ".cue")
                {
                    foreach (string binFile in GetBinFilesFromCue(inputFile))
                    {
                        oldSize += GetFileSizeMb(binFile);
                    }
                }

                UI.Write();
                UI.Write($"Size difference: {oldSize} MB -> {newSize} MB");
            }

            if (deleteFiles) DeleteFile(inputFile);
            else UI.Write("No files deleted.");
        }
        else
        {
            failList.Add(inputFile);
        }

        UI.Write($"\nProgress: {i + 1} of {files.Count} done\n");
    }

    UI.Header("Done!", false);

    if (failList.Count > 0)
    {
        UI.Write($"{failList.Count} file(s) failed to process:");
        UI.Write();
        foreach (string file in failList)
        {
            UI.Write($"{file}");
        }
        UI.Write();
    }

    UI.Pause();
}


void CueGdiIsoToChdDvd()
{
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
            if (File.Exists(outputFile))
            {
                double oldSize = GetFileSizeMb(inputFile);
                double newSize = GetFileSizeMb(outputFile);

                if (Path.GetExtension(inputFile).ToLower() == ".cue")
                {
                    foreach (string binFile in GetBinFilesFromCue(inputFile))
                    {
                        oldSize += GetFileSizeMb(binFile);
                    }
                }

                UI.Write();
                UI.Write($"Size difference: {oldSize} MB -> {newSize} MB");
            }

            if (deleteFiles) DeleteFile(inputFile);
            else UI.Write("No files deleted.");
        }
        else
        {
            failList.Add(inputFile);
        }

        UI.Write($"\nProgress: {i + 1} of {files.Count} done\n");
    }

    UI.Header("Done!", false);

    if (failList.Count > 0)
    {
        UI.Write($"{failList.Count} file(s) failed to process:");
        UI.Write();
        foreach (string file in failList)
        {
            UI.Write($"{file}");
        }
        UI.Write();
    }

    UI.Pause();
}


void CueGdiIsoToChdPsp()
{
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
            if (File.Exists(outputFile))
            {
                double oldSize = GetFileSizeMb(inputFile);
                double newSize = GetFileSizeMb(outputFile);

                if (Path.GetExtension(inputFile).ToLower() == ".cue")
                {
                    foreach (string binFile in GetBinFilesFromCue(inputFile))
                    {
                        oldSize += GetFileSizeMb(binFile);
                    }
                }

                UI.Write();
                UI.Write($"Size difference: {oldSize} MB -> {newSize} MB");
            }

            if (deleteFiles) DeleteFile(inputFile);
            else UI.Write("No files deleted.");
        }
        else
        {
            failList.Add(inputFile);
        }

        UI.Write($"\nProgress: {i + 1} of {files.Count} done\n");
    }

    UI.Header("Done!", false);

    if (failList.Count > 0)
    {
        UI.Write($"{failList.Count} file(s) failed to process:");
        UI.Write();
        foreach (string file in failList)
        {
            UI.Write($"{file}");
        }
        UI.Write();
    }

    UI.Pause();
}


void ExtractDvdToIso()
{
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
            if (File.Exists(outputFile))
            {
                double oldSize = GetFileSizeMb(inputFile);
                double newSize = GetFileSizeMb(outputFile);

                UI.Write();
                UI.Write($"Size difference: {oldSize} MB -> {newSize} MB");
            }

            if (deleteFiles) DeleteFile(inputFile);
            else UI.Write("No files deleted.");
        }
        else
        {
            failList.Add(inputFile);
        }

        UI.Write($"\nProgress: {i + 1} of {files.Count} done\n");
    }

    UI.Header("Done!", false);

    if (failList.Count > 0)
    {
        UI.Write($"{failList.Count} file(s) failed to process:");
        UI.Write();
        foreach (string file in failList)
        {
            UI.Write($"{file}");
        }
        UI.Write();
    }

    UI.Pause();
}


void ExtractCdChdToCueBin()
{
    UI.Header("CHD to CUE/BIN");

    List<string> files = chdFiles;
    List<string> failList = new();

    UI.Write($"Processing {files.Count} file(s)...\n");

    for (int i = 0; i < files.Count; i++)
    {
        string inputFile = files[i];
        string outputFile = Path.ChangeExtension(inputFile, ".cue");

        if (RunChdman($"extractcd -i \"{inputFile}\" -o \"{outputFile}\""))
        {
            if (File.Exists(outputFile))
            {
                double oldSize = GetFileSizeMb(inputFile);
                double newSize = GetFileSizeMb(outputFile);

                if (Path.GetExtension(outputFile).ToLower() == ".cue")
                {
                    foreach (string binFile in GetBinFilesFromCue(outputFile))
                    {
                        newSize += GetFileSizeMb(binFile);
                    }
                }

                UI.Write();
                UI.Write($"Size difference: {oldSize} MB -> {newSize} MB");
            }

            if (deleteFiles) DeleteFile(inputFile);
            else UI.Write("No files deleted.");
        }
        else
        {
            failList.Add(inputFile);
        }

        UI.Write($"\nProgress: {i + 1} of {files.Count} done\n");
    }

    UI.Header("Done!", false);

    if (failList.Count > 0)
    {
        UI.Write($"{failList.Count} file(s) failed to process:");
        UI.Write();
        foreach (string file in failList)
        {
            UI.Write($"{file}");
        }
        UI.Write();
    }

    UI.Pause();
}


void ExtractCdChdToGdi()
{
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
            if (File.Exists(outputFile))
            {
                double oldSize = GetFileSizeMb(inputFile);
                double newSize = GetFileSizeMb(outputFile);

                UI.Write();
                UI.Write($"Size difference: {oldSize} MB -> {newSize} MB");
            }

            if (deleteFiles) DeleteFile(inputFile);
            else UI.Write("No files deleted.");
        }
        else
        {
            failList.Add(inputFile);
        }

        UI.Write($"\nProgress: {i + 1} of {files.Count} done\n");
    }

    UI.Header("Done!", false);

    if (failList.Count > 0)
    {
        UI.Write($"{failList.Count} file(s) failed to process:");
        UI.Write();
        foreach (string file in failList)
        {
            UI.Write($"{file}");
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
            UI.Write($"Deleted \"{binFile}\"");
        }
        
        File.Delete(file);
        UI.Write($"Deleted \"{file}\"");

        string? dir = Path.GetDirectoryName(file);
        if (dir != null && moveToParent)
        {
            Directory.Delete(dir);
            UI.Write($"Deleted directory \"{dir}\"");
        }
    }
    else
    {
        File.Delete(file);
        UI.Write($"Deleted \"{file}\"");
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
    // Reset lists
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


void PrintFiles()
{
    UI.Write();
    UI.Write("Directory contents:");
    UI.Write();
    UI.Write($"Found {cueFiles.Count()} .CUE files");
    UI.Write($"Found {binFiles.Count()} .BIN files");
    UI.Write($"Found {isoFiles.Count()} .ISO files");
    UI.Write($"Found {gdiFiles.Count()} .GDI files");
    UI.Write($"Found {chdFiles.Count()} .CHD files");
    UI.Write();
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