namespace WebApi.Models.ViewModels
{
    /// <summary>
    /// Objeto de Transferência de Dados (DTO) para a criação de uma nova equipa.
    /// Encapsula e oculta.
    /// </summary>
    public class TeamCreateDTO
    {
        /// <summary>
        /// Nome oficial da equipa.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Caminho para o ficheiro do logótipo.
        /// </summary>
        public string? LogoPath { get; set; }
    }
}