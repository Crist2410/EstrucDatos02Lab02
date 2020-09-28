using System;
using System.Collections.Generic;
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
        Nodo NodoHermano2;
        Nodo NuevaRaiz;
        string[] ValoresMayores;
        string ValorVacio;
        int[] HijosMayores;
        int[] NodoHijos;
        byte[] Datos;
        FileStream Archivo;
        bool Encontrado;

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
        #region Insertar
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
        #endregion
        public bool Delete(T Valor, Delegate Delegado)
        {
            Archivo = new FileStream(RutaArbol, FileMode.Open);
            Encontrado = false;
            bool Eliminado = true;
            int HijoPosicion = Raiz;
            do
            {
                NodoActual = BucarNodoTXT(HijoPosicion, Valor);
                T[] ValoresT = Valor.VectorTextoAClases(NodoActual.Valores);
                for (int i = 0; i < Grado; i++)
                    if (ValoresT[i] != null && Convert.ToInt32(Delegado.DynamicInvoke(Valor, ValoresT[i])) == -1)
                    {
                        HijoPosicion = NodoActual.Hijos[i];
                        i = Grado;
                    }
                    else if (ValoresT[i] != null && Convert.ToInt32(Delegado.DynamicInvoke(Valor, ValoresT[i])) == 0)
                    {
                        Eliminar(Valor, Delegado, i);
                        i = Grado;
                        Eliminado = false;
                        Encontrado = true;
                    }
                    else if (ValoresT[i] == null && Convert.ToInt32(Delegado.DynamicInvoke(Valor, ValoresT[i - 1])) == 1)
                    {
                        HijoPosicion = NodoActual.Hijos[i];
                        i = Grado;
                    }
                    else if (i == Grado - 1 && Convert.ToInt32(Delegado.DynamicInvoke(Valor, ValoresT[i])) == 0 && NodoActual.HijosVacios())
                        Eliminado = false;
            } while (Eliminado);
            Archivo.Close();
            return Encontrado;
        }
        void Eliminar(T Valor, Delegate Delegado, int PosicionHijo)
        {
            bool Eliminado = true;
            int UnderFlow = Convert.ToInt32((Grado / 2) + 0.5) - 1;
            NuevaRaiz = BucarNodoTXT(NodoActual.Padre, Valor);
            T[] ValoresPadre = Valor.VectorTextoAClases(NuevaRaiz.Valores);
            T[] ValoresActual = Valor.VectorTextoAClases(NodoActual.Valores);
            do
            {
                if (ValoresActual[UnderFlow] != null && NodoActual.HijosVacios())
                {
                    for (int i = 0; i < Grado; i++)
                        if (ValoresActual[i] != null && Convert.ToInt32(Delegado.DynamicInvoke(Valor, ValoresActual[i])) == 0)
                        {
                            NodoActual.Valores[i] = ValorVacio;
                            for (int f = i; f < Grado - 1; f++)
                                if (NodoActual.Valores[f].Trim() == "-1" && f != Grado - 2)
                                {
                                    NodoActual.Valores[f] = NodoActual.Valores[i + 1];
                                    NodoActual.Valores[f + 1] = ValorVacio;
                                }
                            EscribirNodoTXT(NodoActual);
                            i = Grado;
                        }
                    Eliminado = false;
                }
                else if (ValoresActual[UnderFlow] != null && NodoActual.HijosVacios())
                {
                    if (PosicionHijo == 0)
                    {
                        NodoHermano2 = BucarNodoTXT(NodoActual.Hijos[PosicionHijo + 1], Valor);
                        if (NodoHermano2.Valores[UnderFlow] != ValorVacio)
                        {

                        }
                    }
                    else if (Posicion == Grado - 1)
                    {
                        NodoHermano = BucarNodoTXT(NodoActual.Hijos[PosicionHijo - 1], Valor);
                        if (NodoHermano.Valores[UnderFlow] != ValorVacio)
                        {

                        }
                    }
                    else
                    {
                        NodoHermano = BucarNodoTXT(NodoActual.Hijos[PosicionHijo - 1], Valor);
                        NodoHermano2 = BucarNodoTXT(NodoActual.Hijos[PosicionHijo + 1], Valor);
                        if (NodoHermano.Valores[UnderFlow] != ValorVacio)
                        {

                        }
                        else if (NodoHermano2.Valores[UnderFlow] != ValorVacio)
                        {

                        }
                    }


                }
                else if (ValoresActual[UnderFlow] != null && !NodoActual.HijosVacios())
                {
                    Eliminado = false;
                }
            } while (Eliminado);
        }


        public List<string> Recorrido(int Recorrido, T Valor)
        {
            Archivo = new FileStream(RutaArbol, FileMode.Open);
            List<string> Lista = new List<string>();
            if (Recorrido == 1)
            {
                InOrder(Raiz, 0, Lista, Valor);
            }
            else if (Recorrido == 2)
            {
                PostOrder(Raiz, 0, Lista, Valor);
            }
            else if (Recorrido == 3)
            {
                PreOrder(Raiz, 0, Lista, Valor);
            }
            Archivo.Close();
            return Lista;
        }

        public void InOrder(int Posicion, int Num, List<string> Lista, T Valor)
        {
            Nodo NodoActual = BucarNodoTXT(Posicion, Valor);
            for (int i = 0; i < NodoActual.Valores.Length; i++)
            {
                if (NodoActual.Hijos[i] != -1)
                {
                    InOrder(NodoActual.Hijos[i], 0, Lista, Valor);
                    if (NodoActual.Valores[i] != null)
                    {
                        if (NodoActual.Valores[i] != "                                                                                                                           -1")
                        {
                            Lista.Add(NodoActual.Valores[i]);
                        }

                    }
                }
                else
                {
                    if (NodoActual.Valores[i] != null)
                    {
                        if (NodoActual.Valores[i] != "                                                                                                                           -1")
                        {
                            Lista.Add(NodoActual.Valores[i]);
                        }

                    }
                }
            }
        }
        public void PostOrder(int Posicion, int Num, List<string> Lista, T Valor)
        {
            Nodo NodoActual = BucarNodoTXT(Posicion, Valor);
            for (int i = 0; i < NodoActual.Valores.Length; i++)
            {
                if (NodoActual.Valores.Length - 1 == i)
                {
                    Lista = InsercionRecorrido(Lista, NodoActual);
                }
                else
                {
                    if (NodoActual.Hijos[i] != -1)
                    {
                        PostOrder(NodoActual.Hijos[i], Num, Lista, Valor);
                    }
                }
            }
        }

        List<string> InsercionRecorrido(List<string> Lista, Nodo NodoActual)
        {
            for (int i = 0; i < NodoActual.Valores.Length; i++)
            {
                if (NodoActual.Valores[i] != null)
                {
                    if (NodoActual.Valores[i] != "                                                                                                                           -1")
                    {
                        Lista.Add(NodoActual.Valores[i]);
                    }
                }
            }
            return Lista;
        }

        public void PreOrder(int Posicion, int Num, List<String> Lista, T Valor)
        {
            Nodo NodoActual = BucarNodoTXT(Posicion, Valor);
            for (int i = 0; i < NodoActual.Valores.Length; i++)
            {
                if (i == 0)
                {
                    Lista = InsercionRecorrido(Lista, NodoActual);
                }
                else
                {
                    if (NodoActual.Hijos[i] != -1)
                    {
                        PreOrder(NodoActual.Hijos[i], Num, Lista, Valor);
                    }
                }
            }

        }
    }
}
