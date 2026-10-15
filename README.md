# CONTRATEI

> Conectando profissionais e contratantes de forma simples, rápida e acessível.

---

## 1. Objetivo do projeto

O Contratei é uma aplicação desenvolvida para aproximar profissionais prestadores de serviços e pessoas interessadas em contratá-los.

Este projeto implementa dois componentes de software, definidos a partir da arquitetura baseada em componentes desenvolvida na disciplina de Tópicos Avançados de Engenharia de Software:

- **ProfissionalComponente:** responsável por disponibilizar informações dos profissionais cadastrados e consultar suas disponibilidades.
- **AgendamentoComponente:** responsável por criar, consultar e cancelar agendamentos entre profissionais e contratantes.

O objetivo é demonstrar a organização dos componentes, suas responsabilidades, as interfaces de comunicação, as operações disponibilizadas e os testes realizados para verificar seu funcionamento e sua integração.

---

## 2. Tecnologias utilizadas

| Tecnologia | Finalidade |
|---|---|
| C# | Linguagem de programação utilizada na implementação. |
| .NET 10 | Plataforma de desenvolvimento e execução da aplicação. |
| ASP.NET Core Minimal APIs | Implementação dos endpoints HTTP que disponibilizam as operações da aplicação. |
| OpenAPI e Swagger UI | Documentação dos endpoints e realização de testes manuais por meio de uma interface interativa. |
| Git | Controle de versão do código-fonte. |
| GitHub | Hospedagem do repositório e compartilhamento do projeto. |

---

## 3. Estrutura dos componentes

A solução está organizada em projetos que separam as responsabilidades da aplicação.

```text
Contratei/
├── Contratei.slnx
├── src/
    ├── Contratei.Api/
    ├── Contratei.Service/
    ├── Contratei.Domain/
    ├── Contratei.Data/
    └── Contratei.CrossCutting/

```

---

### 3.1. ProfissionalComponente

**Responsabilidade:** gerenciar e disponibilizar informações dos profissionais cadastrados, permitindo consultar seus dados, pesquisar profissionais por critérios e verificar horários disponíveis para a realização de serviços.

O componente fornece informações necessárias para que o processo de agendamento possa identificar o profissional e consultar sua disponibilidade.

**Principais operações:**

| Operação | Descrição |
|---|---|
| Consultar profissional | Recupera os dados de um profissional por seu identificador. |
| Listar profissionais | Retorna os profissionais cadastrados. |
| Pesquisar profissionais | Filtra profissionais conforme critérios informados, como área de atuação e localização. |
| Cadastrar disponibilidade | Registra os períodos em que um profissional pode atender. |
| Consultar disponibilidade | Retorna os períodos de disponibilidade cadastrados. |
| Consultar horários disponíveis | Calcula os horários compatíveis com a data e a duração solicitadas. |
| Remover disponibilidade | Exclui um período de disponibilidade cadastrado. |

**Interface de negócio prevista:** `IProfissional`.

Essa interface representa o contrato de acesso às operações de consulta de profissionais e de suas disponibilidades.

---

### 3.2. AgendamentoComponente

**Responsabilidade:** gerenciar o ciclo de vida dos agendamentos, incluindo sua criação, consulta e cancelamento.

O componente utiliza informações do profissional e do contratante para validar os dados necessários à solicitação. Também deve considerar as regras de disponibilidade e de conflito de horários.

**Principais operações:**

| Operação | Descrição |
|---|---|
| Criar agendamento | Registra uma solicitação de agendamento com os dados necessários. |
| Consultar agendamento | Recupera um agendamento por seu identificador. |
| Listar agendamentos do contratante | Retorna os agendamentos associados a um contratante. |
| Listar agendamentos do profissional | Retorna os agendamentos associados a um profissional. |
| Cancelar agendamento | Altera o estado do agendamento para cancelado. |

**Interface de negócio prevista:** `IAgendamento`.

Essa interface representa o contrato de acesso às operações de criação, consulta e cancelamento de agendamentos.

---

### 3.3. Responsabilidades dos projetos da solução

| Projeto | Responsabilidade |
|---|---|
| `Contratei.Api` | Receber requisições HTTP, disponibilizar endpoints e retornar respostas aos clientes. |
| `Contratei.Service` | Coordenar operações da aplicação e implementar regras de negócio. |
| `Contratei.Domain` | Representar os conceitos e as entidades centrais do negócio. |
| `Contratei.Data` | Concentrar os mecanismos de acesso e persistência dos dados. |
| `Contratei.CrossCutting` | Reunir configurações e mecanismos compartilhados, incluindo o registro de dependências, conforme implementado. |

Esses projetos formam a estrutura técnica da solução. Os componentes de negócio ProfissionalComponente e AgendamentoComponente representam responsabilidades funcionais que podem utilizar mais de um projeto técnico.

---

## 4. Interfaces e operações implementadas

As interfaces definem contratos que permitem a comunicação entre partes do sistema sem exigir que o componente consumidor conheça os detalhes internos da implementação.

Na especificação dos componentes, são consideradas as seguintes interfaces de negócio:

