using System;
using UnityEngine;
using DiscordRP.States;
using System.Security.Cryptography;

namespace DiscordRP
{
    class StateConfig
    {
        public string details = "temp";
        public string state = "temp";
    }
    class StateTracker
    {
        private readonly GameStateTimer launchStateTimer;
        private readonly GameStateTimer landedStateTimer;

        private readonly GameStateTimer buildingStateTimer;

        private readonly GameStateTimer idleStateTimer;
        private readonly StateConfig[] stateConfigs;

        public bool Paused { private get; set; }

        public StateTracker(StateConfig[] stateConfigs)
        {
            this.stateConfigs = stateConfigs;

            this.launchStateTimer = new GameStateTimer(scene => HighLogic.LoadedSceneIsGame && FlightGlobals.ActiveVessel != null, scene => !FlightGlobals.ActiveVessel.Landed && !FlightGlobals.ActiveVessel.Splashed);
            this.landedStateTimer = GameStateTimer.Inverse(launchStateTimer);

            this.buildingStateTimer = new GameStateTimer(scene => true, scene => scene == GameScenes.EDITOR);

            this.idleStateTimer = new GameStateTimer(scene => true, scene => scene != GameScenes.EDITOR && scene != GameScenes.FLIGHT);

            UpdateTimers();
        }

        public void UpdateTimers()
        {
            launchStateTimer.Update();
            landedStateTimer.Update();

            buildingStateTimer.Update();

            idleStateTimer.Update();
        }

        public PresenceState UpdateState()
        {
            if (HighLogic.LoadedSceneIsGame)
            {
                Vessel activeVessel = FlightGlobals.ActiveVessel;
                Part rootEditorPart = EditorLogic.RootPart;

                if (activeVessel != null)
                {
                    return GetFlightState(activeVessel);
                }
                else if (rootEditorPart != null)
                {
                    return new BuildingState(buildingStateTimer.Timestamp, stateConfigs[0], rootEditorPart);
                }
            }

            return new IdlingState(idleStateTimer.Timestamp, HighLogic.LoadedScene, stateConfigs[3]);
        }

        private PresenceState GetFlightState(Vessel activeVessel)
        {
            double periapsis = activeVessel.orbit.GetPeriapsis();
            double apoapsis = activeVessel.orbit.GetApoapsis();

            if (activeVessel.Landed)
            {
                return new LandedState(activeVessel.mainBody, landedStateTimer.Timestamp, Paused, stateConfigs[4]);
            }
            else if (activeVessel.Splashed)
            {
                return new SplashedState(activeVessel.mainBody, landedStateTimer.Timestamp, Paused, stateConfigs[5]);
            }
            else if (apoapsis > activeVessel.mainBody.sphereOfInfluence || (apoapsis < 0.0 && periapsis > apoapsis))
            {
                return new EscapingState(activeVessel.mainBody, launchStateTimer.Timestamp, Paused, stateConfigs[1]);
            }
            else if (activeVessel.mainBody.atmosphereDepth > activeVessel.altitude || periapsis < activeVessel.mainBody.Radius)
            {
                return new FlyingState(activeVessel.mainBody, launchStateTimer.Timestamp, Paused, stateConfigs[2]);
            }
            else
            {
                return new OrbitingState(activeVessel.mainBody, launchStateTimer.Timestamp, Paused, stateConfigs[6]);
            }
        }
    }
}
