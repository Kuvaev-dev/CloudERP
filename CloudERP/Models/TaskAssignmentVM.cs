using Domain.Models;
using System.Collections.Generic;

namespace CloudERP.Models
{
    public class TaskAssignmentVM
    {
        public TaskModel TaskModel { get; set; }
        public int BranchID { get; set; }
        public IEnumerable<Employee> AvailableEmployees { get; set; }
    }
}