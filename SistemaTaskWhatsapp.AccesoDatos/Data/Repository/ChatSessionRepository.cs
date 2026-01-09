using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Data;
using SistemaTaskWhatsapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.AccesoDatos.Data.Repository
{
    public class ChatSessionRepository : Repository<ChatSession>, IChatSessionRepository
    {
        private readonly ApplicationDbContext _db;

        public ChatSessionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
