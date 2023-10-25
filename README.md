Arquitetura orientada a eventos (EDA) é um padrão de design de software que permite a uma organização detectar “eventos” ou momentos de negócios importantes (como uma transação, visita ao site, abandono do carrinho de compras, etc.) e agir sobre eles em tempo real ou quase real

Uma blank Solution – AppEventDriven

Dois projetos Web API  (SQLite)
ApiPostService  (consome eventos)  
ApiUserService  (publica mensagens)

Pacotes nugtes:
Microsoft.EntityFrameworkCore.Sqlite
RabbitMQ.Client
Newtonsoft.Json
Swashbuckle.AspNetCore

Objetivo: Consumir eventos do RabbitMQ na api ApiPostService que serão publicados a partir da api ApiUserService

RabbitMQ no Docker: docker run -d  
                     -p 15672:15672 -p 5672:5672 
                     --hostname my-rabbit 
                     --name mac-rabbit 
                     rabbitmq:3-management

Roteiro para testar a implementação:

1 - Invocar ApiUserService e adicionar um usuário ao banco de dados do users.db   
   - O serviço criará um evento que ApiPostService vai consumir e adicionará o usuário em posts.db

2 - Acessar ApiPostService e adicionar um post para o usuário que foi incluído

3 - Em ApiPostService e carregar o post (GET) e o usuário do banco de dados de posts.db

4 - Chamar ApiUserService e alterar o nome do usuário incluído   
   - O serviço criará um evento que ApiPostService consome e atualizando o nome do usuário em posts.db

5 - Invocar ApiPostService e carregar o post (GET) e o usuário renomeado em posts.db






  

