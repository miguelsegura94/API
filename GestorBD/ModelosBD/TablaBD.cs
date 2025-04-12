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
    public class Condicion
    {
        public string NombreColumna { get; set; }
        public string ValorRegistro { get; set; }
    }
    public class RegistroMultipleEditar
    {
        public List<Condicion> Condiciones { get; set; }
        public ColumnaBasica ValoresNuevos { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TipoDato
    {
        @int,  
        @double, 
        @string,   
        date,   
        @bool 
    }
    public static class TipoDatoExtensions
    {
        public static string ObtenerTipoSQL(this TipoDato tipo, int? longitud = null)
        {
            switch (tipo)
            {
                case TipoDato.@int:
                    return "INT";
                case TipoDato.@double:
                    return "DECIMAL(18,2)";
                case TipoDato.@string:
                    return (longitud.HasValue && longitud.Value > 0)
        ? $"NVARCHAR({longitud})"
        : "NVARCHAR(MAX)";
                case TipoDato.date:
                    return "DATETIME";
                case TipoDato.@bool:
                    return "BIT";
                default:
                    throw new ArgumentException("Tipo de dato no soportado");
            }
        }
    }
    

}
