using System;
using UnityEngine;

namespace DiscordRP.States
{
    class BuildingState : PresenceState
    {
        private readonly int partCount;
        private readonly string craftName;
        private readonly long startTimestamp;

        public BuildingState(int partCount, string craftName, long startTimestamp)
        {
            this.partCount = partCount;
            this.craftName = craftName;
            this.startTimestamp = startTimestamp;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is BuildingState)
            {
                BuildingState buildingState = (BuildingState)obj;

                return buildingState.partCount == partCount && buildingState.craftName == craftName && buildingState.startTimestamp == startTimestamp; ;
            }

            return false;
        }

        public DiscordRpc.RichPresence create()
        {
            return new DiscordRpc.RichPresence()
            {
                state = string.Format("{0} parts", partCount),
                details = string.Format("Building {0}", craftName),
                largeImageKey = "building_craft",
                largeImageText = "Building a craft",
                startTimestamp = startTimestamp,
                smallImageKey = "default",
                smallImageText = "Kerbal Space Program",
            };
        }
    }
}
