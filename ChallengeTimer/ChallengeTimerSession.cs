using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.ChallengeTimer
{
    public class ChallengeTimerSession : EverestModuleSession
    {
        public List<Vector2> Jellies = new List<Vector2>();

        public bool NoLerp = false;
    }
}