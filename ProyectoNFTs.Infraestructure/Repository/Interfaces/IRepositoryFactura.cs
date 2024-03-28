using ProyectoNFTs.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoNFTs.Infraestructure.Repository.Interfaces;

public interface IRepositoryFactura
{
    Task<int> AddAsync(FacturaEncabezado entity);
    Task<FacturaEncabezado> FindByIdAsync(int id);
    Task<int> GetNextReceiptNumber();

}