| Interface | Responsabilidade | Operações previstas |
|---|---|---|
| `IProfissional` | Disponibilizar informações de profissionais e suas disponibilidades. | Consultar, listar, pesquisar profissionais e consultar horários disponíveis. |
| `IAgendamento` | Disponibilizar as operações de gerenciamento de agendamentos. | Criar, consultar, listar e cancelar agendamentos. |

---

### 4.1. Endpoints HTTP disponibilizados

A API expõe as operações por meio dos seguintes endpoints:

| Método HTTP | Endpoint | Finalidade |
|---|---|---|
| `POST` | `/professionals` | Cadastrar profissional. |
| `GET` | `/professionals` | Listar profissionais. |
| `GET` | `/professionals/{id}` | Consultar profissional por identificador. |
| `GET` | `/professionals/search` | Pesquisar profissionais por filtros. |
| `POST` | `/professionals/{professionalId}/availability` | Cadastrar disponibilidade. |
| `GET` | `/professionals/{professionalId}/availability` | Consultar disponibilidades. |
| `GET` | `/professionals/{professionalId}/availability/slots?date={data}&durationMinutes={duracao}` | Consultar horários disponíveis para uma data e duração. |
| `DELETE` | `/professionals/{professionalId}/availability/{availabilityId}` | Remover disponibilidade. |
| `POST` | `/contractors` | Cadastrar contratante. |
| `GET` | `/contractors` | Listar contratantes. |
| `GET` | `/contractors/{id}` | Consultar contratante por identificador. |
| `POST` | `/appointments` | Criar agendamento. |
| `GET` | `/appointments/{appointmentId}` | Consultar agendamento. |
| `PATCH` | `/appointments/{appointmentId}/cancel` | Cancelar agendamento. |
| `GET` | `/contractors/{contractorId}/appointments` | Listar agendamentos de um contratante. |
| `GET` | `/professionals/{professionalId}/appointments` | Listar agendamentos de um profissional. |
| `POST` | `/professionals/{professionalId}/services` | Cadastrar serviço oferecido pelo profissional. |
| `GET` | `/professionals/{professionalId}/services` | Listar serviços de um profissional. |

Os identificadores representados entre chaves são parâmetros de rota. Os parâmetros de consulta, como `date` e `durationMinutes`, permitem informar a data e a duração desejadas para a pesquisa de horários.

---

## 5. Comunicação entre os componentes

A comunicação entre os componentes ocorre por meio de contratos bem definidos, preservando a separação de responsabilidades.

No fluxo de criação de um agendamento, o cliente envia uma requisição HTTP para a API. A aplicação recebe os dados, encaminha a operação para a camada responsável pelas regras de negócio e verifica as informações necessárias para processar a solicitação.

O fluxo funcional esperado é:

1. O cliente solicita um agendamento.
2. A API recebe e valida os dados de entrada.
3. A lógica de agendamento verifica a existência das informações necessárias e consulta a disponibilidade do profissional.
4. As regras de negócio verificam se o horário solicitado pode ser reservado e se existe algum conflito.
5. Se as validações forem satisfeitas, o agendamento é registrado.
6. A API retorna uma resposta indicando o resultado da operação.

Quando a comunicação entre componentes é implementada por interfaces C#, o componente consumidor depende do contrato, enquanto a implementação concreta é fornecida por meio da injeção de dependências. Se a integração ocorrer por HTTP, a comunicação será realizada por requisições e respostas entre endpoints.

---

## 6. Instruções para execução

### 6.1. Pré-requisitos

- SDK do .NET 10 instalado.
- Git instalado.
- Acesso ao repositório do projeto.
- IDE ou editor de código, como Visual Studio ou Visual Studio Code.

### 6.2. Clonar o repositório

```bash
git clone <URL_DO_REPOSITORIO>
cd Contratei
```

Substitua `<URL_DO_REPOSITORIO>` pelo endereço real do repositório no GitHub.

### 6.3. Restaurar as dependências

Na pasta raiz da solução, execute:

```bash
dotnet restore .\Contratei.slnx
```

### 6.4. Compilar a solução

```bash
dotnet build .\Contratei.slnx
```

A compilação permite verificar se os projetos da solução estão consistentes e se as dependências necessárias foram resolvidas.

### 6.5. Executar a API

```bash
dotnet run --project .\src\Contratei.Api\Contratei.csproj --urls http://localhost:5000
```

Para utilizar a interface Swagger UI, execute a aplicação no ambiente de desenvolvimento e acesse:

`http://localhost:5000/swagger`

A interface permite consultar os endpoints disponíveis, informar parâmetros, enviar requisições e visualizar as respostas retornadas pela API. A URL pressupõe que o Swagger UI esteja configurado no projeto.

---

## 7. Testes realizados

Os testes verificam tanto o comportamento individual das operações quanto a integração entre os componentes.

---

### Integrantes

|       Integrante         |       RA       |             
| ------------------------ | -------------- |
| **Aline Rocha de Jeuss** |  22.123.106-1  |
| **Bianca Silva Oliveira**|  22.123.113-7  |
| **Camila Laurindo Reis** |  22.222.037-8  |

---
