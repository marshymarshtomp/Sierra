using HarmonyLib;
using Sierra.ExternalAPIs.Kokoro;
using Nickel;
using Nanoray.PluginManager;
using System.Reflection;
using static Sierra.ExternalAPIs.Kokoro.IKokoroApi.IV2.IStatusLogicApi.IHook;

namespace Sierra.features;

internal sealed class OilManager : IRegisterable
{
    internal static IStatusEntry OilStatus { get; private set; } = null!;
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        OilStatus = ModEntry.Instance.Helper.Content.Statuses.RegisterStatus("Oil", new()
        {
            Definition = new()
            {
                icon = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/combat/oil.png")).Sprite,
                color = new("eccc4c"),
                isGood = false
            },
            Name = ModEntry.Instance.AnyLocs.Bind(["status", "Oil", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocs.Bind(["status", "Oil", "description"]).Localize
        });
        ModEntry.Instance.KokoroApi.StatusLogic.RegisterHook(new Hook());
    }
    private sealed class Hook : IKokoroApi.IV2.IStatusLogicApi.IHook
    {
        public int ModifyStatusChange(IModifyStatusChangeArgs args)
        {
            var isPlayerShip = args.Ship.isPlayerShip;
            if (args.Status == Status.heat)
            {
                if (args.OldAmount >= args.NewAmount || args.NewAmount <= 0) return args.NewAmount;
                var oil = args.Ship.Get(OilStatus.Status);
                if (oil > 0)
                {
                    args.Combat.QueueImmediate(new AHurt()
                    {
                        hurtAmount = args.NewAmount - args.OldAmount,
                        targetPlayer = isPlayerShip,
                        hurtShieldsFirst = true,
                        timer = args.NewAmount <= 0 ? 0 : 1.0
                    });
                    args.Ship.Set(OilStatus.Status, oil - 1);
                    return args.OldAmount;
                }
                else return args.NewAmount;
            } else return args.NewAmount;
        }
    }
}
