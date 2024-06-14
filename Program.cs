using ConsoleUI;


string? rootDirectory = AppDomain.CurrentDomain.BaseDirectory;

string? chdmanPath = null;
string logPath = "chdman_log.txt";

List<string> chdFiles = new();
List<string> isoFiles = new();
List<string> cueFiles = new();
List<string> binFiles = new();
List<string> gdiFiles = new();

bool deleteFiles = false;



// MAIN
if (FindChdmanExe())
{
    MainMenu();
}



// HERE LIE METHODS
void MainMenu()
{
    //bool active = true;
    //while (active)
    //{
        // Reset on menu loop
        //GetFiles(null);
        //deleteFiles = false;
        //Console.Clear();

        UI.Header("Main Menu");
        UI.Write($"Found chdman.exe at: \"{chdmanPath}\"");
        UI.Write();
        UI.Write("Enter directory containing files to process.");
        UI.Write("Leave blank to use this application's directory.");
        UI.Write();

        string workingDirectory = Input.GetDirectory();

        if (workingDirectory != "")
        {
            if (Path.Exists(workingDirectory))
            {
                GetFiles(workingDirectory);
                ProcessMenu(workingDirectory);
            }
            else
            {
                UI.Error("Invalid directory!");
                //active = false;
                //continue;
            }
        }
        else
        {
            workingDirectory = rootDirectory;

            GetFiles(workingDirectory);
            ProcessMenu(workingDirectory);
        }
    }
//}


void ProcessMenu(string? dir = null)
{
    if (dir != null)
    {
        UI.Header("Select Process");

        PrintFiles();

        UI.Option("[1] Create CD CHD file(s) - PSX, Dreamcast, NeoGeo CD, (some) PS2");
        UI.Option("[2] Create DVD CHD file(s) - PS2 (default hunk size)");
        UI.Option("[3] Create DVD CHD file(s) - PSP (2048 hunk size)");
        UI.Write();
        UI.Option("[4] Extract DVD CHD to ISO");
        UI.Option("[5] Extract CD CHD to CUE/BIN");
        UI.Option("[6] Extract CD CHD to GDI");
        UI.Write();

        int input = Input.GetInteger();

        UI.Header("Process Settings");
        UI.Write("\nDelete source files when done?");
        UI.Option("[1] No");
        UI.Option("[2] Yes");

        int inputDel = Input.GetInteger(1);

        switch (inputDel)
        {
            case 1:
                deleteFiles = false;
                break;
            case 2:
                deleteFiles = true;
                break;

            // Keep files by default
            default:
                deleteFiles = false;
                break;
        }

        switch (input)
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
                break;
        }
    }
    else
    {
        UI.Write("You should never have seen this.");
    }
}


void CueGdiIsoToChd()
{
    UI.Header("CD to CHD");

    List<string> files = isoFiles.Concat(cueFiles).Concat(gdiFiles).ToList();
    List<string> failList = new();

    UI.Write($"Processing {files.Count} file(s)...\n");

    for (int i = 0; i < files.Count; i++)
    {
        string file = files[i];

        int exitCode = RunChdman($"createcd -i \"{file}\" -o \"{Path.ChangeExtension(file, ".chd")}\"");

        if (exitCode == 0)
        {
            if (deleteFiles)
            {
                DeleteFile(file);
            }
        }
        else
        {
            failList.Add(file);
        }

        UI.Write($"\nProgress: {i + 1} of {files.Count} done\n");
    }

    UI.Write("------------------------------------------");
    UI.Write("\nDone!\n");

    if (failList.Count > 0)
    {
        UI.Write($"{failList.Count} file(s) failed to process:");
        UI.Write();
        foreach (string file in failList)
        {
            UI.Write($"{file}");
        }
        UI.Write();
        UI.Pause();
    }
}


void CueGdiIsoToChdDvd()
{
    UI.Header("DVD to CHD");

    List<string> files = isoFiles.Concat(cueFiles).Concat(gdiFiles).ToList();
    List<string> failList = new();

    UI.Write($"Processing {files.Count} file(s)...\n");

    for (int i = 0; i < files.Count; i++)
    {
        string file = files[i];

        int exitCode = RunChdman($"createdvd -i \"{file}\" -o \"{Path.ChangeExtension(file, ".chd")}\"");

        if (exitCode == 0)
        {
            if (deleteFiles)
            {
                DeleteFile(file);
            }
        }
        else
        {
            failList.Add(file);
        }

        UI.Write($"\nProgress: {i + 1} of {files.Count} done\n");
    }

    UI.Write("------------------------------------------");
    UI.Write("\nDone!\n");

    if (failList.Count > 0)
    {
        UI.Write($"{failList.Count} file(s) failed to process:");
        UI.Write();
        foreach (string file in failList)
        {
            UI.Write($"{file}");
        }
        UI.Write();
        UI.Pause();
    }
}


void CueGdiIsoToChdPsp()
{
    UI.Header("PSP DVD to CHD");

    List<string> files = isoFiles.Concat(cueFiles).Concat(gdiFiles).ToList();
    List<string> failList = new();

    UI.Write($"Processing {files.Count} file(s)...\n");

    for (int i = 0; i < files.Count; i++)
    {
        string file = files[i];

        int exitCode = RunChdman($"createdvd -hs 2048 -i \"{file}\" -o \"{Path.ChangeExtension(file, ".chd")}\"");

        if (exitCode == 0)
        {
            if (deleteFiles)
            {
                DeleteFile(file);
            }
        }
        else
        {
            failList.Add(file);
        }

        UI.Write($"\nProgress: {i + 1} of {files.Count} done\n");
    }

    UI.Write("------------------------------------------");
    UI.Write("\nDone!\n");

    if (failList.Count > 0)
    {
        UI.Write($"{failList.Count} file(s) failed to process:");
        UI.Write();
        foreach (string file in failList)
        {
            UI.Write($"{file}");
        }
        UI.Write();
        UI.Pause();
    }
}


