using System;
using UnityEngine;

namespace DiscordRP.States
{
    class OrbitingState : PresenceState
    {
        private readonly CelestialBody body;
        private readonly double apoapsis;
        private readonly double periapsis;
        private readonly double eccentricity;
        private readonly string craftName;
        private readonly long startTimestamp;
        private readonly bool paused;

        public OrbitingState(CelestialBody body, double apoapsis, double periapsis, double eccentricity, string craftName, long startTimestamp, bool paused)
        {
            this.body = body;
            this.apoapsis = apoapsis;
            this.periapsis = periapsis;
            this.eccentricity = eccentricity;
            this.craftName = craftName;
            this.startTimestamp = startTimestamp;
            this.paused = paused;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is OrbitingState)
            {
                OrbitingState orbitingState = (OrbitingState)obj;

                return orbitingState.body.Equals(body) && orbitingState.apoapsis == apoapsis && orbitingState.periapsis == periapsis && orbitingState.eccentricity == eccentricity && orbitingState.craftName == craftName && orbitingState.startTimestamp == startTimestamp && orbitingState.paused == paused;
            }

            return false;
        }

        public DiscordRpc.RichPresence create()
        {
            string state = state = string.Format("Orbiting around {0} in {1}", body.name, craftName);

            return new DiscordRpc.RichPresence()
            {
                state = state,
                details = string.Format("AP {0:F0} | PE {1:F0} | Ec: {2:F2}", Utils.FormatDistance(apoapsis - body.Radius), Utils.FormatDistance(periapsis - body.Radius), eccentricity),
                largeImageKey = string.Format("body_{0}", body.name.ToLower()),
                largeImageText = body.name,
                startTimestamp = startTimestamp,
                smallImageKey = Utils.GetSmallFlightIcon(paused),
                smallImageText = Utils.GetSmallFlightIconDetails(paused),
            };
        }
    }
}
