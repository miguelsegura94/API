using BBDD.Exceptions;
using BBDD.Modelos;
using BBDD.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //TODO hacer el CRUD de elo
    public class EloController : ControllerBase
    {
        private readonly ServicioElo servicioElo;
        public EloController(ServicioElo _servicioElo)
        {
            servicioElo = _servicioElo;
        }
        [HttpGet("")]//devuelve todos
        public IActionResult GetListaElo()
        {
            List<Elo> list = null;
            try
            {
                list = servicioElo.GetListaElo();
                if (list != null && list.Count > 0)
                {
                    return Ok(list);
                }
                else
                {
                    throw new APIException("La lista esta vacía.");
                }
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Error interno: La lista no se pudo obtener.");
            }
            catch (APIException ex)
            {
                Console.WriteLine($"Error de API: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
                return StatusCode(500, "Ocurrió un error inesperado.");
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            Elo eloReturn = null;
            try
            {
                eloReturn = servicioElo.GetEloById(id);
                if (eloReturn != null)
                {
                    return Ok(eloReturn);
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Error: No es un número.");
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error de operación inválida: {ex.Message}");
            }
            catch (BDException ex)
            {
                Console.WriteLine($"Error de base de datos: {ex.Message}");
            }
            catch (APIException ex)
            {
                Console.WriteLine($"Error de API : {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            }
            return NotFound();
        }
        [HttpPost("")]
        public IActionResult Crear([FromBody] EloInsert modeloRequest)
        {
            int result = -1;
            try
            {
                result = servicioElo.CrearElo(modeloRequest);
                if (result == 0)
                {
                    return BadRequest(modeloRequest);
                }
                else if (result == 1)
                {
                    return Ok(modeloRequest);
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error: Formato incorrecto del modelo.{ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Error modelo null: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error de operación no válida: {ex.Message}");
            }
            catch (BDException ex)
            {
                Console.WriteLine($"Error de base de datos: {ex.Message}");
            }
            catch (APIException ex)
            {
                Console.WriteLine($"Error de API : {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            }
            return BadRequest(result);
        }
        [HttpPut("{id}")]
        public IActionResult Actualizar([FromBody] EloInsert modeloRequest, int id)
        {
            int result = -1;
            try
            {
                result = servicioElo.ActualizarElo(modeloRequest, id);
                if (result == 0)
                {
                    return BadRequest(modeloRequest);
                }
                else if (result == 1)
                {
                    return Ok(modeloRequest);
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error: Formato incorrecto en modelo o id.{ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Error null modelo o id: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error de operación no válida: {ex.Message}");
            }
            catch (BDException ex)
            {
                Console.WriteLine($"Error de base de datos: {ex.Message}");
            }
            catch (APIException ex)
            {
                Console.WriteLine($"Error de API : {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            }
            return BadRequest(result);
        }
        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            bool result = false;
            try
            {
                result = servicioElo.EliminarElo(id);
                if (!result)
                {
                    return BadRequest(id);
                }
                else if (result)
                {
                    return Ok(id);
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Error: No es un número.");
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error de operación inválida: {ex.Message}");
            }
            catch (BDException ex)
            {
                Console.WriteLine($"Error de base de datos: {ex.Message}");
            }
            catch (APIException ex)
            {
                Console.WriteLine($"Error de API : {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            }
            return BadRequest(result);
        }
    }
}
