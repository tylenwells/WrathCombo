using ECommons.DalamudServices;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE.Content;

internal static class VariantActions
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
        IsEnabled(preset) && IsEnabled(VariantRampart) && IsOffCooldown(VariantRampart);

    public static bool CanSpiritDart(CustomComboPreset preset) =>
        IsEnabled(preset) && IsEnabled(VariantSpiritDart) && HasBattleTarget() && GetDebuffRemainingTime(VariantActions.Debuffs.SustainedDamage) <= 3;

    public static bool CanCure(CustomComboPreset preset, int healthpercent) =>
        IsEnabled(preset) && IsEnabled(VariantCure) &&
        PlayerHealthPercentageHp() <= healthpercent;

    public static bool CanRaise(CustomComboPreset preset) =>
        IsEnabled(preset) && IsEnabled(VariantRaise) && HasEffect(MagicRole.Buffs.Swiftcast);

    public static bool CanUltimatum(CustomComboPreset preset) => CanCircleAoe(5) > 0;



}

public class VariantTank
{
    public static uint Cure => VariantActions.VariantCure;
    public static uint Ultimatum => VariantActions.VariantUltimatum;
    public static uint Raise => VariantActions.VariantRaise;
    public static uint SpiritDart => VariantActions.VariantSpiritDart;

    public static bool CanCure(CustomComboPreset preset, int healthpercent) => VariantActions.CanCure(preset, healthpercent);
    public static bool CanUltimatum(CustomComboPreset preset) => VariantActions.CanUltimatum(preset);
    public static bool CanRaise(CustomComboPreset preset) => VariantActions.CanRaise(preset);
    public static bool CanSpiritDart(CustomComboPreset preset) => VariantActions.CanSpiritDart(preset);

}

public class VariantHealer
{
    public static uint SpiritDart => VariantActions.VariantSpiritDart;
    public static uint Rampart => VariantActions.VariantRampart;
    public static uint Ultimatum => VariantActions.VariantUltimatum;

    public static bool CanSpiritDart(CustomComboPreset preset) => VariantActions.CanSpiritDart(preset);
    public static bool CanRampart(CustomComboPreset preset) => VariantActions.CanRampart(preset) && CanSpellWeave();
    public static bool CanUltimatum(CustomComboPreset preset) => VariantActions.CanUltimatum(preset);
}
public class VariantPDPS
{
    public static uint Cure => VariantActions.VariantCure;
    public static uint Ultimatum => VariantActions.VariantUltimatum;
    public static uint Raise => VariantActions.VariantRaise;
    public static uint Rampart => VariantActions.VariantRampart;

    public static bool CanCure(CustomComboPreset preset, int healthpercent) => VariantActions.CanCure(preset, healthpercent);
    public static bool CanUltimatum(CustomComboPreset preset) => VariantActions.CanUltimatum(preset);
    public static bool CanRaise(CustomComboPreset preset) => VariantActions.CanRaise(preset);
    public static bool CanRampart(CustomComboPreset preset, bool checkweave = true) => 
        VariantActions.CanRampart(preset) && 
        (!checkweave || CanWeave());
}

public class VariantMDPS
{
    public static uint Cure => VariantActions.VariantCure;
    public static uint Ultimatum => VariantActions.VariantUltimatum;
    public static uint Raise => VariantActions.VariantRaise;
    public static uint Rampart => VariantActions.VariantRampart;

    public static bool CanCure(CustomComboPreset preset, int healthpercent) => VariantActions.CanCure(preset, healthpercent);
    public static bool CanUltimatum(CustomComboPreset preset) => VariantActions.CanUltimatum(preset);
    public static bool CanRaise(CustomComboPreset preset) => VariantActions.CanRaise(preset);
    public static bool CanRampart(CustomComboPreset preset) => VariantActions.CanRampart(preset) && CanSpellWeave();
}
