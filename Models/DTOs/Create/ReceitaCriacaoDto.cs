using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace site_de_receita_api.Models.DTOs.Create
{
    public class ReceitaCriacaoDto
    {
        public string FotoUrl { get; set; }
        public string LinkTutorial { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public TiposDeReceita Tipo { get; set; }
        public List<int> IngredienteId { get; set; }
    }
}