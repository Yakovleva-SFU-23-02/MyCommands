using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Доступные команды:");
            Console.WriteLine("pwd - показать текущую директорию");
            Console.WriteLine("ls [path] - список файлов и папок");
            Console.WriteLine("cat <file> - показать содержимое файла");
            return;
        }

        string command = args[0].ToLower();

        switch (command)
        {
            case "pwd":
                PwdCommand();
                break;
            case "ls":
                LsCommand(args);
                break;
            case "cat":
                CatCommand(args);
                break;
            default:
                Console.WriteLine($"Неизвестная команда: {command}");
                break;
        }
    }

    public static void PwdCommand()
    {
        Console.WriteLine($"Текущая директория: {Directory.GetCurrentDirectory()}");
    }

    public static void LsCommand(string[] args)
    {
        string path = args.Length > 1 ? args[1] : Directory.GetCurrentDirectory();

        try
        {
            Console.Write($"{path}: ");

            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (var dir in Directory.GetDirectories(path))
            {
                Console.Write($"{Path.GetFileName(dir)}/ ");
            }

            foreach (var file in Directory.GetFiles(path))
            {
                var fileInfo = new FileInfo(file);
                Console.ForegroundColor = GetFileColor(fileInfo);
                Console.Write($"{fileInfo.Name} ({GetHumanReadableSize(fileInfo.Length)}) ");
            }

            Console.ResetColor();
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ошибка: {ex.Message}");
            Console.ResetColor();
        }
    }

    private static ConsoleColor GetFileColor(FileInfo file)
    {
        string ext = file.Extension.ToLower();

        return ext switch
        {
            ".exe" or ".bat" or ".cmd" or ".msi" => ConsoleColor.Green,
            ".jpg" or ".png" or ".gif" or ".bmp" or ".svg" => ConsoleColor.Magenta,
            ".zip" or ".rar" or ".7z" or ".tar" or ".gz" => ConsoleColor.Yellow,
            ".txt" or ".doc" or ".docx" or ".pdf" or ".rtf" => ConsoleColor.Cyan,
            ".cs" or ".java" or ".js" or ".py" or ".cpp" => ConsoleColor.DarkCyan,
            ".json" or ".xml" or ".yml" or ".yaml" => ConsoleColor.DarkYellow,
            ".log" or ".tmp" => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };
    }

    private static string GetHumanReadableSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double len = bytes;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }

    public static void CatCommand(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Не указан файл для просмотра");
            return;
        }

        string filePath = Path.Combine(Directory.GetCurrentDirectory(), args[1]);

        try
        {
            Console.WriteLine($"Содержимое файла {filePath}:");
            Console.WriteLine("----------------------------------------");

            string fileExtension = Path.GetExtension(filePath).ToLower();
            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    switch (fileExtension)
                    {
                        case ".txt":
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case ".json":
                        case ".xml":
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            break;
                        case ".cs":
                        case ".java":
                        case ".js":
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        case ".html":
                        case ".htm":
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                        case ".log":
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                    }

                    Console.WriteLine(line);
                }
            }

            Console.ResetColor();
            Console.WriteLine("----------------------------------------");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
    public static void CpCommand(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Использование: cp <источник> <назначение>");
            Console.WriteLine("Примеры:");
            Console.WriteLine("  cp file.txt backup/file.txt");
            Console.WriteLine("  cp -r dir1/ dir2/");
            return;
        }

        bool recursive = false;
        string source = args[1];
        string destination = args[2];

        if (args[1] == "-r" || args[1] == "--recursive")
        {
            recursive = true;
            source = args[2];
            destination = args[3];
        }

        try
        {
            string fullSource = Path.GetFullPath(source);
            string fullDest = Path.GetFullPath(destination);

            if (File.Exists(fullSource))
            {
                CopyFile(fullSource, fullDest);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Файл скопирован: {fullSource} -> {fullDest}");
            }
            else if (Directory.Exists(fullSource))
            {
                if (!recursive)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ошибка: Для копирования директорий используйте -r");
                    return;
                }

                CopyDirectory(fullSource, fullDest);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Директория скопирована: {fullSource} -> {fullDest}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка: Источник не найден: {fullSource}");
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        finally
        {
            Console.ResetColor();
        }
    }

    private static void CopyFile(string sourceFile, string destinationFile)
    {
        if (Directory.Exists(destinationFile))
        {
            string fileName = Path.GetFileName(sourceFile);
            destinationFile = Path.Combine(destinationFile, fileName);
        }

        File.Copy(sourceFile, destinationFile, overwrite: true);
    }

    private static void CopyDirectory(string sourceDir, string destinationDir)
    {
        Directory.CreateDirectory(destinationDir);

        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(destinationDir, fileName);
            File.Copy(file, destFile, overwrite: true);
        }

        foreach (string subDir in Directory.GetDirectories(sourceDir))
        {
            string dirName = Path.GetFileName(subDir);
            string destSubDir = Path.Combine(destinationDir, dirName);
            CopyDirectory(subDir, destSubDir);
        }
    }
}