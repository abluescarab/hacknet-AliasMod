using System.Collections.Generic;
using Hacknet;

namespace AliasMod {
    class Alias {
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
                os.execute(Command);
                return true;
            }
        }
    }
}
