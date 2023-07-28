using SysBot.Base;

namespace RaidCrawler.Core.Interfaces
{
    public interface IWebhookConfig
    {
        SwitchProtocol Protocol { get; set; }
        bool EnableNotification { get; set; }
        bool EnableFomoNotification { get; set; }
        bool ToggleDen { get; set; }
        string InstanceName { get; set; }
        string DiscordWebhook { get; set; }
        string DiscordFomoWebhook { get; set; }
        string DiscordMessageContent { get; set; }

        bool EnableEmoji { get; set; }
        bool CopyEmoji { get; set; }
        Dictionary<string, string> Emoji { get; set; }

        bool VerboseIVs { get; set; }
        int IVsStyle { get; set; }
        string IVsSpacer { get; set; }
    }
}
