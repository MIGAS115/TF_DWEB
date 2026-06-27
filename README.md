### Identificação dos Alunos e Aplicação

| Nº | Nome | Email |
| :--- | :--- | :--- |
| **25954** | Daniel Gonçalves | aluno25954@ipt.pt |
| **25981** | Miguel Araújo | aluno25981@ipt.pt |

**Nome da aplicação:** E-Sports IPT

**Principais funcionalidades:**
* Gestão de Equipas, Torneios e Jogos (CRUD) com relações de base de dados 1-N e M-N.
* Sistema de autenticação com dois perfis: Admin (gestão total) e Utilizador (visualização e favoritos).
* API REST (projeto WebApi) que expõe endpoints para Jogos, Equipas e Torneios.
* Atualizações de resultados de jogos em tempo real através do SignalR (MatchHub), transmitindo as alterações de resultado e de estado a todos os clientes conectados.
* Sistema de favoritos que permite aos utilizadores registados marcar e seguir as suas equipas de e-sports preferidas.

---

### Roles (ou outras formas de ‘segmentar’ os utilizadores) a serem concretizadas nas duas versões do trabalho:

| Role | Função |
| :--- | :--- |
| **Admin** | Gestão total: criar, editar e eliminar Equipas, Torneios, Partidas e Categorias. Acesso a todas as páginas da aplicação. |
| **User** | Acesso apenas de leitura a Equipas, Torneios e Partidas. Pode adicionar ou remover Equipas da sua lista pessoal de Favoritos. |
| **Anonymous** | Pode navegar nas páginas públicas (detalhes de Equipas, Torneios e Partidas) sem iniciar sessão. Não pode adicionar favoritos nem gerir dados. |

### Utilizadores pré-carregados na app

| Role | User name | Password | Componente |
| :--- | :--- | :--- | :--- |
| **Admin** | admin@esports.pt | PasswordAdmin123! | WebApp; WebApi |
| **User** | user@esports | PasswordUser123! | WebApp |

---

### Acesso a Base de Dados
Uma das exigências das regras de avaliação pressupõe que a aplicação seja capaz de interagir com uma base de dados nos relacionamentos 1-N e M-N.

Para garantir que vocês são corretamente avaliados, solicitamos que nos informem onde essa interação está a ser executada. Escrevam o link que se vê no browser quando a aplicação está a ser acedida.

#### Interface gráfica (browser)

**Relacionamento 1-N**
* **Insert:** `https://<host>/api/Matches`
* **Edit:** `https://<host>/api/Matches/{id}`
* **Delete:** `https://<host>/api/Matches/{id}`
* **Select:** `https://<host>/api/Matches`
* **Select:** `https://<host>/api/Matches/{id}`

**Relacionamento M-N**
* **Insert:** *[Inserir link se aplicável]* `https://`
* **Edit:** *[Inserir link se aplicável]* `https://`
* **Delete:** *[Inserir link se aplicável]* `https://`
* **Select (All Teams):** `GET api/Teams`
* **Select (All Tournaments):** `GET api/Tournaments`

---

### API - Funcionalidades implementadas

| Função | URL (endpoints) |
| :--- | :--- |
| GET all Matches | `https://<host>/api/Matches` |
| GET Match by id | `https://<host>/api/Matches/{id}` |
| GET all Teams | `https://<host>/api/Teams` |
| GET Team by id | `https://<host>/api/Teams/{id}` |
| GET all Tournaments | `https://<host>/api/Tournaments` |
| GET Tournament by id | `https://<host>/api/Tournaments/{id}` |

---

### SignalR

> **O que foi implementado com SignalR:**
> Transmissão em tempo real dos resultados dos jogos através do `MatchHub` (SignalR). Quando um administrador atualiza o resultado de um jogo, o hub chama a função `BroadcastMatchUpdate(matchId, status, homeScore, awayScore)`, enviando atualizações em direto para todos os clientes do browser sem necessidade de recarregar a página.

---

### Instruções para a execução da aplicação

1. Clone o repositório do GitHub e abre a solução no **Visual Studio 2022** ou posterior.
2. Configura a string de ligação no ficheiro `appsettings.json` (em ambos os projetos **WebApp** e **WebApi**) para apontar para a tua instância do SQL Server.
3. Define múltiplos projetos de arranque: clique com o botão direito na solução → **Propriedades** → **Projetos de arranque múltiplos**, define o **WebApp** e o **WebApi** ambos como **Iniciar**.
4. Executa a aplicação. A base de dados e os dados de seed (categorias, equipas, torneios, utilizador administrador) são criados automaticamente no primeiro arranque através do `DbInitializer`.

*Nota:* Inicie sessão com `admin@esports.pt` / `PasswordAdmin123!` para aceder às funcionalidades de gestão completa. Regista um novo utilizador para testar o perfil de Utilizador.

---

### Observações Finais

O projeto está estruturado em três camadas:
* **ESports.Domain:** models, DbContext, seed data
* **WebApp:** Razor Pages UI
* **WebApi:** REST API

**Relações:**
* A relação **1-N** demonstrada é `Category` → `Team` (uma Category tem muitas Teams).
* A relação **M-N** é `Tournament` ↔ `Team` (via `TournamentTeam`) e `User` ↔ `Team` via Favorites.

**Detalhes adicionais:**
* Os logótipos das Teams são feitos upload e armazenados como ficheiros no servidor.
* O campo `ExternalSourceId` em Teams, Matches e Tournaments está reservado para futura integração com external e-sports data APIs.
