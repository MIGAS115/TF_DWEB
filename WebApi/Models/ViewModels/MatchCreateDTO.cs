namespace WebApi.Models.ViewModels
{
    /// <summary>
    /// DTO para submeter a criação ou edição de um Jogo.
    /// </summary>
    public class MatchCreateDTO
    {
        /// <summary>
        /// ID da equipa da casa.
        /// </summary>
        public int HomeTeamFK { get; set; }

        /// <summary>
        /// ID da equipa visitante.
        /// </summary>
        public int AwayTeamFK { get; set; }

        /// <summary>
        /// Data e hora planeada para o jogo.
        /// </summary>
        public DateTime MatchDate { get; set; }

        /// <summary>
        /// ID do torneio a que este jogo pertence.
        /// </summary>
        public int TournamentFK { get; set; }
    }
}