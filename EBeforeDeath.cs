using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Linq;

namespace Kalista
{
    class EBeforeDeath
    {
        internal class Time
        {
            private static readonly DateTime AssemblyLoadTime = DateTime.Now;
            public static float LastRendTick { get; set; }
            public static float LastNonKillable { get; set; }

            public static float TickCount
            {
                get
                {
                    return (int)DateTime.Now.Subtract(AssemblyLoadTime).TotalMilliseconds;
                }
            }

            public static bool CheckRendDelay()
            {
                return !(TickCount - LastRendTick < 750);
            }

            public static bool CheckNonKillable()
            {
                return !(TickCount - LastNonKillable < 2000);
            }
        }

        public static bool EDeath()
        {
            if (!Time.CheckRendDelay()) return false;

            var champs = 0;
            foreach (var target in HeroManager.Enemies)
            {
                if (!target.IsValid) continue;
                if (!target.IsValidTarget(1000)) continue;
                if (!target.HasBuff("KalistaExpungeMarker")) continue;
                if (!Time.CheckRendDelay()) continue;
                if (ObjectManager.Player.HealthPercent > Config.SliderLinks["miscEBeforeDeathMaxHP"].Value.Value) continue;
                if (target.GetBuffCount("kalistaexpungemarker") < Config.SliderLinks["miscEBeforeDeathStacks"].Value.Value) continue;
                champs++;
                if (champs < Config.SliderLinks["miscEBeforeDeathChamps"].Value.Value) continue;

                UseRend();
                return true;
            }
            return false;
        }

        public static void UseRend()
        {
            ActiveModes.E.Cast();

            Time.LastRendTick = Time.TickCount;
        }
    }
}
