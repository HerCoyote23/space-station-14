using Robust.Shared.GameStates;

namespace Content.Shared.ExamineBlocker;

/// <summary>
/// Attached to entities who shouldn't be examinable
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class UnExaminableComponent : Component { }
