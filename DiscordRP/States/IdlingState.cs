using System;
using System.Linq;

namespace DiscordRP.States
{
    class IdlingState : PresenceState
    {
        private readonly long startTimestamp;
        private readonly GameScenes scene;
        private readonly string details;
        private readonly string state;

        public IdlingState(long startTimestamp, GameScenes scene, StateConfig stateConfig)
        {
            this.startTimestamp = startTimestamp;
            this.scene = scene;
            this.details = stateConfig.details;
            this.state = stateConfig.state;
        }

        public DiscordRpc.RichPresence create()
        {
            string sceneDescription = GetSceneDescription();

            return new DiscordRpc.RichPresence()
            {
                state = state,
                details = sceneDescription,
                largeImageKey = "default",
                largeImageText = "Idling",
                smallImageKey = GetSmallIcon(),
                smallImageText = sceneDescription,
                startTimestamp = startTimestamp,
            };
        }

        private String GetSceneDescription()
        {
            int modCount = AssemblyLoader.loadedAssemblies.Count;
            int activeVessels = 0;
            if (scene == GameScenes.TRACKSTATION)
            {
                activeVessels = FlightGlobals.Vessels.Count(v =>
                    v.vesselType != VesselType.Debris &&
                    v.vesselType != VesselType.Flag &&
                    v.vesselType != VesselType.SpaceObject &&
                    v.vesselType != VesselType.Unknown
                );
            }

            switch (scene)
            {
                case GameScenes.LOADING:
                case GameScenes.LOADINGBUFFER:
                    return string.Format("Loading Game, {0} mods loaded", modCount);
                case GameScenes.MAINMENU:
                    return string.Format("In the Main Menu, {0} mods loaded", modCount);
                case GameScenes.SETTINGS:
                    return "Configuring their game";
                case GameScenes.SPACECENTER:
                    return "At the KSC";
                case GameScenes.TRACKSTATION:
                    return string.Format("In the Tracking Station, {0} vessels", activeVessels);
                case GameScenes.CREDITS:
                    return "Watching the Credits";
            }

            return scene.ToString();
        }

        private String GetSmallIcon()
        {
            switch (scene)
            {
                case GameScenes.LOADING:
                case GameScenes.LOADINGBUFFER:
                    return "loading";
                case GameScenes.SETTINGS:
                    return "settings";
                case GameScenes.SPACECENTER:
                    return "space_center";
                case GameScenes.TRACKSTATION:
                    return "tracking_station";
                case GameScenes.CREDITS:
                    return "heart";
            }

            return "";
        }
    }
}
