using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoMod.Utils;

namespace Celeste.Mod.ChallengeTimer
{
    public class ChallengeTimerModule : EverestModule
    {

        // Only one alive module instance can exist at any given time.
        public static ChallengeTimerModule Instance;

        public ChallengeTimerModule()
        {
            Instance = this;
        }

        // Check the next section for more information about mod settings, save data and session.
        // Those are optional: if you don't need one of those, you can remove it from the module.

        // If you need to store settings:
        public override Type SettingsType => typeof(ChallengeTimerSettings);
        public static ChallengeTimerSettings Settings => (ChallengeTimerSettings)Instance._Settings;

        // If you need to store save data:
        public override Type SaveDataType => null;

        // If you need to store session data:
        public override Type SessionType => typeof(ChallengeTimerSession);
        public static ChallengeTimerSession Session => (ChallengeTimerSession)Instance._Session;

        // Set up any hooks, event handlers and your mod in general here.
        // Load runs before Celeste itself has initialized properly.
        public override void Load()
        {
            On.Celeste.Player.Update += Player_Update;
            On.Celeste.Glider.Added += Glider_Added;
            On.Celeste.Glider.OnPickup += Glider_OnPickup;
            On.Celeste.SpeedrunTimerDisplay.Update += SpeedrunTimerDisplay_Update;
        }

        void Glider_Added(On.Celeste.Glider.orig_Added orig, Glider self, Monocle.Scene scene)
        {
            if (Settings.Enabled)
            {
                DynData<Glider> gliderData = new DynData<Glider>(self);
                gliderData["fresh"] = true;
            }
            orig(self, scene);
        }

        public void Glider_OnPickup(On.Celeste.Glider.orig_OnPickup orig, Glider self)
        {
            if (Settings.Enabled)
            {
                if (Settings.Week == 2 && (self.Scene as Level).Session.Area.SID == "Celeste/LostLevels")
                {
                    DynData<Glider> gliderData = new DynData<Glider>(self);
                    if (gliderData.Get<bool>("fresh") != false)
                    {
                        Session.Jellies.Add(self.Position);
                        gliderData["fresh"] = false;
                    }
                }
            }
            orig(self);
        }


        public void Player_Update(On.Celeste.Player.orig_Update orig, Player self)
        {
            orig(self);
            if (Settings.Enabled)
            {
                if (Settings.Week == 2 && (self.Scene as Level).Session.Area.SID == "Celeste/LostLevels")
                {
                    Level level = self.Scene as Level;
                    float x = self.CenterX;
                    float y = self.Bottom;
                    if (Session.Jellies.Count >= 6 && self.OnGround() && 12906 < x && x < 12959 && -2974 < y && y < -2966)
                    {
                        ChallengeComplete(level);
                    }
                }
            }
        }

        public void SpeedrunTimerDisplay_Update(On.Celeste.SpeedrunTimerDisplay.orig_Update orig, SpeedrunTimerDisplay self)
        {
            orig(self);
            if (Session.NoLerp)
                self.DrawLerp = 1;
        }


        public void ChallengeComplete(Level level)
        {
            level.Completed = true;
            Session.NoLerp = true;
        }

        // Optional, initialize anything after Celeste has initialized itself properly.
        public override void Initialize()
        {
        }

        // Optional, do anything requiring either the Celeste or mod content here.
        public override void LoadContent(bool firstLoad)
        {
        }

        // Unload the entirety of your mod's content. Free up any native resources.
        public override void Unload()
        {
            On.Celeste.Player.Update -= Player_Update;
            On.Celeste.Glider.Added -= Glider_Added;
            On.Celeste.Glider.OnPickup -= Glider_OnPickup;
            On.Celeste.SpeedrunTimerDisplay.Update -= SpeedrunTimerDisplay_Update;
        }

    }
}
