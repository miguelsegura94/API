using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        public TipoDato Tipo { get; set; }
        public int? Longitud { get; set; } = 0;
        public bool PrimaryKey { get; set; } = false;
        public bool Null { get; set; } = false;
        public ForeignKey? ForeignKey { get; set; } = null;

    }
    public class ForeignKey 
    { 
        public string NombreColumna { get; set; }
        public string TablaOrigen { get; set; }
    }
    public class ColumnaInsert
    {
        public string Nombre { get; set; }
        public string TipoDato { get; set; }
    }
    public class ColumnaDelete
    {
        public string Nombre { get; set; }
    }
    public class RegistroDelete
    {
        public string Columna { get; set; }
        public string Valor { get; set; }
    }
    public class ColumnaBasica
    {
        public string NombreColumna { get; set; }
        public string ValorRegistro { get; set; }
    }
    public class RegistroEditar
    {
        public List<ColumnaBasica> ValoresExistentes { get; set; }
        public List<ColumnaBasica> ValoresNuevos { get; set; }
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TipoDato
    {
        Entero,  
        Decimal, 
        Texto,   
        Fecha,   
        Booleano 
    }
    //TODO PEKE, como hacer para poder enviar por el json int, y que devuelva INT,ETC
    public static class TipoDatoExtensions
    {
        public static string ObtenerTipoSQL(this TipoDato tipo, int? longitud = null)
        {
            switch (tipo)
            {
                case TipoDato.Entero:
                    return "INT";
                case TipoDato.Decimal:
                    return "DECIMAL(18,2)";
                case TipoDato.Texto:
                    return longitud.HasValue ? $"NVARCHAR({longitud})" : "NVARCHAR(MAX)";
                case TipoDato.Fecha:
                    return "DATETIME";
                case TipoDato.Booleano:
                    return "BIT";
                default:
                    throw new ArgumentException("Tipo de dato no soportado");
            }
        }
    }
    

}
