using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using Nickel.Common;
using Sierra.artifacts;
using Sierra.cards.common;
using Sierra.cards.rare;
using Sierra.cards.special;
using Sierra.cards.uncommon;
using Sierra.ExternalAPIs.Kokoro;
using Sierra.features;
using System.Reflection;
using Sierra.ExternalAPIs;
using static Sierra.features.PickDestroyMidrow;

namespace Sierra;


public sealed class ModEntry: SimpleMod
{
    internal static ModEntry Instance { get; private set; } = null!;
    public string Name { get; init; } = typeof(ModEntry).Namespace!;
    internal readonly IKokoroApi.IV2 KokoroApi;
    internal readonly IHarmony Harmony;
    internal readonly ILocalizationProvider<IReadOnlyList<string>> AnyLocs;
    internal readonly ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Locs;

    internal readonly ApiImplementation Api;

    internal readonly IDeckEntry SierraDeck;

    internal static readonly IReadOnlyList<Type> CommonCardTypes = [
            typeof(BarrelRollCard),
            typeof(MischiefCard),
            typeof(BobAndWeaveCard),
            typeof(BullyCard),
            typeof(BurnBabyBurnCard),
            typeof(DoubleBarrelCard),
            typeof(DeepFryCard),
            typeof(MayhemCard),
            typeof(BoozeCruiseCard),
            typeof(BonkCard),
            typeof(PumpTheGasCard)
        ];
    internal static readonly IReadOnlyList<Type> UncommonCardTypes = [
            typeof(BlisterBlasterCard),
            typeof(HammerlockCard),
            typeof(FuseCard),
            typeof(HuckinJunkCard),
            typeof(PartingGiftCard),
            typeof(DrunkenFuryCard),
            typeof(SadismCard)
        ];
    internal static readonly IReadOnlyList<Type> RareCardTypes = [
            typeof(DominateCard),
            typeof(AddictionCard),
            typeof(CantripCard),
            typeof(HotCommodityCard),
            typeof(FOMOCard)
        ];
    internal static readonly IEnumerable<Type> AllCardTypes = 
        [
            .. CommonCardTypes,
            .. UncommonCardTypes,
            .. RareCardTypes,
        ];

    internal static readonly IReadOnlyList<Type> CommonArtifacts = [
            typeof(SlimeAndPunishment),
            typeof(ThickGel),
            typeof(MrBreaky),
            typeof(SpaceJunk)
        ];
    internal static readonly IReadOnlyList<Type> BossArtifacts = [
            typeof(CheapBeer),
            typeof(PassiveAggression),
            typeof(SprayCan)
        ];
    internal static readonly IEnumerable<Type> AllArtifactTypes = [
            .. CommonArtifacts,
            .. BossArtifacts
        ];

    internal static readonly IEnumerable<Type> RegisterableTypes =
        [
            ..AllCardTypes,
            ..AllArtifactTypes,
            typeof(OilManager),
            typeof(IntimidationManager),
            typeof(TurnEndTriggerTraitManager)
        ];
    internal ISpriteEntry oilDrumSprite { get; }
    internal ISpriteEntry oilDrumIcon { get; }
    internal ISpriteEntry EndTrigger1 { get; }
    internal ISpriteEntry EndTrigger2 { get; }
    internal ISpriteEntry EndTrigger3 { get; }
    internal ISpriteEntry PumpTheGas { get; }
    internal ISpriteEntry PumpTheGasFlipped { get; }
    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
    {
        Instance = this;
        Harmony = helper.Utilities.Harmony;
        KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!.V2!;
        Api = new ApiImplementation();

        AnyLocs = new JsonLocalizationProvider(
                tokenExtractor: new SimpleLocalizationTokenExtractor(),
                localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.json").OpenRead()
                );
        Locs = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
                new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocs)
            );

        oilDrumSprite = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/combat/oildrum2.png"));
        oilDrumIcon = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/combat/oildrumicon.png"));
        EndTrigger1 = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/EndTrigger1.png"));
        EndTrigger2 = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/EndTrigger2.png"));
        EndTrigger3 = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/EndTrigger3.png"));
        PumpTheGas = Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/PumpTheGas.png"));
        PumpTheGasFlipped = Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/card/PumpTheGasFlip.png"));

        SierraDeck = helper.Content.Decks.RegisterDeck("Sierra", new()
        {
            Definition = new () { color = new("ffc860"), titleColor = Colors.black },
            BorderSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/char/sierra_card_panel.png")).Sprite,
            Name = AnyLocs.Bind(["character", "name"]).Localize,
            DefaultCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/char/sierra_card_background.png")).Sprite
        });

        foreach (var type in RegisterableTypes)
        {
            AccessTools.DeclaredMethod(type, nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
        }

        helper.Content.Characters.V2.RegisterPlayableCharacter("Sierra", new()
        {
            Deck = SierraDeck.Deck,
            Description = AnyLocs.Bind(["character", "description"]).Localize,
            BorderSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/char/sierra_character_panel.png")).Sprite,
            NeutralAnimation = new()
            {
                CharacterType = SierraDeck.UniqueName,
                LoopTag = "neutral",
                Frames = [
                    helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/char/sierra_character_neutral_0.png")).Sprite
                    ]
            },
            MiniAnimation = new()
            {
                CharacterType = SierraDeck.UniqueName,
                LoopTag = "mini",
                Frames = [helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/char/sierra_character_mini.png")).Sprite]
            },
            Starters = new()
            {
                cards = [
                        new BarrelRollCard(),
                        new MischiefCard()
                    ]
            }
        });

        helper.Content.Characters.V2.RegisterCharacterAnimation(new()
        {
            CharacterType = SierraDeck.UniqueName,
            LoopTag = "gameover",
            Frames = [
                helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/char/sierra_character_neutral_0.png")).Sprite
                ]
        });

        helper.ModRegistry.AwaitApi<IMoreDifficultiesApi>(
            "TheJazMaster.MoreDifficulties",
            new SemanticVersion(1, 3, 0),
            api => api.RegisterAltStarters(
                deck: SierraDeck.Deck,
                starterDeck: new StarterDeck
                    {
                        cards = [
                            new MayhemCard(),
                            new DeepFryCard()
                        ]
                    }
                )
            );

        Instance.Harmony!.Patch(
                original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.IsVisible)),
                postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Combat_IsVisible_Postfix))
                );
    }
    public override object? GetApi(IModManifest reguestingMod) => new ApiImplementation();
    private static void Combat_IsVisible_Postfix(Combat __instance, ref bool __result)
    {
        if (__instance.routeOverride is ActionRoute)
            __result = true;
    }
}