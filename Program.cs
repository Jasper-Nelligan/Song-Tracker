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
                if (jsonRoot.Entries == null){
                    jsonRoot.Entries = new List<Entry>();
                }
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
                        Entry prevEntry = null; 
                        if (entries.Count > 0)
                        {
                            prevEntry = entries[entries.Count - 1];
                        }
                        Entry newEntry = CreateNewEntry(prevEntry);
                        if (newEntry != null && newEntry.Songs.Count > 0){
                            SaveNewEntry(ref jsonRoot, in newEntry);
                            Console.WriteLine("\nNew Entry has been saved.");
                        }
                        else{
                            Console.WriteLine("\nThere must be at least 1 song in the list to create a new entry.");
                        }
                        break;
                    case "2":
                        SearchFavourites(in entries);
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
            Entry newEntry = null;
            while (true)
            {
                if ((prevEntry == null)){
                    Console.WriteLine("\nHere you can create your first entry! Try inserting your favourite "
                                     +"songs into the list.\n");
                }
                else if (prevEntry.Songs.Count > 0){
                    Console.WriteLine($"\nYour latest entry from {prevEntry.Date} was:");
                    Console.WriteLine(prevEntry.ToString());
                }
                else{
                    Console.WriteLine("\nYour previous entry is empty. Try adding some songs to it "
                                     +"to create a new entry!\n");
                }
                
                Console.WriteLine("What would you like to do? (input a number)");
                Console.WriteLine("1 - Insert song into list\n2 - Delete song from list\n3 - Save new entry and go back\n");
                var input = Console.ReadLine();
                switch(input)
                {
                    case "1":
                        newEntry = AddNewSong(prevEntry);
                        if (newEntry != null)
                        {
                            prevEntry = newEntry;
                        }
                        continue;
                    case "2":
                        newEntry = DelSong(prevEntry);
                        if (newEntry != null)
                        {
                            prevEntry = newEntry;
                        }
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
            if (prevEntry == null){
                prevEntry = new Entry("",new List<Song>());
            }
            if (prevEntry.Songs.Count > 0){
                bool looping = true;
                while (looping){
                    Console.WriteLine("\nWhat position would you like to insert your new favourite song?");
                    Console.WriteLine("(Type a number between 1 and 3, 4 to go back)\n");                   
                    try{
                        pos = int.Parse(Console.ReadLine());
                    }
                    catch (FormatException){
                        Console.WriteLine($"\nError: Not a valid answer.");
                        continue;
                    }
                    if (pos == 4){
                        return null;
                    }
                    int count = prevEntry.Songs.Count;
                    if (pos > count + 1){
                        Console.WriteLine($"\nPosition {count + 1} needs a song before " +
                                          $"position {pos}. Adding new song to position {count + 1}");
                        pos = count;
                    }
                    else{
                        pos -= 1;
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
            songs.Insert(pos, newSong);
            if (songs.Count > 3){
                songs.RemoveAt(3);
            }
            Entry newEntry = new Entry(DateTime.Now.ToString("MM/dd/yyyy"), songs);

            Console.WriteLine($"\n{newSong.Title} by {newSong.Artist} was added to position {pos}.");
            return newEntry;
        }

        /// <summary> Allows user to delete songs off their previous entry. 
        /// Creates new entry in the process.
        /// </summary>
        /// <param name="prevEntry"> A copy of the last entry on file </param>
        /// <returns> 
        /// A new instance of Entry, null if user decides not to change anything.
        /// </returns>
        static Entry DelSong(Entry prevEntry)
        {
            if (prevEntry == null || prevEntry.Songs.Count == 0)
            {
                Console.WriteLine("\nError: no songs to delete.");
                return null;
            }

            int pos = 0;
            bool looping = true;
            while (looping)
            {
                Console.WriteLine("\nWhat song would you like to delete?");
                Console.WriteLine("(type in a number between 1 and 3, 4 to go back)\n");

                try
                {
                    pos = int.Parse(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine($"\nError: Not a valid answer.");
                    continue;
                }
                if (pos == 4)
                {
                    return null;
                }
                if (pos < 0 || pos > 4){
                    Console.WriteLine($"\nError: position {pos} is out of range.");
                    continue;
                }
                if (pos > prevEntry.Songs.Count)
                {
                    if (pos < 4){
                        Console.WriteLine($"\nError: no song at position {pos}");
                        continue;
                    }
                }
                looping = false;
            }

            List<Song> songs = prevEntry.Songs;
            Song removedSong = songs[pos - 1];
            songs.RemoveAt(pos - 1);
            Entry newEntry = new Entry(DateTime.Now.ToString("MM/dd/yyyy"), songs);

            Console.WriteLine($"\n{removedSong.Title} by {removedSong.Artist} was removed from the list.");
            return newEntry;
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
            if (entries.Count > 0){
                int endIndex = entries.Count - 1;
                Entry endNode = entries[endIndex];
                // Previous entries on the same day will be removed
                while (endNode.Date == newEntry.Date)
                {
                    entries.RemoveAt(endIndex--);
                    endNode = entries[endIndex];
                }
            }
            entries.Add(newEntry);
            try{
                WriteJson(jsonRoot);
            } catch (JsonException){
                Console.WriteLine("Error: data could not be written to file.");
                System.Environment.Exit(1);
            }
        }

        static void SearchFavourites(in List<Entry> entries)
        {
            if (entries == null || entries.Count == 0){
                Console.WriteLine("\nError: no entries to search through.");
                return;
            }

            int endIndex = entries.Count - 1;
            int curIndex = endIndex;
            int curEntryNum = 1;
            int lastEntryNum;
            if (entries.Count > 9){
                lastEntryNum = 9;
            }
            else{
                lastEntryNum = entries.Count;
            }
            int loadMoreCount = 0;
            while(true){
                Console.WriteLine("\nPick a date to see what your favourite songs were.");
                Console.WriteLine("(type in a number, or 0 to go back to main menu)");
                int i, j = (lastEntryNum < 9) ? lastEntryNum : 9, k = curIndex;
                for(i = 0;i<j && k > 0;i++){
                    Console.WriteLine($"{curEntryNum++}. {entries[k--].Date}");
                }
                if (entries.Count > 9){
                    Console.WriteLine($"{curEntryNum++}. Load more dates");
                }
                Console.WriteLine("");

                bool inputValid = false;
                int pos = 0;
                while(!inputValid){
                    try
                    {
                        pos = int.Parse(Console.ReadLine());
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine($"\nError: Not a valid answer. (type in a number, or 0 to go back to main menu)\n");
                        continue;
                    }
                    if (pos < 0 || pos > lastEntryNum + 1){
                        Console.WriteLine($"\nError: Not a valid answer. (type in a number, or 0 to go back to main menu)\n");
                        continue;
                    }
                    inputValid = true;
                }
                
                if (pos == 0){
                    Console.WriteLine("\nReturning back to Main Menu");
                    return;
                }

                if (pos % 10 == 0){
                    lastEntryNum += 10;
                    curIndex -= 9;
                    loadMoreCount++;
                    continue;
                }
                else{
                    Entry viewEntry = entries[endIndex - (pos - 1) + loadMoreCount];
                    Console.WriteLine($"\nYour favourite songs from {viewEntry.Date} were:");
                    Console.WriteLine(viewEntry.ToString());
                    endIndex = entries.Count - 1;
                    curIndex = endIndex;
                    curEntryNum = 1;
                    if (entries.Count > 9)
                    {
                        lastEntryNum = 9;
                    }
                    else
                    {
                        lastEntryNum = entries.Count;
                    }
                    loadMoreCount = 0;
                }
            } // end while
        }

        static void Info(){
            Console.WriteLine("\nSong Tracker is a small command line program that helps you track your "
                             +"favourite songs throughout time.\nFrom the main menu, choose \"Add new entry\" "
                             +"to create a new favourite songs list. You can add 3 favourite \nsongs to each entry. "
                             +"Each new entry will be given a time stamp so it can be searched by date created. "
                             +"\nChoose \"Search previous favourite songs\" from the main menu to view past "
                             +"favourite song lists.");
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
