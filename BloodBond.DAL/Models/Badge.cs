using System.ComponentModel.DataAnnotations;

namespace BloodBond.DAL.Models
{
    public class Badge
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(80)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string Icon { get; set; } = "🏅";

        /// How many donation points are required to unlock this badge.
        [Range(0, int.MaxValue)]
        public int PointsRequired { get; set; }
    }
}
