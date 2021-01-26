import discord
import os 

from discord.ext import commands

default_prefix = "$"

async def get_prefix(ctx):
    if ctx.guild == None:
        return default_prefix
    else:
        prefix = os.getenv("BOT_PREFIX", "$")
        if str(prefix).strip() != "":
            return "$"
        else:
            return "$"
