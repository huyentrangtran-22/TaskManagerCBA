using TaskManager.Shared.Entities; // để dùng TaskStatus

namespace TaskManager.Tasks.Dtos
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? AssignedUserId { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.New;

        public TaskDto() { }

        public TaskDto(int id, string name, string? description, DateTime start, DateTime end, string? userId, TaskStatus status)
        {
            Id = id;
            Name = name;
            Description = description;
            StartDate = start;
            EndDate = end;
            AssignedUserId = userId;
            Status = status;
        }
    }

    public class TaskCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? AssignedUserId { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.New;
    }

        public class TaskUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? AssignedUserId { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.New;
    }
}
