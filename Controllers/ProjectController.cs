using framework_backend.Data;
using Microsoft.AspNetCore.Mvc;

namespace framework_backend.Controllers
{
    public class ProjectController
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<ActionResult> CreateProject()
        {
            return new OkResult();
        }
    }
}
