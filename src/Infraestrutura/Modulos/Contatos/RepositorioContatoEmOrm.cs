using eAgenda.Dominio.Modulos.Contatos;
using eAgenda.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infraestrutura.Modulos.Contatos;

public sealed class RepositorioContatoEmOrm(EAgendaDbContext dbContext) :
    RepositorioBaseEmOrm<Contato>(dbContext), IRepositorioContato
{
    public override Contato? SelecionarPorId(Guid idSelecionado)
    {
        return registros
            .Include(c => c.Compromissos)
            .SingleOrDefault(c => c.Id == idSelecionado);
    }

    public override List<Contato> SelecionarTodos()
    {
        return registros
            .OrderBy(c => c.Nome)
            .ToList();
    }

    public bool ExisteContatoPorEmail(string email, Guid? idIgnorado = null)
    {
        return registros.Any(c =>
            c.Id != idIgnorado &&
            c.Email.ToLower() == email.ToLower()
        );
    }

    public bool ExisteContatoPorTelefone(string telefone, Guid? idIgnorado = null)
    {
        return registros.Any(c =>
            c.Id != idIgnorado &&
            c.Telefone == telefone
        );
    }
}
