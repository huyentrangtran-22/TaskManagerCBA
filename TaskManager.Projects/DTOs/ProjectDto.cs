namespace TaskManager.Projects.Dtos
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Danh sách username của thành viên
        public List<string> MemberUsernames { get; set; } = new();

        // Trạng thái phản hồi
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; } = true;
    }
}