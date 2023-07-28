using PKHeX.Core;
using RaidCrawler.Core.Interfaces;
using RaidCrawler.Core.Structures;
using SysBot.Base;
using System.Globalization;
using System.Text.Json;
using System.Text;
using System.Runtime.InteropServices.ObjectiveC;
using static System.Net.WebRequestMethods;

namespace RaidCrawler.Core.Discord
{
    public class NotificationHandler
    {
        private readonly HttpClient _client;
        private readonly string[]? DiscordWebhooks;
        private readonly string DiscordMessageContent;
        private readonly IWebhookConfig Config;

        public NotificationHandler(IWebhookConfig config)
        {
            _client = new();
            Config = config;
            DiscordWebhooks = config.EnableNotification ? config.DiscordWebhook.Split(',') : null;
            DiscordMessageContent = config.DiscordMessageContent;
        }

        public NotificationHandler(IWebhookConfig config, bool fomo)
        {
            _client = new();
            Config = config;
            DiscordWebhooks = config.EnableFomoNotification ? config.DiscordFomoWebhook.Split(',') : null;
            DiscordMessageContent = string.Empty;
        }

        public async Task SendNotification(ITeraRaid encounter, Raid raid, RaidFilter filter, string time, IReadOnlyList<(int, int, int)> RewardsList, string hexColor, string spriteName, CancellationToken token)
        {
            if (DiscordWebhooks is null || !Config.EnableNotification)
                return;

            var webhook = GenerateWebhook(encounter, raid, filter, time, RewardsList, hexColor, spriteName, "webhook");
            var content = new StringContent(JsonSerializer.Serialize(webhook), Encoding.UTF8, "application/json");
            foreach (var url in DiscordWebhooks)
                await _client.PostAsync(url.Trim(), content, token).ConfigureAwait(false);
        }

        public async Task SendFomoNotification(ITeraRaid encounter, Raid raid, RaidFilter filter, string time, IReadOnlyList<(int, int, int)> RewardsList, string hexColor, string spriteName, CancellationToken token)
        {
            if (DiscordWebhooks is null || !Config.EnableFomoNotification)
                return;

            var webhook = GenerateWebhook(encounter, raid, filter, time, RewardsList, hexColor, spriteName, "webhook");
            var content = new StringContent(JsonSerializer.Serialize(webhook), Encoding.UTF8, "application/json");
            foreach (var url in DiscordWebhooks)
                await _client.PostAsync(url.Trim(), content, token).ConfigureAwait(false);
        }

