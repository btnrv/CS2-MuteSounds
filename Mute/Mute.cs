using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Entities;

namespace MuteSounds;

public partial class MuteSounds
{
    // Dictionary to store player mute preferences: [Player][MuteType] = MuteTarget
    public Dictionary<CCSPlayerController, Dictionary<MuteType, MuteTarget>> PlayerMutePreferences = new();
    
    // Legacy: MVP Music is not targetable
    public List<CCSPlayerController> MVPMusicMuters = new();
    
    public bool HasMuteEnabled(CCSPlayerController player, MuteType muteType)
    {
        return PlayerMutePreferences.ContainsKey(player) && PlayerMutePreferences[player].ContainsKey(muteType);
    }
    public MuteTarget? GetMuteTarget(CCSPlayerController player, MuteType muteType)
    {
        if (HasMuteEnabled(player, muteType))
            return PlayerMutePreferences[player][muteType];
        return null;
    }
    public void SetMutePreference(CCSPlayerController player, MuteType muteType, MuteTarget target)
    {
        if (!PlayerMutePreferences.ContainsKey(player))
            PlayerMutePreferences[player] = new Dictionary<MuteType, MuteTarget>();
        PlayerMutePreferences[player][muteType] = target;
    }
    public void RemoveMutePreference(CCSPlayerController player, MuteType muteType)
    {
        if (PlayerMutePreferences.ContainsKey(player))
        {
            PlayerMutePreferences[player].Remove(muteType);
            if (PlayerMutePreferences[player].Count == 0)
                PlayerMutePreferences.Remove(player);
        }
    }
    public bool ShouldMuteSound(CCSPlayerController listener, CBaseEntity? soundSource, MuteTarget target)
    {
        // If we can't determine teams, don't mute (except for Everyone)
        if (soundSource?.TeamNum == null || listener?.TeamNum == null)
            return target == MuteTarget.Everyone;
        
        switch (target)
        {
            case MuteTarget.Everyone:
                return true;
            case MuteTarget.Team:
                return soundSource.TeamNum == listener.TeamNum;
            case MuteTarget.Enemy:
                return soundSource.TeamNum != listener.TeamNum;
            default:
                return false;
        }
    }

    public void HookMuteSounds()
    {
        HookUserMessage(208, um =>
        {
            if (um.Recipients == null || um.Recipients.Count == 0)
                return HookResult.Continue;

            var soundevent = um.ReadUInt("soundevent_hash");
            if (!Footsteps.SoundEventActions.Contains(soundevent) && !PlayerHurt.SoundEventActions.Contains(soundevent))
                return HookResult.Continue;

            var entityIndex = um.ReadInt("source_entity_index");
            var entity = Utilities.GetEntityFromIndex<CBaseEntity>(entityIndex);

            if (entity == null)
                return HookResult.Continue;

            if (Footsteps.SoundEventActions.Contains(soundevent))
            {
                foreach (var kvp in PlayerMutePreferences.ToList())
                {
                    var p = kvp.Key;
                    if (p == null || !p.IsValid || p.Connected != PlayerConnectedState.PlayerConnected)
                    {
                        if (p != null)
                            PlayerMutePreferences.Remove(p);
                        continue;
                    }
                    
                    if (!kvp.Value.ContainsKey(MuteType.Footsteps))
                        continue;
                    
                    var target = kvp.Value[MuteType.Footsteps];
                    if (ShouldMuteSound(p, entity, target))
                        um.Recipients.Remove(p);
                }
                return HookResult.Continue;
            }

            if (PlayerHurt.SoundEventActions.Contains(soundevent))
            {
                foreach (var kvp in PlayerMutePreferences.ToList())
                {
                    var p = kvp.Key;
                    if (p == null || !p.IsValid || p.Connected != PlayerConnectedState.PlayerConnected)
                    {
                        if (p != null)
                            PlayerMutePreferences.Remove(p);
                        continue;
                    }
                    
                    if (!kvp.Value.ContainsKey(MuteType.PlayerSounds))
                        continue;
                    
                    var target = kvp.Value[MuteType.PlayerSounds];
                    if (ShouldMuteSound(p, entity, target))
                        um.Recipients.Remove(p);
                }
                return HookResult.Continue;
            }
            return HookResult.Continue;

        }, HookMode.Pre);

        // Gun sounds
        HookUserMessage(452, um =>
        {
            if (um.Recipients == null || um.Recipients.Count == 0)
                return HookResult.Continue;

            uint entityHandle = um.ReadUInt("player");
            uint pawnIndex = entityHandle & (Utilities.MaxEdicts - 1);
            var player = Utilities.GetEntityFromIndex<CCSPlayerController>((int)pawnIndex);

            if (player == null)
                return HookResult.Continue;

            foreach (var kvp in PlayerMutePreferences.ToList())
            {
                var p = kvp.Key;
                if (p == null || !p.IsValid || p.Connected != PlayerConnectedState.PlayerConnected)
                {
                    if (p != null)
                        PlayerMutePreferences.Remove(p);
                    continue;
                }
                
                if (!kvp.Value.ContainsKey(MuteType.GunSounds))
                    continue;
                
                var target = kvp.Value[MuteType.GunSounds];
                if (ShouldMuteSound(p, player, target))
                    um.Recipients.Remove(p);
            }
            return HookResult.Continue;

        }, HookMode.Pre);

        RegisterEventHandler<EventRoundMvp>(OnEventRoundMvp);
    }

