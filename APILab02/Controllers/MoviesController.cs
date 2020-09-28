using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APILab02.Manager;
using APILab02.Models;
using LibreriaGenericos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APILab02.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        public static Movies AuxMovie = new Movies();
        public static string Ruta = Path.GetFullPath("Archivos\\ArchivosDisco.txt");
        public static Arbol_B<Movies> Arbol = new Arbol_B<Movies>(5, AuxMovie.LongitudClase(), Ruta);
        // GET: api/Movies
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Jose Daniel Giron", "Cristian Josue Barrientos" };
        }

        // POST: api/movies
        [HttpPost]
        public IActionResult Grado([FromBody] ArbolGrado Nivel)
        {
            try
            {
                Arbol = new Arbol_B<Movies>(Nivel.Grado, AuxMovie.LongitudClase(), Ruta);
                return Created("", Nivel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public IActionResult EliminarArbol()
        {
            Arbol = new Arbol_B<Movies>(4, AuxMovie.LongitudClase(), Ruta);
            return Ok();
        }

        // POST: api/movies/populate
        [HttpPost("populate")]
        public async Task<ActionResult> Creacion([FromForm] IFormFile file)
        {
            try
            {
                using var ContentInMemory = new MemoryStream();
                await file.CopyToAsync(ContentInMemory);
                var Content = Encoding.ASCII.GetString(ContentInMemory.ToArray());
                List<Movies> Response = ManagerMovies.ProccessMoviesFile(Content);
                await Task.Run(() =>
                {
                    foreach (Movies Peli in Response)
                    {
                        Peli.id = Peli.title + "-" + Peli.releaseDate.Substring(7, 4);
                        Arbol.Add(Peli, Peli.BuscarId);
                    }
                });
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("populate/{Id}")]
        public IActionResult Borrar([FromRoute] string Id)
        {
            try
            {
               // AuxMovie.id = Convert.ToInt32(Id);
                bool Encontrado = Arbol.Delete(AuxMovie, AuxMovie.BuscarId);
                if (Encontrado)
                    return Ok();
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/movies/{tranversal}
        [HttpGet("{tranversal}")]
        public IEnumerable<string> Ordenamiento([FromRoute] string tranversal)
        {
            Movies Clase = new Movies();
            if (tranversal == "inorder")
                return Arbol.Recorrido(1, Clase);
            else if (tranversal == "postorder")
                return Arbol.Recorrido(2, Clase);
            else if (tranversal == "preorder")
                return Arbol.Recorrido(3, Clase);
            return null;
        }

    }
}
