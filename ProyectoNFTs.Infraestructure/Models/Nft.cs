using System;
using System.Collections.Generic;

namespace ProyectoNFTs.Infraestructure.Models;

public partial class Nft
{
    public Guid IdNft { get; set; }

    public string Nombre { get; set; } = null!;

    public int Autor { get; set; }

    public decimal Precio { get; set; }

    public int Cantidad { get; set; }

    public byte[] Imagen { get; set; } = null!;

    public virtual ICollection<FacturaDetalle> FacturaDetalle { get; set; } = new List<FacturaDetalle>();

    public virtual ICollection<Cliente> IdCliente { get; set; } = new List<Cliente>();
}
