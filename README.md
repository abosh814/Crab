# Crab
might as well call it MoMMIv3 at this point

## About
So yeah this is an attempt to remake [MoMMIv2](https://github.com/PJB3005/MoMMI) in .Net but better.
It started as a challenge to myself but now it looks like it might actually be something so watch out!

## Features
what it gots
#### Modular Loading
You can reload basically everything but the module loader.
Since the config can be changed at runtime, you could even add entire new modules at runtime without restarting the bot.
#### Config can be changed at runtime
Yeah i mean pretty self-explanatory
Config holds everything from API auth code, admin discord keys to modules and repositories
Is still json rn, but that can change
#### Commands (so far)
- alotta bs testing commands (imma make this list a bit better someday)
- git issues like the original mommer (w/ repo prefixes)
- botcontrol like restart, shutdown, reload [module]
- when?
#### Regex Commands
yeah i gottem

## PLANNED
its brewing...
#### Porting over functionalities from MoMMIv2
- Basically anything from Reminders to DM code testing. EVERYTHING
- help command
#### Expanding Core
- Making every module an instance so it can handle its own data and can be (de)serialized when loading/unloading
- nuke that ugly logging thing from discord.net and use sawmill or whatever its called
- log more stuff, requires ^
- buy beer
#### OnIssue, OnPR, webhooks basically
idunno someday, its fucky weird