using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Identity.Entities;
using TaskManager.Projects.Data;
using TaskManager.Projects.Dtos;
using TaskManager.Projects.Entities;
using TaskManager.Projects.Interfaces;

namespace TaskManager.Projects.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ProjectDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectService(ProjectDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<ProjectDto>> GetAllAsync()
        {
            var projects = await _context.Projects
                .Include(p => p.Members)
                .ToListAsync();

            var users = await _userManager.Users.ToListAsync();

            var projectDtos = projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                MemberUsernames = p.Members
                    .Join(users, m => m.UserId, u => u.Id, (m, u) => u.UserName ?? string.Empty) // tránh null
                    .Where(username => !string.IsNullOrWhiteSpace(username)) // lọc chuỗi rỗng
                    .ToList()
            }).ToList();
            return projectDtos;
        }

        public async Task<ProjectDto?> GetByIdAsync(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return null;

            var users = await _userManager.Users.ToListAsync();

            var memberUsernames = project.Members
                .Join(users, m => m.UserId, u => u.Id, (m, u) => u.UserName)
                .ToList();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                MemberUsernames = memberUsernames
            };
        }

        public async Task<ProjectDto> CreateAsync(ProjectCreateDto dto)
        {
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                StartDate = dto.StartDate != default ? dto.StartDate : DateTime.UtcNow,
                EndDate = dto.EndDate
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var addedUsernames = new List<string>();
            var failedUsernames = new List<string>();

            if (dto.MemberUsernames != null && dto.MemberUsernames.Any())
            {
                foreach (var username in dto.MemberUsernames)
                {
                    var user = await _userManager.FindByNameAsync(username);
                    if (user == null)
                    {
                        failedUsernames.Add(username);
                        continue;
                    }

                    var exists = await _context.ProjectMembers
                        .AnyAsync(pm => pm.ProjectId == project.Id && pm.UserId == user.Id);

                    if (exists)
                        continue;

                    var member = new ProjectMember
                    {
                        ProjectId = project.Id,
                        UserId = user.Id
                    };

                    _context.ProjectMembers.Add(member);
                    addedUsernames.Add(username);
                }

                await _context.SaveChangesAsync();
            }

            var message = "Tạo dự án thành công.";
            if (failedUsernames.Any())
                message += $" Không tìm thấy các username: {string.Join(", ", failedUsernames)}.";

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                MemberUsernames = addedUsernames,
                Success = true,
                Message = message
            };
        }

        public async Task<bool> UpdateAsync(int id, ProjectUpdateDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            project.Name = dto.Name;
            project.Description = dto.Description;
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            // Cập nhật danh sách thành viên
            if (dto.MemberUsernames != null)
            {
                await UpdateMembersAsync(id, dto.MemberUsernames);
            }

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddMemberAsync(int projectId, string userId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            var user = await _userManager.FindByIdAsync(userId);

            if (project == null || user == null)
                return false;

            var exists = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

            if (exists)
                return false;

            var member = new ProjectMember
            {
                ProjectId = projectId,
                UserId = userId
            };

            _context.ProjectMembers.Add(member);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveMemberAsync(int projectId, string userId)
        {
            var member = await _context.ProjectMembers
                .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == userId);

            if (member == null) return false;

            _context.ProjectMembers.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateMembersAsync(int projectId, List<string> newUsernames)
        {
            var existingMembers = await _context.ProjectMembers
                .Where(pm => pm.ProjectId == projectId)
                .ToListAsync();

            _context.ProjectMembers.RemoveRange(existingMembers);

            foreach (var username in newUsernames.Distinct())
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user != null)
                {
                    _context.ProjectMembers.Add(new ProjectMember
                    {
                        ProjectId = projectId,
                        UserId = user.Id
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}