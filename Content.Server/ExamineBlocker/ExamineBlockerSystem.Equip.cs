using Content.Shared.Clothing;
using Content.Shared.ExamineBlocker;

namespace Content.Server.ExamineBlocker;

public sealed partial class ExamineBlockerSystem
{
    private void OnEquip(EntityUid uid, ExamineBlockerComponent component, ClothingGotEquippedEvent args)
    {
        EnsureComp<UnExaminableComponent>(args.Wearer);
    }
    private void OnUnequip(EntityUid uid, ExamineBlockerComponent compnent, ClothingGotUnequippedEvent args)
    {
        RemComp<UnExaminableComponent>(args.Wearer);
    }
}
