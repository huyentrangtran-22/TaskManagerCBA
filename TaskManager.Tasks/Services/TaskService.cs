using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Services;
using TaskManager.Shared.Data;        // AppDbContext
using TaskManager.Shared.Entities;
using TaskManager.Tasks.Dtos;        // TaskDto, TaskCreateDto, TaskUpdateDto

namespace TaskManager.Tasks.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả task
        public async Task<IEnumerable<TaskDto>> GetAllAsync()
        {
            return await _context.TaskItems
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Name = t.Title,
                    Description = t.Description,
                    StartDate = (DateTime)t.StartDate,
                    EndDate = (DateTime)t.EndDate,
                    AssignedUserId = t.AssignedUserId,
                    Status = (TaskStatus)t.Status
                })
                .ToListAsync();
        }

        // Lấy task theo Id
        public async Task<TaskDto?> GetByIdAsync(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return null;

            return new TaskDto
            {
                Id = task.Id,
                Name = task.Title,
                Description = task.Description,
                StartDate = (DateTime)task.StartDate,
                EndDate = (DateTime)task.EndDate,
                AssignedUserId = task.AssignedUserId,
                Status = (TaskStatus)task.Status
            };
        }

        // Tạo task mới
        public async Task<TaskDto?> CreateAsync(TaskCreateDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                AssignedUserId = dto.AssignedUserId,
                Status = (Shared.Entities.TaskStatus)dto.Status
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return new TaskDto
            {
                Id = task.Id,
                Name = task.Title,
                Description = task.Description,
                StartDate = (DateTime)task.StartDate,
                EndDate = (DateTime)task.EndDate,
                AssignedUserId = task.AssignedUserId,
                Status = (TaskStatus)task.Status
            };
        }

        // Cập nhật task
        public async Task<TaskDto?> UpdateAsync(int id, TaskUpdateDto dto)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return null;

            task.Title = dto.Name;
            task.Description = dto.Description;
            task.StartDate = dto.StartDate;
            task.EndDate = dto.EndDate;
            task.AssignedUserId = dto.AssignedUserId;
            task.Status = (Shared.Entities.TaskStatus)dto.Status;

            await _context.SaveChangesAsync();

            return new TaskDto
            {
                Id = task.Id,
                Name = task.Title,
                Description = task.Description,
                StartDate = (DateTime)task.StartDate,
                EndDate = (DateTime)task.EndDate,
                AssignedUserId = task.AssignedUserId,
                Status = (TaskStatus)task.Status
            };
        }

        // Xoá task
        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return false;

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        // Gán user cho task
        public async Task<bool> AssignUserAsync(int taskId, string userId)
        {
            var task = await _context.TaskItems.FindAsync(taskId);
            if (task == null) return false;

            task.AssignedUserId = userId;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
