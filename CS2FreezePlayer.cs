using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

namespace CS2FreezePlayer
{
    public class FreezePlugin : BasePlugin
    {
        public override string ModuleName => "CS2FreezePlayer";
        public override string ModuleAuthor => "kanoah";
        public override string ModuleDescription => "Adds a command to freeze a player";
        public override string ModuleVersion => "1.0.0";

        public override void Load(bool HotReload)
        {
            Logger.LogInformation("{ModuleName} v{ModuleVersion} by {ModuleAuthor} has been loaded.", ModuleName, ModuleVersion, ModuleAuthor);
        }

        [RequiresPermissions("@css/chat")]
        [ConsoleCommand("css_freeze", "freezes a player. NOW INCLUDING: partial name searching!")]
        [CommandHelper(minArgs: 1, whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        public void OnCommandFreeze(CCSPlayerController? player, CommandInfo info)
        {
            if (player == null)
            {
                return;
            }

            string search = info.ArgString.Trim().ToLower();

            var matches = Utilities.GetPlayers()
                .Where(p => p != null && p.PlayerName.Contains(search, StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            if (matches.Count == 0)
            {
                info.ReplyToCommand($"No players found matching '{search}'.");
                return;
            }

            if (matches.Count > 1)
            {
                info.ReplyToCommand("Multiple matches found:");
                foreach (var m in matches)
                    info.ReplyToCommand($"- {m.PlayerName} [{m.SteamID}]");
                return;
            }

            var target = matches.First();
            var pawn = target.Pawn?.Get();

            if (pawn == null)
            {
                return;
            }

            pawn.MoveType = MoveType_t.MOVETYPE_INVALID;
            Schema.SetSchemaValue(pawn.Handle, "CBaseEntity", "m_nActualMoveType", 11);
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_MoveType");
            info.ReplyToCommand($"Freezed {ChatColors.Gold}{target.PlayerName} {ChatColors.Default}({ChatColors.Green}{target.SteamID}{ChatColors.Default})");
        }

        [RequiresPermissions("@css/chat")]
        [ConsoleCommand("css_unfreeze", "unfreezes a player. NOW INCLUDING: partial name searching!")]
        [CommandHelper(minArgs: 1, whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        public void OnCommandUnFreeze(CCSPlayerController? player, CommandInfo info)
        {
            if (player == null)
            {
                return;
            }

            string search = info.ArgString.Trim().ToLower();

            var matches = Utilities.GetPlayers()
                .Where(p => p != null && p.PlayerName.Contains(search, StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            if (matches.Count == 0)
            {
                info.ReplyToCommand($"No players found matching '{search}'.");
                return;
            }

            if (matches.Count > 1)
            {
                info.ReplyToCommand("Multiple matches found:");
                foreach (var m in matches)
                    info.ReplyToCommand($"- {m.PlayerName} [{m.SteamID}]");
                return;
            }

            var target = matches.First();
            var pawn = target.Pawn?.Get();

            if (pawn == null)
            {
                return;
            }

            pawn.MoveType = MoveType_t.MOVETYPE_WALK;
            Schema.SetSchemaValue(pawn.Handle, "CBaseEntity", "m_nActualMoveType", 2);
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_MoveType");
            info.ReplyToCommand($" Unfreezed {ChatColors.Gold}{target.PlayerName} {ChatColors.Default}[{ChatColors.Green}{target.SteamID}{ChatColors.Default}]");
        }
    }
}
