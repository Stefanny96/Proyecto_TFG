namespace Proyecto_TFG.Models
{
    public class Proveedor
    {
        public string proveedor_id { get; set; }
        public string nombre_razon_social { get; set; }
        public string nif_cif { get; set; }
        public Direccion direccion { get; set; }
        public Contacto_Principal contacto_principal { get; set; }

        public class Direccion
        {
            public string calle { get; set; }
            public string ciudad { get; set; }
            public string pais { get; set; }
            public string codigo_postal { get; set; }
        }

        public class Contacto_Principal
        {
            public string nombre { get; set; }
            public string cargo { get; set; }
            public string telefono { get; set; }
            public string correo_electronico { get; set; }
        }

    }
}