        public async Task SendErrorNotification(string error, string caption, CancellationToken token)
        {
            if (DiscordWebhooks is null || !Config.EnableNotification)
                return;

            var instance = Config.InstanceName != "" ? $"RaidCrawler{Config.InstanceName}" : "RaidCrawler";
            var webhook = new
            {
                username = instance,
                avatar_url = "https://www.serebii.net/scarletviolet/ribbons/mightiestmark.png",
                content = DiscordMessageContent,
                embeds = new List<object>
                {
                    new
                    {
                        title = caption != "" ? caption : "Error",
                        description = error,
                        color = 0xff4f4e,
                        thumbnail = new
                            {
                                url = $"https://cdn.discordapp.com/emojis/1065868106360160346.png?v=1"
                            },
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(webhook), Encoding.UTF8, "application/json");
            foreach (var url in DiscordWebhooks)
                await _client.PostAsync(url.Trim(), content, token).ConfigureAwait(false);
        }

        public async Task SendScreenshot(ISwitchConnectionAsync nx, CancellationToken token)
        {
            if (DiscordWebhooks is null || !Config.EnableNotification)
                return;

            var data = await nx.PixelPeek(token).ConfigureAwait(false);
            var content = new MultipartFormDataContent();
            var info = new
            {
                username = "RaidCrawler",
                avatar_url = "https://www.serebii.net/scarletviolet/ribbons/mightiestmark.png",
                content = "Switch Screenshot",
            };

            var basic_info = new StringContent(JsonSerializer.Serialize(info), System.Text.Encoding.UTF8, "application/json");
            content.Add(basic_info, "payload_json");
            content.Add(new ByteArrayContent(data), "screenshot.jpg", "screenshot.jpg");
            foreach (var url in DiscordWebhooks)
                await _client.PostAsync(url.Trim(), content, token).ConfigureAwait(false);
        }

        public string GetAnnouncement(ITeraRaid encounter, Raid raid, RaidFilter filter, string time, IReadOnlyList<(int, int, int)> RewardsList, string hexColor, string spriteName, string eventtype) 
        {
            var announcement = GenerateWebhook(encounter, raid, filter, time, RewardsList, hexColor, spriteName, eventtype);
            return announcement.ToString();
        }

        private object GenerateWebhook(ITeraRaid encounter, Raid raid, RaidFilter filter, string time, IReadOnlyList<(int, int, int)> rewardsList, string hexColor, string spriteName, string eventtype)
        {
            var strings = GameInfo.GetStrings(1);
            var param = encounter.GetParam();
            var blank = new PK9
            {
                Species = encounter.Species,
                Form = encounter.Form
            };

            Encounter9RNG.GenerateData(blank, param, EncounterCriteria.Unrestricted, raid.Seed);
            var form = Utils.GetFormString(blank.Species, blank.Form, strings);
            var species = $"{strings.Species[encounter.Species]}";
            var rarevariant = $"{(raid.EC % 100 == 0 && (encounter!.Species == 924 || encounter.Species == 206) ? " Rare Variant" : "" )}";
            var difficulty = Difficulty(encounter.Stars, raid.IsEvent, eventtype);
            var nature = $"{strings.Natures[blank.Nature]}";
            var ability = $"{strings.Ability[blank.Ability]}";
            var shiny = Shiny(raid.CheckIsShiny(encounter), ShinyExtensions.IsSquareShinyExist(blank), eventtype);
            var gender = GenderEmoji(blank.Gender, eventtype);
            var teratype = raid.GetTeraType(encounter);
            var tera = $"{strings.types[teratype]}";
            var teraemoji = TeraEmoji(strings.types[teratype], eventtype);
            var ivs = IVsStringEmoji(ToSpeedLast(blank.IVs), eventtype);
            var perfectIvCount = blank.IVs.Count(iv => iv == 31);
            var moves = new ushort[4] { encounter.Move1, encounter.Move2, encounter.Move3, encounter.Move4 };
            var movestr = string.Concat(moves.Where(z => z != 0).Select(z => $"{strings.Move[z]}ㅤ\n")).Trim();
            var extramoves = string.Concat(encounter.ExtraMoves.Where(z => z != 0).Select(z => $"{strings.Move[z]}ㅤ\n")).Trim();
            var extramovesstr = extramoves == string.Empty ? "None" : extramoves;
            var area = $"{Areas.GetArea((int)(raid.Area - 1))}" + (Config.ToggleDen ? $" [Den {raid.Den}]ㅤ" : "ㅤ");
            var rewards = GetRewards(rewardsList, eventtype);
            var scale = blank.Scale;
            var copy = $"{difficulty} {shiny} **{species}{form}** {gender} **{teraemoji}** `{PokeSizeDetailedUtil.GetSizeRating(scale)} ({scale})`\n" +
                           $"**__{perfectIvCount}__IV**: {ivs}  **Nature:** `{nature}`  **Ability:** `{ability}`\n" +
                           $"**Moves:** \n{movestr}\n" +
                           $"{(extramoves == "" ? "" : $"**Extra Moves:** \n{extramovesstr}\n")}" +
                           $"{(rewards != "" ? $"**Rewards:** {rewards}\n :" : "")}" +
                           $"***Code:*** ";
            var technicalcopy = $"{encounter.Stars},{shiny},{species},{blank.Form},{blank.Gender},{teratype},{nature},{ability},{blank.Scale},{IVsStringEmoji(ToSpeedLast(blank.IVs), "technicalcopy")}";
            var SuccessWebHook = new
            {
                username = "RaidCrawler " + Config.InstanceName,
                avatar_url = "https://www.serebii.net/scarletviolet/ribbons/mightiestmark.png",
                content = DiscordMessageContent,
                embeds = new List<object>
                {
                    new
                    {
                        title = $"{shiny} {species}{form}{rarevariant} {gender} {teraemoji}",
                        description = "",
                        color = int.Parse(hexColor, NumberStyles.HexNumber),
                        thumbnail = new
                        {
                            url = $"https://github.com/kwsch/PKHeX/blob/master/PKHeX.Drawing.PokeSprite/Resources/img/Artwork%20Pokemon%20Sprites/a{spriteName}.png?raw=true"
                            //url = raid.CheckIsShiny(encounter) ? $"https://github.com/ViolentSpatula/PokeSprite/blob/main/SmallShiny/{encounter.Species-1}{(encounter.Form != 0 ? $"-{encounter.Form}" : "")}.gif?raw=true" : $"https://github.com/kwsch/PKHeX/blob/master/PKHeX.Drawing.PokeSprite/Resources/img/Artwork%20Pokemon%20Sprites/a{spriteName}.png?raw=true"
                        },
                        fields = new List<object>
                        {
                            new { name = "Difficultyㅤㅤㅤㅤㅤㅤ", value = difficulty, inline = true, },
                            new { name = "Natureㅤㅤㅤ", value = nature, inline = true },
                            new { name = "Ability", value = ability, inline = true, },

                            new { name = "IVs", value = ivs, inline = true, },
                            new { name = "Moves", value = movestr, inline = true, },
                            new { name = "Extra Moves", value = extramoves == string.Empty ? "None" : extramoves, inline = true, },

                            new { name = "Location󠀠󠀠󠀠", value = area, inline = true, },
                            new { name = "Search Time󠀠󠀠󠀠", value = time, inline = true, },
                            new { name = "Filter Name", value = filter.Name, inline = true, },

                            new { name = rewards != "" ? "Rewards" : "", value = rewards, inline = true, },
                            new { name = "", value = "", inline = true, },
                            new { name = "Size", value = $"{PokeSizeDetailedUtil.GetSizeRating(scale)} ({scale})", inline = true, },
                            ///new { name = "Species", value = encounter.Species, inline = true, },
                            //new { name = "# CopyTest", value = copy, inline = false, },
                            //new { name = "# TechnicalCopyTest", value = technicalcopy, inline = false, },
                        },
                    }
                }
            };
            return (eventtype == "webhook" ? SuccessWebHook : eventtype == "copy" ? copy : technicalcopy);
            //return SuccessWebHook;
        }

        private string Difficulty(byte stars, bool isEvent, string eventtype)
        {
            bool enable = eventtype == "webhook" ? Config.EnableEmoji : eventtype == "copy" ? Config.CopyEmoji : false;
            string emoji = !enable ? ":star:"
                                   : stars == 7 ? Config.Emoji["7 Star"]
                                   : isEvent ? Config.Emoji["Event Star"]
                                   : Config.Emoji["Star"];

            return string.Concat(Enumerable.Repeat(emoji, stars));
        }

        private string GenderEmoji(int genderInt, string eventtype)
        {
            string gender = string.Empty;
            bool emoji = eventtype == "webhook" ? Config.EnableEmoji : eventtype == "copy" ? Config.CopyEmoji : false;
            switch (genderInt)
            {
                case 0: gender = (emoji ? Config.Emoji["Male"] : "♂"); break;
                case 1: gender = (emoji ? Config.Emoji["Female"] : "♀"); break;
                case 2: gender = ""; break;
            }
            return gender;
        }

        private string GetRewards(IReadOnlyList<(int, int, int)> rewards, string eventtype)
        {
            string s = string.Empty;
            int abilitycapsule = 0;
            int bottlecap = 0;
            int abilitypatch = 0;
            int sweetherba = 0;
            int saltyherba = 0;
            int sourherba = 0;
            int bitterherba = 0;
            int spicyherba = 0;

            for (int i = 0; i < rewards.Count; i++)
            {
                switch (rewards[i].Item1)
                {
                    case 0645: abilitycapsule++; break;
                    case 0795: bottlecap++; break;
                    case 1606: abilitypatch++; break;
                    case 1904: sweetherba++; break;
                    case 1905: saltyherba++; break;
                    case 1906: sourherba++; break;
                    case 1907: bitterherba++; break;
                    case 1908: spicyherba++; break;
                }
            }

            bool emoji = eventtype == "webhook" ? Config.EnableEmoji : eventtype == "copy" ? Config.CopyEmoji : false;
            s += (abilitycapsule > 0) ? (emoji ? $"`{abilitycapsule}`{Config.Emoji["Ability Capsule"]} " : $"`{abilitycapsule}` Ability Capsule  ") : "";
            s += (bottlecap > 0) ? (emoji ? $"`{bottlecap}`{Config.Emoji["Bottle Cap"]} " : $"`{bottlecap}` Bottle Cap  ") : "";
            s += (abilitypatch > 0) ? (emoji ? $"`{abilitypatch}`{Config.Emoji["Ability Patch"]} " : $"`{abilitypatch}` Ability Patch  ") : "";
            s += (sweetherba > 0) ? (emoji ? $"`{sweetherba}`{Config.Emoji["Sweet Herba"]} " : $"`{sweetherba}` Sweet Herba  ") : "";
            s += (saltyherba > 0) ? (emoji ? $"`{saltyherba}`{Config.Emoji["Salty Herba"]} " : $"`{saltyherba}` Salty Herba  ") : "";
            s += (sourherba > 0) ? (emoji ? $"`{sourherba}`{Config.Emoji["Sour Herba"]} " : $"`{sourherba}` Sour Herba  ") : "";
            s += (bitterherba > 0) ? (emoji ? $"`{bitterherba}`{Config.Emoji["Bitter Herba"]} " : $"`{bitterherba}` Bitter Herba  ") : "";
            s += (spicyherba > 0) ? (emoji ? $"`{spicyherba}`{Config.Emoji["Spicy Herba"]} " : $"`{spicyherba}` Spicy Herba  ") : "";

            return s;
        }

        private string IVsStringEmoji(int[] ivs, string eventtype)
        {
            string s = string.Empty;
            string spacer = eventtype == "technicalcopy" ? "," : Config.IVsSpacer;
            bool emoji = eventtype == "webhook" ? Config.EnableEmoji : eventtype == "copy" ? Config.CopyEmoji : false;
            bool verbose = eventtype == "technicalcopy" ? false : Config.VerboseIVs;
            int IVsStyle = eventtype == "technicalcopy" ? 2 : Config.IVsStyle;
            var stats = new[] { "HP", "Atk", "Def", "SpA", "SpD", "Spe" };
            var iv0 = new[] { Config.Emoji["Health 0"], Config.Emoji["Attack 0"], Config.Emoji["Defense 0"], Config.Emoji["SpAttack 0"], Config.Emoji["SpDefense 0"], Config.Emoji["Speed 0"] };
            var iv31 = new[] { Config.Emoji["Health 31"], Config.Emoji["Attack 31"], Config.Emoji["Defense 31"], Config.Emoji["SpAttack 31"], Config.Emoji["SpDefense 31"], Config.Emoji["Speed 31"] };
            for (int i = 0; i < ivs.Length; i++)
            {
                switch (IVsStyle)
                {
                    case 0:
                        {
                            s += ivs[i] switch
                            {
                                0 => emoji ? $"{iv0[i]:D}{(verbose ? " " + stats[i] : string.Empty)}" : $"`{"✓":D}`{(verbose ? " " + stats[i] : string.Empty)}",
                                31 => emoji ? $"{iv31[i]:D}{(verbose ? " " + stats[i] : string.Empty)}" : $"`{"✓":D}`{(verbose ? " " + stats[i] : string.Empty)}",
                                _ => $"`{ivs[i]:D}`{(verbose ? " " + stats[i] : string.Empty)}",
                            };

                            if (i < 5)
                                s += spacer;
                            break;
                        }
                    case 1:
                        {
                            s += $"`{ivs[i]:D}`{(verbose ? " " + stats[i] : string.Empty)}";
                            if (i < 5)
                                s += spacer;
                            break;
                        }
                    case 2:
                        {
                            s += $"{ivs[i]:D}{(verbose ? " " + stats[i] : string.Empty)}";
                            if (i < 5)
                                s += spacer;
                            break;
                        }
                }
            }
            return s;
        }

        private string Shiny(bool shiny, bool square, string eventtype)
        {
            bool emoji = eventtype == "webhook" ? Config.EnableEmoji : eventtype == "copy" ? Config.CopyEmoji : false;
            string s = string.Empty;
            if (square && shiny)
                s = $"{(emoji ? Config.Emoji["Square Shiny"] : eventtype == "technicalcopy" ? 2 : "Square shiny")}";
            else if (shiny)
                s = $"{(emoji ? Config.Emoji["Shiny"] : eventtype == "technicalcopy" ? 1 : "Shiny")}";
            else
                s = $"{(eventtype == "technicalcopy" ? 0 : "")}";

            return s;
        }

        private static int[] ToSpeedLast(int[] ivs)
        {
            var res = new int[6];
            res[0] = ivs[0];
            res[1] = ivs[1];
            res[2] = ivs[2];
            res[3] = ivs[4];
            res[4] = ivs[5];
            res[5] = ivs[3];
            return res;
        }

        //private string TeraEmoji(string tera) => Config.EnableEmoji ? Config.Emoji[tera] : tera;
        private string TeraEmoji(string tera, string eventtype) => eventtype == "webhook" ? (Config.EnableEmoji ? Config.Emoji[tera] : tera) : eventtype == "copy" ? (Config.CopyEmoji ? Config.Emoji[tera] : tera) : tera;

    }
}
