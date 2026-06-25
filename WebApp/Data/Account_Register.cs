using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class Account_Register(DbContextOptions<Account_Register> options) : IdentityDbContext<WebApp.Data.ApplicationUser>(options)
{
}
