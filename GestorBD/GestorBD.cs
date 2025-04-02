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

    //TODO HACER EL METODO QUE BUSQUE UN REGISTRO EN UNA TABLA POR UN PARAMETRO GENERICO

    //TODO HACER UN METODO ELIMINAR REGISTRO POR IDENTITY Y TAMBIEN EDITAR REGISTRO

    //TODOO HACER DOCUMENTACION SUMMARY DE LOS METODOS EN GESTOR BD Y EN CONTROLLER
    //BAJAR EL POSTMAN Y CREAR COLECCION QUE SE LLAME GESTION BD

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
                if (EsNombreValido(tablaBuscar))
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
        /// <summary>
        /// Obtener el json para poder utilizarlo para añadir registros desde otro metodo
        /// </summary>
        /// <param name="tablaBuscar">Tabla en la que buscar los parámetros</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devuelve el json junto con el mensaje correspondiente si es correcto o no</returns>
        public Gestion ObtenerJsonParaRegistroEnTablaGestor(string tablaBuscar, string connectionString)
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
                            string query = @"
                SELECT COLUMN_NAME, DATA_TYPE 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = @tableName AND COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 0";

                            Dictionary<string, object> jsonDict = new Dictionary<string, object>();

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@tableName", tablaBuscar);
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string columnName = reader.GetString(0);
                                        string dataType = reader.GetString(1);

                                        object ejemploValor = dataType switch
                                        {
                                            "int" => 0,
                                            "bigint" => 0,
                                            "decimal" => 0.0,
                                            "float" => 0.0,
                                            "bit" => false,
                                            "nvarchar" => "",
                                            "varchar" => "",
                                            "text" => "",
                                            "date" => "2024-01-01",
                                            "datetime" => "2024-01-01T00:00:00",
                                            _ => null
                                        };

                                        jsonDict[columnName] = ejemploValor;
                                    }
                                }
                            }
                            gestion.data = jsonDict;
                            gestion.Correct($"Aqui tienes el json para entrada de datos en tabla {tablaBuscar}");
                            Console.WriteLine(gestion.data);

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
        /// <summary>
        /// Crea el resgistro especifico en la tabla a partir del json obtenido en el metodo de obtencion de json
        /// </summary>
        /// <param name="tablaBuscar">Tabla en la que añadir el nuevo resgistro</param>
        /// <param name="datosAñadir">Todos los datos necesarios para añadir un nuevo registro</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devuelve el mensaje correspondiente si es correcto o el error si hay algun problema</returns>
        public Gestion CrearRegistroEnTablaFrombodyGestor(string tablaBuscar, Dictionary<string, object> datosAñadir, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (EsNombreValido(tablaBuscar))
                    {
                        if (ExisteTabla(tablaBuscar, connectionString))
                        {
                            connection.Open();
                            string columnasQuery = @"
                SELECT COLUMN_NAME, COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, 'IsIdentity') AS IsIdentity 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = @tablaName";

                            List<string> columnas = new List<string>();

                            using (SqlCommand command = new SqlCommand(columnasQuery, connection))
                            {
                                command.Parameters.AddWithValue("@tablaName", tablaBuscar);
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string columna = reader.GetString(0);
                                        bool isIdentity = reader.GetInt32(1) == 1;
                                        if (!isIdentity)
                                        {
                                            columnas.Add(columna);
                                        }
                                    }
                                }
                            }

                            if (datosAñadir.Count != columnas.Count)
                            {
                                gestion.setError("Error: La cantidad de valores no coincide con la cantidad de columnas (excluyendo IDENTITY).");
                                return gestion;
                            }

                            string columnasString = string.Join(", ", columnas);
                            string valoresString = string.Join(", ", columnas.Select(c => $"@{c}"));

                            string insertQuery = $"INSERT INTO {tablaBuscar} ({columnasString}) VALUES ({valoresString})";

                            using (SqlCommand command = new SqlCommand(insertQuery, connection))
                            {
                                foreach (var kvp in datosAñadir)
                                {
                                    command.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value ?? DBNull.Value);
                                }

                                command.ExecuteNonQuery();
                                gestion.Correct("Datos insertados correctamente.");
                            }
                        }
                        else
                        {
                            gestion.setError($"Error: No existe la tabla {tablaBuscar}");
                        }
                    }
                    else
                    {
                        gestion.setError("Error: El nombre de la tabla " + tablaBuscar + " no es válido");
                    }
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return gestion;
        }
        /*
        public Gestion CrearRegistroEnTablaGestor(string tablaBuscar,string datosAñadir, string connectionString)
        {
            //primero saber en que tabla añadir dato
            // comprobar si la tabla existe
            // una vez se en que tabla añadir, devolver el cuestionario(el modelo a rellenar) con los datos necesarios para añadir algo a esa tabla especifica
            //el dato especifico que añadir
            //comprobar que no exista ese dato en esa tabla
            //añadir dato

            Gestion gestion = new Gestion();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (ExisteTabla(tablaBuscar, connectionString))
                    {
                        string columnasQuery = $@"
            SELECT COLUMN_NAME, COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, 'IsIdentity') AS IsIdentity 
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = @tablaName";

                        List<string> columnas = new List<string>();
                        List<string> valores = datosAñadir.Split(',').ToList();
                        using (SqlCommand command = new SqlCommand(columnasQuery, connection))
                        {
                            command.Parameters.AddWithValue("@tablaName", tablaBuscar);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string columna = reader.GetString(0);
                                    bool isIdentity = reader.GetInt32(1) == 1;
                                    if (!isIdentity)
                                    {
                                        columnas.Add(columna);
                                    }
                                }
                            }
                        }
                        if (valores.Count != columnas.Count)
                        {
                            gestion.setError("Error: La cantidad de valores no coincide con la cantidad de columnas (excluyendo IDENTITY).");
                            return gestion;
                        }
                        string columnasString = string.Join(", ", columnas);
                        List<string> valoresFormateados = new List<string>();
                        foreach (var valor in valores)
                        {
                            if (int.TryParse(valor, out _))
                            {
                                valoresFormateados.Add(valor);
                            }
                            else
                            {
                                
                                valoresFormateados.Add($"'{valor.Replace("'", "''")}'");
                            }
                        }

                        string valoresString = string.Join(", ", valoresFormateados);
                        string insertQuery = $"INSERT INTO {tablaBuscar} ({columnasString}) VALUES ({valoresString})";
                        using (SqlCommand command = new SqlCommand(insertQuery, connection))
                        {
                            command.ExecuteNonQuery();
                            gestion.Correct("Datos insertados correctamente.");
                        } 
                    }
                    else
                    {
                        gestion.setError($"Error: No existe la tabla {tablaBuscar}");
                    }
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return gestion;
        }*/
        //TODO HACER METODO QUE COMPRUEBE QUE EL TIPO DE DATO ES CORRECTO CON ENUM
        /// <summary>
        /// Metodo para crear una nueva tabla
        /// </summary>
        /// <param name="tabla">Todos los parametros necesarios para crear la nueva tabla</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devuelve el mensaje correspondiente si ha podido crear la tabla o el error especificamente</returns>
        public Gestion CrearTablaGestor(TablaBD tabla, string connectionString)
        {
            Gestion gestion = new Gestion();
            StringBuilder sb = new StringBuilder();
            List<string> foreignKeys = new List<string>();
            try
            {
                if (EsNombreValido(tabla.Nombre))
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
                                    string tipoSQL = columna.Tipo.ObtenerTipoSQL(columna.Longitud);
                                    sb.Append($"{columna.Nombre} {tipoSQL}");
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
                                    if (columna.ForeignKey != null && !string.IsNullOrEmpty(columna.ForeignKey.NombreColumna))
                                    {
                                        if (ExisteTabla(columna.ForeignKey.TablaOrigen, connectionString))
                                        {
                                            //TODO COMPROBAR QUE LA FOREIGN KEY Y LA REFERENCIA SEAN DEL MISMO TIPO DE DATO Y LONGITUD SI TIENE( QUE SEAN COMPATIBLES)
                                            if (ExisteColumna(columna.ForeignKey.TablaOrigen, columna.ForeignKey.NombreColumna, connectionString))
                                            {
                                                foreignKeys.Add($"FOREIGN KEY ({columna.ForeignKey.NombreColumna}) REFERENCES {columna.ForeignKey.TablaOrigen}({columna.ForeignKey.NombreColumna})");
                                            }
                                            else
                                            {
                                                gestion.setError($"Error de foreign key: La columna {columna.ForeignKey.NombreColumna} no existe en la tabla {columna.ForeignKey.TablaOrigen}.");
                                            }
                                        }
                                        else
                                        {
                                            gestion.setError($"Error de foreign key: La tabla {columna.ForeignKey.TablaOrigen} no existe.");
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
                else
                {
                    gestion.setError("Error: El nombre de la tabla " + tabla.Nombre + " no es válido");
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
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
                if (EsNombreValido(nombreTabla)) 
                {
                    if (ExisteTabla(nombreTabla, connectionString))
                    {
                        if (EsNombreValido(columnaAñadir.Nombre))
                        {
                            if (!ExisteColumna(nombreTabla, columnaAñadir.Nombre, connectionString))
                            {
                                using (SqlConnection connection = new SqlConnection(connectionString))
                                {
                                    connection.Open();
                                    sb.Append($"ALTER TABLE {nombreTabla} ADD ");
                                    sb.Append($"{columnaAñadir.Nombre} {columnaAñadir.Tipo}");
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
                                    if (columnaAñadir.ForeignKey != null && !string.IsNullOrEmpty(columnaAñadir.ForeignKey.NombreColumna))
                                    {
                                        if (ExisteTabla(columnaAñadir.ForeignKey.TablaOrigen, connectionString))
                                        {
                                            if (ExisteColumna(columnaAñadir.ForeignKey.TablaOrigen, columnaAñadir.ForeignKey.NombreColumna, connectionString))
                                            {
                                                sb.Append($"FOREIGN KEY ({columnaAñadir.ForeignKey.NombreColumna}) REFERENCES {columnaAñadir.ForeignKey.TablaOrigen}({columnaAñadir.ForeignKey.NombreColumna})");
                                            }
                                            else
                                            {
                                                gestion.setError($"Error: La columna {columnaAñadir.ForeignKey.NombreColumna} no existe en la tabla {columnaAñadir.ForeignKey.TablaOrigen}.");
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
                            gestion.setError("Error: El nombre de la Columna " + columnaAñadir.Nombre + " no es válido");
                        }
                    }
                    else
                    {
                        gestion.setError("Error: No existe la tabla con nombre " + nombreTabla);
                    }
                }
                else
                {
                    gestion.setError("Error: El nombre de la tabla " + nombreTabla + " no es válido");
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return gestion;
        }
        /// <summary>
        /// Elimina la tabla especificada
        /// </summary>
        /// <param name="tablaEliminar">Nombre de la tabla que eliminar</param>
        /// <param name="connectionString">La cadena de conexion a base de datos</param>
        /// <returns>Devuelve la gestion con el mensaje correspondiente sea correcta o no</returns>
        public Gestion EliminarTablaGestor(string tablaEliminar, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {
                if (EsNombreValido(tablaEliminar))
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
                else
                {
                    gestion.setError("Error: El nombre de la tabla " + tablaEliminar +" no es válido");
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
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
                if (EsNombreValido(tablaBuscar))
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
        /// Elimina la columna especifica de la tabla especifica
        /// </summary>
        /// <param name="nombreTabla">Nombre de la tabla en la que quieres eliminar la columna</param>
        /// <param name="columnaEliminar">Nombre de la columna que quieres eliminar</param>
        /// <param name="connectionString">La cadena de conexion a base de datos</param>
        /// <returns>Devuelve la gestion, correcta o incorrecta con su mensaje correspondiente si ha podido eliminar o no la columna</returns>
        public Gestion EliminarColumnaTablaGestor(string nombreTabla, ColumnaDelete columnaEliminar, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {
                if (EsNombreValido(nombreTabla))
                {
                    if (ExisteTabla(nombreTabla, connectionString))
                    {
                        if (EsNombreValido(columnaEliminar.Nombre))
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
                            gestion.setError("Error: El nombre de la Columna " + columnaEliminar.Nombre + " no es válido");
                        }
                    }
                    else
                    {
                        gestion.setError("Error: No existe la tabla con nombre " + nombreTabla);
                    }
                }
                else
                {
                    gestion.setError("Error: El nombre de la tabla " + nombreTabla + " no es válido");
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return gestion;
        }

        /// <summary>
        /// Comprueba que la tabla especificada existe
        /// </summary>
        /// <param name="tabla">Nombre de la tabla que quieres comprobar que existe</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devuelve true si existe la tabla, de lo contrario, false</returns>
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
        /// <summary>
        /// Comprueba que la columna especificada existe dentro de la tabla especificada
        /// </summary>
        /// <param name="tabla">Nombre de la tabla en la que buscar la columna</param>
        /// <param name="columna">Nombre de la columna a buscar</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devuelve true si existe la columna, de lo contrario, false</returns>
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
        /// <summary>
        /// Valida que el nombre no contenga caracteres especiales para evitar la inyeccion SQL
        /// </summary>
        /// <param name="nombreValido">Nombre para validar</param>
        /// <returns>Devuelve true si es nombre es válido, si no, false</returns>
        private bool EsNombreValido(string nombreValido)
        {
            string[] caracteresNoPermitidos = { "--", ";", "'", "\"", "/*", "*/" };

            foreach (var caracter in caracteresNoPermitidos)
            {
                if (nombreValido.Contains(caracter))
                {
                    return false;
                }
            }
            if (nombreValido.Any(c => !Char.IsLetterOrDigit(c) && c != '_'))
            {
                return false;
            }
            return true;
        }
    }
}
