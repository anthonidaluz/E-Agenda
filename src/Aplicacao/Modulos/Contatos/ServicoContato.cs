using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.Modulos.Contatos;
using FluentResults;

namespace eAgenda.Aplicacao.Modulos.Contatos;

public sealed class ServicoContato(IRepositorioContato repositorioContato)
{
    public Result<Guid> Cadastrar(CadastrarContatoDto dto)
    {
        Contato contato = new(dto.Nome, dto.Email, dto.Telefone, dto.Cargo, dto.Empresa);

        List<ErroValidacao> erros = contato.Validar();

        erros.AddRange(ValidarDuplicidade(contato));

        if (erros.Count > 0)
        {
            List<Error> errosResultado = erros
                .Select(e => new Error(e.Mensagem).WithMetadata("Campo", e.Campo)).ToList();

            return Result.Fail(errosResultado);
        }

        repositorioContato.Cadastrar(contato);

        return Result.Ok(contato.Id);
    }

    public Result Editar(EditarContatoDto dto)
    {
        Contato contatoAtualizado = new(dto.Nome, dto.Email, dto.Telefone, dto.Cargo, dto.Empresa);

        List<ErroValidacao> erros = contatoAtualizado.Validar();

        erros.AddRange(ValidarDuplicidade(contatoAtualizado, dto.Id));

        if (erros.Count > 0)
        {
            List<Error> errosResultado = erros
                .Select(e => new Error(e.Mensagem).WithMetadata("Campo", e.Campo)).ToList();

            return Result.Fail(errosResultado);
        }

        bool conseguiuEditar = repositorioContato.Editar(dto.Id, contatoAtualizado);

        if (!conseguiuEditar)
            return Result.Fail("Não foi possível encontrar o contato informado.");

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Contato? contato = repositorioContato.SelecionarPorId(id);

        if (contato == null)
            return Result.Fail("Não foi possível encontrar o contato informado.");

        if (contato.Compromissos.Count > 0)
            return Result.Fail("Não é possível excluir um contato com compromissos vinculados.");

        repositorioContato.Excluir(id);

        return Result.Ok();
    }

    public Result<ContatoDto> SelecionarPorId(Guid id)
    {
        Contato? contato = repositorioContato.SelecionarPorId(id);

        if (contato == null)
            return Result.Fail("Contato não encontrado.");

        return Result.Ok(ConverterParaDto(contato));
    }

    public List<ContatoDto> SelecionarTodos()
    {
        return repositorioContato
            .SelecionarTodos()
            .Select(ConverterParaDto)
            .ToList();
    }

    private List<ErroValidacao> ValidarDuplicidade(Contato contato, Guid? idIgnorado = null)
    {
        List<ErroValidacao> erros = [];

        if (repositorioContato.ExisteContatoPorEmail(contato.Email, idIgnorado))
        {
            erros.Add(new ErroValidacao
            {
                Campo = nameof(contato.Email),
                Mensagem = "Já existe um contato cadastrado com o email informado."
            });
        }

        if (repositorioContato.ExisteContatoPorTelefone(contato.Telefone, idIgnorado))
        {
            erros.Add(new ErroValidacao
            {
                Campo = nameof(contato.Telefone),
                Mensagem = "Já existe um contato cadastrado com o telefone informado."
            });
        }

        return erros;
    }

    private static ContatoDto ConverterParaDto(Contato contato)
    {
        return new ContatoDto(
            contato.Id,
            contato.Nome,
            contato.Email,
            contato.Telefone,
            contato.Cargo,
            contato.Empresa
        );
    }
}
