﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Shared.Dtos;
using TaskManager.Shared.Entities;
using TaskManager.Tasks.Dtos;

namespace TaskManager.Application.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetAllAsync();
        Task<TaskDto?> GetByIdAsync(int id);
        Task<TaskDto?> CreateAsync(TaskCreateDto dto);
        Task<TaskDto?> UpdateAsync(int id, TaskUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
