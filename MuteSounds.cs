using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;

namespace MuteSounds;

public partial class MuteSounds : BasePlugin, IPluginConfig<MuteSoundsConfig>
{
    public override string ModuleName => "MuteSounds";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "connect rebornjb.com.tr";

    public MuteSoundsConfig Config { get ; set; } = new();

    public override void Load(bool hotReload)
    {
        HookMuteSounds();
    }

    public void OnConfigParsed(MuteSoundsConfig config)
    {
        Config = config;
        RegisterCommands();
    }
}
