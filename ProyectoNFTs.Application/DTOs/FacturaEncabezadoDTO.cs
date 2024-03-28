using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoNFTs.Application.DTOs;

public record FacturaEncabezadoDTO
{
    [Required(ErrorMessage = "{0} es requerido")]
    [Display(Name = "No. Factura")]
    public int IdFactura { get; set; }

    [Required(ErrorMessage = "{0} es requerido")]
    [Display(Name = "Tarjeta")]
    public int IdTarjeta { get; set; }

    [Required(ErrorMessage = "{0} es requerido")]
    [Display(Name = "Cliente")]
    public Guid IdCliente { get; set; }

    [Required(ErrorMessage = "{0} es requerido")]
    [Display(Name = "Fecha de Facturación")]
    public DateTime FechaFacturacion { get; set; }

    [Required(ErrorMessage = "{0} es requerido")]
    [Display(Name = "Estado Factura")]
    public int EstadoFactura { get; set; }

    [Required(ErrorMessage = "{0} es requerido")]
    [Display(Name = "No. Tarjeta")]
    public string TarjetaNumero { get; set; } = null!;

    public List<FacturaDetalleDTO> ListFacturaDetalle = null!;
}
