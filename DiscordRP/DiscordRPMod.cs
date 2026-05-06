using System;
using UnityEngine;
using ToolbarControl_NS;
using KSP.UI.Screens;
using DiscordRP.Discord;
using DiscordRP.States;
using System.Diagnostics;

namespace DiscordRP
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    class DiscordRPMod : MonoBehaviour
    {
        private PresenceController presenceController;
        private StateConfig[] stateConfigs = new StateConfig[7];
        private StateTracker stateTracker;

        private PresenceState state;

        private float lastUpdate = 0.0F;
        private float updateInterval = 15.0F;

        private bool initialized;

        ToolbarControl toolbarControl;
        private Rect windowRect = new Rect(200, 200, 500, 500);
        private bool showWindow = false;

        private string[] stateVariableHints = {
            "cost\ncraftName\npartCount",
            "craftName\nbody\naltitude\nvelocity",
            "craftName\nbody\naltitude\nvelocity",
            "",
            "craftName\nbody",
            "craftName\nbody",
            "craftName\nbody\naltitude\nvelocity\nAP\nPE\nEc"
        };

        private string[] defaultStateDetails = {
            "Building [craftName]",
            "At escape velocity",
            "Flying over [body] | Alt: [altitude] | Vel: [velocity]",
            "",
            "Landed [craftName] on [body]",
            "Splashed down [craftName] at [body]",
            "Orbiting around [body] in [craftName]"
        };

        private string[] defaultStateState = {
            "[partCount] parts",
            "Escaping [body]",
            "Flying over [body]",
            "Idle",
            "Landed at [body]",
            "Splashed down at [body]",
            "AP [AP] | PE [PE] | Ec [Ec]"
        };

        void Awake()
        {
            for (int i = 0; i < stateConfigs.Length; i++)
            {
                stateConfigs[i] = new StateConfig();
            }

            lastUpdate = Time.time;
            state = new IdlingState(Utils.GetEpochTime(), GameScenes.LOADING, stateConfigs[3]);
        }

        void Start()
        {
            presenceController = new PresenceController();
            presenceController.Initialize();

            LoadSettings();
            stateTracker = new StateTracker(stateConfigs);

            UnityEngine.Debug.Log("DiscordRP: Plugin startup");

            DontDestroyOnLoad(this);

            ToolbarControl.RegisterMod("DiscordRP", "DiscordRPContinued");
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(
                OnToolbarToggle,
                OnToolbarToggle,
                ApplicationLauncher.AppScenes.ALWAYS,
                "DiscordRP",
                "DiscordRPButton",
                "DiscordRPContinued/Textures/DiscordRP.png",
                "DiscordRPContinued/Textures/DiscordRP.png",
                "DiscordRPContinued"
            );

            GameEvents.onGamePause.Add(() =>
            {
                stateTracker.Paused = true;

                UpdatePresence(stateTracker.UpdateState());
            });

            GameEvents.onGameUnpause.Add(() =>
            {
                stateTracker.Paused = false;

                UpdatePresence(stateTracker.UpdateState());
            });
        }

        void OnDisable()
        {
            UnityEngine.Debug.Log("DiscordRP: Plugin disable");
            presenceController.Disable();

            toolbarControl.OnDestroy();

            initialized = false;
        }

        void Update()
        {
            presenceController.UpdateCallbacks();

            stateTracker.UpdateTimers();

            float currentTime = Time.time;

            if (currentTime - lastUpdate > updateInterval || !initialized)
            {
                lastUpdate = currentTime;

                UpdatePresence(stateTracker.UpdateState());

                initialized = true;
            }
        }

        void SaveSettings()
        {
            ConfigNode root = new ConfigNode("DiscordRPSettings");
            root.AddValue("updateInterval", updateInterval);

            for (int i = 0; i < statePages.Length; i++)
            {
                ConfigNode node = root.AddNode(statePages[i]);
                node.AddValue("details", stateConfigs[i].details);
                node.AddValue("state", stateConfigs[i].state);
            }

            root.Save(KSPUtil.ApplicationRootPath + "GameData/DiscordRPContinued/settings.cfg");
        }

        void LoadSettings()
        {
            string path = KSPUtil.ApplicationRootPath + "GameData/DiscordRPContinued/settings.cfg";

            if (!System.IO.File.Exists(path))
            {
                for (int i = 0; i < stateConfigs.Length; i++)
                {
                    stateConfigs[i].details = defaultStateDetails[i];
                    stateConfigs[i].state = defaultStateState[i];
                }

                updateIntervalInput = updateInterval.ToString();
                return;
            }

            ConfigNode root = ConfigNode.Load(path);

            updateInterval = float.Parse(root.GetValue("updateInterval"));
            updateIntervalInput = updateInterval.ToString();

            for (int i = 0; i < statePages.Length; i++)
            {
                ConfigNode node = root.GetNode(statePages[i]);
                if (node != null)
                {
                    stateConfigs[i].details = node.GetValue("details");
                    stateConfigs[i].state = node.GetValue("state");
                }
                else
                {
                    stateConfigs[i].details = defaultStateDetails[i];
                    stateConfigs[i].state = defaultStateState[i];
                }
            }
        }

        private void UpdatePresence(PresenceState state)
        {
            PresenceState previousState = this.state;

            this.state = state;

            if (!state.Equals(previousState) || !initialized)
            {
                presenceController.UpdatePresence(state);
            }
        }

        private void OnToolbarToggle()
        {
            showWindow = !showWindow;
            if (!showWindow)
                SaveSettings();
        }

        void OnGUI()
        {
            if (showWindow)
                windowRect = GUILayout.Window(1234, windowRect, DrawWindow, "DiscordRP");
        }

        private int selectedPage = 0;
        private string[] pages = { "General settings", "Message settings" };

        void DrawWindow(int id)
        {
            selectedPage = GUILayout.Toolbar(selectedPage, pages);

            switch (selectedPage)
            {
                case 0:
                    DrawGeneralSettingsPage();
                    break;
                case 1:
                    DrawMessageSettingsPage();
                    break;
            }

            GUI.DragWindow();
        }

        private string updateIntervalInput = "15.0";
        void DrawGeneralSettingsPage()
        {
            GUILayout.Label("update interval (default=15.0)");
            updateIntervalInput = GUILayout.TextField(updateIntervalInput, 4);
            if (float.TryParse(updateIntervalInput, out float result))
            {
                if (result != 0.0f)
                    updateInterval = result;
            }
        }

        private int selectedStatePage = 0;
        private string[] statePages = { "Building", "Escaping", "Flying", "Idling", "Landed", "Splashed", "Orbiting" };
        void DrawMessageSettingsPage()
        {
            selectedStatePage = GUILayout.Toolbar(selectedStatePage, statePages);
            DrawStatePage(selectedStatePage);
        }

        void DrawStatePage(int index)
        {
            if (index != 3) // I was highkey too lazy to add varibles and stuff to idle
            {
                GUILayout.Label("Details text to be displayed");
                stateConfigs[index].details = GUILayout.TextField(stateConfigs[index].details);
            }

            GUILayout.Label("State text to be displayed");
            stateConfigs[index].state = GUILayout.TextField(stateConfigs[index].state);

            if (index != 3)
            {
                GUILayout.Label("You can put variables in your text by doing [variable], below is a list of variables");
                GUILayout.Label(stateVariableHints[index]);
            }
        }
    }
}
