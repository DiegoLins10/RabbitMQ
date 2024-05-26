# DevTrackR - Curso Microsserviços com ASP.NET Core

Está sendo desenvolvido um sistema utilizando arquitetura de microsserviços, implementando funcionalidades relacionadas a envio de pacotes.

## Tecnologias e práticas utilizadas
- ASP.NET Core com .NET 7
- MongoDB
- Swagger
- Injeção de Dependência
- Padrão Repository
- Envio de E-mails com SendGrid
- Mensageria com RabbitMQ

## Funcionalidades
- Cadastro e Detalhes de Pacote
- Cadastro de Atualização de Pacote
- Notificação por E-mail



No RabbitMQ, um **exchange** é responsável por roteamento de mensagens para uma ou mais filas (queues). O mecanismo de roteamento depende do tipo de exchange e da chave de roteamento (routing key) fornecida com a mensagem. Aqui está uma visão geral de como exchanges e chaves de roteamento funcionam no RabbitMQ:

### Tipos de Exchange

1. **Direct Exchange (Exchange Direto)**
   - **Chave de Roteamento (Routing Key):** A chave de roteamento deve corresponder exatamente à chave de ligação (binding key) da fila.
   - **Caso de Uso:** O direct exchange é usado quando a mensagem precisa ir para uma fila específica.
   - **Exemplo:**
     ```javascript
     exchange.bindQueue(queue, exchange, 'minha.chave.de.roteamento')
     ```

2. **Fanout Exchange (Exchange de Distribuição)**
   - **Chave de Roteamento:** A chave de roteamento é ignorada. A mensagem é roteada para todas as filas vinculadas ao exchange.
   - **Caso de Uso:** O fanout exchange é usado para transmitir mensagens para várias filas.
   - **Exemplo:**
     ```javascript
     exchange.bindQueue(queue, exchange)
     ```

3. **Topic Exchange (Exchange de Tópico)**
   - **Chave de Roteamento:** A chave de roteamento é comparada com o padrão da chave de ligação. A chave de roteamento pode conter palavras separadas por pontos (`.`), e as chaves de ligação podem conter caracteres curinga (`*` para um palavra e `#` para zero ou mais palavras).
   - **Caso de Uso:** O topic exchange é usado para roteamento de mensagens baseado em padrões complexos.
   - **Exemplo:**
     ```javascript
     exchange.bindQueue(queue, exchange, 'corpo.*.noticia')
     exchange.bindQueue(queue, exchange, 'corpo.#')
     ```

4. **Headers Exchange (Exchange de Cabeçalhos)**
   - **Chave de Roteamento:** As chaves de roteamento são ignoradas. Em vez disso, as mensagens são roteadas com base nos cabeçalhos HTTP.
   - **Caso de Uso:** O headers exchange é usado quando o roteamento precisa ser feito com base nos atributos de cabeçalho das mensagens.
   - **Exemplo:**
     ```javascript
     exchange.bindQueue(queue, exchange, headers={"tipo": "pdf", "x-match": "all"})
     ```

### Exemplo Prático

Vamos criar um exemplo onde uma mensagem é enviada para uma fila específica usando um direct exchange:

```python
import pika

# Conexão com o servidor RabbitMQ
connection = pika.BlockingConnection(pika.ConnectionParameters('localhost'))
channel = connection.channel()

# Declaração do exchange
channel.exchange_declare(exchange='meu_direct_exchange', exchange_type='direct')

# Declaração da fila
channel.queue_declare(queue='minha_fila')

# Ligação da fila ao exchange com uma chave de roteamento
channel.queue_bind(exchange='meu_direct_exchange', queue='minha_fila', routing_key='minha.chave.de.roteamento')

# Envio de uma mensagem
channel.basic_publish(exchange='meu_direct_exchange', routing_key='minha.chave.de.roteamento', body='Olá, Mundo!')

# Fechamento da conexão
connection.close()
```

Neste exemplo:
- Um exchange do tipo `direct` chamado `meu_direct_exchange` é declarado.
- Uma fila chamada `minha_fila` é declarada.
- A fila é vinculada ao exchange com a chave de roteamento `minha.chave.de.roteamento`.
- Uma mensagem com o corpo 'Olá, Mundo!' é enviada para o exchange com a mesma chave de roteamento, resultando na entrega da mensagem à fila `minha_fila`.

Essa é uma visão geral básica. Dependendo do cenário, você pode usar diferentes tipos de exchanges e estratégias de roteamento conforme necessário.