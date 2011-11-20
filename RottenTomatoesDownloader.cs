using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

namespace APIHelper
{
    internal class RottenTomatoesDownloader
    {
        private static readonly string movieInfoUrlFormat = "http://api.rottentomatoes.com/api/public/v1.0/movies/{1}.json?apikey={0}";
        private static readonly string getMovieListUrlFormat = "http://api.rottentomatoes.com/api/public/v1.0/lists/movies/in_theaters.json?apikey={0}&page_limit={1}";

        /// <summary>
        /// Downloads movies from rottentomatoes
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="movieLimit"></param>
        internal static void DownloadMovies(string apiKey, string filePath = "movies.db.json", int movieLimit = 10)
        {
            // Our Json Format:

            //{ "movie_list": 
            //   #list#
            //  , 
            //  "movie_details": [
            //     #detail#,
            //     #detail#
            //]}

            using (FileStream file = new FileStream(filePath, FileMode.Create))
            using (StreamWriter fileWriter = new StreamWriter(file))
            {
                string movieListJson = "";

                // Go get the movie list
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(getMovieListUrlFormat, apiKey, movieLimit));
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream jsonStream = response.GetResponseStream())
                using (StreamReader jsonReader = new StreamReader(jsonStream))
                {
                    // Read the returned JSON
                    movieListJson = jsonReader.ReadToEnd();

                    // We are combining different calls so need to add our own little bit of Json
                    fileWriter.WriteLine("{ \"movie_list\":");
                    fileWriter.WriteLine(movieListJson);
                }

                fileWriter.WriteLine(",\"movie_details\": [");

                // Now get all the movies
                // We have to look at the IDs that came back in the movie list.  Rather than
                // trying to deserialize the whole thing we just use regex
                MatchCollection movieIdMatches = Regex.Matches(movieListJson, @"""id"":""(?<ID>\d+)""");
                int numMatches = movieIdMatches.Count;
                int matchNum = 0;
                foreach (Match movieIdMatch in movieIdMatches)
                {
                    matchNum++;
                    string movieId = movieIdMatch.Groups["ID"].Value;
                    request = (HttpWebRequest)WebRequest.Create(String.Format(movieInfoUrlFormat, apiKey, movieId));
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream jsonStream = response.GetResponseStream())
                    using (StreamReader jsonReader = new StreamReader(jsonStream))
                    {
                        // Read the returned JSON
                        fileWriter.Write(jsonReader.ReadToEnd());
                        if (matchNum != numMatches)
                            fileWriter.WriteLine(",");
                    }
                }

                // End our own JSON
                fileWriter.WriteLine("]}");
            }
        }

        /// <summary>ImageUrl
        /// Populdates a DB with the downloaded data
        /// </summary>
        internal static void PopuldateDB(string filePath = "movies.db.json", string dbName = "Movies.db")
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MovieInfo));
            using (DbConnection conn = new SQLiteConnection("Data Source=" + dbName + ";Version=3;"))
            {
                conn.Open();
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    MovieInfo movieInfo = (MovieInfo)serializer.ReadObject(fs);
                    foreach (Movie movie in movieInfo.movie_details)
                    {
                        // Insert the movie. This is probably prone to sql injection. Do don't use this in a production app  
                        DbCommand movieInsert = new SQLiteCommand(String.Format(
                            @"INSERT INTO Movies (Name, TomatoCriticsScore, ImageUrl, MPAARating, RottenTomatoesId, ImdbId)
                              VALUES ('{0}', {1}, '{2}', '{3}', {4}, {5})",
                        movie.title,
                        movie.ratings.critics_score,
                        movie.posters.detailed,
                        movie.mpaa_rating,
                        movie.id,
                        movie.alternate_ids.imdb));
                        movieInsert.Connection = conn;
                        movieInsert.ExecuteNonQuery();
                    }
                }
            }
        }


        /// <summary>
        /// Creates a new DB for our movies
        /// </summary>
        internal static void CreateDB(string dbName = "Movies.db", bool deleteExisting = false)
        {
            if (deleteExisting && File.Exists(dbName))
                File.Delete(dbName);
            else if (!deleteExisting && File.Exists(dbName))
                return;

            using (DbConnection conn = new SQLiteConnection("Data Source=" + dbName + ";Version=3;"))
            {
                conn.Open();
                DbCommand createSQL = conn.CreateCommand();
                createSQL.CommandText = GetTextFromResourceFile("CreateDB.sql");
                createSQL.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Helper to get resource files
        /// </summary>
        /// <param name="resourceFile"></param>
        /// <returns></returns>
        private static string GetTextFromResourceFile(string resourceFile)
        {
            Assembly currentAsm = Assembly.GetExecutingAssembly();
            using (Stream s = currentAsm.GetManifestResourceStream(currentAsm.GetName().Name + "." + resourceFile))
            using (StreamReader sr = new StreamReader(s))
                return sr.ReadToEnd();
        }
    }
}