    public HookResult OnEventRoundMvp(EventRoundMvp @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        var player = @event.Userid;
        if (player == null || !player.IsValid) return HookResult.Continue;

        // If there are players who want to mute MVP music, emit stop sound to them
        if (MVPMusicMuters.Count > 0)
        {
            EmitSoundToPlayers("StopSoundEvents.StopAllMusic", MVPMusicMuters);
        }

        return HookResult.Continue;
    }

    public void EmitSoundToPlayers(string soundEventName, List<CCSPlayerController> recipients)
    {
        if (string.IsNullOrEmpty(soundEventName) || recipients == null || recipients.Count == 0) return;

        var worldEntity = Utilities.GetEntityFromIndex<CBaseEntity>(0);
        if (worldEntity == null || !worldEntity.IsValid || !worldEntity.DesignerName.Contains("world")) return;

        // Clean up invalid players and emit sound to each valid recipient
        foreach (var player in recipients.ToList())
        {
            if (player == null || !player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected)
            {
                if (player != null)
                    MVPMusicMuters.Remove(player);
                continue;
            }

            var recipientFilter = new RecipientFilter(player);
            worldEntity.EmitSound(soundEventName, recipientFilter);
        }
    }
}

public static class Footsteps
{
    public static readonly HashSet<uint> SoundEventActions = new HashSet<uint>
    {
        2800858936, 70011614, 1194677450, 1016523349, 2240518199, 3218103073, 520432428, 1818046345,
        2207486967, 2302139631, 1939055066, 1409986305, 1803111098, 4113422219, 3997353267, 3009312615,
        123085364, 782454593, 3257325156, 3434104102, 2745524735, 117596568, 29217150, 3460445620,
        2684452812, 2067683805, 1388885460, 413358161, 988265811, 3802757032, 2633527058, 1627020521,
        602548457, 859178236, 3749333696, 2899365092, 2061955732, 1535891875, 3368720745, 3057812547,
        135189076, 2790760284, 2448803175, 3753692454, 3666896632, 3166948458, 3099536373, 1690105992,
        115843229, 1763490157, 2546391140, 515548944, 1517575510, 1248619277, 1395892944, 2300993891,
        1183624286, 540697918, 2829617974, 1826799645, 3193435079, 2860219006, 1855038793, 2892812682,
        3342414459, 144629619, 721782259, 2133235849, 3161194970, 819435812, 2804393637, 4222899547,
        1664187801, 2714245023, 1692050905, 961838155, 2638406226, 3008782656, 2070478448, 1247386781,
        58439651, 3172583021, 1557420499, 1485322532, 1598540856, 4163677892, 4082928848, 2708661994,
        893108375, 1506215040, 2231399653, 1116700262, 2594927130, 1019414932, 1218015996, 417910549,
        3299941720, 931543849, 2026488395, 84876002, 1403457606, 2189706910, 1543034, 892882552,
        70939233, 1404198078, 1664329401, 822973253, 3797950766, 4203793682, 3952104171, 1163426340,
        870100484, 935062317, 1161855519, 1253503839, 1635413700, 2333790984, 96240187, 1165397261,
        4084367249, 3109879199, 3984387113, 4045299578, 2551626319, 2479376962, 4085076160, 1661204257,
        2236021746, 1440734007, 585390608, 1194093029, 3755338324, 4152012084, 757978684, 1448154350,
        2053595705, 1909915699, 765706800, 2722081556, 1540837791, 3123711576, 1770765328, 1761772772,
        1424056132, 4160462271, 3806690332, 740474905
    };
}

public static class PlayerHurt
{
    public static readonly HashSet<uint> SoundEventActions = new HashSet<uint>
    {
        3573863551, 2831007164, 3663896169, 2020934318, 3535174312, 2310318859,
        3124768561, 708038349, 524041390, 2323025056, 1771184788, 3396420465,
        856190898, 3767841471, 2447320252, 1823342283, 3988751453, 2192712263,
        427534867, 604181152, 46413566, 1815352525,
        3475734633, 1769891506, 2486534908, 3634660983
    };
}