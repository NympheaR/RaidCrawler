using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using NLog.Filters;
using PKHeX.Core;
using PKHeX.Drawing;
using PKHeX.Drawing.Misc.Properties;
using PKHeX.Drawing.PokeSprite;
using RaidCrawler.Core.Connection;
using RaidCrawler.Core.Discord;
using RaidCrawler.Core.Structures;
using RaidCrawler.WinForms.SubForms;
using SysBot.Base;
using System;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;
using System.Xml.Linq;
using static RaidCrawler.Core.Structures.Offsets;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ScrollBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace RaidCrawler.WinForms
{
    public partial class MainWindow : Form
    {
        private static CancellationTokenSource Source = new();
        private static CancellationTokenSource DateAdvanceSource = new();

        private static readonly object _connectLock = new();
        private static readonly object _readLock = new();

        private readonly ClientConfig Config = new();
        private ConnectionWrapperAsync ConnectionWrapper = default!;
        private readonly SwitchConnectionConfig ConnectionConfig = new()
        { Protocol = SwitchProtocol.WiFi, IP = "192.168.0.0", Port = 6000 };

        private readonly RaidContainer RaidContainer;
        private NotificationHandler Webhook;
        private NotificationHandler FomoWebhook;

        private List<RaidFilter> RaidFilters = new();
        private static readonly Image map = Image.FromStream(new MemoryStream(Utils.GetBinaryResource("paldea.png")));
        private static Dictionary<string, float[]>? den_locations;

        // statistics
        public int StatDaySkipTries = 0;
        public int StatDaySkipSuccess = 0;
        public int StatDaySkipStreak = 0;
        public int StatShinyCount = 0;
        public string formTitle;
        public List<string> Fomo = new();
        public int FomoCount = 0;

        private ulong RaidBlockOffset = 0;
        private bool IsReading = false;
        private bool HideSeed = false;
        private bool ShowExtraMoves = false;

        private Color DefaultColor;
        private FormWindowState _WindowState;
        private readonly Stopwatch stopwatch = new();
        private TeraRaidView? teraRaidView;

        private bool StopAdvances => !Config.EnableFilters || RaidFilters.Count == 0 || RaidFilters.All(x => !x.Enabled);

        public MainWindow()
        {
            string build = string.Empty;
#if DEBUG
            var date = File.GetLastWriteTime(AppContext.BaseDirectory);
            build = $" (dev-{date:yyyyMMdd})";
#endif
            var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!;
            var filterpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "filters.json");
            if (File.Exists(filterpath))
                RaidFilters = JsonSerializer.Deserialize<List<RaidFilter>>(File.ReadAllText(filterpath)) ?? new List<RaidFilter>();
            den_locations = JsonSerializer.Deserialize<Dictionary<string, float[]>>(Utils.GetStringResource("den_locations.json") ?? "{}");

            var configpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            if (File.Exists(configpath))
            {
                var text = File.ReadAllText(configpath);
                Config = JsonSerializer.Deserialize<ClientConfig>(text)!;
            }
            else Config = new();

            formTitle = "RaidCrawlerV v" + v.Major + "." + v.Minor + "." + v.Build + build + " " + Config.InstanceName;
            Text = formTitle;

            // load raids
            RaidContainer = new(Config.Game);

            SpriteBuilder.ShowTeraThicknessStripe = 0x4;
            SpriteBuilder.ShowTeraOpacityStripe = 0xAF;
            SpriteBuilder.ShowTeraOpacityBackground = 0xFF;
            SpriteUtil.ChangeMode(SpriteBuilderMode.SpritesArtwork5668);

            var protocol = Config.Protocol;
            ConnectionConfig = new()
            {
                IP = Config.IP,
                Port = protocol is SwitchProtocol.WiFi ? 6000 : Config.UsbPort,
                Protocol = Config.Protocol,
            };

            Webhook = new(Config);
            FomoWebhook = new(Config, true);
            InitializeComponent();

            btnOpenMap.Enabled = false;
            Rewards.Enabled = false;
            CopyAnnounce.Enabled = false;
            SendScreenshot.Enabled = false;
            ButtonScreenState.Enabled = false;
            CheckEnableFilters.Checked = Config.EnableFilters;

            if (Config.Protocol is SwitchProtocol.USB)
            {
                InputSwitchIP.Visible = false;
                LabelSwitchIP.Visible = false;
                USB_Port_TB.Visible = true;
                USB_Port_label.Visible = true;
            }
            else
            {
                InputSwitchIP.Visible = true;
                LabelSwitchIP.Visible = true;
                USB_Port_TB.Visible = false;
                USB_Port_label.Visible = false;
            }
        }

        private void UpdateStatus(string status)
        {
            ToolStripStatusLabel.Text = status;
        }

        private void ButtonEnable(object[] obj, bool enable)
        {
            lock (_readLock)
            {
                for (int b = 0; b < obj.Length; b++)
                {
                    if (obj[b] is not Button btn)
                        continue;

                    if (InvokeRequired)
                        Invoke(() => { btn.Enabled = enable; });
                    else btn.Enabled = enable;
                }

                IsReading = !enable;
            }
        }

        private void ShowDialog(object obj)
        {
            var window = (Form)obj;
            if (window is null)
                return;

            window.StartPosition = FormStartPosition.CenterParent;
            if (InvokeRequired)
                Invoke(() => { window.ShowDialog(); });
            else window.ShowDialog();
        }

        private int GetRaidBoost()
        {
            if (InvokeRequired)
                return Invoke(() => { return RaidBoost.SelectedIndex; });
            return RaidBoost.SelectedIndex;
        }

        public int GetStatDaySkipTries() => StatDaySkipTries;
        public int GetStatDaySkipSuccess() => StatDaySkipSuccess;
        public int GetStatDaySkipStreak() => StatDaySkipStreak;
        public int GetStatShinyCount() => StatShinyCount;
        public List<string> GetFomo() => Fomo;

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Location = Config.Location;
            if (Location.X == 0 && Location.Y == 0)
                CenterToScreen();
            InputSwitchIP.Text = Config.IP;
            Protocol_dropdown.SelectedIndex = (int)Config.Protocol;
            USB_Port_TB.Text = Config.UsbPort.ToString();
            DefaultColor = IVs.BackColor;
            RaidBoost.SelectedIndex = 0;
            ToggleStreamerView();
        }

        private void InputSwitchIP_Changed(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Config.IP = textBox.Text;
            ConnectionConfig.IP = textBox.Text;
        }

        private void USB_Port_Changed(object sender, EventArgs e)
        {
            if (Config.Protocol is SwitchProtocol.WiFi)
                return;

            TextBox textBox = (TextBox)sender;
            if (int.TryParse(textBox.Text, out int port) && port >= 0)
            {
                Config.UsbPort = port;
                ConnectionConfig.Port = port;
                return;
            }

            Task.Run(async () => await ErrorHandler.DisplayMessageBox(this, Webhook, "Please enter a valid numerical USB port.", Source.Token).ConfigureAwait(false), Source.Token);
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            lock (_connectLock)
            {
                if (ConnectionWrapper is not null && ConnectionWrapper.Connected)
                    return;

                ConnectionWrapper = new(ConnectionConfig, UpdateStatus);
                Connect(Source.Token);
            }
        }

        private void Connect(CancellationToken token)
        {
            Task.Run(async () =>
            {
                ButtonEnable(new[] { ButtonConnect, SendScreenshot, btnOpenMap, Rewards, CopyAnnounce, ButtonScreenState }, false);
                try
                {
                    (bool success, string err) = await ConnectionWrapper.Connect(token).ConfigureAwait(false);
                    if (!success)
                    {
                        ButtonEnable(new[] { ButtonConnect }, true);
                        await ErrorHandler.DisplayMessageBox(this, Webhook, err, token).ConfigureAwait(false);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ButtonEnable(new[] { ButtonConnect }, true);
                    await ErrorHandler.DisplayMessageBox(this, Webhook, ex.Message, token).ConfigureAwait(false);
                    return;
                }

                UpdateStatus("Detecting game version");
                string id = await ConnectionWrapper.Connection.GetTitleID(token).ConfigureAwait(false);
                var game = id switch
                {
                    ScarletID => "Scarlet",
                    VioletID => "Violet",
                    _ => "",
                };

                if (game is "")
                {
                    try
                    {
                        (bool success, string err) = await ConnectionWrapper.DisconnectAsync(token).ConfigureAwait(false);
                        if (!success)
                        {
                            ButtonEnable(new[] { ButtonConnect }, true);
                            await ErrorHandler.DisplayMessageBox(this, Webhook, err, token).ConfigureAwait(false);
                            return;
                        }
                    }
                    catch { }
                    finally
                    {
                        ButtonEnable(new[] { ButtonConnect }, true);
                        await ErrorHandler.DisplayMessageBox(this, Webhook, "Unable to detect Pokémon Scarlet or Pokémon Violet running on your Switch", token).ConfigureAwait(false);
                    }
                    return;
                }

                Config.Game = game;
                RaidContainer.SetGame(Config.Game);

                UpdateStatus("Reading story progress");
                Config.Progress = await ConnectionWrapper.GetStoryProgress(token).ConfigureAwait(false);
                Config.EventProgress = Math.Min(Config.Progress, 3);

                UpdateStatus("Reading event raid status");
                try
                {
                    await ReadEventRaids(token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    ButtonEnable(new[] { ButtonConnect }, true);
                    await ErrorHandler.DisplayMessageBox(this, Webhook, $"Error occurred while reading event raids: {ex.Message}", token).ConfigureAwait(false);
                    return;
                }

                UpdateStatus("Reading raids");
                try
                {
                    await ReadRaids(token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    ButtonEnable(new[] { ButtonConnect }, true);
                    await ErrorHandler.DisplayMessageBox(this, Webhook, $"Error occurred while reading raids: {ex.Message}", token).ConfigureAwait(false);
                    return;
                }

                ButtonEnable(new[] { ButtonAdvanceDate, ButtonReadRaids, ButtonDisconnect, ButtonViewRAM, ButtonDownloadEvents, SendScreenshot, btnOpenMap, Rewards, CopyAnnounce, ButtonScreenState }, true);
                if (InvokeRequired)
                    Invoke(() => { ComboIndex.Enabled = true; ComboIndex.SelectedIndex = 0; });
                else ComboIndex.SelectedIndex = 0;

                UpdateStatus("Completed!");
            }, token);
        }

        private void Disconnect_Click(object sender, EventArgs e)
        {
            lock (_connectLock)
            {
                if (ConnectionWrapper is null || !ConnectionWrapper.Connected)
                    return;

                Disconnect(Source.Token);
            }
        }

        private void Disconnect(CancellationToken token)
        {
            Task.Run(async () =>
            {
                ButtonEnable(new[] { ButtonAdvanceDate, ButtonReadRaids, ButtonDisconnect, ButtonViewRAM, ButtonDownloadEvents, SendScreenshot }, false);
                try
                {
                    (bool success, string err) = await ConnectionWrapper.DisconnectAsync(token).ConfigureAwait(false);
                    if (!success)
                        await ErrorHandler.DisplayMessageBox(this, Webhook, err, token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await ErrorHandler.DisplayMessageBox(this, Webhook, ex.Message, token).ConfigureAwait(false);
                }

                Source.Cancel();
                DateAdvanceSource.Cancel();
                Source = new();
                DateAdvanceSource = new();
                RaidBlockOffset = 0;
                ButtonEnable(new[] { ButtonConnect }, true);
            }, token);
        }

        private void ButtonPrevious_Click(object sender, EventArgs e)
        {
            var count = RaidContainer.GetRaidCount();
            if (count > 0)
            {
                var index = (ComboIndex.SelectedIndex + count - 1) % count; // Wrap around
                if (ModifierKeys == Keys.Shift)
                {
                    for (int i = 0; i < count; i++)
                    {
                        var chk = (index + count - i) % count;
                        if (StopAdvances || RaidFilters.Any(z => z.FilterSatisfied(RaidContainer, RaidContainer.Encounters[chk], RaidContainer.Raids[chk], RaidBoost.SelectedIndex)))
                        {
                            index = chk;
                            break;
                        }
                    }
                }
                ComboIndex.SelectedIndex = index;
            }
        }

        private void ButtonNext_Click(object sender, EventArgs e)
        {
            var count = RaidContainer.GetRaidCount();
            if (count > 0)
            {
                var index = (ComboIndex.SelectedIndex + count + 1) % count; // Wrap around
                if (ModifierKeys == Keys.Shift)
                {
                    for (int i = 0; i < count; i++)
                    {
                        var chk = (index + count + i) % count;
                        if (StopAdvances || RaidFilters.Any(z => z.FilterSatisfied(RaidContainer, RaidContainer.Encounters[chk], RaidContainer.Raids[chk], RaidBoost.SelectedIndex)))
                        {
                            index = chk;
                            break;
                        }
                    }
                }
                ComboIndex.SelectedIndex = index;
            }
        }

        private void ButtonAdvanceDate_Click(object sender, EventArgs e)
        {
            if (ConnectionWrapper is null || !ConnectionWrapper.Connected)
                return;

            ButtonAdvanceDate.Visible = false;
            StopAdvance_Button.Visible = true;
            Task.Run(async () => await AdvanceDateClick(DateAdvanceSource.Token).ConfigureAwait(false), Source.Token);
        }

        private async Task AdvanceDateClick(CancellationToken token)
        {
            try
            {
                ButtonEnable(new[] { ButtonViewRAM, ButtonAdvanceDate, ButtonDisconnect, ButtonDownloadEvents, SendScreenshot, ButtonReadRaids }, false);
                Invoke(() => Label_DayAdvance.Visible = true);
                SearchTimer.Start();
                stopwatch.Reset();
                stopwatch.Start();
                StatDaySkipTries = 0;
                StatDaySkipSuccess = 0;
                StatDaySkipStreak = 0;
                StatShinyCount = 0;
                _WindowState = WindowState;

                var advanceTextInit = $"Skip Rate: {GetStatDaySkipSuccess()}/{GetStatDaySkipTries()}";
                var missInit = $"Total Miss: {GetStatDaySkipTries() - GetStatDaySkipSuccess()}";
                var streakInit = $"Streak: {GetStatDaySkipStreak()}";
                var shinyTextInint = $"Shinies Missed: {GetStatShinyCount()}";
                var fomoInit = GetFomo();
                Invoke(() => FomoTip.SetToolTip(LabelShinyCount, string.Join(Environment.NewLine, fomoInit)));
                Invoke(() => DaySkipSuccessRate.Text = advanceTextInit);
                Invoke(() => TotalMiss.Text = missInit);
                Invoke(() => Streak.Text = streakInit);
                Invoke(() => LabelShinyCount.Text = shinyTextInint);
                if (teraRaidView is not null)
                    Invoke(() => teraRaidView.DaySkips.Text = advanceTextInit);

                var stop = false;
                var raids = RaidContainer.Raids;
                while (!stop)
                {
                    var previousSeeds = raids.Select(z => z.Seed).ToList();
                    UpdateStatus("Changing date");

                    bool streamer = Config.StreamerView && teraRaidView is not null;
                    Action<int>? action = streamer ? teraRaidView!.UpdateProgressBar : null;
                    await ConnectionWrapper.AdvanceDate(Config, token, action).ConfigureAwait(false);
                    await ReadRaids(token).ConfigureAwait(false);
                    raids = RaidContainer.Raids;

                    Invoke(DisplayRaid);
                    if (streamer)
                        Invoke(DisplayPrettyRaid);

                    stop = StopAdvanceDate(previousSeeds);

                    var advanceText = $"Skip Rate: {GetStatDaySkipSuccess()}/{GetStatDaySkipTries()}";
                    var miss = $"Total Miss: {GetStatDaySkipTries() - GetStatDaySkipSuccess()}";
                    var streak = $"Streak: {GetStatDaySkipStreak()}";
                    var shinyText = $"Shinies Missed: {GetStatShinyCount()}";
                    var fomo = GetFomo();
                    Invoke(() => FomoTip.SetToolTip(LabelShinyCount, string.Join(Environment.NewLine, fomo)));
                    Invoke(() => DaySkipSuccessRate.Text = advanceText);
                    Invoke(() => TotalMiss.Text = miss);
                    Invoke(() => Streak.Text = streak);
                    Invoke(() => LabelShinyCount.Text = shinyText);
                    if (teraRaidView is not null)
                        Invoke(() => teraRaidView.DaySkips.Text = advanceText);
                }

                stopwatch.Stop();
                SearchTimer.Stop();
                var timeSpan = stopwatch.Elapsed;
                var timeEmpty = new TimeSpan(0, 0, 0, 0);
                string time = string.Empty;
                if (((int)timeSpan.TotalDays) != timeEmpty.TotalDays) { time = timeSpan.ToString(@"d\d\ %h\h\ mm\m\ ss\s"); }
                else if (((int)timeSpan.TotalHours) != timeEmpty.TotalHours) { time = timeSpan.ToString(@"%h\h\ mm\m\ ss\s"); }
                else if (((int)timeSpan.TotalMinutes) != timeEmpty.TotalMinutes) { time = timeSpan.ToString(@"%m\m\ ss\s"); }
                else { time = timeSpan.ToString(@"%s\s"); }

                if (Config.PlaySound)
                    System.Media.SystemSounds.Asterisk.Play();

                if (Config.FocusWindow)
                    Invoke(() => { WindowState = _WindowState; Activate(); });

                if (Config.EnableFilters)
                {
                    var encounters = RaidContainer.Encounters;
                    var rewards = RaidContainer.Rewards;
                    var boost = Invoke(() => { return RaidBoost.SelectedIndex; });
                    var satisfied_filters = new List<(RaidFilter, ITeraRaid, Raid, IReadOnlyList<(int, int, int)>)>();
                    for (int i = 0; i < raids.Count; i++)
                    {
                        foreach (var filter in RaidFilters)
                        {
                            if (filter is null)
                                continue;

                            if (filter.FilterSatisfied(RaidContainer, encounters[i], raids[i], boost))
                            {
                                satisfied_filters.Add((filter, encounters[i], raids[i], rewards[i]));
                                if (InvokeRequired)
                                    Invoke(() => { ComboIndex.SelectedIndex = i; });
                                else ComboIndex.SelectedIndex = i;
                            }
                        }
                    }

                    if (Config.EnableNotification)
                    {
                        foreach (var satisfied in satisfied_filters)
                        {
                            var teraType = satisfied.Item3.GetTeraType(satisfied.Item2);
                            var color = TypeColor.GetTypeSpriteColor((byte)teraType);
                            var hexColor = $"{color.R:X2}{color.G:X2}{color.B:X2}";
                            var blank = new PK9
                            {
                                Species = satisfied.Item2.Species,
                                Form = satisfied.Item2.Form
                            };

                            var spriteName = GetSpriteNameForUrl(blank, satisfied.Item3.CheckIsShiny(satisfied.Item2));
                            await Webhook.SendNotification(satisfied.Item2, satisfied.Item3, satisfied.Item1, time, satisfied.Item4, hexColor, spriteName, Source.Token).ConfigureAwait(false);
                        }
                    }

                    // Save game on match.
                    if (Config.SaveOnMatch && satisfied_filters.Count > 0)
                        await ConnectionWrapper.SaveGame(Config, token).ConfigureAwait(false);

                    if (Config.EnableAlertWindow)
                        await ErrorHandler.DisplayMessageBox(this, Webhook, $"{Config.AlertWindowMessage}\n\nTime Spent: {time}", token, "Result found!").ConfigureAwait(false);

                    if (Config.SearchTimeInTitle)
                        Invoke(() => Text = $"{formTitle} [Match Found in {time}]");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus("Date advance stopped.");
                SearchTimer.Stop();
                if (ex is not TaskCanceledException)
                    await ErrorHandler.DisplayMessageBox(this, Webhook, ex.Message, token, "Date Advance Error").ConfigureAwait(false);
            }

            if (InvokeRequired)
                Invoke(() => { ButtonAdvanceDate.Visible = true; StopAdvance_Button.Visible = false; });
            else
            {
                ButtonAdvanceDate.Visible = true;
                StopAdvance_Button.Visible = false;
            }

            ButtonEnable(new[] { ButtonViewRAM, ButtonAdvanceDate, ButtonDisconnect, ButtonDownloadEvents, SendScreenshot, ButtonReadRaids }, true);
            DateAdvanceSource = new();
        }

        private void StopAdvanceButton_Click(object sender, EventArgs e)
        {
            StopAdvance_Button.Visible = false;
            ButtonAdvanceDate.Visible = true;
            DateAdvanceSource.Cancel();
            DateAdvanceSource = new();
            teraRaidView?.ResetProgressBar();

            stopwatch.Stop();
            SearchTimer.Stop();
        }

        private void ButtonReadRaids_Click(object sender, EventArgs e)
        {
            Task.Run(async () => await ReadRaidsAsync(Source.Token).ConfigureAwait(false), Source.Token);
        }

        private async Task ReadRaidsAsync(CancellationToken token)
        {
            if (IsReading)
            {
                await ErrorHandler.DisplayMessageBox(this, Webhook, "Please wait for the current RAM read to finish.", token).ConfigureAwait(false);
                return;
            }

            ButtonEnable(new[] { ButtonViewRAM, ButtonAdvanceDate, ButtonDisconnect, ButtonDownloadEvents, SendScreenshot, ButtonReadRaids }, false);
            try
            {
                await ReadRaids(token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await ErrorHandler.DisplayMessageBox(this, Webhook, $"Error occurred while reading raids: {ex.Message}", token).ConfigureAwait(false);
            }

            ButtonEnable(new[] { ButtonViewRAM, ButtonAdvanceDate, ButtonDisconnect, ButtonDownloadEvents, SendScreenshot, ButtonReadRaids }, true);
        }

        private void ViewRAM_Click(object sender, EventArgs e)
        {
            if (IsReading)
            {
                Task.Run(async () => await ErrorHandler.DisplayMessageBox(this, Webhook, "Please wait for the current RAM read to finish.", Source.Token).ConfigureAwait(false), Source.Token);
                return;
            }

            ButtonEnable(new[] { ButtonViewRAM }, false);
            RaidBlockViewer window = default!;

            if (ConnectionWrapper is not null && ConnectionWrapper.Connected && ModifierKeys == Keys.Shift)
            {
                try
                {
                    var data = ConnectionWrapper.Connection.ReadBytesAbsoluteAsync(RaidBlockOffset, (int)RaidBlock.SIZE, Source.Token).Result;
                    window = new(data, RaidBlockOffset);
                }
                catch (Exception ex)
                {
                    ButtonEnable(new[] { ButtonViewRAM }, true);
                    Task.Run(async () => await ErrorHandler.DisplayMessageBox(this, Webhook, ex.Message, Source.Token).ConfigureAwait(false), Source.Token);
                    return;
                }
            }
            else if (RaidContainer.Raids.Count > ComboIndex.SelectedIndex)
            {
                var data = RaidContainer.Raids[ComboIndex.SelectedIndex].GetData();
                window = new(data, RaidBlockOffset);
            }

            ShowDialog(window);
            ButtonEnable(new[] { ButtonViewRAM }, true);
        }

        private void StopFilter_Click(object sender, EventArgs e)
        {
            var form = new FilterSettings(ref RaidFilters);
            ShowDialog(form);
        }

        private void DownloadEvents_Click(object sender, EventArgs e)
        {
            if (ConnectionWrapper is null || !ConnectionWrapper.Connected)
                return;

            if (IsReading)
            {
                Task.Run(async () => await ErrorHandler.DisplayMessageBox(this, Webhook, "Please wait for the current RAM read to finish.", Source.Token).ConfigureAwait(false), Source.Token);
                return;
            }

            Task.Run(async () => { await DownloadEventsAsync(Source.Token).ConfigureAwait(false); }, Source.Token);
        }

        private async Task DownloadEventsAsync(CancellationToken token)
        {
            ButtonEnable(new[] { ButtonViewRAM, ButtonAdvanceDate, ButtonDisconnect, ButtonDownloadEvents, SendScreenshot, ButtonReadRaids }, false);
            UpdateStatus("Reading event raid status");

            try
            {
                await ReadEventRaids(token, true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await ErrorHandler.DisplayMessageBox(this, Webhook, $"Error occurred while reading event raids: {ex.Message}", token).ConfigureAwait(false);
            }

            ButtonEnable(new[] { ButtonViewRAM, ButtonAdvanceDate, ButtonDisconnect, ButtonDownloadEvents, SendScreenshot, ButtonReadRaids }, true);
            UpdateStatus("Completed!");
        }

        private void Seed_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Shift && RaidContainer.Raids.Count > ComboIndex.SelectedIndex)
            {
                var raid = RaidContainer.Raids[ComboIndex.SelectedIndex];
                Seed.Text = HideSeed ? $"{raid.Seed:X8}" : "Hidden";
                EC.Text = HideSeed ? $"{raid.EC:X8}" : "Hidden";
                PID.Text = (HideSeed ? $"{raid.PID:X8}" : "Hidden") + $"{(raid.IsShiny ? " (☆)" : string.Empty)}";
                HideSeed = !HideSeed;
                ActiveControl = null;
            }
        }

        private void ConfigSettings_Click(object sender, EventArgs e)
        {
            var form = new ConfigWindow(Config);
            ShowDialog(form);
        }

        private void EnableFilters_Click(object sender, EventArgs e)
        {
            Config.EnableFilters = CheckEnableFilters.Checked;
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Config.Location = Location;
            var configpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            JsonSerializerOptions options = new() { WriteIndented = true };
            string output = JsonSerializer.Serialize(Config, options);
            using StreamWriter sw = new(configpath);
            sw.Write(output);

            if (ConnectionWrapper is not null && ConnectionWrapper.Connected)
            {
                try
                {
                    _ = ConnectionWrapper.DisconnectAsync(Source.Token).Result;
                }
                catch { }
            }

            Source.Cancel();
            DateAdvanceSource.Cancel();
            Source = new();
            DateAdvanceSource = new();
        }

        private async Task ReadEventRaids(CancellationToken token, bool force = false)
        {
            var prio_file = Path.Combine(Directory.GetCurrentDirectory(), "cache", "raid_priority_array");
            if (!force && File.Exists(prio_file))
            {
                (_, var version) = FlatbufferDumper.DumpDeliveryPriorities(File.ReadAllBytes(prio_file));
                var blk = await ConnectionWrapper.ReadBlockDefault(BCATRaidPriorityLocation, "raid_priority_array.tmp", true, token).ConfigureAwait(false);
                (_, var v2) = FlatbufferDumper.DumpDeliveryPriorities(blk);
                if (version != v2)
                    force = true;

                var tmp_file = Path.Combine(Directory.GetCurrentDirectory(), "cache", "raid_priority_array.tmp");
                if (File.Exists(tmp_file))
                    File.Delete(tmp_file);

                if (v2 == 0) // raid reset
                    return;
            }

            var delivery_raid_prio = await ConnectionWrapper.ReadBlockDefault(BCATRaidPriorityLocation, "raid_priority_array", force, token).ConfigureAwait(false);
            (var group_id, var priority) = FlatbufferDumper.DumpDeliveryPriorities(delivery_raid_prio);
            if (priority == 0)
                return;

            var delivery_raid_fbs = await ConnectionWrapper.ReadBlockDefault(BCATRaidBinaryLocation, "raid_enemy_array", force, token).ConfigureAwait(false);
            var delivery_fixed_rewards = await ConnectionWrapper.ReadBlockDefault(BCATRaidFixedRewardLocation, "fixed_reward_item_array", force, token).ConfigureAwait(false);
            var delivery_lottery_rewards = await ConnectionWrapper.ReadBlockDefault(BCATRaidLotteryRewardLocation, "lottery_reward_item_array", force, token).ConfigureAwait(false);

            RaidContainer.DistTeraRaids = TeraDistribution.GetAllEncounters(delivery_raid_fbs);
            RaidContainer.DeliveryRaidPriority = group_id;
            RaidContainer.DeliveryRaidFixedRewards = FlatbufferDumper.DumpFixedRewards(delivery_fixed_rewards);
            RaidContainer.DeliveryRaidLotteryRewards = FlatbufferDumper.DumpLotteryRewards(delivery_lottery_rewards);
        }

        private void DisplayRaid()
        {
            int index = ComboIndex.SelectedIndex;
            var raids = RaidContainer.Raids;
            if (raids.Count > index)
            {
                Raid raid = raids[index];
                var encounter = RaidContainer.Encounters[index];

                GameVersionImg.BackgroundImage = (Config.Game == "Violet") ? Properties.Resources.violet : Properties.Resources.scarlet;
                Seed.Text = !HideSeed ? $"{raid.Seed:X8}" : "Hidden";
                EC.Text = !HideSeed ? $"{raid.EC:X8}" : "Hidden";
                PID.Text = GetPIDString(raid, encounter);
                Area.Text = $"{Areas.GetArea((int)(raid.Area - 1))} - Den {raid.Den}";
                //textBox1.Text = $"{raid.Area}-{raid.Den}";
                labelEvent.Visible = raid.IsEvent;

                var teratype = raid.GetTeraType(encounter);
                TeraType.Text = RaidContainer.Strings.types[teratype];

                int StarCount = encounter is TeraDistribution ? encounter.Stars : raid.GetStarCount(raid.Difficulty, Config.Progress, raid.IsBlack);
                Difficulty.Text = string.Concat(Enumerable.Repeat("☆", StarCount));

                ///vio thing
                var map = GenerateMap(raid, teratype);
                statsPanel.BackgroundImage = (Config.MapBackground ? map : null);
                ///vio thing

                var strings = GameInfo.GetStrings(1);
                var param = encounter.GetParam();
                var blank = new PK9
                {
                    Species = encounter.Species,
                    Form = encounter.Form
                };

                Encounter9RNG.GenerateData(blank, param, EncounterCriteria.Unrestricted, raid.Seed);
                var img = blank.Sprite();
                img = ApplyTeraColor((byte)teratype, img, SpriteBackgroundType.BottomStripe);

                var form = Utils.GetFormString(blank.Species, blank.Form, strings);
                /*var form = ShowdownParsing.GetStringFromForm(encounter.Form, RaidContainer.Strings, encounter.Species, EntityContext.Gen9);
                if (form.Length > 0 && form[0] != '-')
                    form = form.Insert(0, "-");*/

                Species.Text = $"{RaidContainer.Strings.Species[encounter.Species]}{form}";
                Sprite.Image = img;
                GemIcon.Image = PKHeX.Drawing.Misc.TypeSpriteUtil.GetTypeSpriteGem((byte)teratype);
                Gender.Text = $"{(Gender)blank.Gender}";

                var nature = blank.Nature;
                Nature.Text = $"{RaidContainer.Strings.Natures[nature]}";
                Ability.Text = $"{RaidContainer.Strings.Ability[blank.Ability]}";
                Scale.Text = $"{PokeSizeDetailedUtil.GetSizeRating(blank.Scale)} ({blank.Scale})";

                var extra_moves = new ushort[] { 0, 0, 0, 0 };
                for (int i = 0; i < encounter.ExtraMoves.Length; i++)
                {
                    if (i < extra_moves.Length)
                        extra_moves[i] = encounter.ExtraMoves[i];
                }

                Move1.Text = ShowExtraMoves ? RaidContainer.Strings.Move[extra_moves[0]] : RaidContainer.Strings.Move[encounter.Move1];
                Move2.Text = ShowExtraMoves ? RaidContainer.Strings.Move[extra_moves[1]] : RaidContainer.Strings.Move[encounter.Move2];
                Move3.Text = ShowExtraMoves ? RaidContainer.Strings.Move[extra_moves[2]] : RaidContainer.Strings.Move[encounter.Move3];
                Move4.Text = ShowExtraMoves ? RaidContainer.Strings.Move[extra_moves[3]] : RaidContainer.Strings.Move[encounter.Move4];

                IVs.Text = IVsString(Utils.ToSpeedLast(blank.IVs));
                toolTip.SetToolTip(IVs, IVsString(Utils.ToSpeedLast(blank.IVs), true));

                shinyBox.Image = raid.CheckIsShiny(encounter) ? (ShinyExtensions.IsSquareShinyExist(blank) ? Properties.Resources.square : Properties.Resources.shiny) : null;
                shinyBox.SizeMode = PictureBoxSizeMode.Zoom;

                PID.BackColor = raid.CheckIsShiny(encounter) ? (ShinyExtensions.IsSquareShinyExist(blank) ? Color.FromArgb(125, 255, 135, 0) : Color.FromArgb(125, 255, 215, 0)) : Color.FromArgb(100, 240, 240, 240); //Square - Orange, Shiny - Yellow
                IVs.BackColor = IVs.Text is "31/31/31/31/31/31" ? Color.FromArgb(125, 154, 205, 50) : Color.FromArgb(100, 240, 240, 240); //Green-yellow
                EC.BackColor = (raid.EC % 100 == 0 && (encounter!.Species == 924 || encounter.Species == 206) ? Color.FromArgb(125, 0, 215, 255) : Color.FromArgb(100, 240, 240, 240)); //Cyan
                return;
            }

            var msg = $"Unable to display raid at index {index}. Ensure there are no cheats running or anything else that might shift RAM (Edizon, overlays, etc.), then reboot your console and try again.";
            Task.Run(async () => await ErrorHandler.DisplayMessageBox(this, Webhook, msg, Source.Token).ConfigureAwait(false), Source.Token);
        }

        private static Image? GetDisplayGemImage(int teratype, Raid raid)
        {
            var display_black = raid.IsBlack || raid.Flags == 3;
            var baseImg = display_black ? (Image?)Properties.Resources.ResourceManager.GetObject($"black_{teratype:D2}")
                                        : (Image?)Properties.Resources.ResourceManager.GetObject($"gem_{teratype:D2}");
            if (baseImg is null)
                return null;

            var backlayer = new Bitmap(baseImg.Width + 10, baseImg.Height + 10, baseImg.PixelFormat);
            baseImg = ImageUtil.LayerImage(backlayer, baseImg, 5, 5);
            var pixels = ImageUtil.GetPixelData((Bitmap)baseImg);
            for (int i = 0; i < pixels.Length; i += 4)
            {
                if (pixels[i + 3] == 0)
                {
                    pixels[i] = 0;
                    pixels[i + 1] = 0;
                    pixels[i + 2] = 0;
                }
            }

            baseImg = ImageUtil.GetBitmap(pixels, baseImg.Width, baseImg.Height, baseImg.PixelFormat);
            if (display_black)
            {
                var color = Color.Indigo;
                SpriteUtil.GetSpriteGlow(baseImg, color.B, color.G, color.R, out var glow, false);
                baseImg = ImageUtil.LayerImage(ImageUtil.GetBitmap(glow, baseImg.Width, baseImg.Height, baseImg.PixelFormat), baseImg, 0, 0);
            }
            else if (raid.IsEvent)
            {
                var color = Color.DarkTurquoise;
                SpriteUtil.GetSpriteGlow(baseImg, color.B, color.G, color.R, out var glow, false);
                baseImg = ImageUtil.LayerImage(ImageUtil.GetBitmap(glow, baseImg.Width, baseImg.Height, baseImg.PixelFormat), baseImg, 0, 0);
            }
            return baseImg;
        }

        private void DisplayPrettyRaid()
        {
            if (teraRaidView is null)
            {
                Task.Run(async () => await ErrorHandler.DisplayMessageBox(this, Webhook, "Something went terribly wrong: teraRaidView is not initialized.", Source.Token).ConfigureAwait(false), Source.Token);
                return;
            }

            int index = ComboIndex.SelectedIndex;
            var raids = RaidContainer.Raids;
            if (raids.Count > index)
            {
                Raid raid = raids[index];
                var encounter = RaidContainer.Encounters[index];

                teraRaidView.Area.Text = $"{Areas.GetArea((int)(raid.Area - 1))} - Den {raid.Den}";

                var teratype = raid.GetTeraType(encounter);
                teraRaidView.TeraType.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("gem_text_" + teratype)!;

                int StarCount = encounter is TeraDistribution ? encounter.Stars : raid.GetStarCount(raid.Difficulty, Config.Progress, raid.IsBlack);
                teraRaidView.Difficulty.Text = string.Concat(Enumerable.Repeat("⭐", StarCount));

                var param = encounter.GetParam();
                var blank = new PK9
                {
                    Species = encounter.Species,
                    Form = encounter.Form
                };

                Encounter9RNG.GenerateData(blank, param, EncounterCriteria.Unrestricted, raid.Seed);
                var img = blank.Sprite();

                teraRaidView.picBoxPokemon.Image = img;
                var form = Utils.GetFormString(blank.Species, blank.Form, RaidContainer.Strings);

                teraRaidView.Species.Text = $"{RaidContainer.Strings.Species[encounter.Species]}{form}";
                teraRaidView.Gender.Text = $"{(Gender)blank.Gender}";

                var nature = blank.Nature;
                teraRaidView.Nature.Text = $"{RaidContainer.Strings.Natures[nature]}";
                teraRaidView.Ability.Text = $"{RaidContainer.Strings.Ability[blank.Ability]}";

                teraRaidView.Move1.Text = encounter.Move1 > 0 ? RaidContainer.Strings.Move[encounter.Move1] : "---";
                teraRaidView.Move2.Text = encounter.Move2 > 0 ? RaidContainer.Strings.Move[encounter.Move2] : "---";
                teraRaidView.Move3.Text = encounter.Move3 > 0 ? RaidContainer.Strings.Move[encounter.Move3] : "---";
                teraRaidView.Move4.Text = encounter.Move4 > 0 ? RaidContainer.Strings.Move[encounter.Move4] : "---";

                var length = encounter.ExtraMoves.Length < 4 ? 4 : encounter.ExtraMoves.Length;
                var extra_moves = new ushort[length];
                for (int i = 0; i < encounter.ExtraMoves.Length; i++)
                    extra_moves[i] = encounter.ExtraMoves[i];

                teraRaidView.Move5.Text = extra_moves[0] > 0 ? RaidContainer.Strings.Move[extra_moves[0]] : "---";
                teraRaidView.Move6.Text = extra_moves[1] > 0 ? RaidContainer.Strings.Move[extra_moves[1]] : "---";
                teraRaidView.Move7.Text = extra_moves[2] > 0 ? RaidContainer.Strings.Move[extra_moves[2]] : "---";
                teraRaidView.Move8.Text = extra_moves[3] > 0 ? RaidContainer.Strings.Move[extra_moves[3]] : "---";

                var ivs = Utils.ToSpeedLast(blank.IVs);

                // HP
                teraRaidView.HP.Text = $"{ivs[0]:D2}";
                teraRaidView.HP.BackColor = Color.FromArgb(0, 5, 25);
                if (teraRaidView.HP.Text is "31")
                    teraRaidView.HP.BackColor = Color.ForestGreen;
                else if (teraRaidView.HP.Text is "00")
                    teraRaidView.HP.BackColor = Color.DarkRed;

                // ATK
                teraRaidView.ATK.Text = $"{ivs[1]:D2}";
                teraRaidView.ATK.BackColor = Color.FromArgb(0, 5, 25);
                if (teraRaidView.ATK.Text is "31")
                    teraRaidView.ATK.BackColor = Color.ForestGreen;
                else if (teraRaidView.ATK.Text is "00")
                    teraRaidView.ATK.BackColor = Color.DarkRed;

                // DEF
                teraRaidView.DEF.Text = $"{ivs[2]:D2}";
                teraRaidView.DEF.BackColor = Color.FromArgb(0, 5, 25);
                if (teraRaidView.DEF.Text is "31")
                    teraRaidView.DEF.BackColor = Color.ForestGreen;
                else if (teraRaidView.DEF.Text is "00")
                    teraRaidView.DEF.BackColor = Color.DarkRed;

                // SPA
                teraRaidView.SPA.Text = $"{ivs[3]:D2}";
                teraRaidView.SPA.BackColor = Color.FromArgb(0, 5, 25);
                if (teraRaidView.SPA.Text is "31")
                    teraRaidView.SPA.BackColor = Color.ForestGreen;
                else if (teraRaidView.SPA.Text is "00")
                    teraRaidView.SPA.BackColor = Color.DarkRed;

                // SPD
                teraRaidView.SPD.Text = $"{ivs[4]:D2}";
                teraRaidView.SPD.BackColor = Color.FromArgb(0, 5, 25);
                if (teraRaidView.SPD.Text is "31")
                    teraRaidView.SPD.BackColor = Color.ForestGreen;
                else if (teraRaidView.SPD.Text is "00")
                    teraRaidView.SPD.BackColor = Color.DarkRed;

                // SPEED
                teraRaidView.SPEED.Text = $"{ivs[5]:D2}";
                teraRaidView.SPEED.BackColor = Color.FromArgb(0, 5, 25);
                if (teraRaidView.SPEED.Text is "31")
                    teraRaidView.SPEED.BackColor = Color.ForestGreen;
                else if (teraRaidView.SPEED.Text is "00")
                    teraRaidView.SPEED.BackColor = Color.DarkRed;


                var map = GenerateMap(raid, teratype);
                if (map is null)
                    Task.Run(async () => await ErrorHandler.DisplayMessageBox(this, Webhook, "Error generating map.", Source.Token).ConfigureAwait(false), Source.Token);
                teraRaidView.Map.Image = map;

                // Rewards
                var rewards = RaidContainer.Rewards[index];

                teraRaidView.textAbilityPatch.Text = "0";
                teraRaidView.textAbilityPatch.ForeColor = Color.DimGray;
                teraRaidView.labelAbilityPatch.ForeColor = Color.DimGray;

                teraRaidView.textAbilityCapsule.Text = "0";
                teraRaidView.textAbilityCapsule.ForeColor = Color.DimGray;
                teraRaidView.labelAbilityCapsule.ForeColor = Color.DimGray;

                teraRaidView.textBottleCap.Text = "0";
                teraRaidView.textBottleCap.ForeColor = Color.DimGray;
                teraRaidView.labelBottleCap.ForeColor = Color.DimGray;

                teraRaidView.textSweetHerba.Text = "0";
                teraRaidView.textSweetHerba.ForeColor = Color.DimGray;
                teraRaidView.labelSweetHerba.ForeColor = Color.DimGray;

                teraRaidView.textSaltyHerba.Text = "0";
                teraRaidView.textSaltyHerba.ForeColor = Color.DimGray;
                teraRaidView.labelSaltyHerba.ForeColor = Color.DimGray;

                teraRaidView.textBitterHerba.Text = "0";
                teraRaidView.textBitterHerba.ForeColor = Color.DimGray;
                teraRaidView.labelBitterHerba.ForeColor = Color.DimGray;

                teraRaidView.textSourHerba.Text = "0";
                teraRaidView.textSourHerba.ForeColor = Color.DimGray;
                teraRaidView.labelSourHerba.ForeColor = Color.DimGray;

                teraRaidView.textSpicyHerba.Text = "0";
                teraRaidView.textSpicyHerba.ForeColor = Color.DimGray;
                teraRaidView.labelSpicyHerba.ForeColor = Color.DimGray;

                for (int i = 0; i < rewards.Count; i++)
                {
                    if (rewards[i].Item1 == 645)
                    {
                        teraRaidView.textAbilityCapsule.Text = (int.Parse(teraRaidView.textAbilityCapsule.Text) + 1).ToString();
                        teraRaidView.textAbilityCapsule.ForeColor = Color.White;
                        teraRaidView.labelAbilityCapsule.ForeColor = Color.WhiteSmoke;
                    }
                    if (rewards[i].Item1 == 795)
                    {
                        teraRaidView.textBottleCap.Text = (int.Parse(teraRaidView.textBottleCap.Text) + 1).ToString();
                        teraRaidView.textBottleCap.ForeColor = Color.White;
                        teraRaidView.labelBottleCap.ForeColor = Color.WhiteSmoke;
                    }
                    if (rewards[i].Item1 == 1606)
                    {
                        teraRaidView.textAbilityPatch.Text = (int.Parse(teraRaidView.textAbilityPatch.Text) + 1).ToString();
                        teraRaidView.textAbilityPatch.ForeColor = Color.White;
                        teraRaidView.labelAbilityPatch.ForeColor = Color.WhiteSmoke;
                    }
                    if (rewards[i].Item1 == 1904)
                    {
                        teraRaidView.textSweetHerba.Text = (int.Parse(teraRaidView.textSweetHerba.Text) + 1).ToString();
                        teraRaidView.textSweetHerba.ForeColor = Color.White;
                        teraRaidView.labelSweetHerba.ForeColor = Color.WhiteSmoke;
                    }
                    if (rewards[i].Item1 == 1905)
                    {
                        teraRaidView.textSaltyHerba.Text = (int.Parse(teraRaidView.textSaltyHerba.Text) + 1).ToString();
                        teraRaidView.textSaltyHerba.ForeColor = Color.White;
                        teraRaidView.labelSaltyHerba.ForeColor = Color.WhiteSmoke;
                    }
                    if (rewards[i].Item1 == 1906)
                    {
                        teraRaidView.textSourHerba.Text = (int.Parse(teraRaidView.textSourHerba.Text) + 1).ToString();
                        teraRaidView.textSourHerba.ForeColor = Color.White;
                        teraRaidView.labelSourHerba.ForeColor = Color.WhiteSmoke;
                    }
                    if (rewards[i].Item1 == 1907)
                    {
                        teraRaidView.textBitterHerba.Text = (int.Parse(teraRaidView.textBitterHerba.Text) + 1).ToString();
                        teraRaidView.textBitterHerba.ForeColor = Color.White;
                        teraRaidView.labelBitterHerba.ForeColor = Color.WhiteSmoke;
                    }
                    if (rewards[i].Item1 == 1908)
                    {
                        teraRaidView.textSpicyHerba.Text = (int.Parse(teraRaidView.textSpicyHerba.Text) + 1).ToString();
                        teraRaidView.textSpicyHerba.ForeColor = Color.White;
                        teraRaidView.labelSpicyHerba.ForeColor = Color.WhiteSmoke;
                    }
                }

                var shiny = raid.CheckIsShiny(encounter);
                teraRaidView.Shiny.Visible = shiny;
                teraRaidView.picShinyAlert.Enabled = shiny;
                return;
            }

            var msg = $"Unable to display raid at index {index}. Ensure there are no cheats running or anything else that might shift RAM (Edizon, overlays, etc.), then reboot your console and try again.";
            Task.Run(async () => await ErrorHandler.DisplayMessageBox(this, Webhook, msg, Source.Token).ConfigureAwait(false), Source.Token);
        }

        private string GetPIDString(Raid raid, ITeraRaid? enc)
        {
            if (HideSeed)
                return "Hidden";

            var shiny_mark = " (☆)";
            var pid = $"{raid.PID:X8}";
            return raid.CheckIsShiny(enc) ? pid + shiny_mark : pid;
        }

        private static string IVsString(int[] ivs, bool verbose = false)
        {
            string s = string.Empty;
            var stats = new[] { "HP", "Atk", "Def", "SpA", "SpD", "Spe" };
            for (int i = 0; i < ivs.Length; i++)
            {
                s += $"{ivs[i]:D2}{(verbose ? " " + stats[i] : string.Empty)}";
                if (i < 5)
                    s += "/";
            }
            return s;
        }

        private static Image ApplyTeraColor(byte elementalType, Image img, SpriteBackgroundType type)
        {
            var color = TypeColor.GetTypeSpriteColor(elementalType);
            var thk = SpriteBuilder.ShowTeraThicknessStripe;
            var op = SpriteBuilder.ShowTeraOpacityStripe;
            var bg = SpriteBuilder.ShowTeraOpacityBackground;
            return ApplyColor(img, type, color, thk, op, bg);
        }

        private static Image ApplyColor(Image img, SpriteBackgroundType type, Color color, int thick, byte opacStripe, byte opacBack)
        {
            if (type == SpriteBackgroundType.BottomStripe)
            {
                int stripeHeight = thick; // from bottom
                if ((uint)stripeHeight > img.Height) // clamp negative & too-high values back to height.
                    stripeHeight = img.Height;

                return ImageUtil.BlendTransparentTo(img, color, opacStripe, img.Width * 4 * (img.Height - stripeHeight));
            }
            if (type == SpriteBackgroundType.TopStripe)
            {
                int stripeHeight = thick; // from top
                if ((uint)stripeHeight > img.Height) // clamp negative & too-high values back to height.
                    stripeHeight = img.Height;

                return ImageUtil.BlendTransparentTo(img, color, opacStripe, 0, (img.Width * 4 * stripeHeight) - 4);
            }
            if (type == SpriteBackgroundType.FullBackground) // full background
                return ImageUtil.BlendTransparentTo(img, color, opacBack);
            return img;
        }

        /*private static Image? GenerateMap(Raid raid, int teratype)
        {
            var original = PKHeX.Drawing.Misc.TypeSpriteUtil.GetTypeSpriteGem((byte)teratype);
            if (original is null)
                return null;
            var mapimg = (Image)new Bitmap(original, new Size(30, 30));
            var gem = (Image)new Bitmap(original, new Size(30, 30));
            var gem2 = (Image)new Bitmap(original, new Size(30, 30));
            SpriteUtil.GetSpriteGlow(gem, 0xFF, 0xFF, 0xFF, out var glow, true);
            SpriteUtil.GetSpriteGlow(gem, 0xCC, 0xCC, 0xCC, out var glow2, true);
            gem = ImageUtil.LayerImage(gem, ImageUtil.GetBitmap(glow, gem.Width, gem.Height, gem.PixelFormat), 0, 0);
            gem2 = ImageUtil.LayerImage(gem, ImageUtil.GetBitmap(glow2, gem.Width, gem.Height, gem.PixelFormat), 0, 0);
            if (den_locations is null || den_locations.Count == 0)
                return null;

            double x, y;
            try
            {
                if (den_locations.TryGetValue($"{raid.Area}-{raid.Den}_", out float[]? value))
                {
                    x = (value[0] + 2.072021484) * 512 / 5000;
                    y = (value[2] + 5255.240018) * 512 / 5000;
                    mapimg = ImageUtil.LayerImage(mapimg, gem, (int)x, (int)y);
                    //return ImageUtil.LayerImage(map, gem, (int)x, (int)y);
                }

                x = (den_locations[$"{raid.Area}-{raid.Den}"][0] + 2.072021484) * 512 / 5000;
                y = (den_locations[$"{raid.Area}-{raid.Den}"][2] + 5255.240018) * 512 / 5000;
                mapimg = ImageUtil.LayerImage(mapimg, gem2, (int)x, (int)y);
                //return ImageUtil.LayerImage(map, gem2, (int)x, (int)y);
                return mapimg;
            }
            catch { return null; }
        }*/

        private static Image? GenerateMap(Raid raid, int teratype)
        {
            var original = PKHeX.Drawing.Misc.TypeSpriteUtil.GetTypeSpriteGem((byte)teratype);
            if (original == null)
                return null;
            var gem = (Image)new Bitmap(original, new Size(30, 30));
            var gem2 = (Image)new Bitmap(original, new Size(30, 30));
            SpriteUtil.GetSpriteGlow(gem, 0xFF, 0xFF, 0xFF, out var glow, true);
            SpriteUtil.GetSpriteGlow(gem, 0xCC, 0xCC, 0xCC, out var glow2, true);
            gem = ImageUtil.LayerImage(gem, ImageUtil.GetBitmap(glow, gem.Width, gem.Height, gem.PixelFormat), 0, 0);
            gem2 = ImageUtil.LayerImage(gem, ImageUtil.GetBitmap(glow2, gem.Width, gem.Height, gem.PixelFormat), 0, 0);
            if (den_locations == null || den_locations.Count == 0)
                return null;
            try
            {
                var x = (den_locations[$"{raid.Area}-{raid.Den}"][0] - 100.072021484) * 512 / 5000;
                var y = (den_locations[$"{raid.Area}-{raid.Den}"][2] + 5350.240018) * 512 / 5000;
                var mapimg = ImageUtil.LayerImage(map, gem, (int)x, (int)y);
                if (den_locations.ContainsKey($"{raid.Area}-{raid.Den}_"))
                {
                    x = (den_locations[$"{raid.Area}-{raid.Den}_"][0] - 100.072021484) * 512 / 5000;
                    y = (den_locations[$"{raid.Area}-{raid.Den}_"][2] + 5350.240018) * 512 / 5000;
                    mapimg = ImageUtil.LayerImage(mapimg, gem2, (int)x, (int)y);
                }
                return mapimg;
            }
            catch { return null; }
        }

        private bool StopAdvanceDate(List<uint> previousSeeds)
        {
            var raids = RaidContainer.Raids;
            var encounters = RaidContainer.Encounters;
            var rewards = RaidContainer.Rewards;
            var strings = GameInfo.GetStrings(1);
            var curSeeds = raids.Select(x => x.Seed).ToArray();
            var sameraids = curSeeds.Except(previousSeeds).ToArray().Length == 0;

            StatDaySkipTries++;
            if (sameraids)
            {
                StatDaySkipStreak = 0;
                return false;
            }

            StatDaySkipSuccess++;
            StatDaySkipStreak++;
            if (!Config.EnableFilters)
                return true;

            //StatShinyCount += Enumerable.Range(0, raids.Count).Where(i => raids[i].CheckIsShiny(encounters[i])).Count();

            for (int i = 0; i < RaidFilters.Count; i++)
            {
                var index = 0;
                if (InvokeRequired)
                    index = Invoke(() => { return RaidBoost.SelectedIndex; });
                else index = RaidBoost.SelectedIndex;

                //var encounters = RaidContainer.Encounters;
                if (RaidFilters[i].FilterSatisfied(RaidContainer, encounters, raids, index))
                    return true;
            }

            for (int i = 0; i < raids.Count; i++)
            {
                var raid = raids[i];
                var encounter = encounters[i];
                var reward = rewards[i];
                var param = encounter.GetParam();

                var timeSpan = stopwatch.Elapsed;
                var timeEmpty = new TimeSpan(0, 0, 0, 0);
                string time = string.Empty;
                if (((int)timeSpan.TotalDays) != timeEmpty.TotalDays) { time = timeSpan.ToString(@"d\d\ %h\h\ mm\m\ ss\s"); }
                else if (((int)timeSpan.TotalHours) != timeEmpty.TotalHours) { time = timeSpan.ToString(@"%h\h\ mm\m\ ss\s"); }
                else if (((int)timeSpan.TotalMinutes) != timeEmpty.TotalMinutes) { time = timeSpan.ToString(@"%m\m\ ss\s"); }
                else { time = timeSpan.ToString(@"%s\s"); }
                var teraType = raids[i].GetTeraType(encounters[i]);
                var color = TypeColor.GetTypeSpriteColor((byte)teraType);
                var hexColor = $"{color.R:X2}{color.G:X2}{color.B:X2}";
                var filter = new RaidFilter { Name = "FoMO" };

                var blank = new PK9
                {
                    Species = encounter.Species,
                    Form = encounter.Form,
                    Gender = encounters[i].Gender,
                };
                blank.SetSuggestedFormArgument();
                Encounter9RNG.GenerateData(blank, param, EncounterCriteria.Unrestricted, raid.Seed);
                var spriteName = GetSpriteNameForUrl(blank, raids[i].CheckIsShiny(encounters[i]));
                var form = Utils.GetFormString(blank.Species, blank.Form, strings);
                var species = $"{strings.Species[encounter.Species]}";
                var shiny = $"{(raid.CheckIsShiny(encounter) ? (ShinyExtensions.IsSquareShinyExist(blank) ? "⛋" : "☆") : "")}";
                if (raids[i].CheckIsShiny(encounters[i]))
                {
                    StatShinyCount++;
                    Fomo.Add($"{shiny} {species}{form}");
                    if (Config.EnableFomoNotification)
                        Task.Run(async () => await FomoWebhook.SendFomoNotification(encounter, raid, filter, time, reward, hexColor, spriteName, Source.Token));
                    if (Config.SaveOnFomo)
                        Task.WaitAll(Task.Run(async () => await ConnectionWrapper.SaveGame(Config, Source.Token).ConfigureAwait(false)));
                    //Task.Run(async () => await SendFomoWebhookAsync(encounter, raid, time, reward, hexColor, spriteName));
                }

            }

            return StopAdvances;
        }

        private async Task ReadRaids(CancellationToken token)
        {
            if (RaidBlockOffset == 0)
            {
                UpdateStatus("Caching raid block pointer");
                RaidBlockOffset = await ConnectionWrapper.Connection.PointerAll(ConnectionWrapper.RaidBlockPointer, token).ConfigureAwait(false);
            }

            RaidContainer.ClearEncounters();
            RaidContainer.ClearRewards();

            RaidContainer.ClearRaids();
            UpdateStatus("Reading raid block");
            var data = await ConnectionWrapper.Connection.ReadBytesAbsoluteAsync(RaidBlockOffset + RaidBlock.HEADER_SIZE, (int)(RaidBlock.SIZE - RaidBlock.HEADER_SIZE), token).ConfigureAwait(false);

            var msg = string.Empty;
            (int delivery, int enc) = RaidContainer.ReadAllRaids(data, Config.Progress, Config.EventProgress, GetRaidBoost());
            if (enc > 0)
                msg += $"Failed to find encounters for {enc} raid(s).\n";

            if (delivery > 0)
                msg += $"Invalid delivery group ID for {delivery} raid(s). Try deleting the \"cache\" folder.\n";

            if (msg != string.Empty)
            {
                msg += "\nMore info can be found in the \"raid_dbg.txt\" file.";
                await ErrorHandler.DisplayMessageBox(this, Webhook, msg, token, "Raid Read Error").ConfigureAwait(false);
            }

            var raids = RaidContainer.Raids;
            var encounters = RaidContainer.Encounters;
            UpdateStatus("Completed!");

            var filterMatchCount = Enumerable.Range(0, raids.Count).Count(c => RaidFilters.Any(z => z.FilterSatisfied(RaidContainer, encounters[c], raids[c], GetRaidBoost())));
            var shinyCount = Enumerable.Range(0, raids.Count).Where(i => raids[i].CheckIsShiny(encounters[i])).Count();
            if (InvokeRequired)
                Invoke(() => { LabelLoadedRaids.Text = $"Met Filters/Shiny: {filterMatchCount}/{shinyCount}"; });
            else LabelLoadedRaids.Text = $"Met Filters/Shiny: {filterMatchCount}/{shinyCount}";

            if (raids.Count > 0)
            {
                ButtonEnable(new[] { ButtonPrevious, ButtonNext }, true);
                //var dataSource = Enumerable.Range(0, raids.Count).Select(z => $"{z + 1:D} / {raids.Count:D}").ToArray();
                var dataSource = GetComboList();
                if (InvokeRequired)
                    Invoke(() => { ComboIndex.DataSource = dataSource; });
                else ComboIndex.DataSource = dataSource;

                if (InvokeRequired)
                    Invoke(() => { ComboIndex.SelectedIndex = ComboIndex.SelectedIndex < raids.Count ? ComboIndex.SelectedIndex : 0; });
                else ComboIndex.SelectedIndex = ComboIndex.SelectedIndex < raids.Count ? ComboIndex.SelectedIndex : 0;
            }
            else
            {
                ButtonEnable(new[] { ButtonPrevious, ButtonNext }, false);
                if (raids.Count > RaidBlock.MAX_COUNT || raids.Count == 0)
                {
                    msg = "Bad read, ensure there are no cheats running or anything else that might shift RAM (Edizon, overlays, etc.), then reboot your console and try again.";
                    await ErrorHandler.DisplayMessageBox(this, Webhook, msg, token, "Raid Read Error").ConfigureAwait(false);
                }
            }
        }

        private List<string> GetComboList()
        {
            var nameList = new List<String>();
            var raids = RaidContainer.Raids;
            var encounters = RaidContainer.Encounters;
            var strings = GameInfo.GetStrings(1);

            for (var i = 0; raids.Count > i; i++)
            {
                var raid = raids[i];
                var encounter = encounters[i];
                var param = encounter.GetParam();
                var blank = new PK9
                {
                    Species = encounter.Species,
                    Form = encounter.Form
                };
                Encounter9RNG.GenerateData(blank, param, EncounterCriteria.Unrestricted, raid.Seed);
                var form = Utils.GetFormString(blank.Species, blank.Form, strings);
                var species = $"{strings.Species[encounter.Species]}";
                var shiny = $"{(raid.CheckIsShiny(encounter) ? (ShinyExtensions.IsSquareShinyExist(blank) ? "⛋" : "☆") : "")}";

                nameList.Add($"{shiny}{i + 1:D2} {species}{form}{shiny}");
            }

            return nameList;
        }

        public void Game_SelectedIndexChanged(string name)
        {
            Config.Game = name;
            RaidContainer.SetGame(name);
            if (RaidContainer.Raids.Count > 0)
                DisplayRaid();
        }

        public void Protocol_SelectedIndexChanged(SwitchProtocol protocol)
        {
            Config.Protocol = protocol;
            ConnectionConfig.Protocol = protocol;
            if (protocol is SwitchProtocol.USB)
            {
                InputSwitchIP.Visible = false;
                LabelSwitchIP.Visible = false;
                USB_Port_label.Visible = true;
                USB_Port_TB.Visible = true;
                ConnectionConfig.Port = Config.UsbPort;
            }
            else
            {
                InputSwitchIP.Visible = true;
                LabelSwitchIP.Visible = true;
                USB_Port_label.Visible = false;
                USB_Port_TB.Visible = false;
                ConnectionConfig.Port = 6000;
            }
        }

        private void DisplayMap(object sender, EventArgs e)
        {
            var raids = RaidContainer.Raids;
            if (raids.Count == 0)
            {
                Task.Run(async () => await ErrorHandler.DisplayMessageBox(this, Webhook, "Raids not loaded.", Source.Token).ConfigureAwait(false), Source.Token);
                return;
            }

            var raid = raids[ComboIndex.SelectedIndex];
            var encounter = RaidContainer.Encounters[ComboIndex.SelectedIndex];
            var teratype = raid.GetTeraType(encounter);
            var map = GenerateMap(raid, teratype);
            if (map is null)
            {
                Task.Run(async () => await ErrorHandler.DisplayMessageBox(this, Webhook, "Error generating map.", Source.Token).ConfigureAwait(false), Source.Token);
                return;
            }

            var form = new MapView(map);
            ShowDialog(form);
        }

        private void Rewards_Click(object sender, EventArgs e)
        {
            if (RaidContainer.Raids.Count == 0)
            {
                Task.Run(async () => await ErrorHandler.DisplayMessageBox(this, Webhook, "Raids not loaded.", Source.Token).ConfigureAwait(false), Source.Token);
                return;
            }

            var rewards = RaidContainer.Rewards[ComboIndex.SelectedIndex];
            if (rewards is null)
            {
                Task.Run(async () => await ErrorHandler.DisplayMessageBox(this, Webhook, "Error while displaying rewards.", Source.Token).ConfigureAwait(false), Source.Token);
                return;
            }

            var form = new RewardsView(RaidContainer.Strings.Item, rewards);
            ShowDialog(form);
        }

        private void RaidBoost_SelectedIndexChanged(object sender, EventArgs e)
        {
            RaidContainer.ClearRewards();
            var raids = RaidContainer.Raids;
            var encounters = RaidContainer.Encounters;

            List<List<(int, int, int)>> newRewards = new();
            for (int i = 0; i < raids.Count; i++)
            {
                var raid = raids[i];
                var encounter = encounters[i];
                newRewards.Add(encounter.GetRewards(RaidContainer, raid, RaidBoost.SelectedIndex));
            }
            RaidContainer.SetRewards(newRewards);
        }

        private void Move_Clicked(object sender, EventArgs e)
        {
            if (RaidContainer.Raids.Count == 0)
            {
                Task.Run(async () => await ErrorHandler.DisplayMessageBox(this, Webhook, "Raids not loaded.", Source.Token).ConfigureAwait(false), Source.Token);
                return;
            }

            var encounter = RaidContainer.Encounters[ComboIndex.SelectedIndex];
            if (encounter is null)
                return;

            ShowExtraMoves ^= true;
            LabelMoves.Text = ShowExtraMoves ? "Extra:" : "Moves:";
            LabelMoves.Location = new(LabelMoves.Location.X + (ShowExtraMoves ? 9 : -9), LabelMoves.Location.Y);

            var length = encounter.ExtraMoves.Length < 4 ? 4 : encounter.ExtraMoves.Length;
            var extra_moves = new ushort[length];
            for (int i = 0; i < encounter.ExtraMoves.Length; i++)
                extra_moves[i] = encounter.ExtraMoves[i];

            Move1.Text = ShowExtraMoves ? RaidContainer.Strings.Move[extra_moves[0]] : RaidContainer.Strings.Move[encounter.Move1];
            Move2.Text = ShowExtraMoves ? RaidContainer.Strings.Move[extra_moves[1]] : RaidContainer.Strings.Move[encounter.Move2];
            Move3.Text = ShowExtraMoves ? RaidContainer.Strings.Move[extra_moves[2]] : RaidContainer.Strings.Move[encounter.Move3];
            Move4.Text = ShowExtraMoves ? RaidContainer.Strings.Move[extra_moves[3]] : RaidContainer.Strings.Move[encounter.Move4];
        }

        private void ComboIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RaidContainer.Raids.Count == 0)
                return;

            DisplayRaid();
            if (Config.StreamerView)
                DisplayPrettyRaid();
        }

        private void SendScreenshot_Click(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Webhook.SendScreenshot(ConnectionWrapper.Connection, Source.Token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await ErrorHandler.DisplayMessageBox(this, Webhook, $"Could not send the screenshot: {ex.Message}", Source.Token).ConfigureAwait(false);
                }
            }, Source.Token);
        }

        private void SearchTimer_Elapsed(object sender, EventArgs e)
        {

            var timeSpan = stopwatch.Elapsed;
            /*string time = string.Format("{0:00}:{1:00}:{2:00}:{3:00}",
            timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);*/
            var timeEmpty = new TimeSpan(0, 0, 0, 0);
            string time = string.Empty;
            if (((int)timeSpan.TotalDays) != timeEmpty.TotalDays) { time = timeSpan.ToString(@"d\d\ %h\h\ mm\m\ ss\s"); }
            else if (((int)timeSpan.TotalHours) != timeEmpty.TotalHours) { time = timeSpan.ToString(@"%h\h\ mm\m\ ss\s"); }
            else if (((int)timeSpan.TotalMinutes) != timeEmpty.TotalMinutes) { time = timeSpan.ToString(@"%m\m\ ss\s"); }
            else { time = timeSpan.ToString(@"%s\s"); }

            if (Config.SearchTimeInTitle)
                Invoke(() => Text = formTitle + " [Searching for " + time + "]");
            SearchTime.Text = "Searching for: " + time;
            if (Config.StreamerView && teraRaidView is not null)
                Invoke(() => teraRaidView.textSearchTime.Text = time);
        }

        public void TestWebhook()
        {
            Task.Run(async () => await TestWebhookAsync(Source.Token).ConfigureAwait(false), Source.Token);
        }

        private async Task TestWebhookAsync(CancellationToken token)
        {
            var filter = new RaidFilter { Name = "Test Webhook" };

            int i = -1;
            if (InvokeRequired)
                i = Invoke(() => { return ComboIndex.SelectedIndex; });
            else i = ComboIndex.SelectedIndex;

            var raids = RaidContainer.Raids;
            var encounters = RaidContainer.Encounters;
            var rewards = RaidContainer.Rewards;
            if (i > -1 && encounters[i] is not null && raids[i] is not null)
            {
                var timeSpan = stopwatch.Elapsed;
                var timeEmpty = new TimeSpan(0, 0, 0, 0);
                string time = string.Empty;
                if (((int)timeSpan.TotalDays) != timeEmpty.TotalDays) { time = timeSpan.ToString(@"d\d\ %h\h\ mm\m\ ss\s"); }
                else if (((int)timeSpan.TotalHours) != timeEmpty.TotalHours) { time = timeSpan.ToString(@"%h\h\ mm\m\ ss\s"); }
                else if (((int)timeSpan.TotalMinutes) != timeEmpty.TotalMinutes) { time = timeSpan.ToString(@"%m\m\ ss\s"); }
                else { time = timeSpan.ToString(@"%s\s"); }
                var teraType = raids[i].GetTeraType(encounters[i]);
                var color = TypeColor.GetTypeSpriteColor((byte)teraType);
                var hexColor = $"{color.R:X2}{color.G:X2}{color.B:X2}";

                var blank = new PK9
                {
                    Species = encounters[i].Species,
                    Form = encounters[i].Form,
                    Gender = encounters[i].Gender,
                };
                blank.SetSuggestedFormArgument();

                var spriteName = GetSpriteNameForUrl(blank, raids[i].CheckIsShiny(encounters[i]));
                await Webhook.SendNotification(encounters[i], raids[i], filter, time, rewards[i], hexColor, spriteName, token).ConfigureAwait(false);
                return;
            }

            await ErrorHandler.DisplayMessageBox(this, Webhook, "Please connect to your device and ensure a raid has been found.", token).ConfigureAwait(false);
        }

        public void ToggleStreamerView()
        {
            if (Config.StreamerView)
            {
                teraRaidView = new();
                teraRaidView.Map.Image = map;
                teraRaidView.Show();
            }
            else if (!Config.StreamerView && teraRaidView is not null)
                teraRaidView.Close();
        }

        private static string GetSpriteNameForUrl(PK9 pk, bool shiny)
        {
            // Since we're later using this for URL assembly later, we need dashes instead of underscores for forms.
            var spriteName = SpriteName.GetResourceStringSprite(pk.Species, pk.Form, pk.Gender, pk.FormArgument, EntityContext.Gen9, shiny)[1..];
            return spriteName.Replace('_', '-').Insert(0, "_");
        }

        private void CopyAnnounce_Click(object sender, EventArgs e)
        {

            var filter = new RaidFilter { Name = "Test Webhook" };

            int i = -1;
            if (InvokeRequired)
                i = Invoke(() => { return ComboIndex.SelectedIndex; });
            else i = ComboIndex.SelectedIndex;

            var raids = RaidContainer.Raids;
            var encounters = RaidContainer.Encounters;
            var rewards = RaidContainer.Rewards;
            if (i > -1 && encounters[i] is not null && raids[i] is not null)
            {
                var timeSpan = stopwatch.Elapsed;
                var timeEmpty = new TimeSpan(0, 0, 0, 0);
                string time = string.Empty;
                if (((int)timeSpan.TotalDays) != timeEmpty.TotalDays) { time = timeSpan.ToString(@"d\d\ %h\h\ mm\m\ ss\s"); }
                else if (((int)timeSpan.TotalHours) != timeEmpty.TotalHours) { time = timeSpan.ToString(@"%h\h\ mm\m\ ss\s"); }
                else if (((int)timeSpan.TotalMinutes) != timeEmpty.TotalMinutes) { time = timeSpan.ToString(@"%m\m\ ss\s"); }
                else { time = timeSpan.ToString(@"%s\s"); }
                var teraType = raids[i].GetTeraType(encounters[i]);
                var color = TypeColor.GetTypeSpriteColor((byte)teraType);
                var hexColor = $"{color.R:X2}{color.G:X2}{color.B:X2}";

                var blank = new PK9
                {
                    Species = encounters[i].Species,
                    Form = encounters[i].Form,
                    Gender = encounters[i].Gender,
                };
                blank.SetSuggestedFormArgument();

                var spriteName = GetSpriteNameForUrl(blank, raids[i].CheckIsShiny(encounters[i]));

                if (ModifierKeys == Keys.Control)
                    Clipboard.SetText(Webhook.GetAnnouncement(encounters[i], raids[i], filter, time, rewards[i], hexColor, spriteName, "technicalcopy"));
                else if (ModifierKeys == (Keys.Shift | Keys.Control))
                    Task.Run(async () => await Webhook.SendNotification(encounters[i], raids[i], filter, time, rewards[i], hexColor, spriteName, Source.Token));
                else if (ModifierKeys == (Keys.Shift | Keys.Control | Keys.Alt))
                    Task.Run(async () => await FomoWebhook.SendFomoNotification(encounters[i], raids[i], filter, time, rewards[i], hexColor, spriteName, Source.Token));
                else
                    Clipboard.SetText(Webhook.GetAnnouncement(encounters[i], raids[i], filter, time, rewards[i], hexColor, spriteName, "copy"));
                return;
            }
        }

        private void Protocol_dropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.Protocol = (SysBot.Base.SwitchProtocol)Protocol_dropdown.SelectedIndex;
            Protocol_SelectedIndexChanged(Config.Protocol);
            WriteConfig();
        }

        private async void ButtonScreenState_Click(object sender, EventArgs e)
        {
            ButtonScreenState.Text = $"{(ConnectionWrapper.screenState ? "Screen Off" : "Screen On")}";
            await ConnectionWrapper.ScreenToggle(Source.Token).ConfigureAwait(false);
        }

        public void UpdateWebhook(ClientConfig config)
        {
            Webhook = new(config);
            FomoWebhook = new(config, true);
        }

        public void WriteConfig()
        {
            JsonSerializerOptions options = new() { WriteIndented = true };
            string output = JsonSerializer.Serialize(Config, options);
            using StreamWriter sw = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"));
            sw.Write(output);
        }

        private void LabelShinyCount_Click(object sender, EventArgs e)
        {
            FomoCount++;
            //LabelShinyCount.Text = LabelShinyCount.Text + "" + FomoCount.ToString();
            if (FomoCount == 7)
            {
                FomoCount = 0;
                Config.SaveOnFomo = !Config.SaveOnFomo;
                LabelShinyCount.Text = $"FoMO Saves: {(Config.SaveOnFomo ? "On" : "Off")}";

                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer()
                {
                    Interval = 800,
                    Enabled = true
                };

                timer.Tick += (sender, e) =>
                {
                    LabelShinyCount.Text = $"Shinies Missed: {GetStatShinyCount()}";
                    WriteConfig();
                    timer.Dispose();
                };
            }
        }

    }
}
