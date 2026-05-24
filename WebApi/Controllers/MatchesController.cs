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
        /// <param name="matchCreateDto">Dados para a criação do jogo.</param>
        /// <returns>O jogo recém-criado.</returns>
        /// <response code="21">Created - Jogo criado com sucesso.</response>
        /// <response code="400">Bad Request - Dados inválidos ou violação de chaves estrangeiras.</response>
        [HttpPost]
        public async Task<ActionResult<MatchDTO>> PostMatch(MatchCreateDTO matchCreateDto)
        {
            if (matchCreateDto.HomeTeamFK == matchCreateDto.AwayTeamFK)
            {
                return BadRequest(new { Message = "A equipa da casa não pode ser igual à equipa visitante." });
            }

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

                var matchDtoRetorno = new MatchDTO
                {
                    Id = novoJogo.Id,
                    HomeTeamName = "Criada com Sucesso",
                    AwayTeamName = "Criada com Sucesso",
                    MatchDate = novoJogo.MatchDate
                };

                return CreatedAtAction(nameof(GetMatch), new { id = novoJogo.Id }, matchDtoRetorno);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Edita um jogo existente.
        /// </summary>
        /// <param name="id">ID do jogo a editar.</param>
        /// <param name="matchUpdateDto">Novos dados do jogo.</param>
        /// <response code="204">No Content - Atualizado com sucesso.</response>
        /// <response code="400">Bad Request - Dados inválidos ou chaves corrompidas.</response>
        /// <response code="404">Not Found - Jogo não encontrado.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMatch(int id, MatchCreateDTO matchUpdateDto)
        {
            if (matchUpdateDto.HomeTeamFK == matchUpdateDto.AwayTeamFK)
            {
                return BadRequest(new { Message = "A equipa da casa não pode ser igual à equipa visitante." });
            }

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
                return BadRequest();
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