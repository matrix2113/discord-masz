import os

from discord.ext import commands
from discord import Embed

from data import get_guildconfig

CHECK = "✅"
X_CHECK = "❌"

@commands.command(help="Checks if further configuration is needed to use MASZ features.")
async def features(ctx):
    if ctx.guild is None:
        await ctx.send("Only useable in a guild.")
        return
    try:
        cfg = await get_guildconfig(str(ctx.guild.id))
    except Exception as e:
        print(e)
        await ctx.send("Failed to fetch data from database.")
        return
    
    if cfg is None:
        if os.getenv('META_SERVICE_BASE_URL', ''):
            await ctx.send(f"MASZ is not registered on this guild.\nA siteadmin can register this guild at: {os.getenv('META_SERVICE_BASE_URL', '')}/guilds/new?guildid={ctx.guild.id}")
        else:
            await ctx.send(f"MASZ is not registered on this guild.")
        return
    
    muted_role_defined = cfg["MutedRoleId"]
    if muted_role_defined:
        muted_role = ctx.guild.get_role(int(muted_role_defined))
    else:
        muted_role = None
    muted_role_managable = muted_role is not None and ctx.me.top_role > muted_role

    permissions = ctx.me.guild_permissions
    kick = permissions.kick_members
    ban = permissions.ban_members
    mute = permissions.manage_roles

    embed = Embed(title="Features")
    missing_permissions = f"\n- {CHECK if kick else X_CHECK} Kick permission {'not' if not kick else ''} granted" \
                          f"\n- {CHECK if ban else X_CHECK} Ban permission {'not' if not ban else ''} granted" \
                          f"\n- {CHECK if mute else X_CHECK} Manage role permission {'not' if not mute else ''} granted"

    if not muted_role_defined:
        missing_permissions += f"\n- {X_CHECK} Muted role not defined"
    else:
        if muted_role:
            if muted_role_managable:
                missing_permissions += f"\n- {CHECK} Muted role defined"
            else:
                missing_permissions += f"\n- {X_CHECK} Muted role defined but too high in role hierarchy"
        else:
            missing_permissions += f"\n- {X_CHECK} Muted role defined but invalid"

    embed.add_field(
        name = f"{CHECK if kick and ban and mute and muted_role and muted_role_managable else X_CHECK} Punishment feature",
        value = f"Register and manage punishments (e.g. tempbans, mutes, etc.).{missing_permissions if not (kick and ban and mute and muted_role and muted_role_managable) else ''}",
        inline=False
    )
    embed.add_field(
        name = f"{CHECK if ban else X_CHECK} Unban feature",
        value = "Allows banned members to see their cases and comment on it for unban requests.\n" +
                f"{'Grant this bot the ban permission to use this feature.' if not ban else ''}",
        inline=False
    )

    if kick and ban and mute and muted_role and muted_role_managable:
        embed.description = f"{CHECK} Your bot on this guild is configured correctly. All features of MASZ can be used."
        embed.color = 0x07eb0b
    else:
        embed.description = f"{X_CHECK} There are features of MASZ that you cannot use right now."
        embed.color = 0xf71b02

    await ctx.send(embed=embed)
