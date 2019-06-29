# Crab
might as well call it MommIv3 at this point

## About
So yeah this is an attempt to remake [MommIv2](https://github.com/PJB3005/MoMMI) in .Net but better.
It started as a challenge to myself but now it looks like it might actually be something so watch out!

## Features
what it gots
#### Modular Loading
You can reload basically everything but the module loader.
Since the config can be changed at runtime, you could even add new modules (for the dense lads: this means new commands) at runtime without restarting the bot.
#### Config can be changed at runtime
Yeah i mean pretty self-explanatory
Config holds everything from API auth code, admin discord keys to modules and repositories
#### Commands (so far)
- alotta bs testing commands (imma make this list a bit better someday)
- git issues like the original mommer (w/ repo prefixes)
- botcontrol like restart, shutdown, reload [module]

## PLANNED
its brewing...
#### Regex Commands
Right now you need a prefix in front of your command, also the command syntax is very static:
[prefix][command] [param] [param] [...] (thanks discord.net)
#### Porting over functionalities from MommIv2
Basically anything from Reminders to DM code testing. EVERYTHING
#### Expanding Core
- needs_restart attribute for modules
- nuke that ugly logging thing from discord.net and use sawmill or whatever its called
- log more stuff, requires ^
- make core return codes when properly shutdown so head can interpret them
- buy beer

Notes
- when core isn't loaded on init, head still successfully starts the Core class, this might be due to head referencing Core. investigate (if this seems dumb to you please tell my why im a beginner in .net)