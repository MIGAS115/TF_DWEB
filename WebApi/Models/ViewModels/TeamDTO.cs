using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.ViewModels
{
    /// <summary>
    /// Objeto de Transferência de Dados (DTO) para a visualização detalhada de uma equipa.
    /// Otimiza a resposta da API ao agregar métricas e expor os estados do modo híbrido.
    /// </summary>
    public class TeamDTO
    {
        /// <summary>
        /// Identificador único da equipa no sistema local.
        /// </summary>
        [Display(Name = "ID")]
        public int Id { get; set; }

        /// <summary>
        /// Nome oficial da equipa de e-sports.
        /// </summary>
        [Display(Name = "Nome da Equipa")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Caminho relativo para o ficheiro do logótipo armazenado no servidor.
        /// </summary>
        [Display(Name = "Logótipo")]
        public string? LogoPath { get; set; }

        /// <summary>
        /// Identificador da categoria (jogo) à qual a equipa pertence.
        /// </summary>
        [Display(Name = "ID Categoria")]
        public int CategoryFK { get; set; }

        /// <summary>
        /// Nome legível da categoria (ex: Counter-Strike 2, League of Legends).
        /// </summary>
        [Display(Name = "Jogo / Categoria")]
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Total de utilizadores que marcaram esta equipa como favorita (Relação N:M comprimida).
        /// </summary>
        [Display(Name = "Total Favoritos")]
        public int TotalFavorites { get; set; }

        /// <summary>
        /// Total de partidas/jogos registados em que esta equipa participa (Relação 1:N comprimida).
        /// </summary>
        [Display(Name = "Total Partidas")]
        public int TotalMatches { get; set; }

        /// <summary>
        /// Indica se o registo foi manipulado manualmente pelo Administrador.
        /// </summary>
        [Display(Name = "Substituição Manual")]
        public bool IsManualOverride { get; set; }

        /// <summary>
        /// ID correspondente na API externa de e-sports (nulo se registado manualmente).
        /// </summary>
        [Display(Name = "ID Fonte Externa")]
        public string? ExternalSourceId { get; set; }
    }
}