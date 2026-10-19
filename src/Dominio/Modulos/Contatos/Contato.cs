using System.Net.Mail;
using System.Text.RegularExpressions;
using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.Modulos.Compromissos;

namespace eAgenda.Dominio.Modulos.Contatos;

public sealed class Contato : EntidadeBase<Contato>
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Cargo { get; set; }
    public string? Empresa { get; set; }
    public List<Compromisso> Compromissos { get; set; } = [];

    public Contato() { }

    public Contato(string nome, string email, string telefone, string? cargo, string? empresa) : this()
    {
        Nome = nome?.Trim() ?? string.Empty;
        Email = email?.Trim() ?? string.Empty;
        Telefone = telefone?.Trim() ?? string.Empty;
        Cargo = string.IsNullOrWhiteSpace(cargo) ? null : cargo.Trim();
        Empresa = string.IsNullOrWhiteSpace(empresa) ? null : empresa.Trim();
    }

    public override List<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (string.IsNullOrWhiteSpace(Nome) || Nome.Length < 2 || Nome.Length > 100)
        {
            erros.Add(new ErroValidacao
            {
                Campo = nameof(Nome),
                Mensagem = "O campo \"Nome\" deve conter entre 2 e 100 caracteres."
            });
        }

        if (!EmailValido(Email))
        {
            erros.Add(new ErroValidacao
            {
                Campo = nameof(Email),
                Mensagem = "O campo \"Email\" deve conter um endereço de email válido."
            });
        }

        if (string.IsNullOrWhiteSpace(Telefone) || !Regex.IsMatch(Telefone, @"^\(\d{2}\) \d{4,5}-\d{4}$"))
        {
            erros.Add(new ErroValidacao
            {
                Campo = nameof(Telefone),
                Mensagem = "O campo \"Telefone\" deve seguir o formato (XX) XXXX-XXXX ou (XX) XXXXX-XXXX."
            });
        }

        if (Cargo?.Length > 100)
        {
            erros.Add(new ErroValidacao
            {
                Campo = nameof(Cargo),
                Mensagem = "O campo \"Cargo\" deve conter no máximo 100 caracteres."
            });
        }

        if (Empresa?.Length > 100)
        {
            erros.Add(new ErroValidacao
            {
                Campo = nameof(Empresa),
                Mensagem = "O campo \"Empresa\" deve conter no máximo 100 caracteres."
            });
        }

        return erros;
    }

    public override void Atualizar(Contato entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome;
        Email = entidadeAtualizada.Email;
        Telefone = entidadeAtualizada.Telefone;
        Cargo = entidadeAtualizada.Cargo;
        Empresa = entidadeAtualizada.Empresa;
    }

    private static bool EmailValido(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Length > 254)
            return false;

        if (!MailAddress.TryCreate(email, out MailAddress? endereco))
            return false;

        // MailAddress aceita "nome <email>" e emails sem domínio com ponto, por isso a conferência extra
        return endereco.Address == email && endereco.Host.Contains('.');
    }
}
