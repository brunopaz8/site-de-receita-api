using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace site_de_receita_api.Models
{
    public class Ingrediente
    {
        public int Id { get; set; }
        
        public string FotoUrl { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        
        public ICollection<ReceitaIngrediente> ReceitaIngredientes { get; set; }
    }
}