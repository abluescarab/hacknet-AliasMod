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

        public bool RunCommand(OS os, string[] args) {
            if(string.IsNullOrEmpty(Command))
                return false;

            if(Command.Contains(";")) {
                string[] commands = Command.Split(';');

                foreach(string command in commands) {
                    os.execute(command.Trim());
                }
            }
            else {
                if(args.Length > 1) {
                    string command = string.Join(" ", args);
                    command = command.Substring(command.IndexOf(Name) + Name.Length + 1);
                    command = Command + (Regex.IsMatch(command, @"^(/|\))") ? "" : " ") + command;
                    os.execute(command);
                }
                else {
                    os.execute(Command);
                }
            }

            return true;
        }
    }
}
