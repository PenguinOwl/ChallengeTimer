namespace Celeste.Mod.ChallengeTimer
{
    public class ChallengeTimerSettings : EverestModuleSettings
    {
        public bool Enabled { get; set; } = false;

        [SettingRange(2, 2)]
        public int Week { get; set; } = 2;
    }
}
