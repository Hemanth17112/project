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
    [Route("api/patients")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IAppointmentRepository<Patient> repo;
        private readonly ISenderService sender;

        public PatientsController(IAppointmentRepository<Patient> patientRepo,
            ISenderService senderSrv)
        {
            repo = patientRepo;
            sender = senderSrv;
        }

        [HttpGet("GetPatientId/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var patient = repo.GetById(id);

            var data = new
            {
                patient.Name,
                patient.Age,
                patient.Gender.Gender,
                patient.Number,
                DOB = patient.DOB.Date,
                patient.Email,
                patient.BloodGroup.BloodGroup
            };

            await sender.SendMessageAsync(data, "appointmentsqueue");
            return Ok(data);
        }
    }
}
