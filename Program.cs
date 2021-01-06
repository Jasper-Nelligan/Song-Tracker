using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;


namespace Song_Tracker
{
    class Program
    {
        static List<Entry> LoadJson()
        {
            using (StreamReader r = new StreamReader("song_list.json"))
            {
                string json = r.ReadToEnd();
                Root jsonroot = JsonConvert.DeserializeObject<Root>(json);
                return jsonroot.entries;
            }
        }
        

        static void Main(string[] args)
        {
            List<Entry> entries = LoadJson();
            foreach(Entry entry in entries){
                Console.WriteLine(entry.ToString());
            }

            // DateTime today = DateTime.Now;
            // Console.WriteLine(today.ToString("MM/dd/yyyy"));

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
        public string title { get; set; } 
        public string artist { get; set; } 
    }

    public class Entry{
        public string date { get; set; } 
        public List<Song> songs { get; set; }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.date + "\n");

            int i = 1;
            foreach(Song song in this.songs)
            {
                sb.AppendFormat($"{i}. {song.title} by {song.artist}\n");
                i++;
            }
            return sb.ToString();
        }
    }

    public class Root{
        public List<Entry> entries { get; set; }
    } 
}
