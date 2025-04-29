# MonitoramentoRede

Um projeto para monitoramento de rede em tempo real, utilizando ASP.NET Core e SignalR.

## Funcionalidades

- Monitoramento de status de portas de rede.
- Atualizações em tempo real via SignalR.
- Interface web para visualização do status.

## Estrutura do Projeto

```
MonitoramentoRede/
├── Controllers/          # Controladores MVC
├── Hubs/                 # Hubs SignalR e serviços de monitoramento
├── Models/               # Modelos de dados
├── Views/                # Arquivos de visualização (Razor)
├── wwwroot/              # Arquivos estáticos (CSS, JS, etc.)
├── appsettings.json      # Configurações do aplicativo
├── Program.cs            # Ponto de entrada da aplicação
├── MonitoramentoRede.sln # Solução do projeto
```

## Requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## Configuração

1. Clone o repositório:

   ```bash
   git clone https://github.com/seu-usuario/MonitoramentoRede.git
   cd MonitoramentoRede
   ```

2. Restaure os pacotes NuGet:

   ```bash
   dotnet restore
   ```

3. Configure o arquivo `appsettings.json` com as informações necessárias para o monitoramento.

## Execução

1. Execute o projeto:

   ```bash
   dotnet run
   ```

2. Acesse o aplicativo no navegador em [http://localhost:5000](http://localhost:5000).

## Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues ou enviar pull requests.

## Licença

Este projeto está licenciado sob a [MIT License](LICENSE).

## Contato

Para dúvidas ou sugestões, entre em contato pelo e-mail: `web.marcelo@gmail.com`.
