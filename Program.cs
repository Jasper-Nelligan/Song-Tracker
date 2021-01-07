﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

//https://json2csharp.com

namespace Song_Tracker
{
    class Program
    {
        /// <summary> Loads in song list data from song_list.json </summary>
        /// <returns> 
        /// Instance of JsonRoot, which contains all data from file.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Raised if song_list.json does not exist in the program directory.
        /// <exception>
        /// <exception cref="JsonException">
        /// Raised if there was an error reading in data from song_list.json.
        /// </exception>
        static JsonRoot LoadJson()
        {
            
            using (StreamReader r = new StreamReader("song_list.json"))
            {
                string json = r.ReadToEnd();
                JsonRoot jsonRoot = JsonConvert.DeserializeObject<JsonRoot>(json);
                return jsonRoot;
            }
        }

        /// <summary> Writes data from jsonRoot into song_list.json </summary>
        /// <param name="jsonRoot">
        /// The instance of jsonRoot containing the data to write to file.
        /// </param>
        /// <exception cref="JsonException">
        /// Raised if there was an error writing data to song_list.json.
        /// </exception>
        static void WriteJson(JsonRoot jsonRoot){
            string jsonString = JsonConvert.SerializeObject(jsonRoot, Formatting.Indented);
            System.IO.File.WriteAllText("song_list.json", jsonString);
        }
        
        static void Main()
        {
            JsonRoot jsonRoot = null;
            try{
                jsonRoot = LoadJson();
            } catch (FileNotFoundException){
                Console.WriteLine("Error: song_list.json was not found in the program directory.");
                System.Environment.Exit(1);
            } catch (JsonException){
                Console.WriteLine("Error: could not read data from song_list.json");
                System.Environment.Exit(1);
            }
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
                        Entry newEntry = CreateNewEntry(prevEntry);
                        SaveNewEntry(ref jsonRoot, in newEntry);
                        Console.WriteLine("\nNew Entry has been saved.");
                        break;
                    case "2":
                        SearchFavourites();
                        break;
                    case "3":
                        Info();
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

        /// <summary> 
        /// Allows user to create a new entry by modifying their previous entry
        /// </summary>
        /// <param name="prevEntry"> The last entry made by the user </param>
        /// <returns> The new modified entry </returns>
        static Entry CreateNewEntry(Entry prevEntry)
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
                        Entry newEntry = AddNewSong(prevEntry);
                        if (newEntry != null)
                        {
                            prevEntry = newEntry;
                        }
                        continue;
                    case "2":
                        DelSong(prevEntry);
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
        static Entry AddNewSong(Entry prevEntry)
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
                    catch (FormatException){
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

        static Entry DelSong(Entry prevEntry)
        {
            return null;
        }

        /// <summary> Adds newEntry to jsonRoot and saves data to file </summary>
        /// <param name="jsonRoot"> 
        /// A reference to the instance of JsonRoot containing all song data
        /// </param>
        /// <param name="newEntry"> 
        /// New instance of Entry that needs to be saved to file.
        /// </param>
        static void SaveNewEntry(ref JsonRoot jsonRoot, in Entry newEntry){
            List<Entry> entries = jsonRoot.Entries;
            int endIndex = entries.Count - 1; 
            Entry endNode = entries[endIndex];
            // Previous entries on the same day will be removed
            while(endNode.Date == newEntry.Date){
                entries.RemoveAt(endIndex--);
                endNode = entries[endIndex];
            }
            entries.Add(newEntry);
            try{
                WriteJson(jsonRoot);
            } catch (JsonException){
                Console.WriteLine("Error: data could not be written to file.");
                System.Environment.Exit(1);
            }
        }

        static void SearchFavourites()
        {

        }

        static void Info(){
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
