using Hyip_Payments.Context;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Connection
{

    [ApiController]
    [Route("api/[controller]")]
    public class ConnectionController : Controller
    {
        private readonly ILogger<ConnectionController> _logger;
        private readonly IConfiguration _configuration;

        private PaymentsDbContext _context;

        public ConnectionController(ILogger<ConnectionController> logger, IConfiguration configuration, PaymentsDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        // GET: ConnectionController
        [HttpGet]
        public ActionResult GetConnection()
        {
            string? connString = _configuration.GetConnectionString("DefaultConnection") ?? "Not Found";
            bool canConnect = false;
            string message;

            try
            {
                canConnect = _context.Database.CanConnect();
                message = canConnect
                    ? $"Connection string: {connString}\nDatabase connection: SUCCESS"
                    : $"Connection string: {connString}\nDatabase connection: FAILED";
            }
            catch (Exception ex)
            {
                message = $"Connection string: {connString}\nDatabase connection: ERROR - {ex.Message}";
            }

            return Ok(message);
        }
    }
}
