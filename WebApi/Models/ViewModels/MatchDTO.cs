namespace WebApi.Models.ViewModels
{
    /// <summary>
    /// DTO para visualizar os dados de um Jogo, ocultando os metadados da base de dados.
    /// </summary>
    public class MatchDTO
    {
        public int Id { get; set; }

        /// <summary>
        /// Nome da equipa que joga em casa.
        /// </summary>
        public string HomeTeamName { get; set; } = string.Empty;

        /// <summary>
        /// Nome da equipa visitante.
        /// </summary>
        public string AwayTeamName { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora do jogo.
        /// </summary>
        public DateTime MatchDate { get; set; }
    }
}