Esse comando Docker executa um contêiner RabbitMQ com a interface de gerenciamento habilitada. Aqui está uma explicação detalhada sobre o comando e o que cada parte dele faz:

### Descrição do Comando

```bash
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management
```

#### Componentes do Comando

1. **`docker run`**: Inicia um novo contêiner.
2. **`-it`**: Abre um terminal interativo dentro do contêiner.
   - `-i`: Modo interativo.
   - `-t`: Aloca um pseudo-terminal.
3. **`--rm`**: Remove automaticamente o contêiner quando ele for parado.
4. **`--name rabbitmq`**: Nomeia o contêiner como `rabbitmq`.
5. **`-p 5672:5672`**: Mapeia a porta 5672 do host (máquina local) para a porta 5672 do contêiner, usada para conexões AMQP (o protocolo padrão do RabbitMQ).
6. **`-p 15672:15672`**: Mapeia a porta 15672 do host para a porta 15672 do contêiner, usada para a interface de gerenciamento web do RabbitMQ.
7. **`rabbitmq:3.13-management`**: Especifica a imagem do Docker a ser usada, neste caso, a versão `3.13` do RabbitMQ com a interface de gerenciamento (`management`) habilitada.

### Executando o Comando

Para executar o comando, abra um terminal e digite:

```bash
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management
```

### Acessando a Interface de Gerenciamento

1. **Conexão RabbitMQ**: Você pode conectar clientes RabbitMQ (como o `RabbitMqService` do seu código) ao servidor RabbitMQ usando `localhost` na porta `5672`.
2. **Interface de Gerenciamento Web**: Abra um navegador da web e acesse `http://localhost:15672`. Você verá a interface de gerenciamento do RabbitMQ, onde pode visualizar e gerenciar exchanges, filas, bindings, usuários, e muito mais.

### Credenciais Padrão

As credenciais padrão para acessar a interface de gerenciamento do RabbitMQ são:

- **Usuário**: `guest`
- **Senha**: `guest`

### Exemplo de Publicação e Consumo com o RabbitMQ em Execução no Docker

Com o RabbitMQ em execução no Docker, você pode utilizar o código de publicação e consumo que foi fornecido anteriormente. Aqui está um exemplo de como isso seria usado com o RabbitMQ rodando no contêiner Docker:

```csharp
using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DevTrackR.Tracking.Infrastructure.Messaging
{
    public class RabbitMqService : IMessageBusService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string _exchange = "trackings-service";
        private const string _exchangeType = "topic";

        public RabbitMqService()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost", // Conectar ao RabbitMQ no Docker
                Port = 5672 // Porta padrão do RabbitMQ
            };

            _connection = connectionFactory.CreateConnection("trackings-service-publisher");
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: _exchange, type: _exchangeType);
        }

        public void Publish(object data, string routingKey)
        {
            var type = data.GetType();
            var payload = JsonConvert.SerializeObject(data);
            var byteArray = Encoding.UTF8.GetBytes(payload);

            Console.WriteLine($"{type.Name} Published");

            _channel.BasicPublish(exchange: _exchange, routingKey: routingKey, basicProperties: null, body: byteArray);
        }
    }

    public class RabbitMqConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string _exchange = "trackings-service";

        public RabbitMqConsumer()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost", // Conectar ao RabbitMQ no Docker
                Port = 5672 // Porta padrão do RabbitMQ
            };

            _connection = connectionFactory.CreateConnection("trackings-service-consumer");
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: _exchange, type: "topic");
        }

        public void Consume(string routingKeyPattern)
        {
            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: _exchange, routingKey: routingKeyPattern);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received: {message}");
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var publicador = new RabbitMqService();
                var mensagem = new { Nome = "Teste", Valor = 123 };

                publicador.Publish(mensagem, "quick.orange.rabbit");
                publicador.Publish(mensagem, "lazy.brown.fox");
                publicador.Publish(mensagem, "lazy.orange.elephant");

                var consumidor = new RabbitMqConsumer();
                consumidor.Consume("quick.orange.*");
                consumidor.Consume("lazy.#");

                Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
```

### Resumo

1. **Executar RabbitMQ no Docker**:
   ```bash
   docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management
   ```

2. **Publicar e Consumir Mensagens**:
   - O código C# acima publica e consome mensagens de um `Topic Exchange` no RabbitMQ rodando no contêiner Docker.
   - Acesse a interface de gerenciamento em `http://localhost:15672` com as credenciais `guest/guest` para monitorar suas exchanges, filas e mensagens.
