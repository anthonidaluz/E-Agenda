using eAgenda.Aplicacao.Modulos.Compromissos;
using eAgenda.Aplicacao.Modulos.Contatos;
using eAgenda.Dominio.Modulos.Compromissos;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Modulos.Compromissos;

public sealed class CompromissoController(
    ServicoCompromisso servicoCompromisso,
    ServicoContato servicoContato
) : Controller
{
    [HttpGet]
    public ActionResult Listar()
    {
        List<CompromissoDto> compromissos = servicoCompromisso.SelecionarTodos();

        return View(compromissos);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarCompromissoViewModel viewModel = new(
            string.Empty,
            DateOnly.FromDateTime(DateTime.Now),
            new TimeOnly(9, 0),
            new TimeOnly(10, 0),
            TipoCompromisso.Remoto,
            null,
            null,
            null
        );

        return View(viewModel with { ContatosDisponiveis = SelecionarContatosDisponiveis() });
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarCompromissoViewModel viewModel)
    {
        CadastrarCompromissoDto dto = new(
            viewModel.Assunto ?? string.Empty,
            viewModel.DataOcorrencia,
            viewModel.HoraInicio,
            viewModel.HoraTermino,
            viewModel.Tipo,
            viewModel.Local,
            viewModel.Link,
            viewModel.ContatoId
        );

        Result<Guid> resultado = servicoCompromisso.Cadastrar(dto);

        if (resultado.IsFailed)
        {
            foreach (IError erro in resultado.Errors)
            {
                string campo = erro.Metadata["Campo"].ToString() ?? string.Empty;

                ModelState.AddModelError(campo, erro.Message);
            }

            return View(viewModel with { ContatosDisponiveis = SelecionarContatosDisponiveis() });
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Editar(Guid id)
    {
        Result<CompromissoDto> resultado = servicoCompromisso.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData["MensagemErro"] = resultado.Errors.First().Message;

            return RedirectToAction(nameof(Listar));
        }

        CompromissoDto dto = resultado.Value;

        EditarCompromissoViewModel viewModel = new(
            dto.Id,
            dto.Assunto,
            dto.DataOcorrencia,
            dto.HoraInicio,
            dto.HoraTermino,
            dto.Tipo,
            dto.Local,
            dto.Link,
            dto.Contato?.Id
        );

        return View(viewModel with { ContatosDisponiveis = SelecionarContatosDisponiveis() });
    }

    [HttpPost]
    public ActionResult Editar(EditarCompromissoViewModel viewModel)
    {
        EditarCompromissoDto dto = new(
            viewModel.Id,
            viewModel.Assunto ?? string.Empty,
            viewModel.DataOcorrencia,
            viewModel.HoraInicio,
            viewModel.HoraTermino,
            viewModel.Tipo,
            viewModel.Local,
            viewModel.Link,
            viewModel.ContatoId
        );

        Result resultado = servicoCompromisso.Editar(dto);

        if (resultado.IsFailed)
        {
            if (!resultado.Errors.Any(e => e.HasMetadataKey("Campo")))
            {
                TempData["MensagemErro"] = resultado.Errors.First().Message;

                return RedirectToAction(nameof(Listar));
            }

            foreach (IError erro in resultado.Errors)
            {
                string campo = erro.Metadata["Campo"].ToString() ?? string.Empty;

                ModelState.AddModelError(campo, erro.Message);
            }

            return View(viewModel with { ContatosDisponiveis = SelecionarContatosDisponiveis() });
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Excluir(Guid id)
    {
        Result<CompromissoDto> resultado = servicoCompromisso.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData["MensagemErro"] = resultado.Errors.First().Message;

            return RedirectToAction(nameof(Listar));
        }

        CompromissoDto dto = resultado.Value;

        ExcluirCompromissoViewModel viewModel = new(
            dto.Id,
            dto.Assunto,
            dto.DataOcorrencia,
            dto.HoraInicio,
            dto.HoraTermino
        );

        return View(viewModel);
    }

    [HttpPost]
    public ActionResult Excluir(ExcluirCompromissoViewModel viewModel)
    {
        Result resultado = servicoCompromisso.Excluir(viewModel.Id);

        if (resultado.IsFailed)
            TempData["MensagemErro"] = resultado.Errors.First().Message;

        return RedirectToAction(nameof(Listar));
    }

    private List<ContatoCompromissoDto> SelecionarContatosDisponiveis()
    {
        return servicoContato
            .SelecionarTodos()
            .Select(c => new ContatoCompromissoDto(c.Id, c.Nome))
            .ToList();
    }
}
