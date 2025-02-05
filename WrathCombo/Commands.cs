#region

using System;
using System.Linq;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Logging;
using Lumina.Excel.Sheets;
using WrathCombo.Combos;
using WrathCombo.Core;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Extensions;
using WrathCombo.Services;
using WrathCombo.Window;
using WrathCombo.Window.Tabs;

#endregion

namespace WrathCombo;

public partial class WrathCombo
{
    private const string Command = "/wrath";
    private const string OldCommand = "/scombo";

    private void RegisterCommands()
    {
        EzCmd.Add(Command, OnCommand,
            "Open a window to edit custom combo settings.\n" +
            $"{Command} auto → Toggle Auto-rotation on/off.\n" +
            $"{Command} debug → Dumps a debug log onto your desktop for developers.\n" +
            $"{OldCommand} → Old alias from XIVSlothCombo, still works!");
        EzCmd.Add(OldCommand, OnCommand);
    }

    private void OnCommand(string command, string arguments)
    {
        var argumentParts = arguments.ToLowerInvariant().Split();
        switch (argumentParts[0])
        {
            case "unsetall":
            case "set":
            case "toggle":
            case "unset":
                HandleSetCommands(argumentParts); break;

            case "list":
            case "enabled":
            case "disabled":
                HandleListCommands(argumentParts); break;

            case "combo":
                HandleComboCommands(argumentParts); break;

            case "auto":
                HandleAutoCommands(argumentParts); break;

            case "ignore":
                HandleIgnoreCommand(); break;

            case "debug":
                HandleDebugCommands(argumentParts); break;

            default:
                HandleOpenCommand(argumentParts); break;
        }

        Service.Configuration.Save();
    }

    private void HandleSetCommands(string[] argument)
    {
        #region Variable Setup

        const string toggle = "toggle";
        const string set = "set";
        const string unset = "unset";

        const string all = "all";

        string? action = null;
        string? target = null;

        CustomComboPreset? preset = null;

        #endregion

        if (Player.Available && CustomComboFunctions.InCombat())
        {
            DuoLog.Error("Cannot use this command in combat");
            return;
        }

        // Parse the action
        switch (argument[0])
        {
            case "unsetall":
                action = unset;
                target = all;
                break;

            case "set":
                action = set;
                break;

            case "toggle":
                action = toggle;
                break;

            case "unset":
                action = unset;
                break;
        }

        if (target is null && argument.Length < 2)
            DuoLog.Error($"Please specify a feature to {action}");

        // Parse the target feature
        target ??= argument[1];
        if (target != all)
        {
            var presetCanNumber = int.TryParse(target, out var targetNumber);
            preset = presetCanNumber
                ? (CustomComboPreset)targetNumber
                : Enum.Parse<CustomComboPreset>(target, true);
        }

        // Give the correct method for the action
        Func<CustomComboPreset, bool, bool> method = action switch
        {
            toggle => PresetStorage.TogglePreset,
            set => PresetStorage.EnablePreset,
            unset => PresetStorage.DisablePreset,
            _ => throw new ArgumentOutOfRangeException(nameof(argument), action,
                null),
        };

        // Execute the method
        if (target == all)
        {
            Service.Configuration.EnabledActions.Clear();
            DuoLog.Information("All unset");
        }
        else
        {
            var usablePreset = (CustomComboPreset)preset!;
            method(usablePreset, false);

            var ctrlText = P.UIHelper.PresetControlled(usablePreset) is not null
                ? " " + OptionControlledByIPC
                : "";

            DuoLog.Information(
                $"{target} {(action == toggle ? "toggled" : action)} {ctrlText}");
        }
    }

    private void HandleListCommands(string[] argument)
    {
        var filter = argument.Length > 1 ? argument[1].ToLowerInvariant() :
            argument[0].ToLowerInvariant() == "enabled" ? "enabled" : "all";

        switch (filter)
        {
            case "enabled":
            case "set":
                foreach (var preset in Enum.GetValues<CustomComboPreset>()
                             .Where(preset =>
                                 IPC.GetComboState(preset.ToString())!.First()
                                     .Value))
                {
                    var controlled =
                        P.UIHelper.PresetControlled(preset) is not null;
                    var ctrlText = controlled ? " " + OptionControlledByIPC : "";
                    DuoLog.Information($"{(int)preset} - {preset}{ctrlText}");
                }

                break;

            case "unset":
                foreach (var preset in Enum.GetValues<CustomComboPreset>()
                             .Where(preset =>
                                 !IPC.GetComboState(preset.ToString())!.First()
                                     .Value))
                {
                    var controlled =
                        P.UIHelper.PresetControlled(preset) is not null;
                    var ctrlText = controlled ? " " + OptionControlledByIPC : "";
                    DuoLog.Information($"{(int)preset} - {preset}{ctrlText}");
                }

                break;

            case "all":
                foreach (var preset in Enum.GetValues<CustomComboPreset>())
                {
                    var controlled =
                        P.UIHelper.PresetControlled(preset) is not null;
                    var ctrlText = controlled ? " " + OptionControlledByIPC : "";
                    DuoLog.Information($"{(int)preset} - {preset}{ctrlText}");
                }

                break;

            default:
                DuoLog.Error("Available list filters: set, enabled, unset, all");
                break;
        }
    }

