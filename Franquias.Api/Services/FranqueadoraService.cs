using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

public class FranqueadoraService : IFranqueadoraService
{
    private readonly IFranqueadoraRepository _franqueadoraRepository;

    public FranqueadoraService(IFranqueadoraRepository franqueadoraRepository)
    {
        _franqueadoraRepository = franqueadoraRepository;
    }

    public async Task<List<FranqueadoraDto>> ListarAsync()
    {
        var franqueadoras = await _franqueadoraRepository.ListarAsync();
        return franqueadoras.Select(MapearParaDto).ToList();
    }

    public async Task<FranqueadoraDto> CriarAsync(FranqueadoraDto dto)
    {
        if (await _franqueadoraRepository.ExisteCnpjAsync(dto.Cnpj))
        {
            throw new InvalidOperationException("Já existe uma franqueadora cadastrada com este CNPJ.");
        }

        var franqueadora = new Franqueadora
        {
            RazaoSocial = dto.RazaoSocial,
            NomeFantasia = dto.NomeFantasia,
            Cnpj = dto.Cnpj,
            Email = dto.Email,
            Telefone = dto.Telefone,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        await _franqueadoraRepository.AdicionarAsync(franqueadora);
        await _franqueadoraRepository.SalvarAlteracoesAsync();

        return MapearParaDto(franqueadora);
    }

    public async Task<FranqueadoraDto> AtualizarAsync(int id, FranqueadoraDto dto)
    {
        var franqueadora = await _franqueadoraRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Franqueadora não encontrada.");

        if (await _franqueadoraRepository.ExisteCnpjAsync(dto.Cnpj, id))
        {
            throw new InvalidOperationException("Já existe uma franqueadora cadastrada com este CNPJ.");
        }

        franqueadora.RazaoSocial = dto.RazaoSocial;
        franqueadora.NomeFantasia = dto.NomeFantasia;
        franqueadora.Cnpj = dto.Cnpj;
        franqueadora.Email = dto.Email;
        franqueadora.Telefone = dto.Telefone;

        await _franqueadoraRepository.SalvarAlteracoesAsync();

        return MapearParaDto(franqueadora);
    }

    private static FranqueadoraDto MapearParaDto(Franqueadora franqueadora)
    {
        return new FranqueadoraDto
        {
            Id = franqueadora.Id,
            RazaoSocial = franqueadora.RazaoSocial,
            NomeFantasia = franqueadora.NomeFantasia,
            Cnpj = franqueadora.Cnpj,
            Email = franqueadora.Email,
            Telefone = franqueadora.Telefone,
            Ativo = franqueadora.Ativo,
            DataCriacao = franqueadora.DataCriacao
        };
    }
}
