using System.Collections.Generic;
using System.Text.RegularExpressions;
using Hacknet;

namespace AliasMod {
    public class Alias {
        public string Name { get; }
        public string Command { get; set; }

        public Alias(string name, string command) {
            Name = name;
            Command = command;
        }

        public bool RunCommand(OS os, List<string> args) {
            if(string.IsNullOrEmpty(Command)) {
                return false;
            }
            else {
                if(args.Count > 1) {
                    string command = string.Join(" ", args.ToArray());
                    command = command.Substring(command.IndexOf(Name) + Name.Length + 1);
                    command = Command + (Regex.IsMatch(command, @"^(/|\))") ? "" : " ") + command;
                    os.execute(command);
                }
                else {
                    os.execute(Command);
                }

                return true;
            }
        }
    }
}
