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

internal sealed class SlimeAndPunishment : Artifact
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
            Sprite = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/combat/SlimeAndPunishment.png")).Sprite,
            Name = ModEntry.Instance.AnyLocs.Bind(["artifact", "SlimeAndPunishment", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocs.Bind(["artifact", "SlimeAndPunishment", "description"]).Localize
        });
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.DestroyDroneAt)),
            postfix: new HarmonyMethod(AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(Combat_DestroyDroneAt_Postfix)))
        );
    }
    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            .. StatusMeta.GetTooltips(OilManager.OilStatus.Status, 1),
            .. StatusMeta.GetTooltips(Status.tempShield, 1)
            ];
    }
    private static void Combat_DestroyDroneAt_Postfix(State s, int x, bool playerDidIt)
    {
        if (s.route is Combat c)
        {
            if (s.EnumerateAllArtifacts().FirstOrDefault(a => a is SlimeAndPunishment) is { } artifact)
            {
                if (playerDidIt)
                {
                    c.Queue(new AStatus()
                    {
                        targetPlayer = true,
                        status = Status.tempShield,
                        statusAmount = 1
                    });
                }
            }
        }
    }
    public override void AfterPlayerStatusAction(State state, Combat combat, Status status, AStatusMode mode, int statusAmount)
    {
        if (status == IntimidationManager.IntimidationStatus.Status)
        {
            combat.Queue(new AStatus()
            {
                targetPlayer = true,
                status = Status.tempShield,
                statusAmount = 1
            });
        }
    }
}
