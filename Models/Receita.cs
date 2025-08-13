using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace site_de_receita_api.Models
{
    public class Receita
    {
        public int Id { get; set; }
        
        public string FotoUrl { get; set; }
        
        public string LinkTutorial { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(500)]
        public string Descricao { get; set; }

        [Required]
        public TiposDeReceita Tipo { get; set; }

        [Required]
        public ICollection<ReceitaIngrediente> ReceitaIngredientes { get; set; }
    }
}