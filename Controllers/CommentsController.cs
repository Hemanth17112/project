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
    [Route("api/comments")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IAppointmentRepository<Comment> repo;
        private readonly ISenderService sender;

        public CommentsController(IAppointmentRepository<Comment> commentRepo, 
            ISenderService senderSrv)
        {
            repo = commentRepo;
            sender = senderSrv;
        }

        [HttpGet("GetComment/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var comment = repo.GetById(id);

            var data = new
            {
                AppointmentId = id,
                comment.CommentMessage
            };

            await sender.SendMessageAsync(data, "appointmentsqueue");
            return Ok(data);
        }

        [HttpPut("EditComment/{id}")]
        public IActionResult Update([FromBody] Comment comment, int id)
        {
            repo.UpdateItem(comment, id);

            return Ok();
        }
    }
}
