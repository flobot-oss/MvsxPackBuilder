using DamienG.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;

namespace MvsxPackBuilder
{
    public class FBA
    {
        public class FbaGameMetadata
        {
            public FbaGameMetadata()
            {
            }

            public bool DoesRomFileExists = false;
            public bool DoesRomCrcMatch = true; // assume true until we run the extensive validation check
            public StringBuilder ValidationLog = new StringBuilder();
            public StringBuilder CrcLog = new StringBuilder();
        }

        private datafile[] Datafiles;
        private Dictionary<string, int>[] NameToDatIndexMapping;
        private FbaGameMetadata[][] GameMetadaPerPlatform;

        public Dictionary<string, int> GetNameToDatIndexMapping(SupportedPlatforms platform)
        {
            return NameToDatIndexMapping[(int)platform];
        }

        public enum SupportedPlatforms : int
        {
            Arcade = 0,
            //Colecovision,
            //GameGear,
            //MasterSystem,
            Megadrive,
            //MSX1,
            //PCEngine,
            //SG1000,
            //SuperGrafx,
            //TurboGrafx16
        };

        static public string[] PlatformPrefix = new string[] {
            "",
            //"cv_",
            //"gg_",
            //"sms_",
            "md_",
            //"msx_",
            //"pce_",
            //"sg1k_",
            //"sgx_",
            //"tg_",
        };

        static public string[] RomFolder = new string[] {
            "roms",
            //"coleco",
            //"gamegear",
            //"sms",
            "megadriv",
            //"msx",
            //"pce",
            //"sg1000",
            //"sgx",
            //"tg16",
        };

        static public FBA.SupportedPlatforms GetPlaformFromDirName(string dirName)
        {
            string filename = Path.GetFileName(dirName);
            int platformCount = Enum.GetNames(typeof(FBA.SupportedPlatforms)).Length;

            // skip arcade, as this is our fallthrough
            for (int platformIndex = 1; platformIndex < platformCount; ++platformIndex)
            {
                if (filename.StartsWith(FBA.PlatformPrefix[platformIndex]))
                {
                    return (FBA.SupportedPlatforms)platformIndex;
                }
            }

            return FBA.SupportedPlatforms.Arcade;
        }

        static public string GetRomsPrefixFolderFromPlatform(FBA.SupportedPlatforms platform, bool ignoreArcadePath)
        {
            if(ignoreArcadePath && platform == SupportedPlatforms.Arcade)
            {
                return "";
            }
            return FBA.RomFolder[(int)platform];
        }

        static public string GetGameNameWithoutPlatform(FBA.SupportedPlatforms platform, string iniDirName)
        {
            string filename = Path.GetFileName(iniDirName);

            if (FBA.PlatformPrefix[(int)platform].Length != 0)
            {
                filename = filename.Remove(0, FBA.PlatformPrefix[(int)platform].Length);
            }

            return filename;
        }

        static public string GetGameNameWithPlatform(FBA.SupportedPlatforms platform, string gameName)
        {
            if (platform != SupportedPlatforms.Arcade)
            {
                string tmp = PlatformPrefix[(int)platform] + gameName;
                return Path.Combine(RomFolder[(int)platform], tmp);
            }

            return gameName;
        }
        

        public List<string> GetFbaRomPathFromGameIndex(string RomsFolder, FBA.SupportedPlatforms platform, Int32 gameIndex)
        {
            List<string> romList = new List<string>();
            string rompath = Path.Combine(RomsFolder, GetRomsPrefixFolderFromPlatform(platform, false));

            datafile romset = GetDatafileFromPlatorm(platform);
            Dictionary<string, int> nameToGameIndex = GetNameToDatIndexMapping(platform);
            game fbaGame = romset.game[gameIndex];

            while(fbaGame != null)
            {
                romList.Add(Path.Combine(rompath, fbaGame.name + ".zip"));

                // check for parent hierarchy
                if (!string.IsNullOrEmpty(fbaGame.romof))
                {
                    Int32 parentIndex = 0;
                    if (nameToGameIndex.TryGetValue(fbaGame.romof, out parentIndex))
                    {
                        fbaGame = romset.game[parentIndex];
                        continue;
                    }
                }

                fbaGame = null;
            }

            return romList;
        }

        public void ValidateRomsExists(string RomsFolder, out StringBuilder log, out List<int> missingGameIndices)
        {
            log = new StringBuilder();
            missingGameIndices = new List<int>();

            foreach (SupportedPlatforms platform in Enum.GetValues(typeof(SupportedPlatforms)))
            {
                datafile currentDatFile = GetDatafileFromPlatorm(platform);
                string rompathPrefix = GetRomsPrefixFolderFromPlatform(platform, false);
                string rompath = Path.Combine(RomsFolder, rompathPrefix);

                if (!Directory.Exists(rompath))
                {
                    log.AppendLine(String.Format("[ERROR] Folder {2} does not exist: Roms from platform {0} needs to be in the {1} subfolder. ", platform, rompathPrefix, rompath));
                    return;
                }

                for (int gameIndex = 0; gameIndex < currentDatFile.game.Length; ++gameIndex)
                {
                    FbaGameMetadata metaData = GetFbaGameMetadataFromIndex(platform, gameIndex);

                    game currentGame = currentDatFile.game[gameIndex];
                    string filename = Path.ChangeExtension(currentGame.name, @".zip");
                    string filepath = Path.Combine(rompath, filename);

                    metaData.DoesRomFileExists = File.Exists(filepath);
                    metaData.ValidationLog.Clear();

                    if (!metaData.DoesRomFileExists)
                    {
                        metaData.ValidationLog.AppendLine(string.Format("[WARNING] Game \"{0}\" is missing, file needs to be placed here: {1}", currentGame.description, filepath));
                        missingGameIndices.Add(gameIndex);
                    }

                    log.Append(metaData.ValidationLog);
                }
            }
        }

