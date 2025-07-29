using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSnap.Api.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public int PortfolioUserId { get; set; }

        [ForeignKey("PortfolioUserId")]
        public PortfolioUser PortfolioUser { get; set; }
    }
}
