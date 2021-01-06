using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;


namespace Song_Tracker
{
    class Program
    {
        static JsonRoot LoadJson()
        {
            using (StreamReader r = new StreamReader("song_list.json"))
            {
                string json = r.ReadToEnd();
                JsonRoot jsonRoot = JsonConvert.DeserializeObject<JsonRoot>(json);
                return jsonRoot;
            }
        }

        static void WriteJson(JsonRoot jsonRoot){
            string jsonString = JsonConvert.SerializeObject(jsonRoot, Formatting.Indented);
            System.IO.File.WriteAllText("song_list.json", jsonString);
        }
        
        static void Main(string[] args)
        {
            JsonRoot jsonRoot = LoadJson();

            // Code for inserting new Entry and writing to Json:
            // DateTime today = DateTime.Now;
            //
            // Song test = new Song("Hello", "Test");
            // Song test2 = new Song("Hello2", "Test2");
            // List<Song> songList = new List<Song>();
            // songList.Add(test);
            // songList.Add(test2);
            // Entry newEntry = new Entry(today.ToString("MM/dd/yyyy"), songList);
            // jsonRoot.Entries.Add(newEntry);
            // foreach(Entry entry in jsonRoot.Entries){
            //     Console.WriteLine(entry.ToString());
            // }
            // WriteJson(jsonRoot);

            Console.WriteLine("\nHello! Welcome to Song Tracker.\n");
            while (true)
            {
                Console.WriteLine("What would you like to do? (input a number)");
                Console.WriteLine("1 - Add New Entry\n2 - Search Previous Favourites\n3 - info\n");
                var input = Console.ReadLine();
                switch(input)
                {
                    case "1":
                        add_new_entry();
                        break;
                    case "2":
                        search_favourites();
                        break;
                    case "3":
                        info();
                        break;
                    default:
                        Console.WriteLine($"\n'{input}' is not a valid answer.\n");
                        break;
                }
            }
        }


        static void add_new_entry()
        {
            //Read in last entry made in Json
            //get last entry date and store in variable
            //Console.WriteLine($"Your last entry from {last_entry_date} was:);
            //Write down entry here, line by line

            // Console.WriteLine("\nWhat would you like to do? (input a number)");
            // Console.WriteLine("\n1 - insert song\n2 - delete song\n3 - Save and go back\n");
            // var input = Console.ReadLine();
            // switch(input)
            // {
            //     case "1":
            //         Console.WriteLine("What position would you like to insert the new song?");
            //         var pos = Console.ReadLine();s
            //         break;
            //     case "2":
            //         search_favourites();
            //         break;
            //     case "3":
            //         info();
            //         break;
            //     default:
            //         Console.WriteLine($"\n'{input}' is not a valid answer.\n");
            //         break;
            // }
        }

        static void search_favourites()
        {

        }

        static void info(){
            Console.WriteLine("Song Tracker is a small command line program that helps you track your favourite songs throughout "
                             +"time. Every time you modify your top songs list, the program will add a timestamp to that list."
                             +"can view past favourite songs lists via option 2.\n");
        }
    }
 
    public class Song{
        public string Title { get; private set; }
        public string Artist { get; private set; }

        public Song(string title, string artist){
            Title = title;
            Artist = artist;
        }
    }

    public class Entry{
        public string Date { get; } 
        public List<Song> Songs { get; }

        public Entry(string date, List<Song> songs){
            Date = date;
            Songs = songs;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.Date + "\n");

            int i = 1;
            foreach(Song song in this.Songs)
            {
                sb.AppendFormat($"{i}. {song.Title} by {song.Artist}\n");
                i++;
            }
            return sb.ToString();
        }
    }

    public class JsonRoot{
        public List<Entry> Entries { get; set; }

        public void Root(List<Entry> entries){
            Entries = entries;
        }
    } 
}
