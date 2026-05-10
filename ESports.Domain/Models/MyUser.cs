using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
namespace ESports.Domain.Models;

/// <summary>
/// Extensão do IdentityUser para incluir campos personalizados de perfil.
/// </summary>
public class MyUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}