        // TODO: move this onto an async job, as this can take a while
        // TODO: handle merge set correctly.
        private void ValidateRomsCRC(string RomsFolder, FBA.SupportedPlatforms platform, out StringBuilder log, out List<int> invalidGameIndices)
        {
            log = new StringBuilder();
            invalidGameIndices = new List<int>();

            datafile currentDatFile = GetDatafileFromPlatorm(platform);
            string rompathPrefix = GetRomsPrefixFolderFromPlatform(platform, false);
            string rompath = Path.Combine(RomsFolder, rompathPrefix);

            if (!Directory.Exists(rompath))
            {
                log.AppendLine(String.Format("[ERROR] Folder {2} does not exist: Roms from platform {0} needs to be in the {1} subfolder. ", platform, rompathPrefix, rompath));
                return;
            }

            for (int gameIndex = 0; gameIndex < currentDatFile.game.Length; ++gameIndex)
            {
                game currentGame = currentDatFile.game[gameIndex];
                string filename = Path.ChangeExtension(currentGame.name, @".zip");
                string filepath = Path.Combine(rompath, filename);

                FbaGameMetadata metaData = GetFbaGameMetadataFromIndex(platform, gameIndex);
                metaData.DoesRomCrcMatch = false;
                metaData.CrcLog.Clear();

                if (!File.Exists(filepath))
                {
                    metaData.CrcLog.AppendLine(string.Format("[WARNING] Game \"{0}\" is missing, file needs to be placed here: {1}", currentGame.description, filepath));
                    invalidGameIndices.Add(gameIndex);
                    continue;
                }

                using (ZipArchive archive = ZipFile.Open(filepath, ZipArchiveMode.Read))
                {
                    bool IsValid = true;
                    foreach (rom theRom in currentGame.rom)
                    {
                        ZipArchiveEntry zipEntry = archive.GetEntry(theRom.name);
                        if (zipEntry == null)
                        {
                            metaData.CrcLog.AppendLine(string.Format("[ERROR] Game \"{0}\" [{2}.zip] is missing rom {1}", currentGame.description, theRom.name, currentGame.name));
                            IsValid = false;
                            continue;
                        }

                        var memory = new byte[zipEntry.Length];
                        Span<byte> buffer = new Span<byte>(memory);
                        if (zipEntry.Open().Read(buffer) != zipEntry.Length)
                        {
                            metaData.CrcLog.AppendLine(string.Format("[ERROR] Game \"{0}\" [{2}.zip] Unable to read rom {1} in zip archive", currentGame.description, theRom.name, currentGame.name));
                            IsValid = false;
                            continue;
                        }

                        // validate CRC
                        UInt32 theCrc = Crc32.Compute(buffer.ToArray());
                        string crcInHex = theCrc.ToString("X").PadLeft(8, '0').ToLower();
                        if (crcInHex != theRom.crc)
                        {
                            metaData.CrcLog.AppendLine(string.Format("[ERROR] Game \"{0}\" [{4}.zip] rom {1} has an invalid CRC {2}, expected {3}", currentGame.description, theRom.name, crcInHex, theRom.crc, currentGame.name));
                            IsValid = false;
                            continue;
                        }
                    }

                    if (IsValid == false)
                    {
                        invalidGameIndices.Add(gameIndex);
                    }
                }

                log.Append(metaData.CrcLog);
            }
        }


