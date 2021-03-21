using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MvsxPackBuilder
{
    public class Hylo
    {
        public class GameEntry
        {
            public Int32 ID = 0; // unique id
            public string Time = ""; // the year?
            public string Type = ""; // not sure if this is used
            public string Name = ""; // the name to display
            public string Dir = ""; // the directory + name of the rom, also used to determine the platform...

            
            // None of the properties below are important for serialization
            // TODO: this should not really be here, but I am getting lazy now.
            public FBA.SupportedPlatforms Platform = FBA.SupportedPlatforms.Arcade;
            public Int32 FbaGameIndex = -1;
            public string GameName = "";
            public string CustomCoverPath = "";
        }

        public class Category
        {
            public string FolderName;
            public string DisplayName
            {
                get;
                set;
            }

            public string DisplayNameWithCount
            {
                get { return string.Format("{0:s} ({1:d})", DisplayName, Entries.Count); }
            }

            public List<Hylo.GameEntry> Entries = new List<Hylo.GameEntry>();
        }

        public void Reset()
        {
            Categories = new List<Category>();
        }

        public List<Category> Categories = new List<Category>();

        // the root path structure of the hylo hack
        static public string RootPath = @"res";

        // default name of the cover art inside each sub directory
        static public string CoverArtFilename = @"cover.png";

        // cover art folder, each cover art in within a <game_name> subfolder
        static public string CoverArtFolder = @"games";

        // path of the roms folder, each roms can go in there, or within a subfolder per platform.
        static public string RomsFolder = @"roms";

        // the local structure is as follow:
        // local/:  - contains the lang_array.ini which is used to create categories
        //          - also contains each "language subfolder" which contains the gamesX.ini
        static public string LocalFolder = @"local";

        // used to create new categories
        static public string LanguageIni = @"lang_array.ini";

        // default font name, one per language name
        static public string DefaultFontName = @"font.ttf";

        // the format for the games ini name, they all live in the English folder
        static public string GameIniFormat = @"games{0}.ini";

        static public string EnglishFolder = @"English";


        // helper function
        static public string GetCoverArtPath(string HyloPath)
        {
            return Path.Combine(HyloPath, RootPath, CoverArtFolder);
        }

        static public string GetRomPath(string HyloPath)
        {
            return Path.Combine(HyloPath, RootPath, RomsFolder);
        }

        static public string GetLocalPath(string HyloPath)
        {
            return Path.Combine(HyloPath, RootPath, LocalFolder);
        }

        static public List<string> GetLanguagesFolder(string HyloPath)
        {
            string localPath = GetLocalPath(HyloPath);
            return new List<string>(Directory.EnumerateDirectories(localPath));
        }


        static public string GetGamesIniFromCategoryIndex(string HyloPath, Int32 Index)
        {
            string localPath = GetLocalPath(HyloPath);
            string gamesIniPath = Path.Combine(localPath, EnglishFolder);
            return Path.Combine(gamesIniPath, string.Format("games{0:d}.ini", Index));
        }

        static public string GetOfficialGamesIniFromCategoryIndex(string HyloPath)
        {
            string localPath = GetLocalPath(HyloPath);
            string gamesIniPath = Path.Combine(localPath, EnglishFolder);
            return Path.Combine(gamesIniPath, "games.ini");
        }


        static public List<string> GetGamesIni(string HyloPath)
        {
            string localPath = GetLocalPath(HyloPath);
            string gamesIniPath = Path.Combine(localPath, EnglishFolder);
            return new List<string>(Directory.EnumerateFiles(gamesIniPath, @"games*.ini"));
        }

        static public string GetLanguageIni(string HyloPath)
        {
            string localPath = GetLocalPath(HyloPath);
            return Path.Combine(localPath, LanguageIni);
        }
    }
}
