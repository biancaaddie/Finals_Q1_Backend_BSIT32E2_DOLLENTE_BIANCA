using Microsoft.AspNetCore.Mvc;
using Finals_Q1.Models;

namespace Finals_Q1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private static readonly List<Todo> _todos = new();

        [HttpGet]
        public ActionResult<IEnumerable<Todo>> Get()
        {
            return Ok(_todos);
        }

        [HttpPost]
        public ActionResult<Todo> Post([FromBody] Todo todo)
        {
            if (string.IsNullOrWhiteSpace(todo.Title))
            {
                return BadRequest(new { message = "Title is required." });
            }

            var newTodo = new Todo
            {
                Id = Guid.NewGuid(),
                Title = todo.Title.Trim(),
                Completed = todo.Completed
            };

            _todos.Add(newTodo);

            return CreatedAtAction(nameof(Get), new { id = newTodo.Id }, newTodo);
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] Todo updatedTodo)
        {
            if (string.IsNullOrWhiteSpace(updatedTodo.Title))
            {
                return BadRequest(new { message = "Title is required." });
            }

            var existingTodo = _todos.FirstOrDefault(t => t.Id == id);

            if (existingTodo == null)
            {
                return NotFound(new { message = "Todo not found." });
            }

            existingTodo.Title = updatedTodo.Title.Trim();
            existingTodo.Completed = updatedTodo.Completed;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);

            if (todo == null)
            {
                return NotFound(new { message = "Todo not found." });
            }

            _todos.Remove(todo);

            return NoContent();
        }
    }
}