using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace site_de_receita_api.Models.DTOs.Read
{
    public class ReceitaLeituraDto
    {
        public int Id { get; set; }
        public string FotoUrl { get; set; }
        public string LinkTutorial { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Tipo { get; set; }

        public List<IngredienteLeituraDto> Ingredientes { get; set; }
    }
}