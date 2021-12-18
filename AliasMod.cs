using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Hacknet;
using Pathfinder.Command;
using Pathfinder.Event;
using Pathfinder.Event.Gameplay;
using Pathfinder.Event.Loading;

namespace AliasMod {
    [BepInPlugin(GUID, Name, Version)]
    public class AliasMod : HacknetPlugin {
        public const string GUID = "io.github.abluescarab.AliasMod";
        public const string Name = "Alias Mod";
        public const string Version = "5.0";
        public const string Author = "abluescarab";
        public const string Homepage = "https://github.com/abluescarab/hacknet-AliasMod";

        public static KeyValueFile file;
        public static SortedDictionary<string, Alias> aliases;

        public override bool Load() {
            CommandManager.RegisterCommand(Commands.AliasCmd.Key, Commands.AliasCmd.RunCommand);
            CommandManager.RegisterCommand(Commands.UnaliasCmd.Key, Commands.UnaliasCmd.RunCommand);
            EventManager<OSLoadedEvent>.AddHandler(LoadAliases);
            EventManager<CommandExecuteEvent>.AddHandler(CheckCommand);

            return true;
        }

        public override bool Unload() {
            CommandManager.UnregisterCommand(Commands.AliasCmd.Key);
            CommandManager.UnregisterCommand(Commands.UnaliasCmd.Key);
            EventManager<OSLoadedEvent>.RemoveHandler(LoadAliases);
            EventManager<CommandExecuteEvent>.RemoveHandler(CheckCommand);

            aliases.Clear();
            aliases = null;
            file = null;

            return base.Unload();
        }

        /// <summary>
        /// Check if a command exists in the alias dictionary.
        /// </summary>
        private static void CheckCommand(CommandExecuteEvent e) {
            if(aliases == null || !aliases.TryGetValue(e.Args[0], out Alias alias))
                return;

            e.Cancelled = true;
            alias.RunCommand(e.Os, e.Args);
        }

        /// <summary>
        /// Load aliases on startup.
        /// </summary>
        private static void LoadAliases(OSLoadedEvent e) {
            Console.WriteLine("test");
            Commands.AliasCmd.Load(e.Os);
        }
    }
}