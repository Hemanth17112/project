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
    [Route("api/recommendations")]
    [ApiController]

    public class RecommendationsController : ControllerBase
    {
        private readonly IAppointmentRepository<Recommendation> repo;
        private readonly ISenderService sender;

        public RecommendationsController(IAppointmentRepository<Recommendation> recRepo,
            ISenderService senderSrv)
        {
            repo = recRepo;
            sender = senderSrv;
        }

        [HttpGet("GetAll/{appointmentId}")]
        public async Task<IActionResult> Get(int appointmentId)
        {
            var recommendations = repo.GetAll(appointmentId);

            var data = recommendations.Select(r => new
            {
                r.DoctorName
            });

            await sender.SendMessageAsync(data, "appointmentsqueue");
            return Ok(data);
        }

        [HttpGet("GetRecommendation/{appointmentId}")]
        public async Task<IActionResult> Get(int appointmentId, int recommendationId)
        {
            var recommendation = repo.GetById(appointmentId, recommendationId);

            var data = new
            {
                recommendation.DoctorName
            };

            await sender.SendMessageAsync(data, "appointmentsqueue");
            return Ok(data);
        }

        [HttpPost("AddRecommendation/{appointmentId}")]
        public IActionResult Add([FromBody] Recommendation recommendation, int appointmentId)
        {
            if (repo.AddItem(recommendation, appointmentId))
                return Ok();
            else
                return BadRequest();
        }

        [HttpDelete("DeleteRecommendation/{appointmentId}/{recommendationId}")]
        public IActionResult Delete(int appointmentId, int recommendationId)
        {
            repo.Delete(appointmentId, recommendationId);
            return Ok();
        }
    }
}
