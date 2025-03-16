using WrathCombo.Combos.PvE.Content;

namespace WrathCombo.Combos.PvE
{
    //This defines a FFXIV job type, and maps specific Role and Variant actions to that job
    //Examples
    // GNB.Role.Interject would work, SGE.Role.Interject would not.
    //THis should help for future jobs and future random actions to quickly wireup job appropriate actions
    class Healer
    {
        public class Variant : VariantHealer;
        public class Role : HealerRole;
    }

    class Tank
    {
        public class Variant : VariantTank;
        public class Role : TankRole;
    }

    class MeleeDPS
    {
        public class Variant : VariantPDPS;
        public class Role : MeleeRole;
    }

    class PhysRangedDPS
    {
        public class Variant : VariantPDPS;
        public class Role : RangedRole;
    }

    class MagicDPS
    {
        public class Variant : VariantMDPS;
        public class Role : CasterRole;
    }



}
