using BBDD.Exceptions;
using BBDD.Modelos;
using BBDD.GestionCarpeta;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BBDD.Servicios
{
    public class ServicioSkin
    {
        private readonly string _connectionString;

        public ServicioSkin(string connectionString)
        {
            
            _connectionString = connectionString;
        }
        public Gestion GetListaUsuario()
        {
            Gestion gestion = new Gestion();
            gestion.data = new List<Usuario>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {

                    string query = @"SELECT u.id, u.nombre, u.edad, e.id, e.nombre, e.rating, u.dinero 
                         FROM usuarios u 
                         JOIN Elo e ON u.EloId = e.id";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var usuario = new Usuario
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Edad = reader.GetInt32(2),
                            dinero = reader.GetInt32(6),
                            Elo = new Elo
                            {
                                Id = reader.GetInt32(3),
                                nombre = reader.GetString(4),
                                rating = reader.GetInt32(5)
                            }
                        };

                        gestion.data.Add(usuario);
                    }
                }
            }
            catch (SqlException ex)
            {
                gestion.setError($"Error de base de datos.{ex.Message}");
            }
            catch (TimeoutException ex)
            {
                gestion.setError($"Tiempo de espera agotado en la base de datos.{ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                gestion.setError($"Referencia nula en el modelo.{ex.Message}");
            }
            catch (FormatException ex)
            {
                gestion.setError($"Error de formato en el modelo.{ex.Message}");
            }
            catch (Exception ex)
            {
                gestion.setError($"Error inesperado en la base de datos.{ex.Message}");
            }
            return gestion;
        }
        public Gestion GetUsuarioById(int id)
        {
            Gestion gestion = new Gestion();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {//comprobar que exista
                 //si existe recuperar elo
                 //montar la respuesta(sumar usuario y elo)
                 //return usuario
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM usuarios WHERE id=@id";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@id", id);
                    int count = (int)checkCommand.ExecuteScalar();
                    int eloId = -1;
                    if (count > 0)
                    {
                        string query = "SELECT id,nombre,edad,dinero,eloid FROM usuarios WHERE id=@id";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                gestion.data = new Usuario
                                {
                                    Id = reader.GetInt32(0),
                                    Nombre = reader.GetString(1),
                                    Edad = reader.GetInt32(2),
                                    dinero = reader.GetInt32(3)
                                };
                                eloId = reader.GetInt32(4);
                                reader.Close();

                                string queryElo = "SELECT id,nombre,rating FROM elo WHERE id=@eloid";
                                SqlCommand commandElo = new SqlCommand(queryElo, connection);
                                commandElo.Parameters.AddWithValue("@eloid", eloId);
                                using (SqlDataReader readerElo = commandElo.ExecuteReader())
                                {
                                    if (readerElo.Read())
                                    {
                                        gestion.data.Elo = new Elo
                                        {
                                            Id = readerElo.GetInt32(0),
                                            nombre = readerElo.GetString(1),
                                            rating = readerElo.GetInt32(2)
                                        };
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        gestion.setError("Error: El usuario no existe.");
                    }

                }
            }
            catch (SqlException ex)
            {
                gestion.setError($"Error de base de datos.{ex.Message}");
            }
            catch (TimeoutException ex)
            {
                gestion.setError($"Tiempo de espera agotado en la base de datos.{ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                gestion.setError($"Referencia nula en el modelo.{ex.Message}");
            }
            catch (FormatException ex)
            {
                gestion.setError($"Error de formato en el modelo.{ex.Message}");
            }
            catch (Exception ex)
            {
                gestion.setError($"Error inesperado en la base de datos.{ex.Message}");
            }
            return gestion;
        }

        public Gestion CrearUsuarioGestion(UsuarioInsert modeloRequest)
        {
            Gestion gestion = new Gestion();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM usuarios WHERE nombre=@nombre";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@nombre", modeloRequest.Nombre);
                    int count = (int)checkCommand.ExecuteScalar();
                    if (count > 0)
                    {
                        gestion.setError("Error: El usuario ya existe.");
                        return gestion;
                    }
                    if (modeloRequest.Eloid.HasValue)
                    {
                        if (modeloRequest.Eloid == 0)
                        {
                            modeloRequest.Eloid = 1;
                        }
                        else
                        {
                            string checkQueryElo = "SELECT COUNT(*) FROM elo WHERE id=@eloid";
                            SqlCommand checkCommandElo = new SqlCommand(checkQueryElo, connection);
                            checkCommandElo.Parameters.AddWithValue("@eloid", modeloRequest.Eloid);
                            int countElo = (int)checkCommandElo.ExecuteScalar();
                            if (countElo == 0)
                            {
                                gestion.setError("Error: El eloid no existe.");
                                return gestion;
                            }
                        }
                    }
                    else
                    {
                        modeloRequest.Eloid = 1;
                    }
                    string insertQuery = "INSERT INTO usuarios (nombre, edad, Eloid) VALUES (@nombre, @edad, @Eloid)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@nombre", modeloRequest.Nombre);
                    insertCommand.Parameters.AddWithValue("@edad", modeloRequest.Edad);
                    insertCommand.Parameters.AddWithValue("@Eloid", modeloRequest.Eloid);
                    insertCommand.ExecuteNonQuery();
                    gestion.Correct("Usuario creado correctamente");
                    return gestion;
                }
                catch (SqlException ex)
                {
                    gestion.setError($"Error de base de datos.{ex.Message}");
                }
                catch (TimeoutException ex)
                {
                    gestion.setError($"Tiempo de espera agotado en la base de datos.{ex.Message}");
                }
                catch (NullReferenceException ex)
                {
                    gestion.setError($"Referencia nula en el modelo.{ex.Message}");
                }
                catch (FormatException ex)
                {
                    gestion.setError($"Error de formato en el modelo.{ex.Message}");
                }
                catch (Exception ex)
                {
                    gestion.setError($"Error inesperado en la base de datos.{ex.Message}");
                }
            }
            return gestion;
        }
        public Gestion ActualizarUsuario(UsuarioActualizar modeloRequest, int id)
        {
            Gestion gestion = new Gestion();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM usuarios WHERE id=@id";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@id", id);
                    int count = (int)checkCommand.ExecuteScalar();
                    if (count <= 0)
                    {
                        gestion.setError("Error: El usuario no existe.");
                        return gestion;
                    }
                    if (modeloRequest.Eloid.HasValue)
                    {
                        if (modeloRequest.Eloid != 0)
                        {
                            string insertQuery = "UPDATE usuarios SET Nombre = @nombre, edad = @edad, eloid=@eloid WHERE Id = @id";
                            SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                            insertCommand.Parameters.AddWithValue("@nombre", modeloRequest.Nombre);
                            insertCommand.Parameters.AddWithValue("@edad", modeloRequest.Edad);
                            insertCommand.Parameters.AddWithValue("@eloid", modeloRequest.Eloid);
                            insertCommand.Parameters.AddWithValue("@id", id);
                            insertCommand.ExecuteNonQuery();
                            gestion.Correct();
                            return gestion;
                        }
                        else
                        {
                            string insertQuery = "UPDATE usuarios SET Nombre = @nombre, edad = @edad WHERE Id = @id";
                            SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                            insertCommand.Parameters.AddWithValue("@nombre", modeloRequest.Nombre);
                            insertCommand.Parameters.AddWithValue("@edad", modeloRequest.Edad);
                            insertCommand.Parameters.AddWithValue("@id", id);
                            insertCommand.ExecuteNonQuery();
                            gestion.Correct();
                            return gestion;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    gestion.setError($"Error de base de datos.{ex.Message}");
                }
                catch (TimeoutException ex)
                {
                    gestion.setError($"Tiempo de espera agotado en la base de datos.{ex.Message}");
                }
                catch (NullReferenceException ex)
                {
                    gestion.setError($"Error de base de datos.{ex.Message}");
                }
                catch (FormatException ex)
                {
                    gestion.setError($"Error de base de datos.{ex.Message}");
                }
                catch (Exception ex)
                {
                    gestion.setError($"Error de base de datos.{ex.Message}");
                }

            }
            return gestion;

        }
        public Gestion EliminarUsuarioGestion(int id)
        {
            Gestion gestion = new Gestion();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = "DELETE FROM usuarios WHERE id =@id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    int count = (int)command.ExecuteNonQuery();
                    if (count > 0)
                    {
                        gestion.Correct();
                        return gestion;
                    }
                    else
                    {
                        gestion.setError("Error: El usuario no existe.");
                        return gestion;
                    }
                }
                catch (SqlException ex)
                {
                    gestion.setError($"Error de base de datos{ex.Message}");
                }
                catch (TimeoutException ex)
                {
                    gestion.setError($"Tiempo de espera agotado en la base de datos{ex.Message}");
                }
                catch (NullReferenceException ex)
                {
                    gestion.setError($"Referencia nula en base de datos.{ex.Message}");
                }
                catch (FormatException ex)
                {
                    gestion.setError($"Error de formato base de datos.{ex.Message}");
                }
                catch (Exception ex)
                {
                    gestion.setError($"Error inesperado en la base de datos{ex.Message}");
                }
                return gestion;
            }
        }
        public Gestion ModificarDineroUsuario(int id, int cantidad)
        {
            Gestion gestion = new Gestion();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM usuarios WHERE id=@id";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@id", id);
                    int count = (int)checkCommand.ExecuteScalar();
                    if (count > 0)
                    {
                        string updateQuery = "UPDATE usuarios SET dinero=@cantidad WHERE Id = @id";
                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@cantidad", cantidad);
                        updateCommand.Parameters.AddWithValue("@id", id);
                        updateCommand.ExecuteNonQuery();
                        gestion.Correct("Dinero añadido correctamente");

                    }
                    else
                    {
                        gestion.setError("Error: El usuario no existe.");
                        return gestion;
                    }
                }


                catch (SqlException ex)
                {
                    gestion.setError($"Error de base de datos{ex.Message}");
                }
                catch (TimeoutException ex)
                {
                    gestion.setError($"Tiempo de espera agotado en la base de datos{ex.Message}");
                }
                catch (NullReferenceException ex)
                {
                    gestion.setError($"Referencia nula en base de datos.{ex.Message}");
                }
                catch (FormatException ex)
                {
                    gestion.setError($"Error de formato base de datos.{ex.Message}");
                }
                catch (Exception ex)
                {
                    gestion.setError($"Error inesperado en la base de datos{ex.Message}");
                }
                return gestion;

            }
        }
        public Gestion AñadirDineroUsuario(int id, int cantidad)
        {
            Gestion gestion = new Gestion();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    if (id <= 0)
                    {
                        gestion.setError("Error: El ID del usuario no es válido.");
                        return gestion;
                    }

                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM usuarios WHERE id=@id";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@id", id);
                    int count = (int)checkCommand.ExecuteScalar();
                    if (count > 0)
                    {
                        if (cantidad == 0)
                        {
                            gestion.setError("Error: La cantidad a sumar no puede ser 0.");
                            return gestion;
                        }
                        else
                        {
                            string updateQuery = "UPDATE usuarios SET dinero= dinero + @cantidad WHERE Id = @id";
                            SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                            updateCommand.Parameters.AddWithValue("@cantidad", cantidad);
                            updateCommand.Parameters.AddWithValue("@id", id);
                            updateCommand.ExecuteNonQuery();
                            gestion.Correct("Dinero añadido correctamente");
                        }

                    }
                    else
                    {
                        gestion.setError("Error: El usuario no existe.");
                        return gestion;
                    }
                }


                catch (SqlException ex)
                {
                    gestion.setError($"Error de base de datos{ex.Message}");
                }
                catch (TimeoutException ex)
                {
                    gestion.setError($"Tiempo de espera agotado en la base de datos{ex.Message}");
                }
                catch (NullReferenceException ex)
                {
                    gestion.setError($"Referencia nula en base de datos.{ex.Message}");
                }
                catch (FormatException ex)
                {
                    gestion.setError($"Error de formato base de datos.{ex.Message}");
                }
                catch (Exception ex)
                {
                    gestion.setError($"Error inesperado en la base de datos{ex.Message}");
                }
                return gestion;

            }
        }
    }
}
