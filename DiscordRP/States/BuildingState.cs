using System;
using UnityEngine;
using DiscordRP.StateTextFormat;

namespace DiscordRP.States
{
    class BuildingState : PresenceState
    {
        private string details;
        private string state;
        private readonly Part part;
        private readonly long startTimestamp;

        public BuildingState(long timestamp, StateConfig config, Part part)
        {
            this.startTimestamp = timestamp;
            this.details = config.details;
            this.state = config.state;
            this.part = part;
        }

        public DiscordRpc.RichPresence create()
        {
            return new DiscordRpc.RichPresence()
            {
                state = TextParser.ParseVariables(state, part),
                details = TextParser.ParseVariables(details, part),
                largeImageKey = "building_craft",
                largeImageText = "Building a craft",
                startTimestamp = startTimestamp,
                smallImageKey = "default",
                smallImageText = "Kerbal Space Program",
            };
        }
    };
}
