using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using site_de_receita_api.Data;
using site_de_receita_api.Models;
using site_de_receita_api.Models.DTOs.Create;
using site_de_receita_api.Models.DTOs.Read;


namespace site_de_receita_api.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceitaController : ControllerBase
    {
        private readonly AppDbContext _Context;

        public ReceitaController(AppDbContext appDbContext)
        {
            _Context = appDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReceitaCriacaoDto dto)
        {
            var existe = await _Context.Receitas.AnyAsync(r => r.Nome.ToLower() == dto.Nome.ToLower());

            if (existe)
            {
                return Conflict("Essa receita já foi cadastrada.");
            }
            else if (dto.IngredienteId.Count == 0)
            {
                return BadRequest("O campo ingrediente não pode estar vazio!");
            }

            var receita = new Receita
            {
                FotoUrl = dto.FotoUrl,
                LinkTutorial = dto.LinkTutorial,
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Tipo = dto.Tipo
            };

            _Context.Receitas.Add(receita);
            await _Context.SaveChangesAsync();

            foreach (var ingredienteId in dto.IngredienteId)
            {
                var relacao = new ReceitaIngrediente
                {
                    ReceitaId = receita.Id,
                    IngredienteId = ingredienteId
                };
                _Context.Set<ReceitaIngrediente>().Add(relacao);
            }

            await _Context.SaveChangesAsync();

            var receitaDto = new ReceitaLeituraDto
            {
                Id = receita.Id,
                FotoUrl = receita.FotoUrl,
                LinkTutorial = receita.LinkTutorial,
                Nome = receita.Nome,
                Descricao = receita.Descricao,
                Tipo = receita.Tipo.ToString(),
                Ingredientes = await _Context.ReceitaIngredientes.Where(ri => ri.ReceitaId == receita.Id)
                .Include(ri => ri.Ingrediente).Select(ri => new IngredienteLeituraDto
                    {
                        Id = ri.Ingrediente.Id,
                        Nome = ri.Ingrediente.Nome,
                        FotoUrl = ri.Ingrediente.FotoUrl
                    }).ToListAsync()
            };
            return CreatedAtAction(nameof(FindById), new { id = receita.Id }, receitaDto); ;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Receita>>> FindAll()
        {
            var receitas = await _Context.Receitas.Include(r => r.ReceitaIngredientes)
            .ThenInclude(ri => ri.Ingrediente).Select(r => new ReceitaLeituraDto
            {
                Id = r.Id,
                FotoUrl = r.FotoUrl,
                LinkTutorial = r.LinkTutorial,
                Nome = r.Nome,
                Descricao = r.Descricao,
                Tipo = r.Tipo.ToString(),
                Ingredientes = r.ReceitaIngredientes
                .Select(ri => new IngredienteLeituraDto
                {
                    Id = ri.Ingrediente.Id,
                    Nome = ri.Ingrediente.Nome,
                    FotoUrl = ri.Ingrediente.FotoUrl
                }).ToList()
            }).ToListAsync();

            return Ok(receitas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReceitaLeituraDto>> FindById(int id)
        {
            var receita = await _Context.Receitas.Include(r => r.ReceitaIngredientes)
            .ThenInclude(ri => ri.Ingrediente).Where(r => r.Id == id).Select(r => new ReceitaLeituraDto
            {
                Id = r.Id,
                FotoUrl = r.FotoUrl,
                LinkTutorial = r.LinkTutorial,
                Nome = r.Nome,
                Descricao = r.Descricao,
                Tipo = r.Tipo.ToString(),
                Ingredientes = r.ReceitaIngredientes
                        .Select(ri => new IngredienteLeituraDto
                        {
                            Id = ri.Ingrediente.Id,
                            Nome = ri.Ingrediente.Nome,
                            FotoUrl = ri.Ingrediente.FotoUrl
                        })
                        .ToList()
            })
                .FirstOrDefaultAsync();

            if (receita == null)
            {
                return NotFound($"Receita com id {id} não encontrada.");
            }

            return Ok(receita);
        }

        [HttpGet("por-ingredientes-sugeridos")]
        public async Task<ActionResult<IEnumerable<ReceitaLeituraDto>>> FindByIngredientes([FromQuery] List<int> ingredientesIds)
        {
            if (ingredientesIds == null || ingredientesIds.Count == 0)
            {
                return BadRequest("É necessário informar ao menos um ID de ingrediente.");
            }
                
            var receitas = await _Context.Receitas.Include(r => r.ReceitaIngredientes).ThenInclude(ri => ri.Ingrediente)
                .Where(r => r.ReceitaIngredientes.Any(ri => ingredientesIds.Contains(ri.IngredienteId)))
                .Select(r => new ReceitaLeituraDto
                {
                    Id = r.Id,
                    FotoUrl = r.FotoUrl,
                    LinkTutorial = r.LinkTutorial,
                    Nome = r.Nome,
                    Descricao = r.Descricao,
                    Tipo = r.Tipo.ToString(),
                    Ingredientes = r.ReceitaIngredientes.Select(ri => new IngredienteLeituraDto
                    {
                        Id = ri.Ingrediente.Id,
                        Nome = ri.Ingrediente.Nome,
                        FotoUrl = ri.Ingrediente.FotoUrl
                    }).ToList()
                }).ToListAsync();

            if (!receitas.Any())
            {
                return NotFound("Nenhuma receita encontrada com os ingredientes informados.");
            }

            return Ok(receitas);
        }

        [HttpGet("por-ingredientes-exato")]
        public async Task<ActionResult<IEnumerable<ReceitaLeituraDto>>> FindByIngredientesContidos([FromQuery] List<int> ingredientesIds)
        {
            if (ingredientesIds == null || ingredientesIds.Count == 0)
                return BadRequest("É necessário informar ao menos um ID de ingrediente.");

            var receitas = await _Context.Receitas
                .Include(r => r.ReceitaIngredientes).ThenInclude(ri => ri.Ingrediente)
                .Where(r => r.ReceitaIngredientes.All(ri => ingredientesIds.Contains(ri.IngredienteId)))
                .Select(r => new ReceitaLeituraDto
                {
                    Id = r.Id,
                    FotoUrl = r.FotoUrl,
                    LinkTutorial = r.LinkTutorial,
                    Nome = r.Nome,
                    Descricao = r.Descricao,
                    Tipo = r.Tipo.ToString(),
                    Ingredientes = r.ReceitaIngredientes.Select(ri => new IngredienteLeituraDto
                    {
                        Id = ri.Ingrediente.Id,
                        Nome = ri.Ingrediente.Nome,
                        FotoUrl = ri.Ingrediente.FotoUrl
                    }).ToList()
                })
                .ToListAsync();

            if (!receitas.Any())
                return NotFound("Nenhuma receita encontrada com os ingredientes informados.");

            return Ok(receitas);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReceitaCriacaoDto dto)
        {
            var receita = await _Context.Receitas.Include(r => r.ReceitaIngredientes).FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null)
                return NotFound($"Receita com id {id} não encontrada.");

            receita.FotoUrl = dto.FotoUrl ?? receita.FotoUrl;
            receita.LinkTutorial = dto.LinkTutorial ?? receita.LinkTutorial;
            receita.Nome = dto.Nome ?? receita.Nome;
            receita.Descricao = dto.Descricao ?? receita.Descricao;
            receita.Tipo = dto.Tipo;

            if (dto.IngredienteId != null)
            {
                _Context.ReceitaIngredientes.RemoveRange(receita.ReceitaIngredientes);

                foreach (var ingredienteId in dto.IngredienteId)
                {
                    receita.ReceitaIngredientes.Add(new ReceitaIngrediente
                    {
                        ReceitaId = id,
                        IngredienteId = ingredienteId
                    });
                }
            }

            await _Context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var receitaBanco = await _Context.Receitas.FindAsync(id);
            if (receitaBanco == null)
            {
                return NotFound($"Nenhuma receita com id {id} foi encontrada!");
            }
            _Context.Receitas.Remove(receitaBanco);
            await _Context.SaveChangesAsync();

            return NoContent();
        }
    }
}