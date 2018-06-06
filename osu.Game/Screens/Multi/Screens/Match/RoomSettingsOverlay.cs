﻿// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.Multiplayer;
using OpenTK;
using OpenTK.Graphics;

namespace osu.Game.Screens.Multi.Screens.Match
{
    public class RoomSettingsOverlay : OverlayContainer
    {
        private const float transition_duration = 500;

        private readonly Container content;
        private readonly SettingsTextBox name, maxParticipants;
        private readonly RoomAvailabilityPicker availability;
        private readonly GameTypePicker type;

        private Room room;
        public Room Room
        {
            get => room;
            set
            {
                if (value == room) return;
                room = value;

                name.Text = room.Name.Value;
                maxParticipants.Text = room.MaxParticipants.Value?.ToString();
                availability.Current.Value = room.Availability.Value;
                type.Current.Value = room.Type.Value;
            }
        }

        public RoomSettingsOverlay()
        {
            Masking = true;

            Child = content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = Axes.Y,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = OsuColour.FromHex(@"28242d"),
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Top = 35, Bottom = 75, Horizontal = 50 },
                        Children = new[]
                        {
                            new SectionContainer
                            {
                                Children = new[]
                                {
                                    new Section("ROOM NAME")
                                    {
                                        Child = name = new SettingsTextBox(),
                                    },
                                    new Section("ROOM VISIBILITY")
                                    {
                                        Child = availability = new RoomAvailabilityPicker(),
                                    },
                                    new Section("GAME TYPE")
                                    {
                                        Child = type = new GameTypePicker(),
                                    },
                                },
                            },
                            new SectionContainer
                            {
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.TopRight,
                                Children = new[]
                                {
                                    new Section("MAX PARTICIPANTS")
                                    {
                                        Child = maxParticipants = new SettingsTextBox(),
                                    },
                                    new Section("PASSWORD (OPTIONAL)")
                                    {
                                        Child = new SettingsTextBox("Password"),
                                    },
                                },
                            },
                        },
                    },
                    new ApplyButton
                    {
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        Size = new Vector2(230, 35),
                        Margin = new MarginPadding { Bottom = 20 },
                        Action = () =>
                        {
                            if (room != null)
                            {
                                room.Name.Value = name.Text;
                                room.Availability.Value = availability.Current.Value;
                                room.Type.Value = type.Current.Value;

                                int max;
                                if (int.TryParse(maxParticipants.Text, out max))
                                    room.MaxParticipants.Value = max;
                                else
                                    room.MaxParticipants.Value = null;
                            }

                            Hide();
                        },
                    },
                },
            };
        }

        protected override void PopIn()
        {
            content.MoveToY(0, transition_duration, Easing.OutQuint);
        }

        protected override void PopOut()
        {
            content.MoveToY(-1, transition_duration, Easing.InSine);
        }

        private class SettingsTextBox : OsuTextBox
        {
            private readonly Container labelContainer;

            protected override Color4 BackgroundUnfocused => Color4.Black;
            protected override Color4 BackgroundFocused => Color4.Black;

            protected override Drawable GetDrawableCharacter(char c) => new OsuSpriteText
            {
                Text = c.ToString(),
                TextSize = 18,
            };

            public SettingsTextBox(string label = null)
            {
                RelativeSizeAxes = Axes.X;

                if (label != null)
                {
                    // todo: overflow broken
                    Add(labelContainer = new Container
                    {
                        AutoSizeAxes = Axes.X,
                        RelativeSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = OsuColour.FromHex(@"3d3943"),
                            },
                            new OsuSpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Font = @"Exo2.0-Bold",
                                Text = label,
                                Margin = new MarginPadding { Horizontal = 10 },
                            },
                        },
                    });
                }
            }

            protected override void UpdateAfterChildren()
            {
                base.UpdateAfterChildren();

                if (labelContainer != null)
                    TextContainer.Padding = new MarginPadding { Horizontal = labelContainer.DrawWidth };
            }
        }

        private class SectionContainer : FillFlowContainer<Section>
        {
            public SectionContainer()
            {
                RelativeSizeAxes = Axes.Both;
                Width = 0.45f;
                Direction = FillDirection.Vertical;
                Spacing = new Vector2(45);
            }
        }

        private class Section : Container
        {
            private readonly Container content;

            protected override Container<Drawable> Content => content;

            public Section(string title)
            {
                AutoSizeAxes = Axes.Y;
                RelativeSizeAxes = Axes.X;

                InternalChild = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(5),
                    Children = new Drawable[]
                    {
                        new OsuSpriteText
                        {
                            TextSize = 12,
                            Font = @"Exo2.0-Bold",
                            Text = title.ToUpper(),
                        },
                        content = new Container
                        {
                            AutoSizeAxes = Axes.Y,
                            RelativeSizeAxes = Axes.X,
                        },
                    },
                };
            }
        }

        private class ApplyButton : TriangleButton
        {
            public ApplyButton()
            {
                Text = "Apply";
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                BackgroundColour = colours.Yellow;
                Triangles.ColourLight = colours.YellowLight;
                Triangles.ColourDark = colours.YellowDark;
            }
        }
    }
}
