using eAgenda.Dominio.Modulos.Compromissos;

namespace eAgenda.Aplicacao.Modulos.Compromissos;

public record ContatoCompromissoDto(Guid Id, string Nome);

public record CompromissoDto(
    Guid Id,
    string Assunto,
    DateOnly DataOcorrencia,
    TimeOnly HoraInicio,
    TimeOnly HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    ContatoCompromissoDto? Contato
);

public record CadastrarCompromissoDto(
    string Assunto,
    DateOnly DataOcorrencia,
    TimeOnly HoraInicio,
    TimeOnly HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    Guid? ContatoId
);

public record EditarCompromissoDto(
    Guid Id,
    string Assunto,
    DateOnly DataOcorrencia,
    TimeOnly HoraInicio,
    TimeOnly HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    Guid? ContatoId
);
