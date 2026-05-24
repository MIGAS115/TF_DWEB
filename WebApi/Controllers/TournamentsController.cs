using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ESports.Domain.Data;
using ESports.Domain.Models;
using WebApi.Models.ViewModels;

namespace WebApi.Controllers;

/// <summary>
/// Controlador da API REST responsável pela consulta, criação, edição e remoção de Torneios na plataforma.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TournamentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TournamentsController> _logger;

    /// <summary>
    /// Construtor do controlador de torneios com injeção de dependências do contexto de dados e infraestrutura de logging.
    /// </summary>
    /// <param name="context">Contexto da base de dados.</param>
    /// <param name="logger">Componente para registo de logs do sistema.</param>
    public TournamentsController(ApplicationDbContext context, ILogger<TournamentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtém a listagem geral de todos os torneios registados com projeção direta para DTO.
    /// </summary>
    /// <returns>Uma lista assíncrona contendo os objetos TournamentDTO.</returns>
    /// <response code="200">OK - Listagem de torneios recuperada com sucesso.</response>
    /// <response code="404">Not Found - O recurso de torneios não está disponível.</response>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<TournamentDTO>>> GetTournaments()
    {
        if (_context.Tournaments == null)
        {
            return NotFound();
        }

        var tournamentsDto = await _context.Tournaments
            .Select(t => new TournamentDTO
            {
                Id = t.Id,
                Name = t.Name,
                TotalMatches = t.MatchesList.Count(),
                TotalTeams = t.TournamentTeams.Count()
            })
            .OrderBy(t => t.Name)
            .ToListAsync();

        return Ok(tournamentsDto);
    }

    /// <summary>
    /// Obtém os detalhes de um torneio específico através do seu identificador único.
    /// </summary>
    /// <param name="id">O identificador numérico do torneio solicitado.</param>
    /// <returns>O objeto TournamentDTO correspondente ao torneio.</returns>
    /// <response code="200">OK - Torneio localizado com sucesso.</response>
    /// <response code="404">Not Found - O torneio indicado não existe no sistema.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<TournamentDTO>> GetTournament(int id)
    {
        if (_context.Tournaments == null)
        {
            return NotFound();
        }

        var tournamentDto = await _context.Tournaments
            .Where(t => t.Id == id)
            .Select(t => new TournamentDTO
            {
                Id = t.Id,
                Name = t.Name,
                TotalMatches = t.MatchesList.Count(),
                TotalTeams = t.TournamentTeams.Count()
            })
            .FirstOrDefaultAsync();

        if (tournamentDto == null)
        {
            return NotFound();
        }

        return Ok(tournamentDto);
    }

    /// <summary>
    /// Cria e persiste um novo registo de Torneio na plataforma.
    /// </summary>
    /// <param name="tournamentCreateDto">Modelo contendo os dados de inserção do torneio.</param>
    /// <returns>O TournamentDTO com os dados do recurso recém-criado.</returns>
    /// <response code="201">Created - Torneio inserido com sucesso.</response>
    /// <response code="400">Bad Request - Os parâmetros submetidos contêm erros de validação.</response>
    /// <response code="500">Internal Server Error - Falha imprevista ao gravar o registo no servidor.</response>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<TournamentDTO>> PostTournament(TournamentCreateDTO tournamentCreateDto)
    {
        if (!ModelState.IsValid || tournamentCreateDto == null)
        {
            return BadRequest();
        }

        var novoTorneio = new Tournament
        {
            Name = tournamentCreateDto.Name,
            IsManualOverride = false
        };

        try
        {
            _context.Tournaments.Add(novoTorneio);
            await _context.SaveChangesAsync();

            var retornoDto = new TournamentDTO
            {
                Id = novoTorneio.Id,
                Name = novoTorneio.Name,
                TotalMatches = 0,
                TotalTeams = 0
            };

            return CreatedAtAction(nameof(GetTournament), new { id = novoTorneio.Id }, retornoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro de infraestrutura ao tentar criar o torneio: {Name}", tournamentCreateDto.Name);
            return StatusCode(500, "Ocorreu um erro interno de sistema ao processar a criação do torneio.");
        }
    }

    /// <summary>
    /// Modifica o nome ou os parâmetros de um torneio já existente no sistema.
    /// </summary>
    /// <param name="id">O ID numérico do torneio a alterar.</param>
    /// <param name="tournamentUpdateDto">Objeto contendo os dados de atualização modificados.</param>
    /// <returns>HTTP 204 No Content se atualizado com sucesso.</returns>
    /// <response code="204">No Content - Atualização efetuada com sucesso.</response>
    /// <response code="400">Bad Request - ID inconsistente ou dados inválidos.</response>
    /// <response code="404">Not Found - O torneio indicado não foi localizado.</response>
    /// <response code="500">Internal Server Error - Falha técnica de concorrência ou persistência na BD.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> PutTournament(int id, TournamentCreateDTO tournamentUpdateDto)
    {
        if (!ModelState.IsValid || tournamentUpdateDto == null)
        {
            return BadRequest();
        }

        var torneio = await _context.Tournaments.FindAsync(id);
        if (torneio == null)
        {
            return NotFound();
        }

        torneio.Name = tournamentUpdateDto.Name;
        torneio.IsManualOverride = true;

        _context.Entry(torneio).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concorrência detetada ao atualizar o torneio ID: {Id}", id);
            return StatusCode(500, "Ocorreu um erro de concorrência ao tentar atualizar o registo.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro fatal ao tentar atualizar o torneio ID: {Id}", id);
            return StatusCode(500, "Erro interno de sistema ao salvar as alterações do torneio.");
        }
    }

    /// <summary>
    /// Remove um torneio do sistema salvaguardando a integridade contra deleções em cascata.
    /// </summary>
    /// <param name="id">O identificador único do torneio a eliminar.</param>
    /// <returns>HTTP 204 No Content se executado com sucesso.</returns>
    /// <response code="204">No Content - Torneio removido com sucesso.</response>
    /// <response code="400">Bad Request - Violação de restrição. O torneio possui jogos ou equipas vinculadas.</response>
    /// <response code="404">Not Found - O torneio indicado não existe.</response>
    /// <response code="500">Internal Server Error - Falha técnica ao cometer a deleção no disco.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteTournament(int id)
    {
        var torneio = await _context.Tournaments.FindAsync(id);
        if (torneio == null)
        {
            return NotFound();
        }

        var temJogosVinculados = await _context.Matches.AnyAsync(m => m.TournamentFK == id);
        var temEquipasVinculadas = await _context.TournamentTeams.AnyAsync(tt => tt.TournamentFK == id);

        if (temJogosVinculados || temEquipasVinculadas)
        {
            return BadRequest(new { Message = "Não é possível remover o torneio porque existem jogos ou equipas associadas a ele." });
        }

        try
        {
            _context.Tournaments.Remove(torneio);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro de infraestrutura ao tentar eliminar o torneio ID: {Id}", id);
            return StatusCode(500, "Ocorreu um erro interno de sistema ao tentar remover o torneio.");
        }
    }
}