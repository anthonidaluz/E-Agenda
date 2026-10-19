using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.Modulos.Contatos;

namespace eAgenda.Dominio.Modulos.Compromissos;

public sealed class Compromisso : EntidadeBase<Compromisso>
{
    public string Assunto { get; set; } = string.Empty;
    public DateOnly DataOcorrencia { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraTermino { get; set; }
    public TipoCompromisso Tipo { get; set; }
    public string? Local { get; set; }
    public string? Link { get; set; }
    public Contato? Contato { get; set; }

    public Compromisso() { }

    public Compromisso(
        string assunto,
        DateOnly dataOcorrencia,
        TimeOnly horaInicio,
        TimeOnly horaTermino,
        TipoCompromisso tipo,
        string? local,
        string? link,
        Contato? contato
    ) : this()
    {
        Assunto = assunto?.Trim() ?? string.Empty;
        DataOcorrencia = dataOcorrencia;
        HoraInicio = horaInicio;
        HoraTermino = horaTermino;
        Tipo = tipo;
        Contato = contato;

        // Guarda apenas o campo correspondente ao tipo do compromisso
        if (tipo == TipoCompromisso.Presencial)
            Local = string.IsNullOrWhiteSpace(local) ? null : local.Trim();
        else
            Link = string.IsNullOrWhiteSpace(link) ? null : link.Trim();
    }

    public override List<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (string.IsNullOrWhiteSpace(Assunto) || Assunto.Length < 2 || Assunto.Length > 100)
        {
            erros.Add(new ErroValidacao
            {
                Campo = nameof(Assunto),
                Mensagem = "O campo \"Assunto\" deve conter entre 2 e 100 caracteres."
            });
        }

        if (DataOcorrencia == DateOnly.MinValue)
        {
            erros.Add(new ErroValidacao
            {
                Campo = nameof(DataOcorrencia),
                Mensagem = "O campo \"Data de Ocorrência\" deve conter uma data válida."
            });
        }

        if (HoraTermino <= HoraInicio)
        {
            erros.Add(new ErroValidacao
            {
                Campo = nameof(HoraTermino),
                Mensagem = "O campo \"Hora de Término\" deve ser posterior à \"Hora de Início\"."
            });
        }

        if (!Enum.IsDefined(Tipo))
        {
            erros.Add(new ErroValidacao
            {
                Campo = nameof(Tipo),
                Mensagem = "O campo \"Tipo\" deve ser Remoto ou Presencial."
            });
        }

        if (Tipo == TipoCompromisso.Presencial && (string.IsNullOrWhiteSpace(Local) || Local.Length > 200))
        {
            erros.Add(new ErroValidacao
            {
                Campo = nameof(Local),
                Mensagem = "O campo \"Local\" é obrigatório para compromissos presenciais e deve conter no máximo 200 caracteres."
            });
        }

        if (Tipo == TipoCompromisso.Remoto && !LinkValido(Link))
        {
            erros.Add(new ErroValidacao
            {
                Campo = nameof(Link),
                Mensagem = "O campo \"Link\" é obrigatório para compromissos remotos e deve conter um endereço http(s) válido."
            });
        }

        return erros;
    }

    public override void Atualizar(Compromisso entidadeAtualizada)
    {
        Assunto = entidadeAtualizada.Assunto;
        DataOcorrencia = entidadeAtualizada.DataOcorrencia;
        HoraInicio = entidadeAtualizada.HoraInicio;
        HoraTermino = entidadeAtualizada.HoraTermino;
        Tipo = entidadeAtualizada.Tipo;
        Local = entidadeAtualizada.Local;
        Link = entidadeAtualizada.Link;
        Contato = entidadeAtualizada.Contato;
    }

    private static bool LinkValido(string? link)
    {
        if (string.IsNullOrWhiteSpace(link) || link.Length > 500)
            return false;

        return Uri.TryCreate(link, UriKind.Absolute, out Uri? uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
