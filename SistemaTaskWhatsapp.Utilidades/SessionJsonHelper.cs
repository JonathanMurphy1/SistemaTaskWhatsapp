using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Utilidades
{
    public static class SessionJsonHelper
    {
        public static T GetData<T>(string? json) where T : new()
        {
            if (string.IsNullOrEmpty(json))
                return new T();

            return JsonSerializer.Deserialize<T>(json) ?? new T();
        }

        public static string SetData<T>(T data)
        {
            return JsonSerializer.Serialize(data);
        }
    }
}
