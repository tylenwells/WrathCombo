using WrathCombo.Combos.PvE.Content;

namespace WrathCombo.Combos.PvE
{
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
