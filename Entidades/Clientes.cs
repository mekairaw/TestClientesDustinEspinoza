using System;

namespace Entidades
{
    [Serializable]
    public class Clientes
    {
        public int ID { get; set; }
        public string NOMBRE_COMPLETO { get; set; }
        public double IDENTIFICACION { get; set; }
        public double TELEFONO { get; set; }
    }
}
