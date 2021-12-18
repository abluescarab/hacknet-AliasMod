# Alias Mod
This mod adds the ability to alias commands in Hacknet. You need [Hacknet Pathfinder](https://github.com/Arkhist/Hacknet-Pathfinder/releases) to use this mod.

**Note: Due to a bug in Pathfinder 5.0.1, your aliases will not automatically reload when the game starts. You can enter any "alias" or "unalias" command to load the file.**

## Usage

    Usage: alias [option] [name[=value] ... ]
    
        -l    reload aliases from alias file
        -v    display verbose alias list
        -h    display usage help
        -i    display mod info
    
    Examples:
        alias ssh='SSHcrack 22'
        alias sh='shell'
    
    To create a multicommand alias:
        alias hack='SSHcrack 22; FTPBounce 21'

    To remove an alias:
        unalias <name>

## Alias File
Aliases are stored in ~/sys/aliases.sys and reloaded every time the game is restarted. If you edit the file manually, you will have to use the `alias -l` command to reload your aliases. You can also remove this file completely to remove all of your aliases at once.
