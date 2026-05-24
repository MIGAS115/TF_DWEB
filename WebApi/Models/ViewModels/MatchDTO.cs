namespace WebApi.Models.ViewModels
{
    /// <summary>
    /// Objeto de Transferência de Dados para representação detalhada de um Jogo.
    /// </summary>
    public class MatchDTO
    {
        /// <summary>
        /// Identificador Único do Jogo na Base de Dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome da Equipa que joga em Casa (Visitada).
        /// </summary>
        public string HomeTeamName { get; set; } = string.Empty;

        /// <summary>
        /// Nome da Equipa que joga Fora (Visitante).
        /// </summary>
        public string AwayTeamName { get; set; } = string.Empty;

        /// <summary>
        /// Data e Hora programada para o início do confronto.
        /// </summary>
        public DateTime MatchDate { get; set; }
    }
}