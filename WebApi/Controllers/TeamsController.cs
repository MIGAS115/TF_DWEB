using ESports.Domain.Data;
using ESports.Domain.Models;
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
        /// </summary>
        /// <returns>Uma lista assíncrona de TeamDTOs.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> GetTeams()
        {
            if (_context.Teams == null)
            {
                return NotFound(new { Message = "A entidade Teams não está disponível." });
            }

            var teamsDto = await _context.Teams
                .Select(t => new TeamDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    LogoPath = t.LogoPath,
                    TotalFavorites = t.FavoritedBy.Count(),
                    TotalMatches = t.HomeMatches.Count() + t.AwayMatches.Count()
                })
                .ToListAsync();

            return Ok(teamsDto);
        }

        /// <summary>
        /// Obtém os detalhes de uma equipa específica através do seu ID.
        /// </summary>
        /// <param name="id">O identificador da equipa.</param>
        /// <returns>O TeamDTO da equipa, se encontrada; caso contrário, HTTP 404.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDTO>> GetTeam(int id)
        {
            if (_context.Teams == null)
            {
                return NotFound(new { Message = "A entidade Teams não está disponível." });
            }

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

            if (teamDto == null)
            {
                return NotFound(new { Message = $"A equipa com o ID {id} não foi encontrada." });
            }

            return Ok(teamDto);
        }

        /// <summary>
        /// Cria uma nova equipa na plataforma.
        /// </summary>
        /// <param name="teamCreateDto">Os dados da equipa a ser criada.</param>
        /// <returns>A equipa recém-criada mapeada para DTO.</returns>
        /// <response code="201">Created - Equipa criada com sucesso.</response>
        /// <response code="400">Bad Request - Os dados submetidos são inválidos.</response>
        [HttpPost]
        public async Task<ActionResult<TeamDTO>> PostTeam(TeamCreateDTO teamCreateDto)
        {
            var novaEquipa = new Team
            {
                Name = teamCreateDto.Name,
                LogoPath = teamCreateDto.LogoPath,
                IsManualOverride = false
            };

            try
            {
                _context.Teams.Add(novaEquipa);
                await _context.SaveChangesAsync();

                var teamDtoRetorno = new TeamDTO
                {
                    Id = novaEquipa.Id,
                    Name = novaEquipa.Name,
                    LogoPath = novaEquipa.LogoPath,
                    TotalFavorites = 0,
                    TotalMatches = 0
                };

                return CreatedAtAction(nameof(GetTeam), new { id = novaEquipa.Id }, teamDtoRetorno);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Atualiza os dados de uma equipa existente.
        /// </summary>
        /// <param name="id">O ID da equipa a editar.</param>
        /// <param name="teamUpdateDto">Os novos dados da equipa.</param>
        /// <returns>Resposta vazia (204 No Content) em caso de sucesso.</returns>
        /// <response code="204">No Content - Equipa atualizada com sucesso.</response>
        /// <response code="400">Bad Request - Dados inválidos.<///response>
        /// <response code="404">Not Found - A equipa especificada não foi encontrada.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeam(int id, TeamCreateDTO teamUpdateDto)
        {
            var equipa = await _context.Teams.FindAsync(id);
            if (equipa == null)
            {
                return NotFound(new { Message = $"A equipa com o ID {id} não foi encontrada." });
            }

            equipa.Name = teamUpdateDto.Name;
            equipa.LogoPath = teamUpdateDto.LogoPath;

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
        /// Remove uma equipa da plataforma salvaguardando a integridade relacional.
        /// </summary>
        /// <param name="id">O ID da equipa a eliminar.</param>
        /// <returns>Resposta vazia (204 No Content) em caso de sucesso.</returns>
        /// <response code="204">No Content - Equipa removida com sucesso.</response>
        /// <response code="400">Bad Request - A equipa possui jogos ou favoritos vinculados e não pode ser eliminada.</response>
        /// <response code="404">Not Found - A equipa especificada não foi encontrada.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var equipa = await _context.Teams.FindAsync(id);
            if (equipa == null)
            {
                return NotFound(new { Message = $"A equipa com o ID {id} não foi encontrada." });
            }

            var temJogosComoCasa = await _context.Matches.AnyAsync(m => m.HomeTeamFK == id);
            var temJogosComoFora = await _context.Matches.AnyAsync(m => m.AwayTeamFK == id);
            var temFavoritos = await _context.Favorites.AnyAsync(f => f.TeamFK == id);

            if (temJogosComoCasa || temJogosComoFora || temFavoritos)
            {
                return BadRequest(new { Message = "Não é possível eliminar a equipa porque existem jogos ou utilizadores associados a ela." });
            }

            try
            {
                _context.Teams.Remove(equipa);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}