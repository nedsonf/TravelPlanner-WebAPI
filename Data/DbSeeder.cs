using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Models;
using TravelPlanner.Api.Security;

namespace TravelPlanner.Api.Data;

public static class DbSeeder
{
    private const string DemoEmail = "viajante@demo.com";
    private const string DemoSenha = "Senha@123";
    private static readonly string[] RolesPadrao = ["Viajante", "Guia", "Admin"];
    private static readonly string[] CategoriasDespesaPadrao = ["Hospedagem", "Alimentacao", "Transporte"];

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedRolesAsync(context);
        await SeedCategoriasDespesaAsync(context);
        await SeedDadosDemoAsync(context);
        await SeedUsuariosDemoAsync(context);
        await SeedCatalogoDestinosAsync(context);
        await SeedPontosAdicionaisAsync(context);
        await SeedPacotesDemoAsync(context);
        await SeedAvaliacoesGuiaDemoAsync(context);
    }

    private static async Task SeedAvaliacoesGuiaDemoAsync(ApplicationDbContext context)
    {
        if (await context.AvaliacoesGuias.AnyAsync())
            return;

        var guia = await context.Guias
            .Include(g => g.Usuario)
            .FirstOrDefaultAsync(g => g.Usuario.Email == "guia@demo.com");

        var viajante = await context.Viajantes
            .Include(v => v.Usuario)
            .FirstOrDefaultAsync(v => v.Usuario.Email == DemoEmail);

        if (guia is null || viajante is null)
            return;

        var destino = await context.Destinos.FirstOrDefaultAsync();
        if (destino is null)
            return;

        var pacote = await context.Pacotes.FirstOrDefaultAsync(p => p.GuiaId == guia.Id);

        var viagem = new Viagem
        {
            ViajanteId = viajante.Id,
            DestinoId = destino.Id,
            DataInicio = DateTime.UtcNow.AddDays(-60),
            DataFim = DateTime.UtcNow.AddDays(-55),
            Status = "Pago"
        };
        context.Viagens.Add(viagem);
        await context.SaveChangesAsync();

        var reserva = new Reserva
        {
            ViagemId = viagem.Id,
            GuiaId = guia.Id,
            PacoteId = pacote?.Id,
            Status = "Pago",
            ValorTotal = pacote?.Preco ?? 1500m,
            TaxaPlataforma = 150m,
            ValorLiquidoGuia = 1350m
        };
        context.Reservas.Add(reserva);
        await context.SaveChangesAsync();

        context.AvaliacoesGuias.Add(new AvaliacaoGuia
        {
            GuiaId = guia.Id,
            ViajanteId = viajante.Id,
            ReservaId = reserva.Id,
            Nota = 5,
            Comentario = "Guia muito atencioso, roteiro impecável!",
            CriadoEm = DateTime.UtcNow.AddDays(-50)
        });

        guia.Rating = 5.0m;
        await context.SaveChangesAsync();
    }

    private sealed record PontoCatalogo(string Nome, string Categoria, double LatOffset = 0, double LonOffset = 0);

    private sealed record DestinoCatalogo(
        string Cidade,
        string Pais,
        double Lat,
        double Lon,
        string Hotel,
        string Endereco,
        PontoCatalogo[] Pontos);

    private static async Task SeedCatalogoDestinosAsync(ApplicationDbContext context)
    {
        var catalogo = new[]
        {
            new DestinoCatalogo("Rio de Janeiro", "Brasil", -22.9068, -43.1729,
                "Hotel Copacabana Mar", "Av. Atlântica, Copacabana",
                [
                    new("Cristo Redentor", "Monumento", 0.02, 0.01),
                    new("Pão de Açúcar", "Natureza", -0.01, -0.03),
                    new("Praia de Ipanema", "Praia", -0.02, -0.02),
                    new("Jardim Botânico", "Parque", 0.01, -0.01)
                ]),
            new DestinoCatalogo("Gramado", "Brasil", -29.3789, -50.8740,
                "Pousada Serra Nevada", "Centro, Gramado - RS",
                [
                    new("Lago Negro", "Natureza"),
                    new("Rua Coberta", "Passeio", 0.001, 0.002),
                    new("Mini Mundo", "Parque", -0.002, 0.001),
                    new("Snowland", "Parque", 0.003, -0.001)
                ]),
            new DestinoCatalogo("Fortaleza", "Brasil", -3.7319, -38.5267,
                "Resort Praia do Futuro", "Praia do Futuro, Fortaleza - CE",
                [
                    new("Praia do Futuro", "Praia"),
                    new("Beach Park", "Parque", 0.05, -0.02),
                    new("Centro Dragão do Mar", "Cultura", 0.01, 0.01),
                    new("Mercado Central", "Gastronomia", 0.002, 0.003)
                ]),
            new DestinoCatalogo("Salvador", "Brasil", -12.9777, -38.5016,
                "Hotel Bahia Othon", "Ondina, Salvador - BA",
                [
                    new("Pelourinho", "Histórico"),
                    new("Elevador Lacerda", "Monumento", 0.002, 0.001),
                    new("Farol da Barra", "Monumento", -0.01, -0.02),
                    new("Praia do Porto da Barra", "Praia", -0.008, -0.015)
                ]),
            new DestinoCatalogo("Florianópolis", "Brasil", -27.5954, -48.5480,
                "Hotel Majestic Palace", "Centro, Florianópolis - SC",
                [
                    new("Praia Mole", "Praia", 0.02, 0.03),
                    new("Lagoa da Conceição", "Natureza", 0.015, 0.025),
                    new("Morro da Cruz", "Mirante", 0.005, 0.01),
                    new("Mercado Público", "Gastronomia", -0.001, 0.002)
                ]),
            new DestinoCatalogo("Foz do Iguaçu", "Brasil", -25.5163, -54.5854,
                "Hotel Belmond das Cataratas", "Rod. Br-469, Foz do Iguaçu - PR",
                [
                    new("Cataratas do Iguaçu", "Natureza", 0.03, -0.02),
                    new("Marco das Três Fronteiras", "Monumento", -0.01, -0.03),
                    new("Parque das Aves", "Parque", 0.02, -0.015),
                    new("Itaipu Binacional", "Engenharia", 0.04, 0.01)
                ]),
            new DestinoCatalogo("Manaus", "Brasil", -3.1190, -60.0217,
                "Hotel Tropical Manaus", "Av. Coronel Teixeira, Manaus - AM",
                [
                    new("Encontro das Águas", "Natureza", 0.08, -0.05),
                    new("Teatro Amazonas", "Cultura"),
                    new("Mercado Adolpho Lisboa", "Gastronomia", 0.001, 0.002),
                    new("Floresta Nacional do Tapajós", "Ecoturismo", 0.1, 0.05)
                ]),
            new DestinoCatalogo("São Paulo", "Brasil", -23.5505, -46.6333,
                "Hotel Unique", "Av. Brigadeiro Luís Antônio, São Paulo - SP",
                [
                    new("MASP", "Cultura", 0.01, -0.01),
                    new("Parque Ibirapuera", "Parque", 0.02, -0.02),
                    new("Mercado Municipal", "Gastronomia", -0.005, 0.01),
                    new("Avenida Paulista", "Passeio", 0.008, -0.005)
                ]),
            new DestinoCatalogo("Recife", "Brasil", -8.0476, -34.8770,
                "Hotel Atlante Plaza", "Boa Viagem, Recife - PE",
                [
                    new("Marco Zero", "Histórico"),
                    new("Praia de Boa Viagem", "Praia", -0.01, -0.005),
                    new("Instituto Ricardo Brennand", "Cultura", 0.02, -0.01),
                    new("Olinda (centro histórico)", "Histórico", 0.015, 0.01)
                ]),
            new DestinoCatalogo("Maceió", "Brasil", -9.6658, -35.7353,
                "Hotel Ritz Lagoa da Anta", "Lagoa da Anta, Maceió - AL",
                [
                    new("Praia de Pajuçara", "Praia", -0.005, -0.01),
                    new("Piscinas Naturais de Pajuçara", "Natureza", -0.02, -0.03),
                    new("Pontal do Coruripe", "Praia", 0.08, -0.05),
                    new("Mercado do Artesanato", "Cultura", 0.001, 0.002)
                ]),
            new DestinoCatalogo("Brasília", "Brasil", -15.7939, -47.8828,
                "Hotel Nacional", "Setor Hoteleiro Sul, Brasília - DF",
                [
                    new("Congresso Nacional", "Arquitetura"),
                    new("Catedral de Brasília", "Religioso", 0.005, -0.003),
                    new("Pontão do Lago Sul", "Passeio", 0.01, 0.02),
                    new("Memorial JK", "Histórico", -0.008, -0.01)
                ]),
            new DestinoCatalogo("Curitiba", "Brasil", -25.4284, -49.2733,
                "Hotel Bourbon Curitiba", "Rua Cândido de Abreu, Curitiba - PR",
                [
                    new("Jardim Botânico", "Parque", 0.01, 0.005),
                    new("Ópera de Arame", "Cultura", 0.02, -0.01),
                    new("Rua das Flores", "Passeio", 0.001, 0.001),
                    new("Parque Tanguá", "Parque", 0.015, -0.02)
                ]),
            new DestinoCatalogo("Bonito", "Brasil", -21.1261, -56.4816,
                "Hotel Zagaia Eco Resort", "Centro, Bonito - MS",
                [
                    new("Gruta do Lago Azul", "Natureza", 0.01, 0.02),
                    new("Rio da Prata (flutuação)", "Ecoturismo", 0.005, 0.01),
                    new("Buraco das Araras", "Natureza", 0.02, -0.01),
                    new("Balneário Municipal", "Natureza", -0.002, 0.003)
                ]),
            new DestinoCatalogo("Ouro Preto", "Brasil", -20.3855, -43.5035,
                "Pousada do Mondego", "Centro Histórico, Ouro Preto - MG",
                [
                    new("Igreja de São Francisco de Assis", "Religioso"),
                    new("Mina da Passagem", "Histórico", 0.005, 0.008),
                    new("Praça Tiradentes", "Histórico", 0.001, 0.001),
                    new("Museu da Inconfidência", "Cultura", 0.002, -0.001)
                ]),
            new DestinoCatalogo("Fernando de Noronha", "Brasil", -3.8548, -32.4239,
                "Pousada Maravilha", "Vila dos Remédios, Fernando de Noronha - PE",
                [
                    new("Baía do Sancho", "Praia", 0.01, -0.02),
                    new("Morro do Pico", "Mirante", 0.005, 0.01),
                    new("Atalaia", "Natureza", -0.008, 0.005),
                    new("Projeto Tamar", "Ecoturismo", 0.003, 0.002)
                ]),
            new DestinoCatalogo("Natal", "Brasil", -5.7945, -35.2110,
                "Hotel Praiamar Natal", "Ponta Negra, Natal - RN",
                [
                    new("Praia de Ponta Negra", "Praia", -0.01, -0.01),
                    new("Forte dos Reis Magos", "Histórico", 0.02, -0.02),
                    new("Dunas de Genipabu", "Natureza", 0.05, -0.03),
                    new("Parrachos de Maracajaú", "Natureza", 0.08, -0.04)
                ]),
            new DestinoCatalogo("Porto Seguro", "Brasil", -16.4435, -39.0643,
                "Resort Porto Seguro Praia", "Arraial d'Ajuda, Porto Seguro - BA",
                [
                    new("Passarela do Álcool", "Passeio"),
                    new("Praia do Mutá", "Praia", 0.005, -0.01),
                    new("Quadrado de Trancoso", "Histórico", 0.03, -0.02),
                    new("Recife de Coroa Vermelha", "Natureza", 0.01, -0.015)
                ]),
            new DestinoCatalogo("Jericoacoara", "Brasil", -2.7925, -40.5136,
                "Pousada Vila Kalango", "Jericoacoara, Jijoca de Jericoacoara - CE",
                [
                    new("Duna do Pôr do Sol", "Natureza"),
                    new("Pedra Furada", "Natureza", 0.01, -0.01),
                    new("Lagoa do Paraíso", "Natureza", 0.02, 0.01),
                    new("Praia de Jericoacoara", "Praia", -0.005, -0.008)
                ]),
            new DestinoCatalogo("Buenos Aires", "Argentina", -34.6037, -58.3816,
                "Hotel Alvear Palace", "Av. Alvear, Recoleta",
                [
                    new("Casa Rosada", "Histórico"),
                    new("Caminito (La Boca)", "Cultura", 0.02, 0.01),
                    new("Teatro Colón", "Cultura", 0.005, -0.003),
                    new("Recoleta Cemetery", "Histórico", 0.01, -0.01)
                ]),
            new DestinoCatalogo("Lisboa", "Portugal", 38.7223, -9.1393,
                "Hotel Avenida Palace", "Rua 1º de Dezembro, Lisboa",
                [
                    new("Torre de Belém", "Monumento", -0.02, -0.03),
                    new("Alfama", "Histórico", 0.005, 0.01),
                    new("Mosteiro dos Jerónimos", "Religioso", -0.025, -0.035),
                    new("Miradouro da Senhora do Monte", "Mirante", 0.01, 0.015)
                ]),
            new DestinoCatalogo("Santiago", "Chile", -33.4489, -70.6693,
                "Hotel Plaza San Francisco", "Av. Libertador Bernardo O'Higgins",
                [
                    new("Cerro San Cristóbal", "Mirante", 0.02, 0.01),
                    new("Plaza de Armas", "Histórico"),
                    new("Mercado Central", "Gastronomia", 0.005, -0.005),
                    new("Valle Nevado (ski)", "Natureza", 0.15, 0.08)
                ])
        };

        foreach (var item in catalogo)
            await EnsureDestinoCatalogoAsync(context, item);
    }

    private static async Task EnsureDestinoCatalogoAsync(ApplicationDbContext context, DestinoCatalogo item)
    {
        if (await context.Destinos.AnyAsync(d => d.Cidade == item.Cidade))
            return;

        var destino = new Destino
        {
            Cidade = item.Cidade,
            Pais = item.Pais,
            Latitude = item.Lat,
            Longitude = item.Lon
        };
        context.Destinos.Add(destino);
        await context.SaveChangesAsync();

        context.Hospedagens.Add(new Hospedagem
        {
            DestinoId = destino.Id,
            Nome = item.Hotel,
            Endereco = item.Endereco,
            CheckIn = DateTime.UtcNow.AddDays(30),
            CheckOut = DateTime.UtcNow.AddDays(37)
        });

        foreach (var ponto in item.Pontos)
        {
            context.PontosTuristicos.Add(new PontoTuristico
            {
                DestinoId = destino.Id,
                Nome = ponto.Nome,
                Categoria = ponto.Categoria,
                Latitude = item.Lat + ponto.LatOffset,
                Longitude = item.Lon + ponto.LonOffset
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedPontosAdicionaisAsync(ApplicationDbContext context)
    {
        var extrasPorCidade = new Dictionary<string, PontoCatalogo[]>
        {
            ["Palmas - TO"] =
            [
                new("Parque Cesamar", "Parque", 0.01, 0.005),
                new("Praia da Graciosa", "Praia", 0.02, -0.01)
            ],
            ["Rio de Janeiro"] =
            [
                new("Museu do Amanhã", "Cultura", 0.005, -0.02)
            ],
            ["Gramado"] =
            [
                new("Parque Caracol", "Natureza", 0.03, -0.02)
            ],
            ["Fortaleza"] =
            [
                new("Praia de Iracema", "Praia", 0.008, 0.01)
            ]
        };

        foreach (var (cidade, pontos) in extrasPorCidade)
        {
            var destino = await context.Destinos.FirstOrDefaultAsync(d => d.Cidade == cidade);
            if (destino is null)
                continue;

            foreach (var ponto in pontos)
            {
                if (await context.PontosTuristicos.AnyAsync(p =>
                        p.DestinoId == destino.Id && p.Nome == ponto.Nome))
                    continue;

                context.PontosTuristicos.Add(new PontoTuristico
                {
                    DestinoId = destino.Id,
                    Nome = ponto.Nome,
                    Categoria = ponto.Categoria,
                    Latitude = destino.Latitude + ponto.LatOffset,
                    Longitude = destino.Longitude + ponto.LonOffset
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedPacotesDemoAsync(ApplicationDbContext context)
    {
        if (await context.Pacotes.AnyAsync())
            return;

        var guia = await context.Guias
            .Include(g => g.Usuario)
            .FirstOrDefaultAsync(g => g.Usuario.Email == "guia@demo.com");

        var pacotes = new[]
        {
            new { Cidade = "Rio de Janeiro", Titulo = "Rio Essencial — 5 dias",
                  Descricao = "Cristo, Pão de Açúcar e as melhores praias com guia local.",
                  Preco = 2890m, InicioEmDias = 35, Duracao = 5,
                  Dias = new[] { "Chegada e praia de Copacabana", "Cristo Redentor e Santa Teresa", "Pão de Açúcar e Praia Vermelha", "Praia de Ipanema e Lagoa", "Compras e retorno" } },
            new { Cidade = "Gramado", Titulo = "Serra Gaúcha Romântica — 4 dias",
                  Descricao = "Gramado e Canela com fondue, vinícolas e passeios de inverno.",
                  Preco = 2190m, InicioEmDias = 60, Duracao = 4,
                  Dias = new[] { "Chegada e Rua Coberta", "Lago Negro e Mini Mundo", "Canela e Cascata do Caracol", "Café colonial e retorno" } },
            new { Cidade = "Palmas - TO", Titulo = "Expedição Jalapão — 7 dias",
                  Descricao = "Dunas, fervedouros e cachoeiras na maior aventura do cerrado.",
                  Preco = 3490m, InicioEmDias = 90, Duracao = 7,
                  Dias = new[] { "Chegada em Palmas", "Trilha nas dunas do Jalapão", "Fervedouros cristalinos", "Cachoeira da Velha", "Serra do Espírito Santo", "Comunidade quilombola e artesanato", "Retorno a Palmas" } }
        };

        foreach (var p in pacotes)
        {
            var destino = await context.Destinos.FirstOrDefaultAsync(d => d.Cidade == p.Cidade);
            if (destino is null)
                continue;

            var hospedagem = await context.Hospedagens.FirstOrDefaultAsync(h => h.DestinoId == destino.Id);
            var inicio = DateTime.UtcNow.Date.AddDays(p.InicioEmDias);

            var pacote = new Pacote
            {
                GuiaId = guia?.Id,
                DestinoId = destino.Id,
                HospedagemId = hospedagem?.Id,
                Titulo = p.Titulo,
                Descricao = p.Descricao,
                DataInicio = inicio,
                DataFim = inicio.AddDays(p.Duracao - 1),
                Preco = p.Preco
            };

            for (var i = 0; i < p.Dias.Length; i++)
                pacote.Dias.Add(new PacoteDia { NumeroDia = i + 1, Descricao = p.Dias[i] });

            context.Pacotes.Add(pacote);
        }

        await context.SaveChangesAsync();
    }

    public static async Task SeedUsuariosDemoAsync(ApplicationDbContext context)
    {
        if (!await context.Usuarios.AnyAsync(u => u.Email == "guia@demo.com"))
        {
            var roleGuia = await context.Roles.FirstAsync(r => r.Nome == "Guia");
            var usuarioGuia = new Usuario
            {
                Nome = "Guia Demo",
                Email = "guia@demo.com",
                SenhaHash = PasswordHasher.Hash(DemoSenha),
                DataCriacao = DateTime.UtcNow,
                RoleId = roleGuia.Id
            };
            context.Usuarios.Add(usuarioGuia);
            await context.SaveChangesAsync();

            context.Guias.Add(new Guia
            {
                UsuarioId = usuarioGuia.Id,
                Especialidade = "Ecoturismo e serra gaúcha",
                Rating = 4.8m
            });
            await context.SaveChangesAsync();
        }

        if (!await context.Usuarios.AnyAsync(u => u.Email == "admin@demo.com"))
        {
            var roleAdmin = await context.Roles.FirstAsync(r => r.Nome == "Admin");
            var usuarioAdmin = new Usuario
            {
                Nome = "Admin Demo",
                Email = "admin@demo.com",
                SenhaHash = PasswordHasher.Hash(DemoSenha),
                DataCriacao = DateTime.UtcNow,
                RoleId = roleAdmin.Id
            };
            context.Usuarios.Add(usuarioAdmin);
            await context.SaveChangesAsync();

            context.Administradores.Add(new Administrador
            {
                UsuarioId = usuarioAdmin.Id,
                NivelAcesso = 10
            });
            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedRolesAsync(ApplicationDbContext context)
    {
        foreach (var nome in RolesPadrao)
        {
            if (!await context.Roles.AnyAsync(r => r.Nome == nome))
                context.Roles.Add(new Role { Nome = nome });
        }

        await context.SaveChangesAsync();
    }

    public static async Task SeedCategoriasDespesaAsync(ApplicationDbContext context)
    {
        foreach (var nome in CategoriasDespesaPadrao)
        {
            if (!await context.CategoriasDespesa.AnyAsync(c => c.Nome == nome))
                context.CategoriasDespesa.Add(new CategoriaDespesa { Nome = nome });
        }

        await context.SaveChangesAsync();
    }

    public static async Task SeedDadosDemoAsync(ApplicationDbContext context)
    {
        if (await context.Viagens.AnyAsync(v => v.Id == 1))
            return;

        await EnsureDestinoDemoAsync(context);
        await EnsureHospedagemDemoAsync(context);
        await EnsurePontosTuristicosDemoAsync(context);

        var viajante = await EnsureViajanteDemoAsync(context);
        await EnsureViagemDemoAsync(context, viajante.Id);
    }

    private static async Task EnsureDestinoDemoAsync(ApplicationDbContext context)
    {
        if (await context.Destinos.AnyAsync(d => d.Id == 1))
            return;

        var destino = new Destino
        {
            Id = 1,
            Cidade = "Palmas - TO",
            Pais = "Brasil",
            Latitude = -10.1844,
            Longitude = -48.3336
        };

        await InserirComIdentityAsync(context, "Destinos", () =>
        {
            context.Destinos.Add(destino);
            return Task.CompletedTask;
        });
    }

    private static async Task EnsureHospedagemDemoAsync(ApplicationDbContext context)
    {
        if (await context.Hospedagens.AnyAsync(h => h.Id == 1))
            return;

        var hospedagem = new Hospedagem
        {
            Id = 1,
            DestinoId = 1,
            Nome = "Hotel Jalapão Eco",
            Endereco = "Região do Jalapão, Palmas - TO",
            CheckIn = DateTime.UtcNow.AddDays(30),
            CheckOut = DateTime.UtcNow.AddDays(37)
        };

        await InserirComIdentityAsync(context, "Hospedagens", () =>
        {
            context.Hospedagens.Add(hospedagem);
            return Task.CompletedTask;
        });
    }

    private static async Task EnsurePontosTuristicosDemoAsync(ApplicationDbContext context)
    {
        if (await context.PontosTuristicos.AnyAsync())
            return;

        var pontos = new[]
        {
            new PontoTuristico { DestinoId = 1, Nome = "Jalapão", Categoria = "Natureza", Latitude = -10.25, Longitude = -46.80 },
            new PontoTuristico { DestinoId = 1, Nome = "Lago da Seda", Categoria = "Lago", Latitude = -10.30, Longitude = -48.20 },
            new PontoTuristico { DestinoId = 1, Nome = "Catedral Metropolitana de Palmas", Categoria = "Religioso", Latitude = -10.1844, Longitude = -48.3336 }
        };

        context.PontosTuristicos.AddRange(pontos);
        await context.SaveChangesAsync();
    }

    private static async Task<Viajante> EnsureViajanteDemoAsync(ApplicationDbContext context)
    {
        var viajanteExistente = await context.Viajantes
            .Include(v => v.Usuario)
            .FirstOrDefaultAsync(v => v.Usuario.Email == DemoEmail);

        if (viajanteExistente is not null)
            return viajanteExistente;

        var roleViajante = await context.Roles.FirstAsync(r => r.Nome == "Viajante");

        var usuario = new Usuario
        {
            Nome = "Viajante Demo",
            Email = DemoEmail,
            SenhaHash = PasswordHasher.Hash(DemoSenha),
            DataCriacao = DateTime.UtcNow,
            RoleId = roleViajante.Id
        };

        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var viajante = new Viajante
        {
            UsuarioId = usuario.Id,
            PreferenciasViagem = "Ecoturismo, Jalapão e cultura tocantinense"
        };

        context.Viajantes.Add(viajante);
        await context.SaveChangesAsync();

        return viajante;
    }

    private static async Task EnsureViagemDemoAsync(ApplicationDbContext context, int viajanteId)
    {
        if (await context.Viagens.AnyAsync(v => v.Id == 1))
            return;

        var viagem = new Viagem
        {
            Id = 1,
            ViajanteId = viajanteId,
            DestinoId = 1,
            DataInicio = DateTime.UtcNow.AddDays(30),
            DataFim = DateTime.UtcNow.AddDays(37),
            Status = "Planejada"
        };

        await InserirComIdentityAsync(context, "Viagens", () =>
        {
            context.Viagens.Add(viagem);
            return Task.CompletedTask;
        });
    }

    private static async Task InserirComIdentityAsync(
        ApplicationDbContext context,
        string tableName,
        Func<Task> inserir)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await ExecutarIdentityInsertAsync(context, tableName, ativar: true);
            await inserir();
            await context.SaveChangesAsync();
            await ExecutarIdentityInsertAsync(context, tableName, ativar: false);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static Task ExecutarIdentityInsertAsync(
        ApplicationDbContext context,
        string tableName,
        bool ativar)
    {
        var sql = (tableName, ativar) switch
        {
            ("Destinos", true) => "SET IDENTITY_INSERT [Destinos] ON",
            ("Destinos", false) => "SET IDENTITY_INSERT [Destinos] OFF",
            ("Hospedagens", true) => "SET IDENTITY_INSERT [Hospedagens] ON",
            ("Hospedagens", false) => "SET IDENTITY_INSERT [Hospedagens] OFF",
            ("Viagens", true) => "SET IDENTITY_INSERT [Viagens] ON",
            ("Viagens", false) => "SET IDENTITY_INSERT [Viagens] OFF",
            _ => throw new ArgumentException($"Tabela não suportada para seed: {tableName}", nameof(tableName))
        };

        return context.Database.ExecuteSqlRawAsync(sql);
    }
}
