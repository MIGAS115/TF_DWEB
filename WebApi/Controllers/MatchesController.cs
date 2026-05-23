using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models.ViewModels;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controlador da API REST para consulta e gestão de Jogos (Matches).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MatchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém a listagem de todos os jogos.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchDTO>>> GetMatches()
        {
            if (_context.Matches == null)
                return NotFound(new { Message = "A entidade Matches não está disponível." });

            var matchesDto = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Select(m => new MatchDTO
                {
                    Id = m.Id,
                    HomeTeamName = m.HomeTeam != null ? m.HomeTeam.Name : "Desconhecida",
                    AwayTeamName = m.AwayTeam != null ? m.AwayTeam.Name : "Desconhecida",
                    MatchDate = m.MatchDate 
                })
                .ToListAsync();

            return Ok(matchesDto);
        }

        /// <summary>
        /// Obtém um jogo específico pelo seu ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDTO>> GetMatch(int id)
        {
            var match = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
                return NotFound(new { Message = $"O jogo com o ID {id} não foi encontrado." });

            var matchDto = new MatchDTO
            {
                Id = match.Id,
                HomeTeamName = match.HomeTeam?.Name ?? "Desconhecida",
                AwayTeamName = match.AwayTeam?.Name ?? "Desconhecida",
                MatchDate = match.MatchDate
            };

            return Ok(matchDto);
        }

        /// <summary>
        /// Cria um novo jogo.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MatchDTO>> PostMatch(MatchCreateDTO matchCreateDto)
        {
            var novoJogo = new Match
            {
                HomeTeamFK = matchCreateDto.HomeTeamFK,
                AwayTeamFK = matchCreateDto.AwayTeamFK,
                MatchDate = matchCreateDto.MatchDate, 
                TournamentFK = matchCreateDto.TournamentFK 
            };

            try
            {
                _context.Matches.Add(novoJogo);
                await _context.SaveChangesAsync();

                await _context.Entry(novoJogo).Reference(m => m.HomeTeam).LoadAsync();
                await _context.Entry(novoJogo).Reference(m => m.AwayTeam).LoadAsync();

                var matchDtoRetorno = new MatchDTO
                {
                    Id = novoJogo.Id,
                    HomeTeamName = novoJogo.HomeTeam?.Name ?? "Desconhecida",
                    AwayTeamName = novoJogo.AwayTeam?.Name ?? "Desconhecida",
                    MatchDate = novoJogo.MatchDate
                };

                return CreatedAtAction(nameof(GetMatch), new { id = novoJogo.Id }, matchDtoRetorno);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro ao guardar o jogo.");
            }
        }

        /// <summary>
        /// Edita um jogo existente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMatch(int id, MatchCreateDTO matchUpdateDto)
        {
            var jogo = await _context.Matches.FindAsync(id);
            if (jogo == null)
                return NotFound(new { Message = $"O jogo com o ID {id} não foi encontrado." });

            jogo.HomeTeamFK = matchUpdateDto.HomeTeamFK;
            jogo.AwayTeamFK = matchUpdateDto.AwayTeamFK;
            jogo.MatchDate = matchUpdateDto.MatchDate;
            jogo.TournamentFK = matchUpdateDto.TournamentFK; 

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno ao atualizar o jogo.");
            }
        }

        /// <summary>
        /// Elimina um jogo.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var jogo = await _context.Matches.FindAsync(id);
            if (jogo == null)
                return NotFound();

            _context.Matches.Remove(jogo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}