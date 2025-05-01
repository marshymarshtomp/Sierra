using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Sierra.features;

internal sealed class TurnEndTriggerTraitManager : IRegisterable
{
    internal static ICardTraitEntry TurnEndTriggerTrait { get; private set; } = null!;
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        var icon = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/combat/turnendtriggertrait.png"));
        TurnEndTriggerTrait = ModEntry.Instance.Helper.Content.Cards.RegisterTrait("TurnEndTrigger", new()
        {
            Icon = (_,_) => icon.Sprite,
            Name = ModEntry.Instance.AnyLocs.Bind(["trait", "TurnEndTrigger", "name"]).Localize,
            Tooltips = (_,_) =>
                [
                    new GlossaryTooltip($"trait.{ModEntry.Instance.Package.Manifest.UniqueName}::TurnEndTrigger")
                    {
                        Icon = icon.Sprite,
                        TitleColor = Colors.cardtrait,
                        Title = ModEntry.Instance.Locs.Localize(["trait", "TurnEndTrigger", "name"]),
                        Description = ModEntry.Instance.Locs.Localize(["trait", "TurnEndTrigger", "description"])
                    }
                ]
        });

    }


}

