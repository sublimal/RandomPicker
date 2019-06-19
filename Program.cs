using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RandomPicker
{
    class Program
    {
        static String[] extensions = { ".mov", ".mp4", ".avi", ".mpeg", ".mpg", ".wmv", ".mkv", ".m4v", ".flv" };
        static Random random = new Random();
        enum UserChoice
        {
            Quit = 0,
            Yes = 1,
            Next = 2,
            Previous = 3,
            Find = 4,
            Delete = 5
        }

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Scanning...");
            var files = getAllMediaFiles(Directory.GetCurrentDirectory());
            Console.WriteLine("Done.");

            
            // Stop if no files
            if (!files.Any())
            {
                Console.WriteLine("No files found.");
                return;
            }
            // Randomize list (Fisher-Yates)
            int n = files.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                var value = files[k];
                files[k] = files[n];
                files[n] = value;
            }

            UserChoice choice;
            String fileName;
            int picker = 0;
     
            do
            {
                // Go through the randomized list
                fileName = files[picker];
                if (picker == 0)
                    Console.WriteLine(generatePromptString(fileName, true, false));
                else if (picker == files.Count - 1)
                    Console.WriteLine(generatePromptString(fileName, false, true));
                else
                    Console.WriteLine(generatePromptString(fileName, true, true));

                // Continue to loop, play or stop
                choice = readInput();
                switch (choice)
                {
                    case UserChoice.Next:
                        if (picker == files.Count - 1)
                            continue;// Last one, do nothing
                        else
                            picker++;
                        break;
                    case UserChoice.Previous:
                        if (picker == 0)
                            continue;// First one, do nothing
                        else
                            picker--;
                        break;
                    case UserChoice.Quit:
                        return;
                    case UserChoice.Delete:
                        if (fileName == "") break;
                        Console.WriteLine("\nConfirm [y/N]:");
                        var confirm = Console.ReadKey();
                        if (confirm.KeyChar == 'y')
                        {
                            File.Delete(fileName);
                            files[picker] = "";
                            if (picker == files.Count - 1)
                                continue; // Last one
                            else
                                picker++;
                        }
                        break;
                    case UserChoice.Yes:
                        if (fileName == "") break;
                        System.Diagnostics.Process.Start(fileName);
                        break;
                    case UserChoice.Find:
                        Console.WriteLine("\nInput search string:");
                        var search = Console.ReadLine();
                        var found = files.FirstOrDefault(f => f.ToLower().Contains(search.ToLower()));
                        if (found == null)
                            Console.WriteLine("Not Found.");
                        else
                            picker = files.IndexOf(found);
                        break;
                }

            } while (choice > UserChoice.Quit);
        }

        private static String generatePromptString(String fileName, bool next, bool previous)
        {
            StringBuilder sb = new StringBuilder();
            if (fileName == "") {
                sb.Append("\n(deleted)");
            } else {
                sb.Append(String.Format("\nPlay \"{0}\"? [Y]es,", fileName));
                sb.Append(" [D]elete,");
            }
            if (next)
                sb.Append(" [N]ext,");

            if (previous)
                sb.Append(" [P]revious,");

            sb.Append(" [F]ind, [Q]uit.");

            return sb.ToString();
        }

        static UserChoice readInput()
        {
            // Read key and apply choice
            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case 'y':
                    return UserChoice.Yes;
                case 'd':
                    return UserChoice.Delete;
                case 'p':
                    return UserChoice.Previous;
                case 'q':
                    return UserChoice.Quit;
                case 'f':
                    return UserChoice.Find;
                default:
                    return UserChoice.Next;
            }
        }

        static List<String> getAllMediaFiles(String path)
        {
            // List to return
            var files = new List<String>();

            try
            {
                // Get all files in current dir
                files.AddRange(Directory.GetFiles(path).Where(f => extensions.Any(e => f.EndsWith(e))).ToList());
            }
            catch (Exception)
            {
                //Skip places with exceptions
            }

            String[] dirs = null;
            try
            {
                // Get all dirs in current dir
                dirs = Directory.GetDirectories(path);
            }
            catch (Exception)
            {
                //Skip places with exceptions
            }

            if (dirs != null)
            {
                // Do all this to the subdirs
                foreach (var dir in dirs)
                {
                    files.AddRange(getAllMediaFiles(dir));
                }
            }

            // Return all files collected
            return files;
        }
    }
}
