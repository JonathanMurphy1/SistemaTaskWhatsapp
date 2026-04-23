using Microsoft.AspNetCore.Identity;
using SistemaTaskWhatsapp.Data;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.AccesoDatos.Data.Seed
{
    public static class DbInitializer
    {
        public static async Task InicializarAsync(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<Usuario> userManager
            )
        {
            //Crear los roles si no existen
            foreach (var rol in Enum.GetValues(typeof(Roles)))
            {
                var nombreRol = rol.ToString();

                if (!await roleManager.RoleExistsAsync(nombreRol))
                {
                    await roleManager.CreateAsync(new IdentityRole(nombreRol));
                }
            }

            var programa = context.Programa.FirstOrDefault();

            if (programa == null)
            {
                programa = new Programa
                {
                    Nombre = "SistemaTaskWhatsapp",
                    FechaRegistro = DateTime.Now
                };

                context.Programa.Add(programa);
                await context.SaveChangesAsync();
            }

            string adminEmail = "admin@sistema.com";
            string adminPassword = "Admin123*";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new Usuario
                {
                    UserName = adminEmail,
                    NormalizedUserName = adminEmail.ToUpper(),
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpper(),
                    EmailConfirmed = true,
                    Nombre = "Administrador",
                    ProgramaId = programa.Id,
                    Rol = (int)Roles.Administrador
                };

                var resultado = await userManager.CreateAsync(admin, adminPassword);

                if (resultado.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Roles.Administrador.ToString());
                }
                else
                {
                    Console.WriteLine("Error creando el usuario administrador");

                    foreach (var error in resultado.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                    }
                }
            }

        }
    }
}