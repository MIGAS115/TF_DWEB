using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ESports.Domain.Data;
using ESports.Domain.Models;
using WebApi.Models.ViewModels;

namespace WebApi.Controllers;

/// <summary>
/// Controlador da API REST responsável pela consulta, criação, edição e remoção de Equipas na plataforma.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TeamsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TeamsController> _logger;

    /// <summary>
    /// Construtor do controlador de equipas com injeção de dependências do contexto e infraestrutura de logging.
    /// </summary>
    /// <param name="context">Contexto da base de dados.</param>
    /// <param name="logger">Componente para registo de logs do sistema.</param>
    public TeamsController(ApplicationDbContext context, ILogger<TeamsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtém a listagem geral de equipas mapeadas para formato de transferência (DTO).
    /// </summary>
    /// <returns>Uma lista assíncrona contendo os objetos TeamDTO.</returns>
    /// <response code="200">OK - Listagem recuperada com sucesso.</response>
    /// <response code="404">Not Found - O recurso de equipas não se encontra disponível.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// Obtém os detalhes técnicos de uma equipa específica através do seu identificador.
    /// </summary>
    /// <param name="id">O identificador numérico único da equipa.</param>
    /// <returns>O TeamDTO correspondente à equipa solicitada.</returns>
    /// <response code="200">OK - Detalhes da equipa localizados com sucesso.</response>
    /// <response code="404">Not Found - A equipa especificada não existe no sistema.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// Cria e persiste um novo registo de equipa na plataforma.
    /// </summary>
    /// <param name="teamCreateDto">Modelo contendo os dados de inserção da equipa.</param>
    /// <returns>O TeamDTO com os dados da equipa criada e a sua rota de acesso.</returns>
    /// <response code="201">Created - Registo de equipa inserido com sucesso.</response>
    /// <response code="400">Bad Request - Os dados submetidos contêm erros de validação.</response>
    /// <response code="500">Internal Server Error - Ocorreu uma falha inesperada ao gravar no servidor.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TeamDTO>> PostTeam(TeamCreateDTO teamCreateDto)
    {
        if (!ModelState.IsValid || teamCreateDto == null)
        {
            return BadRequest(new { Message = "Dados submetidos inválidos." });
        }

        var novaEquipa = new Team
        {
            Name = teamCreateDto.Name,
            LogoPath = teamCreateDto.LogoPath ?? "default_team.png",
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao tentar inserir a equipa: {Name}", teamCreateDto.Name);
            return StatusCode(500, "Ocorreu um erro interno no servidor ao tentar processar a criação do registo.");
        }
    }

    /// <summary>
    /// Modifica os dados de uma equipa já existente no sistema.
    /// </summary>
    /// <param name="id">O ID numérico da equipa a ser alterada.</param>
    /// <param name="teamUpdateDto">Objeto contendo os novos parâmetros atualizados.</param>
    /// <returns>HTTP 204 No Content se atualizado com sucesso.</returns>
    /// <response code="204">No Content - Atualização efetuada com sucesso.</response>
    /// <response code="400">Bad Request - Parâmetros fornecidos incorretos ou inconsistentes.</response>
    /// <response code="404">Not Found - A equipa indicada não foi localizada.</response>
    /// <response code="500">Internal Server Error - Falha relacional ao persistir as alterações.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> PutTeam(int id, TeamCreateDTO teamUpdateDto)
    {
        if (!ModelState.IsValid || teamUpdateDto == null)
        {
            return BadRequest(new { Message = "Dados de modificação inválidos." });
        }

        var equipa = await _context.Teams.FindAsync(id);
        if (equipa == null)
        {
            return NotFound(new { Message = $"A equipa com o ID {id} não foi encontrada." });
        }

        equipa.Name = teamUpdateDto.Name;
        if (!string.IsNullOrEmpty(teamUpdateDto.LogoPath))
        {
            equipa.LogoPath = teamUpdateDto.LogoPath;
        }

        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro fatal ao atualizar a equipa ID: {Id}", id);
            return StatusCode(500, "Ocorreu um erro interno no servidor ao tentar atualizar o registo.");
        }
    }

    /// <summary>
    /// Remove um registo de equipa do sistema assegurando o bloqueio contra remoções em cascata.
    /// </summary>
    /// <param name="id">O identificador único da equipa a eliminar.</param>
    /// <returns>HTTP 204 No Content se removido com sucesso.</returns>
    /// <response code="204">No Content - Remoção efetuada com sucesso.</response>
    /// <response code="400">Bad Request - Violação de integridade. A equipa possui jogos ou vínculos e está blindada.</response>
    /// <response code="404">Not Found - A equipa indicada não existe.</response>
    /// <response code="500">Internal Server Error - Falha interna ao executar a eliminação no disco da BD.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro de infraestrutura ao tentar eliminar equipa com o ID: {Id}", id);
            return StatusCode(500, "Erro interno de sistema ao tentar processar a eliminação do registo.");
        }
    }
}