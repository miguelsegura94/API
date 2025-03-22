using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBDD.Modelos
{
    //TODO SEPARAR CLASES API Y CLASES SERVICIO
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set;}
        public int dinero { get; set; } = 0;
        public int Eloid { get; set; }
        public int ?Skinid { get; set; }
        public Elo Elo { get; set; }
        public Skin Skin { get; set; }
    }
    
    [Table("Elo")]
    public class Elo
    {
        [Key]
        public int Id { get; set; }
        public string nombre { get; set; }
        public int rating { get; set; }
    }
    [Table("Skin")]
    public class Skin
    {
        [Key]
        public int Id { get; set; }
        public string nombre { get; set; }
        public string rareza { get; set; }
        public int coste { get; set; }
    }

}
