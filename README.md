# RabbitMQer
RabbitMQ implementation in .NET

## Prerequisites:
1. Install **Erlang** (http://www.erlang.org/downloads - otp_win64_19.3.exe)
2. Install RabbitMQ **server** (http://www.rabbitmq.com/install-windows.html - rabbitmq-server-3.6.10.exe)
3. Enable RabbitMQ management **plugin** (web UI at: http://server-name:15672): 
```
C:\Program Files\RabbitMQ Server\rabbitmq_server-3.6.10\sbin>rabbitmq-plugins enable rabbitmq_management
```
4. Restart RabbitMQ **service** (services.msc)
5. If the following error occures while building the project: _Expected coreclr library not found in package graph_ - restore libriaries in **Package Manager Console**:
```
dotnet restore
```
