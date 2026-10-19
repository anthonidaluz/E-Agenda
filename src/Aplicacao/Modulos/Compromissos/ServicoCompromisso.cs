using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.Modulos.Compromissos;
using eAgenda.Dominio.Modulos.Contatos;
using FluentResults;

namespace eAgenda.Aplicacao.Modulos.Compromissos;

public sealed class ServicoCompromisso(
    IRepositorioCompromisso repositorioCompromisso,
    IRepositorioContato repositorioContato
)
{
    public Result<Guid> Cadastrar(CadastrarCompromissoDto dto)
    {
        List<ErroValidacao> erros = [];

        Contato? contato = SelecionarContato(dto.ContatoId, erros);

        Compromisso compromisso = new(
            dto.Assunto,
            dto.DataOcorrencia,
            dto.HoraInicio,
            dto.HoraTermino,
            dto.Tipo,
            dto.Local,
            dto.Link,
            contato
        );

        erros.AddRange(compromisso.Validar());

        if (erros.Count == 0)
            erros.AddRange(ValidarConflitoHorario(compromisso));

        if (erros.Count > 0)
        {
            List<Error> errosResultado = erros
                .Select(e => new Error(e.Mensagem).WithMetadata("Campo", e.Campo)).ToList();

            return Result.Fail(errosResultado);
        }

        repositorioCompromisso.Cadastrar(compromisso);

        return Result.Ok(compromisso.Id);
    }

    public Result Editar(EditarCompromissoDto dto)
    {
        List<ErroValidacao> erros = [];

        Contato? contato = SelecionarContato(dto.ContatoId, erros);

        Compromisso compromissoAtualizado = new(
            dto.Assunto,
            dto.DataOcorrencia,
            dto.HoraInicio,
            dto.HoraTermino,
            dto.Tipo,
            dto.Local,
            dto.Link,
            contato
        );

        erros.AddRange(compromissoAtualizado.Validar());

        if (erros.Count == 0)
            erros.AddRange(ValidarConflitoHorario(compromissoAtualizado, dto.Id));

        if (erros.Count > 0)
        {
            List<Error> errosResultado = erros
                .Select(e => new Error(e.Mensagem).WithMetadata("Campo", e.Campo)).ToList();

            return Result.Fail(errosResultado);
        }

        bool conseguiuEditar = repositorioCompromisso.Editar(dto.Id, compromissoAtualizado);

        if (!conseguiuEditar)
            return Result.Fail("Não foi possível encontrar o compromisso informado.");

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        bool conseguiuExcluir = repositorioCompromisso.Excluir(id);

        if (!conseguiuExcluir)
            return Result.Fail("Não foi possível encontrar o compromisso informado.");

        return Result.Ok();
    }

    public Result<CompromissoDto> SelecionarPorId(Guid id)
    {
        Compromisso? compromisso = repositorioCompromisso.SelecionarPorId(id);

        if (compromisso == null)
            return Result.Fail("Compromisso não encontrado.");

        return Result.Ok(ConverterParaDto(compromisso));
    }

    public List<CompromissoDto> SelecionarTodos()
    {
        return repositorioCompromisso
            .SelecionarTodos()
            .Select(ConverterParaDto)
            .ToList();
    }

    private Contato? SelecionarContato(Guid? contatoId, List<ErroValidacao> erros)
    {
        if (contatoId == null || contatoId == Guid.Empty)
            return null;

        Contato? contato = repositorioContato.SelecionarPorId(contatoId.Value);

        if (contato == null)
        {
            erros.Add(new ErroValidacao
            {
                Campo = "ContatoId",
                Mensagem = "O contato selecionado não foi encontrado."
            });
        }

        return contato;
    }

    private List<ErroValidacao> ValidarConflitoHorario(Compromisso compromisso, Guid? idIgnorado = null)
    {
        List<ErroValidacao> erros = [];

        bool existeConflito = repositorioCompromisso.ExisteConflitoHorario(
            compromisso.DataOcorrencia,
            compromisso.HoraInicio,
            compromisso.HoraTermino,
            idIgnorado
        );

        if (existeConflito)
        {
            erros.Add(new ErroValidacao
            {
                Campo = nameof(compromisso.HoraInicio),
                Mensagem = "Já existe um compromisso cadastrado neste horário."
            });
        }

        return erros;
    }

    private static CompromissoDto ConverterParaDto(Compromisso compromisso)
    {
        return new CompromissoDto(
            compromisso.Id,
            compromisso.Assunto,
            compromisso.DataOcorrencia,
            compromisso.HoraInicio,
            compromisso.HoraTermino,
            compromisso.Tipo,
            compromisso.Local,
            compromisso.Link,
            compromisso.Contato == null ? null : new ContatoCompromissoDto(compromisso.Contato.Id, compromisso.Contato.Nome)
        );
    }
}
