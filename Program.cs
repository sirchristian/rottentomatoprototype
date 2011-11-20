using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Console.WriteLine("Arguments required.");
                return;
            }

            if (args[0] == "download")
            {
                RottenTomatoesDownloader.DownloadMovies(args[1]);
            }

            if (args[0] == "createdb")
            {
                RottenTomatoesDownloader.CreateDB(deleteExisting: true);
            }

            if (args[0] == "populdatedb")
            {
                RottenTomatoesDownloader.PopuldateDB();
            }
        }
    }
}
