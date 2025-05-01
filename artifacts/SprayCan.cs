using Nanoray.PluginManager;
using Nickel;
using Sierra.ExternalAPIs.Kokoro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Sierra.artifacts.SprayCan;
using static Sierra.ExternalAPIs.Kokoro.IKokoroApi.IV2.IStatusLogicApi.IHook;

namespace Sierra.artifacts;

internal sealed class SprayCan : Artifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        Type type = MethodBase.GetCurrentMethod()!.DeclaringType!;
        helper.Content.Artifacts.RegisterArtifact(type.Name, new()
        {
            ArtifactType = type,
            Meta = new()
            {
                owner = ModEntry.Instance.SierraDeck.Deck,
                pools = [ArtifactPool.Boss],
                unremovable = false
            },
            Sprite = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/combat/SprayCan.png")).Sprite,
            Name = ModEntry.Instance.AnyLocs.Bind(["artifact", "SprayCan", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocs.Bind(["artifact", "SprayCan", "description"]).Localize
        });
        ModEntry.Instance.KokoroApi.StatusLogic.RegisterHook(new LogicHook());
    }
    public sealed class LogicHook : IKokoroApi.IV2.IStatusLogicApi.IHook
    {
        public int ModifyStatusChange(IModifyStatusChangeArgs args)
        {
            if (args.Status == Status.heat)
            {
                if (args.State.EnumerateAllArtifacts().FirstOrDefault(a => a is SprayCan) is { } Artifact)
                {
                    if (args.Ship == args.Combat.otherShip) return args.NewAmount + 1;
                }
            }
            return args.NewAmount;
        }
    }
}
