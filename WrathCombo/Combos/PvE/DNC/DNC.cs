﻿#region
using WrathCombo.CustomComboNS;

// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

#endregion

namespace WrathCombo.Combos.PvE;

internal partial class DNC : PhysicalRanged
{
    internal class DNC_ST_BasicCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_ST_BasicCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Fountain)
                return actionID;
            
            if (LevelChecked(Fountain) && ComboAction is Cascade &&
                ComboTimer > 0)
                return Fountain;

            return Cascade;

        }
    }
    internal class DNC_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_ST_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Cascade) return actionID;

            #region Variables

            var flow = HasStatusEffect(Buffs.SilkenFlow) ||
                       HasStatusEffect(Buffs.FlourishingFlow);
            var symmetry = HasStatusEffect(Buffs.SilkenSymmetry) ||
                           HasStatusEffect(Buffs.FlourishingSymmetry);
            var targetHpThresholdFeather = Config.DNC_ST_Adv_FeatherBurstPercent;
            var targetHpThresholdStandard = Config.DNC_ST_Adv_SSBurstPercent;
            var targetHpThresholdTechnical = Config.DNC_ST_Adv_TSBurstPercent;
            var tillanaDriftProtectionActive =
                Config.DNC_ST_ADV_TillanaUse ==
                (int)Config.TillanaDriftProtection.Favor;

            // Thresholds to wait for TS/SS to come off CD
            var longAlignmentThreshold = 0.6f;
            var shortAlignmentThreshold = 0.3f;
            if (Config.DNC_ST_ADV_AntiDrift == (int)Config.AntiDrift.TripleWeave ||
                Config.DNC_ST_ADV_AntiDrift == (int)Config.AntiDrift.Both)
            {
                longAlignmentThreshold = 0.3f;
                shortAlignmentThreshold = 0.1f;
            }

            var needToTech =
                IsEnabled(CustomComboPreset.DNC_ST_Adv_TS) &&
                Config.DNC_ST_ADV_TS_IncludeTS == (int)Config.IncludeStep.Yes &&
                GetCooldownRemainingTime(TechnicalStep) <
                longAlignmentThreshold && // Up or about to be (some anti-drift)
                !HasStatusEffect(Buffs.StandardStep) && // After Standard
                IsOnCooldown(StandardStep) &&
                GetTargetHPPercent() > targetHpThresholdTechnical && // HP% check
                LevelChecked(TechnicalStep);

            var needToStandardOrFinish =
                GetTargetHPPercent() > targetHpThresholdStandard && // HP% check
                LevelChecked(StandardStep);

            // More Threshold, but only for SS
            if (Config.DNC_ST_ADV_AntiDrift == (int)Config.AntiDrift.Hold ||
                Config.DNC_ST_ADV_AntiDrift == (int)Config.AntiDrift.Both)
            {
                longAlignmentThreshold = (float)GCD;
                shortAlignmentThreshold = (float)GCD;
            }

            var needToFinish =
                IsEnabled(CustomComboPreset.DNC_ST_Adv_FM) &&
                HasStatusEffect(Buffs.FinishingMoveReady) &&
                !HasStatusEffect(Buffs.LastDanceReady) &&
                ((GetCooldownRemainingTime(StandardStep) < longAlignmentThreshold &&
                  HasStatusEffect(Buffs.TechnicalFinish)) || // Aggressive anti-drift
                 (!HasStatusEffect(Buffs.TechnicalFinish) && // Anti-Drift outside of Tech
                  GetCooldownRemainingTime(StandardStep) <
                  shortAlignmentThreshold));

            var needToStandard =
                IsEnabled(CustomComboPreset.DNC_ST_Adv_SS) &&
                Config.DNC_ST_ADV_SS_IncludeSS == (int)Config.IncludeStep.Yes &&
                GetCooldownRemainingTime(StandardStep) <
                longAlignmentThreshold && // Up or about to be (some anti-drift)
                !HasStatusEffect(Buffs.FinishingMoveReady) &&
                (IsOffCooldown(Flourish) ||
                 GetCooldownRemainingTime(Flourish) > 5) &&
                !HasStatusEffect(Buffs.TechnicalFinish);

            #endregion

            #region Dance Partner

            // Dance Partner
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_Partner) && !InCombat() &&
                ActionReady(ClosedPosition) &&
                !HasStatusEffect(Buffs.ClosedPosition) &&
                (GetPartyMembers().Count > 1 || HasCompanionPresent()) &&
                !InAutoMode(true, false)) // Disabled in Auto-Rotation
                // todo: do not disable for auto-rotation, provide targeting
                return ClosedPosition;

            #endregion

            #region Opener

            // Opener
            if (IsEnabled(CustomComboPreset.DNC_ST_BalanceOpener) &&
                Opener().FullOpener(ref actionID))
                return actionID;

            #endregion

            #region Pre-pull

            if (!InCombat() && TargetIsHostile())
            {
                // ST Standard Step (Pre-pull)
                if (IsEnabled(CustomComboPreset.DNC_ST_Adv_SS) &&
                    IsEnabled(CustomComboPreset.DNC_ST_Adv_SS_Prepull) &&
                    Config.DNC_ST_ADV_SS_IncludeSS == (int)Config.IncludeStep.Yes &&
                    ActionReady(StandardStep) &&
                    !HasStatusEffect(Buffs.FinishingMoveReady) &&
                    !HasStatusEffect(Buffs.TechnicalFinish) &&
                    IsOffCooldown(TechnicalStep) &&
                    IsOffCooldown(StandardStep))
                    return StandardStep;

                // ST Standard Steps (Pre-pull)
                if ((IsEnabled(CustomComboPreset.DNC_ST_Adv_SS) &&
                     IsEnabled(CustomComboPreset.DNC_ST_Adv_SS_Prepull)) &&
                    HasStatusEffect(Buffs.StandardStep) &&
                    Gauge.CompletedSteps < 2)
                    return Gauge.NextStep;

                // ST Peloton
                if (IsEnabled(CustomComboPreset.DNC_ST_Adv_Peloton) &&
                    !HasStatusEffect(Buffs.Peloton, anyOwner: true) &&
                    GetStatusEffectRemainingTime(Buffs.StandardStep) > 5)
                    return Peloton;
            }

            #endregion

            #region Dance Fills

            // ST Standard (Dance) Steps & Fill
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_SS) &&
                HasStatusEffect(Buffs.StandardStep))
                return Gauge.CompletedSteps < 2
                    ? Gauge.NextStep
                    : FinishOrHold(StandardFinish2);

            // ST Technical (Dance) Steps & Fill
            if ((IsEnabled(CustomComboPreset.DNC_ST_Adv_TS)) &&
                HasStatusEffect(Buffs.TechnicalStep))
                return Gauge.CompletedSteps < 4
                    ? Gauge.NextStep
                    : FinishOrHold(TechnicalFinish4);

            #endregion

            #region Weaves

            // ST Devilment
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_Devilment) &&
                CanWeave() &&
                LevelChecked(Devilment) &&
                GetCooldownRemainingTime(Devilment) < 0.05 &&
                (HasStatusEffect(Buffs.TechnicalFinish) ||
                 WasLastAction(TechnicalFinish4) ||
                 !LevelChecked(TechnicalStep)))
                return Devilment;

            // ST Flourish
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_Flourish) &&
                CanWeave() &&
                ActionReady(Flourish) &&
                !WasLastWeaponskill(TechnicalFinish4) &&
                IsOnCooldown(Devilment) &&
                (GetCooldownRemainingTime(Devilment) > 50 ||
                 (HasStatusEffect(Buffs.Devilment) &&
                  GetStatusEffectRemainingTime(Buffs.Devilment) < 19)) &&
                !HasStatusEffect(Buffs.ThreeFoldFanDance) &&
                !HasStatusEffect(Buffs.FourFoldFanDance) &&
                !HasStatusEffect(Buffs.FlourishingSymmetry) &&
                !HasStatusEffect(Buffs.FlourishingFlow) &&
                !HasStatusEffect(Buffs.FinishingMoveReady) &&
                ((CombatEngageDuration().TotalSeconds < 20 &&
                  HasStatusEffect(Buffs.TechnicalFinish)) ||
                 CombatEngageDuration().TotalSeconds > 20))
                return Flourish;

            if ((Config.DNC_ST_ADV_AntiDrift == (int)Config.AntiDrift.TripleWeave ||
                 Config.DNC_ST_ADV_AntiDrift == (int)Config.AntiDrift.Both) &&
                (HasStatusEffect(Buffs.ThreeFoldFanDance) ||
                 HasStatusEffect(Buffs.FourFoldFanDance)) &&
                CombatEngageDuration().TotalSeconds > 20 &&
                HasStatusEffect(Buffs.TechnicalFinish) &&
                GetCooldownRemainingTime(Flourish) > 58)
            {
                if (HasStatusEffect(Buffs.ThreeFoldFanDance) &&
                    CanDelayedWeave())
                    return FanDance3;
                if (HasStatusEffect(Buffs.FourFoldFanDance))
                    return FanDance4;
            }

            // Variant Cure
            if (Variant.CanCure(CustomComboPreset.DNC_Variant_Cure, Config.DNCVariantCurePercent))
                return Variant.Cure;

            // ST Interrupt
            if (Role.CanHeadGraze(CustomComboPreset.DNC_ST_Adv_Interrupt, WeaveTypes.Weave) &&
                !HasStatusEffect(Buffs.TechnicalFinish))
                return Role.HeadGraze;

            // Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.DNC_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            if (CanWeave() && !WasLastWeaponskill(TechnicalFinish4))
            {
                // ST Fans
                if (IsEnabled(CustomComboPreset.DNC_ST_Adv_FanProccs))
                {
                    if (IsEnabled(CustomComboPreset.DNC_ST_Adv_FanProcc3) &&
                        HasStatusEffect(Buffs.ThreeFoldFanDance))
                        return FanDance3;

                    if (IsEnabled(CustomComboPreset.DNC_ST_Adv_FanProcc4) &&
                        HasStatusEffect(Buffs.FourFoldFanDance))
                        return FanDance4;
                }

                // ST Feathers
                if (IsEnabled(CustomComboPreset.DNC_ST_Adv_Feathers) &&
                    LevelChecked(FanDance1))
                {
                    // FD1 HP% Dump
                    if (GetTargetHPPercent() <= targetHpThresholdFeather &&
                        Gauge.Feathers > 0)
                        return FanDance1;

                    if (LevelChecked(TechnicalStep))
                    {
                        // Burst FD1
                        if (HasStatusEffect(Buffs.TechnicalFinish) &&
                            Gauge.Feathers > 0)
                            return FanDance1;

                        // FD1 Pooling
                        if (Gauge.Feathers > 3 &&
                            (HasStatusEffect(Buffs.SilkenSymmetry) ||
                             HasStatusEffect(Buffs.SilkenFlow))
                           )

                            return FanDance1;
                    }

                    // FD1 Non-pooling & under burst level
                    if (!LevelChecked(TechnicalStep) && Gauge.Feathers > 0)
                        return FanDance1;
                }

                // ST Panic Heals
                if (IsEnabled(CustomComboPreset.DNC_ST_Adv_PanicHeals))
                {
                    if (ActionReady(CuringWaltz) &&
                        PlayerHealthPercentageHp() <
                        Config.DNC_ST_Adv_PanicHealWaltzPercent)
                        return CuringWaltz;

                    if (Role.CanSecondWind(Config.DNC_ST_Adv_PanicHealWindPercent))
                        return Role.SecondWind;
                }

                // ST Improvisation
                if (IsEnabled(CustomComboPreset.DNC_ST_Adv_Improvisation) &&
                    ActionReady(Improvisation) &&
                    !HasStatusEffect(Buffs.TechnicalFinish) &&
                    InCombat() &&
                    EnemyIn8Yalms)
                    return Improvisation;
            }

            #endregion

            #region GCD

            // ST Technical Step
            if (needToTech && !HasStatusEffect(Buffs.FlourishingFinish))
                return TechnicalStep;

            // ST Last Dance
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_LD) && // Enabled
                HasStatusEffect(Buffs.LastDanceReady) && // Ready
                (HasStatusEffect(Buffs.TechnicalFinish) || // Has Tech
                 !(IsOnCooldown(TechnicalStep) && // Or can't hold it for tech
                   GetCooldownRemainingTime(TechnicalStep) < 20 &&
                   GetStatusEffectRemainingTime(Buffs.LastDanceReady) >
                   GetCooldownRemainingTime(TechnicalStep) + 4) ||
                 GetStatusEffectRemainingTime(Buffs.LastDanceReady) <
                 4)) // Or last second
                return LastDance;

            // ST Standard Step (Finishing Move)
            if (needToStandardOrFinish && needToFinish && EnemyIn15Yalms)
                return OriginalHook(FinishingMove);

            // ST Standard Step
            if (needToStandardOrFinish && needToStandard)
                return StandardStep;

            // Emergency Starfall usage
            if (HasStatusEffect(Buffs.FlourishingStarfall) &&
                GetStatusEffectRemainingTime(Buffs.FlourishingStarfall) < 4)
                return StarfallDance;

            // ST Dance of the Dawn
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_DawnDance) &&
                HasStatusEffect(Buffs.DanceOfTheDawnReady) &&
                LevelChecked(DanceOfTheDawn) &&
                (GetCooldownRemainingTime(TechnicalStep) > 5 ||
                 IsOffCooldown(TechnicalStep)) && // Tech is up
                (Gauge.Esprit >=
                 Config.DNC_ST_Adv_SaberThreshold || // >esprit threshold use
                 (HasStatusEffect(Buffs
                      .TechnicalFinish) && // will overcap with Tillana if not used
                  !tillanaDriftProtectionActive && Gauge.Esprit >= 50) ||
                 (GetStatusEffectRemainingTime(Buffs.DanceOfTheDawnReady) < 5 &&
                  Gauge.Esprit >= 50))) // emergency use
                return OriginalHook(DanceOfTheDawn);

            // ST Saber Dance (Emergency Use)
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_SaberDance) &&
                LevelChecked(SaberDance) &&
                (Gauge.Esprit >=
                 Config
                     .DNC_ST_Adv_SaberThreshold || // above esprit threshold use
                 (HasStatusEffect(Buffs
                      .TechnicalFinish) && // will overcap with Tillana if not used
                  !tillanaDriftProtectionActive && Gauge.Esprit >= 50)))
                return LevelChecked(DanceOfTheDawn) &&
                       HasStatusEffect(Buffs.DanceOfTheDawnReady)
                    ? OriginalHook(DanceOfTheDawn)
                    : SaberDance;

            if (HasStatusEffect(Buffs.FlourishingStarfall))
                return StarfallDance;

            // ST Tillana
            if (HasStatusEffect(Buffs.FlourishingFinish) &&
                IsEnabled(CustomComboPreset.DNC_ST_Adv_Tillana) &&
                EnemyIn15Yalms)
                return Tillana;

            // ST Saber Dance
            if (IsEnabled(CustomComboPreset.DNC_ST_Adv_SaberDance) &&
                LevelChecked(SaberDance) &&
                Gauge.Esprit >=
                Config.DNC_ST_Adv_SaberThreshold || // Above esprit threshold use
                (HasStatusEffect(Buffs.TechnicalFinish) &&
                 Gauge.Esprit >= 50) && // Burst
                (GetCooldownRemainingTime(TechnicalStep) > 5 ||
                 IsOffCooldown(TechnicalStep))) // Tech is up
                return SaberDance;

            // ST combos and burst attacks
            if (LevelChecked(Fountain) &&
                ComboAction is Cascade &&
                ComboTimer is < 2 and > 0)
                return Fountain;

            if (LevelChecked(Fountainfall) && flow)
                return Fountainfall;
            if (LevelChecked(ReverseCascade) && symmetry)
                return ReverseCascade;
            if (LevelChecked(Fountain) && ComboAction is Cascade &&
                ComboTimer > 0)
                return Fountain;

            #endregion

            return actionID;
        }
    }

    internal class DNC_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_ST_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Cascade) return actionID;

            #region Variables

            var flow = HasStatusEffect(Buffs.SilkenFlow) ||
                       HasStatusEffect(Buffs.FlourishingFlow);
            var symmetry = HasStatusEffect(Buffs.SilkenSymmetry) ||
                           HasStatusEffect(Buffs.FlourishingSymmetry);
            var targetHpThresholdFeather = 10;
            var targetHpThresholdStandard = 1;
            var targetHpThresholdTechnical = 1;

            // Thresholds to wait for TS/SS to come off CD
            var longAlignmentThreshold = 0.3f;
            var shortAlignmentThreshold = 0.1f;

            var needToTech =
                GetCooldownRemainingTime(TechnicalStep) <
                longAlignmentThreshold && // Up or about to be (some anti-drift)
                !HasStatusEffect(Buffs.StandardStep) && // After Standard
                IsOnCooldown(StandardStep) &&
                GetTargetHPPercent() > targetHpThresholdTechnical && // HP% check
                LevelChecked(TechnicalStep);

            var needToStandardOrFinish =
                GetTargetHPPercent() > targetHpThresholdStandard && // HP% check
                LevelChecked(StandardStep);

            var needToFinish =
                HasStatusEffect(Buffs.FinishingMoveReady) &&
                !HasStatusEffect(Buffs.LastDanceReady) &&
                ((GetCooldownRemainingTime(StandardStep) < longAlignmentThreshold &&
                  HasStatusEffect(Buffs.TechnicalFinish)) || // Aggressive anti-drift
                 (!HasStatusEffect(Buffs.TechnicalFinish) && // Anti-Drift outside of Tech
                  GetCooldownRemainingTime(StandardStep) <
                  shortAlignmentThreshold));

            var needToStandard =
                GetCooldownRemainingTime(StandardStep) <
                longAlignmentThreshold && // Up or about to be (some anti-drift)
                !HasStatusEffect(Buffs.FinishingMoveReady) &&
                (IsOffCooldown(Flourish) ||
                 GetCooldownRemainingTime(Flourish) > 5) &&
                !HasStatusEffect(Buffs.TechnicalFinish);

            #endregion

            #region Pre-pull

            if (!InCombat())
            {
                // Dance Partner
                if (ActionReady(ClosedPosition) &&
                    !HasStatusEffect(Buffs.ClosedPosition) &&
                    (GetPartyMembers().Count > 1 || HasCompanionPresent()) &&
                    !InAutoMode(true, true)) // Disabled in Auto-Rotation
                    // todo: do not disable for auto-rotation, provide targeting
                    return ClosedPosition;

                if (TargetIsHostile())
                {
                    // ST Standard Step (Pre-pull)
                    if (ActionReady(StandardStep) &&
                        !HasStatusEffect(Buffs.FinishingMoveReady) &&
                        !HasStatusEffect(Buffs.TechnicalFinish) &&
                        IsOffCooldown(TechnicalStep) &&
                        IsOffCooldown(StandardStep))
                        return StandardStep;

                    // ST Standard Steps (Pre-pull)
                    if (HasStatusEffect(Buffs.StandardStep) &&
                        Gauge.CompletedSteps < 2)
                        return Gauge.NextStep;
                }
            }

            #endregion

            #region Dance Fills

            // ST Standard (Dance) Steps & Fill
            if (HasStatusEffect(Buffs.StandardStep))
                return Gauge.CompletedSteps < 2
                    ? Gauge.NextStep
                    : StandardFinish2;

            // ST Technical (Dance) Steps & Fill
            if (HasStatusEffect(Buffs.TechnicalStep))
                return Gauge.CompletedSteps < 4
                    ? Gauge.NextStep
                    : TechnicalFinish4;

            #endregion

            #region Weaves

            // ST Devilment
            if (CanWeave() &&
                LevelChecked(Devilment) &&
                GetCooldownRemainingTime(Devilment) < 0.05 &&
                (HasStatusEffect(Buffs.TechnicalFinish) ||
                 WasLastAction(TechnicalFinish4) ||
                 !LevelChecked(TechnicalStep)))
                return Devilment;

            // ST Flourish
            if (CanWeave() &&
                ActionReady(Flourish) &&
                !WasLastWeaponskill(TechnicalFinish4) &&
                IsOnCooldown(Devilment) &&
                (GetCooldownRemainingTime(Devilment) > 50 ||
                 (HasStatusEffect(Buffs.Devilment) &&
                  GetStatusEffectRemainingTime(Buffs.Devilment) < 19)) &&
                !HasStatusEffect(Buffs.ThreeFoldFanDance) &&
                !HasStatusEffect(Buffs.FourFoldFanDance) &&
                !HasStatusEffect(Buffs.FlourishingSymmetry) &&
                !HasStatusEffect(Buffs.FlourishingFlow) &&
                !HasStatusEffect(Buffs.FinishingMoveReady) &&
                ((CombatEngageDuration().TotalSeconds < 20 &&
                  HasStatusEffect(Buffs.TechnicalFinish)) ||
                 CombatEngageDuration().TotalSeconds > 20))
                return Flourish;

            if ((HasStatusEffect(Buffs.ThreeFoldFanDance) ||
                 HasStatusEffect(Buffs.FourFoldFanDance)) &&
                CombatEngageDuration().TotalSeconds > 20 &&
                HasStatusEffect(Buffs.TechnicalFinish) &&
                GetCooldownRemainingTime(Flourish) > 58)
            {
                if (HasStatusEffect(Buffs.ThreeFoldFanDance) &&
                    CanDelayedWeave())
                    return FanDance3;
                if (HasStatusEffect(Buffs.FourFoldFanDance))
                    return FanDance4;
            }

            // Variant Cure
            if (Variant.CanCure(CustomComboPreset.DNC_Variant_Cure, 50))
                return Variant.Cure;

            // ST Interrupt
            if (Role.CanHeadGraze(CustomComboPreset.DNC_ST_SimpleMode, WeaveTypes.Weave) &&
                !HasStatusEffect(Buffs.TechnicalFinish))
                return Role.HeadGraze;

            // Variant Rampart
            if (Variant.CanRampart(CustomComboPreset.DNC_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            if (CanWeave() && !WasLastWeaponskill(TechnicalFinish4))
            {
                if (HasStatusEffect(Buffs.ThreeFoldFanDance))
                    return FanDance3;

                if (HasStatusEffect(Buffs.FourFoldFanDance))
                    return FanDance4;

                // ST Feathers & Fans
                if (LevelChecked(FanDance1))
                {
                    // FD1 HP% Dump
                    if (GetTargetHPPercent() <= targetHpThresholdFeather &&
                        Gauge.Feathers > 0)
                        return FanDance1;

                    if (LevelChecked(TechnicalStep))
                    {
                        // Burst FD1
                        if (HasStatusEffect(Buffs.TechnicalFinish) &&
                            Gauge.Feathers > 0)
                            return FanDance1;

                        // FD1 Pooling
                        if (Gauge.Feathers > 3 &&
                            (HasStatusEffect(Buffs.SilkenSymmetry) ||
                             HasStatusEffect(Buffs.SilkenFlow)))
                            return FanDance1;
                    }

                    // FD1 Non-pooling & under burst level
                    if (!LevelChecked(TechnicalStep) && Gauge.Feathers > 0)
                        return FanDance1;
                }

                // ST Panic Heals

                if (Role.CanSecondWind(40))
                    return Role.SecondWind;
            }

            #endregion

            #region GCD

            // ST Technical Step
            if (needToTech && !HasStatusEffect(Buffs.FlourishingFinish))
                return TechnicalStep;

            // ST Last Dance
            if (HasStatusEffect(Buffs.LastDanceReady) && // Ready
                (HasStatusEffect(Buffs.TechnicalFinish) || // Has Tech
                 !(IsOnCooldown(TechnicalStep) && // Or can't hold it for tech
                   GetCooldownRemainingTime(TechnicalStep) < 20 &&
                   GetStatusEffectRemainingTime(Buffs.LastDanceReady) >
                   GetCooldownRemainingTime(TechnicalStep) + 4) ||
                 GetStatusEffectRemainingTime(Buffs.LastDanceReady) <
                 4)) // Or last second
                return LastDance;

            // ST Standard Step (Finishing Move)
            if (needToStandardOrFinish && needToFinish && EnemyIn15Yalms)
                return OriginalHook(FinishingMove);

            // ST Standard Step
            if (needToStandardOrFinish && needToStandard)
                return StandardStep;

            // Emergency Starfall usage
            if (HasStatusEffect(Buffs.FlourishingStarfall) &&
                GetStatusEffectRemainingTime(Buffs.FlourishingStarfall) < 4)
                return StarfallDance;

            // ST Dance of the Dawn
            if (HasStatusEffect(Buffs.DanceOfTheDawnReady) &&
                LevelChecked(DanceOfTheDawn) &&
                (GetCooldownRemainingTime(TechnicalStep) > 5 ||
                 IsOffCooldown(TechnicalStep)) && // Tech is up
                (Gauge.Esprit >=
                 Config.DNC_ST_Adv_SaberThreshold || // >esprit threshold use
                 (HasStatusEffect(Buffs
                      .TechnicalFinish) && // will overcap with Tillana if not used
                  Gauge.Esprit >= 50) ||
                 (GetStatusEffectRemainingTime(Buffs.DanceOfTheDawnReady) < 5 &&
                  Gauge.Esprit >= 50))) // emergency use
                return OriginalHook(DanceOfTheDawn);

            // ST Saber Dance
            if (LevelChecked(SaberDance) &&
                Gauge.Esprit >= 50)
                return LevelChecked(DanceOfTheDawn) &&
                       HasStatusEffect(Buffs.DanceOfTheDawnReady)
                    ? OriginalHook(DanceOfTheDawn)
                    : SaberDance;

            if (HasStatusEffect(Buffs.FlourishingStarfall))
                return StarfallDance;

            // ST Tillana
            if (HasStatusEffect(Buffs.FlourishingFinish) &&
                EnemyIn15Yalms)
                return Tillana;

            // ST combos and burst attacks
            if (LevelChecked(Fountain) &&
                ComboAction is Cascade &&
                ComboTimer is < 2 and > 0)
                return Fountain;

            if (LevelChecked(Fountainfall) && flow)
                return Fountainfall;
            if (LevelChecked(ReverseCascade) && symmetry)
                return ReverseCascade;
            if (LevelChecked(Fountain) && ComboAction is Cascade &&
                ComboTimer > 0)
                return Fountain;

            #endregion

            return actionID;
        }
    }

    internal class DNC_AoE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Windmill) return actionID;

            #region Variables

            bool flow = HasStatusEffect(Buffs.SilkenFlow) ||
                        HasStatusEffect(Buffs.FlourishingFlow);
            bool symmetry = HasStatusEffect(Buffs.SilkenSymmetry) ||
                            HasStatusEffect(Buffs.FlourishingSymmetry);
            var targetHpThresholdStandard = Config.DNC_AoE_Adv_SSBurstPercent;
            var targetHpThresholdTechnical = Config.DNC_AoE_Adv_TSBurstPercent;

            var needToTech =
                IsEnabled(CustomComboPreset.DNC_AoE_Adv_TS) &&
                Config.DNC_AoE_Adv_TS_IncludeTS == (int)Config.IncludeStep.Yes &&
                ActionReady(TechnicalStep) && // Up
                !HasStatusEffect(Buffs.StandardStep) && // After Standard
                IsOnCooldown(StandardStep) &&
                GetTargetHPPercent() > targetHpThresholdTechnical && // HP% check
                LevelChecked(TechnicalStep);

            var needToStandardOrFinish =
                ActionReady(StandardStep) && // Up
                GetTargetHPPercent() > targetHpThresholdStandard && // HP% check
                (IsOffCooldown(
                     TechnicalStep) || // Checking burst is ready for standard
                 GetCooldownRemainingTime(TechnicalStep) > 5) && // Don't mangle
                LevelChecked(StandardStep);

            var needToFinish =
                IsEnabled(CustomComboPreset.DNC_AoE_Adv_FM) && // Enabled
                HasStatusEffect(Buffs.FinishingMoveReady) &&
                !HasStatusEffect(Buffs.LastDanceReady);

            var needToStandard =
                IsEnabled(CustomComboPreset.DNC_AoE_Adv_SS) && // Enabled
                Config.DNC_AoE_Adv_SS_IncludeSS == (int)Config.IncludeStep.Yes &&
                !HasStatusEffect(Buffs.FinishingMoveReady) &&
                (IsOffCooldown(Flourish) ||
                 GetCooldownRemainingTime(Flourish) > 5) &&
                !HasStatusEffect(Buffs.TechnicalFinish);

            #endregion

            #region Prepull

            // Dance Partner
            if (!InCombat() &&
                IsEnabled(CustomComboPreset.DNC_AoE_Adv_Partner) &&
                ActionReady(ClosedPosition) &&
                !HasStatusEffect(Buffs.ClosedPosition) &&
                (GetPartyMembers().Count > 1 || HasCompanionPresent()) &&
                !InAutoMode(false, false)) // Disabled in Auto-Rotation
                // todo: do not disable for auto-rotation, provide targeting
                return ClosedPosition;

            #endregion

            #region Dance Fills

            // AoE Standard (Dance) Steps & Fill
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_SS) &&
                HasStatusEffect(Buffs.StandardStep))
                return Gauge.CompletedSteps < 2
                    ? Gauge.NextStep
                    : FinishOrHold(StandardFinish2);

            // AoE Technical (Dance) Steps & Fill
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_TS) &&
                HasStatusEffect(Buffs.TechnicalStep))
                return Gauge.CompletedSteps < 4
                    ? Gauge.NextStep
                    : FinishOrHold(TechnicalFinish4);

            #endregion

            #region Weaves

            // AoE Devilment
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_Devilment) &&
                CanWeave() &&
                LevelChecked(Devilment) &&
                GetCooldownRemainingTime(Devilment) < 0.05 &&
                (HasStatusEffect(Buffs.TechnicalFinish) ||
                 WasLastAction(TechnicalFinish4) ||
                 !LevelChecked(TechnicalStep)))
                return Devilment;

            // AoE Flourish
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_Flourish) &&
                CanWeave() &&
                ActionReady(Flourish) &&
                !WasLastWeaponskill(TechnicalFinish4) &&
                IsOnCooldown(Devilment) &&
                (GetCooldownRemainingTime(Devilment) > 50 ||
                 (HasStatusEffect(Buffs.Devilment) &&
                  GetStatusEffectRemainingTime(Buffs.Devilment) < 19)) &&
                !HasStatusEffect(Buffs.ThreeFoldFanDance) &&
                !HasStatusEffect(Buffs.FourFoldFanDance) &&
                !HasStatusEffect(Buffs.FlourishingSymmetry) &&
                !HasStatusEffect(Buffs.FlourishingFlow) &&
                !HasStatusEffect(Buffs.FinishingMoveReady))
                return Flourish;

            if (Variant.CanCure(CustomComboPreset.DNC_Variant_Cure, Config.DNCVariantCurePercent))
                return Variant.Cure;

            // AoE Interrupt
            if (Role.CanHeadGraze(CustomComboPreset.DNC_AoE_Adv_Interrupt, WeaveTypes.Weave) &&
                !HasStatusEffect(Buffs.TechnicalFinish))
                return Role.HeadGraze;

            if (Variant.CanRampart(CustomComboPreset.DNC_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            if (CanWeave() && !WasLastWeaponskill(TechnicalFinish4))
            {
                // AoE Fan 3
                if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_FanProccs) &&
                    IsEnabled(CustomComboPreset.DNC_AoE_Adv_FanProcc3) &&
                    HasStatusEffect(Buffs.ThreeFoldFanDance))
                    return FanDance3;

                // AoE Feathers
                if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_Feathers) &&
                    LevelChecked(FanDance1))
                {
                    if (LevelChecked(FanDance2))
                    {
                        if (LevelChecked(TechnicalStep))
                        {
                            // Burst FD2
                            if (HasStatusEffect(Buffs.TechnicalFinish) &&
                                Gauge.Feathers > 0)
                                return FanDance2;

                            // FD2 Pooling
                            if (Gauge.Feathers > 3 &&
                                (HasStatusEffect(Buffs.SilkenSymmetry) ||
                                 HasStatusEffect(Buffs.SilkenFlow)))
                                return FanDance2;
                        }

                        // FD2 Non-pooling & under burst level
                        if (!LevelChecked(TechnicalStep) &&
                            Gauge.Feathers > 0)
                            return FanDance2;
                    }

                    // FD1 Replacement for Lv.30-49
                    if (!LevelChecked(FanDance2) &&
                        Gauge.Feathers > 0)
                        return FanDance1;
                }

                // AoE Fan 4
                if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_FanProccs) &&
                    IsEnabled(CustomComboPreset.DNC_AoE_Adv_FanProcc4) &&
                    HasStatusEffect(Buffs.FourFoldFanDance))
                    return FanDance4;

                // AoE Panic Heals
                if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_PanicHeals))
                {
                    if (ActionReady(CuringWaltz) &&
                        PlayerHealthPercentageHp() <
                        Config.DNC_AoE_Adv_PanicHealWaltzPercent)
                        return CuringWaltz;

                    if (Role.CanSecondWind(Config.DNC_AoE_Adv_PanicHealWindPercent))
                        return Role.SecondWind;
                }

                // AoE Improvisation
                if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_Improvisation) &&
                    ActionReady(Improvisation) &&
                    !HasStatusEffect(Buffs.TechnicalStep) &&
                    InCombat())
                    return Improvisation;
            }

            #endregion

            #region GCD

            // AoE Technical Step
            if (needToTech && !HasStatusEffect(Buffs.FlourishingFinish))
                return TechnicalStep;

            // AoE Last Dance
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_LD) && // Enabled
                HasStatusEffect(Buffs.LastDanceReady) && // Ready
                (HasStatusEffect(Buffs.TechnicalFinish) || // Has Tech
                 !(IsOnCooldown(TechnicalStep) && // Or can't hold it for tech
                   GetCooldownRemainingTime(TechnicalStep) < 20 &&
                   GetStatusEffectRemainingTime(Buffs.LastDanceReady) >
                   GetCooldownRemainingTime(TechnicalStep) + 4) ||
                 GetStatusEffectRemainingTime(Buffs.LastDanceReady) <
                 4)) // Or last second
                return LastDance;

            // AoE Standard Step (Finishing Move)
            if (needToStandardOrFinish && needToFinish)
                return OriginalHook(FinishingMove);

            // AoE Standard Step
            if (needToStandardOrFinish && needToStandard)
                return StandardStep;

            // Emergency Starfall usage
            if (HasStatusEffect(Buffs.FlourishingStarfall) &&
                GetStatusEffectRemainingTime(Buffs.FlourishingStarfall) < 4)
                return StarfallDance;

            // AoE Dance of the Dawn
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_DawnDance) &&
                HasStatusEffect(Buffs.DanceOfTheDawnReady) &&
                LevelChecked(DanceOfTheDawn) &&
                (GetCooldownRemainingTime(TechnicalStep) > 5 ||
                 IsOffCooldown(TechnicalStep)) && // Tech is up
                (Gauge.Esprit >=
                 Config
                     .DNC_AoE_Adv_SaberThreshold || // above esprit threshold use
                 (HasStatusEffect(Buffs.TechnicalFinish) &&
                  Gauge.Esprit >= 50) || // will overcap with Tillana if not used
                 (GetStatusEffectRemainingTime(Buffs.DanceOfTheDawnReady) < 5 &&
                  Gauge.Esprit >= 50))) // emergency use
                return OriginalHook(DanceOfTheDawn);

            // AoE Saber Dance (Emergency Use)
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_SaberDance) &&
                LevelChecked(SaberDance) &&
                (Gauge.Esprit >=
                 Config
                     .DNC_AoE_Adv_SaberThreshold || // above esprit threshold use
                 (HasStatusEffect(Buffs.TechnicalFinish) &&
                  Gauge.Esprit >=
                  50)) && // will overcap with Tillana if not used
                ActionReady(SaberDance))
                return SaberDance;

            if (HasStatusEffect(Buffs.FlourishingStarfall))
                return StarfallDance;

            // AoE Tillana
            if (HasStatusEffect(Buffs.FlourishingFinish) &&
                IsEnabled(CustomComboPreset.DNC_AoE_Adv_Tillana))
                return Tillana;

            // AoE Saber Dance
            if (IsEnabled(CustomComboPreset.DNC_AoE_Adv_SaberDance) &&
                LevelChecked(SaberDance) &&
                Gauge.Esprit >=
                Config.DNC_ST_Adv_SaberThreshold || // Above esprit threshold use
                (HasStatusEffect(Buffs.TechnicalFinish) &&
                 Gauge.Esprit >= 50) && // Burst
                (GetCooldownRemainingTime(TechnicalStep) > 5 ||
                 IsOffCooldown(TechnicalStep))) // Tech is up
                return SaberDance;

            // AoE combos and burst attacks
            if (LevelChecked(Bladeshower) &&
                ComboAction is Windmill &&
                ComboTimer is < 2 and > 0)
                return Bladeshower;

            if (LevelChecked(Bloodshower) && flow)
                return Bloodshower;
            if (LevelChecked(RisingWindmill) && symmetry)
                return RisingWindmill;
            if (LevelChecked(Bladeshower) && ComboAction is Windmill &&
                ComboTimer > 0)
                return Bladeshower;

            #endregion

            return actionID;
        }
    }

    internal class DNC_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_AoE_SimpleMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Windmill) return actionID;

            #region Variables

            bool flow = HasStatusEffect(Buffs.SilkenFlow) ||
                        HasStatusEffect(Buffs.FlourishingFlow);
            bool symmetry = HasStatusEffect(Buffs.SilkenSymmetry) ||
                            HasStatusEffect(Buffs.FlourishingSymmetry);
            var targetHpThresholdStandard = 25;
            var targetHpThresholdTechnical = 25;

            var needToTech =
                ActionReady(TechnicalStep) && // Up
                !HasStatusEffect(Buffs.StandardStep) && // After Standard
                IsOnCooldown(StandardStep) &&
                GetTargetHPPercent() > targetHpThresholdTechnical && // HP% check
                LevelChecked(TechnicalStep);

            var needToStandardOrFinish =
                ActionReady(StandardStep) && // Up
                GetTargetHPPercent() > targetHpThresholdStandard && // HP% check
                (IsOffCooldown(
                     TechnicalStep) || // Checking burst is ready for standard
                 GetCooldownRemainingTime(TechnicalStep) > 5) && // Don't mangle
                LevelChecked(StandardStep);

            var needToFinish =
                HasStatusEffect(Buffs.FinishingMoveReady) &&
                !HasStatusEffect(Buffs.LastDanceReady);

            var needToStandard =
                !HasStatusEffect(Buffs.FinishingMoveReady) &&
                (IsOffCooldown(Flourish) ||
                 GetCooldownRemainingTime(Flourish) > 5) &&
                !HasStatusEffect(Buffs.TechnicalFinish);

            #endregion

            #region Prepull

            // Dance Partner
            if (!InCombat() &&
                ActionReady(ClosedPosition) &&
                !HasStatusEffect(Buffs.ClosedPosition) &&
                (GetPartyMembers().Count > 1 || HasCompanionPresent()) &&
                !InAutoMode(false, true)) // Disabled in Auto-Rotation
                // todo: do not disable for auto-rotation, provide targeting
                return ClosedPosition;

            #endregion

            #region Dance Fills

            // AoE Standard (Dance) Steps & Fill
            if (HasStatusEffect(Buffs.StandardStep))
                return Gauge.CompletedSteps < 2
                    ? Gauge.NextStep
                    : StandardFinish2;

            // AoE Technical (Dance) Steps & Fill
            if (HasStatusEffect(Buffs.TechnicalStep))
                return Gauge.CompletedSteps < 4
                    ? Gauge.NextStep
                    : TechnicalFinish4;

            #endregion

            #region Weaves

            // AoE Devilment
            if (CanWeave() &&
                LevelChecked(Devilment) &&
                GetCooldownRemainingTime(Devilment) < 0.05 &&
                (HasStatusEffect(Buffs.TechnicalFinish) ||
                 WasLastAction(TechnicalFinish4) ||
                 !LevelChecked(TechnicalStep)))
                return Devilment;

            // AoE Flourish
            if (CanWeave() &&
                ActionReady(Flourish) &&
                !WasLastWeaponskill(TechnicalFinish4) &&
                IsOnCooldown(Devilment) &&
                (GetCooldownRemainingTime(Devilment) > 50 ||
                 (HasStatusEffect(Buffs.Devilment) &&
                  GetStatusEffectRemainingTime(Buffs.Devilment) < 19)) &&
                !HasStatusEffect(Buffs.ThreeFoldFanDance) &&
                !HasStatusEffect(Buffs.FourFoldFanDance) &&
                !HasStatusEffect(Buffs.FlourishingSymmetry) &&
                !HasStatusEffect(Buffs.FlourishingFlow) &&
                !HasStatusEffect(Buffs.FinishingMoveReady))
                return Flourish;

            if (Variant.CanCure(CustomComboPreset.DNC_Variant_Cure, 50))
                return Variant.Cure;

            // AoE Interrupt
            if (Role.CanHeadGraze(CustomComboPreset.DNC_AoE_SimpleMode, WeaveTypes.Weave)&&
                !HasStatusEffect(Buffs.TechnicalFinish))
                return Role.HeadGraze;

            if (Variant.CanRampart(CustomComboPreset.DNC_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            if (CanWeave() && !WasLastWeaponskill(TechnicalFinish4))
            {
                // AoE Feathers & Fans
                if (LevelChecked(FanDance1))
                {
                    // FD3
                    if (HasStatusEffect(Buffs.ThreeFoldFanDance))
                        return FanDance3;

                    if (LevelChecked(FanDance2))
                    {
                        if (LevelChecked(TechnicalStep))
                        {
                            // Burst FD2
                            if (HasStatusEffect(Buffs.TechnicalFinish) &&
                                Gauge.Feathers > 0)
                                return FanDance2;

                            // FD2 Pooling
                            if (Gauge.Feathers > 3 &&
                                (HasStatusEffect(Buffs.SilkenSymmetry) ||
                                 HasStatusEffect(Buffs.SilkenFlow)))
                                return FanDance2;
                        }

                        // FD2 Non-pooling & under burst level
                        if (!LevelChecked(TechnicalStep) &&
                            Gauge.Feathers > 0)
                            return FanDance2;
                    }

                    // FD1 Replacement for Lv.30-49
                    if (!LevelChecked(FanDance2) &&
                        Gauge.Feathers > 0)
                        return FanDance1;
                }

                if (HasStatusEffect(Buffs.FourFoldFanDance))
                    return FanDance4;

                // AoE Panic Heals
                if (Role.CanSecondWind(40))
                    return Role.SecondWind;
            }

            #endregion

            #region GCD

            // AoE Technical Step
            if (needToTech && !HasStatusEffect(Buffs.FlourishingFinish))
                return TechnicalStep;

            // AoE Last Dance
            if (HasStatusEffect(Buffs.LastDanceReady) && // Ready
                (HasStatusEffect(Buffs.TechnicalFinish) || // Has Tech
                 !(IsOnCooldown(TechnicalStep) && // Or can't hold it for tech
                   GetCooldownRemainingTime(TechnicalStep) < 20 &&
                   GetStatusEffectRemainingTime(Buffs.LastDanceReady) >
                   GetCooldownRemainingTime(TechnicalStep) + 4) ||
                 GetStatusEffectRemainingTime(Buffs.LastDanceReady) <
                 4)) // Or last second
                return LastDance;

            // AoE Standard Step (Finishing Move)
            if (needToStandardOrFinish && needToFinish)
                return OriginalHook(FinishingMove);

            // AoE Standard Step
            if (needToStandardOrFinish && needToStandard)
                return StandardStep;

            // Emergency Starfall usage
            if (HasStatusEffect(Buffs.FlourishingStarfall) &&
                GetStatusEffectRemainingTime(Buffs.FlourishingStarfall) < 4)
                return StarfallDance;

            // AoE Dance of the Dawn
            if (HasStatusEffect(Buffs.DanceOfTheDawnReady) &&
                LevelChecked(DanceOfTheDawn) &&
                (GetCooldownRemainingTime(TechnicalStep) > 5 ||
                 IsOffCooldown(TechnicalStep)) && // Tech is up
                (Gauge.Esprit >= 50))
                return OriginalHook(DanceOfTheDawn);

            // AoE Saber Dance
            if (ActionReady(SaberDance) &&
                Gauge.Esprit >= 50)
                return SaberDance;

            if (HasStatusEffect(Buffs.FlourishingStarfall))
                return StarfallDance;

            // AoE Tillana
            if (HasStatusEffect(Buffs.FlourishingFinish))
                return Tillana;

            // AoE combos and burst attacks
            if (LevelChecked(Bladeshower) &&
                ComboAction is Windmill &&
                ComboTimer is < 2 and > 0)
                return Bladeshower;

            if (LevelChecked(Bloodshower) && flow)
                return Bloodshower;
            if (LevelChecked(RisingWindmill) && symmetry)
                return RisingWindmill;
            if (LevelChecked(Bladeshower) && ComboAction is Windmill &&
                ComboTimer > 0)
                return Bladeshower;

            #endregion

            return actionID;
        }
    }

    #region MultiButton Combos

    internal class DNC_ST_MultiButton : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_ST_MultiButton;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Cascade) return actionID;

            #region Types

            bool flow = HasStatusEffect(Buffs.SilkenFlow) ||
                        HasStatusEffect(Buffs.FlourishingFlow);
            bool symmetry = HasStatusEffect(Buffs.SilkenSymmetry) ||
                            HasStatusEffect(Buffs.FlourishingSymmetry);

            #endregion

            // ST Esprit overcap protection
            if (IsEnabled(CustomComboPreset.DNC_ST_EspritOvercap) &&
                LevelChecked(DanceOfTheDawn) &&
                HasStatusEffect(Buffs.DanceOfTheDawnReady) &&
                Gauge.Esprit >= Config.DNCEspritThreshold_ST)
                return OriginalHook(DanceOfTheDawn);
            if (IsEnabled(CustomComboPreset.DNC_ST_EspritOvercap) &&
                LevelChecked(SaberDance) &&
                Gauge.Esprit >= Config.DNCEspritThreshold_ST)
                return SaberDance;

            if (CanWeave())
            {
                // ST Fan Dance overcap protection
                if (IsEnabled(CustomComboPreset.DNC_ST_FanDanceOvercap) &&
                    LevelChecked(FanDance1) && Gauge.Feathers is 4 &&
                    (HasStatusEffect(Buffs.SilkenSymmetry) ||
                     HasStatusEffect(Buffs.SilkenFlow)))
                    return FanDance1;

                // ST Fan Dance 3/4 on combo
                if (IsEnabled(CustomComboPreset.DNC_ST_FanDance34))
                {
                    if (HasStatusEffect(Buffs.ThreeFoldFanDance))
                        return FanDance3;
                    if (HasStatusEffect(Buffs.FourFoldFanDance))
                        return FanDance4;
                }
            }

            // ST base combos
            if (LevelChecked(Fountainfall) && flow)
                return Fountainfall;
            if (LevelChecked(ReverseCascade) && symmetry)
                return ReverseCascade;
            if (LevelChecked(Fountain) && ComboAction is Cascade)
                return Fountain;

            return actionID;
        }
    }

    internal class DNC_AoE_MultiButton : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_AoE_MultiButton;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Windmill) return actionID;

            #region Types

            bool flow = HasStatusEffect(Buffs.SilkenFlow) ||
                        HasStatusEffect(Buffs.FlourishingFlow);
            bool symmetry = HasStatusEffect(Buffs.SilkenSymmetry) ||
                            HasStatusEffect(Buffs.FlourishingSymmetry);

            #endregion

            // AoE Esprit overcap protection
            if (IsEnabled(CustomComboPreset.DNC_AoE_EspritOvercap) &&
                LevelChecked(DanceOfTheDawn) &&
                HasStatusEffect(Buffs.DanceOfTheDawnReady) &&
                Gauge.Esprit >= Config.DNCEspritThreshold_ST)
                return OriginalHook(DanceOfTheDawn);
            if (IsEnabled(CustomComboPreset.DNC_AoE_EspritOvercap) &&
                LevelChecked(SaberDance) &&
                Gauge.Esprit >= Config.DNCEspritThreshold_AoE)
                return SaberDance;

            if (CanWeave())
            {
                // AoE Fan Dance overcap protection
                if (IsEnabled(CustomComboPreset.DNC_AoE_FanDanceOvercap) &&
                    LevelChecked(FanDance2) && Gauge.Feathers is 4 &&
                    (HasStatusEffect(Buffs.SilkenSymmetry) ||
                     HasStatusEffect(Buffs.SilkenFlow)))
                    return FanDance2;

                // AoE Fan Dance 3/4 on combo
                if (IsEnabled(CustomComboPreset.DNC_AoE_FanDance34))
                {
                    if (HasStatusEffect(Buffs.ThreeFoldFanDance))
                        return FanDance3;
                    if (HasStatusEffect(Buffs.FourFoldFanDance))
                        return FanDance4;
                }
            }

            // AoE base combos
            if (LevelChecked(Bloodshower) && flow)
                return Bloodshower;
            if (LevelChecked(RisingWindmill) && symmetry)
                return RisingWindmill;
            if (LevelChecked(Bladeshower) && ComboAction is Windmill)
                return Bladeshower;

            return actionID;
        }
    }

    #region Smaller Features

    #region Dance Partner Features

    /*internal class DNC_DesirablePartner : CustomCombo
    {
        private static DateTime _lastPartnerCheckTime = DateTime.MinValue;

        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_DesirablePartner;

        private static bool CurrentPartnerNonOptimal =>
            DesirableDancePartner is not null &&
            DesirableDancePartner.GameObjectId != CurrentDancePartner;

        private static IGameObject? DesirableDancePartner
        {
            get
            {
                if ((DateTime.Now - _lastPartnerCheckTime).TotalSeconds <= 3)
                    return field;

                _lastPartnerCheckTime = DateTime.Now;
                field = TryGetDancePartner(out var partner, true)
                    ? partner
                    : null;

                return field;
            }
        }

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (ClosedPosition or Ending)) return actionID;

            var currentTarget = LocalPlayer.TargetObject;
            var noCurrentPartner = !HasStatusEffect(Buffs.ClosedPosition);

            if (noCurrentPartner || CurrentPartnerNonOptimal)
                if (DesirableDancePartner.GameObjectId == currentTarget.GameObjectId)
                    return Ending;
                else
                {
                    SetTarget(DesirableDancePartner);
                    TM.DelayNext(250);
                    TM.Enqueue(() => SetTarget(currentTarget));
                    return ClosedPosition;
                }

            return actionID;
        }
    }*/

    #endregion

    #region Dance Features

    internal class DNC_StandardStepCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_StandardStepCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not StandardStep) return actionID;

            // Standard Step
            if (actionID is StandardStep && Gauge.IsDancing &&
                HasStatusEffect(Buffs.StandardStep))
                return Gauge.CompletedSteps < 2
                    ? Gauge.NextStep
                    : FinishOrHold(StandardFinish2);

            return actionID;
        }
    }

    internal class DNC_TechnicalStepCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_TechnicalStepCombo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not TechnicalStep) return actionID;

            // Technical Step
            if (actionID is TechnicalStep && Gauge.IsDancing &&
                HasStatusEffect(Buffs.TechnicalStep))
                return Gauge.CompletedSteps < 4
                    ? Gauge.NextStep
                    : FinishOrHold(TechnicalFinish4);

            return actionID;
        }
    }

    internal class DNC_DanceComboReplacer : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_CustomDanceSteps;

        protected override uint Invoke(uint actionID)
        {
            if (!Gauge.IsDancing) return actionID;

            if (GetCustomDanceStep(actionID, out var danceStep))
                return danceStep;

            return actionID;
        }
    }

    #endregion

    #region Fan Features

    internal class DNC_FlourishingFanDances : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_FlourishingFanDances;

        protected override uint Invoke(uint actionID)
        {
            // Fan Dance 3 & 4 on Flourish
            if (actionID is not Flourish || !CanWeave()) return actionID;

            if (WantsCustomStepsOnSmallerFeatures)
                if (GetCustomDanceStep(actionID, out var danceStep))
                    return danceStep;

            if (IsEnabled(CustomComboPreset.DNC_Flourishing_FD3) &&
                HasStatusEffect(Buffs.ThreeFoldFanDance))
                return FanDance3;

            if (HasStatusEffect(Buffs.FourFoldFanDance))
                return FanDance4;

            return actionID;
        }
    }

    internal class DNC_FanDanceCombos : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_FanDanceCombos;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (FanDance1 or FanDance2)) return actionID;

            if (WantsCustomStepsOnSmallerFeatures)
                if (GetCustomDanceStep(actionID, out var danceStep))
                    return danceStep;

            return actionID switch
            {
                // FD 1 --> 3, FD 1 --> 4
                FanDance1 when
                    IsEnabled(CustomComboPreset.DNC_FanDance_1to3_Combo) &&
                    HasStatusEffect(Buffs.ThreeFoldFanDance) => FanDance3,
                FanDance1 when
                    IsEnabled(CustomComboPreset.DNC_FanDance_1to4_Combo) &&
                    HasStatusEffect(Buffs.FourFoldFanDance) => FanDance4,
                // FD 2 --> 3, FD 2 --> 4
                FanDance2 when
                    IsEnabled(CustomComboPreset.DNC_FanDance_2to3_Combo) &&
                    HasStatusEffect(Buffs.ThreeFoldFanDance) => FanDance3,
                FanDance2 when
                    IsEnabled(CustomComboPreset.DNC_FanDance_2to4_Combo) &&
                    HasStatusEffect(Buffs.FourFoldFanDance) => FanDance4,
                _ => actionID
            };
        }
    }

    #endregion

    internal class DNC_Starfall_Devilment : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_Starfall_Devilment;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Devilment) return actionID;

            if (WantsCustomStepsOnSmallerFeatures)
                if (GetCustomDanceStep(actionID, out var danceStep))
                    return danceStep;

            if (HasStatusEffect(Buffs.FlourishingStarfall))
                return StarfallDance;

            return actionID;
        }
    }

    internal class DNC_StandardStep_LastDance : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_StandardStep_LastDance;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (StandardStep or FinishingMove)) return actionID;

            if (WantsCustomStepsOnSmallerFeatures)
                if (GetCustomDanceStep(actionID, out var danceStep))
                    return danceStep;

            if (HasStatusEffect(Buffs.LastDanceReady))
                return LastDance;

            return actionID;
        }
    }

    internal class DNC_TechnicalStep_Devilment : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_TechnicalStep_Devilment;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not TechnicalStep) return actionID;

            if (WantsCustomStepsOnSmallerFeatures)
                if (GetCustomDanceStep(actionID, out var danceStep))
                    return danceStep;

            if (WasLastWeaponskill(TechnicalFinish4) &&
                HasStatusEffect(Buffs.TechnicalFinish))
                return Devilment;

            return actionID;
        }
    }

    internal class DNC_Procc_Bladeshower : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_Procc_Bladeshower;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Bladeshower) return actionID;

            if (WantsCustomStepsOnSmallerFeatures)
                if (GetCustomDanceStep(actionID, out var danceStep))
                    return danceStep;

            if (HasStatusEffect(Buffs.FlourishingFlow) ||
                HasStatusEffect(Buffs.SilkenFlow))
                return Bloodshower;

            return actionID;
        }
    }

    internal class DNC_Procc_Windmill : CustomCombo
    {
        protected internal override CustomComboPreset Preset =>
            CustomComboPreset.DNC_Procc_Windmill;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Windmill) return actionID;

            if (WantsCustomStepsOnSmallerFeatures)
                if (GetCustomDanceStep(actionID, out var danceStep))
                    return danceStep;

            if (HasStatusEffect(Buffs.FlourishingSymmetry) ||
                HasStatusEffect(Buffs.SilkenSymmetry))
                return RisingWindmill;

            return actionID;
        }
    }

    #endregion

    #endregion
}
