using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Entidade de domínio que representa uma partida ou jogo entre duas equipas num torneio.
    /// Implementa o suporte a dados híbridos e chaves estrangeiras duplas auto-referenciais.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    public class Match
    {
        /// <summary>
        /// Identificador único do jogo na base de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Data e hora agendada para a realização da partida.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
        [Display(Name = "Data e Hora do Jogo")]
        public DateTime MatchDate { get; set; }

        /// <summary>
        /// Pontuação ou número de mapas/rondas vencidos pela equipa visitada (Casa).
        /// </summary>
        [Display(Name = "Resultado Equipa Casa")]
        [Range(0, 500, ErrorMessage = "A {0} deve ser um valor positivo.")]
        public int? HomeScore { get; set; }

        /// <summary>
        /// Pontuação ou número de mapas/rondas vencidos pela equipa visitante (Fora).
        /// </summary>
        [Display(Name = "Resultado Equipa Fora")]
        [Range(0, 500, ErrorMessage = "A {0} deve ser um valor positivo.")]
        public int? AwayScore { get; set; }

        /// <summary>
        /// Estado atual do jogo (ex: Agendado, Ao Vivo, Terminado).
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
        [StringLength(50, ErrorMessage = "O {0} não pode exceder os {1} caracteres.")]
        [Display(Name = "Estado do Jogo")]
        public string Status { get; set; } = null!;

        /// <summary>
        /// Sinalizador (flag) que indica se o registo foi criado ou editado manualmente por um administrador.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Substituição Manual")]
        public bool IsManualOverride { get; set; }

        /// <summary>
        /// Identificador alfanumérico do registo proveniente da API externa.
        /// </summary>
        [StringLength(100, ErrorMessage = "O {0} não pode exceder os {1} caracteres.")]
        [Display(Name = "ID Fonte Externa")]
        public string? ExternalSourceId { get; set; }

        /// <summary>
        /// Chave estrangeira para a equipa visitada (casa).
        /// </summary>
        [Required(ErrorMessage = "A associação à {0} é obrigatória.")]
        [ForeignKey(nameof(HomeTeam))]
        [Display(Name = "Equipa da Casa")]
        public int HomeTeamFK { get; set; }

        /// <summary>
        /// Propriedade de navegação para os detalhes da equipa visitada (casa).
        /// Define explicitamente a ausência de ação em cascata para mitigar ciclos relacionais.
        /// </summary>
        [ValidateNever]
        [InverseProperty(nameof(Team.HomeMatches))]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Team HomeTeam { get; set; } = null!;

        /// <summary>
        /// Chave estrangeira para a equipa visitante (fora).
        /// </summary>
        [Required(ErrorMessage = "A associação à {0} é obrigatória.")]
        [ForeignKey(nameof(AwayTeam))]
        [Display(Name = "Equipa Fora")]
        public int AwayTeamFK { get; set; }

        /// <summary>
        /// Propriedade de navegação para os detalhes da equipa visitante (fora).
        /// Define explicitamente a ausência de ação em cascata para mitigar ciclos relacionais.
        /// </summary>
        [ValidateNever]
        [InverseProperty(nameof(Team.AwayMatches))]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Team AwayTeam { get; set; } = null!;

        /// <summary>
        /// Chave estrangeira para o Torneio.
        /// </summary>
        [Required(ErrorMessage = "A associação a um {0} é obrigatória.")]
        [ForeignKey(nameof(Tournament))]
        [Display(Name = "Torneio")]
        public int TournamentFK { get; set; }

        /// <summary>
        /// Propriedade de navegação para o Torneio.
        /// </summary>
        [ValidateNever]
        public Tournament Tournament { get; set; } = null!;
    }
}