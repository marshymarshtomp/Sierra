using Nanoray.PluginManager;
using Nickel;
using Sierra.features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sierra.artifacts;

internal sealed class ThickGel : Artifact
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
                pools = [ArtifactPool.Common],
                unremovable = false
            },
            Sprite = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/combat/ThickGel.png")).Sprite,
            Name = ModEntry.Instance.AnyLocs.Bind(["artifact", "ThickGel", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocs.Bind(["artifact", "ThickGel", "description"]).Localize
        });
    }
    public override void OnTurnStart(State state, Combat combat)
    {
        if (state.ship.Get(OilManager.OilStatus.Status) >= 2)
        {
            combat.Queue(new AStatus()
            {
                status = Status.heat,
                statusAmount = -1,
                targetPlayer = true
            });
        }
    }
}
