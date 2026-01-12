using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Services
{
    public class SupervisorFlowService : ISupervisorFlowService
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public SupervisorFlowService(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }
        public Task<string> ProcesarAsync(Usuario usuario, ChatSession sesion, string mensaje)
        {
            throw new NotImplementedException();
        }
    }
}
