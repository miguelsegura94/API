using System.Data;
using System.Security.Principal;
using BBDD.Modelos;
using GestorBaseDatos.GestionCarpeta;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GestorBaseDatos
{
    
    //TODO un metodo que sea para crear una tabla en bd, pedir: nombre tabla y lista de columnas, y hay que comprobar que una columna sea primaria
    //cada columna tiene nombre de columna tipo de dato, longitud(atributo opcional) y si puede ser null o no, 
    //tiene que tener un bool que diga si es clave primaria esa columna
    //un parametro opcional que sera si tiene foreing key contra otra tabla, 
    //la foreign key nombre columna , nombre de tabla origen, y nombre de columna origen+

    //TODO el metodo tiene que pedir los datos para la nueva tabla

    //TODO HACER EL CRUD

    //TODOO HACER DOCUMENTACION SUMMARY DE LOS METODOS EN GESTOR BD Y EN CONTROLLER
    //BAJAR EL POSTMAN Y CREAR COLECCION QUE SE LLAME GESTION BD

    //TODO IMPORTANTE, EL TENGO QUE CREAR LA CLASE  MODELOTABLA QUE TENGA TODOS LOS ATRIBUTOS NECESARIOS PARA CREAR UNA NUEVA TABLA
    public class GestorBD
    {
        //AQUI VA EL USING QUERY ETC, TODO GENERICO, QUE RECIBE LOS PARAMETROS DESDE EL SERVICIO,
        //HACER CRUD+CREAR TABLA
        // PARA CONVERTIR LOS REGISTROS A LISTA QUE CADA REGISTRO SEA UN DICCIONARY DE STRING DYNAMIC
        /// <summary>
        /// Obtiene todos los registros de una tabla especifica en la base de datos
        /// </summary>
        /// <param name="tablaBuscar">El nombre de la tabla de la que se obtienen registros</param>
        /// <param name="connectionString"> La cadena de conexion a base de datos</param>
        /// <returns>
        /// Devuelve la gestion en el atributo data vienen los registros, ya sea correcta o incorrecta, con su mensaje de error correspondiente si no existe la tabla
        /// </returns>
        public Gestion GetListaCompletaGestor(string tablaBuscar, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    if (ExisteTabla(tablaBuscar, connectionString))
                    {
                        if (ExisteColumna(tablaBuscar, connectionString))
                        {
                            string query = $"SELECT * FROM [{tablaBuscar}]";
                            SqlCommand command = new SqlCommand(query, connection);
                            SqlDataReader reader = command.ExecuteReader();
                            gestion.data = new List<dynamic>();
                            while (reader.Read())
                            {
                                Dictionary<string, dynamic> generico = new Dictionary<string, dynamic>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    generico[reader.GetName(i)] = reader.GetValue(i);
                                }

                                gestion.data.Add(generico);

                            }
                        }
                        else
                        {
                            gestion.setError("Esta tabla no tiene columnas");
                            return gestion;
                        }
                    }
                    else
                    {
                        gestion.setError($"No existe la tabla {tablaBuscar}");
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
            catch (InvalidOperationException ex)
            {
                gestion.setError("Error de conexión a la base de datos: " + ex.Message);
                return gestion;
            }
            catch (Exception ex)
            {
                gestion.setError($"Error inesperado en la base de datos.{ex.Message}");
            }
            return gestion;
        }
        public Gestion GetDatoEnTablaPorIdGestor(string tablaBuscar,int id, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (ExisteTabla(tablaBuscar, connectionString))
                    {
                        if (ExisteColumna(tablaBuscar, connectionString))
                        {
                            string query = $"SELECT * FROM [{tablaBuscar}] WHERE id=@id";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@id", id);
                            SqlDataReader reader = command.ExecuteReader();
                            gestion.data = new List<dynamic>();
                            if (reader.Read())
                            {
                                Dictionary<string, dynamic> generico = new Dictionary<string, dynamic>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    generico[reader.GetName(i)] = reader.GetValue(i);
                                }

                                gestion.data.Add(generico);
                            }
                            else
                            {
                                gestion.setError($"Error: No existe el id {id} en la tabla {tablaBuscar}");
                                return gestion;
                            }
                        }
                        else
                        {
                            gestion.setError("Error: Esta tabla no tiene columnas");
                            return gestion;
                        }
                    }
                    else
                    {
                        gestion.setError($"Error: No existe la tabla {tablaBuscar}");
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
            catch (InvalidOperationException ex)
            {
                gestion.setError("Error de conexión a la base de datos: " + ex.Message);
                return gestion;
            }
            catch (Exception ex)
            {
                gestion.setError($"Error inesperado en la base de datos.{ex.Message}");
            }
            return gestion;
        }
        public Gestion CrearDatoEnTablaGestor(string tablaBuscar, string connectionString)
        {
            //primero saber en que tabla añadir dato
            // comprobar si la tabla existe
            // una vez se en que tabla añadir, devolver el cuestionario(el modelo a rellenar) con los datos necesarios para añadir algo a esa tabla especifica
            //el dato especifico que añadir
            //comprobar que no exista ese dato en esa tabla
            //añadir dato

            //TODO PRIMERO UN METODO GET, PARA SABER ATRIBUTOS DE LA TABLA QUE RELLENAR Y LUEGO EN EL METODO POST, PASAR CADA ATRIBUTO RELLENADO
            Gestion gestion = new Gestion();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (ExisteTabla(tablaBuscar, connectionString))
                    {
                        
                    }
                    else
                    {
                        gestion.setError($"Error: No existe la tabla {tablaBuscar}");
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
            catch (InvalidOperationException ex)
            {
                gestion.setError("Error de conexión a la base de datos: " + ex.Message);
                return gestion;
            }
            catch (Exception ex)
            {
                gestion.setError($"Error inesperado en la base de datos.{ex.Message}");
            }
            return gestion;
            
        }
        public Gestion CrearTablaGestor(TablaBD tabla, string connectionString)
        {
            //hacer un bucle que recorra cada columna de la nueva tabla, y le asigne todos los valores en el query
            Gestion gestion = new Gestion();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string queryNombre = $"CREATE TABLE {tabla.Nombre}";
                    List<string> queryColumnas = new List<string>();
                    foreach (var columna in tabla.Columnas)
                    {
                        string test = $"{columna.Nombre} {columna.TipoDato}"; 
                        if (columna.Longitud != null)
                        {
                            if (!columna.Null)
                            {
                                if (columna.PrimaryKey)
                                {
                                    test = $"{columna.Nombre} {columna.TipoDato} ({columna.Longitud}) NOT NULL PRYMARY KEY ";
                                }
                                else
                                {
                                    test = $"{columna.Nombre} {columna.TipoDato} ({columna.Longitud}) NOT NULL ";
                                }
                            }
                            else
                            {
                                test = $"{columna.Nombre} {columna.TipoDato} ";
                            }
                        }
                        else
                        {
                            test = $"{columna.Nombre} {columna.TipoDato}";
                        }
                        if (columna.ForeignKey != null && !string.IsNullOrEmpty(columna.ForeignKey.Nombre))
                        {
                            test = test + $"FOREIGN KEY({columna.ForeignKey.Nombre}) REFERENCES {columna.ForeignKey.TablaOrigen}({columna.ForeignKey.Nombre})";
                        }
                        queryColumnas.Add(test);
                    }
                    for (int i = 0; i < queryColumnas.Count; i++)
                    {
                        queryNombre +=queryColumnas[i];
                    }
                    Console.WriteLine(queryNombre);
                    //TODO HACERLO CON STRING BUILDER EN VEZ DE LISTA STRING
                    return gestion;
                    
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
            catch (InvalidOperationException ex)
            {
                gestion.setError("Error de conexión a la base de datos: " + ex.Message);
                return gestion;
            }
            catch (Exception ex)
            {
                gestion.setError($"Error inesperado en la base de datos.{ex.Message}");
            }
            return gestion;

        }

        public bool ExisteTabla(string tabla, string connectionString)
        {
            bool existe = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tabla";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tabla", tabla);
                    int count = (int)command.ExecuteScalar();
                    if(count > 0)
                    {
                        existe = true;
                    }
                }
            }

            return existe;
        }
        public bool ExisteColumna(string tabla, string connectionString)
        {
            bool existe = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tabla}'";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tabla", tabla);
                    int columnCount = (int)command.ExecuteScalar();
                    if (columnCount > 0)
                    {
                        existe = true;
                    }
                }
            }

            return existe;
        }
        
        //TODO PEKE, HAY ALGUNA FORMA DE QUE AL AÑADIR UN NUEVO DATO EN UNA TABLA, PRIMERO ME PIDA EL NOMBRE DE LA TALBA Y DEVULVA UN JSON
        //CON LA ESTRUCTURA DEL MODELO A RELLENAR DENTRO DEL MISMO METODO EN EL SWAGGER. O LA PERSONA QUE INTENTA AÑADIR NUEVOS DATOS A UNA TABLA
        //ESPECIFICA YA DEBERIA SABER LA ESTRUCTURA EXACTA QUE TIENE QUE MANDAR.
    }
}
