using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBDD.Modelos;
using GestorBaseDatos.GestionCarpeta;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;

namespace GestorBaseDatos.GestorBD.GestorBD
{
    public partial class GestorBD
    {
        //AQUI ESTAN EL AÑADIR COLUMNA EL GET COLUMNA Y EL ELIMINAR COLUMNA

        /// <summary>
        /// Añade una columna a una tabla especifica
        /// </summary>
        /// <param name="nombreTabla">Nombre de la tabla a la que se va a añadir la columna</param>
        /// <param name="columnaAñadir">Columna que se va a añadir a la tabla especificada, con todas las caracteristicas necesarias</param>
        /// <param name="connectionString">La cadena de conexion a base de datos</param>
        /// <returns>Devuelve la gestion, si es correcto devuelve un ok, si no devuelve el mensaje de error correspondiente</returns>
        public Gestion AñadirColumnaCompletaATablaGestor(string nombreTabla, Columna columnaAñadir, string connectionString)
        {
            //TODO arreglar exception already has a primary key defined
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
                                    string tipoSQL = columnaAñadir.Tipo.ObtenerTipoSQL(columnaAñadir.Longitud);
                                    sb.Append($"{columnaAñadir.Nombre} {tipoSQL}");
                                    if (columnaAñadir.PrimaryKey)
                                    {
                                        if (TienePrimaryKey(nombreTabla, connectionString))
                                        {
                                            gestion.setError($"Error: La tabla {nombreTabla} ya tiene primary key.");
                                            return gestion;
                                        }
                                        sb.Append(" NOT NULL ");
                                        sb.Append($" PRIMARY KEY ");
                                    }
                                    else if (!columnaAñadir.Null)
                                    {
                                        sb.Append(" NOT NULL ");
                                    }
                                    if (columnaAñadir.ForeignKey != null && !string.IsNullOrEmpty(columnaAñadir.ForeignKey.ColumnaOrigen))
                                    {
                                        if (ExisteTabla(columnaAñadir.ForeignKey.TablaOrigen, connectionString))
                                        {
                                            if (ExisteColumna(columnaAñadir.ForeignKey.TablaOrigen, columnaAñadir.ForeignKey.ColumnaOrigen, connectionString))
                                            {
                                                if (!EsPrimaryKey(columnaAñadir.ForeignKey.TablaOrigen, columnaAñadir.ForeignKey.ColumnaOrigen, connectionString))
                                                {
                                                    gestion.setError($"Error de foreign key: La columna {columnaAñadir.ForeignKey.ColumnaOrigen} no es primary key en la tabla {columnaAñadir.ForeignKey.TablaOrigen}.");
                                                    return gestion;
                                                }
                                                string foreignTipo = TipoDato(columnaAñadir.ForeignKey.TablaOrigen, columnaAñadir.ForeignKey.ColumnaOrigen,connectionString);
                                                
                                                if (tipoSQL== foreignTipo)
                                                {
                                                    sb.Append($" FOREIGN KEY ({columnaAñadir.Nombre}) REFERENCES {columnaAñadir.ForeignKey.TablaOrigen}({columnaAñadir.ForeignKey.ColumnaOrigen})");
                                                }
                                                else
                                                {
                                                    gestion.setError($"Error: La columna {columnaAñadir.ForeignKey.ColumnaOrigen} no tiene el mismo tipo que la columna que quieres añadir.");
                                                    return gestion;
                                                }
                                                
                                            }
                                            else
                                            {
                                                gestion.setError($"Error: La columna {columnaAñadir.ForeignKey.ColumnaOrigen} no existe en la tabla {columnaAñadir.ForeignKey.TablaOrigen}.");
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
                                    if (EsPrimaryKey(nombreTabla, columnaEliminar.Nombre, connectionString))
                                    {
                                        gestion.setError($"Error: No se puede eliminar la columna {columnaEliminar.Nombre} por ser primary key.");
                                        return gestion;
                                    }
                                    List<string> clavesForaneas = new List<string>();
                                    clavesForaneas = NombreClaveForanea(nombreTabla,connectionString);
                                    if (clavesForaneas.Count > 0)
                                    {
                                        for (int i = 0; i < clavesForaneas.Count; i++)
                                        {
                                            string queryForeign = $"ALTER TABLE {nombreTabla} DROP CONSTRAINT {clavesForaneas[i]};";
                                            SqlCommand commandForeign = new SqlCommand(queryForeign, connection);
                                            commandForeign.ExecuteNonQuery();
                                        }
                                    }
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
    }
}
