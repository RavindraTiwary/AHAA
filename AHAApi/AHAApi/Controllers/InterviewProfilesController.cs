using AHAApi.DataModels;
using AHAApi.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace AHAApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewProfilesController : ControllerBase
    {
        private readonly InterviewProfilesRepository _repository;

        public InterviewProfilesController(InterviewProfilesRepository repository)
        {
            _repository = repository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<InterviewProfiles>>> GetAll()
        {
            var profile = await _repository.GetAllAsync();
            return Ok(profile);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Post(InterviewProfiles profile)
        {
            await _repository.CreateAsync(profile);
            return Ok(true);
        }

        [HttpGet]
        [Route("Id")]
        public async Task<ActionResult<InterviewProfiles>> GetById(string id)
        {
            var profile = await _repository.GetByIdAsync(id);
            return Ok(profile);
        }

        [HttpPut]
        public async Task<ActionResult<IEnumerable<InterviewProfiles>>> Update(InterviewProfiles profile)
        {
            await _repository.UpdateAsync(profile);
            return Ok(true);
        }

    }
}
