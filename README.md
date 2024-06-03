# Cesla "Portal"

O sistema simula um portal onde usuários poderiam administrar informações da empresa. 
Para simplicidade, não foi implementado um mecanismo de autenticação.

# Estrutura

- [Domain](#domain)
- [Application](#application)
- [Infrastructure](#infrastructure)
- [WebUI](#web-ui)
- [Integration Tests](#integration-tests)
- [Solution Items](#solution-items)
  - [Docker](#sobre-o-docker-compose)
- [Executando localmente](#executando-localmente)

## Domain

A solução é um Web APP desenvolvido com Blazor na versão .NET 8, usando Clean Architecture dividindo as diferentes partes.

O projeto de Domain contém os modelos de dominio, e as abstrações para definição e identificação de objetos.

A validação de um modelo de dominio acontece só dentro dele, onde regras de negócio específicas são aplicadas. Dependendo da complexidade, a criação de objetos pode ser feita com Factory static methods ou simplesmente usando o constructor quando for o suficientemente fácil validar fora antes de criar o objeto. 

> Cada aggregate esta definido numa pasta, e nela esta tudo o que refer especificamente a ele, tendo definições de entidades da qual o aggregate é dono e value objects especificos.

## Application

Contém os diferentes casos de uso da aplicação, que se definem como Commands e Queries. Cada caso de uso é implementado num handler, que é chamado pelo `ISender` do MediatR.

Também ela define as interfaces a ser implementadas pelos diferentes projetos de infraestrutura, estabelecendo um contrato comum para a persistencia e integração.

- Companies
    - Commands
        - UpdateCompany
    - Queries
        - GetCompany
- Employees
    - Commands
        - CreateEmployee
        - UpdateEmployee
        - DeleteEmployee
    - Queries
        - ListEmployees
        - GetEmployee

## Infrastructure

Contém a implementação concreta dos diferentes contratos definidos no projeto de Application e define o mecanismo de persistencia, no caso usando EF Core. Também possui interceptors para mecanismo de OutboxPattern e de SoftDelete (para implementar event sourcing e logical deletes).

Um background service constantemente publica os eventos de dominio para os handlers do MediatR. No exemplo não existem handlers para os eventos, pelo que apenas são marcados como finalizados.

## Web UI

É a layer de presentação, onde se define a interface com o usuário. Usa Blazor para renderizar a interface e chama os diferentes casos de uso da aplicação usando o `ISender`. As diferentes telas são implementadas com Razor Components. 

Também define modelos de formulários para separar a lógica de validação não gerar dependências dos modelos definidos pelas outras camadas.

A configuração dos serviços usados pelo sistema inteiro é feita no `Program.cs` do projeto.

Cada projeto tem um arquivo que expõe o metodo necessário para usar dependency injection, sendo que cada projeto é responsável pelo cadastro dos serviços necessários que ele implementa. No caso do projeto de Infrastructure também foi implementado um método de "setup" para o seed do banco de dados.

## Integration Tests

O Cesla.Portal conta com um projeto de tests de integração na pasta "tests" interna dele. O projeto usa xUnit, FluentAssertions, FluentDocker e Playwright para fazer um teste completo E2E da aplicação.

Ele possui a seguinte estrutura pastas:

- Pages
  - CompanyPages
    - CompanyDetailsTests
    - CompanyEditTests 
  - EmployeePages
    - EmployeeEditTests
    - EmployeeListTests
    - EmployeeNewTests
- SharedTestCollection
- SharedTestContext
- docker-compose.integration.yml

### Docker

O projeto usa o mesmo `Dockerfile` definido para a aplicação web, modificando algumas variáveis de ambiente para apontar para o banco de dados de teste. O `docker-compose.integration.yml` define também seu própio container de `mysql`.

Não é necessário rodar comandos de de docker-compose para rodar os testes, já que a biblioteca Ductus.FluentDocker faz isso automaticamente. O teste pode demorar um pouco em começar, devido a que ele precisa esperar que o container esteja pronto para aceitar conexões.

### SharedTestContext

É uma classe que contém a configuração do teste, e é responsável por criar o container do docker-compose e inicializar o browser para os testes. O `SharedTestCollection` é marcado como fixture para poder ser reaproveitado em todos os testes e não ter que gerar um container novo para cada um.

### Playwright

Playwright é uma ferramenta de automação de testes que permite testar aplicações web em diferentes navegadores. No caso do Cesla.Portal, o teste é feito no navegador Chromium.

⚠️**Para o teste funcionar é necessário ter instalados os drivers de browser usados pelo Playwright. Para isso, se debe fazer build do projeto de testes de integração e depois executar o script criado na pasta de bin**

> // Windows power shell desde tests/Cesla.Portal.Tests.Integration
>
> **pwsh** .\bin\Debug\net8.0\playwright.ps1 install



## Solution Items

Os arquivos dentro da "pasta" Solution Items existem físicamente na raíz da solução. Entre os arquivos relevantes se encontram:

- README.md
- .gitignore
- cert.pfx (https)
- .editorconfig (regras básicas e conventions para programação)
- docker-compose.yml
- Dockerfile

# Executando localmente

## Criando os containers

Para executar a solução localmente, primeiro é necessário ter instalado [docker](https://docs.docker.com/engine/install/) e [docker-compose](https://docs.docker.com/compose/install/), depois, se deve posicionar com um terminal na pasta onde fica o arquivo `docker-compose.yml` e rodar o comando

> docker-compose up -d --build

Este comando cria os containers necessários para rodar a aplicação, e os deixa rodando em background.  

Para acessar a aplicação, uma vez que os containers estejam rodando, se deve acessar a url `https://localhost:1234` no navegador. É possivel mudar a porta de acesso modificando o arquivo `docker-compose.yml`.

## Cleanup

Para apagar os containers, se deve rodar o comando

> docker-compose down