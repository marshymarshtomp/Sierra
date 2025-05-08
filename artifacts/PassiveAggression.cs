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

internal sealed class PassiveAggression : Artifact
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
            Sprite = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/combat/PassiveAggression.png")).Sprite,
            Name = ModEntry.Instance.AnyLocs.Bind(["artifact", "PassiveAggression", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocs.Bind(["artifact", "PassiveAggression", "description"]).Localize
        });

    }
    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            .. StatusMeta.GetTooltips(IntimidationManager.IntimidationStatus.Status, 1)
            ];
    }


    public override void OnTurnStart(State state, Combat combat)
    {
        combat.Queue(new AStatus()
        {
            status = IntimidationManager.IntimidationStatus.Status,
            targetPlayer = false,
            statusAmount = 1
        });
    }
    public override int ModifyBaseDamage(int baseDamage, Card? card, State state, Combat? combat, bool fromPlayer)
    {
        if (state.EnumerateAllArtifacts().FirstOrDefault(a => a is PassiveAggression) is { } artifact)
        {
            if (fromPlayer == true) return -1;
        }
        return baseDamage;
    }
}
