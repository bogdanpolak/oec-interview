using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RL.Data.DataModels.Common;

namespace RL.Data.DataModels;

public class Procedure : IChangeTrackable
{
    public Procedure()
    {
        ProcedureUsers = new LinkedList<ProcedureUser>();
    }
    [Key]
    public int ProcedureId { get; set; }
    public string ProcedureTitle { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    [ForeignKey("ProcedureId")]
    public virtual ICollection<ProcedureUser> ProcedureUsers { get; set; }
}
