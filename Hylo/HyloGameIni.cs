using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MvsxPackBuilder
{
    public class HyloGameIniParser
    {

        private static string ExtractParameter(string str)
        {
            // this is super inefficient, but whatever
            int startIndex = str.IndexOf('<');
            int endIndex = str.IndexOf('>');

            if (startIndex != -1 && endIndex != -1)
            {
                string tmp = str.Substring(startIndex + 1, endIndex - (startIndex + 1));
                return tmp;
            }

            return "";
        }

        private static Int32 ReadInt32(string str)
        {
            string parameter = ExtractParameter(str);
            int result = 0;
            if (Int32.TryParse(parameter, out result))
            {
                return result;
            }

            return 0;
        }

        private static string ReadString(string str)
        {
            string parameter = ExtractParameter(str);
            return parameter;
        }

        private Int32 ExtractGameIDFromFilename(string filename)
        {
            string token = "games";
            int startIndex = filename.IndexOf(token);
            int endIndex = filename.IndexOf(".ini");

            if (startIndex != -1 && endIndex != -1)
            {
                string tmp = filename.Substring(startIndex + token.Length, endIndex - (startIndex + token.Length));

                Int32 result;
                if (Int32.TryParse(tmp, out result))
                {
                    return result;
                }
            }

            return 0;
        }

        public static List<Hylo.GameEntry> Deserialize(string FilePath)
        {
            List<Hylo.GameEntry> gameEntries = new List<Hylo.GameEntry>();

            if(!File.Exists(FilePath))
            {
                return gameEntries;
            }

            using (StreamReader reader = File.OpenText(FilePath))
            {
                Hylo.GameEntry activeEntry = null;

                while (reader.Peek() >= 0)
                {
                    string lineOfText = reader.ReadLine();

                    if (lineOfText.Length == 0)
                        continue;

                    if (lineOfText.StartsWith('#')) // this is a comment, skip
                        continue;

                    // look for an open bracket
                    if (lineOfText.StartsWith('['))
                    {
                        // if we do not have an active entry, try to create one
                        if (activeEntry == null)
                        {
                            if (lineOfText.Contains(@"[GAME]"))
                            {
                                activeEntry = new Hylo.GameEntry();
                            }
                        }
                        else
                        {
                            // we do have a currently active entry
                            // parse the rest of the data

                            // look for the closing token
                            if (lineOfText.Contains(@"[GAME\]"))
                            {
                                // push the active entry
                                gameEntries.Add(activeEntry);
                                activeEntry = null;
                                continue;
                            }

                            // parse the parameter
                            string[] tokens = lineOfText.Split('=', StringSplitOptions.RemoveEmptyEntries);

                            // invalid parameter
                            if (tokens.Length != 2)
                                continue;

                            if (tokens[0].Contains(@"[ID]"))
                            {
                                activeEntry.ID = ReadInt32(tokens[1]);
                                continue;
                            }

                            if (tokens[0].Contains(@"[TIME]"))
                            {
                                activeEntry.Time = ReadString(tokens[1]);
                                continue;
                            }

                            if (tokens[0].Contains(@"[TYPE]"))
                            {
                                activeEntry.Type = ReadString(tokens[1]);
                                continue;
                            }

                            if (tokens[0].Contains(@"[NAME]"))
                            {
                                activeEntry.Name = ReadString(tokens[1]);
                                continue;
                            }

                            if (tokens[0].Contains(@"[DIR]"))
                            {
                                activeEntry.Dir = ReadString(tokens[1]);
                                continue;
                            }
                        }
                    }
                }
            }

            return gameEntries;
        }

        public static void Serialize(List<Hylo.GameEntry> GameEntries, string filename)
        {
            string directoryName = Path.GetDirectoryName(filename);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine("# auto generated by MvsxPackBuilder, do not edit!");
                writer.WriteLine("");

                foreach (Hylo.GameEntry entry in GameEntries)
                {
                    writer.WriteLine("");
                    writer.WriteLine(@"[GAME]");
                    writer.WriteLine(@"[ID]=<{0:d}>", entry.ID);
                    writer.WriteLine(@"[TIME]=<{0:d}>", string.IsNullOrEmpty(entry.Time) ? " " : entry.Time);
                    writer.WriteLine(@"[TYPE]=<{0:s}>", string.IsNullOrEmpty(entry.Type) ? " " : entry.Type);
                    writer.WriteLine(@"[NAME]=<{0:s}>", string.IsNullOrEmpty(entry.Name) ? " " : entry.Name);
                    writer.WriteLine(@"[DIR]=<{0:s}>", entry.Dir);
                    writer.WriteLine(@"[GAME\]");
                }
            }
        }
    }

}
