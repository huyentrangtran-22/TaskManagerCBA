using TaskManager.Projects.Dtos;

namespace TaskManager.Projects.Interfaces
{
    public interface IProjectService
    {
        Task<List<ProjectDto>> GetAllAsync();
        Task<ProjectDto?> GetByIdAsync(int id);
        Task<ProjectDto> CreateAsync(ProjectCreateDto dto);
        Task<bool> UpdateAsync(int id, ProjectUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> AddMemberAsync(int projectId, string userId);
        Task<bool> RemoveMemberAsync(int projectId, string userId);
    }
}
