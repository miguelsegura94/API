using BBDD.Exceptions;
using GestorBaseDatos.GestionCarpeta;
using BBDD.Servicios;
using Microsoft.AspNetCore.Mvc;
using BBDD.Modelos;
using System.Text.Json;
using Microsoft.Data.SqlClient;
namespace API.Controllers
{
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
        [HttpGet("{tabla}")]
        public IActionResult GetListaCompleta(string tabla)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioBD.GetListaCompletaServicio(tabla);
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
        [HttpGet("{tabla}/{id}")]
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
        [HttpPost("{tabla}")]
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
        [HttpPost("")]
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
    }
}
