#region Dependencies
using System.Linq;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
#endregion

namespace WrathCombo.Combos.PvE;

internal partial class GNB
{
    #region Simple Mode - Single Target
    internal class GNB_ST_Simple : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_ST_Simple;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not KeenEdge)
                return actionID;

            #region Non-Standard
            uint variantAction = GetVariantAction();
            if (variantAction != 0)
                return variantAction;

            if (Bozja.IsInBozja)
            {
                uint bozjaAction = GetBozjaAction();
                if (bozjaAction != 0)
                    return bozjaAction;
            }

            if (ActionReady(All.Interject)
                && CanInterruptEnemy())
                return All.Interject;

            #region Mitigations
            if (Config.GNB_ST_MitsOptions != 1)
            {
                if (InCombat() &&
                    !JustMitted)
                {
                    if (ActionReady(Superbolide) &&
                        PlayerHealthPercentageHp() < 30)
                        return Superbolide;

                    if (IsPlayerTargeted())
                    {
                        if (ActionReady(OriginalHook(Nebula)) &&
                            PlayerHealthPercentageHp() < 60)
                            return OriginalHook(Nebula);

                        if (ActionReady(All.Rampart) &&
                            PlayerHealthPercentageHp() < 80)
                            return All.Rampart;

                        if (ActionReady(All.Reprisal) &&
                            InActionRange(All.Reprisal) &&
                            PlayerHealthPercentageHp() < 90)
                            return All.Reprisal;
                    }

                    if (ActionReady(Camouflage) &&
                        PlayerHealthPercentageHp() < 70)
                        return Camouflage;

                    if (ActionReady(OriginalHook(HeartOfStone)) &&
                        PlayerHealthPercentageHp() < 90)
                        return OriginalHook(HeartOfStone);

                    if (ActionReady(Aurora) &&
                        !(HasEffect(Buffs.Aurora) || TargetHasEffectAny(Buffs.Aurora)) &&
                        PlayerHealthPercentageHp() < 85)
                        return Aurora;
                }
            }
            #endregion

            #endregion

            #region Standard
            if (ShouldUseLightningShot())
                return LightningShot;

            if (ShouldUseNoMercy())
                return NoMercy;

            if (JustUsed(BurstStrike, 5f) &&
                LevelChecked(Hypervelocity) &&
                HasEffect(Buffs.ReadyToBlast) &&
                NmCD is > 1 or <= 0.1f)
                return Hypervelocity;

            if (ShouldUseContinuation())
                return OriginalHook(Continuation);

            if (ShouldUseBloodfest())
                return Bloodfest;

            if (ShouldUseZone())
                return OriginalHook(DangerZone);

            if (ShouldUseBowShock())
                return BowShock;

            if (LevelChecked(DoubleDown) &&
                HasNM &&
                GunStep == 0 &&
                ComboAction is BrutalShell &&
                Ammo == 1)
                return SolidBarrel;

            if (ShouldUseGnashingFang())
                return GnashingFang;

            if (ShouldUseDoubleDown())
                return DoubleDown;

            if (ShouldUseSonicBreak())
                return SonicBreak;

            if (ShouldUseReignOfBeasts())
                return ReignOfBeasts;

            if (ShouldUseBurstStrike())
                return BurstStrike;

            if (GunStep is 1 or 2)
                return OriginalHook(GnashingFang);
            if (GunStep is 3 or 4)
                return OriginalHook(ReignOfBeasts);

            if (ComboTimer > 0)
            {
                if (LevelChecked(BrutalShell) &&
                    ComboAction == KeenEdge)
                    return BrutalShell;

                if (LevelChecked(SolidBarrel) &&
                    ComboAction == BrutalShell)
                {
                    if (LevelChecked(Hypervelocity) &&
                        HasEffect(Buffs.ReadyToBlast) &&
                        NmCD is > 1 or <= 0.1f)
                        return Hypervelocity;

                    return SolidBarrel;
                }
            }
            #endregion

            return KeenEdge;
        }
    }
    #endregion

    #region Advanced Mode - Single Target
    internal class GNB_ST_Advanced : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_ST_Advanced;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not KeenEdge)
                return actionID;

            #region Non-Standard
            uint variantAction = GetVariantAction();
            if (variantAction != 0)
                return variantAction;

            if (Bozja.IsInBozja)
            {
                uint bozjaAction = GetBozjaAction();
                if (bozjaAction != 0)
                    return bozjaAction;
            }

            if (IsEnabled(CustomComboPreset.GNB_ST_Interrupt)
                && ActionReady(All.Interject)
                && CanInterruptEnemy())
                return All.Interject;

            #region Mitigations
            if (IsEnabled(CustomComboPreset.GNB_ST_Mitigation) &&
                InCombat() &&
                !JustMitted)
            {
                if (IsEnabled(CustomComboPreset.GNB_ST_Superbolide) &&
                    ActionReady(Superbolide) &&
                    PlayerHealthPercentageHp() < Config.GNB_ST_Superbolide_Health &&
                    (Config.GNB_ST_Superbolide_SubOption == 0 ||
                     TargetIsBoss() && Config.GNB_ST_Superbolide_SubOption == 1))
                    return Superbolide;

                if (IsPlayerTargeted())
                {
                    if (IsEnabled(CustomComboPreset.GNB_ST_Nebula) &&
                        ActionReady(OriginalHook(Nebula)) &&
                        PlayerHealthPercentageHp() < Config.GNB_ST_Nebula_Health &&
                        (Config.GNB_ST_Nebula_SubOption == 0 ||
                         TargetIsBoss() && Config.GNB_ST_Nebula_SubOption == 1))
                        return OriginalHook(Nebula);

                    if (IsEnabled(CustomComboPreset.GNB_ST_Rampart) &&
                        ActionReady(All.Rampart) &&
                        PlayerHealthPercentageHp() < Config.GNB_ST_Rampart_Health &&
                        (Config.GNB_ST_Rampart_SubOption == 0 ||
                         TargetIsBoss() && Config.GNB_ST_Rampart_SubOption == 1))
                        return All.Rampart;

                    if (IsEnabled(CustomComboPreset.GNB_ST_Reprisal) &&
                        ActionReady(All.Reprisal) &&
                        InActionRange(All.Reprisal) &&
                        PlayerHealthPercentageHp() < Config.GNB_ST_Reprisal_Health &&
                        (Config.GNB_ST_Reprisal_SubOption == 0 ||
                         TargetIsBoss() && Config.GNB_ST_Reprisal_SubOption == 1))
                        return All.Reprisal;

                    if (IsEnabled(CustomComboPreset.GNB_ST_ArmsLength) &&
                        ActionReady(All.ArmsLength) &&
                        PlayerHealthPercentageHp() < Config.GNB_ST_ArmsLength_Health &&
                        !InBossEncounter())
                        return All.ArmsLength;
                }

                if (IsEnabled(CustomComboPreset.GNB_ST_Camouflage) &&
                    ActionReady(Camouflage) &&
                    PlayerHealthPercentageHp() < Config.GNB_ST_Camouflage_Health &&
                    (Config.GNB_ST_Camouflage_SubOption == 0 ||
                     TargetIsBoss() && Config.GNB_ST_Camouflage_SubOption == 1))
                    return Camouflage;

                if (IsEnabled(CustomComboPreset.GNB_ST_Corundum) &&
                    ActionReady(OriginalHook(HeartOfStone)) &&
                    PlayerHealthPercentageHp() < Config.GNB_ST_Corundum_Health &&
                    (Config.GNB_ST_Corundum_SubOption == 0 ||
                     TargetIsBoss() && Config.GNB_ST_Corundum_SubOption == 1))
                    return OriginalHook(HeartOfStone);

                if (IsEnabled(CustomComboPreset.GNB_ST_Aurora) &&
                    ActionReady(Aurora) &&
                    !(HasEffect(Buffs.Aurora) || TargetHasEffectAny(Buffs.Aurora)) &&
                    GetRemainingCharges(Aurora) > Config.GNB_ST_Aurora_Charges &&
                    PlayerHealthPercentageHp() < Config.GNB_ST_Aurora_Health &&
                    (Config.GNB_ST_Aurora_SubOption == 0 ||
                     TargetIsBoss() && Config.GNB_ST_Aurora_SubOption == 1))
                    return Aurora;
            }
            #endregion

            #endregion

            #region Standard
            if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_Opener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            if (IsEnabled(CustomComboPreset.GNB_ST_RangedUptime) &&
                ShouldUseLightningShot())
                return LightningShot;

            if (IsEnabled(CustomComboPreset.GNB_ST_NoMercy) &&
                ShouldUseNoMercy() &&
                (Config.GNB_ST_NoMercy_SubOption == 0 ||
                 Config.GNB_ST_NoMercy_SubOption == 1 && InBossEncounter()))
            {
                if (LevelChecked(DoubleDown))
                {
                    if (InOdd &&
                        (Ammo >= 2 || ComboAction is BrutalShell && Ammo == 1) ||
                        !InOdd &&
                        Ammo != 3)
                        return NoMercy;
                }
                if (!LevelChecked(DoubleDown))
                {
                    if (CanLateWeave &&
                        Ammo > 0)
                        return NoMercy;
                }
            }

            if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_Cooldowns) &&
                IsEnabled(CustomComboPreset.GNB_ST_Continuation) &&
                JustUsed(BurstStrike, 5f) &&
                LevelChecked(Hypervelocity) &&
                HasEffect(Buffs.ReadyToBlast) &&
                IsEnabled(CustomComboPreset.GNB_ST_NoMercy) &&
                NmCD is > 1 or <= 0.1f)
                return Hypervelocity;

            if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_Cooldowns) &&
                IsEnabled(CustomComboPreset.GNB_ST_Continuation) &&
                ShouldUseContinuation())
                return OriginalHook(Continuation);

            if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_Cooldowns))
            {
                if (IsEnabled(CustomComboPreset.GNB_ST_Bloodfest) &&
                    ShouldUseBloodfest())
                    return Bloodfest;

                if (IsEnabled(CustomComboPreset.GNB_ST_Zone) &&
                    ShouldUseZone())
                    return OriginalHook(DangerZone);

                if (IsEnabled(CustomComboPreset.GNB_ST_BowShock) &&
                    ShouldUseBowShock())
                    return BowShock;
            }

            if (LevelChecked(DoubleDown) &&
                HasNM &&
                GunStep == 0 &&
                ComboAction is BrutalShell &&
                Ammo == 1)
                return SolidBarrel;

            if (IsEnabled(CustomComboPreset.GNB_ST_Advanced_Cooldowns))
            {
                if (IsEnabled(CustomComboPreset.GNB_ST_GnashingFang) &&
                    ShouldUseGnashingFang())
                    return GnashingFang;

                if (IsEnabled(CustomComboPreset.GNB_ST_DoubleDown) &&
                    ShouldUseDoubleDown())
                    return DoubleDown;

                if (IsEnabled(CustomComboPreset.GNB_ST_SonicBreak) &&
                    ShouldUseSonicBreak())
                    return SonicBreak;

                if (IsEnabled(CustomComboPreset.GNB_ST_Reign) &&
                    ShouldUseReignOfBeasts())
                    return OriginalHook(ReignOfBeasts);

                if (ShouldUseAdvancedBS())
                    return BurstStrike;
            }

            if (IsEnabled(CustomComboPreset.GNB_ST_GnashingFang) &&
                GunStep is 1 or 2)
                return OriginalHook(GnashingFang);
            if (IsEnabled(CustomComboPreset.GNB_ST_Reign) &&
                GunStep is 3 or 4)
                return OriginalHook(ReignOfBeasts);

            if (ComboTimer > 0)
            {
                if (LevelChecked(BrutalShell) &&
                    ComboAction == KeenEdge)
                    return BrutalShell;

                if (LevelChecked(SolidBarrel) &&
                    ComboAction == BrutalShell)
                {
                    if (IsEnabled(CustomComboPreset.GNB_ST_Continuation) &&
                        IsEnabled(CustomComboPreset.GNB_ST_NoMercy) &&
                        LevelChecked(Hypervelocity) &&
                        HasEffect(Buffs.ReadyToBlast) &&
                        (NmCD is > 1 or <= 0.1f &&
                         Config.GNB_ST_NoMercy_SubOption == 0 ||
                         Config.GNB_ST_NoMercy_SubOption == 1 && InBossEncounter()))
                        return Hypervelocity;

                    return SolidBarrel;
                }
            }
            #endregion

            return KeenEdge;
        }
    }
    #endregion

    #region Simple Mode - AoE

    internal class GNB_AoE_Simple : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_AoE_Simple;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DemonSlice)
                return actionID;

            // Variant
            uint variantAction = GetVariantAction();
            if (variantAction != 0)
                return variantAction;

            // Bozja
            if (Bozja.IsInBozja)
            {
                uint bozjaAction = GetBozjaAction();
                if (bozjaAction != 0)
                    return bozjaAction;
            }

            #region Stuns
            // Interrupt
            if (ActionReady(All.Interject)
                && CanInterruptEnemy())
                return All.Interject;

            // Stun
            if (ActionReady(All.LowBlow)
                && TargetIsCasting())
                return All.LowBlow;
            #endregion

            #region Mitigations
            if (Config.GNB_AoE_MitsOptions != 1)
            {
                if (InCombat() && //Player is in combat
                    !JustMitted) //Player has not used a mitigation ability in the last 4-9 seconds
                {
                    //Superbolide
                    if (ActionReady(Superbolide) && //Superbolide is ready
                        PlayerHealthPercentageHp() < 30) //Player's health is below 30%
                        return Superbolide;

                    if (IsPlayerTargeted())
                    {
                        //Nebula
                        if (ActionReady(OriginalHook(Nebula)) && //Nebula is ready
                            PlayerHealthPercentageHp() < 60) //Player's health is below 60%
                            return OriginalHook(Nebula);

                        //Rampart
                        if (ActionReady(All.Rampart) && //Rampart is ready
                            PlayerHealthPercentageHp() < 80) //Player's health is below 80%
                            return All.Rampart;

                        //Reprisal
                        if (ActionReady(All.Reprisal) && //Reprisal is ready
                            InActionRange(All.Reprisal) && //Player is in range of Reprisal
                            PlayerHealthPercentageHp() < 90) //Player's health is below 90%
                            return All.Reprisal;
                    }

                    //Camouflage
                    if (ActionReady(Camouflage) && //Camouflage is ready
                        PlayerHealthPercentageHp() < 70) //Player's health is below 80%
                        return Camouflage;

                    //Corundum
                    if (ActionReady(OriginalHook(HeartOfStone)) && //Corundum
                        PlayerHealthPercentageHp() < 90) //Player's health is below 95%
                        return OriginalHook(HeartOfStone);

                    //Aurora
                    if (ActionReady(Aurora) && //Aurora is ready
                        !(HasEffect(Buffs.Aurora) || TargetHasEffectAny(Buffs.Aurora)) && //Aurora is not active on self or target
                        PlayerHealthPercentageHp() < 85) //Player's health is below 85%
                        return Aurora;
                }
            }
            #endregion

            #region Rotation
            if (InCombat()) //if already in combat
            {
                if (CanWeave()) //if we can weave
                {
                    //NoMercy
                    if (ActionReady(NoMercy) && //if No Mercy is ready
                        GetTargetHPPercent() > 5) //if target HP is above threshold
                        return NoMercy; //execute No Mercy
                    //BowShock
                    if (CanBow && //if Bow Shock is ready
                        HasNM) //if No Mercy is active
                        return BowShock; //execute Bow Shock
                    //Zone
                    if (CanZone &&
                        NmCD is < 57.5f and > 17) //use on CD after first usage in NM
                        return OriginalHook(DangerZone); //execute Zone
                    //Bloodfest
                    if (CanBF) //if Bloodfest is ready & gauge is empty
                        return Bloodfest; //execute Bloodfest
                    //Continuation
                    if (LevelChecked(FatedBrand) && //if Fated Brand is unlocked
                        HasEffect(Buffs.ReadyToRaze)) //if Ready To Raze is active
                        return FatedBrand; //execute Fated Brand
                }

                //SonicBreak
                if (CanBreak && //if Ready To Break is active & unlocked
                    !HasEffect(Buffs.ReadyToRaze) && //if Ready To Raze is not active
                    HasNM) //if No Mercy is active
                    return SonicBreak;
                //DoubleDown
                if (CanDD && //if Double Down is ready && gauge is not empty
                    HasNM) //if No Mercy is active
                    return DoubleDown; //execute Double Down
                //Reign - because leaving this out anywhere is a waste
                if (LevelChecked(ReignOfBeasts)) //if Reign of Beasts is unlocked
                {
                    if (CanReign || //can execute Reign of Beasts
                        GunStep is 3 or 4) //can execute Noble Blood or Lion Heart
                        return OriginalHook(ReignOfBeasts);
                }
                //FatedCircle - if not LevelChecked, use BurstStrike
                if (CanFC && //if Fated Circle is unlocked && gauge is not empty
                    //Normal
                    (HasNM && //if No Mercy is active
                     !ActionReady(DoubleDown) && //if Double Down is not ready
                     GunStep == 0 || //if Gnashing Fang or Reign combo is not active
                     //Bloodfest prep
                     IsEnabled(CustomComboPreset.GNB_AoE_Bloodfest) && //if Bloodfest option is enabled
                     BfCD < 6)) //if Bloodfest is about to be ready
                    return FatedCircle;
                if (Ammo > 0 && //if gauge is not empty
                    !LevelChecked(FatedCircle) && //if Fated Circle is not unlocked
                    LevelChecked(BurstStrike) && //if Burst Strike is unlocked
                    HasNM && //if No Mercy is active
                    GunStep == 0) //if Gnashing Fang or Reign combo is not active
                    return BurstStrike;
            }

            //1-2
            if (ComboTimer > 0) //if we're in combo
            {
                if (ComboAction == DemonSlice && //if last action was Demon Slice
                    LevelChecked(DemonSlaughter)) //if Demon Slaughter is unlocked
                {
                    if (Ammo == MaxCartridges())
                    {
                        if (LevelChecked(FatedCircle)) //if Fated Circle is unlocked
                            return FatedCircle; //execute Fated Circle
                        if (!LevelChecked(FatedCircle)) //if Fated Circle is not unlocked
                            return BurstStrike; //execute Burst Strike
                    }
                    if (Ammo != MaxCartridges()) //if gauge is full && if Fated Circle is not unlocked
                        return DemonSlaughter; //execute Demon Slaughter
                }
            }
            #endregion

            return DemonSlice; //Always default back to Demon Slice
        }
    }

    #endregion

    #region Advanced Mode - AoE

    internal class GNB_AoE_Advanced : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_AoE_Advanced;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not DemonSlice)
                return actionID;

            // Variant
            uint variantAction = GetVariantAction();
            if (variantAction != 0)
                return variantAction;

            // Bozja
            if (Bozja.IsInBozja)
            {
                uint bozjaAction = GetBozjaAction();
                if (bozjaAction != 0)
                    return bozjaAction;
            }

            #region Stuns
            // Interrupt
            if (IsEnabled(CustomComboPreset.GNB_AoE_Interrupt)
                && ActionReady(All.Interject)
                && CanInterruptEnemy())
                return All.Interject;

            // Stun
            if (IsEnabled(CustomComboPreset.GNB_AoE_Stun)
                && ActionReady(All.LowBlow)
                && TargetIsCasting())
                return All.LowBlow;
            #endregion

            #region Mitigations
            if (IsEnabled(CustomComboPreset.GNB_AoE_Mitigation) && //Mitigation option is enabled
                InCombat() && //Player is in combat
                !JustMitted) //Player has not used a mitigation ability in the last 4-9 seconds
            {
                //Superbolide
                if (IsEnabled(CustomComboPreset.GNB_AoE_Superbolide) && //Superbolide option is enabled
                    ActionReady(Superbolide) && //Superbolide is ready
                    PlayerHealthPercentageHp() < Config.GNB_AoE_Superbolide_Health && //Player's health is below selected threshold
                    (Config.GNB_AoE_Superbolide_SubOption == 0 || //Superbolide is enabled for all targets
                     TargetIsBoss() && Config.GNB_AoE_Superbolide_SubOption == 1)) //Superbolide is enabled for bosses only
                    return Superbolide;

                if (IsPlayerTargeted()) //Player is being targeted by current target
                {
                    //Nebula
                    if (IsEnabled(CustomComboPreset.GNB_AoE_Nebula) && //Nebula option is enabled
                        ActionReady(OriginalHook(Nebula)) && //Nebula is ready
                        PlayerHealthPercentageHp() < Config.GNB_AoE_Nebula_Health && //Player's health is below selected threshold
                        (Config.GNB_AoE_Nebula_SubOption == 0 || //Nebula is enabled for all targets
                         TargetIsBoss() && Config.GNB_AoE_Nebula_SubOption == 1)) //Nebula is enabled for bosses only
                        return OriginalHook(Nebula);

                    //Rampart
                    if (IsEnabled(CustomComboPreset.GNB_AoE_Rampart) && //Rampart option is enabled
                        ActionReady(All.Rampart) && //Rampart is ready
                        PlayerHealthPercentageHp() < Config.GNB_AoE_Rampart_Health && //Player's health is below selected threshold
                        (Config.GNB_AoE_Rampart_SubOption == 0 || //Rampart is enabled for all targets
                         TargetIsBoss() && Config.GNB_AoE_Rampart_SubOption == 1)) //Rampart is enabled for bosses only
                        return All.Rampart;

                    //Reprisal
                    if (IsEnabled(CustomComboPreset.GNB_AoE_Reprisal) && //Reprisal option is enabled
                        ActionReady(All.Reprisal) && //Reprisal is ready
                        InActionRange(All.Reprisal) && //Player is in range of Reprisal
                        PlayerHealthPercentageHp() < Config.GNB_AoE_Reprisal_Health && //Player's health is below selected threshold
                        (Config.GNB_AoE_Reprisal_SubOption == 0 || //Reprisal is enabled for all targets
                         TargetIsBoss() && Config.GNB_AoE_Reprisal_SubOption == 1)) //Reprisal is enabled for bosses only
                        return All.Reprisal;

                    //Arm's Length
                    if (IsEnabled(CustomComboPreset.GNB_AoE_ArmsLength) && //Arm's Length option is enabled
                        ActionReady(All.ArmsLength) && //Arm's Length is ready
                        PlayerHealthPercentageHp() < Config.GNB_AoE_ArmsLength_Health && //Player's health is below selected threshold
                        !InBossEncounter()) //Arms Length is enabled for bosses only
                        return All.ArmsLength;
                }

                //Camouflage
                if (IsEnabled(CustomComboPreset.GNB_AoE_Camouflage) && //Camouflage option is enabled
                    ActionReady(Camouflage) && //Camouflage is ready
                    PlayerHealthPercentageHp() < Config.GNB_AoE_Camouflage_Health && //Player's health is below selected threshold
                    (Config.GNB_AoE_Camouflage_SubOption == 0 || //Camouflage is enabled for all targets
                     TargetIsBoss() && Config.GNB_AoE_Camouflage_SubOption == 1)) //Camouflage is enabled for bosses only
                    return Camouflage;

                //Corundum
                if (IsEnabled(CustomComboPreset.GNB_AoE_Corundum) && //Corundum option is enabled
                    ActionReady(OriginalHook(HeartOfStone)) && //Corundum is ready
                    PlayerHealthPercentageHp() < Config.GNB_AoE_Corundum_Health && //Player's health is below selected threshold
                    (Config.GNB_AoE_Corundum_SubOption == 0 || //Corundum is enabled for all targets
                     TargetIsBoss() && Config.GNB_AoE_Corundum_SubOption == 1)) //Corundum is enabled for bosses only
                    return OriginalHook(HeartOfStone);

                //Aurora
                if (IsEnabled(CustomComboPreset.GNB_AoE_Aurora) && //Aurora option is enabled
                    ActionReady(Aurora) && //Aurora is ready
                    GetRemainingCharges(Aurora) > Config.GNB_AoE_Aurora_Charges && //Aurora has more charges than set threshold
                    !(HasEffect(Buffs.Aurora) || TargetHasEffectAny(Buffs.Aurora)) && //Aurora is not already active on player or target
                    PlayerHealthPercentageHp() < Config.GNB_AoE_Aurora_Health && //Player's health is below selected threshold
                    (Config.GNB_AoE_Aurora_SubOption == 0 || //Aurora is enabled for all targets
                     TargetIsBoss() && Config.GNB_AoE_Aurora_SubOption == 1)) //Aurora is enabled for bosses only
                    return Aurora;
            }
            #endregion

            #region Rotation
            if (InCombat()) //if already in combat
            {
                if (CanWeave()) //if we can weave
                {
                    //NoMercy
                    if (IsEnabled(CustomComboPreset.GNB_AoE_NoMercy) && //if No Mercy option is enabled
                        ActionReady(NoMercy) && //if No Mercy is ready
                        GetTargetHPPercent() > NmStop) //if target HP is above threshold
                        return NoMercy; //execute No Mercy
                    //BowShock
                    if (IsEnabled(CustomComboPreset.GNB_AoE_BowShock) && //if Bow Shock option is enabled
                        CanBow && //if Bow Shock is ready
                        HasNM) //if No Mercy is active
                        return BowShock; //execute Bow Shock
                    //Zone
                    if (IsEnabled(CustomComboPreset.GNB_AoE_Zone) &&
                        CanZone &&
                        NmCD is < 57.5f and > 17) //use on CD after first usage in NM
                        return OriginalHook(DangerZone); //execute Zone
                    //Bloodfest
                    if (IsEnabled(CustomComboPreset.GNB_AoE_Bloodfest) && //if Bloodfest option is enabled
                        CanBF) //if Bloodfest is ready & gauge is empty
                        return Bloodfest; //execute Bloodfest
                    //Continuation
                    if (LevelChecked(FatedBrand) && //if Fated Brand is unlocked
                        HasEffect(Buffs.ReadyToRaze)) //if Ready To Raze is active
                        return FatedBrand; //execute Fated Brand
                }

                //SonicBreak
                if (IsEnabled(CustomComboPreset.GNB_AoE_SonicBreak) && //if Sonic Break option is enabled
                    CanBreak && //if Ready To Break is active & unlocked
                    !HasEffect(Buffs.ReadyToRaze) && //if Ready To Raze is not active
                    HasNM) //if No Mercy is active
                    return SonicBreak;
                //DoubleDown
                if (IsEnabled(CustomComboPreset.GNB_AoE_DoubleDown) && //if Double Down option is enabled
                    CanDD && //if Double Down is ready && gauge is not empty
                    HasNM) //if No Mercy is active
                    return DoubleDown; //execute Double Down
                //Reign - because leaving this out anywhere is a waste
                if (IsEnabled(CustomComboPreset.GNB_AoE_Reign) && //if Reign of Beasts option is enabled
                    LevelChecked(ReignOfBeasts)) //if Reign of Beasts is unlocked
                {
                    if (CanReign || //can execute Reign of Beasts
                        GunStep is 3 or 4) //can execute Noble Blood or Lion Heart
                        return OriginalHook(ReignOfBeasts);
                }
                //FatedCircle - if not LevelChecked, use BurstStrike
                if (IsEnabled(CustomComboPreset.GNB_AoE_FatedCircle) && //if Fated Circle option is enabled
                    CanFC && //if Fated Circle is unlocked && gauge is not empty
                    //Normal
                    (HasNM && //if No Mercy is active
                     !ActionReady(DoubleDown) && //if Double Down is not ready
                     GunStep == 0 || //if Gnashing Fang or Reign combo is not active
                     //Bloodfest prep
                     IsEnabled(CustomComboPreset.GNB_AoE_Bloodfest) && //if Bloodfest option is enabled
                     BfCD < 6)) //if Bloodfest is about to be ready
                    return FatedCircle;
                if (IsEnabled(CustomComboPreset.GNB_AoE_noFatedCircle) && //if Fated Circle Burst Strike option is disabled
                    Ammo > 0 && //if gauge is not empty
                    !LevelChecked(FatedCircle) && //if Fated Circle is not unlocked
                    LevelChecked(BurstStrike) && //if Burst Strike is unlocked
                    HasNM && //if No Mercy is active
                    GunStep == 0) //if Gnashing Fang or Reign combo is not active
                    return BurstStrike;
            }

            //1-2
            if (ComboTimer > 0) //if we're in combo
            {
                if (ComboAction == DemonSlice && //if last action was Demon Slice
                    LevelChecked(DemonSlaughter)) //if Demon Slaughter is unlocked
                {
                    if (Ammo == MaxCartridges())
                    {
                        if (IsEnabled(CustomComboPreset.GNB_AoE_Overcap) && //if Overcap option is enabled
                            LevelChecked(FatedCircle)) //if Fated Circle is unlocked
                            return FatedCircle; //execute Fated Circle
                        if (IsEnabled(CustomComboPreset.GNB_AoE_BSOvercap) && //if Burst Strike Overcap option is enabled
                            !LevelChecked(FatedCircle)) //if Fated Circle is not unlocked
                            return BurstStrike; //execute Burst Strike
                    }
                    if (Ammo != MaxCartridges() || //if gauge is not full
                        Ammo == MaxCartridges() && //if gauge is full
                        !LevelChecked(FatedCircle) && //if Fated Circle is not unlocked
                        !IsEnabled(CustomComboPreset.GNB_AoE_BSOvercap)) //if Burst Strike Overcap option is disabled
                    {
                        return DemonSlaughter; //execute Demon Slaughter
                    }
                }
            }
            #endregion

            return DemonSlice; //execute Demon Slice
        }
    }

    #endregion

    #region Gnashing Fang Features

    internal class GNB_GF_Features : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_GF_Features;

        protected override uint Invoke(uint actionID)
        {
            bool gFchoice = Config.GNB_GF_Features_Choice == 0; //Gnashing Fang as button
            bool nMchoice = Config.GNB_GF_Features_Choice == 1; //No Mercy as button

            if (gFchoice && actionID is not GnashingFang ||
                nMchoice && actionID is not NoMercy)
                return actionID;

            //oGCDs
            if (CanWeave())
            {
                //No Mercy
                if (IsEnabled(CustomComboPreset.GNB_GF_NoMercy) && //No Mercy option is enabled
                    ActionReady(NoMercy) && //No Mercy is ready
                    InCombat() && //In combat
                    HasTarget() && //Has target
                    CanWeave()) //Able to weave
                {
                    if (LevelChecked(DoubleDown)) //Lv90+
                    {
                        if (InOdd && //Odd Minute window
                            (Ammo >= 2 || ComboAction is BrutalShell && Ammo == 1) || //2 or 3 Ammo or 1 Ammo with Solid Barrel next in combo
                            !InOdd && //Even Minute window
                            Ammo != 3) //Ammo is not full (3)
                            return NoMercy; //Execute No Mercy if conditions are met
                    }
                    if (!LevelChecked(DoubleDown)) //Lv1-89
                    {
                        if (CanLateWeave && //Late-weaveable
                            Ammo == MaxCartridges()) //Ammo is full
                            return NoMercy; //Execute No Mercy if conditions are met
                    }
                }

                //Cooldowns
                if (IsEnabled(CustomComboPreset.GNB_GF_Features)) //Features are enabled
                {
                    //Hypervelocity
                    if (IsEnabled(CustomComboPreset.GNB_GF_Continuation) && //Continuation option is enabled
                        LevelChecked(Hypervelocity) && //Hypervelocity is unlocked
                        (JustUsed(BurstStrike, 1) || //Burst Strike was just used within 1 second
                         HasEffect(Buffs.ReadyToBlast))) //Ready To Blast buff is active
                        return Hypervelocity; //Execute Hypervelocity if conditions are met

                    //Continuation
                    if (IsEnabled(CustomComboPreset.GNB_GF_Continuation) && //Continuation option is enabled
                        CanContinue && //able to use Continuation
                        (HasEffect(Buffs.ReadyToRip) || //after Gnashing Fang
                         HasEffect(Buffs.ReadyToTear) || //after Savage Claw
                         HasEffect(Buffs.ReadyToGouge) || //after Wicked Talon
                         HasEffect(Buffs.ReadyToBlast))) //after Burst Strike
                        return OriginalHook(Continuation); //Execute appopriate Continuation action if conditions are met

                    //Bloodfest
                    if (IsEnabled(CustomComboPreset.GNB_GF_Bloodfest) && //Bloodfest option is enabled
                        InCombat() && //In combat
                        HasTarget() && //Has target
                        CanBF && //able to use Bloodfest
                        Ammo == 0) //Only when ammo is empty
                        return Bloodfest; //Execute Bloodfest if conditions are met

                    //Zone
                    if (IsEnabled(CustomComboPreset.GNB_GF_Zone) && //Zone option is enabled
                        CanZone && //able to use Zone
                        NmCD is < 57.5f and > 17f) //Optimal use; twice per minute, 1 in NM, 1 out of NM
                        return OriginalHook(DangerZone); //Execute Zone if conditions are met

                    //Bow Shock
                    if (IsEnabled(CustomComboPreset.GNB_GF_BowShock) && //Bow Shock option is enabled
                        CanBow && //able to use Bow Shock
                        NmCD is < 57.5f and >= 40) //No Mercy is up & was not just used within 1 GCD
                        return BowShock;
                }
            }

            //GCDs
            if (IsEnabled(CustomComboPreset.GNB_GF_Features)) //Features are enabled
            {
                //GnashingFang
                if (CanGF && //able to use Gnashing Fang
                    (NmCD is > 17 and < 35 || //30s Optimal use
                     JustUsed(NoMercy, 6f))) //No Mercy was just used within 4 seconds
                    return GnashingFang;

                //Double Down
                if (IsEnabled(CustomComboPreset.GNB_GF_DoubleDown) && //Double Down option is enabled
                    CanDD && //able to use Double Down
                    IsOnCooldown(GnashingFang) && //Gnashing Fang is on cooldown
                    HasNM) //No Mercy is active
                    return DoubleDown;

                //Sonic Break
                if (IsEnabled(CustomComboPreset.GNB_GF_SonicBreak) && //Sonic Break option is enabled
                    CanBreak && //able to use Sonic Break
                    (IsOnCooldown(GnashingFang) && IsOnCooldown(DoubleDown) || //Gnashing Fang and Double Down are both on cooldown
                     Ammo == 0)) //No Ammo
                    return SonicBreak; //Execute Sonic Break if conditions are met

                //Reign of Beasts
                if (IsEnabled(CustomComboPreset.GNB_GF_Reign) && //Reign of Beasts option is enabled
                    CanReign && //able to use Reign of Beasts
                    IsOnCooldown(GnashingFang) && //Gnashing Fang is on cooldown
                    IsOnCooldown(DoubleDown) && //Double Down is on cooldown
                    !HasEffect(Buffs.ReadyToBreak) && //Ready To Break is not active
                    GunStep == 0) //Gnashing Fang or Reign combo is not active or finished
                    return OriginalHook(ReignOfBeasts); //Execute Reign of Beasts if conditions are met

                //Burst Strike
                if (IsEnabled(CustomComboPreset.GNB_GF_BurstStrike) && //Burst Strike option is enabled
                    CanBS && //able to use Burst Strike
                    HasNM && //No Mercy is active
                    IsOnCooldown(GnashingFang) && //Gnashing Fang is on cooldown
                    (IsOnCooldown(DoubleDown) || //Double Down is on cooldown
                    !LevelChecked(DoubleDown) && Ammo > 0) && //Double Down is not unlocked and Ammo is not empty
                    !HasEffect(Buffs.ReadyToReign) && //Ready To Reign is not active
                    GunStep == 0) //Gnashing Fang or Reign combo is not active or finished
                    return BurstStrike; //Execute Burst Strike if conditions are met
            }

            //Lv90+ 2cart forced Reopener
            if (IsEnabled(CustomComboPreset.GNB_GF_Features) && //Cooldowns option is enabled
                IsEnabled(CustomComboPreset.GNB_GF_NoMercy) && //No Mercy option is enabled
                IsEnabled(CustomComboPreset.GNB_GF_BurstStrike) && //Burst Strike option is enabled
                LevelChecked(DoubleDown) && //Lv90+
                NmCD < 1 && //No Mercy is ready or about to be
                Ammo is 3 && //Ammo is full
                BfCD > 110 && //Bloodfest was recently used, but not just used
                ComboAction is KeenEdge) //Just used Keen Edge
                return BurstStrike;
            //Lv100 2cart forced 2min starter
            if (IsEnabled(CustomComboPreset.GNB_GF_Features) && //Cooldowns option is enabled
                IsEnabled(CustomComboPreset.GNB_GF_NoMercy) && //No Mercy option is enabled
                IsEnabled(CustomComboPreset.GNB_GF_BurstStrike) && //Burst Strike option is enabled
                LevelChecked(ReignOfBeasts) && //Lv100
                NmCD < 1 && //No Mercy is ready or about to be
                Ammo is 3 && //Ammo is full
                BfCD < GCD * 12) //Bloodfest is ready or about to be
                return BurstStrike;

            //Gauge Combo Steps
            if (GunStep is 1 or 2) //Gnashing Fang combo is only for 1 and 2
                return OriginalHook(GnashingFang); //Execute Gnashing Fang combo if conditions are met

            if (GunStep is 3 or 4) //Reign of Beasts combo is only for 3 and 4
                return OriginalHook(ReignOfBeasts); //Execute Reign of Beasts combo if conditions are met

            return actionID;
        }
    }

    #endregion

    #region Burst Strike Features

    internal class GNB_BS_Features : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_BS_Features;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not BurstStrike)
                return actionID;

            //Hypervelocity
            if (IsEnabled(CustomComboPreset.GNB_BS_Continuation) && //Continuation option is enabled
                IsEnabled(CustomComboPreset.GNB_BS_Hypervelocity) && //Continuation option is enabled
                LevelChecked(Hypervelocity) && //Hypervelocity is unlocked
                (JustUsed(BurstStrike, 1) || //Burst Strike was just used within 1 second
                 HasEffect(Buffs.ReadyToBlast))) //Ready To Blast buff is active
                return Hypervelocity; //Execute Hypervelocity if conditions are met

            //Continuation
            if (IsEnabled(CustomComboPreset.GNB_BS_Continuation) && //Continuation option is enabled
                !IsEnabled(CustomComboPreset.GNB_BS_Hypervelocity) && //Hypervelocity Only option is disabled
                CanContinue && //able to use Continuation
                (HasEffect(Buffs.ReadyToRip) || //after Gnashing Fang
                 HasEffect(Buffs.ReadyToTear) || //after Savage Claw
                 HasEffect(Buffs.ReadyToGouge) || //after Wicked Talon
                 HasEffect(Buffs.ReadyToBlast))) //after Burst Strike
                return OriginalHook(Continuation); //Execute appopriate Continuation action if conditions are met

            //Bloodfest
            if (IsEnabled(CustomComboPreset.GNB_BS_Bloodfest) && //Bloodfest option is enabled
                HasTarget() && //Has target
                CanBF && //able to use Bloodfest
                Ammo == 0) //Only when ammo is empty
                return Bloodfest; //Execute Bloodfest if conditions are met

            //Double Down higher prio if only 1 cartridge
            if (IsEnabled(CustomComboPreset.GNB_BS_DoubleDown) && //Double Down option is enabled
                LevelChecked(DoubleDown) && //Double Down is unlocked
                DdCD < 0.6f && //Double Down is off cooldown
                Ammo == 1) //Has Ammo
                return DoubleDown; //Execute Double Down if conditions are met

            //Gnashing Fang
            if (IsEnabled(CustomComboPreset.GNB_BS_GnashingFang) && //Gnashing Fang option is enabled
                CanGF || GunStep is 1 or 2) //able to use Gnashing Fang combo
                return OriginalHook(GnashingFang); //Execute Gnashing Fang if conditions are met

            //Double Down
            if (IsEnabled(CustomComboPreset.GNB_BS_DoubleDown) && //Double Down option is enabled
                CanDD) //able to use Double Down
                return DoubleDown; //Execute Double Down if conditions are met

            //Reign
            if (IsEnabled(CustomComboPreset.GNB_BS_Reign) && //Reign of Beasts option is enabled
                (CanReign || GunStep is 3 or 4)) //able to use Reign of Beasts
                return OriginalHook(ReignOfBeasts); //Execute Reign combo if conditions are met

            return actionID;
        }
    }

    #endregion

    #region Fated Circle Features

    internal class GNB_FC_Features : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_FC_Features;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not FatedCircle)
                return actionID;

            //Fated Brand
            if (IsEnabled(CustomComboPreset.GNB_FC_Continuation) && //Continuation option is enabled
                HasEffect(Buffs.ReadyToRaze) &&
                LevelChecked(FatedBrand))
                return FatedBrand;

            //Double Down under NM only
            if (IsEnabled(CustomComboPreset.GNB_FC_DoubleDown) && //Double Down option is enabled
                IsEnabled(CustomComboPreset.GNB_FC_DoubleDown_NM) && //Double Down No Mercy option is enabled
                CanDD && //able to use Double Down
                HasNM) //No Mercy is active
                return DoubleDown; //Execute Double Down if conditions are met

            //Bloodfest
            if (IsEnabled(CustomComboPreset.GNB_FC_Bloodfest) && //Bloodfest option is enabled
                HasTarget() && //Has target
                CanBF && //able to use Bloodfest
                Ammo == 0) //Only when ammo is empty
                return Bloodfest; //Execute Bloodfest if conditions are met

            //Bow Shock
            if (IsEnabled(CustomComboPreset.GNB_FC_BowShock) && //Bow Shock option is enabled
                CanBow) //able to use Bow Shock
                return BowShock; //Execute Bow Shock if conditions are met

            //Double Down
            if (IsEnabled(CustomComboPreset.GNB_FC_DoubleDown) && //Double Down option is enabled
                !IsEnabled(CustomComboPreset.GNB_FC_DoubleDown_NM) && //Double Down No Mercy option is disabled
                CanDD) //able to use Double Down
                return DoubleDown; //Execute Double Down if conditions are met

            //Reign
            if (IsEnabled(CustomComboPreset.GNB_FC_Reign) && //Reign of Beasts option is enabled
                (CanReign || GunStep is 3 or 4)) //able to use Reign of Beasts
                return OriginalHook(ReignOfBeasts); //Execute Reign combo if conditions are met

            return actionID;
        }
    }

    #endregion

    #region No Mercy Features

    internal class GNB_NM_Features : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_NM_Features;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not NoMercy)
                return actionID;



            //oGCDs
            if (Config.GNB_NM_Features_Weave == 0) //Weave option is enabled
            {
                if (CanWeave())
                {
                    //Continuation
                    if (IsEnabled(CustomComboPreset.GNB_NM_Continuation) && //Continuation option is enabled
                        CanContinue && //able to use Continuation
                        (HasEffect(Buffs.ReadyToRip) || //after Gnashing Fang
                         HasEffect(Buffs.ReadyToTear) || //after Savage Claw
                         HasEffect(Buffs.ReadyToGouge) || //after Wicked Talon
                         HasEffect(Buffs.ReadyToBlast) || //after Burst Strike
                         HasEffect(Buffs.ReadyToRaze))) //after Fated Circle
                        return OriginalHook(Continuation); //Execute appopriate Continuation action if conditions are met

                    //Bloodfest
                    if (IsEnabled(CustomComboPreset.GNB_NM_Bloodfest) && //Bloodfest option is enabled
                        InCombat() && //In combat
                        HasTarget() && //Has target
                        CanBF && //able to use Bloodfest
                        Ammo == 0) //Only when ammo is empty
                        return Bloodfest; //Execute Bloodfest if conditions are met

                    //Bow Shock
                    if (IsEnabled(CustomComboPreset.GNB_NM_BowShock) && //Bow Shock option is enabled
                        CanBow && //able to use Bow Shock
                        NmCD is < 57.5f and >= 40) //No Mercy is up & was not just used within 1 GCD
                        return BowShock;

                    //Zone
                    if (IsEnabled(CustomComboPreset.GNB_NM_Zone) && //Zone option is enabled
                        CanZone && //able to use Zone
                        NmCD is < 57.5f and > 17f) //Optimal use; twice per minute, 1 in NM, 1 out of NM
                        return OriginalHook(DangerZone); //Execute Zone if conditions are met
                }
            }

            if (Config.GNB_NM_Features_Weave == 1) //Force option is enabled
            {
                //Continuation
                if (IsEnabled(CustomComboPreset.GNB_NM_Continuation) && //Continuation option is enabled
                    CanContinue && //able to use Continuation
                    (HasEffect(Buffs.ReadyToRip) || //after Gnashing Fang
                     HasEffect(Buffs.ReadyToTear) || //after Savage Claw
                     HasEffect(Buffs.ReadyToGouge) || //after Wicked Talon
                     HasEffect(Buffs.ReadyToBlast) || //after Burst Strike
                     HasEffect(Buffs.ReadyToRaze))) //after Fated Circle
                    return OriginalHook(Continuation); //Execute appopriate Continuation action if conditions are met

                //Bloodfest
                if (IsEnabled(CustomComboPreset.GNB_NM_Bloodfest) && //Bloodfest option is enabled
                    InCombat() && //In combat
                    HasTarget() && //Has target
                    CanBF && //able to use Bloodfest
                    Ammo == 0) //Only when ammo is empty
                    return Bloodfest; //Execute Bloodfest if conditions are met

                //Bow Shock
                if (IsEnabled(CustomComboPreset.GNB_NM_BowShock) && //Bow Shock option is enabled
                    CanBow && //able to use Bow Shock
                    NmCD is < 57.5f and >= 40) //No Mercy is up & was not just used within 1 GCD
                    return BowShock;

                //Zone
                if (IsEnabled(CustomComboPreset.GNB_NM_Zone) && //Zone option is enabled
                    CanZone && //able to use Zone
                    NmCD is < 57.5f and > 17f) //Optimal use; twice per minute, 1 in NM, 1 out of NM
                    return OriginalHook(DangerZone); //Execute Zone if conditions are met
            }

            return actionID;
        }
    }

    #endregion

    #region Aurora Protection

    internal class GNB_AuroraProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_AuroraProtection;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Aurora)
                return actionID;

            if (HasFriendlyTarget() && TargetHasEffectAny(Buffs.Aurora) ||
                !HasFriendlyTarget() && HasEffectAny(Buffs.Aurora))
                return All.SavageBlade;

            return actionID;
        }
    }

    #endregion

    #region One-Button Mitigation

    internal class GNB_Mit_OneButton : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GNB_Mit_OneButton;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Camouflage)
                return actionID;

            if (IsEnabled(CustomComboPreset.GNB_Mit_Superbolide_Max) &&
                ActionReady(Superbolide) &&
                PlayerHealthPercentageHp() <= Config.GNB_Mit_Superbolide_Health &&
                ContentCheck.IsInConfiguredContent(
                    Config.GNB_Mit_Superbolide_Difficulty,
                    Config.GNB_Mit_Superbolide_DifficultyListSet
                ))
                return Superbolide;

            foreach(int priority in Config.GNB_Mit_Priorities.Items.OrderBy(x => x))
            {
                int index = Config.GNB_Mit_Priorities.IndexOf(priority);
                if (CheckMitigationConfigMeetsRequirements(index, out uint action))
                    return action;
            }

            return actionID;
        }
    }

    #endregion
}
