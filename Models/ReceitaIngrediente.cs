using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace site_de_receita_api.Models
{
    public class ReceitaIngrediente
    {
    public int Id { get; set; }
    
    public int ReceitaId { get; set; }
    public Receita Receita { get; set; }

    public int IngredienteId { get; set; }
    public Ingrediente Ingrediente { get; set; }

    }

}