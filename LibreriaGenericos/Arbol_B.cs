using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LibreriaGenericos
{
    public class Arbol_B<T> where T : ConvertirClase<T>
    {
        string RutaArbol;
        int Grado;
        int Raiz;
        int Posicion;
        Nodo NodoActual;
        Nodo NodoHermano;
        Nodo NuevaRaiz;
        string[] ValoresMayores;
        string ValorVacio;
        int[] HijosMayores;
        int[] NodoHijos;
        byte[] Datos;
        FileStream Archivo;

        public Arbol_B(int Tamaño, int LongitudClase, string RutaDatos)
        {
            RutaArbol = RutaDatos;
            Grado = Tamaño;
            NodoActual = new Nodo(Grado);
            Raiz = 0;
            ValorVacio = string.Concat(Enumerable.Repeat(" ", LongitudClase - 2));
            ValorVacio += "-1";
            NodoHijos = new int[Grado + 1];
            for (int i = 0; i < Grado; i++)
                NodoHijos[i] = -1;
            StreamWriter LimpiarArchivo = new StreamWriter(RutaArbol, false);
            LimpiarArchivo.Write("-1");
            LimpiarArchivo.Close();
            Datos = new byte[NodoActual.LongitudNodo()];
        }
        public void Add(T Valor, Delegate Delegado)
        {
            Archivo = new FileStream(RutaArbol, FileMode.Open);
            if (Raiz == 0)
            {
                Posicion++;
                NodoActual = new Nodo(Grado);
                NodoActual.Id = ++Raiz;
                NodoActual.Padre = -1;
                NodoActual.Hijos = NodoHijos;
                NodoActual.Valores[0] = Valor.ClaseATexto();
                for (int i = 1; i < Grado - 1; i++)
                    NodoActual.Valores[i] = ValorVacio;
                EscribirNodoTXT(NodoActual);
            }
            else
            {
                bool ValorInsertado = true;
                int HijoPosicion = Raiz;
                do
                {
                    NodoActual = BucarNodoTXT(HijoPosicion, Valor);
                    if (!NodoActual.HijosVacios())
                    {
                        T[] ValoresT = Valor.VectorTextoAClases(NodoActual.Valores);
                        for (int i = 0; i < Grado; i++)
                        {
                            if (ValoresT[i] != null && Convert.ToInt32(Delegado.DynamicInvoke(Valor, ValoresT[i])) == -1)
                            {
                                HijoPosicion = NodoActual.Hijos[i];
                                i = Grado;
                            }
                            else if (ValoresT[i] != null && Convert.ToInt32(Delegado.DynamicInvoke(Valor, ValoresT[i])) == 0)
                            {
                                i = Grado;
                                ValorInsertado = false;
                            }
                            else if (ValoresT[i] == null)
                            {
                                HijoPosicion = NodoActual.Hijos[i];
                                i = Grado;
                            }
                        }
                    }
                    else if (NodoActual.HijosVacios())
                    {
                        NodoActual = AgregarValor(NodoActual, Valor, Delegado);
                        if (NodoActual.Valores[Grado - 1] != null && NodoActual.Valores[Grado - 1].Trim() != "-1")
                            Subida(Valor, Delegado);
                        else
                            EscribirNodoTXT(NodoActual);
                        ValorInsertado = false;
                    }
                } while (ValorInsertado);
            }
            Archivo.Close();
        }
        void Subida(T Valor, Delegate Delegado)
        {
            bool ValorInsertado = true;
            do
            {
                int Centro = Convert.ToInt32((Grado / 2) + 0.5) - 1;
                NodoHermano = new Nodo(Grado);
                NuevaRaiz = new Nodo(Grado);
                ValoresMayores = new string[Grado];
                HijosMayores = new int[Grado + 1];
                for (int i = Centro + 1; i < Grado; i++)
                {
                    ValoresMayores[i - (Centro + 1)] = NodoActual.Valores[i];
                    NodoActual.Valores[i] = ValorVacio;
                    HijosMayores[i - (Centro + 1)] = NodoActual.Hijos[i];
                    NodoActual.Hijos[i] = -1;
                    if (i == Grado - 1)
                    {
                        HijosMayores[i - Centro] = NodoActual.Hijos[i + 1];
                        NodoActual.Hijos[i + 1] = -1;
                    }
                }
                for (int i = 0; i < Grado; i++)
                {
                    if (ValoresMayores[i] == null)
                        ValoresMayores[i] = ValorVacio;
                    if (HijosMayores[i + 1] == 0)
                        HijosMayores[i + 1] = -1;
                }
                NodoHermano.Id = ++Posicion;
                NodoHermano.Hijos = HijosMayores;
                NodoHermano.Valores = ValoresMayores;
                if (NodoActual.Padre == -1)
                {
                    NuevaRaiz.Id = ++Posicion;
                    NuevaRaiz.Hijos[0] = NodoActual.Id;
                    NuevaRaiz.Hijos[1] = NodoHermano.Id;
                    NuevaRaiz.Valores[0] = NodoActual.Valores[Centro];
                    Raiz = NuevaRaiz.Id;
                    for (int i = 2; i < Grado + 1; i++)
                    {
                        if (NuevaRaiz.Hijos[i] == 0)
                            NuevaRaiz.Hijos[i] = -1;
                        if (NuevaRaiz.Valores[i - 1] == null)
                            NuevaRaiz.Valores[i - 1] = ValorVacio;
                    }
                    NodoHermano.Padre = NuevaRaiz.Id;
                    NodoActual.Padre = NuevaRaiz.Id;
                    NuevaRaiz.Padre = -1;
                }
                else if (NodoActual.Padre >= 0)
                {
                    NuevaRaiz = BucarNodoTXT(NodoActual.Padre, Valor);
                    T ValorAux = Valor.TextoAClase(NodoActual.Valores[Centro]);
                    NuevaRaiz = AgregarValor(NuevaRaiz, ValorAux, Delegado);
                    NuevaRaiz.Hijos = OrdenarHijos(NuevaRaiz.Hijos, NodoActual.Id, Posicion);
                    NodoHermano.Padre = NuevaRaiz.Id;
                }
                NodoActual.Valores[Centro] = ValorVacio;
                if (!NodoHermano.HijosVacios())
                {
                    for (int i = 0; i < NodoHermano.Hijos.Length - 1; i++)
                        if (NodoHermano.Hijos[i] >= 1)
                        {
                            Nodo HijoAux = BucarNodoTXT(NodoHermano.Hijos[i], Valor);
                            HijoAux.Padre = NodoHermano.Id;
                            EscribirNodoTXT(HijoAux);
                        }
                }
                //NodoAcutal
                EscribirNodoTXT(NodoActual);
                //NuevoNodo
                EscribirNodoTXT(NodoHermano);
                //NodoRaiz
                EscribirNodoTXT(NuevaRaiz);
                if (NuevaRaiz.Valores[Grado - 1] != null && NuevaRaiz.Valores[Grado - 1].Trim() != "-1")
                    NodoActual = NuevaRaiz;
                else
                    ValorInsertado = false;
            } while (ValorInsertado);
        }

        Nodo BucarNodoTXT(int IdNodo, T Valor)
        {
            Datos = new byte[NodoActual.LongitudNodo()];
            Archivo.Seek((IdNodo - 1) * NodoActual.LongitudNodo(), SeekOrigin.Begin);
            Archivo.Read(Datos, 0, NodoActual.LongitudNodo());
            string LineaDatos = Encoding.ASCII.GetString(Datos);
            Archivo.Flush();
            Nodo NodoAux = new Nodo(Grado)
            {
                Id = Convert.ToInt32(LineaDatos.Substring(0, 9)),
                Padre = Convert.ToInt32(LineaDatos.Substring(10, 9))
            };
            NodoAux.Hijos[0] = Convert.ToInt32(LineaDatos.Substring(20, 9));
            for (int i = 0; i < Grado - 1; i++)
            {
                NodoAux.Hijos[i + 1] = Convert.ToInt32(LineaDatos.Substring(30 + (10 * i), 9));
                NodoAux.Valores[i] = LineaDatos.Substring((21 + (10 * Grado) + (Valor.LongitudClase() + 1) * i), Valor.LongitudClase());
            };
            Archivo.Flush();
            return NodoAux;
        }
        void EscribirNodoTXT(Nodo NodoAux)
        {
            Datos = Encoding.ASCII.GetBytes(NodoAux.ToString());
            Archivo.Seek((NodoAux.Id - 1) * NodoAux.LongitudNodo(), SeekOrigin.Begin);
            Archivo.Write(Datos, 0, Datos.Length);
            Archivo.Flush();
        }
        string[] Ordenamiento(T[] ValoresVector, Delegate Delegado)
        {
            int Limite = ValoresVector.Length - 1;
            string[] ValoresOrden = new string[ValoresVector.Length];
            for (int i = 0; i < Limite; i++)
                for (int f = 0; f < Limite - i; f++)
                    if (ValoresVector[f + 1] == null)
                        f = Limite;
                    else if (Convert.ToInt32(Delegado.DynamicInvoke(ValoresVector[f], ValoresVector[f + 1])) == 1)
                    {
                        T Aux = ValoresVector[f];
                        ValoresVector[f] = ValoresVector[f + 1];
                        ValoresVector[f + 1] = Aux;
                    }
            for (int i = 0; i < ValoresVector.Length; i++)
                if (ValoresVector[i] == null)
                    ValoresOrden[i] = ValorVacio;
                else
                    ValoresOrden[i] = ValoresVector[i].ClaseATexto();
            return ValoresOrden;
        }
        Nodo AgregarValor(Nodo NodoAux, T Valor, Delegate Delegado)
        {
            for (int i = 0; i < Grado; i++)
            {
                if (i == Grado - 1)
                    NodoAux.Valores[i] = Valor.ClaseATexto();
                if (NodoAux.Valores[i].Trim() == "-1")
                {
                    NodoAux.Valores[i] = Valor.ClaseATexto();
                    i = Grado;
                }
            }
            NodoAux.Valores = Ordenamiento(Valor.VectorTextoAClases(NodoAux.Valores), Delegado);
            return NodoAux;
        }
        int[] OrdenarHijos(int[] Hijos, int Id, int Numero)
        {
            for (int i = Hijos.Length - 2; i >= 0; i--)
            {
                if (Hijos[i] != Id)
                    Hijos[i + 1] = Hijos[i];
                else if (Hijos[i] == Id)
                {
                    Hijos[i + 1] = Numero;
                    i = 0;
                }
            }
            return Hijos;
        }
    }
}