    private void HandleComboCommands(string[] argument)
    {
        if (argument.Length < 2)
        {
            if (Service.IconReplacer.getIconHook.IsEnabled)
                Service.IconReplacer.getIconHook.Disable();
            else
                Service.IconReplacer.getIconHook.Enable();
            return;
        }

        switch (argument[1])
        {
            case "on":
                if (!Service.IconReplacer.getIconHook.IsEnabled)
                    Service.IconReplacer.getIconHook.Enable();
                break;

            case "off":
                if (Service.IconReplacer.getIconHook.IsEnabled)
                    Service.IconReplacer.getIconHook.Disable();
                break;

            case "toggle":
                if (Service.IconReplacer.getIconHook.IsEnabled)
                    Service.IconReplacer.getIconHook.Disable();
                else
                    Service.IconReplacer.getIconHook.Enable();
                break;

            default:
                DuoLog.Error("Available combo options: on, off, toggle");
                break;
        }
    }

    private void HandleAutoCommands(string[] argument)
    {
        var toggledVal = !Service.Configuration.RotationConfig.Enabled;
        var newVal = argument.Length > 1
            ? argument[1] == "toggle"
                ? toggledVal
                : argument[1] == "on"
            : toggledVal;

        if (newVal != Service.Configuration.RotationConfig.Enabled)
            ToggleAutoRotation(newVal);
    }

    private static void ToggleAutoRotation(bool value)
    {
        Service.Configuration.RotationConfig.Enabled = value;
        Service.Configuration.Save();

        var stateControlled =
            P.UIHelper.AutoRotationStateControlled() is not null;

        DuoLog.Information(
            "Auto-Rotation set to "
            + (Service.Configuration.RotationConfig.Enabled ? "ON" : "OFF")
            + (stateControlled ? " " + OptionControlledByIPC : "")
        );
    }

    private void HandleIgnoreCommand()
    {
        var target = Svc.Targets.Target;

        if (target == null)
        {
            DuoLog.Error("No target selected");
            return;
        }

        if (!target.IsHostile())
        {
            DuoLog.Error("No valid target selected");
            return;
        }

        if (Service.Configuration.IgnoredNPCs.Any(x => x.Key == target.DataId))
        {
            DuoLog.Error(
                $"{target.Name} (ID: {target.DataId}) is already on the ignored list");
            return;
        }

        if (Service.Configuration.IgnoredNPCs.All(x => x.Key != target.DataId))
        {
            Service.Configuration.IgnoredNPCs.Add(target.DataId, target.GetNameId());

            DuoLog.Information(
                $"Successfully added {target.Name} (ID: {target.DataId}) to ignored list");
        }
    }

    private void HandleDebugCommands(string[] argument)
    {
        try
        {
            ClassJob? job = null;

            // Handle an entered job abbreviation
            if (argument.Length > 1)
            {
                if (argument[1].Length != 3)
                {
                    DuoLog.Error("Invalid job abbreviation");
                    throw new ArgumentException("Invalid job abbreviation");
                }

                if (argument[1] == "all")
                {
                    DebugFile.MakeDebugFile(allJobs: true);
                    return;
                }

                var jobName = argument[1].ToUpperInvariant();
                try
                {
                    // Look up the entered job
                    var jobSearch = Svc.Data.Excel.GetSheet<ClassJob>()
                        .First(j => j.Abbreviation == jobName);
                    var jobId = jobSearch.RowId;
                    // Switch class to job, if necessary
                    if (jobSearch.ClassJobParent.RowId != jobSearch.RowId)
                        jobId =
                            CustomComboFunctions.JobIDs.ClassToJob(jobSearch.RowId);
                    job = Svc.Data.Excel.GetSheet<ClassJob>().GetRow(jobId);
                }
                // the .first() failed
                catch (InvalidOperationException)
                {
                    DuoLog.Error($"Invalid job abbreviation, '{jobName}'");
                    throw;
                }
                // unknown
                catch (Exception ex)
                {
                    DuoLog.Error($"Error looking up job abbreviation, '{jobName}'");
                    Svc.Log.Error(ex, "Debug Log");
                    throw;
                }

                if (job.Value.RowId !=
                    Svc.ClientState.LocalPlayer.ClassJob.Value.RowId)
                    DuoLog.Warning($"You are not on {job.Value.Name}");
            }

            // Request a debug file, with null, or the entered Job
            // (if converted successfully)
            DebugFile.MakeDebugFile(job);
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "Debug Log");
            DuoLog.Error("Unable to write Debug log");
        }
    }

    private void HandleOpenCommand
        (string[]? argument = null, OpenWindow? tab = null, bool? forceOpen = null)
    {
        argument ??= [""];

        ConfigWindow.IsOpen = !ConfigWindow.IsOpen;
        if (forceOpen.HasValue)
            ConfigWindow.IsOpen = forceOpen.Value;

        if (tab is not null)
        {
            ConfigWindow.OpenWindow = tab.Value;
            return;
        }

        if (Service.Configuration.OpenToCurrentJob && Player.Available)
            PvEFeatures.OpenJob = ConfigWindow.groupedPresets
                .FirstOrDefault(x =>
                    x.Value.Any(y => y.Info.JobShorthand == Player.Job.ToString()))
                .Key;

        if (argument[0].Length <= 0) return;

        var jobName = ConfigWindow.groupedPresets
            .FirstOrDefault(x =>
                x.Value.Any(y =>
                    y.Info.JobShorthand == argument[0].ToUpperInvariant()))
            .Key;
        PvEFeatures.OpenJob = jobName;
    }
}
