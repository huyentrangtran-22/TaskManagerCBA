using System;
using System.Collections.Generic;

namespace TaskManager.Projects.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string?Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // ✅ Thêm thuộc tính này để tránh lỗi khi insert
        public DateTime CreatedAt { get; set; }

        // Quan hệ
        public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    }
}
