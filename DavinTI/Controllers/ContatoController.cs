using DavinTI.Data;
using DavinTI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DavinTI.Controllers
{
    public class ContatoController : Controller
    {
        private readonly AgendaContext _context;

        public ContatoController(AgendaContext context)
        {
            _context = context;
        }

        // GET: /Contato/Criar
        public IActionResult Criar()
        {
            return View();
        }

        // POST: /Contato/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Criar(Contato contato)
        {
            if (ModelState.IsValid)
            {
                contato.Telefones ??= new List<Telefone>();
                contato.DataCriacao = DateTime.Now;

                _context.Contatos.Add(contato);
                _context.SaveChanges();

                RegistrarLog("Criado", contato);
                return RedirectToAction("Index");
            }

            return View(contato);
        }

        // GET: /Contato/Index
        public IActionResult Index()
        {
            var contatos = _context.Contatos
                .Include(c => c.Telefones)
                .ToList();

            return View(contatos);
        }

        // GET: /Contato/Editar/{id}
        public IActionResult Editar(long id)
        {
            var contato = _context.Contatos
                .Include(c => c.Telefones)
                .FirstOrDefault(c => c.Id == id);

            if (contato == null)
                return NotFound();

            return View(contato);
        }

        // POST: /Contato/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Contato contato)
        {
            if (ModelState.IsValid)
            {
                contato.Telefones ??= new List<Telefone>();
                contato.DataUltimaAtualizacao = DateTime.Now;

                // Pega os dados antigos para comparação
                var contatoAntigo = _context.Contatos
                    .Include(c => c.Telefones)
                    .AsNoTracking()
                    .FirstOrDefault(c => c.Id == contato.Id);

                _context.Update(contato);
                _context.SaveChanges();

                RegistrarLog("Alterado", contato, contatoAntigo);
                return RedirectToAction("Index");
            }

            return View(contato);
        }

        // GET: /Contato/Deletar/{id}
        public IActionResult Deletar(long id)
        {
            var contato = _context.Contatos
                .Include(c => c.Telefones)
                .FirstOrDefault(c => c.Id == id);

            if (contato == null)
                return NotFound();

            return View(contato);
        }

        // POST: /Contato/Deletar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deletar(Contato contato)
        {
            var contatoBanco = _context.Contatos
                .Include(c => c.Telefones)
                .FirstOrDefault(c => c.Id == contato.Id);

            if (contatoBanco == null)
                return NotFound();

            RegistrarLog("Excluído", contatoBanco);

            _context.Contatos.Remove(contatoBanco);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // AJAX: Filtro parcial
        public IActionResult FiltrarTabela(string? nome, int? idade, string? numero)
        {
            var query = _context.Contatos
                .Include(c => c.Telefones)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(c => c.Nome.Contains(nome));

            if (idade.HasValue)
                query = query.Where(c => c.Idade == idade);

            if (!string.IsNullOrEmpty(numero))
                query = query.Where(c => c.Telefones.Any(t => t.Numero.Contains(numero)));

            return PartialView("_TabelaContatos", query.ToList());
        }

        // Utilitário para registrar logs
        private void RegistrarLog(string acao, Contato contato, Contato? antes = null)
        {
            var caminho = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

            if (!Directory.Exists(caminho))
                Directory.CreateDirectory(caminho);

            var arquivo = Path.Combine(caminho, "log_contatos.txt");

            using (var writer = new StreamWriter(arquivo, append: true))
            {
                writer.WriteLine($"=== {acao.ToUpper()} ===");
                writer.WriteLine($"Data/Hora: {DateTime.Now}");
                writer.WriteLine($"ID: {contato.Id}");
                writer.WriteLine($"Nome: {contato.Nome}");
                writer.WriteLine($"Idade: {contato.Idade}");
                writer.WriteLine("Telefones:");
                foreach (var tel in contato.Telefones)
                    writer.WriteLine($" - {tel.Numero}");

                if (acao == "Alterado" && antes != null)
                {
                    writer.WriteLine(">> Alterações:");

                    if (antes.Nome != contato.Nome)
                        writer.WriteLine($"Nome: {antes.Nome} => {contato.Nome}");

                    if (antes.Idade != contato.Idade)
                        writer.WriteLine($"Idade: {antes.Idade} => {contato.Idade}");

                    var telefonesAntes = antes.Telefones.Select(t => t.Numero).ToList();
                    var telefonesDepois = contato.Telefones.Select(t => t.Numero).ToList();

                    var adicionados = telefonesDepois.Except(telefonesAntes).ToList();
                    var removidos = telefonesAntes.Except(telefonesDepois).ToList();

                    if (adicionados.Any())
                        writer.WriteLine("Telefones adicionados: " + string.Join(", ", adicionados));

                    if (removidos.Any())
                        writer.WriteLine("Telefones removidos: " + string.Join(", ", removidos));
                }

                writer.WriteLine("=========================");
                writer.WriteLine();
            }
        }

        // Permite baixar o log gerado
        public IActionResult BaixarLog()
        {
            var caminho = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log_contatos.txt");

            if (!System.IO.File.Exists(caminho))
                return NotFound("Arquivo de log não encontrado.");

            var bytes = System.IO.File.ReadAllBytes(caminho);
            return File(bytes, "text/plain", "log_contatos.txt");
        }
    }
}
