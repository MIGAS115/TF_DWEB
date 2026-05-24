using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ESports.Domain.Data;
using ESports.Domain.Models;
using WebApi.Models.ViewModels;

namespace WebApi.Controllers;

/// <summary>
/// Controlador da API REST responsável pela consulta, agendamento, modificação e eliminação de Jogos (Matches).
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class MatchesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MatchesController> _logger;

    /// <summary>
    /// Construtor do controlador de jogos com injeção de dependências do contexto de dados e infraestrutura de logging.
    /// </summary>
    /// <param name="context">Contexto da base de dados.</param>
    /// <param name="logger">Componente para registo de logs de sistema.</param>
    public MatchesController(ApplicationDbContext context, ILogger<MatchesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtém a listagem completa de jogos agendados ou realizados na plataforma com projeção de dados das equipas.
    /// </summary>
    /// <returns>Uma lista assíncrona contendo objetos do tipo MatchDTO.</returns>
    /// <response code="200">OK - Listagem geral de jogos obtida com sucesso.</response>
    /// <response code="404">Not Found - O recurso de jogos não está disponível.</response>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<MatchDTO>>> GetMatches()
    {
        if (_context.Matches == null)
        {
            return NotFound(new { Message = "A entidade Matches não está disponível." });
        }

        var matchesDto = await _context.Matches
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
    /// Obtém os detalhes de um jogo específico através do seu identificador único.
    /// </summary>
    /// <param name="id">O ID numérico do jogo solicitado.</param>
    /// <returns>O objeto MatchDTO com as informações detalhadas da partida.</returns>
    /// <response code="200">OK - Jogo localizado com sucesso.</response>
    /// <response code="404">Not Found - O jogo com o ID especificado não existe.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<MatchDTO>> GetMatch(int id)
    {
        if (_context.Matches == null)
        {
            return NotFound(new { Message = "A entidade Matches não está disponível." });
        }

        var matchDto = await _context.Matches
            .Where(m => m.Id == id)
            .Select(m => new MatchDTO
            {
                Id = m.Id,
                HomeTeamName = m.HomeTeam != null ? m.HomeTeam.Name : "Desconhecida",
                AwayTeamName = m.AwayTeam != null ? m.AwayTeam.Name : "Desconhecida",
                MatchDate = m.MatchDate
            })
            .FirstOrDefaultAsync();

        if (matchDto == null)
        {
            return NotFound(new { Message = $"O jogo com o ID {id} não foi encontrado." });
        }

        return Ok(matchDto);
    }

    /// <summary>
    /// Cria e agenda um novo jogo entre duas equipas distintas vinculadas a um Torneio.
    /// </summary>
    /// <param name="matchCreateDto">Modelo contendo as chaves estrangeiras e a cronologia da partida.</param>
    /// <returns>O MatchDTO representativo do jogo inserido.</returns>
    /// <response code="201">Created - Jogo agendado com sucesso na plataforma.</response>
    /// <response code="400">Bad Request - Erros de validação lógica ou chaves em duplicado.</response>
    /// <response code="500">Internal Server Error - Falha relacional ou exceção de persistência no servidor.</response>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<MatchDTO>> PostMatch(MatchCreateDTO matchCreateDto)
    {
        if (!ModelState.IsValid || matchCreateDto == null)
        {
            return BadRequest(new { Message = "Dados de inserção de jogo inválidos." });
        }

        if (matchCreateDto.HomeTeamFK == matchCreateDto.AwayTeamFK)
        {
            return BadRequest(new { Message = "A equipa da casa não pode ser igual à equipa visitante." });
        }

        var homeExists = await _context.Teams.AnyAsync(t => t.Id == matchCreateDto.HomeTeamFK);
        var awayExists = await _context.Teams.AnyAsync(t => t.Id == matchCreateDto.AwayTeamFK);
        var tournamentExists = await _context.Tournaments.AnyAsync(t => t.Id == matchCreateDto.TournamentFK);

        if (!homeExists || !awayExists || !tournamentExists)
        {
            return BadRequest(new { Message = "Uma ou mais chaves estrangeiras fornecidas (Equipas ou Torneio) não existem na base de dados." });
        }

        var novoJogo = new Match
        {
            HomeTeamFK = matchCreateDto.HomeTeamFK,
            AwayTeamFK = matchCreateDto.AwayTeamFK,
            MatchDate = matchCreateDto.MatchDate,
            TournamentFK = matchCreateDto.TournamentFK,
            IsManualOverride = false
        };

        try
        {
            _context.Matches.Add(novoJogo);
            await _context.SaveChangesAsync();

            var retornoDto = await _context.Matches
                .Where(m => m.Id == novoJogo.Id)
                .Select(m => new MatchDTO
                {
                    Id = m.Id,
                    HomeTeamName = m.HomeTeam != null ? m.HomeTeam.Name : "Desconhecida",
                    AwayTeamName = m.AwayTeam != null ? m.AwayTeam.Name : "Desconhecida",
                    MatchDate = m.MatchDate
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetMatch), new { id = novoJogo.Id }, retornoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro de infraestrutura ao criar o jogo na data {Date}", matchCreateDto.MatchDate);
            return StatusCode(500, "Ocorreu um erro interno de sistema ao tentar agendar a partida.");
        }
    }

    /// <summary>
    /// Modifica os parâmetros, equipas ou cronograma de um jogo existente na plataforma.
    /// </summary>
    /// <param name="id">O ID numérico do jogo a alterar.</param>
    /// <param name="matchUpdateDto">Objeto contendo os dados de atualização modificados.</param>
    /// <returns>HTTP 204 No Content em caso de sucesso.</returns>
    /// <response code="204">No Content - Jogo atualizado com sucesso.</response>
    /// <response code="400">Bad Request - Parâmetros inválidos ou colisão de equipas.</response>
    /// <response code="404">Not Found - O jogo indicado não foi localizado.</response>
    /// <response code="500">Internal Server Error - Falha técnica ao cometer a atualização na base de dados.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> PutMatch(int id, MatchCreateDTO matchUpdateDto)
    {
        if (!ModelState.IsValid || matchUpdateDto == null)
        {
            return BadRequest(new { Message = "Dados de modificação de jogo inválidos." });
        }

        if (matchUpdateDto.HomeTeamFK == matchUpdateDto.AwayTeamFK)
        {
            return BadRequest(new { Message = "A equipa da casa não pode ser igual à equipa visitante." });
        }

        var jogo = await _context.Matches.FindAsync(id);
        if (jogo == null)
        {
            return NotFound(new { Message = $"O jogo com o ID {id} não foi encontrado." });
        }

        var homeExists = await _context.Teams.AnyAsync(t => t.Id == matchUpdateDto.HomeTeamFK);
        var awayExists = await _context.Teams.AnyAsync(t => t.Id == matchUpdateDto.AwayTeamFK);
        var tournamentExists = await _context.Tournaments.AnyAsync(t => t.Id == matchUpdateDto.TournamentFK);

        if (!homeExists || !awayExists || !tournamentExists)
        {
            return BadRequest(new { Message = "As novas chaves de equipas ou torneio especificadas não existem." });
        }

        jogo.HomeTeamFK = matchUpdateDto.HomeTeamFK;
        jogo.AwayTeamFK = matchUpdateDto.AwayTeamFK;
        jogo.MatchDate = matchUpdateDto.MatchDate;
        jogo.TournamentFK = matchUpdateDto.TournamentFK;
        jogo.IsManualOverride = true;

        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro de infraestrutura ao tentar atualizar o jogo ID: {Id}", id);
            return StatusCode(500, "Ocorreu um erro interno de sistema ao tentar atualizar a partida.");
        }
    }

    /// <summary>
    /// Remove permanentemente um jogo do sistema.
    /// </summary>
    /// <param name="id">O ID do jogo a ser eliminado.</param>
    /// <returns>HTTP 204 No Content se executado com sucesso.</returns>
    /// <response code="204">No Content - Jogo removido com sucesso.</response>
    /// <response code="404">Not Found - O jogo indicado não existe.</response>
    /// <response code="500">Internal Server Error - Falha técnica ao processar a eliminação no disco.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteMatch(int id)
    {
        var jogo = await _context.Matches.FindAsync(id);
        if (jogo == null)
        {
            return NotFound(new { Message = $"O jogo com o ID {id} não foi encontrado." });
        }

        try
        {
            _context.Matches.Remove(jogo);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro de infraestrutura ao tentar eliminar o jogo ID: {Id}", id);
            return StatusCode(500, "Ocorreu um erro interno de sistema ao tentar remover a partida.");
        }
    }
}