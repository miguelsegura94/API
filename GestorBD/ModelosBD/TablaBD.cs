using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBDD.Modelos
{
    public class TablaBD
    {
        public string Nombre { get; set; }
        public List<Columna> Columnas { get; set; }
    }
    public class Columna
    {
        public string Nombre { get; set; }
        public string TipoDato { get; set; }
        public string? Longitud { get; set; } = null;
        public bool PrimaryKey { get; set; } = false;
        public bool Null { get; set; } = false;
        public ForeignKey? ForeignKey { get; set; } = null;

    }
    public class ForeignKey 
    { 
        public string Nombre { get; set; }
        public string TablaOrigen { get; set; }
        public string ColumnaOrigen { get; set; }
    }


}
