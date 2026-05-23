namespace WebApi.Models.ViewModels
{
    /// <summary>
    /// Objeto de Transferência de Dados (DTO) para a entidade Team.
    /// Encapsula e oculta metadados do sistema (IsManualOverride, ExternalSourceId) 
    /// seguindo as melhores práticas e regras da UC.
    /// </summary>
    public class TeamDTO
    {
        /// <summary>
        /// Identificador único da equipa.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome oficial da equipa.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Caminho para o ficheiro do logótipo.
        /// </summary>
        public string? LogoPath { get; set; }

        /// <summary>
        /// Demonstração da Relação N:M (Equipa <-> Utilizador Normal via Favorite).
        /// Em vez de expor a lista completa de utilizadores (o que causaria ciclos e peso na resposta),
        /// agregamos o total de utilizadores que marcaram esta equipa como favorita.
        /// </summary>
        public int TotalFavorites { get; set; }

        /// <summary>
        /// Demonstração da Relação 1:N (Equipa <-> Match).
        /// Total de jogos em que a equipa participa.
        /// </summary>
        public int TotalMatches { get; set; }
    }
}