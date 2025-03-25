using System.Collections.Generic;
using System.Text.Json;
using BBDD.Exceptions;
using GestorBaseDatos.GestionCarpeta;
using BBDD.Modelos;
using BBDD.Servicios;
using Microsoft.AspNetCore.Mvc;
using static System.Collections.Specialized.BitVector32;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly ServicioUsuario servicioUsuario;
        
        public UsuarioController(ServicioUsuario _servicioUsuario)
        {
            servicioUsuario = _servicioUsuario;
        }
        //TODO hacer el metodo comprar skin, que busque si existe la skin por id, y si existe, que mire que tenga dinero
        //y asignarsela al usuario restando el precio
        [HttpGet("")]//devuelve todos
        public IActionResult GetLista()
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioUsuario.GetListaUsuario();
                if (gestion.data != null && gestion.data.Count > 0) 
                {
                    gestion.Correct();
                    return Ok(gestion);
                }
                else
                {
                    gestion.setError("No se encontraron usuarios.");
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
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioUsuario.GetUsuarioById(id);
                if (gestion.data != null)
                {
                    gestion.Correct();
                    return Ok(gestion);
                }
                else
                {
                    gestion.setError("No se encontro el usuario.");
                    return NotFound(gestion);
                }
            }
            catch (FormatException ex)
            {
                gestion.setError("Error: No es un número.");
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
            return NotFound(gestion);
        }
        [HttpPost("")]
        public IActionResult CrearGestion([FromBody] UsuarioInsert modeloRequest)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioUsuario.CrearUsuarioGestion(modeloRequest);
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

        [HttpPut("{id}")]
        public IActionResult Actualizar([FromBody] UsuarioActualizar modeloRequest, int id)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioUsuario.ActualizarUsuario(modeloRequest, id);
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
                gestion.setError($"Error: Formato incorrecto en modelo o id.{ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                gestion.setError($"Error null modelo o id: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                gestion.setError($"Error de operación no válida: {ex.Message}");
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
        [HttpPut("modificar-dinero/{id}")]
        public IActionResult ModificarDinero(int id, [FromBody] int cantidad)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioUsuario.ModificarDineroUsuario(id, cantidad);
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
                gestion.setError($"Error: Formato incorrecto en modelo o id.{ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                gestion.setError($"Error null modelo o id: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                gestion.setError($"Error de operación no válida: {ex.Message}");
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
        [HttpPut("anadir-dinero/{id}")]
        public IActionResult AñadirDinero(int id, [FromBody] int cantidad)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioUsuario.AñadirDineroUsuario(id, cantidad);
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
                gestion.setError($"Error: Formato incorrecto en modelo o id.{ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                gestion.setError($"Error null modelo o id: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                gestion.setError($"Error de operación no válida: {ex.Message}");
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
        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioUsuario.EliminarUsuarioGestion(id);
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
