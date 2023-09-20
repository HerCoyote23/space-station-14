using Content.Shared.Spider.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.Spider.Components;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedSpiderSystem))]
public sealed partial class SpiderWebObjectComponent : Component
{
}
