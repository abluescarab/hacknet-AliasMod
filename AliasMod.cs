using System.Collections.Generic;
using Pathfinder;
using Pathfinder.Command;
using Pathfinder.Util;

namespace AliasMod {
    public class AliasMod : IPathfinderMod {
        public const string Name = "Alias Mod";
        public const string Version = "2_0";
        public const string Filename = "aliases.sys";
        public const string ID = Name + " v" + Version;
        public const string Homepage = "https://github.com/abluescarab/hacknet-aliasmod";
        public string Identifier => ID;

        public static Dictionary<string, Alias> aliases;

        public void Load() {
            Logger.Verbose("Loading " + ID + "...");
        }

        public void LoadContent() {
            Handler.AddCommand(Commands.AliasCmd.Key, Commands.AliasCmd.RunCommand, Commands.AliasCmd.Description, true);
            Handler.AddCommand(Commands.UnaliasCmd.Key, Commands.UnaliasCmd.RunCommand, Commands.UnaliasCmd.Description, true);
        }

        public void Unload() {
            Logger.Verbose("Unloading " + ID + "...");
        }
    }
}