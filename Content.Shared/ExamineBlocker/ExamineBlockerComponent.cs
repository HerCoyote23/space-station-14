using Robust.Shared.GameStates;

namespace Content.Shared.ExamineBlocker;

/// <summary>
/// Attached to entities who shouldn't be examinable
/// </summary>
[RegisterComponent, NetworkedComponent/*, AutoGenerateComponentState*/]
public sealed partial class ExamineBlockerComponent : Component
{

}
