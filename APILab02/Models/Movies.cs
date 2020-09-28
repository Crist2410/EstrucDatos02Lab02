using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibreriaGenericos;

namespace APILab02.Models
{
    public class Movies : ConvertirClase<Movies>
    {
        public string id { get; set; }
        public string title { get; set; }
        public string director { get; set; }
        public double imdbRating { get; set; }
        public string genre { get; set; }
        public string releaseDate { get; set; }
        public double rottenTomatoesRating { get; set; }

        public Comparison<Movies> BuscarId = delegate (Movies M1, Movies M2)
        {
            return M1.id.CompareTo(M2.id);
        };
        public Comparison<Movies> BuscarNombre = delegate (Movies M1, Movies M2)
        {
            return M1.title.CompareTo(M2.title);
        };
        public int LongitudClase()
        {
            return ClaseATexto().Length;
        }

        public string ClaseATexto()
        {
            return $"{string.Format("{0,-155}", id)}^{string.Format("{0,-150}", title)}^{string.Format("{0,-45}", director)}" +
                $"^{imdbRating.ToString("00000000000;-0000000000")}^{string.Format("{0,-75}", genre)}^" +
                $"{string.Format("{0,-11}", releaseDate)}^{rottenTomatoesRating.ToString("00000000000; -0000000000")}";
        }

        public Movies TextoAClase(string Texto)
        {
            string[] Separacion = Texto.Split("^");
            Movies Movi = new Movies
            {
                id = Separacion[0],
                title = Separacion[1],
                director = Separacion[2],
                imdbRating = Convert.ToDouble(Separacion[3]),
                genre = Separacion[4],
                releaseDate = Separacion[5],
                rottenTomatoesRating = Convert.ToDouble(Separacion[6])
            };
            return Movi;
        }

        public Movies[] VectorTextoAClases(string[] Texto)
        {
            Movies[] VectorPeliculas = new Movies[Texto.Length];
            for (int i = 0; i < Texto.Length; i++)
            {
                if (Texto[i] != null && Texto[i].Trim() != "-1")
                {
                    string[] Separacion = Texto[i].Split("^");
                    VectorPeliculas[i] = new Movies
                    {
                        id = Separacion[0],
                        title = Separacion[1],
                        director = Separacion[2],
                        imdbRating = Convert.ToDouble(Separacion[3]),
                        genre = Separacion[4],
                        releaseDate = Separacion[5],
                        rottenTomatoesRating = Convert.ToDouble(Separacion[6])
                    };
                }

            }
            return VectorPeliculas;
        }
    }
}
