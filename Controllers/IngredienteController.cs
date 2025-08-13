using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.Replication.PgOutput.Messages;
using site_de_receita_api.Data;
using site_de_receita_api.Models;
using site_de_receita_api.Models.DTOs.Create;
using site_de_receita_api.Models.DTOs.Read;

namespace site_de_receita_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IngredienteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateIngrediente(IngredienteCriacaoDto dto)
        {

            var existe = await _context.Ingredientes.AnyAsync(i => i.Nome.ToLower() == dto.Nome.ToLower());

            if (existe)
            {
                return Conflict("Esse ingrediente já foi cadastrado.");
            }
            var ingrediente = new Ingrediente
            {
                Nome = dto.Nome,
                FotoUrl = dto.FotoUrl
            };

            _context.Ingredientes.Add(ingrediente);
            await _context.SaveChangesAsync();

            return StatusCode(201, ingrediente);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingrediente>>> FindAll()
        {
            var ingrediente = await _context.Ingredientes.Select(i => new IngredienteLeituraDto
            {
                Id = i.Id,
                FotoUrl = i.FotoUrl,
                Nome = i.Nome

            }).ToListAsync();
            return Ok(ingrediente);
        }

        [HttpGet("Nome")]
        public async Task<IActionResult> FindByName(string nome)
        {
            var ingredientes = await _context.Ingredientes
            .Where(i => i.Nome.ToLower().Contains(nome.ToLower())).Select(i => new IngredienteLeituraDto
            {
                Id = i.Id,
                FotoUrl = i.FotoUrl,
                Nome = i.Nome

            }).ToListAsync();

            if (!ingredientes.Any())
            {
                return NotFound($"Nenhum ingrediente com o nome {nome} foi encontrado!");
            }

            return Ok(ingredientes);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] IngredienteCriacaoDto ingrediente)
        {
            var ingredienteBanco = await _context.Ingredientes.FindAsync(id);

            if (ingredienteBanco == null)
            {
                return NotFound($"Nenhum ingrediente com o id {id} foi encontrado!");
            }

            if (!string.IsNullOrWhiteSpace(ingrediente.Nome))
            {
                ingredienteBanco.Nome = ingrediente.Nome;
            }

            if (!string.IsNullOrWhiteSpace(ingrediente.FotoUrl))
            {
                ingredienteBanco.FotoUrl = ingrediente.FotoUrl;
            }

            await _context.SaveChangesAsync();

            var leituraDto = new IngredienteLeituraDto
            {
                Id = ingredienteBanco.Id,
                Nome = ingredienteBanco.Nome,
                FotoUrl = ingredienteBanco.FotoUrl
            };

            return Ok(leituraDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ingredienteBanco = await _context.Ingredientes.FindAsync(id);
            if (ingredienteBanco == null)
            {
                return NotFound($"Nenhum ingrediente de id {id} foi encontrado");
            }

            _context.Ingredientes.Remove(ingredienteBanco);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}