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
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return BadRequest(gestion);
        }
        /// <summary>
        /// Hace una llamada a la base de datos y obtiene el registro especifico por id
        /// </summary>
        /// <param name="tabla">Nombre de la tabla en la que buscar el registro</param>
        /// <param name="id">Id del registro que buscar</param>
        /// <returns>Devuelve el registro con el mensaje correspondiente sea correcto o no</returns>
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
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return BadRequest(gestion);
        }
        /// <summary>
        /// Hace una llamada a la base de datos y obtiene uno o mas registros de la tabla especificada
        /// </summary>
        /// <param name="tabla">Tabla donde se buscan los registros</param>
        /// <param name="columna">Columna donde se encuentran los valores a buscar</param>
        /// <param name="valor">Valor a buscar en la columna</param>
        /// <returns>Si es correcto devuelve uno o mas registros,si no un mensaje de error especificando cual es el error</returns>
        [HttpGet("ObtenerRegistroEnTablaPorValor/{tabla}/{columna}/{valor}")]
        public IActionResult ObtenerRegistroEnTablaPorValor(string tabla,string columna,string valor)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioBD.GetRegistroEnTablaPorValorServicio(tabla,columna,valor);
                if (gestion.isCorrect())
                {
                    return Ok(gestion);
                }
                else
                {
                    return NotFound(gestion);
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return BadRequest(gestion);
        }
        /// <summary>
        /// Hace una llamada a la base de datos para obtener el json necesario para crear un nuevo registro en otro metodo
        /// </summary>
        /// <param name="tabla">Nombre de la tabla en la que recibir el json con los atributos</param>
        /// <returns>Devuelve el json con los atributos necesarios para crear un nuevo registro en otro metodo</returns>
        [HttpGet("ObtenerJsonParaRegistro/{tabla}")]
        public IActionResult ObtenerJsonParaRegistro(string tabla)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioBD.ObtenerJsonParaRegistroEnTablaServicio(tabla);
                if (gestion.isCorrect())
                {
                    return Ok(gestion);
                }
                else
                {
                    return NotFound(gestion);
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return BadRequest(gestion);
        }
        /// <summary>
        /// Hace una llamada a la base de datos y crear un nuevo registro en una tabla especificada
        /// </summary>
        /// <param name="tabla">Nombre de la tabla donde crear el nuevo registro</param>
        /// <param name="datosAñadir">Todos los datos necesarios para la creacion de un nuevo registro</param>
        /// <returns>Devuelve el mensaje correspondiente correcto o no, especificando el error</returns>
        [HttpPost("CrearRegistroEnTablaFrombody/{tabla}")]
        public IActionResult CrearRegistroEnTablaFrombody(string tabla, [FromBody] JsonElement datosAñadir)
        {
            Gestion gestion = new Gestion();
            try
            {
                var datos = new Dictionary<string, object>();

                foreach (var property in datosAñadir.EnumerateObject())
                {
                    if (property.Value.ValueKind == JsonValueKind.String)
                    {
                        datos[property.Name] = property.Value.GetString();
                    }
                    else if (property.Value.ValueKind == JsonValueKind.Number)
                    {
                        if (property.Value.TryGetInt32(out int intValue))
                        {
                            datos[property.Name] = intValue;
                        }
                        else if (property.Value.TryGetDouble(out double doubleValue))
                        {
                            datos[property.Name] = doubleValue;
                        }
                    }
                    else if (property.Value.ValueKind == JsonValueKind.True || property.Value.ValueKind == JsonValueKind.False)
                    {
                        datos[property.Name] = property.Value.GetBoolean();
                    }
                    else
                    {
                        datos[property.Name] = null;
                    }
                }
                gestion = servicioBD.CrearRegistroEnTablaFrombodyServicio(tabla, datos);
                if (gestion.isCorrect())
                {
                    return Ok(gestion);
                }
                else
                {
                    return NotFound(gestion);
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return BadRequest(gestion);
        }
        /// <summary>
        /// Hace una llamada a la base de datos y elimina el registro especifico de la tabla
        /// </summary>
        /// <param name="tabla">Nombre de la tabla de la que eliminar el registro</param>
        /// <param name="registro">Nombre de la columna y el valor del registro a eliminar</param>
        /// <returns>Devuelve el mensaje correcto o de error correspondiente</returns>
        [HttpDelete("EliminarRegistro/{tabla}")]
        public IActionResult EliminarRegistroEnTabla(string tabla,[FromBody] RegistroDelete registro)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioBD.EliminarRegistroEnTablaServicio(tabla, registro);
                if (gestion.isCorrect())
                {
                    return Ok(gestion);
                }
                else
                {
                    return NotFound(gestion);
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return BadRequest(gestion);
        }
        /// <summary>
        /// Hace una llamada a la base de datos y crear la tabla con los datos especificados en el modelo
        /// </summary>
        /// <param name="modeloTabla">Parametros que va a tener la nueva tabla</param>
        /// <returns>Devuelve el mensaje correspondiente si es correcto o no</returns>
        [HttpPost("Crear tabla")]
        public IActionResult CrearTabla([FromBody] TablaBD modeloTabla)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioBD.CrearTablaServicio(modeloTabla);
                if (gestion.isCorrect())
                {
                    return Ok(gestion);
                }
                else
                {
                    return NotFound(gestion);
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return BadRequest(gestion);
        }
        /// <summary>
        /// Hace una llamada a la base de datos y elimina la tabla especificada
        /// </summary>
        /// <param name="tabla">Nombre de la tabla que quieres eliminar</param>
        /// <returns>Devuelve el mensaje correspondiente sea correcto o no</returns>
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
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
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
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
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
                if (gestion.isCorrect())
                {
                    return Ok(gestion);
                }
                else
                {
                    return NotFound(gestion);
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
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
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return BadRequest(gestion);
        }
        /// <summary>
        /// Hace una llamada a la base de datos y elimina la columna especifica de la tabla especifica
        /// </summary>
        /// <param name="tabla">Tabla de la que quieres eliminar la columna</param>
        /// <param name="columnaDelete">Columna que quieres eliminar</param>
        /// <returns>Devuelve el mensaje con la gestion diciendo si es correcto o no</returns>
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
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return BadRequest(gestion);
        }
        
    }
}
