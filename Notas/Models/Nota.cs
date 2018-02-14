//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Notas.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.SqlClient;

    public partial class Nota
    {
        public int id { get; set; }
        [Display (Name = "Titulo")]
        public string titulo { get; set; }
        [Display(Name = "Descripcion")]
        [Required]
        public string descripcion { get; set; }
        public Nullable<int> id_usuario { get; set; }
    
        public virtual Usuario Usuario { get; set; }

        public Nota()
        {
                
        }
        public Nota(int _id, string _titulo, string _descripcion, int _id_usuario)
        {
            id = _id;
            titulo = _titulo;
            descripcion = _descripcion;
            id_usuario = _id_usuario;
        }
    }
}
