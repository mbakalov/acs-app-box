using System.ComponentModel.DataAnnotations.Schema;

namespace ACSAppBox.Models
{
    [Table("ApplicationSettings")]
    public class ApplicationSetting
    {
        public Guid Id { get; set; }

        public string CommunicationAdminUserId { get; set; }
    }
}
