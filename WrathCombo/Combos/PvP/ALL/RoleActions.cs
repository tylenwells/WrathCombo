using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvP
{
    class PvPCaster //Offensive Magic
    {
        public const uint
            Comet = 43252,
            PhantomDart = 43291,
            Rust = 43254;

        public class Buffs
        {
            public const ushort
                HasComet = 4492,
                HasPhantomDart = 4516,
                HasRust = 4494;                
        }

        public class Debuffs
        {
            public const ushort
                PhantomDart = 1234,
                Rust = 4480;
        }
    }

    class PvPHealer //Healers
    {
        public const uint
            Haelan = 1234,
            StoneskinII = 1234,
            Diabrosis = 1234;

        public class Buffs
        {
            public const ushort
                StoneSkinII = 1234,
                Diabrosis = 1234;
        }
    }

    class PvPPhysRanged
    {
        public const uint
            Devrish = 1234,
            Bravery = 1234,
            EagleEyeShot = 1234;
    }

    class PvPMelee
    {
        public const uint
            Bloodbath = 1234,
            Swift = 1234,
            Smile = 1234;

        public static bool CanBloodBath(int healthpercent) =>
            IsEnabled(Bloodbath) && ActionReady(Bloodbath) && PlayerHealthPercentageHp() < healthpercent;
    }

    class PvPTank
    {
        public const uint
            Rampage = 1234,
            Rampart = 1234,
            FullSwing = 1234;

        public static bool CanRampage() =>
            IsEnabled(Rampage) && ActionReady(Rampage);

        public static bool CanRampart(int healthPercent) =>
            IsEnabled(Rampage) && ActionReady(Rampart) && PlayerHealthPercentageHp() < healthPercent;

        public static bool CanFullSwing() =>
            IsEnabled(FullSwing) && ActionReady(FullSwing);
    }
}
