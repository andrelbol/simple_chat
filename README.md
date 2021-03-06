# Simple Chat
O Simple Chat é uma aplicação de chat que utiliza comunicação TCP para conectar os usuários. Ela é formada principalmente por um servidor TCP capaz de receber conexões dos clientes TCP (também existentes nesse repositório) permitindo que eles enviem mensagens uns para os outros.

## Simple Server
Servidor da aplicação, é uma `ConsoleApplication` capaz de receber conexões TCP e "conectar" os clientes a outros já conectados. O código do `Simple Server` pode ser encontrado dentro da pasta `Server` na pasta _root_ da solução. Por padrão foi fixado o _host_ `127.0.0.1` e a porta `20000` por essa ser uma aplicação sem fins definidos.

Ele é composto basicamente por 3 componentes principais:

1. `Program.cs`: ponto de entrada da `ConsoleApplication`
1. `SimpleServer`: contém o servidor TCP que recebe as conexões dos clientes e os encaminha para processamento
1. `ClientManager`: serviço responsável por lidar com as conexões dos clientes, lendo e escrevendo mensagens para eles de acordo com a funcionalidade da aplicação

O `SimpleServer` possui um objeto de `ClientManager`, o qual possui uma lista de clientes e uma lista de salas (que começa por padrão com uma sala `#general`). Sempre que uma conexão é recebida, esse cliente é adicionado à lista de clientes do `ClientManager` na sala `#general`.

O `ClientManager` cria uma _thread_ para cada cliente que se conecta, sabendo como interpretar as mensagens que são recebidas e agindo de acordo com o conteúdo delas (classe estática `MessageHandler`). As _threads_ criadas ficam aguardando mensagens do cliente ao longo de sua duração até que o cliente saia do chat. Dado que as _threads_ **compartilham as listas** de clientes e salas, optou-se por utilizar listas que fossem _thread safe_ e que permitissem adição e remoção de itens de forma segura (no caso a classe `ConcurrentDictionary`).

Todo cliente tem como primeira comunicação a escolha do seu apelido (que não pode se repetir dentre os clientes da aplicação). Depois de escolhido o apelido, o cliente possui as seguintes funcionalidades:
1. `/u <apelido> <mensagem>`: escreve uma mensagem pública para alguém
1. `/p <apelido> <mensagem>`: escreve uma mensagem privada para alguém
1. `/room <nome_da_sala>`: troca de sala (se a sala não existe, cria uma nova sala)
1. `/exit`: sai do chat
1. `/help`: lista as opções de comandos para o usuário
1. `<mensagem>`: escreve uma mensagem pública para todos os usuários da sala

As mensagens que possuem comandos (`/p`, `/u` entre outras) possuem número de argumentos fixo. Essa verificação é realizada no momento em que a mensagem vai ser processada. Para cada um dos tipos de mensagem foram criados testes unitários encontrados na pasta `Tests` na pasta _root_ da solução.

A comunicação com o cliente é feita através da classe `Client` que representa um cliente e possui métodos para **receber uma mensagem do cliente** e para **escrever uma mensagem para o cliente**. É nessa classe também que se armazena o **apelido** do usuário conectado e **a sala** em que ele se encontra no momento.

Além de dentro do objeto `Client`, o `ClientManager` também guarda a lista de salas existentes no chat no momento. Cada sala (da class `Room`) tem conhecimento de quais clientes estão presentes nela. É a partir desse controle por exemplo que é possível saber **para quem as mensagens públicas devem ser enviadas**. A classe `Room` também tém o conhecimento de como **adicionar um usuário** à sala, impedindo por exemplo que um cliente esteja em mais de uma sala ao mesmo tempo.

## Simple Client

Cliente da aplicação, é uma `ConsoleApplication` capaz de realizar conexões TCP com o servidor. Por padrão foi fixado o _host_ `127.0.0.1` e a porta `20000` por essa ser uma aplicação sem fins definidos. O `SimpleClient` é composto basicamente de 2 componentes:

1. `Program.cs`: ponto de partida da `ConsoleApplication`
1. `SimpleClient.cs`: cliente da aplicação capaz de comunicar com o `SimpleServer`

Durante a implementação do projeto optou-se por deixar a lógica em sua maior parte no servidor, sendo o cliente somente um cliente TCP sem muita lógica de negócio sobre ele. Assim, o `SimpleClient` representa um `TcpClient`, tendo seu fluxo de vida definido como:

1. Realiza conexão TCP com o servidor
1. Cria uma _thread_ para receber entradas do usuário no console que são enviadas para o servidor
1. Cria uma _thread_ para receber mensagens do servidor que são mostradas no console

Assim que a conexão com o servidor é finalizada (por meio do comando `/exit` onde o servidor fecha a conexão com o cliente), a aplicação fecha a conexão com o servidor por parte do cliente e finaliza o processo.

### Interface `IClient`

Acredito ser relevante explicar a necessidade da `interface IClient`. Para construir os testes unitários, era necessário isolar a lógica da comunicação TCP. Para isso, foi criada essa interface que possui uma implementação para a aplicação (`Client`) e uma implementação para os testes (`TestClient`). A diferença da `TestClient` é que, no lugar de escrever mensagens em um _stream_ TCP, as escritas desse cliente somente preenchem uma propriedade `WrittenMessage` que é utilizar para verificar se as mensagens são corretamente interpretadas.

## Compilação e execução

Existem 2 caminhos para se compilar e executar a aplicação. O primeiro deles é por meio da **IDE Visual Studio**, cuja versão _community_ pode ser baixada **gratuitamente**. Por meio dessa IDE é possível abrir a solução que se encontra no repositório (SimpleChat.sln). Tendo a solução aberta, basta **construir** a aplicação e **executar** normalmente. É importante ressaltar que **na solução** se encontra **tanto a aplicação servidor** (SimpleChat.Server) **quanto a aplicação cliente** (SimpleChat.Client), assim será necessário **escolher qual executar** caso opte pela IDE (menos recomendado).

O segundo caminho é por meio da **ferramenta de linha de comando dotnet**, a qual pode ser obtida baixando gratuitamente o **SDK do .NET Core** (disponível para múltiplas plataformas). Depois de baixar o SDK, para executar a aplicação é necessário primeiramente **publicá-la**. Para isso, utilizando um terminal:

* navegue até a pasta raiz do repositório na máquina
* execute o comando `dotnet publish -p "caminho/para/pasta/de/saida"`

Após executar esse comando, na pasta de saída escolhida poderá ser encontrado uma série de arquivos. Dentre esses a **aplicação servidor** é a **SimpleChat.Server.exe** e a **aplicação cliente** é a **SimpleChat.Client.exe**. Essa forma de publicação serve somente para sistemas **Windows**. Caso deseje publicar para outros sistemas operacionais é necessário consultar a documentação do comando `dotnet publish`.

### Instruções de Execução
Para utilizar o SimpleChat, **primeiramente** é necessário executar o **servidor**. Assim que esse estiver de pé, pode-se executar **um ou mais clientes** (eles se conectaram imediatamente ao servidor através da porta 20000 da sua máquina local). Assim que o cliente se conectar, será requisitado o apelido daquele cliente, e daí em diante ele já estará conectado no chat (mais informações de utilização por meio do comando `/help` no cliente).