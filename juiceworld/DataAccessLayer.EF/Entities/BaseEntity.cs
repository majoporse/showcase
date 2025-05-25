using System.ComponentModel.DataAnnotations;
using JuiceWorld.Entities.Interfaces;

namespace JuiceWorld.Entities;

/**
 * Base entity for all entities in the database.
 */
public class BaseEntity : IBaseEntity
{
    [Key]
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}