        static public datafile LoadRomdat(string filename)
        {
            datafile datFile = new datafile();

            // start the load of the xml files
            XmlSerializer serializer = new XmlSerializer(typeof(datafile));

            using (Stream reader = new FileStream(filename, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                datFile = (datafile)serializer.Deserialize(reader);
            }

            return datFile;
        }

        public bool LoadAllRomdats(string RomdatFolder)
        {
            int platformCount = Enum.GetNames(typeof(FBA.SupportedPlatforms)).Length;

            Datafiles = new datafile[platformCount];

            Datafiles[(int)FBA.SupportedPlatforms.Arcade] = FBA.LoadRomdat(Path.Combine(RomdatFolder, @"FB Alpha v0.2.97.43 (ClrMame Pro XML).dat"));
            //Datafiles[(int)FBA.SupportedPlatforms.Colecovision] = FBA.LoadRomdat(Path.Combine(RomdatFolder, @"FB Alpha v0.2.97.43 (ClrMame Pro XML, ColecoVision only).dat"));
            //Datafiles[(int)FBA.SupportedPlatforms.GameGear] = FBA.LoadRomdat(Path.Combine(RomdatFolder, @"FB Alpha v0.2.97.43 (ClrMame Pro XML, Game Gear only).dat"));
            //Datafiles[(int)FBA.SupportedPlatforms.MasterSystem] = FBA.LoadRomdat(Path.Combine(RomdatFolder, @"FB Alpha v0.2.97.43 (ClrMame Pro XML, Master System only).dat"));
            Datafiles[(int)FBA.SupportedPlatforms.Megadrive] = FBA.LoadRomdat(Path.Combine(RomdatFolder, @"FB Alpha v0.2.97.43 (ClrMame Pro XML, Megadrive only).dat"));
            //Datafiles[(int)FBA.SupportedPlatforms.MSX1] = FBA.LoadRomdat(Path.Combine(RomdatFolder, @"FB Alpha v0.2.97.43 (ClrMame Pro XML, MSX 1 Games only).dat"));
            //Datafiles[(int)FBA.SupportedPlatforms.PCEngine] = FBA.LoadRomdat(Path.Combine(RomdatFolder, @"FB Alpha v0.2.97.43 (ClrMame Pro XML, PC-Engine only).dat"));
            //Datafiles[(int)FBA.SupportedPlatforms.SG1000] = FBA.LoadRomdat(Path.Combine(RomdatFolder, @"FB Alpha v0.2.97.43 (ClrMame Pro XML, Sega SG-1000 only).dat"));
            //Datafiles[(int)FBA.SupportedPlatforms.SuperGrafx] = FBA.LoadRomdat(Path.Combine(RomdatFolder, @"FB Alpha v0.2.97.43 (ClrMame Pro XML, SuprGrafx only).dat"));
            //Datafiles[(int)FBA.SupportedPlatforms.TurboGrafx16] = FBA.LoadRomdat(Path.Combine(RomdatFolder, @"FB Alpha v0.2.97.43 (ClrMame Pro XML, TurboGrafx16 only).dat"));

            // allocate name to index mapping
            NameToDatIndexMapping = new Dictionary<string, int>[platformCount];

            PopulateNameToDatIndexMapping(FBA.SupportedPlatforms.Arcade);
            //PopulateNameToDatIndexMapping(FBA.SupportedPlatforms.Colecovision);
            //PopulateNameToDatIndexMapping(FBA.SupportedPlatforms.GameGear);
            //PopulateNameToDatIndexMapping(FBA.SupportedPlatforms.MasterSystem);
            PopulateNameToDatIndexMapping(FBA.SupportedPlatforms.Megadrive);
            //PopulateNameToDatIndexMapping(FBA.SupportedPlatforms.MSX1);
            //PopulateNameToDatIndexMapping(FBA.SupportedPlatforms.PCEngine);
            //PopulateNameToDatIndexMapping(FBA.SupportedPlatforms.SG1000);
            //PopulateNameToDatIndexMapping(FBA.SupportedPlatforms.SuperGrafx);
            //PopulateNameToDatIndexMapping(FBA.SupportedPlatforms.TurboGrafx16);

            // allocate metadata
            GameMetadaPerPlatform = new FbaGameMetadata[platformCount][];
            for(Int32 platformIndex = 0; platformIndex < platformCount; ++platformIndex)
            {
                GameMetadaPerPlatform[platformIndex] = new FbaGameMetadata[Datafiles[platformIndex].game.Length];
                for(Int32 index = 0; index < GameMetadaPerPlatform[platformIndex].Length; ++index)
                {
                    GameMetadaPerPlatform[platformIndex][index] = new FbaGameMetadata();
                }
            }

            return true;
        }

        public void PopulateNameToDatIndexMapping(FBA.SupportedPlatforms platform)
        {
            // populate the NameToIndex dictionary
            datafile currentDatafile = GetDatafileFromPlatorm(platform);

            NameToDatIndexMapping[(int)platform] = new Dictionary<string, int>();

            for (Int32 GameIndex = 0; GameIndex < currentDatafile.game.Length; ++GameIndex)
            {
                NameToDatIndexMapping[(int)platform].Add(currentDatafile.game[GameIndex].name, GameIndex);
            }
        }

        public Int32 GetFbaGameIndexFromRomName(FBA.SupportedPlatforms platform, string romName)
        {
            return string.IsNullOrEmpty(romName) ? -1 : GetNameToDatIndexMapping(platform)[romName];
        }

        public FbaGameMetadata GetFbaGameMetadataFromIndex(FBA.SupportedPlatforms platform, Int32 gameIndex)
        {
            if (gameIndex != -1)
            {
                return GameMetadaPerPlatform[(int)platform][gameIndex];
            }
            return null;
        }


        public game GetGameFromIndex(FBA.SupportedPlatforms platform, int index)
        {
            return Datafiles[(int)platform].game[index];
        }

        public datafile GetDatafileFromPlatorm(FBA.SupportedPlatforms platform)
        {
            return Datafiles[(int)platform];
        }



    }
}
