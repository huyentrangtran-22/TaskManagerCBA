using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Services;
using TaskManager.Projects.Interfaces;
using TaskManager.Tasks.Dtos;
using TaskManager.Tasks.Services;
using TaskManager.Shared.Entities;


namespace TaskManager.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ITaskService _taskService;

        public ReportController(IProjectService projectService, ITaskService taskService)
        {
            _projectService = projectService;
            _taskService = taskService;
        }

        // GET: /Report
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả dự án
            var projects = await _projectService.GetAllAsync();

            // Lấy tất cả task
            var tasks = await _taskService.GetAllAsync();

            // Tạo ViewModel cho báo cáo
            var report = projects.Select(p =>
            {
                var projectTasks = tasks.Where(t => t.Id == p.Id).ToList();
                int total = projectTasks.Count;
                var completed = tasks.Count(t => t.Status == TaskStatus.Completed);

                return new ProjectReportViewModel
                {
                    ProjectId = p.Id,
                    ProjectName = p.Name,
                    TotalTasks = total,
                    CompletedTasks = completed,
                    ProgressPercentage = total > 0 ? (completed * 100 / total) : 0
                };
            }).ToList();

            return View(report);
        }
    }

    // ViewModel dùng cho báo cáo tiến độ
    public class ProjectReportViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int ProgressPercentage { get; set; }
    }
}
