using ESports.Domain.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models.ViewModels;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controlador da API REST para consulta e gestão de Equipas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém a listagem geral de equipas mapeadas para TeamDTO.
        /// GET: api/teams
        /// </summary>
        /// <returns>Uma lista assíncrona de TeamDTOs.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> GetTeams()
        {
            if (_context.Teams == null)
            {
                return NotFound(new { Message = "A entidade Teams não está disponível no contexto de dados." });
            }

            // Projeção assíncrona LINQ: vai à BD buscar apenas os campos necessários 
            // e converte-os imediatamente em TeamDTO.
            var teamsDto = await _context.Teams
                .Select(t => new TeamDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    LogoPath = t.LogoPath,
                    // Contagem de relações N:M
                    TotalFavorites = t.FavoritedBy.Count(),
                    // Contagem de relações 1:N
                    TotalMatches = t.HomeMatches.Count() + t.AwayMatches.Count()
                })
                .ToListAsync();

            // Devolve HTTP 200 OK
            return Ok(teamsDto);
        }

        /// <summary>
        /// Obtém os detalhes de uma equipa específica através do seu ID.
        /// GET: api/teams/{id}
        /// </summary>
        /// <param name="id">O identificador da equipa.</param>
        /// <returns>O TeamDTO da equipa, se encontrada; caso contrário, HTTP 404.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDTO>> GetTeam(int id)
        {
            if (_context.Teams == null)
            {
                return NotFound(new { Message = "A entidade Teams não está disponível no contexto de dados." });
            }

            // Busca defensiva com projeção via LINQ
            var teamDto = await _context.Teams
                .Where(t => t.Id == id)
                .Select(t => new TeamDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    LogoPath = t.LogoPath,
                    TotalFavorites = t.FavoritedBy.Count(),
                    TotalMatches = t.HomeMatches.Count() + t.AwayMatches.Count()
                })
                .FirstOrDefaultAsync();

            // Validação defensiva onde o registo não existe
            if (teamDto == null)
            {
                // Devolve HTTP 404 Not Found 
                return NotFound(new { Message = $"A equipa com o ID {id} não foi encontrada na plataforma." });
            }

            // Devolve HTTP 200 OK
            return Ok(teamDto);
        }
    }
}