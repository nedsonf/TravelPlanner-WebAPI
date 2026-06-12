# TravelPlanner — API de Gestão de Viagens

Plataforma web de planejamento e **marketplace de viagens** no modelo de intermediário: a plataforma conecta **viajantes**, **guias turísticos** e **administradores**, sem vender pacotes diretamente.

- **Viajante:** planeja viagem (destino + datas), consulta clima e pontos turísticos, reserva pacotes e gerencia a carteira.
- **Guia:** publica pacotes, atende pedidos personalizados e acompanha vendas.
- **Admin:** curadoria do catálogo (destinos e hotéis) e extrato financeiro (taxa de 10%).

Interface SPA em `wwwroot/index.html` servida pelo próprio backend.

## Tecnologias

| Camada | Stack |
|--------|--------|
| **Backend** | C#, .NET 8, ASP.NET Core Web API, Entity Framework Core 8, SQL Server |
| **Autenticação** | JWT Bearer (papéis: Viajante, Guia, Admin) |
| **Frontend** | HTML5, JavaScript, Tailwind CSS (CDN) |
| **Documentação** | Swagger (ambiente Development) |
| **APIs externas** | Open-Meteo Geocoding, OpenWeatherMap |

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server ou SQL Server Express
- Git (opcional, para clonar o repositório)

## Como rodar

```powershell
# Clonar
git clone https://github.com/nedsonf/TravelPlanner-WebAPI.git
cd TravelPlanner-WebAPI

# Aplicar migrations no banco
dotnet ef database update

# Subir a API
dotnet run --launch-profile http
```

- **Aplicação:** http://localhost:5108  
- **Swagger:** http://localhost:5108/swagger  

Ajuste a connection string em `appsettings.json` se o SQL Server não for `localhost\SQLEXPRESS`.

No primeiro start em **Development**, o `DbSeeder` popula destinos, pontos turísticos, usuários demo e pacotes de exemplo.

## Usuários demo

| Papel | E-mail | Senha |
|-------|--------|-------|
| Viajante | `viajante@demo.com` | `Senha@123` |
| Guia | `guia@demo.com` | `Senha@123` |
| Admin | `admin@demo.com` | `Senha@123` |

## API pública (sem autenticação)

Endpoints para parceiros e integrações — leitura do catálogo interno:

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/public/destinos` | Destinos com pontos em destaque |
| `GET` | `/api/public/hospedagens` | Hotéis parceiros por destino |
| `GET` | `/api/public/pontos-turisticos-populares` | Pontos turísticos do catálogo |

## Estrutura do projeto

```
Controllers/     # Endpoints REST
Services/        # Regras de negócio e integrações externas
Data/            # DbContext e seed
Models/          # Entidades
DTOs/            # Contratos de entrada/saída
Migrations/      # Schema do banco
wwwroot/         # Interface web (SPA)
```

## Repositório

https://github.com/nedsonf/TravelPlanner-WebAPI

## Licença

Projeto acadêmico / demonstração.
