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
    public bool hasTriggered = false;
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
            original: AccessTools.DeclaredMethod(typeof(AStatus), nameof(AStatus.Begin)),
            postfix: new HarmonyMethod(AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(AStatus_Begin_Postfix)))
        );
    }
    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            .. StatusMeta.GetTooltips(OilManager.OilStatus.Status, 1)
            ];
    }
    private static void AStatus_Begin_Postfix(State s, Combat c, AStatus __instance)
    {
        
        if ((SlimeAndPunishment)s.EnumerateAllArtifacts().FirstOrDefault(a => a is SlimeAndPunishment) is { } artifact)
        {
            if (__instance.status == OilManager.OilStatus.Status && !__instance.targetPlayer && !artifact.hasTriggered)
            {
                c.QueueImmediate(new AHurt()
                {
                    targetPlayer = false,
                    hurtAmount = 1,
                    hurtShieldsFirst = false
                });
                artifact.Pulse();
                artifact.hasTriggered = true;
            }
        }
    }
    public override void OnCombatStart(State state, Combat combat)
    {
        hasTriggered = false;
    }
    public override void OnTurnStart(State state, Combat combat)
    {
        hasTriggered = false;
    }
}
