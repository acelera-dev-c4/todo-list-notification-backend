# todo-list-notification-backend
Microserviço de notificação do sistema todo list.

Este microserviço serve para o usuário poder assinar em uma outra tarefa e acompanhá-la, pois ele depende desta tarefa ter sido finalizada para prosseguir com suas sub-tarefas.

Este projeto é um modelo de aprendizagem do Projeto Acelera Dev, do Grupo Carrefour Brasil.

![Logo do Grupo Carrefour Brasil](https://media.licdn.com/dms/image/D4D0BAQGrE_UnFL8plQ/company-logo_200_200/0/1708908772188/grupocarrefourbrasil_logo?e=1723680000&v=beta&t=s8_oIbxqF4K8COSGT4kCYgzU0YLA9u0mKqZForzdB0I)

## Objetivo

Este projeto tem como objetivo fornecer uma API RESTful para gerenciamento de tarefa, permitindo aos usuários assinar e dessasinar em tarefas e receber uma notificação assim que a tarefa assinada seja concluída. É uma aplicação desenvolvida com.NET Core, utilizando o Entity Framework Core para persistência de dados.

## Ferramentas Recomendadas

### Desenvolvimento

- **Visual Studio Community**: Ideal para usuários do Windows.
  - [Download Visual Studio Community](https://visualstudio.microsoft.com/vs/community/)
- **Visual Studio Code**: Recomendado para usuários de Linux, com extensões como C#,.NET Core Tools e Swagger.
  - [Download Visual Studio Code](https://code.visualstudio.com/)


- **.NET SDK** (Versão 8.0.5 recomendada)
  - [Baixe o.NET SDK](https://dotnet.microsoft.com/download)

### Banco de Dados

- **SQL Server**
  - [Documentação oficial do SQL Server](https://docs.microsoft.com/en-us/sql/sql-server/)

### Outras Ferramentas

- **Azure Data Studio** para gerenciamento de SQL Server.
  - [Download Azure Data Studio](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio)

## Modelo de Dados

![Modelo de Dados](https://github.com/raffacabofrio/acelera-dev-todo-list/blob/main/docs/MODELO.drawio.png)

## Como Executar

1. Clone o repositório:
bash git clone https://github.com/acelera-dev-c4/todo-list-notification-backend.git

- ```bash
  cd todo-list-notification-backend

2. Restaure as dependências necessárias:

- ```bash
  bash dotnet restore

3. Configure o banco de dados:

   Abra o Azure Data Studio.
   Conecte-se ao SQL Server.
   Execute os scripts SQL encontrados na pasta /src/Infra/ para configurar o banco de dados.

4. Execute a aplicação:
- ```bash
  dotnet run --project src/Api

5. Este comando irá iniciar o servidor e a API estará acessível em http://localhost:5042.

   Navegue para http://localhost:5042/swagger para ver e interagir com a documentação da API e testar os endpoints.


## Dicas de como fazer uma migration

## Caso precise alterar ou adicionar um dado. Siga os passos abaixo:
### 1 - Altere ou adicione o dado
- 1.1 Na pasta src >> Domain >> Mappers, você tem os mapeamentos entre as entidades e as tabelas do banco.
- 1.2 Na pasta src >> Domain >> Models, você tem as classes referentes as tabelas
- 1.3 Execute as alterações conforme necessário

### 2 - Execute os comandos de migration
- 2.1 No Package Manager Console, selecione o projeto Infra como projeto padrão.
- 2.2 No Package Manager Console, execute os comandos:
- ```bash
  Add-Migration NOME_SIGNIFICATIVO
  Update-Database

- OBS: O nome da migration deve ser facilmente entendido. Logo, faça pequenas alterações, mesmo que com isso você tenha diversas migrations.

## Caso precise incluir uma nova tabela
### 1 - Crie as classes necessárias
- 1.1 Na pasta src >> Domain >> Models, crie a classe referente a tabela
- 1.2 Na pasta src >> Domain >> Mappers, crie o mapeamento entre classe do passo anterior e a tabela do banco.

## 2 - Execute os comandos de migration
- 2.1 No Package Manager Console, selecione o projeto Infra como projeto padrão.
- 2.2 No Package Manager Console, execute os comandos:
- ```bash
  Add-Migration NOME_SIGNIFICATIVO
  Update-Database
- OBS: O nome da migration deve ser facilmente entendido. Logo, faça pequenas alterações, mesmo que com isso você tenha diversas migrations.

## Autores


- [Raffaello Damgaard](https://github.com/raffacabofrio)
- [Vinícius Silva](https://github.com/viniciusapsilva)
- [Diogenes Lima](https://github.com/LimaDiogenes)
- [Igor Cordeiro](https://github.com/igorcordeiro08)
- [Leandro Pio](https://github.com/LeandroMPio)
- [Patric](https://github.com/Patric-BM)
- [Vagner Silva](https://github.com/Vagner1212)

## Relacionados

## Obter o appsettings com o time.
appsettings.Development.json


Segue o link do Trello relacionado

- [Trello - Acelera Dev](https://trello.com/b/DeO6PAeI/acelera-dev-notification)
