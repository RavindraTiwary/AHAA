
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
        public async Task<ActionResult<string>> Post(JobPosting profile)
        {
            return Ok(await _repository.CreateAsync(profile));
        }

        [HttpGet]
        [Route("{id}")]
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
