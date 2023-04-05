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
    [Route("api/AppointmentDetails")]
    [ApiController]
    public class AppointmentDetailsContoller : ControllerBase
    {
        private readonly IAppointmentRepository<AppointmentDetail> repo;
        private readonly ISenderService sender;

        public AppointmentDetailsContoller(IAppointmentRepository<AppointmentDetail> appdetailsRepo,
            ISenderService senderSrv)
        {
            repo = appdetailsRepo;
            sender = senderSrv;
        }

        [HttpGet]
        [Route("AllAppointments")]
        public async Task<IActionResult> Get()
        {
            var appointments = repo.GetAll();

            var data = appointments.Select(a => new
            {
                AppointmentId = a.Id,
                AppointmentDate = a.AppointmentDate.Date,
                AppointmentTime = a.AppointmentTime.ToString(),
                a.AppointmentDescription,
                a.PatientId,
                PatientName = a.Patient.Name,
                a.DoctorId,
                DoctorName = a.Doctor.Name
            }).ToList();

            await sender.SendMessageAsync(data, "appointmentsqueue");
            return Ok(data);
        }

        [HttpGet("GetAppointmentDetails/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var appointment = repo.GetById(id);

            var data = new
            {
                AppointmentId = appointment.Id,
                AppointmentDate = appointment.AppointmentDate.Date,
                AppointmentTime = appointment.AppointmentTime.ToString(),
                appointment.AppointmentDescription,
                appointment.PatientId,
                PatientName = appointment.Patient.Name,
                appointment.DoctorId,
                DoctorName = appointment.Doctor.Name
            };

            await sender.SendMessageAsync(data, "appointmentsqueue");
            return Ok(data);
        }

        [HttpPost]
        [Route("NewAppointment")]
        public async Task<IActionResult> Post([FromBody] AppointmentDetail details)
        {
            var data = new
            {
                AppointmentId = details.Id,
                AppointmentDate = details.AppointmentDate.Date,
                AppointmentTime = details.AppointmentTime.ToString(),
                details.AppointmentDescription,
                details.PatientId,
                PatientName = details.Patient.Name,
                details.DoctorId,
                DoctorName = details.Doctor.Name
            };

            if (repo.AddItem(details))
            {
                await sender.SendMessageAsync(data, "appointmentsqueue");
                return Ok();
            }
            else
                return BadRequest("New Appointment could not be created");
        }

        [HttpPut("CloseAppointment/{id}")]
        public IActionResult Put(int id)
        {
            repo.UpdateItem(id);
            return Ok($"Successfully closed the given Appointment with ID: {id}");
        }
    }
}
