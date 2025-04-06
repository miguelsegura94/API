using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBDD.Modelos;
using GestorBaseDatos.GestionCarpeta;
using Microsoft.Data.SqlClient;

namespace GestorBaseDatos.GestorBD.GestorBD
{
    public partial class GestorBD
    {
        //AQUI ESTAN EL CREAR Y ELIMINAR TABLA



        /// <summary>
        /// Metodo para crear una nueva tabla asjdasd
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
                    gestion.setError("Error: El nombre de la tabla " + tablaEliminar + " no es válido");
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
