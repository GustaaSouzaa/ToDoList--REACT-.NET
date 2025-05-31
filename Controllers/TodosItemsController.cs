using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TodoItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TodoItemsController(AppDbContext context)
        {
            _context = context;
        }

        // Helper para pegar o ID do usuário logado
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
            return int.Parse(userIdClaim?.Value ?? "0");
        }

        // GET: api/todoitems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            var userId = GetUserId();
            return await _context.TodoItems
                .Where(t => t.UserId == userId)
                .Include(t => t.User)
                .ToListAsync();
        }

        // GET: api/todoitems/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            var userId = GetUserId();
            var todoItem = await _context.TodoItems
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todoItem == null)
                return NotFound();

            return todoItem;
        }

        // POST: api/todoitems
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            var userId = GetUserId();
            todoItem.UserId = userId;

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // PUT: api/todoitems/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItem todoItem)
        {
            var userId = GetUserId();

            if (id != todoItem.Id || todoItem.UserId != userId)
                return BadRequest("ID inválido ou usuário não autorizado.");

            var exists = await _context.TodoItems.AnyAsync(t => t.Id == id && t.UserId == userId);
            if (!exists)
                return NotFound();

            _context.Entry(todoItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/todoitems/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var userId = GetUserId();

            var todoItem = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todoItem == null)
                return NotFound();

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
