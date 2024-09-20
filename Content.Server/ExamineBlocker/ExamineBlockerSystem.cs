using Content.Server.Administration.Logs;
using Content.Server.Chat.Systems;
using Content.Server.Popups;
using Content.Shared.Clothing;
using Content.Shared.Database;
using Content.Shared.Popups;
using Content.Shared.Preferences;
using Content.Shared.Speech;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;


namespace Content.Server.ExamineBlocker;

public sealed partial class ExamineBlockerSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<ExamineBlockerComponent, ClothingGotEquippedEvent>(OnEquip);
        SubscribeLocalEvent<ExamineBlockerComponent, ClothingGotUnequippedEvent>(OnUnequip);
    }
}
