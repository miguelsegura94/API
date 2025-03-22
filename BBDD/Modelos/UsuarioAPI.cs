using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBDD.Modelos
{
    public class UsuarioAPI
    {
    }
    public class UsuarioActualizar
    {
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public int? Eloid { get; set; }
    }
    public class UsuarioInsert()
    {
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public int? Eloid { get; set; }
    }
}
