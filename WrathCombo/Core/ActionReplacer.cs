using Dalamud.Hooking;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECommons.Throttlers;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Services;

namespace WrathCombo.Core
{
    /// <summary> This class facilitates action+icon replacement. </summary>
    internal sealed class ActionReplacer : IDisposable
    {
        public readonly List<CustomCombo> CustomCombos;
        private readonly Hook<IsActionReplaceableDelegate> isActionReplaceableHook;
        public readonly Hook<GetActionDelegate> getActionHook;

        private IntPtr actionManager = IntPtr.Zero;
        private readonly IntPtr module = IntPtr.Zero;
        private Dictionary<uint, uint> lastActionInvokeFor = new Dictionary<uint, uint>();

        /// <summary> Initializes a new instance of the <see cref="ActionReplacer"/> class. </summary>
        public ActionReplacer()
        {
            CustomCombos = Assembly.GetAssembly(typeof(CustomCombo))!.GetTypes()
                .Where(t => !t.IsAbstract && t.BaseType == typeof(CustomCombo))
                .Select(t => Activator.CreateInstance(t))
                .Cast<CustomCombo>()
                .OrderByDescending(x => x.Preset)
                .ToList();

            getActionHook = Svc.Hook.HookFromAddress<GetActionDelegate>((nint)ActionManager.Addresses.GetAdjustedActionId.Value, GetAdjustedActionDetour);
            isActionReplaceableHook = Svc.Hook.HookFromAddress<IsActionReplaceableDelegate>(Service.Address.IsActionIdReplaceable, IsActionReplaceableDetour);

            getActionHook.Enable();
            isActionReplaceableHook.Enable();
        }

        private delegate ulong IsActionReplaceableDelegate(uint actionID);

        public delegate uint GetActionDelegate(IntPtr actionManager, uint actionID);

        /// <inheritdoc/>
        public void Dispose()
        {
            getActionHook?.Disable();
            getActionHook?.Dispose();
            isActionReplaceableHook?.Disable();
            isActionReplaceableHook?.Dispose();
        }

        /// <summary> Calls the original hook. </summary>
        /// <param name="actionID"> Action ID. </param>
        /// <returns> The result from the hook. </returns>
        internal uint OriginalHook(uint actionID) => getActionHook.Original(actionManager, actionID);

        public static IEnumerable<CustomCombo>? FilteredCombos;

        public void UpdateFilteredCombos()
        {
            FilteredCombos = CustomCombos.Where(x => x.Preset.Attributes() is not null && x.Preset.Attributes().IsPvP == CustomComboFunctions.InPvP() && ((x.Preset.Attributes().RoleAttribute is not null && x.Preset.Attributes().RoleAttribute.PlayerIsRole()) || x.Preset.Attributes().CustomComboInfo.JobID == Player.JobId || x.Preset.Attributes().CustomComboInfo.JobID == CustomComboFunctions.JobIDs.ClassToJob(Player.JobId)));
            Svc.Log.Debug($"Now running {FilteredCombos.Count()} combos\n{string.Join("\n", FilteredCombos.Select(x => x.Preset.Attributes().CustomComboInfo.Name))}");
        }

        private uint GetAdjustedActionDetour(IntPtr _, uint actionID)
        {
            if (FilteredCombos is null)
                UpdateFilteredCombos();

            // Only refresh every so often
            if (!EzThrottler.Throttle("Actions" + actionID,
                    Service.Configuration.Throttle))
                return lastActionInvokeFor[actionID];

            // Actually get the action
            lastActionInvokeFor[actionID] = GetAdjustedAction(actionID);
            return lastActionInvokeFor[actionID];
        }

        private unsafe uint GetAdjustedAction(uint actionID)
        {
            try
            {
                if (Service.Configuration.PerformanceMode)
                    return OriginalHook(actionID);

                if (Svc.ClientState.LocalPlayer == null)
                    return OriginalHook(actionID);

                if (ClassLocked() ||
                    (DisabledJobsPVE.Any(x => x == Svc.ClientState.LocalPlayer.ClassJob.RowId) && !Svc.ClientState.IsPvP) ||
                    (DisabledJobsPVP.Any(x => x == Svc.ClientState.LocalPlayer.ClassJob.RowId) && Svc.ClientState.IsPvP))
                    return OriginalHook(actionID);

                foreach (CustomCombo? combo in FilteredCombos)
                {
                    if (combo.TryInvoke(actionID, out uint newActionID))
                    {
                        if (Service.Configuration.BlockSpellOnMove && ActionManager.GetAdjustedCastTime(ActionType.Action, newActionID) > 0 && CustomComboFunctions.TimeMoving.Ticks > 0)
                        {
                            return All.SavageBlade;
                        }
                        return newActionID;
                    }
                }

                return OriginalHook(actionID);
            }

            catch (Exception ex)
            {
                Svc.Log.Error(ex, "Preset error");
                return OriginalHook(actionID);
            }
        }

        // Class locking
        public unsafe static bool ClassLocked()
        {
            if (Svc.ClientState.LocalPlayer is null) return false;

            if (Svc.ClientState.LocalPlayer.Level <= 35) return false;

            if (Svc.ClientState.LocalPlayer.ClassJob.RowId is
                (>= 8 and <= 25) or 27 or 28 or >= 30)
                return false;

            if (!UIState.Instance()->IsUnlockLinkUnlockedOrQuestCompleted(66049))
                return false;

            if ((Svc.ClientState.LocalPlayer.ClassJob.RowId is 1 or 2 or 3 or 4 or 5 or 6 or 7 or 26 or 29) &&
                Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty] &&
                Svc.ClientState.LocalPlayer.Level > 35) return true;

            return false;
        }

        private ulong IsActionReplaceableDetour(uint actionID) => 1;
    }
}
