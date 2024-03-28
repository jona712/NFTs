using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoNFTs.Application.DTOs;

public record FacturaDetalleDTO
{
    [Required(ErrorMessage = "{0} es requerido")]
    [Display(Name = "No. Factura")]
    public int IdFactura { get; set; }

    [Required(ErrorMessage = "{0} es requerido")]
    [Display(Name = "Código")]
    public int Secuencia { get; set; }

    [Required(ErrorMessage = "{0} es requerido")]
    [Display(Name = "NFT")]
    public Guid IdNft { get; set; }

    
    [Display(Name = "Nombre NFT")]
    public string NombreNFT { get; set; } = default!;

    [Required(ErrorMessage = "{0} es requerido")]
    [Display(Name = "Precio")]
    public decimal? Precio { get; set; }

    [Required(ErrorMessage = "{0} es requerido")]
    [Display(Name = "Cantidad")]
    public int? Cantidad { get; set; }

    [DisplayFormat(DataFormatString = "{0:n2}")]
    [Display(Name = "Total")]
    public decimal TotalLinea { get; set; }
}
