using DavinTI.Data;
using DavinTI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DavinTI.Controllers
{
    public class TelefoneController : Controller
    {
        private readonly AgendaContext _context;

        public TelefoneController(AgendaContext context)
        {
            _context = context;
        }

        // GET: /Telefone/Index
        // Lista todos os telefones com nome do contato
        public IActionResult Index()
        {
            var telefones = _context.Telefones
                .Include(t => t.Contato)
                .ToList();

            ViewBag.Contatos = _context.Contatos.ToList();
            return View(telefones);
        }

        // GET: /Telefone/Criar
        // Formulário para cadastrar telefone
        public IActionResult Criar()
        {
            ViewBag.Contatos = _context.Contatos.ToList();
            return View();
        }

        // POST: /Telefone/Criar
        // Cadastra telefone no banco
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Criar(Telefone telefone)
        {
            if (ModelState.IsValid)
            {
                _context.Telefones.Add(telefone);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Contatos = _context.Contatos.ToList();
            return View(telefone);
        }

        // GET: /Telefone/Editar/id
        // Carrega telefone para edição
        public IActionResult Editar(long id)
        {
            var telefone = _context.Telefones.Find(id);
            if (telefone == null)
                return NotFound();

            ViewBag.Contatos = _context.Contatos.ToList();
            return View(telefone);
        }

        // POST: /Telefone/Editar
        // Salva alterações no telefone
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Telefone telefone)
        {
            if (ModelState.IsValid)
            {
                _context.Update(telefone);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Contatos = _context.Contatos.ToList();
            return View(telefone);
        }

        // GET: /Telefone/Deletar/id
        // Remove telefone do banco
        public IActionResult Deletar(long id)
        {
            var telefone = _context.Telefones.Find(id);
            if (telefone == null)
                return NotFound();

            _context.Telefones.Remove(telefone);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // AJAX: /Telefone/FiltrarTabela
        // Filtra telefones por número e/ou contato
        public IActionResult FiltrarTabela(string? numero, long? idContato)
        {
            var query = _context.Telefones
                .Include(t => t.Contato)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(numero))
                query = query.Where(t => t.Numero.Contains(numero));

            if (idContato.HasValue)
                query = query.Where(t => t.IdContato == idContato);

            return PartialView("_TabelaTelefones", query.ToList());
        }
    }
}
