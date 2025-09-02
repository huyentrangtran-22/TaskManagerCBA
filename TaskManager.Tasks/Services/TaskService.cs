using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Services;
using TaskManager.Shared.Data;
using TaskManager.Shared.Entities;
using TaskManager.Tasks.Dtos;

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
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    AssignedUserId = string.IsNullOrWhiteSpace(t.AssignedUserId) ? "Chưa phân công" : t.AssignedUserId,
                    Status = (Shared.Entities.TaskStatus)t.Status,
                    ProjectId = t.ProjectId
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
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                AssignedUserId = string.IsNullOrWhiteSpace(task.AssignedUserId) ? "Chưa phân công" : task.AssignedUserId,
                Status = (Shared.Entities.TaskStatus)task.Status,
                ProjectId = task.ProjectId
            };
        }

        // Tạo task mới
        public async Task<TaskDto?> CreateAsync(TaskCreateDto dto)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == dto.ProjectId);
            if (!projectExists)
            {
                throw new Exception($"ProjectId {dto.ProjectId} không tồn tại.");
            }

            var task = new TaskItem
            {
                Title = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = (DateTime)dto.EndDate,
                AssignedUserId = string.IsNullOrWhiteSpace(dto.AssignedUserId) ? null : dto.AssignedUserId,
                Status = (TaskManager.Shared.Entities.TaskStatus)(int)dto.Status,
                ProjectId = dto.ProjectId
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return new TaskDto
            {
                Id = task.Id,
                Name = task.Title,
                Description = task.Description,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                AssignedUserId = string.IsNullOrWhiteSpace(task.AssignedUserId) ? "Chưa phân công" : task.AssignedUserId,
                Status = (Shared.Entities.TaskStatus)(int)task.Status,
                ProjectId = task.ProjectId
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
            task.AssignedUserId = string.IsNullOrWhiteSpace(dto.AssignedUserId) ? null : dto.AssignedUserId;
            task.Status = (Shared.Entities.TaskStatus)dto.Status;

            await _context.SaveChangesAsync();

            return new TaskDto
            {
                Id = task.Id,
                Name = task.Title,
                Description = task.Description,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                AssignedUserId = string.IsNullOrWhiteSpace(task.AssignedUserId) ? "Chưa phân công" : task.AssignedUserId,
                Status = (Shared.Entities.TaskStatus)task.Status,
                ProjectId = task.ProjectId
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

            task.AssignedUserId = string.IsNullOrWhiteSpace(userId) ? null : userId;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}