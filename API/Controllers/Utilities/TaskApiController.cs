using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Utilities
{
    [ApiController]
    public class TaskApiController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public TaskApiController(
            ITaskRepository taskRepository,
            IEmployeeRepository employeeRepository)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskModel>>> GetAll(int companyId, int branchId, int userId)
        {
            try
            {
                var tasks = await _taskRepository.GetAllAsync(companyId, branchId, userId);
                if (tasks == null) return NotFound();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesForTaskAssignment(int companyId, int branchId)
        {
            try
            {
                var employees = await _employeeRepository.GetEmployeesForTaskAssignmentAsync(companyId, branchId);
                if (employees == null) return NotFound();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<TaskModel>> GetById(int id)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(id);
                if (task == null) return NotFound();
                return Ok(task);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<TaskModel>> Create([FromBody] TaskModel model)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                await _taskRepository.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = model.TaskID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] TaskModel model)
        {
            if (model == null || id != model.TaskID) return BadRequest("Invalid data.");

            try
            {
                await _taskRepository.UpdateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _taskRepository.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}