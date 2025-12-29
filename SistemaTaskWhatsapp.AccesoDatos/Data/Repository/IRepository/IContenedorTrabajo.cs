using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository
{
    public interface IContenedorTrabajo : IDisposable
    {
        //Aqui se van agregando los diferentes repositorios
        IUsuarioRepository Usuario { get; }
        IEmpresaRepository Empresa { get; }
        IProyectoRepository Proyecto { get; }
        ITareaRepository Tarea { get; }

        Task SaveAsync();
    }
}
