using System.Text.Json;
using BBDD.Exceptions;
using GestorBaseDatos.GestionCarpeta;
using BBDD.Modelos;
using BBDD.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{/*
    [ApiController]
    [Route("[controller]")]
    //TODO un metodo que sea para crear una tabla en bd, pedir: nombre tabla y lista de columnas, y hay que comprobar que una columna sea primaria
    //cada columna tiene nombre de columna tipo de dato, longitud(atributo opcional) y si puede ser null o no, 
    //tiene que tener un bool que diga si es clave primaria esa columna
    //un parametro opcional que sera si tiene foreing key contra otra tabla, 
    //la foreign key nombre columna , nombre de tabla origen, y nombre de columna origen+

    //TODO el metodo tiene que pedir los datos para la nueva tabla
    public class SkinController : ControllerBase
    {
        private readonly ServicioSkin servicioSkin;
        public SkinController(ServicioSkin _servicioSkin)
        {
            servicioSkin = _servicioSkin;
        }
        [HttpGet("")]//devuelve todos
        public IActionResult GetLista()
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioSkin.GetListaUsuario();
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
                gestion = servicioSkin.GetUsuarioById(id);
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
        public IActionResult CrearGestion([FromBody] SkinAPI modeloRequest)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioSkin.CrearUsuarioGestion(modeloRequest);
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
        public IActionResult Actualizar([FromBody] SkinAPI modeloRequest, int id)
        {
            Gestion gestion = new Gestion();
            try
            {
                gestion = servicioSkin.ActualizarUsuario(modeloRequest, id);
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
                gestion = servicioSkin.EliminarUsuarioGestion(id);
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
    }*/
}
