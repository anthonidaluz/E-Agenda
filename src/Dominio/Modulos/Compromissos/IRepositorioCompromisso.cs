using eAgenda.Dominio.Compartilhado;

namespace eAgenda.Dominio.Modulos.Compromissos;

public interface IRepositorioCompromisso : IRepositorio<Compromisso>
{
    bool ExisteConflitoHorario(DateOnly dataOcorrencia, TimeOnly horaInicio, TimeOnly horaTermino, Guid? idIgnorado = null);
}
