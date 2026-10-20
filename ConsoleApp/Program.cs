using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

public class Program
{
    static async Task Main()
    {
        var stopwatch = Stopwatch.StartNew();

        int totalSpaces = await CountSpacesInThreeFilesAsync("file1.txt", "file2.txt", "file3.txt");
        Console.WriteLine($"Итого: {totalSpaces} пробелов");
        Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
        Console.WriteLine($"=============================");

        int allspaces = 0;
        var files = await GetFilesinDirectory(Environment.CurrentDirectory);

        // foreach(var file in files)
        // {
        //     Task<int> t = CountSpaces(file);
        //     int res = await t; 
        //     Console.WriteLine($"Пробелы в {file}: {res}");
        //     allspaces += res;
        // }
        // это был синхронный подход, не параллельный 

        var tasks = files.Select(file => CountSpaces(file)).ToList();

        int[] results = await Task.WhenAll(tasks);

        for(int i = 0;i < files.Count; i++)
        {
            Console.WriteLine($"Пробелы в {files[i]}: {results[i]}");
        }
        
        allspaces = results.Sum();

        Console.WriteLine($"Всего пробелов: {allspaces}");
        stopwatch.Stop();
        Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
    }
    public static async Task<int> CountSpacesInThreeFilesAsync(params string[] filePaths)
    {
        var tasks = new List<Task<int>>();
        
        foreach (var filePath in filePaths)
        {
            tasks.Add(CountSpaces(filePath));
        }
        
        int[] results = await Task.WhenAll(tasks);
        int totalCount = results.Sum();
        
        Console.WriteLine($"Всего пробелов в файлах: {totalCount}");
        return totalCount;
    }

    public static async Task<List<string>> GetFilesinDirectory(string path)
    {
        List<string> txtFiles = new List<string>();
        try
        {
            var files = await Task.Run(() => 
                Directory.EnumerateFiles(path, "*.txt", SearchOption.AllDirectories)
            );
            txtFiles.AddRange(files);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Доступ запрещён: {ex.Message}");
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.WriteLine($"Папка не найдена: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Другое ошибка: {ex.Message}");
        }
        return txtFiles;
    }
    public static async Task<int> CountSpaces(string filePath)
    {
        try
        {
            string content = await File.ReadAllTextAsync(filePath);
            int count = content.Count(char.IsWhiteSpace);
            Console.WriteLine($"✓ Обработан файл: {filePath}");
            return count;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Ошибка при чтении {filePath}: {ex.Message}");
            return 0;
        }
    }
}