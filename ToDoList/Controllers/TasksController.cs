using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using ToDoList.Hubs;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class TasksController : Controller
    {
        private readonly ToDoListDBContext _context;
        private readonly IHubContext<ToDoListHub> _hubContext;
        public TasksController(ToDoListDBContext context, IHubContext<ToDoListHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
              return View();
        }

        public async Task<ActionResult> BuildListOfToDos()
        {
            GetDonePercentage();
            return PartialView("ListOfToDos", await _context.Tasks.ToListAsync());
        }

        public async Task<IActionResult> CreateTask([Bind("Id,Description")] TaskItem task)
        {
            if (ModelState.IsValid)
            {
                task.Id = Guid.NewGuid();
                task.Done = false;
                _context.Add(task);
                await _context.SaveChangesAsync();
                SendToClients();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AjaxEdit(Guid? id, bool value)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            task.Done = value;
            _context.Entry(task).State = EntityState.Modified;
            _context.SaveChanges();
            SendToClients();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteTask(Guid id)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'ToDoListDBContext.Tasks'  is null.");
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
            }
            
            await _context.SaveChangesAsync();
            SendToClients();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteAll()
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'ToDoListDBContext.Tasks'  is null.");
            }
            foreach(var task in _context.Tasks)
            {
                _context.Tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            SendToClients();
            return RedirectToAction(nameof(Index));
        }

        private void GetDonePercentage()
        {
            var tasks = _context.Tasks;
            int completeCount = 0;
            foreach(var task in tasks)
            {
                if(task.Done)
                {
                    completeCount++;
                }
            }
            ViewBag.Percent = tasks.Count() == 0 ? 0 : Math.Round(100f * ((float)completeCount / (float)tasks.Count()));
        }

        private async void SendToClients()
        {
            await _hubContext.Clients.All.SendAsync("ListOfToDosHasChanged");
        }

        private bool TaskExists(Guid id)
        {
          return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
