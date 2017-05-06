﻿using System.Collections.Generic;
using Pathfinder;
using Pathfinder.Command;
using Pathfinder.Event;
using Pathfinder.Util;

namespace AliasMod {
    public class AliasMod : IPathfinderMod {
        public const string Name = "Alias Mod";
        public const string Version = "2_1";
        public const string Filename = "aliases.sys";
        public const string ID = Name + " v" + Version;
        public const string Homepage = "https://github.com/abluescarab/hacknet-aliasmod";
        public const string Author = "abluescarab";
        public string Identifier => ID;

        public static Dictionary<string, Alias> aliases;

        public void Load() {
            Logger.Verbose("Loading " + ID + "...");
            EventManager.RegisterListener<CommandSentEvent>(CheckCommand);
        }

        public void LoadContent() {
            Handler.AddCommand(Commands.AliasCmd.Key, Commands.AliasCmd.RunCommand, Commands.AliasCmd.Description, true);
            Handler.AddCommand(Commands.UnaliasCmd.Key, Commands.UnaliasCmd.RunCommand, Commands.UnaliasCmd.Description, true);
        }

        public void Unload() {
            Logger.Verbose("Unloading " + ID + "...");
            EventManager.UnregisterListener<CommandSentEvent>(CheckCommand);
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
    }
}