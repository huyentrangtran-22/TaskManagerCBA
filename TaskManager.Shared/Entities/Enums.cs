using System.ComponentModel.DataAnnotations;

namespace TaskManager.Shared.Entities
{
    public enum TaskStatus
    {
        Backlog = 0,
        New = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4
    }


}
