using System.Security.Cryptography;
using System.Text;
using Finals_Q1.Models;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("verify")]
        public IActionResult Verify()
        {
            if (_todos.Count == 0)
            {
                return Ok(new { message = "Chain valid", valid = true });
            }

            for (int i = 0; i < _todos.Count; i++)
            {
                var current = _todos[i];
                var expectedPreviousHash = i == 0 ? "GENESIS" : _todos[i - 1].Hash;

                if (current.PreviousHash != expectedPreviousHash)
                {
                    return Conflict(new
                    {
                        message = "Chain tampered: previous hash mismatch.",
                        valid = false,
                        index = i
                    });
                }

                var recalculatedHash = CalculateHash(
                    current.Id,
                    current.Title,
                    current.Completed,
                    current.PreviousHash
                );

                if (current.Hash != recalculatedHash)
                {
                    return Conflict(new
                    {
                        message = "Chain tampered: hash mismatch.",
                        valid = false,
                        index = i
                    });
                }
            }

            return Ok(new { message = "Chain valid", valid = true });
        }

        [HttpPost]
        public ActionResult<Todo> Post([FromBody] Todo todo)
        {
            if (string.IsNullOrWhiteSpace(todo.Title))
            {
                return BadRequest(new { message = "Title is required." });
            }

            var previousHash = _todos.Count == 0 ? "GENESIS" : _todos.Last().Hash;

            var newTodo = new Todo
            {
                Id = Guid.NewGuid(),
                Title = todo.Title.Trim(),
                Completed = todo.Completed,
                PreviousHash = previousHash
            };

            newTodo.Hash = CalculateHash(
                newTodo.Id,
                newTodo.Title,
                newTodo.Completed,
                newTodo.PreviousHash
            );

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

            RebuildChainFrom(existingTodo.Id);

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
            RebuildEntireChain();

            return NoContent();
        }

        private static void RebuildEntireChain()
        {
            for (int i = 0; i < _todos.Count; i++)
            {
                _todos[i].PreviousHash = i == 0 ? "GENESIS" : _todos[i - 1].Hash;
                _todos[i].Hash = CalculateHash(
                    _todos[i].Id,
                    _todos[i].Title,
                    _todos[i].Completed,
                    _todos[i].PreviousHash
                );
            }
        }

        private static void RebuildChainFrom(Guid id)
        {
            var startIndex = _todos.FindIndex(t => t.Id == id);
            if (startIndex == -1) return;

            for (int i = startIndex; i < _todos.Count; i++)
            {
                _todos[i].PreviousHash = i == 0 ? "GENESIS" : _todos[i - 1].Hash;
                _todos[i].Hash = CalculateHash(
                    _todos[i].Id,
                    _todos[i].Title,
                    _todos[i].Completed,
                    _todos[i].PreviousHash
                );
            }
        }

        private static string CalculateHash(Guid id, string title, bool completed, string previousHash)
        {
            var rawData = $"{id}|{title}|{completed}|{previousHash}";

            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToHexString(bytes);
        }
    }
}