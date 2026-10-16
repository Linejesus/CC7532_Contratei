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

<img width="905" height="872" alt="image" src="https://github.com/user-attachments/assets/7aff50b7-44d5-44b0-a2b9-a20f557173c7" />

<br>

<img width="945" height="677" alt="image" src="https://github.com/user-attachments/assets/ea614a59-2cf6-487e-9e0d-2e531a80ee47" />

<br>

<img width="922" height="765" alt="image" src="https://github.com/user-attachments/assets/6e4699fc-61a7-4fdc-a02a-6962c59a2801" />

<br>

<img width="917" height="866" alt="image" src="https://github.com/user-attachments/assets/51d700d8-2f0f-490e-b090-5d7e576bbf16" />

<br>

<img width="906" height="867" alt="image" src="https://github.com/user-attachments/assets/3a9b38bd-ceb1-41a1-b542-0fb4acfc2e05" />

<br>

<img width="917" height="840" alt="image" src="https://github.com/user-attachments/assets/02f16416-e1f4-4fc4-9989-10d10303837c" />

<br>

<img width="995" height="842" alt="image" src="https://github.com/user-attachments/assets/72435e03-53cc-4843-b982-34b0a2b1d78d" />

<br>

<img width="1025" height="857" alt="image" src="https://github.com/user-attachments/assets/a8112265-aa5c-40b4-be2f-77c2e42c9a57" />

<br>

<img width="1127" height="857" alt="image" src="https://github.com/user-attachments/assets/6afa5333-075e-4619-b6ba-84407f8cd764" />

<br>

<img width="1112" height="861" alt="image" src="https://github.com/user-attachments/assets/49d0219a-a9de-4b3c-b3b2-086bb0f16252" />

<br>

<img width="1165" height="832" alt="image" src="https://github.com/user-attachments/assets/2ca12f56-5bb0-4056-88b2-05eabffd90dc" />

<br>

<img width="947" height="852" alt="image" src="https://github.com/user-attachments/assets/46d4a2f5-3705-4660-9b12-c8575d176808" />

<br>

<img width="1215" height="865" alt="image" src="https://github.com/user-attachments/assets/dcaec1e6-9309-49cb-9bcd-feb2c27e92ca" />

<br>

<img width="1242" height="855" alt="image" src="https://github.com/user-attachments/assets/02223c54-85dc-4159-8c41-01b32998acb4" />

<br>

<img width="1067" height="857" alt="image" src="https://github.com/user-attachments/assets/6a57df64-9d31-498b-b438-9b29f8ef3ae7" />

<br>

<img width="1150" height="820" alt="image" src="https://github.com/user-attachments/assets/70bd354f-d752-46a3-8967-f24ddc5af94a" />

<br>

<img width="1072" height="857" alt="image" src="https://github.com/user-attachments/assets/ae323ea4-a194-4cd0-a2c7-5dbfb2d08a95" />

<br>

<img width="1085" height="856" alt="image" src="https://github.com/user-attachments/assets/3e758fa1-0ab8-4491-a25e-ba3be7eaa8a7" />

<br>

<img width="1077" height="750" alt="image" src="https://github.com/user-attachments/assets/824837a4-dcbe-451e-9203-f4a182ea576c" />

<br>

<img width="1182" height="851" alt="image" src="https://github.com/user-attachments/assets/95168152-7be1-41cf-a5cd-344ade4aa3ee" />

---

## 8. Vídeo de Demonstração

https://youtu.be/BadytxM8Oh4

---

## 9. Especificação de Componentes

### 1. Componente `ProfissionalComponente`

#### 1.1. Nome

`ProfissionalComponente`

#### 1.2. Responsabilidade

Gerenciar e disponibilizar informações dos profissionais cadastrados na plataforma **Contratei**, permitindo consultar seus dados e verificar os horários disponíveis para a realização de serviços.

