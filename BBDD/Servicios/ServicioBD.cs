using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBDD.Modelos;
using GestorBaseDatos.GestionCarpeta;
using GestorBaseDatos.GestorBD;
using GestorBaseDatos.GestorBD.GestorBD;

namespace BBDD.Servicios
{
    public class ServicioBD
    {
        private readonly string _connectionString;
        private readonly GestorBD _gestorBD;
        public ServicioBD(string connectionString)
        {
            _connectionString = connectionString;
            _gestorBD = new GestorBD();
        }
        public Gestion GetListaCompletaRegistrosServicio(string tabla)
        {
            Gestion gestion = new Gestion();
            gestion=_gestorBD.GetListaCompletaRegistrosGestor(tabla, _connectionString);
            return gestion;
        }
        public Gestion GetDatoEnTablaPorIdServicio(string tabla,int id)
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.GetDatoEnTablaPorIdGestor(tabla,id, _connectionString);
            return gestion;
        }
        public Gestion GetTablasServicio()
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.GetTablasGestor(_connectionString);
            return gestion;
        }
        public Gestion TestServicio(TablaBD tabla)
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.TestGestor(tabla,_connectionString);
            return gestion;
        }
        public Gestion GetRegistroEnTablaPorValorServicio(string tabla, string columna, string valor)
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.GetRegistroEnTablaPorValorGestor(tabla, columna, valor, _connectionString);
            return gestion;
        }
        public Gestion ObtenerJsonParaRegistroEnTablaServicio(string tabla)
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.ObtenerJsonParaRegistroEnTablaGestor(tabla, _connectionString);
            return gestion;
        }
        public Gestion CrearRegistroEnTablaFrombodyServicio(string tabla, Dictionary<string, object> datosAñadir)
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.CrearRegistroEnTablaFrombodyGestor(tabla, datosAñadir, _connectionString);
            return gestion;
        }
        public Gestion EditarUnoOVariosRegistrosEnTablaServicio(string tablaBuscar, RegistroEditar registro)
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.EditarUnoOVariosRegistrosEnTablaGestor(tablaBuscar, registro, _connectionString);
            return gestion;
        }
        public Gestion EditarTodosRegistrosQueCumplenVariasCondicionesEnTablaServicio(string tablaBuscar, RegistroMultipleEditar registro)
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.EditarTodosRegistrosQueCumplenVariasCondicionesEnTablaGestor(tablaBuscar, registro, _connectionString);
            return gestion;
        }
        public Gestion EditarTodosRegistrosQueCumplenVariasCondicionesALaVezEnTablaServicio(string tablaBuscar,string andOr, RegistroMultipleEditar registro)
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.EditarTodosRegistrosQueCumplenVariasCondicionesALaVezEnTablaGestor(tablaBuscar, andOr, registro, _connectionString);
            return gestion;
        }
        public Gestion EliminarRegistroEnTablaServicio(string tabla, RegistroDelete registro)
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.EliminarRegistroEnTablaGestor(tabla, registro, _connectionString);
            return gestion;
        }
        public Gestion CrearTablaServicio(TablaBD modeloTabla)
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.CrearTablaGestor(modeloTabla, _connectionString);
            return gestion;
        }
        public Gestion EliminarTablaServicio(string tabla)
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.EliminarTablaGestor(tabla, _connectionString);
            return gestion;
        }
        public Gestion GetColumnasTablaServicio(string tabla)
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.GetColumnasTablaGestor(tabla, _connectionString);
            return gestion;
        }
        public Gestion AñadirColumnaCompletaATablaServicio(string tabla,Columna columna)
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.AñadirColumnaCompletaATablaGestor(tabla,columna, _connectionString);
            return gestion;
        }
        public Gestion AñadirColumnaBasicaATablaServicio(string tabla, ColumnaInsert columna)
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.AñadirColumnaBasicaATablaGestor(tabla, columna, _connectionString);
            return gestion;
        }
        public Gestion EliminarColumnaTablaServicio(string tabla, ColumnaDelete columna)
        {
            Gestion gestion = new Gestion();
            gestion = _gestorBD.EliminarColumnaTablaGestor(tabla, columna, _connectionString);
            return gestion;
        }
        //AQUI TENGO QUE CREAR LOS METODOS QUE VOY A LLAMAR DESDE EL CONTROLLER, ESTOS METODOS RECIBIRAN LOS PARAMETROS NECESARIOS DESDE
        //LA API, ES DECIR EL CONTROLLER, ESTOS METODOS TIENEN QUE SER GENERICOS. ESTOS METODOS LE TIENEN QUE PASAR LOS NECESARIO AL GESTOR PARA EJECUTAR LAS CONSULTAS
        //Y DEVOLVER LOS RESULTADOS A ESTOS METODOS
    }
}
