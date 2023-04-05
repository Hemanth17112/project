using CMDWebAPI.Services;
using CMDDALayer;
using CMDEFLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMDWebAPI.Controllers
{
    [Route("api/tests")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly IAppointmentRepository<Test> repo;
        private readonly ISenderService sender;

        public TestsController(IAppointmentRepository<Test> testRepo, ISenderService senderSrv)
        {
            repo = testRepo;
            sender = senderSrv;
        }

        [HttpGet("GetTest/{appointmentId}/{testId}")]
        public async Task<IActionResult> Get(int appointmentId, int testId)
        {
            var test= repo.GetById(appointmentId, testId);

            var data = new
            {
                TestId = test.Id,
                test.TestCategory.TestName
            };

            await sender.SendMessageAsync(data, "appointmentsqueue");
            return Ok(test);
        }

        [HttpGet("AllTests/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var tests = repo.GetAll(id);

            var data = tests.Select(t => new
            {
                TestId = t.Id,
                t.TestCategory.TestName
            });

            await sender.SendMessageAsync(data, "appointmentsqueue");
            return Ok(tests);
        }

        [HttpDelete("DeleteTest/{appointmentId}/{testId}")]
        public IActionResult Delete(int appointmentId, int testId)
        {
            repo.Delete(appointmentId, testId);
            return Ok();
        }
    }
}
