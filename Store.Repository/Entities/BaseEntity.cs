using System;

namespace KStore.Models
{
    public interface IBaseEntity
    {
        int Id { get; set; }

        bool IsDeleted { get; set; }

        DateTime DeletedAt { get; set; }

        DateTime CreatedAt { get; set; }

        DateTime UpdatedAt { get; set; }
    }

    public class BaseEntity : IBaseEntity
    {
        public int Id { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DeletedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public void UpdateTimeStamps()
        {
            if (!IsDeleted && Id > 0)
            {
                UpdatedAt = DateTime.UtcNow;
            }
            else if (!IsDeleted && Id == 0)
            {
                CreatedAt = DateTime.UtcNow;
            }
            else if (IsDeleted && Id > 0)
            {
                DeletedAt = DateTime.UtcNow;
            }
            else if (IsDeleted && Id == 0)
            {
                throw new NotSupportedException("Cannot delete a new entity");
            }
        }
    }
}
