using System;
using UnityEngine;
using DiscordRP.States;

// Reference: https://github.com/discordapp/discord-rpc
namespace DiscordRP.Discord
{
    class PresenceController
    {
        private DiscordRpc.EventHandlers handlers;
        private DiscordRpc.ReadyCallback readyCallback;
        private DiscordRpc.DisconnectedCallback disconnectedCallback;
        private DiscordRpc.ErrorCallback errorCallback;
        private DiscordRpc.JoinCallback joinCallback;
        private DiscordRpc.SpectateCallback spectateCallback;
        private DiscordRpc.RequestCallback requestCallback;

        public bool initialized;
        private const string applicationId = "386261941259337738";

        public bool Initialize()
        {
            try
            {
                readyCallback = ReadyCallback;
                disconnectedCallback = DisconnectedCallback;
                errorCallback = ErrorCallback;
                joinCallback = JoinCallback;
                spectateCallback = SpectateCallback;
                requestCallback = RequestCallback;

                handlers = new DiscordRpc.EventHandlers();

                handlers.readyCallback = readyCallback;
                handlers.disconnectedCallback = disconnectedCallback;
                handlers.errorCallback = errorCallback;
                handlers.joinCallback = joinCallback;
                handlers.spectateCallback = spectateCallback;
                handlers.requestCallback = requestCallback;

                DiscordRpc.Initialize(applicationId, ref handlers, true, null);

                initialized = true;

                Debug.Log("DiscordRP: Discord Initialize");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"DiscordRP initialize fail: {ex}");

                initialized = false;
                return false;
            }
        }

        public void Disable()
        {
            DiscordRpc.Shutdown();
            Debug.Log("DiscordRP: Discord Shutdown");
            initialized = false;
        }

        public void UpdatePresence(PresenceState state)
        {
            DiscordRpc.RichPresence presence = state.create();

            Debug.Log(string.Format("DiscordRP: Send presence: {0} ({1})", presence, state));

            DiscordRpc.UpdatePresence(ref presence);
        }

        public void UpdateCallbacks()
        {
            DiscordRpc.RunCallbacks();
        }

        public void ReadyCallback()
        {
            Debug.Log("DiscordRP: Discord Ready");
        }

        public void DisconnectedCallback(int errorCode, string message)
        {
            Debug.Log(string.Format("DiscordRP: Discord Disconnect {0}: {1}", errorCode, message));
        }

        public void ErrorCallback(int errorCode, string message)
        {
            Debug.Log(string.Format("DiscordRP: Discord Error: {0}: {1}", errorCode, message));
        }

        public void JoinCallback(string secret)
        {
        }

        public void SpectateCallback(string secret)
        {
        }

        public void RequestCallback(DiscordRpc.JoinRequest request)
        {
            DiscordRpc.Respond(request.userId, DiscordRpc.Reply.Yes);
        }
    }
}
