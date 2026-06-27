    using System.ComponentModel.DataAnnotations;

    namespace ESports.Domain.Models;

    /// <summary>
    /// Classe abstrata base para os utilizadores do domínio de negócio da plataforma.
    /// Estabelece o desacoplamento do ecossistema físico do ASP.NET Core Identity.
    /// </summary>
    public abstract class MyUser
    {
        /// <summary>
        /// Chave primária interna para a gestão e relacionamentos da lógica de negócio.
        /// </summary>
        [Key] //PK
        public int Id { get; set; }

        /// <summary>
        /// Nome completo do utilizador na plataforma de e-sports.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(100, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 3)]
        [Display(Name = "Nome Completo")]
        public string FullName { get; set; } = null!;

        /// <summary>
        /// Chave conceptual (elo de ligação) que aponta para o ID correspondente 
        /// na tabela nativa AspNetUsers do Identity de segurança.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "ID de Segurança Identity")]
        public string UserID { get; set; } = null!;
    }