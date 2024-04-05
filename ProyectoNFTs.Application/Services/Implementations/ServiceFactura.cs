using AutoMapper;
using ProyectoNFTs.Application.Config;
using ProyectoNFTs.Application.DTOs;
using ProyectoNFTs.Application.Services.Interfaces;
using ProyectoNFTs.Infraestructure.Models;
using ProyectoNFTs.Infraestructure.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace ProyectoNFTs.Application.Services.Implementations;

public class ServiceFactura : IServiceFactura
{
    private readonly IRepositoryFactura _repositoryFactura;
    private readonly IRepositoryCliente _repositoryCliente;
    private readonly IRepositoryNft _repositoryNFT;
    private readonly IMapper _mapper;
    private readonly IOptions<AppConfig> _options;
    private readonly ILogger<ServiceFactura> _logger;
    public ServiceFactura(IRepositoryFactura repositoryFactura,
                          IRepositoryCliente repositoryCliente,
                          IRepositoryNft repositoryNFT,
                          IMapper mapper,
                          IOptions<AppConfig> options,
                          ILogger<ServiceFactura> logger)
    {
        _repositoryFactura = repositoryFactura;
        _repositoryCliente = repositoryCliente;
        _repositoryNFT = repositoryNFT;
        _mapper = mapper;
        _options = options;
        _logger = logger;
    }

    public async Task<int> AddAsync(FacturaEncabezadoDTO dto)
    {
        // Validate Stock availability
        foreach (var item in dto.ListFacturaDetalle)
        {
            var nft = await _repositoryNFT.FindByIdAsync(item.IdNft);

            if (nft.Cantidad - item.Cantidad < 0)
            {
                throw new BadHttpRequestException($"No hay stock para el nft {nft.Nombre}, cantidad en stock {nft.Cantidad} ");
             
            }
        }

        var @object = _mapper.Map<FacturaEncabezado>(dto);
        var cliente = await _repositoryCliente.FindByIdAsync(dto.IdCliente);
        // Send email
        //await SendEmail(cliente!.Email!);
        return await _repositoryFactura.AddAsync(@object);
    }

    public async Task<int> GetNextReceiptNumber()
    {
        int nextReceipt = await _repositoryFactura.GetNextReceiptNumber();
        return nextReceipt + 1;
    }

    /// <summary>
    /// Send email 
    /// </summary>
    /// <param name="email"></param>
    private async Task<bool> SendEmail(string email)
    {

        if (_options.Value.SmtpConfiguration == null)
        {
            _logger.LogError($"No se encuentra configurado ningun valor para SMPT en {MethodBase.GetCurrentMethod()!.DeclaringType!.FullName}");
            return false;
        }
        if (string.IsNullOrEmpty(_options.Value.SmtpConfiguration.UserName) || string.IsNullOrEmpty(_options.Value.SmtpConfiguration.FromName))
        {
            _logger.LogError($"No se encuentra configurado UserName o FromName en appSettings.json (Dev | Prod) {MethodBase.GetCurrentMethod()!.DeclaringType!.FullName}");
            return false;
        }
        var mailMessage = new MailMessage(
                new MailAddress(_options.Value.SmtpConfiguration.UserName, _options.Value.SmtpConfiguration.FromName),
                new MailAddress(email))
        {
            Subject = "Factura Electrónica para " + email,
            Body = "Adjunto Factura Electronica de Electronics",
            IsBodyHtml = true
        };
        //Attachment attachment = new Attachment(@"c:\\temp\\factura.pdf");
        //mailMessage.Attachments.Add(attachment);
        using var smtpClient = new SmtpClient(_options.Value.SmtpConfiguration.Server,
                                              _options.Value.SmtpConfiguration.PortNumber)
        {
            Credentials = new NetworkCredential(_options.Value.SmtpConfiguration.UserName,
                                                _options.Value.SmtpConfiguration.Password),
            EnableSsl = _options.Value.SmtpConfiguration.EnableSsl,
        };
        await smtpClient.SendMailAsync(mailMessage);
        return true;
    }
}