using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

//https://json2csharp.com

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
            List<Entry> entries = jsonRoot.Entries;

            Console.WriteLine("\nHello! Welcome to Song Tracker.");
            while (true)
            {
                Console.WriteLine("\nWhat would you like to do? (input a number)");
                Console.WriteLine("1 - Add new entry\n2 - Search previous favourite songs\n3 - Info\n4 - Exit Program\n");
                var input = Console.ReadLine();
                switch(input)
                {
                    case "1":
                        Entry prevEntry = entries[entries.Count - 1];
                        Entry newEntry = add_new_entry(prevEntry);
                        jsonRoot.Entries.Add(newEntry);
                        WriteJson(jsonRoot);
                        break;
                    case "2":
                        search_favourites();
                        break;
                    case "3":
                        info();
                        break;
                    case "4":
                        System.Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine($"\n'{input}' is not a valid answer.\n");
                        break;
                }
            }
        }

        static Entry add_new_entry(Entry prevEntry)
        {
            while (true)
            {
                Console.WriteLine($"\nYour latest entry from {prevEntry.Date} was:");
                Console.WriteLine(prevEntry.ToString());
                Console.WriteLine("What would you like to do? (input a number)\n");
                Console.WriteLine("1 - Insert song into list\n2 - Delete song from list\n3 - Save new entry and go back\n");
                var input = Console.ReadLine();
                switch(input)
                {
                    case "1":
                        Entry newEntry = add_new_song(prevEntry);
                        if (newEntry != null)
                        {
                            prevEntry = newEntry;
                        }
                        continue;
                    case "2":
                        del_song(prevEntry);
                        continue;
                    case "3":
                        break;
                    default:
                        Console.WriteLine($"\n'{input}' is not a valid answer.\n");
                        continue;
                }
                break;
            }
            return prevEntry;
        }

        /// <summary>
        /// Allows user to create new entry by adding or deleting songs from previous entry.
        /// </summary> 
        /// <param name="prevEntry"> A copy of the last entry on file </param>
        /// <returns> 
        /// A new instance of Entry, null if user decides not to change anything.
        /// </returns>
        static Entry add_new_song(Entry prevEntry)
        {
            int pos = 0;
            if (prevEntry.Songs.Count > 0){
                bool looping = true;
                while (looping){
                    Console.WriteLine("\nWhat position would you like to insert your new favourite song?");
                    Console.WriteLine("(Type a number between 1 and 3, 4 to go back)\n");                   
                    try{
                        pos = int.Parse(Console.ReadLine());
                    }
                    catch (FormatException e){
                        Console.WriteLine($"\n{pos} is not a valid entry.");
                        continue;
                    }
                    if (pos == 4){
                        return null;
                    }
                    int count = prevEntry.Songs.Count;
                    if (pos > count + 1){
                        Console.WriteLine($"Position {count + 1} needs a song before " +
                                          $"position {pos}. Adding new song to position {count + 1}");
                        pos = count + 1;
                    }
                    looping = false;
                }
            }
            Console.WriteLine("\nWhat is your new favourite songs' title?\n");
            string title = Console.ReadLine();
            Console.WriteLine("\nWho is the artist?\n");
            string artist = Console.ReadLine();

            List<Song> songs = prevEntry.Songs;
            Song newSong = new Song(title, artist);
            songs.Insert(pos - 1, newSong);
            if (songs.Count > 3){
                songs.RemoveAt(3);
            }
            Entry newEntry = new Entry(DateTime.Now.ToString("MM/dd/yyyy"), songs);

            return newEntry;
        }

        static void del_song(Entry prevEntry)
        {

        }

        static void search_favourites()
        {

        }

        static void info(){
            Console.WriteLine("\nSong Tracker is a small command line program that helps you track your "
                             +"favourite songs throughout time.\nFrom the main menu, choose \"Add new entry\" "
                             +"to create a new favourite songs list. Each new entry will be\ngiven a time stamp "
                             +"so it can be searched by date created. Choose \"Search previous favourite songs\" "
                             +"from\nthe main menu to view past favourite song lists.\n");
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
            StringBuilder sb = new StringBuilder();

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
