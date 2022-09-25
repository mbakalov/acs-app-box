using System.ComponentModel.DataAnnotations.Schema;

namespace ACSAppBox.Models
{
    [Table("Rooms")]
    public class Room
    {
        public Guid Id {get; set;}
        public Guid GroupId {get;set;}
        public String ThreadId {get;set;}
        public String Name {get;set;}
        public Room(Guid groupId, String threadId, Guid Id, String Name)
            {
                this.GroupId = groupId;
                this.ThreadId = threadId;
                this.Name = Name;
                this.Id = Id;
            }
    }
}