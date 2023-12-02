using System.Numerics;
using Content.Shared.Ghost.Roles;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Ghost.Controls.Roles
{
    [GenerateTypedNameReferences]
    public sealed partial class GhostRolesEntry : BoxContainer
    {
        private SpriteSystem _spriteSystem;
        public event Action<GhostRoleInfo>? OnRoleSelected;
        public event Action<GhostRoleInfo>? OnRoleFollow;
        public event Action<GhostRoleInfo>? OnRoleVariantSelected;

        public GhostRolesEntry(string name, string description, bool hasAccess, FormattedMessage? reason, IEnumerable<GhostRoleInfo> roles, SpriteSystem spriteSystem)
        {
            RobustXamlLoader.Load(this);
            _spriteSystem = spriteSystem;

            Title.Text = name;
            Description.SetMessage(description);

            foreach (var role in roles)
            {
                var button = new GhostRoleEntryButtons();
                button.RequestButton.OnPressed += _ => OnRoleSelected?.Invoke(role);
                button.FollowButton.OnPressed += _ => OnRoleFollow?.Invoke(role);
                button.VariantButton.OnPressed += _ => OnRoleVariantSelected?.Invoke(role);

                if (!hasAccess)
                {
                    button.RequestButton.Disabled = true;

                    if (reason != null && !reason.IsEmpty)
                    {
                        var tooltip = new Tooltip();
                        tooltip.SetMessage(reason);
                        button.RequestButton.TooltipSupplier = _ => tooltip;
                    }

                    button.RequestButton.AddChild(new TextureRect
                    {
                        TextureScale = new Vector2(0.4f, 0.4f),
                        Stretch = TextureRect.StretchMode.KeepCentered,
                        Texture = _spriteSystem.Frame0(new SpriteSpecifier.Texture(new ("/Textures/Interface/Nano/lock.svg.192dpi.png"))),
                        HorizontalExpand = true,
                        HorizontalAlignment = HAlignment.Right,
                    });
                }

                Buttons.AddChild(button);
            }
        }
    }
}
