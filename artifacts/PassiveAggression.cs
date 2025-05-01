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
        ModEntry.Instance.Harmony.Patch(
        original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetActionsOverridden)),
        postfix: new HarmonyMethod(AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Card_GetActionsOverridden_Postfix)), priority: Priority.Normal)
        );
    }
    private static void Card_GetActionsOverridden_Postfix(State s, ref List<CardAction> __result, Card __instance)
    {
        var card = __instance;
        if (s.EnumerateAllArtifacts().FirstOrDefault(a => a is PassiveAggression) is not { } artifact) return;
        if (s.route is Combat c)
        {
            if (s.deck.Contains(card) || c.discard.Contains(card) || c.exhausted.Contains(card) || c.hand.Contains(card))
            {
                foreach (var actions in __result)
                {
                    foreach (var unwrappedAction in ModEntry.Instance.KokoroApi.WrappedActions.GetWrappedCardActionsRecursively(actions))
                    {
                        if (unwrappedAction is AAttack attack)
                        {
                            attack.damage = Math.Clamp(attack.damage - 1, 0, attack.damage);
                        }
                    }
                }
            }
        }
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
}
