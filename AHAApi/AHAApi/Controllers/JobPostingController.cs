
using AHAApi.DataModels;
using AHAApi.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace AHAApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostingController : ControllerBase
    {
        private readonly JobPostingRepository _repository;

        public JobPostingController(JobPostingRepository repository)
        {
            _repository = repository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPosting>>> GetAll()
        {
            var profile = await _repository.GetAllAsync();
            return Ok(profile);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Post(JobPosting profile)
        {
            await _repository.CreateAsync(profile);
            return Ok(true);
        }

        [HttpGet]
        [Route("Id")]
        public async Task<ActionResult<JobPosting>> GetById(string id)
        {
            var profile = await _repository.GetByIdAsync(id);
            return Ok(profile);
        }

        [HttpPut]
        public async Task<ActionResult<IEnumerable<JobPosting>>> Update(JobPosting profile)
        {
            await _repository.UpdateAsync(profile);
            return Ok(true);
        }

    }
}
