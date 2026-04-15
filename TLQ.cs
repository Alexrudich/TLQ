using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;

namespace Natalya
{
    // This record holds the various properties for your mod
    public record ModMetadata : AbstractModMetadata
    {
        public override string ModGuid { get; init; } = "com.colo.TLQ";
        public override string Name { get; init; } = "TLQ";
        public override string Author { get; init; } = "Aebisher";
        public override List<string>? Contributors { get; init; } = ["Colo"];
        public override SemanticVersioning.Version Version { get; init; } = new("2.5.5");
        public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.13");
        public override List<string>? Incompatibilities { get; init; } = ["ReadJsonConfigExample"];
        public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
        public override string? Url { get; init; } = "https://github.com/sp-tarkov/server-mod-examples";
        public override bool? IsBundleMod { get; init; } = false;
        public override string License { get; init; } = "MIT2";
    }

    /// <summary>
    /// Feel free to use this as a base for your mod
    /// </summary>
    [Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
    public class Tlq(
        WTTServerCommonLib.WTTServerCommonLib wttCommon
    )
        : IOnLoad
    {
        public Task OnLoad()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Use WTT-CommonLib services
            wttCommon.CustomQuestService.CreateCustomQuests(assembly);

            return Task.CompletedTask;
        }
    }
}
