using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    /// <summary>
    /// API para gestão de Categorias/Torneios de E-Sports.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor do controlador de categorias.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        /// <summary>
        /// Obtém todas as categorias de torneios registadas.
        /// </summary>
        /// <returns>Lista de DTOs de categorias.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tournament>>> GetCategories()
        {
            /* _context.Tournaments.ToListAsync() é um comando LINQ que se traduz em:
             * SELECT * FROM Tournaments 
             */
            return await _context.Tournaments
                                 .OrderBy(t => t.Name)
                                 .ToListAsync();
        }

        // POST: api/Categories
        /// <summary>
        /// Cria uma nova categoria/torneio no sistema.
        /// </summary>
        /// <param name="novoTorneio">Dados do torneio a introduzir.</param>
        /// <returns>O registo criado.</returns>
        [HttpPost]
        public async Task<ActionResult<Tournament>> PostCategory(Tournament novoTorneio)
        {
            try
            {
                _context.Tournaments.Add(novoTorneio);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return CreatedAtAction("GetCategories", new { id = novoTorneio.Id }, novoTorneio);
        }
    }
}