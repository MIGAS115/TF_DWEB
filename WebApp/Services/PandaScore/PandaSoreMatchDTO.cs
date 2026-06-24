using System.Text.Json.Serialization;

namespace WebApp.Services.PandaScore;

/// <summary>
/// Representa a estrutura principal de um jogo devolvida pela API da PandaScore.
/// </summary>
public class PandaScoreMatchDTO
{
    /// <summary>
    /// Identificador único do jogo na API externa.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Nome descritivo do jogo (ex: "Equipa A vs Equipa B").
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Estado atual da partida (ex: "not_started", "running", "finished").
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora de início programada para o jogo.
    /// </summary>
    [JsonPropertyName("scheduled_at")]
    public DateTime ScheduledAt { get; set; }

    /// <summary>
    /// Lista de oponentes (equipas) que participam no jogo.
    /// </summary>
    [JsonPropertyName("opponents")]
    public List<PandaScoreOpponentNodeDTO> Opponents { get; set; } = [];

    /// <summary>
    /// Resultados parciais ou finais do jogo.
    /// </summary>
    [JsonPropertyName("results")]
    public List<PandaScoreResultDTO> Results { get; set; } = [];

    /// <summary>
    /// Informação sobre o torneio a que este jogo pertence.
    /// </summary>
    [JsonPropertyName("tournament")]
    public PandaScoreTournamentDTO Tournament { get; set; } = null!;

    /// <summary>
    /// Informação sobre a categoria (videojogo) associada.
    /// </summary>
    [JsonPropertyName("videogame")]
    public PandaScoreVideogameDTO Videogame { get; set; } = null!;
}

/// <summary>
/// Nó intermediário que encapsula os dados de um oponente.
/// </summary>
public class PandaScoreOpponentNodeDTO
{
    /// <summary>
    /// Tipo do oponente (geralmente "Team").
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Os detalhes efetivos da equipa.
    /// </summary>
    [JsonPropertyName("opponent")]
    public PandaScoreTeamDTO Team { get; set; } = null!;
}

/// <summary>
/// Representa a estrutura de uma equipa na API da PandaScore.
/// </summary>
public class PandaScoreTeamDTO
{
    /// <summary>
    /// Identificador da equipa na API externa.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Nome da equipa.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL da imagem (logótipo) da equipa.
    /// </summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }
}

/// <summary>
/// Representa a pontuação de uma equipa num jogo.
/// </summary>
public class PandaScoreResultDTO
{
    /// <summary>
    /// Identificador da equipa à qual pertence este resultado.
    /// </summary>
    [JsonPropertyName("team_id")]
    public int TeamId { get; set; }

    /// <summary>
    /// Pontuação da equipa.
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; set; }
}

/// <summary>
/// Representa a estrutura de um torneio na API da PandaScore.
/// </summary>
public class PandaScoreTournamentDTO
{
    /// <summary>
    /// Identificador do torneio na API externa.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Nome do torneio (Fase de grupos, Playoffs, etc.).
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Representa a categoria do videojogo na API da PandaScore.
/// </summary>
public class PandaScoreVideogameDTO
{
    /// <summary>
    /// Identificador do videojogo na API externa.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Nome oficial do videojogo (ex: "Counter-Strike", "League of Legends").
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}