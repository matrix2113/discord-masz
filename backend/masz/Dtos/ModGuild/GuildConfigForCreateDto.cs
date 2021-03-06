using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.GuildConfig
{
    public class GuildConfigForCreateDto
    {
        [Required(ErrorMessage = "GuildId field is required")]
        [RegularExpression(@"^[0-9]{18}$", ErrorMessage = "the guild id can only consist of numbers and must be 18 characters long")]
        public string GuildId { get; set; }
        [Required(ErrorMessage = "ModRoleId field is required")]
        [RegularExpression(@"^[0-9]{18}$", ErrorMessage = "the role id can only consist of numbers and must be 18 characters long")]
        public string ModRoleId { get; set; }
        [Required(ErrorMessage = "AdminRoleId field is required")]
        [RegularExpression(@"^[0-9]{18}$", ErrorMessage = "the role id can only consist of numbers and must be 18 characters long")]
        public string AdminRoleId { get; set; }
        [RegularExpression(@"^[0-9]{18}$", ErrorMessage = "the role id can only consist of numbers and must be 18 characters long")]
        public string? MutedRoleId { get; set; }
        public bool ModNotificationDM { get; set; }
        [Url(ErrorMessage = "Webhook needs to be a valid url")]
        [RegularExpression(@"^https://discordapp.com/.*$", ErrorMessage = "please specify a url that starts with 'https://discordapp.com/'.")]
        public string ModPublicNotificationWebhook { get; set; }
        [Url(ErrorMessage = "Webhook needs to be a valid url")]
        [RegularExpression(@"^https://discordapp.com/.*$", ErrorMessage = "please specify a url that starts with 'https://discordapp.com/'.")]
        public string ModInternalNotificationWebhook { get; set; }
    }
}