using System;
using UnityEngine;
using DiscordRP.StateTextFormat;

namespace DiscordRP.States
{
    class SplashedState : PresenceState
    {
        private readonly CelestialBody body;
        private readonly long startTimestamp;
        private readonly bool paused;
        private string details;
        private string state;

        public SplashedState(CelestialBody body, long startTimestamp, bool paused, StateConfig stateConfig)
        {
            this.body = body;
            this.startTimestamp = startTimestamp;
            this.paused = paused;
            this.details = stateConfig.details;
            this.state = stateConfig.state;
        }

        public DiscordRpc.RichPresence create()
        {
            return new DiscordRpc.RichPresence()
            {
                state = TextParser.ParseVariables(state, null, body),
                details = TextParser.ParseVariables(details, null, body),
                largeImageKey = string.Format("body_{0}", body.name.ToLower()),
                largeImageText = body.name,
                startTimestamp = startTimestamp,
                smallImageKey = Utils.GetSmallFlightIcon(paused),
                smallImageText = Utils.GetSmallFlightIconDetails(paused),
            };
        }
    }
}
