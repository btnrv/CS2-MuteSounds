using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Commands;

namespace MuteSounds;

public partial class MuteSounds
{
    public void RegisterCommands()
    {
        foreach (var alias in Config.CommandAliases)
        {
            AddCommand(alias, Localizer["SoundsCommandDescription"], SoundMenuCommand);
        }
    }
    public void SoundMenuCommand(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null || !player.IsValid)
        {
            return;
        }
        var menu = CreateMainMenu(player);
        if (menu == null) return;

        menu.Display(player, -1);
    }
}