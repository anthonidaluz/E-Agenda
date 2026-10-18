using eAgenda.Dominio.Compartilhado;

namespace eAgenda.Dominio.Modulos.Contatos;

public interface IRepositorioContato : IRepositorio<Contato>
{
    bool ExisteContatoPorEmail(string email, Guid? idIgnorado = null);
    bool ExisteContatoPorTelefone(string telefone, Guid? idIgnorado = null);
}
