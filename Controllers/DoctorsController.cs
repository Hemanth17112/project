using CMDWebAPI.Services;
using CMDDALayer;
using CMDEFLayer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMDWebAPI.Controllers
{
    [Route("api/doctors")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IAppointmentRepository<Doctor> repo;
        private readonly ISenderService sender;

        public DoctorsController(IAppointmentRepository<Doctor> doctorRepo, 
            ISenderService senderSrv)
        {
            repo = doctorRepo;
            sender = senderSrv;
        }

        [HttpGet("GetDoctorId/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var doctor = repo.GetById(id);

            var data = new
            {
                doctor.Name,
                doctor.NPINumber,
                doctor.Number,
                doctor.Email,
                Specialization = doctor.Speciality.Category,
                doctor.PracticeLocation.Location,
            };

            await sender.SendMessageAsync(data, "appointmentsqueue");
            return Ok(data);
        }
    }
}
