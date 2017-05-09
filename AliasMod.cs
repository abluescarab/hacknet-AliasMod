using System.Collections.Generic;
using Pathfinder;
using Pathfinder.Command;
using Pathfinder.Event;
using Pathfinder.Util;

namespace AliasMod {
    public class AliasMod : IPathfinderMod {
        public const string Name = "Alias Mod";
        public const string Version = "4_3";
        public static KeyValueFile File;
        public const string ID = Name + " v" + Version;
        public const string Homepage = "https://github.com/abluescarab/hacknet-aliasmod";
        public const string Author = "abluescarab";
        public string Identifier => ID;

        public static SortedDictionary<string, Alias> aliases;

        public void Load() {
            Logger.Verbose("Loading " + ID + "...");
            EventManager.RegisterListener<CommandSentEvent>(CheckCommand);
            EventManager.RegisterListener<OSPostLoadContenEvent>(LoadAliases);
        }

        public void LoadContent() {
            Handler.AddCommand(Commands.AliasCmd.Key, Commands.AliasCmd.RunCommand, Commands.AliasCmd.Description, true);
            Handler.AddCommand(Commands.UnaliasCmd.Key, Commands.UnaliasCmd.RunCommand, Commands.UnaliasCmd.Description, true);
        }

        public void Unload() {
            Logger.Verbose("Unloading " + ID + "...");
            EventManager.UnregisterListener<CommandSentEvent>(CheckCommand);
            EventManager.UnregisterListener<OSPostLoadContenEvent>(LoadAliases);
        }

        /// <summary>
        /// Check if a command exists in the alias dictionary.
        /// </summary>
        private void CheckCommand(CommandSentEvent e) {
            if(aliases.ContainsKey(e.Arguments[0])) {
                e.IsCancelled = true;
                aliases[e.Arguments[0]].RunCommand(e.OS, e.Arguments);
            }
        }

        /// <summary>
        /// Load aliases on startup.
        /// </summary>
        private void LoadAliases(OSPostLoadContenEvent e) {
            File = new KeyValueFile(e.OS, "aliases.sys", "sys");
            Commands.AliasCmd.Load(e.OS);
        }
    }
}