O componente é responsável por fornecer as informações necessárias para que outros componentes possam consultar profissionais e verificar sua disponibilidade antes de realizar um agendamento.

#### 1.3. Operações

| Operação | Descrição |
|---|---|
| `consultarProfissional()` | Consulta os dados de um profissional a partir de seu identificador. |
| `exibirProfissionais()` | Retorna os profissionais cadastrados na plataforma. |
| `filtrarProfissionais()` | Pesquisa profissionais conforme os filtros informados, como área de atuação e localização. |
| `consultarDisponibilidade()` | Consulta os horários disponíveis de um profissional para uma data e duração informadas. |

#### 1.4. Interfaces Fornecidas

**Interface:** `IProfissional`

| Operação | Descrição |
|---|---|
| `consultarProfissional(id)` | Retorna os dados do profissional identificado pelo ID. |
| `exibirProfissionais()` | Retorna a lista de profissionais cadastrados. |
| `filtrarProfissionais(filtros)` | Retorna profissionais compatíveis com os filtros de pesquisa. |
| `consultarDisponibilidade(profissionalId, data, duracaoMinutos)` | Retorna os horários disponíveis do profissional para a data e duração informadas. |

#### 1.5. Interfaces Requeridas

Não possui interfaces requeridas.

#### 1.6. Contratos

##### 1.6.1. Operação `consultarProfissional(id)`

**Entradas**

| Parâmetro | Tipo | Descrição |
|---|---|---|
| `id` | `Guid` | Identificador único do profissional. |

**Saída**

Dados do profissional, incluindo:

- Identificador;
- Nome;
- Descrição;
- Área de atuação;
- Informações de endereço.

**Pré-condições**

- O identificador deve ser um GUID válido.
- A consulta deve ser realizada por meio da interface disponibilizada pelo componente.

**Pós-condições**

- Os dados do profissional são retornados.

##### 1.6.2. Operação `consultarDisponibilidade(profissionalId, data, duracaoMinutos)`

**Entradas**

| Parâmetro | Tipo | Descrição |
|---|---|---|
| `profissionalId` | `Guid` | Identificador do profissional. |
| `data` | `DateOnly` | Data da consulta. |
| `duracaoMinutos` | `int` | Duração desejada para o serviço, em minutos. |

**Saída**

Lista de horários disponíveis compatíveis com a data e a duração informadas.

**Pré-condições**

- O identificador do profissional deve ser válido.
- A data deve ser válida.
- A duração deve ser um valor positivo.
- O profissional deve existir no sistema.

**Pós-condições**

- São retornados os horários considerados disponíveis, segundo as regras de disponibilidade e os agendamentos existentes.

---

### 2. Componente `AgendamentoComponente`

#### 2.1. Nome

`AgendamentoComponente`

#### 2.2. Responsabilidade

Gerenciar os agendamentos realizados na plataforma **Contratei**, incluindo criação, consulta e cancelamento.

O componente deve verificar se o contratante e o profissional existem e consultar a disponibilidade do profissional antes de confirmar um novo agendamento.

Também deve impedir a criação de agendamentos em horários incompatíveis com as regras de disponibilidade e com os agendamentos já registrados.

#### 2.3. Operações

| Operação | Descrição |
|---|---|
| `criarAgendamento()` | Valida os dados, verifica a disponibilidade e registra um novo agendamento. |
| `exibirAgendamentos()` | Retorna os agendamentos associados a um contratante ou profissional. |
| `consultarAgendamento()` | Consulta um agendamento pelo identificador. |
| `cancelarAgendamento()` | Cancela um agendamento existente, respeitando as regras de negócio. |

#### 2.4. Interfaces Fornecidas

**Interface:** `IAgendamento`

| Operação | Descrição |
|---|---|
| `criarAgendamento(dados)` | Cria um agendamento após validar os dados e a disponibilidade. |
| `consultarAgendamento(appointmentId)` | Recupera um agendamento pelo identificador. |
| `exibirAgendamentos(identificador, tipo)` | Retorna os agendamentos de um contratante ou profissional. |
| `cancelarAgendamento(appointmentId)` | Cancela um agendamento existente. |

