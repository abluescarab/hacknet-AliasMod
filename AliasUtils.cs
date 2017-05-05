using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Hacknet;

namespace AliasMod {
    static class AliasUtils {
        /// <summary>
        /// Append a key/value pair to a file.
        /// </summary>
        public static void Append(FileEntry file, string key, string value) {
            file.data += "\n" + ToKeyValueString(key, value);
        }

        /// <summary>
        /// Remove a key/value pair from a file.
        /// </summary>
        public static void Remove(FileEntry file, string key) {
            List<string> data = file.data.Split('\n').ToList();
            file.data = string.Join("\n", data.Where(l => !Regex.IsMatch(l, "^" + key + "=")).ToArray());
        }

        /// <summary>
        /// Replace a key/value pair in a file.
        /// </summary>
        public static void Replace(FileEntry file, string key, string newValue) {
            Remove(file, key);
            Append(file, key, newValue);
        }

        /// <summary>
        /// Combine a key string and value string into a key/value string.
        /// </summary>
        public static string ToKeyValueString(string key, string value) {
            return key + "=\"" + value + "\"";
        }

        /// <summary>
        /// Convert a line in a file into a key/value pair.
        /// </summary>
        /// <param name="line">line index</param>
        public static KeyValuePair<string, string> ToKeyValuePair(FileEntry file, int line) {
            List<string> data = file.data.Split('\n').ToList();
            string ln = data[line];
            return ToKeyValuePair(ln);
        }

        /// <summary>
        /// Convert a string into a key/value pair.
        /// </summary>
        public static KeyValuePair<string, string> ToKeyValuePair(string text) {
            string key = text.Substring(0, text.IndexOf('='));
            string value = StripQuotes(text.Substring(text.IndexOf('=') + 1));

            return new KeyValuePair<string, string>(key, value);
        }

        /// <summary>
        /// Strip the surrounding quotes from a string.
        /// </summary>
        public static string StripQuotes(string text) {
            return Regex.Replace(text, "^\"|\"$", "");
        }
    }
}
