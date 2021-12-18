using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Hacknet;

namespace AliasMod {
    public class KeyValueFile {
        private string path;
        private bool doSort = true;

        public OS OS { get; private set; }
        public bool AutoSort { get; set; }
        public bool AutoSortDescending { get; set; }
        public string Name { get; private set; }
        public string Path {
            get => path;
            set => path = value.TrimEnd('/', '\\');
        }
        public string FullPath => Path + "/" + Name;
        public string Data {
            get => File.data;
            private set {
                File.data = value;

                if(AutoSort && doSort) SortAlphabetic(AutoSortDescending);
            }
        }

        private FileEntry File {
            get {
                Folder folder = OS.thisComputer.getFolderFromPath(path, true);
                FileEntry file;

                if(!folder.containsFile(Name)) {
                    file = new FileEntry("", Name);
                    folder.files.Add(file);
                }
                else {
                    file = folder.searchForFile(Name);
                }

                return file;
            }
        }

        public KeyValueFile(OS os, string name, string path, string data = "", bool autoSort = true, bool sortDescending = false) {
            OS = os;
            Name = name;
            Path = path;
            AutoSort = autoSort;
            AutoSortDescending = sortDescending;

            if(!string.IsNullOrWhiteSpace(data)) {
                Data = data;
            }

            Move(path);
        }

        /// <summary>
        /// Move the file.
        /// </summary>
        public void Move(string path) {
            string[] args = { FullPath, path };
            Programs.mv(args, OS);
        }

        /// <summary>
        /// Rename the file.
        /// </summary>
        public void Rename(string name) {
            string[] args = { Name, name };
            Programs.mv(args, OS);
        }

        /// <summary>
        /// Append a key/value pair.
        /// </summary>
        public void Append(string key, string value) {
            if(Data.Contains(key + "=")) {
                Remove(key);
            }

            Data += "\n" + ToKeyValueString(key, value);
        }

        /// <summary>
        /// Append a KeyValuePair.
        /// </summary>
        /// <param name="pair"></param>
        public void Append(KeyValuePair<string, string> pair) {
            Append(pair.Key, pair.Value);
        }

        /// <summary>
        /// Replace a key/value pair.
        /// </summary>
        public void Replace(string key, string newValue) {
            Remove(key);
            Append(key, newValue);
        }

        /// <summary>
        /// Replace a KeyValuePair.
        /// </summary>
        public void Replace(KeyValuePair<string, string> pair) {
            Replace(pair.Key, pair.Value);
        }

        /// <summary>
        /// Remove a key/value pair by key.
        /// </summary>
        public void Remove(string key) {
            List<string> data = Lines();
            Data = string.Join("\n", data.Where(l => !Regex.IsMatch(l, "^" + key + "=")).ToArray());
        }

        /// <summary>
        /// Get a list of all KeyValuePairs.
        /// </summary>
        public List<KeyValuePair<string, string>> All() {
            List<string> data = Lines();
            List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();

            foreach(string pair in data) {
                pairs.Add(ToKeyValuePair(pair));
            }

            return pairs;
        }

        /// <summary>
        /// Get a key/value pair by key as a string.
        /// </summary>
        public string AsString(string key) {
            List<string> data = Lines();
            return data.Find(d => d.Contains(key + "="));
        }

        /// <summary>
        /// Get a key/value pair by line number as a string.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public string AsString(int line) {
            List<string> data = Lines();
            return data[line];
        }

        /// <summary>
        /// Get a key/value pair by key as a KeyValuePair.
        /// </summary>
        public KeyValuePair<string, string> AsPair(string key) {
            string data = AsString(key);
            return ToKeyValuePair(data);
        }

        /// <summary>
        /// Get a key/value pair by line number as a KeyValuePair.
        /// </summary>
        public KeyValuePair<string, string> AsPair(int line) {
            string data = AsString(line);
            return ToKeyValuePair(data);
        }

        /// <summary>
        /// Clear the file.
        /// </summary>
        public void Clear() {
            Data = "";
        }

        /// <summary>
        /// Sort the file alphabetically by key.
        /// </summary>
        public void SortAlphabetic(bool descending) {
            doSort = false;

            if(descending) {
                Data = string.Join("\n", Lines().OrderByDescending(r => r));
            }
            else {
                Data = string.Join("\n", Lines().OrderBy(r => r));
            }

            doSort = true;
        }

        /// <summary>
        /// Get a list of all non-blank lines.
        /// </summary>
        private List<string> Lines() {
            return Data.Split('\n').Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
        }

        /// <summary>
        /// Combine a key string and value string into a key/value string.
        /// </summary>
        public static string ToKeyValueString(string key, string value) {
            return key + "='" + value + "'";
        }

        /// <summary>
        /// Convert a key/value pair to a string.
        /// </summary>
        public static string ToKeyValueString(KeyValuePair<string, string> pair) {
            return pair.Key + "='" + pair.Value + "'";
        }

        /// <summary>
        /// Convert a string into a key/value pair.
        /// </summary>
        public static KeyValuePair<string, string> ToKeyValuePair(string text) {
            if(!string.IsNullOrWhiteSpace(text)) {
                string key = text.Remove(text.IndexOf('='));
                string value = StripQuotes(text.Substring(text.IndexOf('=') + 1));

                return new KeyValuePair<string, string>(key, value);
            }

            return default(KeyValuePair<string, string>);
        }

        /// <summary>
        /// Strip the surrounding quotes from a string.
        /// </summary>
        public static string StripQuotes(string text) {
            return Regex.Replace(text, "^\'|\'$", "");
        }
    }
}
