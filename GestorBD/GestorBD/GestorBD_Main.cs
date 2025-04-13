using System;
using System.Data;
using System.Drawing;
using System.Security.Principal;
using System.Text;
using BBDD.Modelos;
using GestorBaseDatos.GestionCarpeta;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GestorBaseDatos.GestorBD.GestorBD
{

    
    

    //TODO MASTERCLASS SERIALIZACION Y DESERIALIZACION PARA UNIFICAR "FILTROS"
    //BAJAR EL POSTMAN Y CREAR COLECCION QUE SE LLAME GESTION BD

    public partial class GestorBD
    {
        //AQUI ESTAN LOS METODOS PARA COMPROBAR QUE EXISTE LA TABLA, LA COLUMNA Y LOS CARACTERES VÁLIDOS


        /// <summary>
        /// Obtiene un registro específico de una tabla específica
        /// </summary>
        /// <param name="tablaBuscar">El nombre de la tabla de la que se obtienen registros </param>
        /// <param name="id">El id del que se obtiene el resgistro especifico</param>
        /// <param name="connectionString">La cadena de conexion a base de datos</param>
        /// <returns>Devuelve la gestion, si es correcto, el registro buscado, si es incorrecto, el mensaje de error correspondiente</returns>
        public Gestion GetDatoEnTablaPorIdGestor(string tablaBuscar, int id, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {
                if (EsNombreValido(tablaBuscar))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        if (ExisteTabla(tablaBuscar, connectionString))
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
                            }


                        }
                        else
                        {
                            gestion.setError($"Error: No existe la tabla {tablaBuscar}");
                        }
                    }
                }
                else
                {
                    gestion.setError("Error: El nombre de la tabla " + tablaBuscar + " no es válido");
                }
            }

            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return gestion;
        }





        public Gestion AñadirColumnaBasicaATablaGestor(string nombreTabla, ColumnaInsert columnaAñadir, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {
                if (ExisteTabla(nombreTabla, connectionString))
                {
                    if (!ExisteColumna(nombreTabla, columnaAñadir.Nombre, connectionString))
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = $"ALTER TABLE {nombreTabla} ADD {columnaAñadir.Nombre} {columnaAñadir.TipoDato}";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                            if (ExisteColumna(nombreTabla, columnaAñadir.Nombre, connectionString))
                            {
                                gestion.Correct("Columna añadida correctamente");
                            }
                            else
                            {
                                gestion.setError("Error: No se pudo crear la columna.");
                            }
                        }
                    }
                    else
                    {
                        gestion.setError("Error: Ya existe una columna con nombre " + columnaAñadir.Nombre);
                    }
                }
                else
                {
                    gestion.setError("Error: No existe la tabla con nombre " + nombreTabla);
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return gestion;
        }


        /// <summary>
        /// Comprueba que la tabla especificada existe y tambien evita inyeccion SQL
        /// </summary>
        /// <param name="tabla">Nombre de la tabla que quieres comprobar que existe</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devuelve true si existe la tabla, de lo contrario, false</returns>
        public bool ExisteTabla(string tabla, string connectionString)
        {
            if (!EsNombreValido(tabla))
            {
                return false;
            }
            bool existe = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tabla";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tabla", tabla);
                    int count = (int)command.ExecuteScalar();
                    if (count > 0)
                    {
                        existe = true;
                    }
                }
            }

            return existe;
        }
        /// <summary>
        /// Comprueba que la columna especificada existe dentro de la tabla especificada y tambien evita inyeccion SQL
        /// </summary>
        /// <param name="tabla">Nombre de la tabla en la que buscar la columna</param>
        /// <param name="columna">Nombre de la columna a buscar</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devuelve true si existe la columna, de lo contrario, false</returns>
        public bool ExisteColumna(string tabla, string columna, string connectionString)
        {
            if (!EsNombreValido(columna))
            {
                return false;
            }
            bool existe = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tabla AND COLUMN_NAME = @columna";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tabla", tabla);
                    command.Parameters.AddWithValue("@columna", columna);
                    int columnCount = (int)command.ExecuteScalar();
                    if (columnCount > 0)
                    {
                        existe = true;
                    }
                }
            }
            return existe;
        }
        public bool ExisteValor(string tabla, string columna, string valor, string connectionString)
        {
            bool existe = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT COUNT(*) FROM {tabla} WHERE {columna} = @valor";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@valor", valor);
                    int count = (int)command.ExecuteScalar();
                    if (count > 0)
                    {
                        existe = true;
                    }
                }
            }
            return existe;
        }
        /// <summary>
        /// Comprueba que el tipo de dato sea correcto en esa columna
        /// </summary>
        /// <param name="tabla">Tabla en la que buscar la columna</param>
        /// <param name="columna">Columna donde buscar si el tipo de dato coincide</param>
        /// <param name="valorComprobar">Valor que comprobar que el tipo de dato coincide con el de la columna en la base de datos</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devuelve true si el tipo de dato del valor a añadir coincide con el tipo de dato de la columna, de lo contrario devuelve false</returns>
        public bool TipoDatoCorrecto(string tabla, string columna, string valorComprobar, string connectionString)
        {
            bool tipoDatoCorrecto = false;

            if (string.IsNullOrEmpty(valorComprobar))
                return tipoDatoCorrecto; 

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tabla AND COLUMN_NAME = @columna";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tabla", tabla);
                    command.Parameters.AddWithValue("@columna", columna);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        string tipoDatoSQL = result.ToString().ToLower();

                        tipoDatoCorrecto = ValidarTipoDato(tipoDatoSQL, valorComprobar);
                    }
                }
            }
            return tipoDatoCorrecto;
        }
        private bool ValidarTipoDato(string tipoDatoSQL, string valorComprobar)
        {
            switch (tipoDatoSQL)
            {
                case "int":
                case "bigint":
                case "smallint":
                case "tinyint":
                    return int.TryParse(valorComprobar, out _);

                case "decimal":
                case "numeric":
                case "float":
                case "real":
                    return double.TryParse(valorComprobar, out _);

                case "bit":
                    return valorComprobar == "0" || valorComprobar == "1";

                case "date":
                case "datetime":
                case "smalldatetime":
                case "datetime2":
                    return DateTime.TryParse(valorComprobar, out _);

                case "char":
                case "varchar":
                case "text":
                case "nvarchar":
                case "nchar":
                case "ntext":
                    return true;

                default:
                    return false;
            }
        }
        /// <summary>
        /// Metodo para comprobar si esa columna es primary key
        /// </summary>
        /// <param name="tabla">Nombre de la tabla en la que comprobar</param>
        /// <param name="columna">Nombre de la columna para comrpobar si es primary key</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devuelve true si la columna especifica es primary key, de lo contrario devuelve false</returns>
        public bool EsPrimaryKey(string tabla,string columna, string connectionString) 
        {
            bool esPrimaryKey = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT COUNT(*) 
            FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
            WHERE TABLE_NAME = @tabla 
              AND COLUMN_NAME = @columna 
              AND CONSTRAINT_NAME = (
                  SELECT CONSTRAINT_NAME
                  FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                  WHERE TABLE_NAME = @tabla
                    AND CONSTRAINT_TYPE = 'PRIMARY KEY'
              );
        ";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tabla", tabla);
                    command.Parameters.AddWithValue("@columna", columna);
                    int count = (int)command.ExecuteScalar();
                    if (count >0)
                    {
                        esPrimaryKey = true;
                    }
                }
            }
            return esPrimaryKey;
        }
        /// <summary>
        /// Metodo para comprobar si una tabla ya tiene primary key
        /// </summary>
        /// <param name="tabla">Nombre de la tabla donde buscar si hay primary key</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devuelve true si la tabla tiene primary key, en caso contrario devuelve false</returns>
        public bool TienePrimaryKey(string tabla, string connectionString)
        {
            bool tienePrimaryKey = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE TABLE_NAME = @tabla AND CONSTRAINT_TYPE = 'PRIMARY KEY';";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tabla", tabla);
                    int count = (int)command.ExecuteScalar(); 
                    if (count > 0)
                    {
                        tienePrimaryKey = true;
                    }
                }
            }
            return tienePrimaryKey;
        }
        /// <summary>
        /// Metodo que devuelve las claves foraneas para poder eliminarlas correctamente de la tabla antes de eliminar la columna
        /// </summary>
        /// <param name="tabla">Nombre de la tabla donde buscar si hay claves foraneas</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devuelve una lista con las claves foraneas en esa tabla, ya sea 0, 1 o 50</returns>
        public List<string> NombreClaveForanea(string tabla, string connectionString)
        {
            List<string> foreign = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE TABLE_NAME = @tabla AND CONSTRAINT_TYPE = 'FOREIGN KEY';";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tabla", tabla);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foreign.Add(reader.GetString(0));
                    }
                }
            }

            return foreign;
        }
        /// <summary>
        /// Metodo que devuelve el string con el tipo de dato completo, para poder comprobar que la columna con clave foranea son del mismo tipo de dato
        /// </summary>
        /// <param name="tabla">Tabla donde se cosulta la columna</param>
        /// <param name="columna">Columna donde consultar el tipo de dato y longitud</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devuelve un string con el tipo de dato exacto y longitud si tiene</returns>
        public string TipoDato(string tabla, string columna, string connectionString)
        {
            string tipoCompleto = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tabla AND COLUMN_NAME = @columna";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tabla", tabla);
                    command.Parameters.AddWithValue("@columna", columna);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string tipo = reader.GetString(0).ToUpper();
                            int? longitud = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1);

                            string resultado = tipo;

                            if (longitud.HasValue)
                            {
                                tipoCompleto = $"{tipo}({longitud.Value})";
                            }
                            else
                            {
                                tipoCompleto = tipo;
                            }
                        }
                    }
                }
            }
            return tipoCompleto;
        }
        /// <summary>
        /// Valida que el nombre no contenga caracteres especiales para evitar la inyeccion SQL
        /// </summary>
        /// <param name="nombreValido">Nombre para validar</param>
        /// <returns>Devuelve true si es nombre es válido, si no, false</returns>
        public bool EsNombreValido(string nombreValido)
        {
            string[] caracteresNoPermitidos = { "--", ";", "'", "\"", "/*", "*/" };

            foreach (var caracter in caracteresNoPermitidos)
            {
                if (nombreValido.Contains(caracter))
                {
                    return false;
                }
            }
            if (nombreValido.Any(c => !char.IsLetterOrDigit(c) && c != '_'))
            {
                return false;
            }
            return true;
        }
    }
}
