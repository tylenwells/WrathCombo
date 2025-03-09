using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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



}
