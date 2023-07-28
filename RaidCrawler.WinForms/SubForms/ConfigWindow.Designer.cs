﻿using SysBot.Base;

namespace RaidCrawler.WinForms.SubForms
{
    partial class ConfigWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            L_AdvanceDate = new Label();
            L_BaseDelay = new Label();
            SystemDDownPresses = new NumericUpDown();
            L_DdownInput = new Label();
            NavigateToSettings = new NumericUpDown();
            OpenSettings = new NumericUpDown();
            OpenHome = new NumericUpDown();
            L_OpenHOME = new Label();
            L_NavigateSettings = new Label();
            L_OpenSettingsDelay = new Label();
            L_ScrollSystem = new Label();
            Hold = new NumericUpDown();
            L_SubmenuDelay = new Label();
            Submenu = new NumericUpDown();
            L_DateChangeDelay = new Label();
            DateChange = new NumericUpDown();
            L_ReturnHomeDelay = new Label();
            ReturnHome = new NumericUpDown();
            L_ReOpenGameDelay = new Label();
            ReturnGame = new NumericUpDown();
            L_DaysToSkip = new Label();
            DaysToSkip = new NumericUpDown();
            UseTouch = new CheckBox();
            tabControl1 = new TabControl();
            tabGeneral = new TabPage();
            TitleSearchTime = new CheckBox();
            CopyEmoji = new CheckBox();
            MapBackground = new CheckBox();
            label13 = new Label();
            ExperimentalView = new CheckBox();
            InstanceName = new TextBox();
            label17 = new Label();
            Protocol_dropdown = new ComboBox();
            Protocol_label = new Label();
            label23 = new Label();
            LabelEventProgress = new Label();
            EventProgress = new ComboBox();
            LabelGame = new Label();
            Game = new ComboBox();
            LabelStoryProgress = new Label();
            StoryProgress = new ComboBox();
            tabMatch = new TabPage();
            SaveGame = new CheckBox();
            LabelMatchFound = new Label();
            FocusWindow = new CheckBox();
            EnableAlert = new CheckBox();
            PlayTone = new CheckBox();
            AlertMessage = new TextBox();
            tabAdvanceDate = new TabPage();
            UseSetStick = new CheckBox();
            DodgeSystemUpdate = new CheckBox();
            SaveGameDelay = new NumericUpDown();
            L_SaveGame = new Label();
            L_OvershootHold = new Label();
            SystemOvershoot = new NumericUpDown();
            UseOvershoot = new CheckBox();
            BaseDelay = new NumericUpDown();
            tabWebhook = new TabPage();
            EnableFomoNotifications = new CheckBox();
            FomoWebhook = new TextBox();
            EnableDiscordNotifications = new CheckBox();
            DiscordWebhook = new TextBox();
            IVspacerLabel = new Label();
            IVspacer = new TextBox();
            EmojiConfig = new Button();
            labelWebhooks = new Label();
            label21 = new Label();
            DiscordMessageContent = new TextBox();
            label14 = new Label();
            btnTestWebHook = new Button();
            denToggle = new CheckBox();
            LocationSettings_label = new Label();
            IVstyle = new ComboBox();
            IVverbose = new CheckBox();
            label19 = new Label();
            label18 = new Label();
            EnableEmoji = new CheckBox();
            tabAbout = new TabPage();
            label2 = new Label();
            label1 = new Label();
            linkLabel2 = new LinkLabel();
            linkLabel1 = new LinkLabel();
            labelAppName = new Label();
            picAppIcon = new PictureBox();
            labelAppVersion = new Label();
            Save = new Button();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)SystemDDownPresses).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NavigateToSettings).BeginInit();
            ((System.ComponentModel.ISupportInitialize)OpenSettings).BeginInit();
            ((System.ComponentModel.ISupportInitialize)OpenHome).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Hold).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Submenu).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DateChange).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ReturnHome).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ReturnGame).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DaysToSkip).BeginInit();
            tabControl1.SuspendLayout();
            tabGeneral.SuspendLayout();
            tabMatch.SuspendLayout();
            tabAdvanceDate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SaveGameDelay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SystemOvershoot).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BaseDelay).BeginInit();
            tabWebhook.SuspendLayout();
            tabAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picAppIcon).BeginInit();
            SuspendLayout();
            // 
            // L_AdvanceDate
            // 
            L_AdvanceDate.AutoSize = true;
            L_AdvanceDate.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            L_AdvanceDate.Location = new Point(6, 3);
            L_AdvanceDate.Name = "L_AdvanceDate";
            L_AdvanceDate.Size = new Size(233, 15);
            L_AdvanceDate.TabIndex = 6;
            L_AdvanceDate.Text = "Advance Date Options (all timings in ms):";
            // 
            // L_BaseDelay
            // 
            L_BaseDelay.AutoSize = true;
            L_BaseDelay.Location = new Point(6, 109);
            L_BaseDelay.Name = "L_BaseDelay";
            L_BaseDelay.Size = new Size(196, 15);
            L_BaseDelay.TabIndex = 8;
            L_BaseDelay.Text = "Base delay to be added to all inputs:";
            // 
            // SystemDDownPresses
            // 
            SystemDDownPresses.Location = new Point(265, 252);
            SystemDDownPresses.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            SystemDDownPresses.Name = "SystemDDownPresses";
            SystemDDownPresses.Size = new Size(68, 23);
            SystemDDownPresses.TabIndex = 10;
            SystemDDownPresses.Value = new decimal(new int[] { 38, 0, 0, 0 });
            // 
            // L_DdownInput
            // 
            L_DdownInput.AutoSize = true;
            L_DdownInput.Location = new Point(6, 254);
            L_DdownInput.Name = "L_DdownInput";
            L_DdownInput.Size = new Size(228, 15);
            L_DdownInput.TabIndex = 11;
            L_DdownInput.Text = "DDOWN inputs to get to \"Date and Time\":";
            // 
            // NavigateToSettings
            // 
            NavigateToSettings.Location = new Point(265, 165);
            NavigateToSettings.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            NavigateToSettings.Name = "NavigateToSettings";
            NavigateToSettings.Size = new Size(68, 23);
            NavigateToSettings.TabIndex = 16;
            NavigateToSettings.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // OpenSettings
            // 
            OpenSettings.Location = new Point(265, 194);
            OpenSettings.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            OpenSettings.Name = "OpenSettings";
            OpenSettings.Size = new Size(68, 23);
            OpenSettings.TabIndex = 17;
            OpenSettings.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            // 
            // OpenHome
            // 
            OpenHome.Location = new Point(265, 136);
            OpenHome.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            OpenHome.Name = "OpenHome";
            OpenHome.Size = new Size(68, 23);
            OpenHome.TabIndex = 18;
            OpenHome.Value = new decimal(new int[] { 1800, 0, 0, 0 });
            // 
            // L_OpenHOME
            // 
            L_OpenHOME.AutoSize = true;
            L_OpenHOME.Location = new Point(6, 138);
            L_OpenHOME.Name = "L_OpenHOME";
            L_OpenHOME.Size = new Size(140, 15);
            L_OpenHOME.TabIndex = 19;
            L_OpenHOME.Text = "Open Home Menu delay:";
            // 
            // L_NavigateSettings
            // 
            L_NavigateSettings.AutoSize = true;
            L_NavigateSettings.Location = new Point(6, 167);
            L_NavigateSettings.Name = "L_NavigateSettings";
            L_NavigateSettings.Size = new Size(146, 15);
            L_NavigateSettings.TabIndex = 20;
            L_NavigateSettings.Text = "Navigate to settings delay:";
            // 
            // L_OpenSettingsDelay
            // 
            L_OpenSettingsDelay.AutoSize = true;
            L_OpenSettingsDelay.Location = new Point(6, 196);
            L_OpenSettingsDelay.Name = "L_OpenSettingsDelay";
            L_OpenSettingsDelay.Size = new Size(114, 15);
            L_OpenSettingsDelay.TabIndex = 21;
            L_OpenSettingsDelay.Text = "Open settings delay:";
            // 
            // L_ScrollSystem
            // 
            L_ScrollSystem.AutoSize = true;
            L_ScrollSystem.Location = new Point(6, 225);
            L_ScrollSystem.Name = "L_ScrollSystem";
            L_ScrollSystem.Size = new Size(187, 15);
            L_ScrollSystem.TabIndex = 23;
            L_ScrollSystem.Text = "Time to hold to scroll to \"System\":";
            // 
            // Hold
            // 
            Hold.Location = new Point(265, 223);
            Hold.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            Hold.Name = "Hold";
            Hold.Size = new Size(68, 23);
            Hold.TabIndex = 22;
            Hold.Value = new decimal(new int[] { 1700, 0, 0, 0 });
            // 
            // L_SubmenuDelay
            // 
            L_SubmenuDelay.AutoSize = true;
            L_SubmenuDelay.Location = new Point(6, 283);
            L_SubmenuDelay.Name = "L_SubmenuDelay";
            L_SubmenuDelay.Size = new Size(123, 15);
            L_SubmenuDelay.TabIndex = 25;
            L_SubmenuDelay.Text = "Open submenu delay:";
            // 
            // Submenu
            // 
            Submenu.Location = new Point(265, 281);
            Submenu.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            Submenu.Name = "Submenu";
            Submenu.Size = new Size(68, 23);
            Submenu.TabIndex = 24;
            Submenu.Value = new decimal(new int[] { 800, 0, 0, 0 });
            // 
            // L_DateChangeDelay
            // 
            L_DateChangeDelay.AutoSize = true;
            L_DateChangeDelay.Location = new Point(6, 312);
            L_DateChangeDelay.Name = "L_DateChangeDelay";
            L_DateChangeDelay.Size = new Size(138, 15);
            L_DateChangeDelay.TabIndex = 27;
            L_DateChangeDelay.Text = "Open date change delay:";
            // 
            // DateChange
            // 
            DateChange.Location = new Point(265, 310);
            DateChange.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            DateChange.Name = "DateChange";
            DateChange.Size = new Size(68, 23);
            DateChange.TabIndex = 26;
            DateChange.Value = new decimal(new int[] { 500, 0, 0, 0 });
            // 
            // L_ReturnHomeDelay
            // 
            L_ReturnHomeDelay.AutoSize = true;
            L_ReturnHomeDelay.Location = new Point(6, 370);
            L_ReturnHomeDelay.Name = "L_ReturnHomeDelay";
            L_ReturnHomeDelay.Size = new Size(160, 15);
            L_ReturnHomeDelay.TabIndex = 29;
            L_ReturnHomeDelay.Text = "Return to Home Menu delay:";
            // 
            // ReturnHome
            // 
            ReturnHome.Location = new Point(265, 368);
            ReturnHome.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            ReturnHome.Name = "ReturnHome";
            ReturnHome.Size = new Size(68, 23);
            ReturnHome.TabIndex = 28;
            ReturnHome.Value = new decimal(new int[] { 2500, 0, 0, 0 });
            // 
            // L_ReOpenGameDelay
            // 
            L_ReOpenGameDelay.AutoSize = true;
            L_ReOpenGameDelay.Location = new Point(6, 399);
            L_ReOpenGameDelay.Name = "L_ReOpenGameDelay";
            L_ReOpenGameDelay.Size = new Size(119, 15);
            L_ReOpenGameDelay.TabIndex = 31;
            L_ReOpenGameDelay.Text = "Re-open game delay:";
            // 
            // ReturnGame
            // 
            ReturnGame.Location = new Point(265, 397);
            ReturnGame.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            ReturnGame.Name = "ReturnGame";
            ReturnGame.Size = new Size(68, 23);
            ReturnGame.TabIndex = 30;
            ReturnGame.Value = new decimal(new int[] { 4000, 0, 0, 0 });
            // 
            // L_DaysToSkip
            // 
            L_DaysToSkip.AutoSize = true;
            L_DaysToSkip.Location = new Point(6, 341);
            L_DaysToSkip.Name = "L_DaysToSkip";
            L_DaysToSkip.Size = new Size(179, 15);
            L_DaysToSkip.TabIndex = 33;
            L_DaysToSkip.Text = "Number of days/months to skip:";
            // 
            // DaysToSkip
            // 
            DaysToSkip.Location = new Point(265, 339);
            DaysToSkip.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            DaysToSkip.Name = "DaysToSkip";
            DaysToSkip.Size = new Size(68, 23);
            DaysToSkip.TabIndex = 32;
            // 
            // UseTouch
            // 
            UseTouch.AutoSize = true;
            UseTouch.Location = new Point(8, 22);
            UseTouch.Name = "UseTouch";
            UseTouch.Size = new Size(267, 19);
            UseTouch.TabIndex = 36;
            UseTouch.Text = "Use touch screen inputs (faster, experimental)";
            UseTouch.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabGeneral);
            tabControl1.Controls.Add(tabMatch);
            tabControl1.Controls.Add(tabAdvanceDate);
            tabControl1.Controls.Add(tabWebhook);
            tabControl1.Controls.Add(tabAbout);
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(349, 510);
            tabControl1.TabIndex = 41;
            // 
            // tabGeneral
            // 
            tabGeneral.Controls.Add(TitleSearchTime);
            tabGeneral.Controls.Add(CopyEmoji);
            tabGeneral.Controls.Add(MapBackground);
            tabGeneral.Controls.Add(label13);
            tabGeneral.Controls.Add(ExperimentalView);
            tabGeneral.Controls.Add(InstanceName);
            tabGeneral.Controls.Add(label17);
            tabGeneral.Controls.Add(Protocol_dropdown);
            tabGeneral.Controls.Add(Protocol_label);
            tabGeneral.Controls.Add(label23);
            tabGeneral.Controls.Add(LabelEventProgress);
            tabGeneral.Controls.Add(EventProgress);
            tabGeneral.Controls.Add(LabelGame);
            tabGeneral.Controls.Add(Game);
            tabGeneral.Controls.Add(LabelStoryProgress);
            tabGeneral.Controls.Add(StoryProgress);
            tabGeneral.Location = new Point(4, 24);
            tabGeneral.Name = "tabGeneral";
            tabGeneral.Padding = new Padding(3);
            tabGeneral.Size = new Size(341, 482);
            tabGeneral.TabIndex = 5;
            tabGeneral.Text = "General";
            tabGeneral.UseVisualStyleBackColor = true;
            // 
            // TitleSearchTime
            // 
            TitleSearchTime.AutoSize = true;
            TitleSearchTime.Location = new Point(8, 273);
            TitleSearchTime.Name = "TitleSearchTime";
            TitleSearchTime.Size = new Size(170, 19);
            TitleSearchTime.TabIndex = 118;
            TitleSearchTime.Text = "Include Search Time in Title";
            TitleSearchTime.UseVisualStyleBackColor = true;
            // 
            // CopyEmoji
            // 
            CopyEmoji.AutoSize = true;
            CopyEmoji.Location = new Point(8, 253);
            CopyEmoji.Name = "CopyEmoji";
            CopyEmoji.Size = new Size(174, 19);
            CopyEmoji.TabIndex = 117;
            CopyEmoji.Text = "Enable Copy Message Emoji";
            CopyEmoji.UseVisualStyleBackColor = true;
            // 
            // MapBackground
            // 
            MapBackground.AutoSize = true;
            MapBackground.Location = new Point(8, 233);
            MapBackground.Name = "MapBackground";
            MapBackground.Size = new Size(169, 19);
            MapBackground.TabIndex = 116;
            MapBackground.Text = "Enable Map as Background";
            MapBackground.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label13.Location = new Point(8, 193);
            label13.Name = "label13";
            label13.Size = new Size(87, 15);
            label13.TabIndex = 114;
            label13.Text = "Miscellaneous ";
            // 
            // ExperimentalView
            // 
            ExperimentalView.AutoSize = true;
            ExperimentalView.Location = new Point(8, 213);
            ExperimentalView.Name = "ExperimentalView";
            ExperimentalView.Size = new Size(284, 19);
            ExperimentalView.TabIndex = 115;
            ExperimentalView.Text = "Toggle Streamer Tera Raid View (Requires Restart)";
            ExperimentalView.UseVisualStyleBackColor = true;
            // 
            // InstanceName
            // 
            InstanceName.Location = new Point(6, 21);
            InstanceName.Name = "InstanceName";
            InstanceName.Size = new Size(327, 23);
            InstanceName.TabIndex = 113;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label17.Location = new Point(8, 3);
            label17.Name = "label17";
            label17.Size = new Size(93, 15);
            label17.TabIndex = 112;
            label17.Text = "Instance Name:";
            // 
            // Protocol_dropdown
            // 
            Protocol_dropdown.FormattingEnabled = true;
            Protocol_dropdown.Items.AddRange(new object[] { SwitchProtocol.WiFi, SwitchProtocol.USB });
            Protocol_dropdown.Location = new Point(157, 161);
            Protocol_dropdown.MaxDropDownItems = 2;
            Protocol_dropdown.Name = "Protocol_dropdown";
            Protocol_dropdown.Size = new Size(48, 23);
            Protocol_dropdown.TabIndex = 111;
            Protocol_dropdown.Text = "w";
            Protocol_dropdown.SelectedValueChanged += Protocol_Changed;
            // 
            // Protocol_label
            // 
            Protocol_label.AutoSize = true;
            Protocol_label.Location = new Point(5, 164);
            Protocol_label.Name = "Protocol_label";
            Protocol_label.Size = new Size(120, 15);
            Protocol_label.TabIndex = 110;
            Protocol_label.Text = "Connection Protocol:";
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label23.Location = new Point(8, 56);
            label23.Name = "label23";
            label23.Size = new Size(89, 15);
            label23.TabIndex = 109;
            label23.Text = "Game Settings";
            // 
            // LabelEventProgress
            // 
            LabelEventProgress.AutoSize = true;
            LabelEventProgress.Location = new Point(5, 136);
            LabelEventProgress.Name = "LabelEventProgress";
            LabelEventProgress.Size = new Size(117, 15);
            LabelEventProgress.TabIndex = 108;
            LabelEventProgress.Text = "Event Progress Level:";
            // 
            // EventProgress
            // 
            EventProgress.Enabled = false;
            EventProgress.FormattingEnabled = true;
            EventProgress.Items.AddRange(new object[] { "1", "2", "3", "4" });
            EventProgress.Location = new Point(157, 133);
            EventProgress.Name = "EventProgress";
            EventProgress.Size = new Size(48, 23);
            EventProgress.TabIndex = 107;
            EventProgress.Text = "w";
            // 
            // LabelGame
            // 
            LabelGame.AutoSize = true;
            LabelGame.Location = new Point(6, 81);
            LabelGame.Name = "LabelGame";
            LabelGame.Size = new Size(41, 15);
            LabelGame.TabIndex = 106;
            LabelGame.Text = "Game:";
            // 
            // Game
            // 
            Game.FormattingEnabled = true;
            Game.Items.AddRange(new object[] { "Scarlet", "Violet" });
            Game.Location = new Point(109, 78);
            Game.Name = "Game";
            Game.Size = new Size(96, 23);
            Game.TabIndex = 105;
            Game.Text = "w";
            Game.SelectedIndexChanged += Game_SelectedIndexChanged;
            // 
            // LabelStoryProgress
            // 
            LabelStoryProgress.AutoSize = true;
            LabelStoryProgress.Location = new Point(6, 108);
            LabelStoryProgress.Name = "LabelStoryProgress";
            LabelStoryProgress.Size = new Size(115, 15);
            LabelStoryProgress.TabIndex = 104;
            LabelStoryProgress.Text = "Story Progress Level:";
            // 
            // StoryProgress
            // 
            StoryProgress.Enabled = false;
            StoryProgress.FormattingEnabled = true;
            StoryProgress.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            StoryProgress.Location = new Point(157, 105);
            StoryProgress.Name = "StoryProgress";
            StoryProgress.Size = new Size(48, 23);
            StoryProgress.TabIndex = 103;
            StoryProgress.Text = "w";
            // 
            // tabMatch
            // 
            tabMatch.Controls.Add(SaveGame);
            tabMatch.Controls.Add(LabelMatchFound);
            tabMatch.Controls.Add(FocusWindow);
            tabMatch.Controls.Add(EnableAlert);
            tabMatch.Controls.Add(PlayTone);
            tabMatch.Controls.Add(AlertMessage);
            tabMatch.Location = new Point(4, 24);
            tabMatch.Name = "tabMatch";
            tabMatch.Padding = new Padding(3);
            tabMatch.Size = new Size(341, 482);
            tabMatch.TabIndex = 0;
            tabMatch.Text = "Match";
            tabMatch.UseVisualStyleBackColor = true;
            // 
            // SaveGame
            // 
            SaveGame.AutoSize = true;
            SaveGame.Location = new Point(8, 113);
            SaveGame.Name = "SaveGame";
            SaveGame.Size = new Size(172, 19);
            SaveGame.TabIndex = 42;
            SaveGame.Text = "Save game on match found";
            SaveGame.UseVisualStyleBackColor = true;
            // 
            // LabelMatchFound
            // 
            LabelMatchFound.AutoSize = true;
            LabelMatchFound.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            LabelMatchFound.Location = new Point(6, 3);
            LabelMatchFound.Name = "LabelMatchFound";
            LabelMatchFound.Size = new Size(137, 15);
            LabelMatchFound.TabIndex = 3;
            LabelMatchFound.Text = "When a match is found:";
            // 
            // FocusWindow
            // 
            FocusWindow.AutoSize = true;
            FocusWindow.Location = new Point(8, 44);
            FocusWindow.Name = "FocusWindow";
            FocusWindow.Size = new Size(123, 19);
            FocusWindow.TabIndex = 1;
            FocusWindow.Text = "Focus RaidCrawler";
            FocusWindow.UseVisualStyleBackColor = true;
            // 
            // EnableAlert
            // 
            EnableAlert.AutoSize = true;
            EnableAlert.Location = new Point(8, 65);
            EnableAlert.Name = "EnableAlert";
            EnableAlert.Size = new Size(293, 19);
            EnableAlert.TabIndex = 2;
            EnableAlert.Text = "Show an alert window with the following message:";
            EnableAlert.UseVisualStyleBackColor = true;
            EnableAlert.CheckedChanged += EnableAlert_CheckedChanged;
            // 
            // PlayTone
            // 
            PlayTone.AutoSize = true;
            PlayTone.Location = new Point(8, 23);
            PlayTone.Name = "PlayTone";
            PlayTone.Size = new Size(84, 19);
            PlayTone.TabIndex = 0;
            PlayTone.Text = "Play a tone";
            PlayTone.UseVisualStyleBackColor = true;
            // 
            // AlertMessage
            // 
            AlertMessage.Location = new Point(8, 84);
            AlertMessage.Name = "AlertMessage";
            AlertMessage.Size = new Size(327, 23);
            AlertMessage.TabIndex = 4;
            // 
            // tabAdvanceDate
            // 
            tabAdvanceDate.Controls.Add(UseSetStick);
            tabAdvanceDate.Controls.Add(DodgeSystemUpdate);
            tabAdvanceDate.Controls.Add(SaveGameDelay);
            tabAdvanceDate.Controls.Add(L_SaveGame);
            tabAdvanceDate.Controls.Add(L_OvershootHold);
            tabAdvanceDate.Controls.Add(SystemOvershoot);
            tabAdvanceDate.Controls.Add(UseOvershoot);
            tabAdvanceDate.Controls.Add(L_AdvanceDate);
            tabAdvanceDate.Controls.Add(L_BaseDelay);
            tabAdvanceDate.Controls.Add(UseTouch);
            tabAdvanceDate.Controls.Add(SystemDDownPresses);
            tabAdvanceDate.Controls.Add(L_DdownInput);
            tabAdvanceDate.Controls.Add(L_DaysToSkip);
            tabAdvanceDate.Controls.Add(NavigateToSettings);
            tabAdvanceDate.Controls.Add(DaysToSkip);
            tabAdvanceDate.Controls.Add(OpenSettings);
            tabAdvanceDate.Controls.Add(L_ReOpenGameDelay);
            tabAdvanceDate.Controls.Add(OpenHome);
            tabAdvanceDate.Controls.Add(ReturnGame);
            tabAdvanceDate.Controls.Add(L_OpenHOME);
            tabAdvanceDate.Controls.Add(L_ReturnHomeDelay);
            tabAdvanceDate.Controls.Add(L_NavigateSettings);
            tabAdvanceDate.Controls.Add(ReturnHome);
            tabAdvanceDate.Controls.Add(L_OpenSettingsDelay);
            tabAdvanceDate.Controls.Add(L_DateChangeDelay);
            tabAdvanceDate.Controls.Add(Hold);
            tabAdvanceDate.Controls.Add(DateChange);
            tabAdvanceDate.Controls.Add(L_ScrollSystem);
            tabAdvanceDate.Controls.Add(L_SubmenuDelay);
            tabAdvanceDate.Controls.Add(Submenu);
            tabAdvanceDate.Controls.Add(BaseDelay);
            tabAdvanceDate.Location = new Point(4, 24);
            tabAdvanceDate.Name = "tabAdvanceDate";
            tabAdvanceDate.Padding = new Padding(3);
            tabAdvanceDate.Size = new Size(341, 482);
            tabAdvanceDate.TabIndex = 1;
            tabAdvanceDate.Text = "Advance Date";
            tabAdvanceDate.UseVisualStyleBackColor = true;
            // 
            // UseSetStick
            // 
            UseSetStick.AutoSize = true;
            UseSetStick.Location = new Point(8, 62);
            UseSetStick.Name = "UseSetStick";
            UseSetStick.Size = new Size(222, 19);
            UseSetStick.TabIndex = 45;
            UseSetStick.Text = "Use SetStick instead of PressAndHold";
            UseSetStick.UseVisualStyleBackColor = true;
            UseSetStick.CheckedChanged += UseSetStick_CheckedChanged;
            // 
            // DodgeSystemUpdate
            // 
            DodgeSystemUpdate.AutoSize = true;
            DodgeSystemUpdate.Location = new Point(8, 82);
            DodgeSystemUpdate.Name = "DodgeSystemUpdate";
            DodgeSystemUpdate.Size = new Size(184, 19);
            DodgeSystemUpdate.TabIndex = 44;
            DodgeSystemUpdate.Text = "Dodge system update prompt";
            DodgeSystemUpdate.UseVisualStyleBackColor = true;
            // 
            // SaveGameDelay
            // 
            SaveGameDelay.Location = new Point(265, 455);
            SaveGameDelay.Name = "SaveGameDelay";
            SaveGameDelay.Size = new Size(68, 23);
            SaveGameDelay.TabIndex = 43;
            // 
            // L_SaveGame
            // 
            L_SaveGame.AutoSize = true;
            L_SaveGame.Location = new Point(6, 457);
            L_SaveGame.Name = "L_SaveGame";
            L_SaveGame.Size = new Size(186, 15);
            L_SaveGame.TabIndex = 42;
            L_SaveGame.Text = "Time to wait for the game to save:";
            // 
            // L_OvershootHold
            // 
            L_OvershootHold.AutoSize = true;
            L_OvershootHold.Location = new Point(6, 428);
            L_OvershootHold.Name = "L_OvershootHold";
            L_OvershootHold.Size = new Size(236, 15);
            L_OvershootHold.TabIndex = 40;
            L_OvershootHold.Text = "Time to hold to overshoot \"Date and Time\":";
            // 
            // SystemOvershoot
            // 
            SystemOvershoot.Location = new Point(265, 426);
            SystemOvershoot.Maximum = new decimal(new int[] { 1200, 0, 0, 0 });
            SystemOvershoot.Name = "SystemOvershoot";
            SystemOvershoot.Size = new Size(68, 23);
            SystemOvershoot.TabIndex = 39;
            SystemOvershoot.Value = new decimal(new int[] { 950, 0, 0, 0 });
            // 
            // UseOvershoot
            // 
            UseOvershoot.AutoSize = true;
            UseOvershoot.Location = new Point(8, 42);
            UseOvershoot.Name = "UseOvershoot";
            UseOvershoot.Size = new Size(319, 19);
            UseOvershoot.TabIndex = 38;
            UseOvershoot.Text = "Use overshoot instead of DDOWN (faster, experimental)";
            UseOvershoot.UseVisualStyleBackColor = true;
            UseOvershoot.CheckedChanged += UseOvershoot_CheckedChanged;
            // 
            // BaseDelay
            // 
            BaseDelay.Location = new Point(265, 107);
            BaseDelay.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            BaseDelay.Name = "BaseDelay";
            BaseDelay.Size = new Size(68, 23);
            BaseDelay.TabIndex = 9;
            // 
            // tabWebhook
            // 
            tabWebhook.Controls.Add(EnableFomoNotifications);
            tabWebhook.Controls.Add(FomoWebhook);
            tabWebhook.Controls.Add(EnableDiscordNotifications);
            tabWebhook.Controls.Add(DiscordWebhook);
            tabWebhook.Controls.Add(IVspacerLabel);
            tabWebhook.Controls.Add(IVspacer);
            tabWebhook.Controls.Add(EmojiConfig);
            tabWebhook.Controls.Add(labelWebhooks);
            tabWebhook.Controls.Add(label21);
            tabWebhook.Controls.Add(DiscordMessageContent);
            tabWebhook.Controls.Add(label14);
            tabWebhook.Controls.Add(btnTestWebHook);
            tabWebhook.Controls.Add(denToggle);
            tabWebhook.Controls.Add(LocationSettings_label);
            tabWebhook.Controls.Add(IVstyle);
            tabWebhook.Controls.Add(IVverbose);
            tabWebhook.Controls.Add(label19);
            tabWebhook.Controls.Add(label18);
            tabWebhook.Controls.Add(EnableEmoji);
            tabWebhook.Location = new Point(4, 24);
            tabWebhook.Name = "tabWebhook";
            tabWebhook.Size = new Size(341, 482);
            tabWebhook.TabIndex = 3;
            tabWebhook.Text = "Webhook";
            tabWebhook.UseVisualStyleBackColor = true;
            // 
            // EnableFomoNotifications
            // 
            EnableFomoNotifications.AutoSize = true;
            EnableFomoNotifications.Location = new Point(8, 434);
            EnableFomoNotifications.Name = "EnableFomoNotifications";
            EnableFomoNotifications.Size = new Size(312, 19);
            EnableFomoNotifications.TabIndex = 52;
            EnableFomoNotifications.Text = "FOMO alerts to Discord webhooks (comma separated)";
            toolTip1.SetToolTip(EnableFomoNotifications, "It has no functional purpose, it just makes me feel bad!");
            EnableFomoNotifications.UseVisualStyleBackColor = true;
            EnableFomoNotifications.CheckedChanged += EnableFomoNotifications_CheckedChanged;
            // 
            // FomoWebhook
            // 
            FomoWebhook.Location = new Point(6, 454);
            FomoWebhook.Name = "FomoWebhook";
            FomoWebhook.Size = new Size(327, 23);
            FomoWebhook.TabIndex = 51;
            // 
            // EnableDiscordNotifications
            // 
            EnableDiscordNotifications.AutoSize = true;
            EnableDiscordNotifications.Location = new Point(8, 19);
            EnableDiscordNotifications.Name = "EnableDiscordNotifications";
            EnableDiscordNotifications.Size = new Size(303, 19);
            EnableDiscordNotifications.TabIndex = 50;
            EnableDiscordNotifications.Text = "Send alerts to Discord webhooks (comma separated)";
            EnableDiscordNotifications.UseVisualStyleBackColor = true;
            EnableDiscordNotifications.CheckedChanged += EnableDiscordNotifications_CheckedChanged;
            // 
            // DiscordWebhook
            // 
            DiscordWebhook.Location = new Point(6, 39);
            DiscordWebhook.Name = "DiscordWebhook";
            DiscordWebhook.Size = new Size(327, 23);
            DiscordWebhook.TabIndex = 49;
            // 
            // IVspacerLabel
            // 
            IVspacerLabel.AutoSize = true;
            IVspacerLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            IVspacerLabel.Location = new Point(6, 263);
            IVspacerLabel.Name = "IVspacerLabel";
            IVspacerLabel.Size = new Size(55, 15);
            IVspacerLabel.TabIndex = 48;
            IVspacerLabel.Text = "IV Spacer";
            // 
            // IVspacer
            // 
            IVspacer.Location = new Point(6, 281);
            IVspacer.Name = "IVspacer";
            IVspacer.Size = new Size(100, 23);
            IVspacer.TabIndex = 47;
            // 
            // EmojiConfig
            // 
            EmojiConfig.Location = new Point(6, 137);
            EmojiConfig.Name = "EmojiConfig";
            EmojiConfig.Size = new Size(100, 23);
            EmojiConfig.TabIndex = 46;
            EmojiConfig.Text = "Emoji Config";
            EmojiConfig.UseVisualStyleBackColor = true;
            EmojiConfig.Click += EmojiConfig_Click;
            // 
            // labelWebhooks
            // 
            labelWebhooks.AutoSize = true;
            labelWebhooks.Location = new Point(6, 383);
            labelWebhooks.Name = "labelWebhooks";
            labelWebhooks.Size = new Size(85, 15);
            labelWebhooks.TabIndex = 44;
            labelWebhooks.Text = "Webhooks are ";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label21.Location = new Point(8, 3);
            label21.Name = "label21";
            label21.Size = new Size(110, 15);
            label21.TabIndex = 43;
            label21.Text = "Webhook Settings";
            // 
            // DiscordMessageContent
            // 
            DiscordMessageContent.Location = new Point(6, 87);
            DiscordMessageContent.Name = "DiscordMessageContent";
            DiscordMessageContent.Size = new Size(327, 23);
            DiscordMessageContent.TabIndex = 42;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(6, 69);
            label14.Name = "label14";
            label14.Size = new Size(285, 15);
            label14.TabIndex = 41;
            label14.Text = "Message Content (ping with <@numerical_user_id>)";
            // 
            // btnTestWebHook
            // 
            btnTestWebHook.Location = new Point(234, 379);
            btnTestWebHook.Name = "btnTestWebHook";
            btnTestWebHook.Size = new Size(104, 23);
            btnTestWebHook.TabIndex = 22;
            btnTestWebHook.Text = "Test Webhook";
            btnTestWebHook.UseVisualStyleBackColor = true;
            btnTestWebHook.Click += BtnTestWebHook_Click;
            // 
            // denToggle
            // 
            denToggle.AutoSize = true;
            denToggle.Checked = true;
            denToggle.CheckState = CheckState.Checked;
            denToggle.Location = new Point(8, 336);
            denToggle.Name = "denToggle";
            denToggle.Size = new Size(79, 19);
            denToggle.TabIndex = 21;
            denToggle.Text = "Show Den";
            denToggle.UseVisualStyleBackColor = true;
            // 
            // LocationSettings_label
            // 
            LocationSettings_label.AutoSize = true;
            LocationSettings_label.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            LocationSettings_label.Location = new Point(6, 318);
            LocationSettings_label.Name = "LocationSettings_label";
            LocationSettings_label.Size = new Size(103, 15);
            LocationSettings_label.TabIndex = 20;
            LocationSettings_label.Text = "Location Settings";
            // 
            // IVstyle
            // 
            IVstyle.FormattingEnabled = true;
            IVstyle.Items.AddRange(new object[] { "Emoji", "Highlighted Numerical", "Numerical" });
            IVstyle.Location = new Point(6, 233);
            IVstyle.Name = "IVstyle";
            IVstyle.Size = new Size(121, 23);
            IVstyle.TabIndex = 8;
            // 
            // IVverbose
            // 
            IVverbose.AutoSize = true;
            IVverbose.Location = new Point(8, 194);
            IVverbose.Name = "IVverbose";
            IVverbose.Size = new Size(167, 19);
            IVverbose.TabIndex = 5;
            IVverbose.Text = "Output IV names (Verbose)";
            IVverbose.UseVisualStyleBackColor = true;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label19.Location = new Point(6, 215);
            label19.Name = "label19";
            label19.Size = new Size(44, 15);
            label19.TabIndex = 2;
            label19.Text = "IV style";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label18.Location = new Point(8, 176);
            label18.Name = "label18";
            label18.Size = new Size(68, 15);
            label18.TabIndex = 1;
            label18.Text = "IV Settings";
            // 
            // EnableEmoji
            // 
            EnableEmoji.AutoSize = true;
            EnableEmoji.Checked = true;
            EnableEmoji.CheckState = CheckState.Checked;
            EnableEmoji.Location = new Point(8, 116);
            EnableEmoji.Name = "EnableEmoji";
            EnableEmoji.Size = new Size(94, 19);
            EnableEmoji.TabIndex = 0;
            EnableEmoji.Text = "Enable Emoji";
            EnableEmoji.UseVisualStyleBackColor = true;
            // 
            // tabAbout
            // 
            tabAbout.Controls.Add(label2);
            tabAbout.Controls.Add(label1);
            tabAbout.Controls.Add(linkLabel2);
            tabAbout.Controls.Add(linkLabel1);
            tabAbout.Controls.Add(labelAppName);
            tabAbout.Controls.Add(picAppIcon);
            tabAbout.Controls.Add(labelAppVersion);
            tabAbout.Location = new Point(4, 24);
            tabAbout.Name = "tabAbout";
            tabAbout.Padding = new Padding(3);
            tabAbout.Size = new Size(341, 482);
            tabAbout.TabIndex = 4;
            tabAbout.Text = "About";
            tabAbout.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(31, 293);
            label2.Name = "label2";
            label2.Size = new Size(290, 15);
            label2.TabIndex = 7;
            label2.Text = "Support this modified version of RaidCrawler here!";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(-3, 234);
            label1.Name = "label1";
            label1.Size = new Size(347, 15);
            label1.TabIndex = 6;
            label1.Text = "Support the original RaidCrawler and its skilled creators here!";
            // 
            // linkLabel2
            // 
            linkLabel2.AutoSize = true;
            linkLabel2.Location = new Point(43, 308);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(265, 15);
            linkLabel2.TabIndex = 5;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "https://github.com/ViolentSpatula/RaidCrawlerV";
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.LinkArea = new LinkArea(0, 43);
            linkLabel1.Location = new Point(55, 249);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(253, 15);
            linkLabel1.TabIndex = 4;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "https://github.com/LegoFigure11/RaidCrawler";
            linkLabel1.LinkClicked += LinkLabel1_LinkClicked;
            // 
            // labelAppName
            // 
            labelAppName.AutoSize = true;
            labelAppName.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            labelAppName.ForeColor = Color.FromArgb(128, 67, 149);
            labelAppName.Location = new Point(98, 134);
            labelAppName.Name = "labelAppName";
            labelAppName.Size = new Size(152, 32);
            labelAppName.TabIndex = 2;
            labelAppName.Text = "RaidCrawler";
            labelAppName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // picAppIcon
            // 
            picAppIcon.BackgroundImage = Properties.Resources.mightiestmark;
            picAppIcon.BackgroundImageLayout = ImageLayout.Center;
            picAppIcon.Location = new Point(0, 10);
            picAppIcon.Name = "picAppIcon";
            picAppIcon.Size = new Size(341, 151);
            picAppIcon.TabIndex = 1;
            picAppIcon.TabStop = false;
            // 
            // labelAppVersion
            // 
            labelAppVersion.AutoSize = true;
            labelAppVersion.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            labelAppVersion.Location = new Point(129, 167);
            labelAppVersion.Name = "labelAppVersion";
            labelAppVersion.Size = new Size(88, 15);
            labelAppVersion.TabIndex = 0;
            labelAppVersion.Text = "v0.0.0-000000";
            labelAppVersion.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Save
            // 
            Save.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            Save.Location = new Point(2, 508);
            Save.Name = "Save";
            Save.Size = new Size(345, 32);
            Save.TabIndex = 116;
            Save.Text = "Save";
            Save.UseVisualStyleBackColor = true;
            Save.Click += Save_Click;
            // 
            // ConfigWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(349, 542);
            Controls.Add(Save);
            Controls.Add(tabControl1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConfigWindow";
            StartPosition = FormStartPosition.CenterParent;
            Text = "RaidCrawler Settings";
            FormClosing += Config_Closing;
            ((System.ComponentModel.ISupportInitialize)SystemDDownPresses).EndInit();
            ((System.ComponentModel.ISupportInitialize)NavigateToSettings).EndInit();
            ((System.ComponentModel.ISupportInitialize)OpenSettings).EndInit();
            ((System.ComponentModel.ISupportInitialize)OpenHome).EndInit();
            ((System.ComponentModel.ISupportInitialize)Hold).EndInit();
            ((System.ComponentModel.ISupportInitialize)Submenu).EndInit();
            ((System.ComponentModel.ISupportInitialize)DateChange).EndInit();
            ((System.ComponentModel.ISupportInitialize)ReturnHome).EndInit();
            ((System.ComponentModel.ISupportInitialize)ReturnGame).EndInit();
            ((System.ComponentModel.ISupportInitialize)DaysToSkip).EndInit();
            tabControl1.ResumeLayout(false);
            tabGeneral.ResumeLayout(false);
            tabGeneral.PerformLayout();
            tabMatch.ResumeLayout(false);
            tabMatch.PerformLayout();
            tabAdvanceDate.ResumeLayout(false);
            tabAdvanceDate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)SaveGameDelay).EndInit();
            ((System.ComponentModel.ISupportInitialize)SystemOvershoot).EndInit();
            ((System.ComponentModel.ISupportInitialize)BaseDelay).EndInit();
            tabWebhook.ResumeLayout(false);
            tabWebhook.PerformLayout();
            tabAbout.ResumeLayout(false);
            tabAbout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picAppIcon).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Label L_AdvanceDate;
        private Label L_BaseDelay;
        private NumericUpDown SystemDDownPresses;
        private Label L_DdownInput;
        private NumericUpDown NavigateToSettings;
        private NumericUpDown OpenSettings;
        private NumericUpDown OpenHome;
        private Label L_OpenHOME;
        private Label L_NavigateSettings;
        private Label L_OpenSettingsDelay;
        private Label L_ScrollSystem;
        private NumericUpDown Hold;
        private Label L_SubmenuDelay;
        private NumericUpDown Submenu;
        private Label L_DateChangeDelay;
        private NumericUpDown DateChange;
        private Label L_ReturnHomeDelay;
        private NumericUpDown ReturnHome;
        private Label L_ReOpenGameDelay;
        private NumericUpDown ReturnGame;
        private Label L_DaysToSkip;
        private NumericUpDown DaysToSkip;
        private CheckBox UseTouch;
        private TabControl tabControl1;
        private TabPage tabAdvanceDate;
        private Label L_OvershootHold;
        private NumericUpDown SystemOvershoot;
        private CheckBox UseOvershoot;
        private TabPage tabWebhook;
        private CheckBox EnableEmoji;
        private Label label19;
        private Label label18;
        private ComboBox IVstyle;
        private CheckBox IVverbose;
        private CheckBox denToggle;
        private Label LocationSettings_label;
        private Button btnTestWebHook;
        private TextBox DiscordMessageContent;
        private Label label14;
        private Label labelWebhooks;
        private TabPage tabAbout;
        private Label labelAppVersion;
        private LinkLabel linkLabel1;
        private PictureBox picAppIcon;
        private TabPage tabGeneral;
        private Label label23;
        private Label LabelEventProgress;
        private ComboBox EventProgress;
        private Label LabelGame;
        private ComboBox Game;
        private Label LabelStoryProgress;
        private ComboBox StoryProgress;
        private Button EmojiConfig;
        private ComboBox Protocol_dropdown;
        private Label Protocol_label;
        private NumericUpDown BaseDelay;
        private NumericUpDown SaveGameDelay;
        private Label L_SaveGame;
        private CheckBox DodgeSystemUpdate;
        private CheckBox UseSetStick;
        private TextBox IVspacer;
        private Label IVspacerLabel;
        private LinkLabel linkLabel2;
        private Label labelAppName;
        private TextBox InstanceName;
        private Label label17;
        private CheckBox EnableDiscordNotifications;
        private TextBox DiscordWebhook;
        private Label label13;
        private CheckBox ExperimentalView;
        private TabPage tabMatch;
        private CheckBox SaveGame;
        private Label LabelMatchFound;
        private CheckBox FocusWindow;
        private CheckBox EnableAlert;
        private CheckBox PlayTone;
        private TextBox AlertMessage;
        private Button Save;
        private CheckBox CopyEmoji;
        private CheckBox MapBackground;
        private CheckBox TitleSearchTime;
        private Label label21;
        private Label label2;
        private Label label1;
        private CheckBox EnableFomoNotifications;
        private TextBox FomoWebhook;
        private ToolTip toolTip1;
    }
}
