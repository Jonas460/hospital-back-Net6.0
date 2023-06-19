using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using hospital_back.Data;
using hospital_back.Dto;

namespace hospital_back.Controllers
{
    
        [Route("v1/edit/user")]
        [ApiController]
        public class EditUserByIdController : ControllerBase
        {
            private readonly AppDbContext context;

            public EditUserByIdController(AppDbContext dbContext)
            {
                context = dbContext;
            }

            [HttpPost("{id}")]
            public async Task<IActionResult> EditUser(int id, EditUserModel model)
            {
                var user = await context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                user.Name = model.Name;
                user.Email = model.Email;
                user.Password = model.Password;
                user.CellPhone = model.CellPhone;

                await context.SaveChangesAsync();

                return Ok("Dados salvos com sucesso!");
            }
        }

    }

