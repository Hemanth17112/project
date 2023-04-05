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
    [Route("api/vitals")]
    [ApiController]
    public class VitalsController : ControllerBase
    {
        private readonly IAppointmentRepository<Vitals> repo;
        private readonly ISenderService sender;

        public VitalsController(IAppointmentRepository<Vitals> vitalsRepo,
            ISenderService senderSrv)
        {
            repo = vitalsRepo;
            sender = senderSrv;
        }

        [HttpGet("GetVitals/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var vitals = repo.GetById(id);

            await sender.SendMessageAsync(vitals, "appointmentsqueue");
            return Ok(vitals);
        }

        [HttpGet]
        [Route("GetVitals")]
        public IActionResult Get()
        {
            var vitals = repo.GetAll();
            return Ok(vitals);
        }

        [HttpPut("UpdateVitals/{id}")]
        public IActionResult Put(int id, [FromBody] Vitals item)
        {
            repo.UpdateItem(item, id);
            return Ok("Successfully updated the vitals");
        }
    }
}
