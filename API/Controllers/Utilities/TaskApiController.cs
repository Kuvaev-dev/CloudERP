using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/task")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TaskApiController : ApiController
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

        [HttpGet, Route("{companyId:int}/{branchId:int}/{userId:int}")]
        public async Task<IHttpActionResult> GetAll(int companyId, int branchId, int userId)
        {
            try
            {
                var tasks = await _taskRepository.GetAllAsync(companyId, branchId, userId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("employees/{companyId:int}/{branchId:int}")]
        public async Task<IHttpActionResult> GetEmployeesForTaskAssignment(int companyId, int branchId)
        {
            try
            {
                var employees = await _employeeRepository.GetEmployeesForTaskAssignmentAsync(companyId, branchId);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(id);
                if (task == null) return NotFound();
                return Ok(task);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("create")]
        public async Task<IHttpActionResult> Create([FromBody] TaskModel model)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                await _taskRepository.AddAsync(model);
                return Created($"api/task/{model.TaskID}", model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("update/{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] TaskModel model)
        {
            if (model == null || id != model.TaskID) return BadRequest("Invalid data.");

            try
            {
                await _taskRepository.UpdateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("delete/{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            try
            {
                await _taskRepository.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}