using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCore.ApiToken.SampleApp.Entities
{
    public class ApiToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        public string Token { get; set; }

        [Required]
        [StringLength(50)]
        public string Scheme { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Claims { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public DateTime Expiration { get; set; }
    }
}