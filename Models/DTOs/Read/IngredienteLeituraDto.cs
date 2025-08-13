using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace site_de_receita_api.Models.DTOs.Read
{
    public class IngredienteLeituraDto
    {
        public int Id { get; set; }
        public string FotoUrl { get; set; }
        public string Nome { get; set; }
    }
}