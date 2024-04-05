using Microsoft.AspNetCore.Mvc;
using ProyectoNFTs.Application.DTOs;
using ProyectoNFTs.Application.Services.Interfaces;
using System.Text.Json;

namespace ProyectoNFTs.Web.Controllers;

public class FacturaController : Controller
{
    private readonly IServiceNft _serviceNft;
    private readonly IServiceTarjeta _serviceTarjeta;
    private readonly IServiceFactura _serviceFactura;
    private readonly IServiceCliente _serviceCliente;

    public FacturaController(IServiceNft serviceNft,
                            IServiceTarjeta serviceTarjeta,
                            IServiceFactura serviceFactura, IServiceCliente serviceCliente)
    {
        _serviceNft = serviceNft;
        _serviceTarjeta = serviceTarjeta;
        _serviceFactura = serviceFactura;
        _serviceCliente = serviceCliente;
    }

    public async Task<IActionResult> Index()
    {

        var nextReceiptNumber = await _serviceFactura.GetNextReceiptNumber();
        ViewBag.CurrentReceipt = nextReceiptNumber;
        var collection = await _serviceTarjeta.ListAsync();
        ViewBag.ListTarjeta = collection;

        // Clear CarShopping
        TempData["CartShopping"] = null;
        TempData.Keep();

        return View();
    }

    public async Task<IActionResult> AddProduct(Guid id, int cantidad)
    {
        FacturaDetalleDTO facturaDetalleDTO = new FacturaDetalleDTO();
        List<FacturaDetalleDTO> lista = new List<FacturaDetalleDTO>();
        string json = "";

        var Nft = await _serviceNft.FindByIdAsync(id);

        // Stock ??
        if (cantidad > Nft.Cantidad)
        {
            return BadRequest("No hay inventario suficiente!");
        }

        facturaDetalleDTO.NombreNFT = Nft.Nombre;
        facturaDetalleDTO.Cantidad = cantidad;
        facturaDetalleDTO.Precio = Nft.Precio;
        facturaDetalleDTO.IdNft = id;
        facturaDetalleDTO.TotalLinea = Convert.ToDecimal(facturaDetalleDTO.Precio * facturaDetalleDTO.Cantidad);
        if (TempData["CartShopping"] == null)
        {
            lista.Add(facturaDetalleDTO);
            // Reenumerate 
            int idx = 1;
            lista.ForEach(p => p.Secuencia = idx++);
            json = JsonSerializer.Serialize(lista);
            TempData["CartShopping"] = json;
        }
        else
        {
            json = (string)TempData["CartShopping"]!;
            lista = JsonSerializer.Deserialize<List<FacturaDetalleDTO>>(json!)!;
            lista.Add(facturaDetalleDTO);
            // Reenumerate 
            int idx = 1;
            lista.ForEach(p => p.Secuencia = idx++);
            json = JsonSerializer.Serialize(lista);
            TempData["CartShopping"] = json;
        }

        TempData.Keep();
        return PartialView("_DetailFactura", lista);
    }

    public IActionResult GetDetailFactura()
    {
        List<FacturaDetalleDTO> lista = new List<FacturaDetalleDTO>();
        string json = "";
        json = (string)TempData["CartShopping"]!;
        lista = JsonSerializer.Deserialize<List<FacturaDetalleDTO>>(json!)!;
        // Reenumerate 
        int idx = 1;
        lista.ForEach(p => p.Secuencia = idx++);
        json = JsonSerializer.Serialize(lista);
        TempData["CartShopping"] = json;
        TempData.Keep();

        return PartialView("_DetailFactura", lista);
    }

    public IActionResult DeleteProduct(int id)
    {
        FacturaDetalleDTO facturaDetalleDTO = new FacturaDetalleDTO();
        List<FacturaDetalleDTO> lista = new List<FacturaDetalleDTO>();
        string json = "";

        if (TempData["CartShopping"] != null)
        {
            json = (string)TempData["CartShopping"]!;
            lista = JsonSerializer.Deserialize<List<FacturaDetalleDTO>>(json!)!;
            // Remove from list by Index
            int idx = lista.FindIndex(p => p.Secuencia == id);
            lista.RemoveAt(idx);
            json = JsonSerializer.Serialize(lista);
            TempData["CartShopping"] = json;
        }

        TempData.Keep();

        // return Content("Ok");
        return PartialView("_DetailFactura", lista);

    }

    public async Task<IActionResult> Create(FacturaEncabezadoDTO facturaEncabezadoDTO)
    {
        string json;
        try
        {
            // IdClient exist?
            var cliente = await _serviceCliente.FindByIdAsync(facturaEncabezadoDTO.IdCliente);
            if (cliente == null)
            {
                // Keep Cache data
                TempData.Keep();
                return BadRequest("Cliente No Existe!");
            }


            // TODO: Validate! 
            if (!ModelState.IsValid)
            {

            }

            json = (string)TempData["CartShopping"]!;

            if (string.IsNullOrEmpty(json))
            {
                return BadRequest("No hay datos por facturar");
            }

            var lista = JsonSerializer.Deserialize<List<FacturaDetalleDTO>>(json!)!;

            //Mismo numero de factura para el detalle FK
            lista.ForEach(p => p.IdFactura = facturaEncabezadoDTO.IdFactura);
            facturaEncabezadoDTO.ListFacturaDetalle = lista;

            await _serviceFactura.AddAsync(facturaEncabezadoDTO);

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // Keep Cache data
            TempData.Keep();
            return BadRequest(ex.Message);
        }
    }
}

