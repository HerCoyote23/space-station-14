using Content.Shared.Clothing;

namespace Content.Server.ExamineBlocker;

public sealed partial class ExamineBlockerSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<ExamineBlockerComponent, ClothingGotEquippedEvent>(OnEquip);
        SubscribeLocalEvent<ExamineBlockerComponent, ClothingGotUnequippedEvent>(OnUnequip);
    }
}
