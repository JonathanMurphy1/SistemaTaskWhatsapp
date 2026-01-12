using SistemaTaskWhatsapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Services
{
    public interface IEmpleadoFlowService
    {
        Task<string> ProcesarAsync(
            Usuario usuario,
            ChatSession sesion,
            string mensaje
        );
    }
}