void ExtractDvdToIso()
{
    UI.Header("CHD to ISO");

    List<string> files = chdFiles;
    List<string> failList = new();

    UI.Write($"Processing {files.Count} file(s)...\n");

    for (int i = 0; i < files.Count; i++)
    {
        string file = files[i];

        int exitCode = RunChdman($"extractdvd -i \"{file}\" -o \"{Path.ChangeExtension(file, ".iso")}\"");

        if (exitCode == 0)
        {
            if (deleteFiles)
            {
                DeleteFile(file);
            }
        }
        else
        {
            failList.Add(file);
        }

        UI.Write($"\nProgress: {i + 1} of {files.Count} done\n");
    }

    UI.Write("------------------------------------------");
    UI.Write("\nDone!\n");

    if (failList.Count > 0)
    {
        UI.Write($"{failList.Count} file(s) failed to process:");
        UI.Write();
        foreach (string file in failList)
        {
            UI.Write($"{file}");
        }
        UI.Write();
        UI.Pause();
    }
}


void ExtractCdChdToCueBin()
{
    UI.Header("CHD to CUE/BIN");

    List<string> files = chdFiles;
    List<string> failList = new();

    UI.Write($"Processing {files.Count} file(s)...\n");

    for (int i = 0; i < files.Count; i++)
    {
        string file = files[i];

        int exitCode = RunChdman($"extractcd -i \"{file}\" -o \"{Path.ChangeExtension(file, ".cue")}\"");

        if (exitCode == 0)
        {
            if (deleteFiles)
            {
                DeleteFile(file);
            }
        }
        else
        {
            failList.Add(file);
        }

        UI.Write($"\nProgress: {i + 1} of {files.Count} done\n");
    }

    UI.Write("------------------------------------------");
    UI.Write("\nDone!\n");

    if (failList.Count > 0)
    {
        UI.Write($"{failList.Count} file(s) failed to process:");
        UI.Write();
        foreach (string file in failList)
        {
            UI.Write($"{file}");
        }
        UI.Write();
        UI.Pause();
    }
}


void ExtractCdChdToGdi()
{
    UI.Header("CHD to CUE/BIN");

    List<string> files = chdFiles;
    List<string> failList = new();

    UI.Write($"Processing {files.Count} file(s)...\n");

    for (int i = 0; i < files.Count; i++)
    {
        string file = files[i];

        int exitCode = RunChdman($"extractcd -i \"{file}\" -o \"{Path.ChangeExtension(file, ".gdi")}\"");

        if (exitCode == 0)
        {
            if (deleteFiles)
            {
                DeleteFile(file);
            }
        }
        else
        {
            failList.Add(file);
        }

        UI.Write($"\nProgress: {i + 1} of {files.Count} done\n");
    }

    UI.Write("------------------------------------------");
    UI.Write("\nDone!\n");

    if (failList.Count > 0)
    {
        UI.Write($"{failList.Count} file(s) failed to process:");
        UI.Write();
        foreach (string file in failList)
        {
            UI.Write($"{file}");
        }
        UI.Write();
        UI.Pause();
    }
}


void DeleteFile(string file)
{
    UI.Write();

    // Delete cue files and their bin files
    if (Path.GetExtension(file).ToLower() == ".cue")
    {
        foreach(string line in File.ReadLines(file))
        {
            if (line.Contains("FILE "))
            {
                string[] list = line.Split('"');
                string binName = list[1];

                string path = Path.GetDirectoryName(file);
                string binFile = $"{path}\\{binName}";

                if (File.Exists(binFile))
                {
                    File.Delete(binFile);
                    UI.Write($"Deleted \"{binName}\"");
                }
            }
        }

        File.Delete(file);
        UI.Write($"Deleted \"{file}\"");
    }
    // Delete everything else
    else
    {
        File.Delete(file);
        UI.Write($"Deleted \"{file}\"");
    }
}


int RunChdman(string arg = "")
{
    using (System.Diagnostics.Process p = new System.Diagnostics.Process())
    {
        p.StartInfo.FileName = chdmanPath;
        p.StartInfo.Arguments = arg;
        p.Start();
        p.WaitForExit();

        return p.ExitCode;
    }
}


void GetFiles(string? dir = null)
{
    if (Path.Exists(dir))
    {
        List<string> fileList = Directory.GetFiles(dir, "*.*", System.IO.SearchOption.AllDirectories).ToList();

        foreach (string file in fileList)
        {
            string ext = Path.GetExtension(file);

            if (ext.ToLower() == ".chd") chdFiles.Add(file);
            if (ext.ToLower() == ".iso") isoFiles.Add(file);
            if (ext.ToLower() == ".cue") cueFiles.Add(file);
            if (ext.ToLower() == ".bin") binFiles.Add(file);
            if (ext.ToLower() == ".gdi") gdiFiles.Add(file);
        }
    }
    else
    {
        chdFiles.Clear();
        isoFiles.Clear();
        cueFiles.Clear();
        binFiles.Clear();
        gdiFiles.Clear();
    }
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
    UI.Header("Enter Application Path");
    UI.Write("chdman.exe was not found in root directory.");
    UI.Write();
    UI.Write("Please enter a valid path to chdman.exe");
    UI.Write();

    string path = Input.GetFile();
    
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