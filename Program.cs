using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomPicker
{
    class Program
    {
        static String[] extensions = { ".mp4", ".mkv", ".avi" };
        static Random random = new Random();
        enum UserChoice
        {
            Yes, Next, Cancel
        }

        static void Main(string[] args)
        {
            var files = getAllMediaFiles(Directory.GetCurrentDirectory());

            UserChoice choice;
            String fileName;
            do
            {
                // Stop if no files left
                if (!files.Any())
                    return;

                // Choose a random file and remove it from the list
                var index = random.Next(files.Count);
                fileName = files[index];
                files.RemoveAt(index);
                Console.WriteLine(String.Format("\nPlay \"{0}\"? [Y]es, [N]ext, [C]ancel", fileName.Split('\\').Last()));
                // Continue to loop, play or stop
                choice = readInput();
            } while (choice == UserChoice.Next);

            if (choice == UserChoice.Cancel)
                return;
            else
                System.Diagnostics.Process.Start(fileName);
        }

        static UserChoice readInput()
        {
            // Read key and apply choice
            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case 'y':
                    return UserChoice.Yes;
                case 'c':
                    return UserChoice.Cancel;
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
