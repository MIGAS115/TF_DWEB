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
        /// <param name="teamCreateDto">Os dados da equipa a ser criada (sem metadados de sistema).</param>
        /// <returns>A equipa recém-criada mapeada para DTO.</returns>
        /// <response code="201">Equipa criada com sucesso.</response>
        /// <response code="400">Os dados submetidos são inválidos.</response>
        /// <response code="500">Ocorreu um erro interno no servidor.</response>
        [HttpPost]
        public async Task<ActionResult<TeamDTO>> PostTeam(TeamCreateDTO teamCreateDto)
        {
            // 1. Mapear o DTO recebido para a Entidade de Domínio
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
                
                return StatusCode(500, "Ocorreu um erro interno ao tentar guardar a equipa.");
            }
        }
    }
}