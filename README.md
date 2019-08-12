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
(its regex)

![lazyman](https://media.discordapp.net/attachments/594515293292724249/610430656182353953/unknown.png)


## PLANNED
its brewing...
#### Porting over functionalities from MoMMIv2
- Basically anything from Reminders to DM code testing. EVERYTHING
#### Expanding Core
- nuke that ugly logging thing from discord.net and use sawmill or whatever its called
- log more stuff, requires ^
- buy beer
- help topics
- make commands be able to tell the commandhandler that they failed
- make commandlist only output what you have permissions for
- permissionstest
#### OnIssue, OnPR, webhooks basically
idunno someday, its fucky weird
