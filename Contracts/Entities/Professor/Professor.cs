﻿using Contracts.Entities.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contracts.Entities
{
    [Table("professor")]
    public partial class Professor
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Required]
        public int Id { get; set; }

        [Column("email")]
        [SensitiveData]
        public string Email { get; set; }

        [Column("password")]
        [SensitiveData]
        public string Password { get; set; }

        [Column("name")]
        [SensitiveData]
        public string Name { get; set; }

        [Column("rp")]
        [SensitiveData]
        public string Rp { get; set; }

        [Column("phone")]
        [SensitiveData]
        public string Phone { get; set; }

        [Column("active")]
        public bool Active { get; set; }
    }
}
