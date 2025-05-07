using HarmonyLib;
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

internal sealed class CheapBeer : Artifact
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
            Sprite = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/combat/CheapBeer.png")).Sprite,
            Name = ModEntry.Instance.AnyLocs.Bind(["artifact", "CheapBeer", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocs.Bind(["artifact", "CheapBeer", "description"]).Localize
        });
        ModEntry.Instance.Harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.DestroyDroneAt)),
                postfix: new HarmonyMethod(AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(Combat_DestroyDroneAt_Postfix)))
                );
    }
    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            .. StatusMeta.GetTooltips(IntimidationManager.IntimidationStatus.Status, 1)
            ];
    }
    private static void Combat_DestroyDroneAt_Postfix(State s, int x, bool playerDidIt)
    {
        if (s.route is Combat c)
        {
            if (s.EnumerateAllArtifacts().FirstOrDefault(a => a is CheapBeer) is { } artifact)
            {
                c.Queue(new AEnergy()
                {
                    changeAmount = 2
                });
            }
        }
    }
    public override void OnTurnStart(State state, Combat combat)
    {
        if (state.ship.Get(IntimidationManager.IntimidationStatus.Status) < 4)
        {
            combat.Queue(new AStatus()
            {
                status = IntimidationManager.IntimidationStatus.Status,
                targetPlayer = true,
                statusAmount = 1
            });
        }
    }
}
