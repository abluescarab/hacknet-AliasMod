using System.Collections.Generic;
using Hacknet;
using Pathfinder.Command;

namespace AliasMod {
    static class Commands {
        public static class Aliases {
            public static Dictionary<string, Alias> aliases;
            public static string Key = "alias";
            public static string Description = "Add aliases for commands";

            private static bool firstRun = true;

            private static string defaultUsage =
                "Usage:" +
                "\n    alias show (alias) - show a list of aliases or display what an alias does" +
                "\n    alias load - load the aliases from " + AliasMod.Filename +
                "\n    alias set [alias] \"[command]\" - set or replace an alias" +
                "\n    alias remove [alias] - remove an alias" +
                "\n    alias info - show info about the mod";

            /// <summary>
            /// Run the alias command.
            /// </summary>
            public static bool RunCommand(OS os, List<string> args) {
                if(firstRun) {
                    Load(os);
                    firstRun = false;
                }

                os.write("\n");

                if(args.Count < 2) {
                    os.write(defaultUsage);
                    return false;
                }

                /* args[0] = alias
                 * args[1] = [show/set/remove]
                 * args[2] = [alias]
                 * args[3] = [command] */
                if(args[1].Equals("show")) {
                    if(args.Count < 3) {
                        Show(os);
                    }
                    else {
                        if(aliases.ContainsKey(args[2])) {
                            os.write(args[2] + "=\"" + aliases[args[2]].Command + "\"");
                        }
                        else {
                            os.write("That alias does not exist.");
                        }
                    }
                }
                else if(args[1].Equals("load")) {
                    Load(os);
                }
                else if(args[1].Equals("info")) {
                    os.write(
                        AliasMod.Name +
                        "\nVersion: " + AliasMod.Version +
                        "\nAuthor: abluescarab" +
                        "\nHomepage: " + AliasMod.Homepage
                    );
                }
                else if(args[1].Equals("set")) {
                    if(args.Count < 4) {
                        os.write("Usage: alias set [alias] \"[command]\"");
                        return false;
                    }

                    string command = AliasUtils.StripQuotes(string.Join(" ", args.ToArray(), 3, args.Count - 3));
                    Alias alias = Add(os, args[2], command);

                    os.write("Added alias \"" + alias.Name + "\".");
                }
                else if(args[1].Equals("remove")) {
                    if(args.Count < 3) {
                        os.write("Usage: alias remove [alias]");
                        return false;
                    }

                    if(aliases.ContainsKey(args[2])) {
                        Remove(os, args[2]);
                        os.write("Removed alias \"" + args[2] + "\".");
                    }
                    else {
                        os.write("That alias does not exist.");
                    }
                }
                else {
                    os.write(defaultUsage);
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Show the list of aliases.
            /// </summary>
            private static void Show(OS os) {
                if(aliases.Count < 1) {
                    os.write("There are no aliases.");
                }
                else {
                    foreach(Alias alias in aliases.Values) {
                        os.write(alias.Name);
                    }
                }
            }

            /// <summary>
            /// Add an alias.
            /// </summary>
            private static Alias Add(OS os, string name, string command) {
                Alias alias = null;
                FileEntry file = GetFile(os);

                if(CanAdd(os, name)) {
                    if(aliases.ContainsKey(name)) {
                        alias = aliases[name];
                        alias.Command = command;
                        AliasUtils.Replace(file, name, command);
                    }
                    else {
                        alias = new Alias(name, command);
                        aliases[alias.Name] = alias;
                        AliasUtils.Append(file, name, command);
                    }

                    Handler.AddCommand(alias.Name, alias.RunCommand);
                }
                else {
                    os.write("Cannot create alias: name is reserved. Please try a different name.");
                }

                return alias;
            }

            /// <summary>
            /// Remove an alias.
            /// </summary>
            private static void Remove(OS os, string name) {
                aliases[name].Command = "";
                AliasUtils.Remove(GetFile(os), name);
                aliases.Remove(name);
            }

            /// <summary>
            /// Reload the aliases from their file.
            /// </summary>
            private static void Load(OS os) {
                FileEntry file = GetFile(os);

                if(aliases == null) {
                    aliases = new Dictionary<string, Alias>();
                }
                else aliases.Clear();

                if(!string.IsNullOrWhiteSpace(file.data)) {
                    int lines = file.data.Split('\n').Length;

                    if(lines > 0) {
                        for(int i = 0; i < lines; i++) {
                            KeyValuePair<string, string> pair = AliasUtils.ToKeyValuePair(file, i);

                            if(!pair.Equals(default(KeyValuePair<string, string>))) {
                                Alias alias = new Alias(pair.Key, pair.Value);
                                aliases[pair.Key] = alias;

                                Handler.AddCommand(alias.Name, alias.RunCommand);
                            }
                        }
                    }
                }

                os.write("Loaded aliases.");
            }

            /// <summary>
            /// Get the file that aliases are stored in.
            /// </summary>
            private static FileEntry GetFile(OS os) {
                Folder folder = os.thisComputer.getFolderFromPath("sys");
                FileEntry file;

                if(!folder.containsFile(AliasMod.Filename)) {
                    file = new FileEntry("", AliasMod.Filename);
                    folder.files.Add(file);
                }
                else {
                    file = folder.searchForFile(AliasMod.Filename);
                }

                return file;
            }

            /// <summary>
            /// Check if a command can be added.
            /// </summary>
            private static bool CanAdd(OS os, string name) {
                return !ProgramList.programs.Contains(name) && !ProgramList.getExeList(os).Contains(name);
            }
        }
    }
}