using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using CS2MenuManager;
using CS2MenuManager.API.Class;
using CS2MenuManager.API.Enum;
namespace MuteSounds;

public partial class MuteSounds
{
    public BaseMenu CreateMenu(string title)
    {
        var created = MenuManager.MenuByType(Config.MenuType, title, this);
        return created;
    }

    public BaseMenu? CreateMainMenu(CCSPlayerController player)
    {
        if (player == null || !player.IsValid) return null;

        var menu = CreateMenu(Localizer["MainMenuTitle"]);

        menu.AddItem(Localizer["KnifeSoundsMenu"], (player, info) =>
        {
            var knifeMenu = CreateKnifeSoundsMenu(player);
            knifeMenu?.Display(player, -1);
        });

        menu.AddItem(Localizer["FootstepSoundsMenu"], (player, info) =>
        {
            var footstepMenu = CreateFootstepSoundsMenu(player);
            footstepMenu?.Display(player, -1);
        });

        menu.AddItem(Localizer["GunSoundsMenu"], (player, info) =>
        {
            var gunMenu = CreateGunSoundsMenu(player);
            gunMenu?.Display(player, -1);
        });

        menu.AddItem(Localizer["MVPMusicMenu"], (player, info) =>
        {
            var mvpMenu = CreateMVPMusicMenu(player);
            mvpMenu?.Display(player, -1);
        });

        return menu;
    }

    public BaseMenu? CreateKnifeSoundsMenu(CCSPlayerController player)
    {
        if (player == null || !player.IsValid) return null;

        var menu = CreateMenu(Localizer["KnifeSoundsMenu"]);
        if (KnifeSoundMuters.Contains(player))
        {
            menu.AddItem(Localizer["UnmuteKnifeSounds"], (player, info) =>
            {
                KnifeSoundMuters.Remove(player);
                player.PrintToChat(Localizer["Prefix"] + Localizer["KnifeSoundsUnmuted"]);
                MenuManager.CloseActiveMenu(player);
            });
        }
        else
        {
            menu.AddItem(Localizer["MuteKnifeSounds"], (player, info) =>
            {
                KnifeSoundMuters.Add(player);
                player.PrintToChat(Localizer["Prefix"] + Localizer["KnifeSoundsMuted"]);
                MenuManager.CloseActiveMenu(player);
            });
        }

        menu.AddItem(Localizer["PlayerCountWithThisFilter", KnifeSoundMuters.Count], DisableOption.DisableHideNumber);

        return menu;
    }

    public BaseMenu? CreateFootstepSoundsMenu(CCSPlayerController player)
    {
        if (player == null || !player.IsValid) return null;

        var menu = CreateMenu(Localizer["FootstepSoundsMenu"]);
        if (FootstepSoundMuters.Contains(player))
        {
            menu.AddItem(Localizer["UnmuteFootstepSounds"], (player, info) =>
            {
                FootstepSoundMuters.Remove(player);
                player.PrintToChat(Localizer["Prefix"] + Localizer["FootstepSoundsUnmuted"]);
                MenuManager.CloseActiveMenu(player);
            });
        }
        else
        {
            menu.AddItem(Localizer["MuteFootstepSounds"], (player, info) =>
            {
                FootstepSoundMuters.Add(player);
                player.PrintToChat(Localizer["Prefix"] + Localizer["FootstepSoundsMuted"]);
                MenuManager.CloseActiveMenu(player);
            });
        }

        menu.AddItem(Localizer["PlayerCountWithThisFilter", FootstepSoundMuters.Count], DisableOption.DisableHideNumber);

        return menu;
    }

    public BaseMenu? CreateGunSoundsMenu(CCSPlayerController player)
    {
        if (player == null || !player.IsValid) return null;

        var menu = CreateMenu(Localizer["GunSoundsMenu"]);
        if (GunSoundMuters.Contains(player))
        {
            menu.AddItem(Localizer["UnmuteGunSounds"], (player, info) =>
            {
                GunSoundMuters.Remove(player);
                player.PrintToChat(Localizer["Prefix"] + Localizer["GunSoundsUnmuted"]);
                MenuManager.CloseActiveMenu(player);
            });
        }
        else
        {
            menu.AddItem(Localizer["MuteGunSounds"], (player, info) =>
            {
                GunSoundMuters.Add(player);
                player.PrintToChat(Localizer["Prefix"] + Localizer["GunSoundsMuted"]);
                MenuManager.CloseActiveMenu(player);
            });
        }

        menu.AddItem(Localizer["PlayerCountWithThisFilter", GunSoundMuters.Count], DisableOption.DisableHideNumber);

        return menu;
    }

    public BaseMenu? CreateMVPMusicMenu(CCSPlayerController player)
    {
        if (player == null || !player.IsValid) return null;

        var menu = CreateMenu(Localizer["MVPMusicMenu"]);
        if (MVPMusicMuters.Contains(player))
        {
            menu.AddItem(Localizer["UnmuteMVPMusic"], (player, info) =>
            {
                MVPMusicMuters.Remove(player);
                player.PrintToChat(Localizer["Prefix"] + Localizer["MVPMusicUnmuted"]);
                MenuManager.CloseActiveMenu(player);
            });
        }
        else
        {
            menu.AddItem(Localizer["MuteMVPMusic"], (player, info) =>
            {
                MVPMusicMuters.Add(player);
                player.PrintToChat(Localizer["Prefix"] + Localizer["MVPMusicMuted"]);
                MenuManager.CloseActiveMenu(player);
            });
        }

        menu.AddItem(Localizer["PlayerCountWithThisFilter", MVPMusicMuters.Count], DisableOption.DisableHideNumber);

        return menu;
    }
}