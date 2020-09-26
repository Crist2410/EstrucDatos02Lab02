using System;
using System.Collections.Generic;
using System.Text;

namespace LibreriaGenericos
{
    public interface ConvertirClase<T>
    {
        public abstract int LongitudClase();
        public abstract string ClaseATexto();
        public abstract T TextoAClase(string Texto);
        public abstract T[] VectorTextoAClases(string[] Texto);
    }
}
