using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using System.Text.Json.Serialization;

namespace MuteSounds;

public class MuteSoundsConfig : BasePluginConfig
{
    [JsonPropertyName("menu_type")]
    public string MenuType { get; set; } = "WasdMenu";

    [JsonPropertyName("command_aliases")]
    public List<string> CommandAliases { get; set; } = new() { "css_mutesounds", "css_sounds", "css_seskapat", "css_sesler" };
}