#### 2.5. Interfaces Requeridas

| Interface | Operação | Finalidade |
|---|---|---|
| `IProfissional` | `consultarProfissional(profissionalId)` | Verifica se o profissional existe e obtém seus dados básicos. |
| `IProfissional` | `consultarDisponibilidade(profissionalId, data, duracaoMinutos)` | Verifica se o horário solicitado está entre os horários disponíveis. |
| `IContratante` | `validarUsuario(contratanteId)` | Verifica a existência e a validade do contratante. |

#### 2.6. Contratos

##### 2.6.1. Operação `criarAgendamento(dados)`

**Entradas**

| Parâmetro | Tipo | Descrição |
|---|---|---|
| `contractorId` | `Guid` | Identificador do contratante. |
| `professionalId` | `Guid` | Identificador do profissional. |
| Data e horário de início | `DateTimeOffset` | Data e horário de início do agendamento. |
| Duração do serviço | Numérico inteiro | Duração necessária para determinar o horário de término, quando aplicável. |

**Saída**

Agendamento criado, contendo:

- Identificador;
- Contratante;
- Profissional;
- Data;
- Horário;
- Estado inicial.

**Pré-condições**

- Os identificadores do contratante e do profissional devem ser válidos.
- O contratante deve existir e estar apto a realizar o agendamento.
- O profissional deve existir.
- A data e os horários devem ser válidos.
- A duração deve ser positiva.
- O horário solicitado deve estar disponível para o profissional.
- Não deve existir outro agendamento incompatível com o horário solicitado.

**Pós-condições**

- O agendamento é persistido e recebe um identificador único.
- O agendamento fica associado ao contratante e ao profissional informados.

##### 2.6.2. Operação `cancelarAgendamento(appointmentId)`

**Entradas**

| Parâmetro | Tipo | Descrição |
|---|---|---|
| `appointmentId` | `Guid` | Identificador do agendamento. |

**Saída**

Confirmação do cancelamento.

**Pré-condições**

- O identificador deve ser válido.
- O agendamento deve existir.
- O estado atual deve permitir o cancelamento.

**Pós-condições**

- O estado do agendamento é atualizado para cancelado.

##### 2.6.3. Operação `exibirAgendamentos(identificador, tipo)`

**Entradas**

| Parâmetro | Tipo | Descrição |
|---|---|---|
| `identificador` | `Guid` | Identificador do contratante ou do profissional. |
| `tipo` | `String` | Tipo de consulta: `contratante` ou `profissional`. |

**Saída**

Lista de agendamentos associados ao identificador informado.

**Pré-condições**

- O identificador deve ser válido.
- O tipo de consulta deve ser reconhecido pela operação.

**Pós-condições**

- A lista retornada contém somente os agendamentos associados ao identificador informado.

---

### 3. Resumo das Interfaces

| Componente | Interface | Tipo | Finalidade |
|---|---|---|---|
| `ProfissionalComponente` | `IProfissional` | Fornecida | Disponibilizar dados dos profissionais, filtros de pesquisa e informações de disponibilidade. |
| `AgendamentoComponente` | `IAgendamento` | Fornecida | Disponibilizar operações de criação, consulta, listagem e cancelamento de agendamentos. |
| `AgendamentoComponente` | `IProfissional` | Requerida | Consultar profissionais e verificar seus horários disponíveis. |
| `AgendamentoComponente` | `IContratante` | Requerida | Validar a existência e a situação do contratante. |

---

### Integrantes

|       Integrante         |       RA       |             
| ------------------------ | -------------- |
| **Aline Rocha de Jeuss** |  22.123.106-1  |
| **Bianca Silva Oliveira**|  22.123.113-7  |
| **Camila Laurindo Reis** |  22.222.037-8  |

---
