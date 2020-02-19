using System;

namespace Entidades
{
    //clase creada para su uso a traves de toda la aplicacion, haciendo referencia a los nombres utilizados en la base de datos
    [Serializable]
    public class Clientes
    {
        public int ID { get; set; }
        public string NOMBRE_COMPLETO { get; set; }
        public double IDENTIFICACION { get; set; }
        public double TELEFONO { get; set; }
    }
}
