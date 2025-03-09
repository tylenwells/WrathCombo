using ECommons.DalamudServices;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE.Content;

internal static class Variant
{
    public const uint
        VariantUltimatum = 29730,
        VariantRaise = 29731,
        VariantRaise2 = 29734;

    //1069 = The Sil'dihn Subterrane
    //1137 = Mount Rokkon
    //1176 = Aloalo Island
    public static uint VariantCure => Svc.ClientState.TerritoryType switch
    {
        1069 => 29729,
        1137 or 1176 => 33862,
        _ => 0
    };

    public static uint VariantSpiritDart => Svc.ClientState.TerritoryType switch
    {
        1069 => 29732,
        1137 or 1176 => 33863,
        _ => 0
    };

    public static uint VariantRampart => Svc.ClientState.TerritoryType switch
    {
        1069 => 29733,
        1137 or 1176 => 33864,
        _ => 0
    };

    public static class Buffs
    {
        public const ushort
            EmnityUp = 3358,
            VulnDown = 3360,
            Rehabilitation = 3367,
            DamageBarrier = 3405;
    }

    public static class Debuffs
    {
        public const ushort
            SustainedDamage = 3359;
    }

    public static bool CanRampart(CustomComboPreset preset) =>
        IsEnabled(preset) && IsEnabled(VariantRampart) && IsOffCooldown(VariantRampart) && CanSpellWeave();

    public static bool CanSpiritDart(CustomComboPreset preset) =>
        IsEnabled(preset) && IsEnabled(VariantSpiritDart) && HasBattleTarget() && GetDebuffRemainingTime(Variant.Debuffs.SustainedDamage) <= 3;

    public static bool CanCure(CustomComboPreset preset) => true; //TBD
    public static bool CanRaise(CustomComboPreset preset) => true; //TBD
    public static bool CanUltimatum(CustomComboPreset preset) => true; //TBD



}

public class VariantTank
{
    public static uint Cure => Variant.VariantCure;
    public static uint Ultimatum => Variant.VariantUltimatum;
    public static uint Raise => Variant.VariantRaise;
    public static uint SpiritDart => Variant.VariantSpiritDart;

    public static bool CanCure(CustomComboPreset preset) => Variant.CanCure(preset);
    public static bool CanUltimatum(CustomComboPreset preset) => Variant.CanUltimatum(preset);
    public static bool CanRaise(CustomComboPreset preset) => Variant.CanRaise(preset);
    public static bool CanSpiritDart(CustomComboPreset preset) => Variant.CanSpiritDart(preset);

}

public class VariantHealer
{
    public static uint SpiritDart => Variant.VariantSpiritDart;
    public static uint Rampart => Variant.VariantRampart;
    public static uint Ultimatum => Variant.VariantUltimatum;

    public static bool CanSpiritDart(CustomComboPreset preset) => Variant.CanSpiritDart(preset);
    public static bool CanRampart(CustomComboPreset preset) => Variant.CanRampart(preset);
    public static bool CanUltimatum(CustomComboPreset preset) => Variant.CanUltimatum(preset);
}

public class VariantDPS
{
    public static uint Cure => Variant.VariantCure;
    public static uint Ultimatum => Variant.VariantUltimatum;
    public static uint Raise => Variant.VariantRaise;
    public static uint Rampart => Variant.VariantRampart;

    public static bool CanCure(CustomComboPreset preset) => Variant.CanCure(preset);
    public static bool CanUltimatum(CustomComboPreset preset) => Variant.CanUltimatum(preset);
    public static bool CanRaise(CustomComboPreset preset) => Variant.CanRaise(preset);
    public static bool CanRampart(CustomComboPreset preset) => Variant.CanRampart(preset);
}
