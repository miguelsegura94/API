using System.Data;
using System.Security.Principal;
using System.Text;
using BBDD.Modelos;
using GestorBaseDatos.GestionCarpeta;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GestorBaseDatos
{
    //TODO HACER UN GET COLUMNAS DE UNA TABLA


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
        public Gestion GetListaCompletaRegistrosGestor(string tablaBuscar, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (ExisteTabla(tablaBuscar, connectionString))
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
            }
            catch (Exception ex)
            {
                gestion.setError($"Error inesperado en la base de datos.{ex.Message}");
            }
            return gestion;
        }
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
            }
            catch (Exception ex)
            {
                gestion.setError($"Error inesperado en la base de datos.{ex.Message}");
            }
            return gestion;
        }
        //TODO HACER METODO QUE COMPRUEBE QUE EL TIPO DE DATO ES CORRECTO
        public Gestion CrearTablaGestor(TablaBD tabla, string connectionString)
        {
            //TODO que sea posible crear un tabla sin columnas
            Gestion gestion = new Gestion();
            StringBuilder sb = new StringBuilder();
            List<string> foreignKeys = new List<string>();
            try
            {
                if (!ExisteTabla(tabla.Nombre, connectionString))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        sb.Append($"CREATE TABLE {tabla.Nombre}(");
                        for (int i = 0; i < tabla.Columnas.Count; i++)
                        {
                            var columna = tabla.Columnas[i];
                            if (!ExisteColumna(tabla.Nombre, columna.Nombre, connectionString))
                            {
                                sb.Append($"{columna.Nombre} {columna.TipoDato}");
                                if (columna.Longitud > 0)
                                {
                                    sb.Append($"({columna.Longitud})");
                                }
                                if (columna.PrimaryKey)
                                {
                                    sb.Append(" NOT NULL ");
                                    sb.Append($" PRIMARY KEY ");
                                }
                                else if (!columna.Null)
                                {
                                    sb.Append(" NOT NULL ");
                                }
                                if (i < tabla.Columnas.Count - 1)
                                {
                                    sb.Append(",");
                                }
                                if (columna.ForeignKey != null && !string.IsNullOrEmpty(columna.ForeignKey.Nombre))
                                {
                                    if (ExisteTabla(columna.ForeignKey.TablaOrigen, connectionString))
                                    {
                                        if (ExisteColumna(columna.ForeignKey.TablaOrigen, columna.ForeignKey.Nombre, connectionString))
                                        {
                                            foreignKeys.Add($"FOREIGN KEY ({columna.ForeignKey.Nombre}) REFERENCES {columna.ForeignKey.TablaOrigen}({columna.ForeignKey.Nombre})");
                                        }
                                        else
                                        {
                                            gestion.setError($"Error: La columna {columna.ForeignKey.Nombre} no existe en la tabla {columna.ForeignKey.TablaOrigen}.");
                                        }
                                    }
                                    else
                                    {
                                        gestion.setError($"Error: La tabla {columna.ForeignKey.TablaOrigen} no existe.");
                                    }
                                }
                            }
                            else
                            {
                                gestion.setError($"Error: La columna {columna.Nombre} ya existe.");
                            }
                        }
                        if (foreignKeys.Count > 0)
                        {
                            sb.Append(",");
                            sb.Append(string.Join(",", foreignKeys));
                        }
                        sb.Append(");");
                        string query = sb.ToString();
                        SqlCommand command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        if (ExisteTabla(tabla.Nombre, connectionString))
                        {
                            gestion.Correct("Tabla creada correctamente");
                        }
                        else
                        {
                            gestion.setError("Error: No se pudo crear la tabla.");
                        }

                    }
                }
                else
                {
                    gestion.setError("Error: Ya existe una tabla con el nombre " + tabla.Nombre);
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
        /// <summary>
        /// Añade una columna a una tabla especifica
        /// </summary>
        /// <param name="nombreTabla">Nombre de la tabla a la que se va a añadir la columna</param>
        /// <param name="columnaAñadir">Columna que se va a añadir a la tabla especificada, con todas las caracteristicas necesarias</param>
        /// <param name="connectionString">La cadena de conexion a base de datos</param>
        /// <returns>Devuelve la gestion, si es correcto devuelve un ok, si no devuelve el mensaje de error correspondiente</returns>
        public Gestion AñadirColumnaCompletaATablaGestor(string nombreTabla, Columna columnaAñadir, string connectionString)
        {
            Gestion gestion = new Gestion();
            StringBuilder sb = new StringBuilder();
            try
            {
                if (ExisteTabla(nombreTabla, connectionString))
                {
                    if (!ExisteColumna(nombreTabla, columnaAñadir.Nombre, connectionString))
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            sb.Append($"ALTER TABLE {nombreTabla} ADD ");
                            sb.Append($"{columnaAñadir.Nombre} {columnaAñadir.TipoDato}");
                            if (columnaAñadir.Longitud > 0)
                            {
                                sb.Append($"({columnaAñadir.Longitud})");
                            }
                            if (columnaAñadir.PrimaryKey)
                            {
                                sb.Append(" NOT NULL ");
                                sb.Append($" PRIMARY KEY ");
                            }
                            else if (!columnaAñadir.Null)
                            {
                                sb.Append(" NOT NULL ");
                            }
                            if (columnaAñadir.ForeignKey != null && !string.IsNullOrEmpty(columnaAñadir.ForeignKey.Nombre))
                            {
                                if (ExisteTabla(columnaAñadir.ForeignKey.TablaOrigen, connectionString))
                                {
                                    if (ExisteColumna(columnaAñadir.ForeignKey.TablaOrigen, columnaAñadir.ForeignKey.Nombre, connectionString))
                                    {
                                        sb.Append($"FOREIGN KEY ({columnaAñadir.ForeignKey.Nombre}) REFERENCES {columnaAñadir.ForeignKey.TablaOrigen}({columnaAñadir.ForeignKey.Nombre})");
                                    }
                                    else
                                    {
                                        gestion.setError($"Error: La columna {columnaAñadir.ForeignKey.Nombre} no existe en la tabla {columnaAñadir.ForeignKey.TablaOrigen}.");
                                        return gestion;
                                    }
                                }
                                else
                                {
                                    gestion.setError($"Error: La tabla {columnaAñadir.ForeignKey.TablaOrigen} no existe.");
                                    return gestion;
                                }
                            }
                            string query = sb.ToString();
                            Console.WriteLine(query);
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
                        gestion.setError("Error: Ya existe la columna con nombre " + columnaAñadir.Nombre);
                    }
                }
                else
                {
                    gestion.setError("Error: No existe la tabla con nombre " + nombreTabla);
                }
            }
            catch (SqlException ex)
            {
                gestion.setError($"Error de base de datos.{ex.Message}");
            }
            catch (FormatException ex)
            {
                gestion.setError($"Error de solicitud erronea.{ex.Message}");
            }
            catch (TimeoutException ex)
            {
                gestion.setError($"Tiempo de espera agotado en la base de datos.{ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                gestion.setError("Error de conexión a la base de datos: " + ex.Message);
            }
            catch (Exception ex)
            {
                gestion.setError($"Error inesperado en la base de datos.{ex.Message}");
            }
            return gestion;
        }
        public Gestion EliminarTablaGestor(string tablaEliminar, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {
                if (ExisteTabla(tablaEliminar, connectionString))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DROP TABLE " + tablaEliminar;
                        SqlCommand command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        if (!ExisteTabla(tablaEliminar, connectionString))
                        {
                            gestion.Correct("Tabla eliminada correctamente");
                        }
                        else
                        {
                            gestion.setError("Error: No se pudo eliminar la tabla.");
                        }
                    }
                }
                else
                {
                    gestion.setError("Error: No existe la tabla con nombre " + tablaEliminar);
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
            }
            catch (Exception ex)
            {
                gestion.setError($"Error inesperado en la base de datos.{ex.Message}");
            }
            return gestion;
        }
        /// <summary>
        /// Obtiene todas las columnas de una tabla especifica
        /// </summary>
        /// <param name="tablaBuscar">Tabla en la que se buscan las columnas</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Si es correcto:Devuelve la gestion con cada columna y sus especificaciones. Si es incorrecto: devulve mensaje de error correspondiente</returns>
        public Gestion GetColumnasTablaGestor(string tablaBuscar, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {
                if (ExisteTabla(tablaBuscar, connectionString))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = $"SELECT c.COLUMN_NAME, c.DATA_TYPE, c.CHARACTER_MAXIMUM_LENGTH, c.IS_NULLABLE, CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 'YES' ELSE 'NO' END AS IsPrimaryKey, fk.REFERENCED_TABLE_NAME AS ForeignTable, fk.REFERENCED_COLUMN_NAME AS ForeignColumn FROM INFORMATION_SCHEMA.COLUMNS c LEFT JOIN (SELECT ku.TABLE_NAME, ku.COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc ON ku.CONSTRAINT_NAME = tc.CONSTRAINT_NAME WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY') pk ON c.TABLE_NAME = pk.TABLE_NAME AND c.COLUMN_NAME = pk.COLUMN_NAME LEFT JOIN (SELECT kcu.TABLE_NAME, kcu.COLUMN_NAME, ccu.TABLE_NAME AS REFERENCED_TABLE_NAME, ccu.COLUMN_NAME AS REFERENCED_COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc ON kcu.CONSTRAINT_NAME = rc.CONSTRAINT_NAME JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu ON rc.UNIQUE_CONSTRAINT_NAME = ccu.CONSTRAINT_NAME) fk ON c.TABLE_NAME = fk.TABLE_NAME AND c.COLUMN_NAME = fk.COLUMN_NAME WHERE c.TABLE_NAME = @tablaBuscar";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@tablaBuscar", tablaBuscar);
                        command.ExecuteNonQuery();
                        SqlDataReader reader = command.ExecuteReader();
                        gestion.data = new List<dynamic>();
                        while (reader.Read())
                        {
                            Dictionary<string, dynamic> columnas = new Dictionary<string, dynamic>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columnas[reader.GetName(i)] = reader.GetValue(i);
                            }

                            gestion.data.Add(columnas);

                        }
                    }
                }
                else
                {
                    gestion.setError("Error: No existe la tabla con nombre " + tablaBuscar);
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

            }
            catch (Exception ex)
            {
                gestion.setError($"Error inesperado en la base de datos.{ex.Message}");
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
            }
            catch (Exception ex)
            {
                gestion.setError($"Error inesperado en la base de datos.{ex.Message}");
            }
            return gestion;
        }
        public Gestion EliminarColumnaTablaGestor(string nombreTabla, ColumnaDelete columnaEliminar, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {
                if (ExisteTabla(nombreTabla, connectionString))
                {
                    if (ExisteColumna(nombreTabla, columnaEliminar.Nombre, connectionString))
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = $"ALTER TABLE {nombreTabla} DROP COLUMN {columnaEliminar.Nombre}";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                            if (!ExisteColumna(nombreTabla, columnaEliminar.Nombre, connectionString))
                            {
                                gestion.Correct("Columna eliminada correctamente");
                            }
                            else
                            {
                                gestion.setError("Error: No se pudo eliminar la columna.");
                            }
                        }
                    }
                    else
                    {
                        gestion.setError("Error: No existe una columna con nombre " + columnaEliminar.Nombre);
                    }
                }
                else
                {
                    gestion.setError("Error: No existe la tabla con nombre " + nombreTabla);
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
                    if (count > 0)
                    {
                        existe = true;
                    }
                }
            }

            return existe;
        }
        public bool ExisteColumna(string tabla, string columna, string connectionString)
        {
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

        //TODO PEKE QUE DEBO DE COMPROBAR ADEMAS DE QUE LA COLUMNA Y TABLA EXISTEN
        //TODO PEKE COMO MEJORAR LOS METODOS QUE USAN FOREIGN KEY AL NO PASARLE DATOS CORRECTOS(COMO HACER PARA QUE NO SEA OBLIGATORIO PASARLE DATOS DE FOREIGN KEY)
    }
}
