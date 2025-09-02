using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskManager.Application.Services;
using TaskManager.Tasks.Dtos;
using TaskManager.Tasks.Services;

namespace TaskManager.Web.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET: /Task
        public async Task<IActionResult> Index()
        {
            var tasks = await _taskService.GetAllAsync();
            return View(tasks);
        }

        // GET: /Task/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskCreateDto taskDto)
        {
            if (!ModelState.IsValid)
                return View(taskDto);

            var createdTask = await _taskService.CreateAsync(taskDto);

            if (createdTask == null)
            {
                ModelState.AddModelError(string.Empty, "Cannot create task");
                return View(taskDto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Task/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null) return NotFound();

            var taskDto = new TaskUpdateDto
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                AssignedUserId = task.AssignedUserId,
                Status = task.Status
            };

            return View(taskDto);
        }

        // POST: /Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskUpdateDto taskDto)
        {
            if (id != taskDto.Id) return BadRequest();

            if (!ModelState.IsValid)
                return View(taskDto);

            var updatedTask = await _taskService.UpdateAsync(id, taskDto);

            if (updatedTask == null)
            {
                ModelState.AddModelError(string.Empty, "Cannot update task");
                return View(taskDto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Task/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null) return NotFound();

            return View(task);
        }

        // POST: /Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool success = await _taskService.DeleteAsync(id);

            if (!success)
            {
                TempData["Error"] = "Cannot delete task";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
