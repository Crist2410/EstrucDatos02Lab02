using System;
using System.Collections.Generic;
using System.Text;

namespace LibreriaGenericos
{
    class Nodo<T>
    {
        public int Id { get; set; }
        public int Padre { get; set; }
        public int[] Hijos { get; set; }
        public string[] Valores { get; set; }
        public Nodo(int Grado)
        {
            Hijos = new int[Grado + 1];
            Valores = new string[Grado];
        }

        public int LongitudNodo()
        {
            return ToString().Length;
        }
        public override string ToString()
        {
            string TextoHijos = Hijos[0].ToString("000000000;-00000000") + "|";
            string TextoValores = "";
            for (int i = 0; i < Valores.Length - 1; i++)
            {
                TextoHijos += Hijos[i + 1].ToString("000000000;-00000000") + "|";
                TextoValores += Valores[i] + "|";
            }
            return $"{Id.ToString("000000000;-00000000")}~{Padre.ToString("000000000;-00000000")}~" +
                 $"{TextoHijos}~{TextoValores}\r\n";
        }
        public bool HijosVacios()
        {
            for (int i = 0; i < Hijos.Length; i++)
                if (Hijos[i] != -1 && Hijos[i] != 0)
                    return false;
            return true;
        }
    }
}
