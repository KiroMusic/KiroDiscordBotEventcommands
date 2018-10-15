using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Alpha.Core.Commands.Event_Commands
{
    public class Event_Commands : ModuleBase<SocketCommandContext>
    {
        public static List<ulong> Queue = new List<ulong>();
        public static bool Begin = false;
        public static bool Open = false;
        public static SocketGuildUser CurrentUserPerformer;
        public static bool FirstJoined = false;
        public static IUserMessage TimerMsg;

        [Command("events")]
        public async Task events()
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("**Event Schedule**");
            embed.WithDescription("**Musicord Event Schedule**\n▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬\n**Sunday** - *1PM Open Mic*\n**Monday** - *No Event*\n**Tuesday** - *No Event*\n**Wednesday** - *5PM Open Mic*\n**Thursday** - *No Event*\n**Friday** - *No Event*\n**Saturday** - *5PM Open Mic*\n▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬\n**All of these events are in MST time, 1 hour ahead of CST, 2 hours ahead of EST, and so fourth. Have fun, and remember, respect everyone!!**\n▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬");
            embed.WithColor(new Color(129, 127, 255));
            embed.WithThumbnailUrl(Context.Guild.IconUrl);

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("join")]
        public async Task join()
        {
            if (Begin == true && Open == true)
            {
                if (FirstJoined == false)
                {
                    CurrentUserPerformer = Context.Guild.GetUser(Context.User.Id);
                    var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "CurrentPerformer");
                    Queue.Add(Context.User.Id);
                    await ((SocketGuildUser)Context.User).ModifyAsync(x => x.Mute = false);
                    var embed = new EmbedBuilder();
                    embed.WithTitle("Welcome!");
                    embed.WithDescription($"{Context.User.Mention} Welcome to the queue!");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    embed.WithFooter("Please join with .join. Also, please respect every performer! 🎶");


                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                    FirstJoined = true;
                }
                else if (Queue.Contains(Context.User.Id))
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle("**Oops!**");
                    embed.WithDescription("You're already in the queue silly!");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
    

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
                else
                {
                    Queue.Add(Context.User.Id);
                    SocketGuildUser user = Context.Guild.GetUser(Context.User.Id);
                    var MuteRole = Context.Guild.Roles.FirstOrDefault(x => x.Name == "EventMute");

                    var embed = new EmbedBuilder();
                    embed.WithTitle("Welcome!");
                    embed.WithDescription($"{Context.User.Mention} Welcome to the queue!");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
    

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
            } 
            
            
            if (Begin == true && Open == false)
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("**Sorry!**");
                embed.WithDescription("The queue is closed! Get to the events early to join faster!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            if (Begin == false)
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("**Oops!**");
                embed.WithDescription("There's no event going on right now!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("begin")]
        public async Task HasBegun()
        {
            IReadOnlyCollection<SocketRole> Roles = Context.Guild.GetUser(Context.User.Id).Roles;
            var EventOrg = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Event Organizer");
            var host = Context.Guild.Roles.FirstOrDefault(x => x.Name == "host");
            List<SocketRole> userroles = Roles.ToList();
            if (userroles.Contains(EventOrg) == true)
            {
                if (Begin == true)
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle("**Oops!**");
                    embed.WithDescription("There's already an event going on!");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.Guild.IconUrl);

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
                else
                {
                    Begin = true;
                    

                    var embed = new EmbedBuilder();
                    embed.WithTitle("**Event Started!**");
                    embed.WithDescription("The event has started! Let's have some fun!");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.Guild.IconUrl);

                    await ((SocketGuildUser)Context.User).AddRoleAsync(host);
                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                    {
                        embed.WithTitle("**Event Started!**");
                        embed.WithDescription($"Event started at {DateTime.Now}");
                        embed.WithColor(new Color(129, 127, 255));
                        embed.WithThumbnailUrl(Context.Guild.IconUrl);

                        await Context.User.SendMessageAsync("▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬");
                        await Context.User.SendMessageAsync("", false, embed.Build());
                        await ((SocketGuildUser)Context.User).ModifyAsync(x => x.Nickname = $"[H] {Context.User.Username}");
                    }
                }
            }
            else
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("**Sorry!**");
                embed.WithDescription("You're not an event organizer!!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("end")]
        public async Task end()
        {
            IReadOnlyCollection<SocketRole> Roles = Context.Guild.GetUser(Context.User.Id).Roles;
            var EventOrg = Context.Guild.Roles.FirstOrDefault(x => x.Name == "host");
            List<SocketRole> userroles = Roles.ToList();
            if (userroles.Contains(EventOrg) == true)
            {
                if (Begin == true)
                {
                    Begin = false;
                    Queue.Clear();

                    var embed = new EmbedBuilder();
                    embed.WithTitle("**Ended!**");
                    embed.WithDescription($"Event Ended at {DateTime.Now}");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.Guild.IconUrl);

                    await ((IGuildUser)Context.User).RemoveRoleAsync(EventOrg);
                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                    {
                        embed.WithTitle("**Ended!**");
                        embed.WithDescription($"Event Ended at {DateTime.Now}");
                        embed.WithColor(new Color(129, 127, 255));
                        embed.WithThumbnailUrl(Context.Guild.IconUrl);

                        await Context.User.SendMessageAsync("", false, embed.Build());
                        await ((SocketGuildUser)Context.User).ModifyAsync(x => x.Nickname = Context.User.Username);
                    }
                }
                else
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle("**No Event!**");
                    embed.WithDescription("**There's no event going on :sob:**");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.Guild.IconUrl);

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
            else
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("**Sorry!**");
                embed.WithDescription("You're not the current host!!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("open")]
        public async Task HasOpened()
        {
            IReadOnlyCollection<SocketRole> Roles = Context.Guild.GetUser(Context.User.Id).Roles;
            var EventOrg = Context.Guild.Roles.FirstOrDefault(x => x.Name == "host");
            List<SocketRole> userroles = Roles.ToList();
            if (userroles.Contains(EventOrg) == true)
            {
                if (Begin == true)
                {
                    Open = true;

                    var embed = new EmbedBuilder();
                    embed.WithTitle("**Open!**");
                    embed.WithDescription("The queue is open! Go ahead and join with **``.join``**! :microphone2:");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.Guild.IconUrl);

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
                else
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle("**No Event!**");
                    embed.WithDescription("**There's no event going on :sob:**");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.Guild.IconUrl);

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
            else
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("**Sorry!**");
                embed.WithDescription("You're not the current host!!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("remove")]
        public async Task HasRemoved(IGuildUser user)
        {
            IReadOnlyCollection<SocketRole> Roles = Context.Guild.GetUser(Context.User.Id).Roles;
            var EventOrg = Context.Guild.Roles.FirstOrDefault(x => x.Name == "host");
            List<SocketRole> userroles = Roles.ToList();
            if (userroles.Contains(EventOrg) == true)
            {
                if (Queue.Contains(user.Id))
                {
                    Queue.RemoveAll(r => r == user.Id);

                    var embed = new EmbedBuilder();
                    embed.WithTitle("**Gone!**");
                    embed.WithDescription($"**{user} has been removed from the queue!**");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.Guild.IconUrl);

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
                else
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle("**Nope!**");
                    embed.WithDescription("**This user isn't in the queue!**");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.Guild.IconUrl);

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
            else
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("**Sorry!**");
                embed.WithDescription("You're not the current host!!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("next")]
        public async Task Hasnext()
        {
            IReadOnlyCollection<SocketRole> Roles = Context.Guild.GetUser(Context.User.Id).Roles;
            var EventOrg = Context.Guild.Roles.FirstOrDefault(x => x.Name == "host");
            List<SocketRole> userroles = Roles.ToList();
            if (userroles.Contains(EventOrg) == true)
            {
                if (Begin == true)
                {
                    string QueueMsg = "";
                    int Position = -1;
                    foreach (ulong item in Queue)
                        {
                            SocketGuildUser removinguser = Context.Guild.GetUser(Queue[0]);
                            await removinguser.ModifyAsync(x => x.Mute = true);
                            await ((SocketGuildUser)Context.User).ModifyAsync(x => x.Mute = false);
                            Queue.RemoveAt(0);
                            CurrentUserPerformer = Context.Guild.GetUser(Queue[0]);
                            await CurrentUserPerformer.ModifyAsync(x => x.Mute = false);
                            string mention = "<@!" + CurrentUserPerformer.Id + ">";
                            await Context.Channel.SendMessageAsync($"{CurrentUserPerformer.Mention} is up next! Go show them some love!");
                            await ((SocketGuildUser)Context.User).ModifyAsync(x => x.Nickname = $"[H] {Context.User.Username}");
                    }
                }
                if (Begin == false)
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle("**?**");
                    embed.WithDescription("**There's no event?!**");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.Guild.IconUrl);

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
            else
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("**Sorry!**");
                embed.WithDescription("You're not the current host!!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("queue")]
        public async Task HasQueue()
        {
            IReadOnlyCollection<SocketRole> Roles = Context.Guild.GetUser(Context.User.Id).Roles;
            var EventOrg = Context.Guild.Roles.FirstOrDefault(x => x.Name == "host");
            List<SocketRole> userroles = Roles.ToList();
            if (userroles.Contains(EventOrg) == true)
            {
                if (Open == true || Open == false)
                {
                    string QueueMsg = "";
                    int Position = -1;
                    foreach (ulong item in Queue)
                    {
                        if (Position == -1)
                        {
                            CurrentUserPerformer = Context.Guild.GetUser(item);
                            QueueMsg = QueueMsg + "Current Performer" + "<@!" + CurrentUserPerformer.Id + ">" + "\n\n**CurrentQueue**\n";
                            Position++;
                        }
                        else
                        {
                            Position++;
                            QueueMsg = QueueMsg + "#" + Position + "<@!" + item + ">" + "\n";
                        }

                    }

                    var embed = new EmbedBuilder();
                    embed.WithTitle("**Queue!**");
                    embed.WithDescription($"{QueueMsg}");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    embed.WithFooter("Please join with .join. Also, please respect every performer! 🎶");

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
                else if (Begin == false)
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle("**?**");
                    embed.WithDescription("**There's no event?!**");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.Guild.IconUrl);

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
            else
            {
                string QueueMsg = "";
                int Position = -1;
                foreach (ulong item in Queue)
                {
                    if (Position == -1)
                    {
                        CurrentUserPerformer = Context.Guild.GetUser(item);
                        QueueMsg = QueueMsg + "Current Performer" + "<@!" + CurrentUserPerformer.Id + ">" + "\n\n**CurrentQueue**\n";
                        Position++;
                    }
                    else
                    {
                        Position++;
                        QueueMsg = QueueMsg + "#" + Position + "<@!" + item + ">" + "\n";
                    }

                }

                var embed = new EmbedBuilder();
                embed.WithTitle("**Queue!**");
                embed.WithDescription($"{QueueMsg}");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                embed.WithFooter("Please join with .join. Also, please respect every performer! 🎶");

                await Context.User.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("close")]
        public async Task HasClosed()
        {
            IReadOnlyCollection<SocketRole> Roles = Context.Guild.GetUser(Context.User.Id).Roles;
            var EventOrg = Context.Guild.Roles.FirstOrDefault(x => x.Name == "host");
            List<SocketRole> userroles = Roles.ToList();
            if (userroles.Contains(EventOrg) == true)
            {
                if (Begin == true && Open == true)
                {
                    Open = false;

                    var embed = new EmbedBuilder();
                    embed.WithTitle("**Closed!**");
                    embed.WithDescription("The queue is closed! Go ahead and join another time! :microphone2:");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.Guild.IconUrl);

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
                if (Begin == false)
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle("**No Event!**");
                    embed.WithDescription("There's no event going on :sob:");
                    embed.WithColor(new Color(129, 127, 255));
                    embed.WithThumbnailUrl(Context.Guild.IconUrl);

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
            else
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("**Sorry!**");
                embed.WithDescription("You're not the current host!!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("mute")]
        public async Task mute()
        {
            IReadOnlyCollection<SocketRole> Roles = Context.Guild.GetUser(Context.User.Id).Roles;
            var EventOrg = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Event Organizer");
            var host = Context.Guild.Roles.FirstOrDefault(x => x.Name == "host");
            
            List<SocketRole> userroles = Roles.ToList();
            if (userroles.Contains(EventOrg) == true)
            {
                IReadOnlyCollection<SocketGuildUser> Users = Context.Guild.VoiceChannels.FirstOrDefault(x => x.Name == "Events" || x.Name == "Open Mic" || x.Name == "🏆Events🏆" || x.Name == "Event" || x.Name == "Karaoke" || x.Name == "🎤Open Mic🎤" || x.Name == "Event Voice Chat").Users;
                SocketGuildChannel EventChannel = Context.Guild.VoiceChannels.FirstOrDefault(x => x.Name == "Events" || x.Name == "Open Mic" || x.Name == "🏆Events🏆" || x.Name == "Event" || x.Name == "Karaoke" || x.Name == "🎤Open Mic🎤" || x.Name == "Event Voice Chat");
                List <SocketGuildUser> SocketUsers = Users.ToList();
                foreach (SocketGuildUser user in SocketUsers)
                {
                    new OverwritePermissions(speak: PermValue.Deny);
                    var MuteRole = Context.Guild.Roles.FirstOrDefault(x => x.Name == "EventMute");
                    var EveryoneRole = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Member");
                    await user.ModifyAsync(x => x.Mute = true);
                    await ((SocketGuildUser)Context.User).ModifyAsync(x => x.Mute = false);
                    await EventChannel.AddPermissionOverwriteAsync(EveryoneRole, new OverwritePermissions(speak: PermValue.Deny));
                }
                var embed = new EmbedBuilder();
                embed.WithTitle("**Mute!**");
                embed.WithDescription("Users in the channel have been muted!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.Guild.IconUrl);

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            else
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("**Sorry!**");
                embed.WithDescription("You're not an event organizer!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("unmute")]
        public async Task unmute()
        {
            IReadOnlyCollection<SocketRole> Roles = Context.Guild.GetUser(Context.User.Id).Roles;
            var EventOrg = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Event Organizer");
            List<SocketRole> userroles = Roles.ToList();
            if (userroles.Contains(EventOrg) == true)
            {

                IReadOnlyCollection<SocketGuildUser> Users = Context.Guild.VoiceChannels.FirstOrDefault(x => x.Name == "Open Mic" || x.Name == "Events" || x.Name == "🏆Events🏆" || x.Name == "Event" || x.Name == "Karaoke" || x.Name == "🎤Open Mic🎤" || x.Name == "Event Voice Chat").Users;
                SocketGuildChannel EventChannel = Context.Guild.VoiceChannels.FirstOrDefault(x => x.Name == "Events" || x.Name == "Open Mic" || x.Name == "🏆Events🏆" || x.Name == "Event" || x.Name == "Karaoke" || x.Name == "🎤Open Mic🎤" || x.Name == "Event Voice Chat");
                List <SocketGuildUser> SocketUsers = Users.ToList();
                foreach (SocketGuildUser user in SocketUsers)
                {
                    new OverwritePermissions(speak: PermValue.Allow);
                    var PreformerRole = Context.Guild.Roles.FirstOrDefault(x => x.Name == "CurrentPerformer");
                    var EventMuteRole = Context.Guild.Roles.FirstOrDefault(x => x.Name == "EventMute");
                    var EveryoneRole = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Member");
                    await user.ModifyAsync(x => x.Mute = false);
                    await EventChannel.AddPermissionOverwriteAsync(EveryoneRole, new OverwritePermissions(speak: PermValue.Allow));
                }

                var embed = new EmbedBuilder();
                embed.WithTitle("Yay!");
                embed.WithDescription("Users in the channel have been unmuted!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.Guild.IconUrl);

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            } 
            else
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("**Sorry!**");
                embed.WithDescription("You're not an event organizer!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("grab")]
        public async Task grab(IGuildUser user)
        {
            IReadOnlyCollection<SocketRole> Roles = Context.Guild.GetUser(Context.User.Id).Roles;
            var Staff = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Staff");
            List<SocketRole> userroles = Roles.ToList();
            if (userroles.Contains(Staff) == true)
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("**Grabbed!**");
                embed.WithDescription($"**{user}'s ID is,**\n*{user.Id}*");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(user.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            else
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("**Sorry!**");
                embed.WithDescription("You're not a staff member!!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("eventhelp")]
        public async Task eventhelp()
        {
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("**Event Commands!**");
                embed.WithDescription("▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬\n**``.begin``** - *Begin the event*\n**``.open``** - *Open the queue*\n**``.join``** - *Join the event*\n**``.close``** - *Close the event*\n**``.end``** - *End the event*\n**``.remove [user]``** - *Remove the user from the queue*\n**``.queue``** - *View the queue*\n**``.next``** - *Go to the next user*\n**``.mute``** - *Mute every user in the queue*\n**``.unmute``** - *Unmute every user in the queue*\n**``.grab``** - *Grab the user's id*\n**``.host``** - *Add another user to the host list, (Please only add other event organizers if you need to)*\n**``.hostunmute``** - *Unmutes the host that does the command, (Use only if you're a host and were accidentally muted)*\n**``.removehost``** - *Remove a user from the host list*\n▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.Guild.IconUrl);

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }

        }

        [Command("time")]
        public async Task timer(int time, string amount)
        {
            IReadOnlyCollection<SocketRole> Roles = Context.Guild.GetUser(Context.User.Id).Roles;
            var EventOrg = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Event Organizer");
            List<SocketRole> userroles = Roles.ToList();
            if (userroles.Contains(EventOrg) == true)
            {
                {
                    if (amount == "minute" || amount == "m" || amount == "minutes" || amount == "min" || amount == "mins")
                    {
                        time = (int)TimeSpan.FromMinutes(time).TotalMilliseconds;
                    }
                    if (amount == "second" || amount == "s" || amount == "seconds" || amount == "sec" || amount == "secs")
                    {
                        time = (int)TimeSpan.FromSeconds(time).TotalMilliseconds;
                    }
                    if (amount == "hour" || amount == "h" || amount == "hours" || amount == "hr" || amount == "hrs")
                    {
                        time = (int)TimeSpan.FromHours(time).TotalMilliseconds;
                    }
                    System.Timers.Timer aTimer = new System.Timers.Timer();
                    aTimer.Interval = time;
                    aTimer.Enabled = true;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    TimerMsg = await Context.Channel.SendMessageAsync("Timer Has Started!");
                    while (aTimer.Enabled == true)
                    {
                        await TimerMsg.ModifyAsync(x =>
                        {
                            TimeSpan ts = stopwatch.Elapsed;
                            string sTime = string.Format("Time Elapsed: {0}h {1}m {2}s", ts.Hours, ts.Minutes, ts.Seconds);
                            x.Content = sTime;
                        });
                    }
                }
            }
            else
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("**Sorry!**");
                embed.WithDescription("You're not an event organizer!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }

        }

        [Command("host")]
        public async Task host(IGuildUser user)
        {
            IReadOnlyCollection<SocketRole> Roles = Context.Guild.GetUser(Context.User.Id).Roles;
            var host = Context.Guild.Roles.FirstOrDefault(x => x.Name == "host");
            List<SocketRole> userroles = Roles.ToList();
            if (userroles.Contains(host) == true)
            {
                await user.AddRoleAsync(host);

                var embed = new EmbedBuilder();
                embed.WithTitle("**Congrats!**");
                embed.WithDescription($"{user.Mention} is now a host! Welcome!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
                await user.ModifyAsync(x => x.Nickname = $"[H] {user.Username}");
            }
            if (userroles.Contains(host) == false)
            {
                var embed = new EmbedBuilder();

                embed.WithTitle("**Oops!**");
                embed.WithDescription($"Sorry! You can't make {user.Mention} a host beacuse you're not the current host!!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("hostunmute")]
        public async Task hostunmute()
        {
            IReadOnlyCollection<SocketRole> Roles = Context.Guild.GetUser(Context.User.Id).Roles;
            var host = Context.Guild.Roles.FirstOrDefault(x => x.Name == "host");
            List<SocketRole> userroles = Roles.ToList();
            if (userroles.Contains(host) == true)
            {
                await ((SocketGuildUser)Context.User).ModifyAsync(x => x.Mute = false);

                var embed = new EmbedBuilder();
                embed.WithTitle("**Congrats!**");
                embed.WithDescription($"You're unmuted!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            if (userroles.Contains(host) == false)
            {
                var embed = new EmbedBuilder();

                embed.WithTitle("**Oops!**");
                embed.WithDescription($"Sorry! You don't have the permission to unmute yourself! You're not a host!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("removehost")]
        public async Task nothost(IGuildUser user)
        {
            IReadOnlyCollection<SocketRole> Roles = Context.Guild.GetUser(Context.User.Id).Roles;
            var host = Context.Guild.Roles.FirstOrDefault(x => x.Name == "host");
            List<SocketRole> userroles = Roles.ToList();
            if (userroles.Contains(host) == true)
            {
                await user.RemoveRoleAsync(host);

                var embed = new EmbedBuilder();
                embed.WithTitle("**Awww!**");
                embed.WithDescription($"{user.Mention} is not a host anymore :(");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            if (userroles.Contains(host) == false)
            {
                var embed = new EmbedBuilder();

                embed.WithTitle("**Oops!**");
                embed.WithDescription($"Sorry! You can't take {user.Mention}'s host perms away because you're not the current host!!");
                embed.WithColor(new Color(129, 127, 255));
                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

    } 
}
