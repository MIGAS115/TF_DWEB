using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams
{
    /// <summary>
    /// Modelo de página para a visualização dos detalhes de uma equipa de E-Sports.
    /// </summary>
    public class DetailsModel : PageModel
    {
        /// <summary>
        /// Contexto de acesso à base de dados da aplicação.
        /// </summary>
        private readonly ESports.Domain.Data.ApplicationDbContext _context;

        /// <summary>
        /// Construtor do modelo de detalhes, responsável por injetar o contexto de dados.
        /// </summary>
        /// <param name="context">O contexto de base de dados da aplicação.</param>
        public DetailsModel(ESports.Domain.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Objeto que armazena os dados da equipa a ser exibida na página.
        /// </summary>
        public Team Team { get; set; } = default!;

        /// <summary>
        /// Trata o pedido HTTP GET assíncrono para carregar os detalhes de uma equipa específica através do seu ID.
        /// </summary>
        /// <param name="id">O identificador único da equipa.</param>
        /// <returns>
        /// Um <see cref="IActionResult"/> que renderiza a página com os detalhes da equipa se encontrada, 
        /// ou uma resposta de erro <see cref="NotFoundResult"/> caso contrário.
        /// </returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FirstOrDefaultAsync(m => m.Id == id);

            if (team is not null)
            {
                Team = team;

                return Page();
            }

            return NotFound();
        }
    }
}
