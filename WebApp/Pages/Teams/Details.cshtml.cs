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
    public class DetailsModel : PageModel
    {
        private readonly ESports.Domain.Data.ApplicationDbContext _context;

        public DetailsModel(ESports.Domain.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Team Team { get; set; } = default!;

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
