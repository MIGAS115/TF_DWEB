using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Entidade de domínio que representa uma equipa de e-sports no sistema.
    /// Suporta inserções híbridas (API externa e manual via administração).
    /// </summary>
    [PrimaryKey(nameof(Id))]
    public class Team : ExternalDataEntity
    {
        /// <summary>
        /// Chave primária da equipa.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome oficial da equipa de e-sports.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(50, ErrorMessage = "O {0} não pode exceder {1} caracteres.")]
        [Display(Name = "Nome da Equipa")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Chave estrangeira para a Categoria (Jogo).
        /// </summary>
        [Required(ErrorMessage = "A {0} é obrigatória.")]
        [ForeignKey(nameof(Category))]
        [Display(Name = "Categoria")]
        public int CategoryFK { get; set; }

        /// <summary>
        /// Propriedade de navegação para a Categoria associada.
        /// </summary>
        [ValidateNever]
        public Category Category { get; set; } = null!;

        /// <summary>
        /// Caminho relativo ou nome do ficheiro do logótipo da equipa armazenado no servidor.
        /// </summary> 
        [StringLength(255, ErrorMessage = "O caminho do {0} não pode exceder os {1} caracteres.")]
        [Display(Name = "Logótipo")]
        public string? LogoPath { get; set; }

        /// <summary>
        /// Coleção de jogos efetuados em casa (Relação 1:N).
        /// </summary>
        [ValidateNever]
        [InverseProperty(nameof(Match.HomeTeam))]
        public ICollection<Match> HomeMatches { get; set; } = [];

        /// <summary>
        /// Coleção de jogos efetuados fora (Relação 1:N).
        /// </summary>
        [ValidateNever]
        [InverseProperty(nameof(Match.AwayTeam))]
        public ICollection<Match> AwayMatches { get; set; } = [];

        /// <summary>
        /// Coleção de favoritos associados à equipa (Relação N:N).
        /// </summary>
        public ICollection<Favorite> FavoritedBy { get; set; } = [];
    }
}