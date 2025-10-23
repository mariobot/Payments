using Hyip_Payments.Context;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Connection
{
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
        public ActionResult Index()
        {
            string? connString = _configuration.GetConnectionString("DefaultConnection") ?? "Not Found";
            return connString != null ? Content(connString) : Content("Connection string not found.");                            
        }

        // GET: ConnectionController/Details/5
        public ActionResult Details(int id)
        {
            return null;
        }

        // GET: ConnectionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ConnectionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ConnectionController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ConnectionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ConnectionController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ConnectionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
