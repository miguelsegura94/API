using BBDD.Exceptions;
using GestorBaseDatos.GestionCarpeta;
using BBDD.Servicios;
using Microsoft.AspNetCore.Mvc;
using BBDD.Modelos;
using System.Text.Json;
using Microsoft.Data.SqlClient;
namespace API.Controllers
{
    [ApiController]
    public class GestorBDController : ControllerBase
    {
        private readonly ServicioBD servicioBD;
        public GestorBDController(ServicioBD _servicioBD)
        {
            servicioBD = _servicioBD;
        }
        //AQUI SE CREAN LOS METODOS QUE UTILIZA LOS METODOS DE GESTORBD, ESTO SE LLAMA DESDE EL SWAGGER 
        //HACER EL CRUD GENERICO +LOS METODOS DEL GESTORBD
        /// <summary>
        /// Hace una llamada a la base de datos y obtiene la lista completa de registros de una tabla específica
        /// </summary>
        /// <param name="tabla">Nombre de la tabla que quieres buscar los registros</param>
        /// <returns>Devuelve la gestion, en caso de que la lista este vacia, añade el error </returns>
        [HttpGet("ObtenerResgistros/{tabla}")]
        public IActionResult GetListaCompletaRegistros(string tabla)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioBD.GetListaCompletaRegistrosServicio(tabla);
                if (gestion.data != null && gestion.data.Count > 0)
                {
                    gestion.Correct();
                    return Ok(gestion);
                }
                else
                {
                    if (string.IsNullOrEmpty(gestion.error))
                    {
                        gestion.setError("Error: No se encontraron registros en la tabla " + tabla);
                    }
                    return NotFound(gestion);
                }
            }
            catch (NullReferenceException ex)
            {
                gestion.setError($"Error de referencia nula: {ex.Message}");
            }
            catch (APIException ex)
            {
                gestion.setError($"Error de API: {ex.Message}");
            }
            catch (BDException ex)
            {
                gestion.setError($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                gestion.setError($"Ocurrió un error inesperado: {ex.Message}");
            }
            return BadRequest(gestion);
        }
        [HttpGet("ObtenerResgistrosPorId{tabla}/{id}")]
        public IActionResult GetDatoEnTablaPorId(string tabla, int id)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioBD.GetDatoEnTablaPorIdServicio(tabla, id);
                if (gestion.data != null && gestion.data.Count > 0)
                {
                    gestion.Correct();
                    return Ok(gestion);
                }
                else
                {
                    if (string.IsNullOrEmpty(gestion.error))
                    {
                        gestion.setError("Error: No se encontraron registros en la tabla " + tabla);
                    }
                    return NotFound(gestion);
                }
            }
            catch (NullReferenceException ex)
            {
                gestion.setError($"Error de referencia nula: {ex.Message}");
            }
            catch (APIException ex)
            {
                gestion.setError($"Error de API: {ex.Message}");
            }
            catch (BDException ex)
            {
                gestion.setError($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                gestion.setError($"Ocurrió un error inesperado: {ex.Message}");
            }
            return BadRequest(gestion);
        }
        
        [HttpPost("NoUsar/{tabla}")]
        public IActionResult CrearDatoEnTabla(string tabla)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioBD.CrearDatoEnTablaServicio(tabla);
                if (gestion.data != null && gestion.data.Count > 0)
                {
                    gestion.Correct();
                    return Ok(gestion);
                }
                else
                {
                    if (string.IsNullOrEmpty(gestion.error))
                    {
                        gestion.setError("Error: No se encontraron registros en la tabla " + tabla);
                    }
                    return NotFound(gestion);
                }
            }
            catch (FormatException ex)
            {
                gestion.setError($"Error: Formato incorrecto del modelo.{ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                gestion.setError($"Error modelo null: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                gestion.setError($"Error de operación no válida: {ex.Message}");
            }
            catch (BDException ex)
            {
                gestion.setError($"Error de base de datos: {ex.Message}");
            }
            catch (JsonException ex)
            {
                gestion.setError($"Error en el formato JSON: {ex.Message}");
            }
            catch (APIException ex)
            {
                gestion.setError($"Error de API : {ex.Message}");
            }
            catch (Exception ex)
            {
                gestion.setError($"Ocurrió un error: {ex.Message}");
            }
            return BadRequest(gestion);
        }
        [HttpPost("Crear tabla")]
        public IActionResult CrearTabla([FromBody] TablaBD modeloTabla)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioBD.CrearTablaServicio(modeloTabla);
            }
            catch (FormatException ex)
            {
                gestion.setError($"Error: Formato incorrecto del modelo.{ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                gestion.setError($"Error modelo null: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                gestion.setError($"Error de operación no válida: {ex.Message}");
            }
            catch (BDException ex)
            {
                gestion.setError($"Error de base de datos: {ex.Message}");
            }
            catch (JsonException ex)
            {
                gestion.setError($"Error en el formato JSON: {ex.Message}");
            }
            catch (APIException ex)
            {
                gestion.setError($"Error de API : {ex.Message}");
            }
            catch (Exception ex)
            {
                gestion.setError($"Ocurrió un error: {ex.Message}");
            }
            return BadRequest(gestion);
        }
        [HttpDelete("EliminarTabla/{tabla}")]
        public IActionResult EliminarTabla(string tabla)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioBD.EliminarTablaServicio(tabla);
                if (gestion.isCorrect())
                {
                    return Ok(gestion);
                }
                else
                {
                    return BadRequest(gestion);
                }
            }
            catch (FormatException ex)
            {
                gestion.setError($"Error: No es un número.{ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                gestion.setError($"Error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                gestion.setError($"Error de operación inválida: {ex.Message}");
            }
            catch (BDException ex)
            {
                gestion.setError($"Error de base de datos: {ex.Message}");
            }
            catch (APIException ex)
            {
                gestion.setError($"Error de API : {ex.Message}");
            }
            catch (Exception ex)
            {
                gestion.setError($"Ocurrió un error: {ex.Message}");
            }
            return BadRequest(gestion);
        }
        /// <summary>
        /// Hace una llamada a la base de datos y obtiene todas las columnas detalladas de una tabla especifica
        /// </summary>
        /// <param name="tabla">Tabla de la que se obtienen todas las columnas</param>
        /// <returns>Devulve la gestion,si es correcta con todas las columnas de la tabla o si es incorrecta un mensaje de error </returns>
        [HttpGet("ObtenerColumnas/{tabla}")]
        public IActionResult GetColumnas(string tabla)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioBD.GetColumnasTablaServicio(tabla);
                if (gestion.data != null && gestion.data.Count > 0)
                {
                    gestion.Correct();
                    return Ok(gestion);
                }
                else
                {
                    if (string.IsNullOrEmpty(gestion.error))
                    {
                        gestion.setError("Error: No se encontraron registros en la tabla " + tabla);
                    }
                    return NotFound(gestion);
                }
            }
            catch (NullReferenceException ex)
            {
                gestion.setError($"Error de referencia nula: {ex.Message}");
            }
            catch (APIException ex)
            {
                gestion.setError($"Error de API: {ex.Message}");
            }
            catch (BDException ex)
            {
                gestion.setError($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                gestion.setError($"Ocurrió un error inesperado: {ex.Message}");
            }
            return BadRequest(gestion);
        }
        /// <summary>
        /// Hace una llamada a la base de datos, y añade la columna con los datos obtenidos de la API
        /// </summary>
        /// <param name="tabla">Nombre de la tabla a la que se va a añadir la columna</param>
        /// <param name="columna">Todos los datos necesarios para la creacion de una nueva columna</param>
        /// <returns>Devuelve la gestion ya sea correcta o incorrecta con su respectivo mensaje</returns>
        [HttpPost("AnadirColumnaCompleta/{tabla}")]
        public IActionResult AñadirColumnaCompletaATabla(string tabla, [FromBody] Columna columna)
        {
            Gestion gestion = new Gestion();

            try
            {
                gestion = servicioBD.AñadirColumnaCompletaATablaServicio(tabla, columna);
            }
            catch (FormatException ex)
            {
                gestion.setError($"Error: Formato incorrecto del modelo.{ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                gestion.setError($"Error modelo null: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                gestion.setError($"Error de operación no válida: {ex.Message}");
            }
            catch (BDException ex)
            {
                gestion.setError($"Error de base de datos: {ex.Message}");
            }
            catch (JsonException ex)
            {
                gestion.setError($"Error en el formato JSON: {ex.Message}");
            }
            catch (APIException ex)
            {
                gestion.setError($"Error de API : {ex.Message}");
            }
            catch (Exception ex)
            {
                gestion.setError($"Ocurrió un error: {ex.Message}");
            }
            return BadRequest(gestion);
        }
        [HttpPost("AnadirColumnaBasica/{tabla}")]
        public IActionResult AñadirColumnaBasicaATabla(string tabla, [FromBody] ColumnaInsert columna)
        {
            Gestion gestion = new Gestion();

            try
            {
                gestion = servicioBD.AñadirColumnaBasicaATablaServicio(tabla, columna);
            }
            catch (FormatException ex)
            {
                gestion.setError($"Error: Formato incorrecto del modelo.{ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                gestion.setError($"Error modelo null: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                gestion.setError($"Error de operación no válida: {ex.Message}");
            }
            catch (BDException ex)
            {
                gestion.setError($"Error de base de datos: {ex.Message}");
            }
            catch (JsonException ex)
            {
                gestion.setError($"Error en el formato JSON: {ex.Message}");
            }
            catch (APIException ex)
            {
                gestion.setError($"Error de API : {ex.Message}");
            }
            catch (Exception ex)
            {
                gestion.setError($"Ocurrió un error: {ex.Message}");
            }
            return BadRequest(gestion);
        }
        [HttpDelete("EliminarColumna/{tabla}")]
        public IActionResult EliminarColumna(string tabla, [FromBody] ColumnaDelete columnaDelete)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioBD.EliminarColumnaTablaServicio(tabla, columnaDelete);
                if (gestion.isCorrect())
                {
                    return Ok(gestion);
                }
                else
                {
                    return BadRequest(gestion);
                }
            }
            catch (FormatException ex)
            {
                gestion.setError($"Error: No es un número.{ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                gestion.setError($"Error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                gestion.setError($"Error de operación inválida: {ex.Message}");
            }
            catch (BDException ex)
            {
                gestion.setError($"Error de base de datos: {ex.Message}");
            }
            catch (APIException ex)
            {
                gestion.setError($"Error de API : {ex.Message}");
            }
            catch (Exception ex)
            {
                gestion.setError($"Ocurrió un error: {ex.Message}");
            }
            return BadRequest(gestion);
        }
        
    }
}
