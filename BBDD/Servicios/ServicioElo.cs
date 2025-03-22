using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBDD.Exceptions;
using BBDD.Modelos;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BBDD.Servicios
{
    public class ServicioElo
    {
        private readonly string _connectionString;
        public ServicioElo(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Elo> GetListaElo()
        {
            List<Elo> elos = new List<Elo>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT id, nombre, rating FROM elo";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        elos.Add(new Elo
                        {
                            Id = reader.GetInt32(0),
                            nombre = reader.GetString(1),
                            rating = reader.GetInt32(2)
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new BDException($"Error de base de datos", ex);
            }
            catch (TimeoutException ex)
            {
                throw new BDException($"Tiempo de espera agotado en la base de datos", ex);
            }
            catch (NullReferenceException ex)
            {
                throw new BDException($"Referencia nula en los datos recuperados.", ex);
            }
            catch (FormatException ex)
            {
                throw new BDException($"Error de formato en los datos recuperados de la base de datos.", ex);
            }
            catch (Exception ex)
            {
                throw new BDException($"Error inesperado en la base de datos", ex);
            }
            return elos;
        }
        public Elo GetEloById(int id)
        {
            //comprobar que existe el elo con ese id
            //si existe buscar el elo y devolverlo
            Elo getElo = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT id, nombre, rating FROM elo WHERE id=@id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            getElo = new Elo
                            {
                                Id = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                rating = reader.GetInt32(2)
                            };
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new BDException($"Error de base de datos", ex);
            }
            catch (TimeoutException ex)
            {
                throw new BDException($"Tiempo de espera agotado en la base de datos", ex);
            }
            catch (NullReferenceException ex)
            {
                throw new BDException($"Referencia nula en los datos recuperados.", ex);
            }
            catch (FormatException ex)
            {
                throw new BDException($"Error de formato en los datos recuperados de la base de datos.", ex);
            }
            catch (Exception ex)
            {
                throw new BDException($"Error inesperado en la base de datos", ex);
            }
            return getElo;
        }
        //comprobar que no exista ya un elo con ese nombre o rating
        //si no existe, guardar el elo en bd y devolver un 1;
        public int CrearElo(EloInsert modeloRequest)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    connection.Open();
                    string checkQueryNombre = "SELECT COUNT(*) FROM elo WHERE nombre=@nombre";
                    SqlCommand checkCommand = new SqlCommand(checkQueryNombre, connection);
                    checkCommand.Parameters.AddWithValue("@nombre", modeloRequest.nombre);
                    int count = (int)checkCommand.ExecuteScalar();
                    if (count == 0)
                    {
                        string checkQueryRating = "SELECT COUNT(*) FROM elo WHERE rating=@rating";
                        SqlCommand checkCommandRating = new SqlCommand(checkQueryRating, connection);
                        checkCommandRating.Parameters.AddWithValue("@rating", modeloRequest.rating);
                        int countRating = (int)checkCommandRating.ExecuteScalar();
                        if (countRating == 0)
                        {
                            string insertQuery = "INSERT INTO elo (nombre, rating) VALUES (@nombre, @rating)";
                            SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                            insertCommand.Parameters.AddWithValue("@nombre", modeloRequest.nombre);
                            insertCommand.Parameters.AddWithValue("@rating", modeloRequest.rating);
                            insertCommand.ExecuteNonQuery();
                            return 1;
                        }
                        else
                        {
                            Console.WriteLine("Error: No se puede crear un Elo con el mismo rating de uno que ya existe.");
                            return 0;
                        }

                    }
                    else
                    {
                        Console.WriteLine("Error: El nombre del Elo ya esta registrado.");
                        return 0;
                    }
                }
                catch (SqlException ex)
                {
                    throw new BDException($"Error de base de datos", ex);
                }
                catch (TimeoutException ex)
                {
                    throw new BDException($"Tiempo de espera agotado en la base de datos", ex);
                }
                catch (NullReferenceException ex)
                {
                    throw new BDException($"Referencia nula en el modelo.", ex);
                }
                catch (FormatException ex)
                {
                    throw new BDException($"Error de formato en el modelo.", ex);
                }
                catch (Exception ex)
                {
                    throw new BDException($"Error inesperado en la base de datos", ex);
                }
            }
        }
        //buscar si existe un elo con ese id
        //buscar si los parametros ya existen
        //si no existen cambiar los atributos del elo por los parametros y devolver un 1
        public int ActualizarElo(EloInsert modeloRequest, int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string checkQueryId = "SELECT COUNT(*) FROM elo WHERE id=@id";
                    SqlCommand checkCommandId = new SqlCommand(checkQueryId, connection);
                    checkCommandId.Parameters.AddWithValue("@id", id);
                    int countId = (int)checkCommandId.ExecuteScalar();
                    if (countId > 0)
                    {
                        string checkQueryNombre = "SELECT COUNT(*) FROM elo WHERE nombre=@nombre";
                        SqlCommand checkCommand = new SqlCommand(checkQueryNombre, connection);
                        checkCommand.Parameters.AddWithValue("@nombre", modeloRequest.nombre);
                        int count = (int)checkCommand.ExecuteScalar();
                        if (count == 0)
                        {
                            string checkQueryRating = "SELECT COUNT(*) FROM elo WHERE rating=@rating";
                            SqlCommand checkCommandRating = new SqlCommand(checkQueryRating, connection);
                            checkCommandRating.Parameters.AddWithValue("@rating", modeloRequest.rating);
                            int countRating = (int)checkCommandRating.ExecuteScalar();
                            if (countRating == 0)
                            {
                                string insertQuery = "UPDATE elo SET nombre = @nombre, rating = @rating WHERE Id = @id";
                                SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                                insertCommand.Parameters.AddWithValue("@nombre", modeloRequest.nombre);
                                insertCommand.Parameters.AddWithValue("@rating", modeloRequest.rating);
                                insertCommand.Parameters.AddWithValue("@id", id);
                                insertCommand.ExecuteNonQuery();
                                return 1;
                            }
                            else
                            {
                                Console.WriteLine("Error: Ya existe un elo con ese rating.");
                                return 0;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error: Ya existe un elo con ese nombre.");
                            return 0;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: No existe ningun elo con ese ID.");
                        return 0;
                    }
                }
                catch (SqlException ex)
                {
                    throw new BDException($"Error de base de datos", ex);
                }
                catch (TimeoutException ex)
                {
                    throw new BDException($"Tiempo de espera agotado en la base de datos", ex);
                }
                catch (NullReferenceException ex)
                {
                    throw new BDException($"Referencia nula en el modelo o id.", ex);
                }
                catch (FormatException ex)
                {
                    throw new BDException($"Error de formato en el modelo o id.", ex);
                }
                catch (Exception ex)
                {
                    throw new BDException($"Error inesperado en la base de datos", ex);
                }

            }
        }
        public bool EliminarElo(int id)
        {
            List<Usuario> usuariosPonerEloUnraked = new List<Usuario>();
            try
            {
                if (id != 1)
                {

                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        string checkQueryId = "SELECT COUNT(*) FROM usuarios WHERE eloid=@id";
                        SqlCommand checkCommandId = new SqlCommand(checkQueryId, connection);
                        checkCommandId.Parameters.AddWithValue("@id", id);
                        int countId = (int)checkCommandId.ExecuteScalar();
                        if (countId > 0)
                        {
                            string updateQuery = "UPDATE usuarios SET eloid = 1 WHERE eloid = @id";
                            SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                            updateCommand.Parameters.AddWithValue("@id", id);
                            updateCommand.ExecuteNonQuery();
                            string queryDelete = "DELETE FROM elo WHERE id = @id";
                            SqlCommand commandDelete = new SqlCommand(queryDelete, connection);
                            commandDelete.Parameters.AddWithValue("@id", id);
                            commandDelete.ExecuteNonQuery();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error: El elo con ese ID no se puede eliminar.");
                    return false;
                }
            }
            catch (SqlException ex)
            {
                throw new BDException($"Error de base de datos", ex);
            }
            catch (TimeoutException ex)
            {
                throw new BDException($"Tiempo de espera agotado en la base de datos", ex);
            }
            catch (NullReferenceException ex)
            {
                throw new BDException($"Referencia nula en base de datos.", ex);
            }
            catch (FormatException ex)
            {
                throw new BDException($"Error de formato base de datos.", ex);
            }
            catch (Exception ex)
            {
                throw new BDException($"Error inesperado en la base de datos", ex);
            }
            //comprobar que el elo que recibes no es el unranked
            //si es el unraked ese elo no se puede eliminar
            //si algun usuario tiene ese elo, asignar unranked y eliminar el elo con ese id

        }
    }
}
