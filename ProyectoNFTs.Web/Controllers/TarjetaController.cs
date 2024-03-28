using Microsoft.AspNetCore.Mvc;
using ProyectoNFTs.Application.DTOs;
using ProyectoNFTs.Application.Services.Interfaces;
using ProyectoNFTs.Infraestructure.Models;

namespace ProyectoNFTs.Web.Controllers
{
    public class TarjetaController : Controller
    {
        private readonly IServiceTarjeta _serviceTarjeta;

        public TarjetaController(IServiceTarjeta serviceTarjeta)
        {
            _serviceTarjeta = serviceTarjeta;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var collection = await _serviceTarjeta.ListAsync();
            return View(collection);
        }

        // GET: TarjetaController/Create
        public IActionResult Create()
        {
            return View();
        }


        // POST: BodegaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TarjetaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                // Lee del ModelState todos los errores que
                // vienen para el lado del server
                string errors = string.Join("; ", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));
                return BadRequest(errors);
            }

            // Verificar si la tarjeta ya existe en la base de datos
            var existingTarjeta = await _serviceTarjeta.FindByIdAsync(dto.IdTarjeta);
            if (existingTarjeta != null)
            {
                //// La tarjeta ya existe, enviar un mensaje de error
                return Json(new { success = false, idTarjeta = dto.IdTarjeta }); // Incluir el ID en la respuesta
            }

            // La tarjeta no existe, agregarla normalmente
            await _serviceTarjeta.AddAsync(dto);
            return Json(new { success = true }); // Devolver un JSON para indicar que la operación fue exitosa
        }

        // GET: TarjetaController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var @object = await _serviceTarjeta.FindByIdAsync(id);

            return PartialView(@object);
        }

        // GET: TarjetaController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var @object = await _serviceTarjeta.FindByIdAsync(id);

            return View(@object);
        }

        // POST: TarjetaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TarjetaDTO dto)
        {

            await _serviceTarjeta.UpdateAsync(id, dto);

            return RedirectToAction("Index");

        }

        // GET: TarjetaController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            await _serviceTarjeta.DeleteAsync(id);

            return RedirectToAction("Index");
        }

        // POST: TarjetaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            await _serviceTarjeta.DeleteAsync(id);

            return RedirectToAction("Index");
        }

        //public async Task<JsonResult> GetTarjetaById(int id)
        //{
        //    var Tarjeta = await _serviceTarjeta.FindByIdAsync(id);
        //    return Json(Tarjeta);
        //}
    }
}
