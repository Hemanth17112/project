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
    [Route("api/prescriptions")]
    [ApiController]
    public class PrescriptionsController : Controller
    {
        private readonly IAppointmentRepository<Prescription> repo;
        private readonly ISenderService sender;

        public PrescriptionsController(IAppointmentRepository<Prescription> prescRepo,
            ISenderService senderSrv)
        {
            repo = prescRepo;
            sender = senderSrv;
        }

        [HttpGet("GetPrescriptions/{appointmentId}")]
        public async Task<IActionResult> Get(int appointmentId)
        {
            var prescriptions = repo.GetAll(appointmentId);

            var data = prescriptions.Select(p => new
            {
                p.Medicine.MedicineName,
                p.MedicineDuration,
                p.MedicineCycle,
                BeforeOrAfterFood = p.Food ? "After" : "Before",
                p.MedicineDescription,
                p.DiseaseName
            });

            await sender.SendMessageAsync(data, "appointmentsqueue");
            return Ok(data);
        }

        [HttpGet("GetPrescriptionId/{appointmentId}/{prescriptionId}")]
        public async Task<IActionResult> Get(int appointmentId, int prescriptionId)
        {
            var prescription = repo.GetById(appointmentId, prescriptionId);

            var data = new
            {
                prescription.Medicine.MedicineName,
                prescription.MedicineDuration,
                prescription.MedicineCycle,
                BeforeOrAfterFood = prescription.Food ? "After" : "Before",
                prescription.MedicineDescription,
                prescription.DiseaseName
            };

            await sender.SendMessageAsync(data, "appointmentsqueue");
            return Ok(data);
        }

        [HttpPost("AddPrescription/{appointmentId}")]
        public IActionResult Update([FromBody] Prescription prescription, int appointmentId)
        {
            if (repo.AddItem(prescription, appointmentId))
                return Ok();
            else
                return BadRequest();
        }

        [HttpDelete("DeletePrescription/{appointmentId}/{prescriptionId}")]
        public IActionResult Delete(int appointmentId, int prescriptionId)
        {
            repo.Delete(appointmentId, prescriptionId);
            return Ok($"Deleted prescription from appointment {appointmentId} with Id {prescriptionId}");
        }
    }
}
