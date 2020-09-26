using APILab02.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace APILab02.Manager
{
    public class ManagerMovies
    {
        public static List<Movies> ProccessMoviesFile(string content)
        {
            return JsonSerializer.Deserialize<List<Movies>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}
