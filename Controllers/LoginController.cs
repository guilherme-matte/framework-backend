using framework_backend.Data;

namespace framework_backend.Controllers
{
    public class LoginController
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

    }
}
