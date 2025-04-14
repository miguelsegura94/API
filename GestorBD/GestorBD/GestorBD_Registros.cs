using System;
using System.Collections.Generic;
using System.Drawing;
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
        //AQUI ESTA EL GET TODOS LO REGISTROS, EL GET REGISTRO POR VALOR, ELIMINAR REGISTRO POR VALOR, OBTENER EL JSON PARA INSERTAR UN REGISTRO E INSERTARLO


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
                if (!ExisteTabla(tablaBuscar, connectionString))
                {
                    gestion.setError($"No existe la tabla {tablaBuscar}");
                    return gestion;
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();



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
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return gestion;
        }
        /// <summary>
        /// Obtiene todos los registros de una tabla en los que coincide la columna con el valor
        /// </summary>
        /// <param name="tabla">Tabla de la que se obtienen los registros</param>
        /// <param name="columna">Columna de la tabla donde buscar el valor</param>
        /// <param name="valor">Valor del registro que se esta buscando</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Si es correcto devuelve uno o mas registros donde coinciden los valores en la columna, si es incorrecto devuelve el mensaje de error correspondiente</returns>
        public Gestion GetRegistroEnTablaPorValorGestor(string tabla, string columna, string valor, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {

                if (!ExisteTabla(tabla, connectionString))
                {
                    gestion.setError($"Error: No existe la tabla {tabla}");
                    return gestion;
                }
                if (!ExisteColumna(tabla, columna, connectionString))
                {
                    gestion.setError($"Error: En la tabla {tabla} no existe la columna {columna}");
                    return gestion;
                }
                if (!EsNombreValido(valor))
                {
                    gestion.setError($"Error: El nombre del valor {valor} no es válido");
                    return gestion;
                }
                if (!TipoDatoCorrecto(tabla, columna, valor, connectionString))
                {
                    gestion.setError($"Error: El tipo de dato no concide con la columna {columna}");
                    return gestion;
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"SELECT * FROM [{tabla}] WHERE {columna}=@Valor";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Valor", valor);
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
                    if (((List<dynamic>)gestion.data).Count <= 0)
                    {
                        gestion.setError($"Error: En la tabla {tabla} y la columna {columna} no existe el valor {valor}");
                        return gestion;
                    }
                    gestion.Correct();
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message
    });
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

                if (!ExisteTabla(tablaBuscar, connectionString))
                {
                    gestion.setError($"Error: No existe la tabla {tablaBuscar}");
                    return gestion;
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


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
                if (ExisteTabla(tablaBuscar, connectionString))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {

                        //TODO VALIDAR QUE EL TIPO DE DATO QUE RECIBO ES EL QUE TIENE EL ATRIBUTO

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
                }
                else
                {
                    gestion.setError($"Error: No existe la tabla {tablaBuscar}");
                }

            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return gestion;
        }
        /// <summary>
        /// Crea un registro en la base de datos pasandole el body necesario para esa tabla
        /// </summary>
        /// <param name="tablaBuscar">Tabla donde añadir los registros</param>
        /// <param name="registroAñadir">Registro para añadir, columna y valor, tantas columnas y valores como columnas tenga la tabla</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devulve la gestion con su mensaje correspondiente, si ha podido añadir el registro, o con el mensaje de error </returns>
        public Gestion CrearRegistroEnTablaGestor(string tablaBuscar, List<RegistroInsert> registroAñadir, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {
                //comprobar que el nombre de la columna no se repite en el body
                if (!ExisteTabla(tablaBuscar, connectionString))
                {
                    gestion.setError($"Error: No existe la tabla {tablaBuscar}");
                    return gestion;
                }
                if (registroAñadir.Count <= 0)
                {
                    gestion.setError($"Error: Tienes que añadir algun registro");
                    return gestion;
                }
                for (int i = 0; i < registroAñadir.Count; i++)
                {
                    

                    if (!ExisteColumna(tablaBuscar, registroAñadir[i].NombreColumna, connectionString))
                    {
                        gestion.setError($"Error: En la tabla {tablaBuscar} no existe la columna {registroAñadir[i].NombreColumna}");
                        return gestion;
                    }
                    if (EsAutoincremental(tablaBuscar, registroAñadir[i].NombreColumna, connectionString))
                    {
                        gestion.setError($"Error: No puedes elegir el valor para la columna {registroAñadir[i].NombreColumna}");
                        return gestion;
                    }
                    for (int j = i+1; j < registroAñadir.Count; j++)
                    {
                        if (registroAñadir[i].NombreColumna == registroAñadir[j].NombreColumna)
                        {
                            gestion.setError($"Error: No puedes añadir el nombre {registroAñadir[i].NombreColumna} 2 veces en el body");
                            return gestion;
                        }
                    }
                    if (!EsNombreValido(registroAñadir[i].ValorInsert))
                    {
                        gestion.setError($"Error: El nombre del valor {registroAñadir[i].ValorInsert} no es valido.");
                        return gestion;
                    }
                    if (!TipoDatoCorrecto(tablaBuscar, registroAñadir[i].NombreColumna, registroAñadir[i].ValorInsert, connectionString))
                    {
                        gestion.setError($"Error: El tipo de dato no concide con la columna {registroAñadir[i].NombreColumna}");
                        return gestion;
                    }
                }
                StringBuilder sbc = new StringBuilder();
                StringBuilder sbv = new StringBuilder();
                for (int i = 0; i < registroAñadir.Count; i++)
                {
                    sbc.Append(registroAñadir[i].NombreColumna);
                    sbv.Append($"@Valor{i}");
                    if (i < registroAñadir.Count - 1)
                    {
                        sbc.Append(", ");
                        sbv.Append(", ");
                    }
                }
                string columnas = sbc.ToString();
                string valores = sbv.ToString();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"INSERT INTO {tablaBuscar} ({columnas}) VALUES ({valores})";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        for (int i = 0; i < registroAñadir.Count; i++)
                        {
                            command.Parameters.AddWithValue($"@valor{i}", registroAñadir[i].ValorInsert);
                        }
                        int añadido = command.ExecuteNonQuery();
                        if (añadido > 0)
                        {
                            gestion.Correct("Registro añadido correctamente.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return gestion;
        }
        /// <summary>
        /// Elimina el registro especificado de la tabla especificada
        /// </summary>
        /// <param name="tablaBuscar">Tabla de la que se va a eliminar el registro</param>
        /// <param name="registro">Columna y valor del registro que quieres eliminar</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devuelve mensaje ya sea correcto o de error con su error correspondiente</returns>
        public Gestion EliminarRegistroEnTablaGestor(string tablaBuscar, RegistroDelete registro, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {

                if (!ExisteTabla(tablaBuscar, connectionString))
                {
                    gestion.setError($"Error: No existe la tabla {tablaBuscar}");
                    return gestion;
                }
                if (!ExisteColumna(tablaBuscar, registro.Columna, connectionString))
                {
                    gestion.setError($"Error: En la tabla {tablaBuscar} no existe la columna {registro.Columna}");
                    return gestion;
                }
                if (!TipoDatoCorrecto(tablaBuscar, registro.Columna, registro.Valor, connectionString))
                {
                    gestion.setError($"Error: El tipo de dato no concide con la columna {registro.Columna}");
                    return gestion;
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"DELETE FROM {tablaBuscar} WHERE {registro.Columna} = @Valor";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Valor", registro.Valor);
                        int borrado = command.ExecuteNonQuery();
                        if (borrado > 0)
                        {
                            gestion.Correct("Registro eliminado correctamente.");
                        }
                        else
                        {
                            gestion.setError($"Error: No existe el registro {registro.Valor} en la columna {registro.Columna}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return gestion;
        }
        /// <summary>
        /// Edita uno o varios registros a la vez en una tabla especifica
        /// </summary>
        /// <param name="tablaBuscar">Tabla en la que se editan los registros</param>
        /// <param name="registro">Aqui van uno o mas datos con los anteriores datos y los nuevos para editar</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devulve el mensaje correcto o el mensaje de error especificando en lo que ha fallado</returns>
        public Gestion EditarUnoOVariosRegistrosEnTablaGestor(string tablaBuscar, RegistroEditar registro, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {

                if (!ExisteTabla(tablaBuscar, connectionString))
                {
                    gestion.setError($"Error: No existe la tabla {tablaBuscar}");
                    return gestion;
                }
                for (int i = 0; i < registro.ValoresExistentes.Count; i++)
                {
                    if (!ExisteColumna(tablaBuscar, registro.ValoresExistentes[i].NombreColumna, connectionString))
                    {
                        gestion.setError($"Error: En la tabla {tablaBuscar} no existe la columna {registro.ValoresExistentes[i].NombreColumna}");
                        return gestion;
                    }
                    if (!TipoDatoCorrecto(tablaBuscar, registro.ValoresExistentes[i].NombreColumna, registro.ValoresExistentes[i].ValorRegistro, connectionString))
                    {
                        gestion.setError($"Error: El tipo de dato no concide con la columna {registro.ValoresExistentes[i].NombreColumna}");
                        return gestion;
                    }
                    if (!ExisteColumna(tablaBuscar, registro.ValoresNuevos[i].NombreColumna, connectionString))
                    {
                        gestion.setError($"Error: En la tabla {tablaBuscar} no existe la columna {registro.ValoresNuevos[i].NombreColumna}");
                        return gestion;
                    }
                    if (!TipoDatoCorrecto(tablaBuscar, registro.ValoresNuevos[i].NombreColumna, registro.ValoresNuevos[i].ValorRegistro, connectionString))
                    {
                        gestion.setError($"Error: El tipo de dato no concide con la columna {registro.ValoresNuevos[i].NombreColumna}");
                        return gestion;
                    }
                    if (!ExisteValor(tablaBuscar, registro.ValoresExistentes[i].NombreColumna, registro.ValoresExistentes[i].ValorRegistro, connectionString))
                    {
                        gestion.setError($"Error: No existe el registro {registro.ValoresExistentes[i].ValorRegistro} en la columna {registro.ValoresExistentes[i].NombreColumna}");
                        return gestion;
                    }
                }
                for (int i = 0; i < registro.ValoresExistentes.Count; i++)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = $"UPDATE {tablaBuscar} SET {registro.ValoresNuevos[i].NombreColumna} = @NuevoValor WHERE {registro.ValoresExistentes[i].NombreColumna} = @Valor";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Valor", registro.ValoresExistentes[i].ValorRegistro);
                            command.Parameters.AddWithValue("@NuevoValor", registro.ValoresNuevos[i].ValorRegistro);
                            int borrado = command.ExecuteNonQuery();
                            if (borrado > 0)
                            {
                                gestion.Correct("Registro editado correctamente.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return gestion;
        }
        /// <summary>
        /// Edita varios registros a la vez en los que se cumpla la misma condicion
        /// </summary>
        /// <param name="tablaBuscar">Tabla en la que se editan los registros</param>
        /// <param name="registro">Datos de los nuevos registros,columna y nuevo valor, y condiciones que se tiene que cumplir,lista con nombre de columna y valor actual</param>
        /// <param name="connectionString">La cadena de conexion a la base de datos</param>
        /// <returns>Devuelve el mensaje de correcto si ha podido, o el mensaje de error correspondiente si ha fallado</returns>
        public Gestion EditarTodosRegistrosQueCumplenVariasCondicionesEnTablaGestor(string tablaBuscar, RegistroMultipleEditar registro, string connectionString)
        {
            Gestion gestion = new Gestion();
            try
            {
                if (!ExisteTabla(tablaBuscar, connectionString))
                {
                    gestion.setError($"Error: No existe la tabla {tablaBuscar}");
                    return gestion;
                }
                for (int i = 0; i < registro.Condiciones.Count; i++)
                {
                    if (!ExisteColumna(tablaBuscar, registro.Condiciones[i].NombreColumna, connectionString))
                    {
                        gestion.setError($"Error: En la tabla {tablaBuscar} no existe la columna {registro.Condiciones[i].NombreColumna}");
                        return gestion;
                    }
                    if (!TipoDatoCorrecto(tablaBuscar, registro.Condiciones[i].NombreColumna, registro.Condiciones[i].ValorRegistro, connectionString))
                    {
                        gestion.setError($"Error: El tipo de dato no concide con la columna {registro.Condiciones[i].NombreColumna}");
                        return gestion;
                    }
                    if (!ExisteColumna(tablaBuscar, registro.ValoresNuevos.NombreColumna, connectionString))
                    {
                        gestion.setError($"Error: En la tabla {tablaBuscar} no existe la columna {registro.ValoresNuevos.NombreColumna}");
                        return gestion;
                    }
                    if (!TipoDatoCorrecto(tablaBuscar, registro.ValoresNuevos.NombreColumna, registro.ValoresNuevos.ValorRegistro, connectionString))
                    {
                        gestion.setError($"Error: El tipo de dato no concide con la columna {registro.ValoresNuevos.NombreColumna}");
                        return gestion;
                    }
                    if (!ExisteValor(tablaBuscar, registro.Condiciones[i].NombreColumna, registro.Condiciones[i].ValorRegistro, connectionString))
                    {
                        gestion.setError($"Error: No existe el registro {registro.Condiciones[i].ValorRegistro} en la columna {registro.Condiciones[i].NombreColumna}");
                        return gestion;
                    }

                }
                for (int i = 0; i < registro.Condiciones.Count; i++)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = $"UPDATE {tablaBuscar} SET {registro.ValoresNuevos.NombreColumna} = @NuevoValor WHERE {registro.Condiciones[i].NombreColumna} = @Valor";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Valor", registro.Condiciones[i].ValorRegistro);
                            command.Parameters.AddWithValue("@NuevoValor", registro.ValoresNuevos.ValorRegistro);
                            int editado = command.ExecuteNonQuery();
                            if (editado > 0)
                            {
                                gestion.Correct("Registro editado correctamente.");
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                gestion.setError("Error de tipo {0}, mensaje: {1}", new List<dynamic>() { ex.GetType().Name, ex.Message });
            }
            return gestion;
        }
        public Gestion EditarTodosRegistrosQueCumplenVariasCondicionesALaVezEnTablaGestor(string tablaBuscar, string andOr, RegistroMultipleEditar registro, string connectionString)
        {
            Gestion gestion = new Gestion();
            string conjuncion = "";
            try
            {
                if (!ExisteTabla(tablaBuscar, connectionString))
                {
                    gestion.setError($"Error: No existe la tabla {tablaBuscar}");
                    return gestion;
                }
                if (andOr == "A")
                {
                    conjuncion = " AND ";
                }
                else if (andOr == "O")
                {
                    conjuncion = " OR ";
                }
                else
                {
                    gestion.setError($"Error: Tienes que escribir A o O");
                    return gestion;
                }
                for (int i = 0; i < registro.Condiciones.Count; i++)
                {
                    if (!ExisteColumna(tablaBuscar, registro.Condiciones[i].NombreColumna, connectionString))
                    {
                        gestion.setError($"Error: En la tabla {tablaBuscar} no existe la columna {registro.Condiciones[i].NombreColumna}");
                        return gestion;
                    }
                    if (!TipoDatoCorrecto(tablaBuscar, registro.Condiciones[i].NombreColumna, registro.Condiciones[i].ValorRegistro, connectionString))
                    {
                        gestion.setError($"Error: El tipo de dato no concide con la columna {registro.Condiciones[i].NombreColumna}");
                        return gestion;
                    }
                    if (!ExisteColumna(tablaBuscar, registro.ValoresNuevos.NombreColumna, connectionString))
                    {
                        gestion.setError($"Error: En la tabla {tablaBuscar} no existe la columna {registro.ValoresNuevos.NombreColumna}");
                        return gestion;
                    }
                    if (!TipoDatoCorrecto(tablaBuscar, registro.ValoresNuevos.NombreColumna, registro.ValoresNuevos.ValorRegistro, connectionString))
                    {
                        gestion.setError($"Error: El tipo de dato no concide con la columna {registro.ValoresNuevos.NombreColumna}");
                        return gestion;
                    }
                    if (!ExisteValor(tablaBuscar, registro.Condiciones[i].NombreColumna, registro.Condiciones[i].ValorRegistro, connectionString))
                    {
                        gestion.setError($"Error: No existe el registro {registro.Condiciones[i].ValorRegistro} en la columna {registro.Condiciones[i].NombreColumna}");
                        return gestion;
                    }
                }
                if (registro.Condiciones.Count <= 0)
                {
                    gestion.setError($"Error: No hay suficientes condiciones para poder editar el registro");
                    return gestion;
                }
                StringBuilder sb = new StringBuilder();
                sb.Append($"UPDATE {tablaBuscar} SET {registro.ValoresNuevos.NombreColumna} = @NuevoValor WHERE ");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    for (int i = 0; i < registro.Condiciones.Count; i++)
                    {
                        sb.Append($"{registro.Condiciones[i].NombreColumna} = @valor{i}");
                        if (i < registro.Condiciones.Count - 1)
                        {
                            sb.Append(conjuncion);
                        }
                    }
                    string query = sb.ToString();
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        for (int i = 0; i < registro.Condiciones.Count; i++)
                        {
                            command.Parameters.AddWithValue($"@valor{i}", registro.Condiciones[i].ValorRegistro);
                        }
                        command.Parameters.AddWithValue("@NuevoValor", registro.ValoresNuevos.ValorRegistro);
                        int editado = command.ExecuteNonQuery();
                        if (editado > 0)
                        {
                            gestion.Correct("Registro editado correctamente.");
                        }
                        else
                        {
                            gestion.setError("Error: No se han podido editar registros, condicion no válida.");
                        }
                    }
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
