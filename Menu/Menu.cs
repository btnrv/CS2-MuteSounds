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
        menu.PrevMenu = CreateMainMenu(player);
        
        var currentTarget = GetMuteTarget(player, MuteType.PlayerSounds);
        if (currentTarget.HasValue)
        {
            var item = menu.AddItem(Localizer["UnmuteKnifeSounds"], (player, info) =>
            {
                RemoveMutePreference(player, MuteType.PlayerSounds);
                player.PrintToChat(Localizer["Prefix"] + Localizer["KnifeSoundsUnmuted"]);
                CreateMainMenu(player)?.Display(player, -1);
            });
            item.PostSelectAction = PostSelectAction.Nothing;
            
            var targetName = currentTarget.Value switch
            {
                MuteTarget.Everyone => Localizer["TargetEveryone"],
                MuteTarget.Team => Localizer["TargetTeam"],
                MuteTarget.Enemy => Localizer["TargetEnemy"],
                _ => currentTarget.Value.ToString()
            };
            menu.AddItem(Localizer["CurrentTarget", targetName], DisableOption.DisableHideNumber);
        }
        else
        {
            var everyoneItem = menu.AddItem(Localizer["TargetEveryone"], (player, info) =>
            {
                SetMutePreference(player, MuteType.PlayerSounds, MuteTarget.Everyone);
                player.PrintToChat(Localizer["Prefix"] + Localizer["KnifeSoundsMuted"]);
                CreateMainMenu(player)?.Display(player, -1);
            });
            everyoneItem.PostSelectAction = PostSelectAction.Nothing;
            
            var teamItem = menu.AddItem(Localizer["TargetTeam"], (player, info) =>
            {
                SetMutePreference(player, MuteType.PlayerSounds, MuteTarget.Team);
                player.PrintToChat(Localizer["Prefix"] + Localizer["KnifeSoundsMuted"]);
                CreateMainMenu(player)?.Display(player, -1);
            });
            teamItem.PostSelectAction = PostSelectAction.Nothing;
            
            var enemyItem = menu.AddItem(Localizer["TargetEnemy"], (player, info) =>
            {
                SetMutePreference(player, MuteType.PlayerSounds, MuteTarget.Enemy);
                player.PrintToChat(Localizer["Prefix"] + Localizer["KnifeSoundsMuted"]);
                CreateMainMenu(player)?.Display(player, -1);
            });
            enemyItem.PostSelectAction = PostSelectAction.Nothing;
        }

        var playerCount = PlayerMutePreferences.Count(kvp => kvp.Value.ContainsKey(MuteType.PlayerSounds));
        menu.AddItem(Localizer["PlayerCountWithThisFilter", playerCount], DisableOption.DisableHideNumber);

        return menu;
    }

    public BaseMenu? CreateFootstepSoundsMenu(CCSPlayerController player)
    {
        if (player == null || !player.IsValid) return null;

        var menu = CreateMenu(Localizer["FootstepSoundsMenu"]);
        menu.PrevMenu = CreateMainMenu(player);
        
        var currentTarget = GetMuteTarget(player, MuteType.Footsteps);
        if (currentTarget.HasValue)
        {
            var item = menu.AddItem(Localizer["UnmuteFootstepSounds"], (player, info) =>
            {
                RemoveMutePreference(player, MuteType.Footsteps);
                player.PrintToChat(Localizer["Prefix"] + Localizer["FootstepSoundsUnmuted"]);
                CreateMainMenu(player)?.Display(player, -1);
            });
            item.PostSelectAction = PostSelectAction.Nothing;
            
            var targetName = currentTarget.Value switch
            {
                MuteTarget.Everyone => Localizer["TargetEveryone"],
                MuteTarget.Team => Localizer["TargetTeam"],
                MuteTarget.Enemy => Localizer["TargetEnemy"],
                _ => currentTarget.Value.ToString()
            };
            menu.AddItem(Localizer["CurrentTarget", targetName], DisableOption.DisableHideNumber);
        }
        else
        {
            var everyoneItem = menu.AddItem(Localizer["TargetEveryone"], (player, info) =>
            {
                SetMutePreference(player, MuteType.Footsteps, MuteTarget.Everyone);
                player.PrintToChat(Localizer["Prefix"] + Localizer["FootstepSoundsMuted"]);
                CreateMainMenu(player)?.Display(player, -1);
            });
            everyoneItem.PostSelectAction = PostSelectAction.Nothing;
            
            var teamItem = menu.AddItem(Localizer["TargetTeam"], (player, info) =>
            {
                SetMutePreference(player, MuteType.Footsteps, MuteTarget.Team);
                player.PrintToChat(Localizer["Prefix"] + Localizer["FootstepSoundsMuted"]);
                CreateMainMenu(player)?.Display(player, -1);
            });
            teamItem.PostSelectAction = PostSelectAction.Nothing;
            
            var enemyItem = menu.AddItem(Localizer["TargetEnemy"], (player, info) =>
            {
                SetMutePreference(player, MuteType.Footsteps, MuteTarget.Enemy);
                player.PrintToChat(Localizer["Prefix"] + Localizer["FootstepSoundsMuted"]);
                CreateMainMenu(player)?.Display(player, -1);
            });
            enemyItem.PostSelectAction = PostSelectAction.Nothing;
        }

        var playerCount = PlayerMutePreferences.Count(kvp => kvp.Value.ContainsKey(MuteType.Footsteps));
        menu.AddItem(Localizer["PlayerCountWithThisFilter", playerCount], DisableOption.DisableHideNumber);

        return menu;
    }

    public BaseMenu? CreateGunSoundsMenu(CCSPlayerController player)
    {
        if (player == null || !player.IsValid) return null;

        var menu = CreateMenu(Localizer["GunSoundsMenu"]);
        menu.PrevMenu = CreateMainMenu(player);
        
        var currentTarget = GetMuteTarget(player, MuteType.GunSounds);
        if (currentTarget.HasValue)
        {
            var item = menu.AddItem(Localizer["UnmuteGunSounds"], (player, info) =>
            {
                RemoveMutePreference(player, MuteType.GunSounds);
                player.PrintToChat(Localizer["Prefix"] + Localizer["GunSoundsUnmuted"]);
                CreateMainMenu(player)?.Display(player, -1);
            });
            item.PostSelectAction = PostSelectAction.Nothing;
            
            var targetName = currentTarget.Value switch
            {
                MuteTarget.Everyone => Localizer["TargetEveryone"],
                MuteTarget.Team => Localizer["TargetTeam"],
                MuteTarget.Enemy => Localizer["TargetEnemy"],
                _ => currentTarget.Value.ToString()
            };
            menu.AddItem(Localizer["CurrentTarget", targetName], DisableOption.DisableHideNumber);
        }
        else
        {
            var everyoneItem = menu.AddItem(Localizer["TargetEveryone"], (player, info) =>
            {
                SetMutePreference(player, MuteType.GunSounds, MuteTarget.Everyone);
                player.PrintToChat(Localizer["Prefix"] + Localizer["GunSoundsMuted"]);
                CreateMainMenu(player)?.Display(player, -1);
            });
            everyoneItem.PostSelectAction = PostSelectAction.Nothing;
            
            var teamItem = menu.AddItem(Localizer["TargetTeam"], (player, info) =>
            {
                SetMutePreference(player, MuteType.GunSounds, MuteTarget.Team);
                player.PrintToChat(Localizer["Prefix"] + Localizer["GunSoundsMuted"]);
                CreateMainMenu(player)?.Display(player, -1);
            });
            teamItem.PostSelectAction = PostSelectAction.Nothing;
            
            var enemyItem = menu.AddItem(Localizer["TargetEnemy"], (player, info) =>
            {
                SetMutePreference(player, MuteType.GunSounds, MuteTarget.Enemy);
                player.PrintToChat(Localizer["Prefix"] + Localizer["GunSoundsMuted"]);
                CreateMainMenu(player)?.Display(player, -1);
            });
            enemyItem.PostSelectAction = PostSelectAction.Nothing;
        }

        var playerCount = PlayerMutePreferences.Count(kvp => kvp.Value.ContainsKey(MuteType.GunSounds));
        menu.AddItem(Localizer["PlayerCountWithThisFilter", playerCount], DisableOption.DisableHideNumber);

        return menu;
    }

    public BaseMenu? CreateMVPMusicMenu(CCSPlayerController player)
    {
        if (player == null || !player.IsValid) return null;

        var menu = CreateMenu(Localizer["MVPMusicMenu"]);
        menu.PrevMenu = CreateMainMenu(player);
        if (MVPMusicMuters.Contains(player))
        {
            var item = menu.AddItem(Localizer["UnmuteMVPMusic"], (player, info) =>
            {
                MVPMusicMuters.Remove(player);
                player.PrintToChat(Localizer["Prefix"] + Localizer["MVPMusicUnmuted"]);
                CreateMainMenu(player)?.Display(player, -1);
            });
            item.PostSelectAction = PostSelectAction.Nothing;
        }
        else
        {
            var item = menu.AddItem(Localizer["MuteMVPMusic"], (player, info) =>
            {
                MVPMusicMuters.Add(player);
                player.PrintToChat(Localizer["Prefix"] + Localizer["MVPMusicMuted"]);
                CreateMainMenu(player)?.Display(player, -1);
            });
            item.PostSelectAction = PostSelectAction.Nothing;
        }

        menu.AddItem(Localizer["PlayerCountWithThisFilter", MVPMusicMuters.Count], DisableOption.DisableHideNumber);

        return menu;
    }
}