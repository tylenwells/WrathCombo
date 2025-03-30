using Dalamud.Game.ClientState.JobGauge.Enums;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
namespace WrathCombo.Combos.PvE;

internal partial class BRD : PhysRangedJob
{
    #region Smaller features

    internal class BRD_StraightShotUpgrade : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_StraightShotUpgrade;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HeavyShot or BurstShot))
                return actionID;

            if (IsEnabled(CustomComboPreset.BRD_DoTMaintainance) &&
                InCombat())
            {
                if (Purple is not null && PurpleRemaining < 4)
                    return CanIronJaws
                        ? IronJaws
                        : VenomousBite;

                if (Blue is not null && BlueRemaining < 4)
                    return CanIronJaws
                        ? IronJaws
                        : Windbite;
            }

            if (IsEnabled(CustomComboPreset.BRD_ApexST))
            {
                if (gauge.SoulVoice == 100)
                    return ApexArrow;

                if (HasEffect(Buffs.BlastArrowReady))
                    return BlastArrow;
            }

            if (HasEffect(Buffs.HawksEye) || HasEffect(Buffs.Barrage))
                return OriginalHook(StraightShot);

            return actionID;
        }
    }

    internal class BRD_IronJaws : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_IronJaws;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not IronJaws)
                return actionID;

            // Before Iron Jaws: Alternate between DoTs
            if (!LevelChecked(IronJaws))
                return LevelChecked(Windbite) && BlueRemaining <= PurpleRemaining
                    ? Windbite
                    : VenomousBite;

            // At least Lv56 (Iron Jaws) from here on...

            // DoT application takes priority, as Iron Jaws always cuts ticks
            if (Blue is null && LevelChecked(Windbite))
                return OriginalHook(Windbite);

            if (Purple is null && LevelChecked(VenomousBite))
                return OriginalHook(VenomousBite);

            // DoT refresh over Apex Option
            if (PurpleRemaining < 4 || BlueRemaining < 4)
                return IronJaws;

            // Apex Option
            if (IsEnabled(CustomComboPreset.BRD_IronJawsApex))
            {
                if (LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady))
                    return BlastArrow;

                if (gauge.SoulVoice == 100)
                    return ApexArrow;
            }
            return actionID;
        }
    }

    internal class BRD_IronJaws_Alternate : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_IronJaws_Alternate;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not IronJaws)
                return actionID;

            // Iron Jaws only if it is applicable
            if (LevelChecked(IronJaws) && (
                Purple is not null && PurpleRemaining < 4 ||
                Blue is not null && BlueRemaining < 4))
                return IronJaws;

            // Otherwise alternate between DoTs as needed
            return LevelChecked(Windbite) && BlueRemaining <= PurpleRemaining ?
                OriginalHook(Windbite) :
                OriginalHook(VenomousBite);
        }
    }

    internal class BRD_AoE_oGCD : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_AoE_oGCD;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not RainOfDeath)
                return actionID;

            if (IsEnabled(CustomComboPreset.BRD_AoE_oGCD_Songs) && (gauge.SongTimer < 1 || SongArmy))
            {
                if (ActionReady(WanderersMinuet))
                    return WanderersMinuet;

                if (ActionReady(MagesBallad))
                    return MagesBallad;

                if (ActionReady(ArmysPaeon))
                    return ArmysPaeon;
            }

            if (SongWanderer && gauge.Repertoire == 3)
                return OriginalHook(PitchPerfect);

            if (ActionReady(EmpyrealArrow))
                return EmpyrealArrow;

            if (ActionReady(RainOfDeath))
                return RainOfDeath;

            if (ActionReady(Sidewinder))
                return Sidewinder;

            return actionID;
        }
    }

    internal class BRD_ST_oGCD : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_ST_oGCD;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Bloodletter or HeartbreakShot))
                return actionID;

            if (IsEnabled(CustomComboPreset.BRD_ST_oGCD_Songs) && (gauge.SongTimer < 1 || SongArmy))
            {
                if (ActionReady(WanderersMinuet))
                    return WanderersMinuet;

                if (ActionReady(MagesBallad))
                    return MagesBallad;

                if (ActionReady(ArmysPaeon))
                    return ArmysPaeon;
            }

            if (SongWanderer && gauge.Repertoire == 3)
                return OriginalHook(PitchPerfect);

            if (ActionReady(EmpyrealArrow))
                return EmpyrealArrow;

            if (ActionReady(Sidewinder))
                return Sidewinder;

            if (ActionReady(Bloodletter))
                return OriginalHook(Bloodletter);

            return actionID;
        }
    }

    internal class BRD_AoE_Combo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_AoE_Combo;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (QuickNock or Ladonsbite))
                return actionID;

            if (IsEnabled(CustomComboPreset.BRD_Apex))
            {
                if (gauge.SoulVoice == 100)
                    return ApexArrow;

                if (HasEffect(Buffs.BlastArrowReady))
                    return BlastArrow;
            }

            if (IsEnabled(CustomComboPreset.BRD_AoE_Combo) && ActionReady(WideVolley) && HasEffect(Buffs.HawksEye))
                return OriginalHook(WideVolley);

            return actionID;
        }
    }

    internal class BRD_Buffs : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_Buffs;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not Barrage)
                return actionID;

            if (ActionReady(RagingStrikes))
                return RagingStrikes;

            if (ActionReady(BattleVoice))
                return BattleVoice;

            if (ActionReady(RadiantFinale))
                return RadiantFinale;

            return actionID;
        }
    }

    internal class BRD_OneButtonSongs : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_OneButtonSongs;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not WanderersMinuet)
                return actionID;

            if (ActionReady(WanderersMinuet) || gauge.Song == Song.Wanderer && SongTimerInSeconds > 11)
                return WanderersMinuet;

            if (ActionReady(MagesBallad) || gauge.Song == Song.Mage && SongTimerInSeconds > 2)
                return MagesBallad;

            if (ActionReady(ArmysPaeon) || gauge.Song == Song.Army && SongTimerInSeconds > 2)
                return ArmysPaeon;

            return actionID;
        }
    }

    #endregion

    #region Advanced Modes

    internal class BRD_AoE_AdvMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_AoE_AdvMode;

        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ladonsbite or QuickNock))
                return actionID;

            #region Variables

            int targetHPThreshold = Config.BRD_AoENoWasteHPPercentage;
            bool isEnemyHealthHigh = !IsEnabled(CustomComboPreset.BRD_AoE_Adv_NoWaste) || GetTargetHPPercent() > targetHPThreshold;
            bool ragingEnabled = IsEnabled(CustomComboPreset.BRD_AoE_Adv_Buffs_Raging);
            bool battleVoiceEnabled = IsEnabled(CustomComboPreset.BRD_AoE_Adv_Buffs_Battlevoice);
            bool barrageEnabled = IsEnabled(CustomComboPreset.BRD_AoE_Adv_Buffs_Barrage);
            bool radiantEnabled = IsEnabled(CustomComboPreset.BRD_AoE_Adv_Buffs_RadiantFinale);

            #endregion

            #region Variants

            if (Variant.CanCure(CustomComboPreset.BRD_Variant_Cure, Config.BRD_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BRD_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            #endregion

            #region Songs

            if (IsEnabled(CustomComboPreset.BRD_AoE_Adv_Songs))
            {
                // Limit optimisation to when you are high enough level to benefit from it.
                if (LevelChecked(WanderersMinuet))
                {
                    if (CanBardWeave || !BardHasTarget)
                    {
                        if (SongNone && InCombat())
                        {
                            // Logic to determine first song
                            if (ActionReady(WanderersMinuet) && !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)))
                                return WanderersMinuet;

                            if (ActionReady(MagesBallad) && !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon)))
                                return MagesBallad;

                            if (ActionReady(ArmysPaeon) && !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet)))
                                return ArmysPaeon;
                        }

                        if (SongWanderer)
                        {
                            if (SongTimerInSeconds <= 3 && gauge.Repertoire > 0 && BardHasTarget) // Spend any repertoire before switching to next song
                                return OriginalHook(PitchPerfect);

                            if (SongTimerInSeconds <= 3 && ActionReady(MagesBallad)) // Move to Mage's Ballad if <= 3 seconds left on song
                                return MagesBallad;
                        }

                        // Move to Army's Paeon if < 3 seconds left on song
                        // Special case for Empyreal Arrow: it must be cast before you change to it to avoid drift!
                        if (SongMage && SongTimerInSeconds <= 3 && ActionReady(ArmysPaeon))
                        {
                            return ActionReady(EmpyrealArrow) && BardHasTarget
                                ? EmpyrealArrow
                                : ArmysPaeon;
                        }
                    }

                    // Move to Wanderer's Minuet if <= 12 seconds left on song or WM off CD and have 4 repertoires of AP
                    if (SongArmy && (CanWeaveDelayed || !BardHasTarget) &&
                        (SongTimerInSeconds <= 12 || ActionReady(WanderersMinuet) && gauge.Repertoire == 4))
                        return WanderersMinuet;
                }

                else if (SongTimerInSeconds <= 3 && CanWeaveDelayed)
                {
                    if (ActionReady(MagesBallad))
                        return MagesBallad;

                    if (ActionReady(ArmysPaeon))
                        return ArmysPaeon;
                }
            }

            #endregion

            #region Buffs

            if (IsEnabled(CustomComboPreset.BRD_AoE_Adv_Buffs) && (!SongNone || !LevelChecked(MagesBallad)) && isEnemyHealthHigh)
            {
                // Radiant First with late weave for tighter grouping
                if (radiantEnabled && CanWeaveDelayed && ActionReady(RadiantFinale) && (RagingCD < 2.3 || !ragingEnabled) &&
                    !HasEffect(Buffs.RadiantEncoreReady))
                    return RadiantFinale;

                // BV normal weave into the raging weave
                if (battleVoiceEnabled && CanBardWeave && ActionReady(BattleVoice) && (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale) || !radiantEnabled))
                    return BattleVoice;

                // Late weave Raging last, must have battle voice buff OR not be high enough level for Battlecoice
                if (ragingEnabled && CanBardWeave && ActionReady(RagingStrikes) && (JustUsed(BattleVoice) || !LevelChecked(BattleVoice) || HasEffect(Buffs.BattleVoice) || !battleVoiceEnabled))
                    return RagingStrikes;

                // Barrage Logic to check for raging for low level reasons and it doesn't really need to check for the other buffs
                if (barrageEnabled && CanBardWeave && ActionReady(Barrage) && (HasEffect(Buffs.RagingStrikes) || !ragingEnabled) &&
                    !HasEffect(Buffs.ResonantArrowReady))
                    return Barrage;
            }

            #endregion

            #region OGCDS

            if (CanBardWeave && IsEnabled(CustomComboPreset.BRD_AoE_Adv_oGCD))
            {
                if (ActionReady(EmpyrealArrow))
                    return EmpyrealArrow;

                // Pitch perfect logic. Uses when full, or at 2 stacks before Empy arrow to prevent overcap
                if (LevelChecked(PitchPerfect) && SongWanderer &&
                    (gauge.Repertoire == 3 || LevelChecked(EmpyrealArrow) && gauge.Repertoire == 2 && EmpyrealCD < 2))
                    return OriginalHook(PitchPerfect);

                // Sidewinder Logic to stay in the buff window on 2 min, but on cd with the 1 min
                if (ActionReady(Sidewinder))
                {
                    if (SongWanderer)
                    {
                        if ((HasEffect(Buffs.RagingStrikes) || RagingCD > 10) &&
                            (HasEffect(Buffs.BattleVoice) || BattleVoiceCD > 10) &&
                            (HasEffect(Buffs.RadiantFinale) || RadiantCD > 10 ||
                             !LevelChecked(RadiantFinale)))
                            return Sidewinder;
                    }
                    else
                        return Sidewinder;
                }
            }

            // Interupt Logic, set to delayed weave. Let someone else do it if they want. Better to be last line of defense and stay off cd.
            if (Role.CanHeadGraze(CustomComboPreset.BRD_AoE_Adv_Interrupt) && CanWeaveDelayed)
                return Role.HeadGraze;

            // Rain of death Logic
            if (CanBardWeave && IsEnabled(CustomComboPreset.BRD_AoE_Adv_oGCD))
            {
                if (LevelChecked(RainOfDeath) && !WasLastAction(RainOfDeath) && EmpyrealCD > 1 || !LevelChecked(EmpyrealArrow))
                {
                    if (IsEnabled(CustomComboPreset.BRD_AoE_Pooling) && LevelChecked(WanderersMinuet) && TraitLevelChecked(Traits.EnhancedBloodletter))
                    {
                        //Stop pooling for buff window
                        if (SongWanderer && ((HasEffect(Buffs.RagingStrikes) || RagingCD > 10) &&
                            (HasEffect(Buffs.BattleVoice) || BattleVoiceCD > 10 ||
                             !LevelChecked(BattleVoice)) &&
                            (HasEffect(Buffs.RadiantFinale) || RadiantCD > 10 ||
                             !LevelChecked(RadiantFinale)) &&
                            RainOfDeathCharges > 0 || RainOfDeathCharges > 2))
                            return OriginalHook(RainOfDeath);

                        if (SongArmy && (RainOfDeathCharges == 3 || gauge.SongTimer / 1000 > 30 && RainOfDeathCharges > 0)) //Start pooling in Armys
                            return OriginalHook(RainOfDeath);

                        if (SongMage && RainOfDeathCharges > 0) // Dont poolin mages
                            return OriginalHook(RainOfDeath);

                        if (SongNone && RainOfDeathCharges == 3) //Pool when no song
                            return OriginalHook(RainOfDeath);
                    }

                    else if (RainOfDeathCharges > 0) //Dont pool when not enabled
                        return OriginalHook(RainOfDeath);
                }

                if (!LevelChecked(RainOfDeath) && !(WasLastAction(Bloodletter) && BloodletterCharges > 0))
                    return OriginalHook(Bloodletter);
            }

            #endregion

            #region Self Care

            if (CanBardWeave)
            {
                if (IsEnabled(CustomComboPreset.BRD_ST_SecondWind) && Role.CanSecondWind(Config.BRD_STSecondWindThreshold))
                    return Role.SecondWind;

                // Could be upgraded with a targetting system in the future
                if (IsEnabled(CustomComboPreset.BRD_ST_Wardens) && ActionReady(TheWardensPaeon) && HasCleansableDebuff(LocalPlayer))
                    return OriginalHook(TheWardensPaeon);
            }

            #endregion

            #region GCDS

            if (HasEffect(Buffs.HawksEye) || HasEffect(Buffs.Barrage))
                return OriginalHook(WideVolley);

            // Delay Encore enough for buff window
            if (IsEnabled(CustomComboPreset.BRD_Adv_BuffsEncore) && HasEffect(Buffs.RadiantEncoreReady) && GetBuffRemainingTime(Buffs.RadiantFinale) < 15)
                return OriginalHook(RadiantEncore);

            if (IsEnabled(CustomComboPreset.BRD_ST_ApexArrow)) // Apex Logic to time song in buff window and in mages.
            {
                if (HasEffect(Buffs.BlastArrowReady))
                    return BlastArrow;

                if (LevelChecked(ApexArrow))
                {
                    if (SongMage && gauge.SoulVoice == 100)
                        return ApexArrow;

                    if (SongMage && gauge.SoulVoice >= 80 &&
                        SongTimerInSeconds > 18 && SongTimerInSeconds < 22)
                        return ApexArrow;

                    if (SongWanderer && HasEffect(Buffs.RagingStrikes) && HasEffect(Buffs.BattleVoice) &&
                        (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale)) && gauge.SoulVoice >= 80)
                        return ApexArrow;
                }
            }

            if (IsEnabled(CustomComboPreset.BRD_Adv_BuffsResonant))
            {
                if (HasEffect(Buffs.ResonantArrowReady))
                    return ResonantArrow;
            }

            #endregion

            return actionID;
        }
    }

    internal class BRD_ST_AdvMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_ST_AdvMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HeavyShot or BurstShot))
                return actionID;

            int targetHPThreshold = Config.BRD_NoWasteHPPercentage;
            int ragingJawsRenewTime = Config.BRD_RagingJawsRenewTime;
            bool isEnemyHealthHigh = !IsEnabled(CustomComboPreset.BRD_Adv_NoWaste) || GetTargetHPPercent() > targetHPThreshold;
            bool ragingEnabled = IsEnabled(CustomComboPreset.BRD_Adv_Buffs_Raging);
            bool battleVoiceEnabled = IsEnabled(CustomComboPreset.BRD_Adv_Buffs_Battlevoice);
            bool barrageEnabled = IsEnabled(CustomComboPreset.BRD_Adv_Buffs_Barrage);
            bool radiantEnabled = IsEnabled(CustomComboPreset.BRD_Adv_Buffs_RadiantFinale);

            #region Variants

            if (Variant.CanCure(CustomComboPreset.BRD_Variant_Cure, Config.BRD_VariantCure))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BRD_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            #endregion

            if (IsEnabled(CustomComboPreset.BRD_ST_Adv_Balance_Standard) &&
                Opener().FullOpener(ref actionID))
            {
                if (ActionWatching.GetAttackType(Opener().CurrentOpenerAction) != ActionWatching.ActionAttackType.Ability && CanBardWeave)
                {
                    if (HasEffect(Buffs.RagingStrikes) && (gauge.Repertoire == 3 || gauge.Repertoire == 2 && EmpyrealCD < 2))
                        return OriginalHook(PitchPerfect);

                    if (ActionReady(HeartbreakShot) && HasEffect(Buffs.RagingStrikes))
                        return HeartbreakShot;
                }

                return actionID;
            }

            #region Songs

            if (IsEnabled(CustomComboPreset.BRD_Adv_Song) && isEnemyHealthHigh)
            {
                // Limit optimisation to when you are high enough level to benefit from it.
                if (LevelChecked(WanderersMinuet))
                {
                    if (CanBardWeave || !BardHasTarget)
                    {
                        if (SongNone && InCombat())
                        {
                            // Logic to determine first song
                            if (ActionReady(WanderersMinuet) && !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)))
                                return WanderersMinuet;

                            if (ActionReady(MagesBallad) && !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon)))
                                return MagesBallad;

                            if (ActionReady(ArmysPaeon) && !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet)))
                                return ArmysPaeon;
                        }

                        if (SongWanderer)
                        {
                            if (SongTimerInSeconds <= 3 && gauge.Repertoire > 0 && BardHasTarget) // Spend any repertoire before switching to next song
                                return OriginalHook(PitchPerfect);

                            if (SongTimerInSeconds <= 3 && ActionReady(MagesBallad)) // Move to Mage's Ballad if <= 3 seconds left on song
                                return MagesBallad;
                        }

                        // Move to Army's Paeon if <= 3 seconds left on song
                        // Special case for Empyreal Arrow: it must be cast before you change to it to avoid drift!
                        if (SongMage && SongTimerInSeconds <= 3 && ActionReady(ArmysPaeon))
                        {
                            if (ActionReady(EmpyrealArrow) && BardHasTarget)
                                return EmpyrealArrow;

                            return ArmysPaeon;
                        }
                    }

                    // Move to Wanderer's Minuet if <= 12 seconds left on song or WM off CD and have 4 repertoires of AP
                    if (SongArmy && (CanWeaveDelayed || !BardHasTarget) && (SongTimerInSeconds <= 12 || ActionReady(WanderersMinuet) && gauge.Repertoire == 4))
                        return WanderersMinuet;
                }

                else if (SongTimerInSeconds <= 3 && CanWeaveDelayed) // Before you get Wanderers, it just toggles the two songs.
                {
                    if (ActionReady(MagesBallad))
                        return MagesBallad;

                    if (ActionReady(ArmysPaeon))
                        return ArmysPaeon;
                }
            }

            #endregion

            #region Buffs

            if (IsEnabled(CustomComboPreset.BRD_Adv_Buffs) &&
                (!SongNone || !LevelChecked(MagesBallad)) && isEnemyHealthHigh)
            {
                // Radiant First with late weave for tighter grouping
                if (radiantEnabled && CanWeaveDelayed && ActionReady(RadiantFinale) && (RagingCD < 2.3 || !ragingEnabled) &&
                    !HasEffect(Buffs.RadiantEncoreReady))
                    return RadiantFinale;

                // BV normal weave into the raging weave
                if (battleVoiceEnabled && CanBardWeave && ActionReady(BattleVoice) && (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale) || !radiantEnabled))
                    return BattleVoice;

                // Late weave Raging last, must have battle voice buff OR not be high enough level for Battlecoice
                if (ragingEnabled && CanBardWeave && ActionReady(RagingStrikes) && (JustUsed(BattleVoice) || !LevelChecked(BattleVoice) || HasEffect(Buffs.BattleVoice) || !battleVoiceEnabled))
                    return RagingStrikes;

                // Barrage Logic to check for raging for low level reasons and it doesn't really need to check for the other buffs
                if (barrageEnabled && CanBardWeave && ActionReady(Barrage) && (HasEffect(Buffs.RagingStrikes) || !ragingEnabled) &&
                    !HasEffect(Buffs.ResonantArrowReady))
                    return Barrage;
            }

            #endregion

            #region OGCD

            if (CanBardWeave && IsEnabled(CustomComboPreset.BRD_ST_Adv_oGCD) &&
                (!BuffTime || !IsEnabled(CustomComboPreset.BRD_Adv_Buffs)))
            {
                // Pitch Perfect logic to use when full or when Empyreal arrow might overcap it.
                if (LevelChecked(PitchPerfect) && SongWanderer &&
                    (gauge.Repertoire == 3 || LevelChecked(EmpyrealArrow) && gauge.Repertoire == 2 && EmpyrealCD < 2))
                    return OriginalHook(PitchPerfect);

                if (ActionReady(EmpyrealArrow))
                    return EmpyrealArrow;

                // Sidewinder logic to use in burst window with buffs or on cd on the 1 minutes
                if (ActionReady(Sidewinder))
                {
                    if (IsEnabled(CustomComboPreset.BRD_Adv_Pooling))
                    {
                        if (SongWanderer)
                        {
                            if ((HasEffect(Buffs.RagingStrikes) || RagingCD > 10) &&
                                (HasEffect(Buffs.BattleVoice) || BattleVoiceCD > 10) &&
                                (HasEffect(Buffs.RadiantFinale) || RadiantCD > 10 ||
                                 !LevelChecked(RadiantFinale)))
                                return Sidewinder;
                        }
                        else
                            return Sidewinder;
                    }
                    else
                        return Sidewinder;
                }
            }
            //Interupt Logic, set to delayed weave. Let someone else do it if they want. Better to be last line of defense and stay off cd.
            if (Role.CanHeadGraze(CustomComboPreset.BRD_Adv_Interrupt) && CanWeaveDelayed)
                return Role.HeadGraze;

            // Bloodletter pooling logic. Will Pool as buffs are coming up.
            if (CanBardWeave && IsEnabled(CustomComboPreset.BRD_ST_Adv_oGCD))
            {
                if (ActionReady(Bloodletter) && !(WasLastAction(Bloodletter) || WasLastAction(HeartbreakShot)) && EmpyrealCD > 1 || !LevelChecked(EmpyrealArrow))
                {
                    if (IsEnabled(CustomComboPreset.BRD_Adv_Pooling) &&
                        LevelChecked(WanderersMinuet) && TraitLevelChecked(Traits.EnhancedBloodletter))
                    {
                        if (SongWanderer && ((HasEffect(Buffs.RagingStrikes) || RagingCD > 10) &&
                            (HasEffect(Buffs.BattleVoice) || BattleVoiceCD > 10 ||
                             !LevelChecked(BattleVoice)) &&
                            (HasEffect(Buffs.RadiantFinale) || RadiantCD > 10 ||
                             !LevelChecked(RadiantFinale)) &&
                            BloodletterCharges > 0 || BloodletterCharges > 2))
                            return OriginalHook(Bloodletter);

                        if (SongArmy && (BloodletterCharges == 3 || gauge.SongTimer / 1000 > 30 && BloodletterCharges > 0)) // Start pooling in Army
                            return OriginalHook(Bloodletter);

                        if (SongMage && BloodletterCharges > 0) //Don't pool in Mages
                            return OriginalHook(Bloodletter);

                        if (SongNone && BloodletterCharges == 3) //Pool with no song
                            return OriginalHook(Bloodletter);
                    }
                    else if (BloodletterCharges > 0)
                        return OriginalHook(Bloodletter);
                }
            }

            #endregion

            #region Self Care

            if (CanBardWeave)
            {
                if (IsEnabled(CustomComboPreset.BRD_ST_SecondWind) && Role.CanSecondWind(Config.BRD_STSecondWindThreshold))
                    return Role.SecondWind;

                // Could be upgraded with a targetting system in the future
                if (IsEnabled(CustomComboPreset.BRD_ST_Wardens) && ActionReady(TheWardensPaeon) && HasCleansableDebuff(LocalPlayer))
                    return OriginalHook(TheWardensPaeon);
            }

            #endregion

            #region Dot Management

            if (isEnemyHealthHigh)
            {
                if (IsEnabled(CustomComboPreset.BRD_Adv_DoT))
                {
                    if (Purple is not null && PurpleRemaining < 4)
                        return CanIronJaws
                            ? IronJaws
                            : VenomousBite;

                    if (Blue is not null && BlueRemaining < 4)
                        return CanIronJaws
                            ? IronJaws
                            : Windbite;

                    if (Blue is null && LevelChecked(Windbite))
                        return OriginalHook(Windbite);

                    if (Purple is null && LevelChecked(VenomousBite))
                        return OriginalHook(VenomousBite);

                    if (IsEnabled(CustomComboPreset.BRD_Adv_RagingJaws) && ActionReady(IronJaws) && HasEffect(Buffs.RagingStrikes) &&
                        RagingStrikesDuration < ragingJawsRenewTime && // Raging Jaws Slider Check
                        PurpleRemaining < 35 && BlueRemaining < 35) // Prevention of double refreshing dots
                        return IronJaws;
                }
            }

            #endregion

            #region GCDS

            if (HasEffect(Buffs.HawksEye) || HasEffect(Buffs.Barrage))
                return OriginalHook(StraightShot);

            // Delay Encore enough for buff window
            if (IsEnabled(CustomComboPreset.BRD_Adv_BuffsEncore) &&
                HasEffect(Buffs.RadiantEncoreReady) && HasEffect(Buffs.RagingStrikes))
                return OriginalHook(RadiantEncore);

            if (IsEnabled(CustomComboPreset.BRD_ST_ApexArrow)) // Apex Logic to time song in buff window and in mages.
            {
                if (HasEffect(Buffs.BlastArrowReady))
                    return BlastArrow;

                if (LevelChecked(ApexArrow))
                {
                    if (SongMage && gauge.SoulVoice == 100)
                        return ApexArrow;

                    if (SongMage && gauge.SoulVoice >= 80 &&
                        SongTimerInSeconds > 18 && SongTimerInSeconds < 22)
                        return ApexArrow;

                    if (SongWanderer && HasEffect(Buffs.RagingStrikes) && HasEffect(Buffs.BattleVoice) &&
                        (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale)) && gauge.SoulVoice >= 80)
                        return ApexArrow;
                }
            }

            if (IsEnabled(CustomComboPreset.BRD_Adv_BuffsResonant) && HasEffect(Buffs.ResonantArrowReady))
                return ResonantArrow;

            #endregion

            return actionID;
        }
    }

    #endregion

    #region Simple Modes

    internal class BRD_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_AoE_SimpleMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (Ladonsbite or QuickNock))
                return actionID;

            #region Variants

            if (Variant.CanCure(CustomComboPreset.BRD_Variant_Cure, 50))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BRD_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            #endregion

            #region Songs

            // Limit optimisation to when you are high enough level to benefit from it.
            if (LevelChecked(WanderersMinuet))
            {
                if (CanBardWeave || !BardHasTarget)
                {
                    if (SongNone && InCombat())
                    {
                        // Logic to determine first song
                        if (ActionReady(WanderersMinuet) && !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)))
                            return WanderersMinuet;

                        if (ActionReady(MagesBallad) && !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon)))
                            return MagesBallad;

                        if (ActionReady(ArmysPaeon) && !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet)))
                            return ArmysPaeon;
                    }

                    if (SongWanderer)
                    {
                        if (SongTimerInSeconds <= 3 && gauge.Repertoire > 0 && BardHasTarget) // Spend any repertoire before switching to next song
                            return OriginalHook(PitchPerfect);

                        if (SongTimerInSeconds <= 3 && ActionReady(MagesBallad)) // Move to Mage's Ballad if <= 3 seconds left on song
                            return MagesBallad;
                    }

                    // Move to Army's Paeon if < 3 seconds left on song
                    // Special case for Empyreal Arrow: it must be cast before you change to it to avoid drift!
                    if (SongMage && SongTimerInSeconds <= 3 && ActionReady(ArmysPaeon))
                    {
                        return ActionReady(EmpyrealArrow) && BardHasTarget
                            ? EmpyrealArrow
                            : ArmysPaeon;
                    }
                }

                // Move to Wanderer's Minuet if <= 12 seconds left on song or WM off CD and have 4 repertoires of AP
                if (SongArmy && (CanWeaveDelayed || !BardHasTarget) && (SongTimerInSeconds <= 12 || gauge.Repertoire == 4) && ActionReady(WanderersMinuet))
                    return WanderersMinuet;
            }

            else if (SongTimerInSeconds <= 3 && CanWeaveDelayed) // Not high enough for wanderers Minuet yet
            {
                if (ActionReady(MagesBallad))
                    return MagesBallad;

                if (ActionReady(ArmysPaeon))
                    return ArmysPaeon;
            }

            #endregion

            #region Buffs

            if (!SongNone || !LevelChecked(MagesBallad))
            {
                // Radiant First with late weave for tighter grouping
                if (CanWeaveDelayed && ActionReady(RadiantFinale) && RagingCD < 2.3 &&
                    !HasEffect(Buffs.RadiantEncoreReady))
                    return RadiantFinale;

                // BV normal weave into the raging weave
                if (CanBardWeave && ActionReady(BattleVoice) && (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale)))
                    return BattleVoice;

                // Late weave Raging last, must have battle voice buff OR not be high enough level for Battlecoice
                if (CanBardWeave && ActionReady(RagingStrikes) && (JustUsed(BattleVoice) || !LevelChecked(BattleVoice) || HasEffect(Buffs.BattleVoice)))
                    return RagingStrikes;

                // Barrage Logic to check for raging for low level reasons and it doesn't really need to check for the other buffs
                if (CanBardWeave && ActionReady(Barrage) && HasEffect(Buffs.RagingStrikes) &&
                    !HasEffect(Buffs.ResonantArrowReady))
                    return Barrage;
            }

            #endregion

            #region OGCDS and Selfcare

            if (CanBardWeave)
            {
                if (ActionReady(EmpyrealArrow))
                    return EmpyrealArrow;

                // Pitch Perfect logic to use when full or when Empy arrow can cause an overcap
                if (LevelChecked(PitchPerfect) && SongWanderer &&
                    (gauge.Repertoire == 3 || LevelChecked(EmpyrealArrow) && gauge.Repertoire == 2 && EmpyrealCD < 2))
                    return OriginalHook(PitchPerfect);

                // Sidewinder Logic to use in Window and on the 1 min
                if (ActionReady(Sidewinder))
                {
                    if (SongWanderer)
                    {
                        if ((HasEffect(Buffs.RagingStrikes) || RagingCD > 10) &&
                            (HasEffect(Buffs.BattleVoice) || BattleVoiceCD > 10) &&
                            (HasEffect(Buffs.RadiantFinale) || RadiantCD > 10 ||
                             !LevelChecked(RadiantFinale)))
                            return Sidewinder;
                    }
                    else
                        return Sidewinder;
                }

                // Interupt
                if (Role.CanHeadGraze(CustomComboPreset.BRD_AoE_SimpleMode) && CanWeaveDelayed)
                    return Role.HeadGraze;

                // Pooling logic for rain of death basied on song
                if (LevelChecked(RainOfDeath) && !WasLastAction(RainOfDeath) && (EmpyrealCD > 1 || !LevelChecked(EmpyrealArrow)))
                {
                    if (LevelChecked(WanderersMinuet) && TraitLevelChecked(Traits.EnhancedBloodletter))
                    {
                        if (SongWanderer && ((HasEffect(Buffs.RagingStrikes) || RagingCD > 10) &&
                            (HasEffect(Buffs.BattleVoice) || BattleVoiceCD > 10 ||
                             !LevelChecked(BattleVoice)) &&
                            (HasEffect(Buffs.RadiantFinale) || RadiantCD > 10 ||
                             !LevelChecked(RadiantFinale)) &&
                            RainOfDeathCharges > 0 || RainOfDeathCharges > 2))
                            return OriginalHook(RainOfDeath);

                        if (SongArmy && (RainOfDeathCharges == 3 || gauge.SongTimer / 1000 > 30 && RainOfDeathCharges > 0))
                            return OriginalHook(RainOfDeath);

                        if (SongMage && RainOfDeathCharges > 0)
                            return OriginalHook(RainOfDeath);

                        if (SongNone && RainOfDeathCharges == 3)
                            return OriginalHook(RainOfDeath);
                    }
                    else if (RainOfDeathCharges > 0)
                        return OriginalHook(RainOfDeath);
                }

                if (!LevelChecked(RainOfDeath) && !(WasLastAction(Bloodletter) && BloodletterCharges > 0))
                    return OriginalHook(Bloodletter);

                // Self care section for healing and debuff removal

                if (Role.CanSecondWind(40))
                    return Role.SecondWind;

                if (ActionReady(TheWardensPaeon) && HasCleansableDebuff(LocalPlayer))
                    return OriginalHook(TheWardensPaeon);
            }

            #endregion

            #region GCDS

            if (HasEffect(Buffs.HawksEye) || HasEffect(Buffs.Barrage)) //Ahead of other gcds because of higher risk of losing a proc than a ready buff
                return OriginalHook(WideVolley);

            if (LevelChecked(ApexArrow) && gauge.SoulVoice == 100)
                return ApexArrow;

            if (HasEffect(Buffs.BlastArrowReady))
                return BlastArrow;

            if (HasEffect(Buffs.ResonantArrowReady))
                return ResonantArrow;

            if (HasEffect(Buffs.RadiantEncoreReady))
                return OriginalHook(RadiantEncore);

            #endregion

            return actionID;
        }
    }

    internal class BRD_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_ST_SimpleMode;
        protected override uint Invoke(uint actionID)
        {
            if (actionID is not (HeavyShot or BurstShot))
                return actionID;

            #region Variants

            if (Variant.CanCure(CustomComboPreset.BRD_Variant_Cure, 50))
                return Variant.Cure;

            if (Variant.CanRampart(CustomComboPreset.BRD_Variant_Rampart, WeaveTypes.Weave))
                return Variant.Rampart;

            #endregion

            #region Songs

            // Limit optimisation to when you are high enough level to benefit from it.
            if (LevelChecked(WanderersMinuet))
            {
                // 43s of Wanderer's Minute, ~36s of Mage's Ballad, and ~43s of Army's Paeon

                if (ActionReady(EmpyrealArrow) && JustUsed(WanderersMinuet))
                    return EmpyrealArrow;

                if (CanBardWeave || !BardHasTarget)
                {
                    if (SongNone && InCombat())
                    {
                        // Logic to determine first song
                        if (ActionReady(WanderersMinuet) && !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)))
                            return WanderersMinuet;

                        if (ActionReady(MagesBallad) && !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon)))
                            return MagesBallad;

                        if (ActionReady(ArmysPaeon) && !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet)))
                            return ArmysPaeon;
                    }

                    if (SongWanderer)
                    {
                        if (SongTimerInSeconds <= 3 && gauge.Repertoire > 0 && BardHasTarget) // Spend any repertoire before switching to next song
                            return OriginalHook(PitchPerfect);

                        if (SongTimerInSeconds <= 3 && ActionReady(MagesBallad)) // Move to Mage's Ballad if <= 3 seconds left on song
                            return MagesBallad;
                    }

                    if (SongMage)
                    {
                        // Move to Army's Paeon if <= 3 seconds left on song
                        if (SongTimerInSeconds <= 3 && ActionReady(ArmysPaeon))
                        {
                            // Special case for Empyreal Arrow: it must be cast before you change to it to avoid drift!
                            if (ActionReady(EmpyrealArrow) && BardHasTarget)
                                return EmpyrealArrow;

                            return ArmysPaeon;
                        }
                    }
                }

                // Move to Wanderer's Minuet if <= 12 seconds left on song or WM off CD and have 4 repertoires of AP
                if (SongArmy && (CanWeaveDelayed || !BardHasTarget) &&
                    (SongTimerInSeconds <= 12 || ActionReady(WanderersMinuet) && gauge.Repertoire == 4))
                    return WanderersMinuet;
            }

            else if (SongTimerInSeconds <= 3 && CanWeaveDelayed)
            {
                if (ActionReady(MagesBallad))
                    return MagesBallad;

                if (ActionReady(ArmysPaeon))
                    return ArmysPaeon;
            }

            #endregion

            #region Buffs

            if (!SongNone || !LevelChecked(MagesBallad))
            {
                // Radiant First with late weave for tighter grouping
                if (CanWeaveDelayed && ActionReady(RadiantFinale) && RagingCD < 2.3 &&
                    !HasEffect(Buffs.RadiantEncoreReady))
                    return RadiantFinale;

                // BV normal weave into the raging weave
                if (CanBardWeave && ActionReady(BattleVoice) && (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale)))
                    return BattleVoice;

                // Late weave Raging last, must have battle voice buff OR not be high enough level for Battlecoice
                if (CanBardWeave && ActionReady(RagingStrikes) && (JustUsed(BattleVoice) || !LevelChecked(BattleVoice) || HasEffect(Buffs.BattleVoice)))
                    return RagingStrikes;

                // Barrage Logic to check for raging for low level reasons and it doesn't really need to check for the other buffs
                if (CanBardWeave && ActionReady(Barrage) && HasEffect(Buffs.RagingStrikes) &&
                    !HasEffect(Buffs.ResonantArrowReady))
                    return Barrage;
            }

            #endregion

            #region OGCDS

            if (CanBardWeave)
            {
                // Empyreal Arrow first to minimize drift
                if (ActionReady(EmpyrealArrow))
                    return EmpyrealArrow;

                //Pitch Perfect Logic to not let Empyreal arrow overcap
                if (LevelChecked(PitchPerfect) && SongWanderer &&
                    (gauge.Repertoire == 3 || LevelChecked(EmpyrealArrow) && gauge.Repertoire == 2 && EmpyrealCD < 2))
                    return OriginalHook(PitchPerfect);

                // Sidewinder Logic for burst window and 1 min
                if (ActionReady(Sidewinder))
                {
                    if (SongWanderer)
                    {
                        if ((HasEffect(Buffs.RagingStrikes) || RagingCD > 10) &&
                            (HasEffect(Buffs.BattleVoice) || BattleVoiceCD > 10) &&
                            (HasEffect(Buffs.RadiantFinale) || RadiantCD > 10 ||
                             !LevelChecked(RadiantFinale)))
                            return Sidewinder;
                    }

                    else
                        return Sidewinder;
                }

                //Interupt delayered weave
                if (Role.CanHeadGraze(CustomComboPreset.BRD_ST_SimpleMode) && CanWeaveDelayed)
                    return Role.HeadGraze;

                // Bloodletter pooling logic
                if (ActionReady(Bloodletter) && !(WasLastAction(Bloodletter) || WasLastAction(HeartbreakShot)) && (EmpyrealCD > 1 || !LevelChecked(EmpyrealArrow)))
                {
                    if (LevelChecked(WanderersMinuet) && TraitLevelChecked(Traits.EnhancedBloodletter))
                    {
                        // Stop pooling in burst window
                        if (SongWanderer && ((HasEffect(Buffs.RagingStrikes) || RagingCD > 10) &&
                            (HasEffect(Buffs.BattleVoice) || BattleVoiceCD > 10 ||
                             !LevelChecked(BattleVoice)) &&
                            (HasEffect(Buffs.RadiantFinale) || RadiantCD > 10 ||
                             !LevelChecked(RadiantFinale)) &&
                            BloodletterCharges > 0 || BloodletterCharges > 2))
                            return OriginalHook(Bloodletter);

                        if (SongArmy && (BloodletterCharges == 3 || gauge.SongTimer / 1000 > 30 && BloodletterCharges > 0)) // Start pooling in army
                            return OriginalHook(Bloodletter);

                        if (SongMage && BloodletterCharges > 0) // Dont pool in mages
                            return OriginalHook(Bloodletter);

                        if (SongNone && BloodletterCharges == 3) // No song pooling
                            return OriginalHook(Bloodletter);
                    }

                    else if (BloodletterCharges > 0)
                        return OriginalHook(Bloodletter);
                }

                // Self Care

                if (Role.CanSecondWind(40))
                    return Role.SecondWind;

                if (ActionReady(TheWardensPaeon) && HasCleansableDebuff(LocalPlayer))
                    return OriginalHook(TheWardensPaeon);
            }

            #endregion

            #region Dot Management

            // Iron jaws Dot refresh, or low level manaul dot refresh
            if (Purple is not null && PurpleRemaining < 4)
                return CanIronJaws
                    ? IronJaws
                    : VenomousBite;

            if (Blue is not null && BlueRemaining < 4)
                return CanIronJaws
                    ? IronJaws
                    : Windbite;

            // Dot application
            if (Blue is null && LevelChecked(Windbite))
                return OriginalHook(Windbite);

            if (Purple is null && LevelChecked(VenomousBite))
                return OriginalHook(VenomousBite);

            // Raging jaws dot snapshotting logic
            if (ActionReady(IronJaws) && HasEffect(Buffs.RagingStrikes) &&
                RagingStrikesDuration < 6 && // Raging Jaws 
                PurpleRemaining < 35 && BlueRemaining < 35) // Prevention of double refreshing dots
                return IronJaws;

            #endregion

            #region GCDS

            if (HasEffect(Buffs.HawksEye) || HasEffect(Buffs.Barrage))
                return OriginalHook(StraightShot);

            if (LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady))
                return BlastArrow;

            if (LevelChecked(ApexArrow)) //Apex Logic to use in the burst window and around the 1 min.
            {
                if (SongMage && gauge.SoulVoice == 100)
                    return ApexArrow;

                if (SongMage && gauge.SoulVoice >= 80 &&
                    SongTimerInSeconds > 18 && SongTimerInSeconds < 22)
                    return ApexArrow;

                if (SongWanderer && HasEffect(Buffs.RagingStrikes) && HasEffect(Buffs.BattleVoice) &&
                    (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale)) && gauge.SoulVoice >= 80)
                    return ApexArrow;
            }

            if (HasEffect(Buffs.ResonantArrowReady))
                return ResonantArrow;

            if (HasEffect(Buffs.RadiantEncoreReady) && !JustUsed(RadiantFinale) && GetCooldownElapsed(BattleVoice) >= 4.2f)
                return OriginalHook(RadiantEncore);

            #endregion
            return actionID;
        }
    }

    #endregion
}
