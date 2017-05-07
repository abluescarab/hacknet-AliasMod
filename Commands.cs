using System.Collections.Generic;
using Hacknet;
using Pathfinder.Util;

namespace AliasMod {
    static class Commands {
        public static class AliasCmd {
            public static string Key = "alias";
            public static string Description = "Add aliases for commands";

            private static bool firstRun = true;

            private static string usage =
                "Usage: alias [-h] [-l] [-i] [name[=value] ... ]" +
                "\n" +
                "\n    -h    display usage help" +
                "\n    -l    reload aliases from alias file" +
                "\n    -i    display mod info" +
                "\n" +
                "\nExamples:" +
                "\n    alias ssh='SSHcrack 22'" +
                "\n    alias sh='shell'" +
                "\n" +
                "\nTo create a multicommand alias:" +
                "\n    alias hack='SSHcrack 22; FTPBounce 21'";

            /// <summary>
            /// Run the alias command.
            /// </summary>
            public static bool RunCommand(OS os, List<string> args) {
                if(firstRun) {
                    Load(os);
                }

                os.write("\n");

                if(args.Count < 2) {
                    os.write("Type \"alias -h\" to see usage instructions.");
                    os.write("\n");
                    Show(os);
                }
                else {
                    int startArg = 1;

                    while(ParseArg(os, args[startArg])) {
                        startArg++;
                    }

                    string name = args[startArg];

                    if(!args[startArg].Contains("=")) {
                        if(AliasMod.aliases.ContainsKey(name)) {
                            Logger.Verbose(AliasMod.aliases[name].Command);
                            os.write(KeyValueFile.ToKeyValueString(name, AliasMod.aliases[name].Command));
                        }
                        else {
                            os.write("Alias '" + name + "' not found");
                        }
                    }
                    else {
                        os.write("Alias added: " + Add(os, string.Join(" ", args.ToArray(), startArg, args.Count - startArg))
                            .Name);
                    }
                }

                return true;
            }

            /// <summary>
            /// Parse an argument.
            /// </summary>
            /// <returns>true if option (--) arg; false otherwise</returns>
            private static bool ParseArg(OS os, string arg) {
                bool option = true;

                switch(arg) {
                    case "-h":
                        os.write(usage);
                        break;
                    case "-l":
                        Load(os);
                        break;
                    case "-i":
                        ShowInfo(os);
                        break;
                    default:
                        option = false;
                        break;
                }

                return option;
            }

            /// <summary>
            /// Show info about the mod.
            /// </summary>
            private static void ShowInfo(OS os) {
                os.write(
                    AliasMod.Name +
                    "\nVersion: " + AliasMod.Version +
                    "\nAuthor: " + AliasMod.Author +
                    "\nHomepage: " + AliasMod.Homepage
                );
            }

            /// <summary>
            /// Show the list of aliases.
            /// </summary>
            private static void Show(OS os) {
                if(AliasMod.aliases.Count < 1) {
                    os.write("There are no aliases.");
                }
                else {
                    foreach(Alias alias in AliasMod.aliases.Values) {
                        os.write(alias.Name);
                    }
                }
            }

            /// <summary>
            /// Add an alias.
            /// </summary>
            private static Alias Add(OS os, string alias) {
                Alias al = null;
                
                string name = alias.Remove(alias.IndexOf('='));
                string command = KeyValueFile.StripQuotes(alias.Remove(0, alias.IndexOf('=') + 1));

                if(AliasMod.aliases.ContainsKey(name)) {
                    al = AliasMod.aliases[name];
                    al.Command = command;
                    AliasMod.File.Replace(name, command);
                }
                else {
                    al = new Alias(name, command);
                    AliasMod.aliases[name] = al;
                    AliasMod.File.Append(name, command);
                }

                return al;
            }

            /// <summary>
            /// Reload the aliases from their file.
            /// </summary>
            public static void Load(OS os) {
                if(AliasMod.aliases == null) {
                    AliasMod.aliases = new Dictionary<string, Alias>();
                }
                else AliasMod.aliases.Clear();

                if(!string.IsNullOrWhiteSpace(AliasMod.File.Data)) {
                    foreach(KeyValuePair<string, string> pair in AliasMod.File.All()) {
                        Alias alias = new Alias(pair.Key, pair.Value);
                        AliasMod.aliases[pair.Key] = alias;
                    }
                }

                if(firstRun) firstRun = false;

                os.write("Loaded aliases.");
            }
        }

        public static class UnaliasCmd {
            public static string Key = "unalias";
            public static string Description = "Remove aliases for commands";

            private static string usage = "Usage: unalias <name>";

            /// <summary>
            /// Run the alias command.
            /// </summary>
            public static bool RunCommand(OS os, List<string> args) {
                os.write("\n");

                if(args.Count < 2) {
                    os.write(usage);
                    return false;
                }

                if(Remove(os, args[1])) os.write("Alias removed: " + args[1]);

                return true;
            }

            /// <summary>
            /// Remove an alias.
            /// </summary>
            private static bool Remove(OS os, string name) {
                if(!AliasMod.aliases.ContainsKey(name)) {
                    return false;
                }

                AliasMod.aliases[name].Command = "";
                AliasMod.File.Remove(name);
                AliasMod.aliases.Remove(name);
                return true;
            }
        }
    }
}