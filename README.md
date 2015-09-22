# MiniNet

MiniNet is a library which allows developers to build different types of networking applications.

## Features

The idea is to allow easy support for various Transport Layers and Application Layers as outlined below.

- Transport Layers
  - InVM (within same Virtual Machine)
  - TCP
  - UDP
- Application Layers
  - IPC (Named Pipes)
  - HTTP
  - FTP
  - SMTP
  - SSH
  - DNS
  - XMPP
  - WebSockets

Additional features would also need to be provided such as:
- Encoding (UTF8, UTF16, Unicode, etc)
- Compression
- Buffer Management
- Traffic Control
- Exception Management
- Message Codecs (Encoders and Decoders)
- Security (TLS, Encryption/Decryption)
- Threading Strategies (Single, Thread Pool, Multi-Pool)

## Library Use-Cases

Perhaps one of the main reasons for creating MiniNet was to have a master-master clustering solution in .NET.
By clustering, I mean the ability to had and control cluster-wide caching, or distributed collections, or even service management on each node in the cluster.

Some idea that come to mind for clustering feature would be:
- Distributed Data Structures
  - List
  - Set
  - Map / MultiMap
  - Queue
  - Topic
  - Atomics
  - Locks
  - Semaphores
  - Latches
  - References (Weak / Strong)
- Distributed Threads
  - Timers
  - Threads
  - Jobs
- Distributed Events
- Distributed Transactions
- Cluster Management
  - Node Discovery / Service Discovery
  - Cluster Groups

Once you have all these basic components in place, you could even build an Enterprise Service Bus on top of MiniNet.

## License

The project is licensed under MIT.

## Notes

### Sockets

#### Basics

A socket is like a "handle" to a port which allows us to access data sent to that port.
There are four main steps in using a socket server with TCP:

1. Listen for connection requests on the server.
> The client machine's Windows TCP/IP subsystem will automatically assign an outgoing port to the socket on the client machine.
It will contact the server by sending a SYN packet which is addressed to the socket server's IP address and port number.
After a client initiates a connection on the server's listening socket, the Windows TCP/IP subsystem of the server will respond with SYN, ACK.
Then the client machine's Windows TCP/IP subsystem responds back with an ACK packet.
When the ACK is received by the server, the "handshake" is complete, and the connection is established.
Windows will handle this TCP/IP protocol stuff for you.
In other words, SYN, ACK, PSH, packets, and similar parts of TCP/IP protocol do not have to be coded by you.
The server's listening socket can maintain a queue of connection requests waiting to be accepted.
This queue is called the "backlog". The listening socket passes the connection info to another socket via an "accept" operation, 
and then gets the next incoming connection in the backlog queue, or if there is none, waits till there is a new connection request from a client.

2. Accept connection requests
> In order to have multiple connections on the same port, the server's listening socket must pass off the connection info to another socket, 
which accepts it. The accepting socket is not bound to the port. You post an accept operation to pass the connection from the listening socket 
to the accepting socket. The client does not need to perform an accept operation.

3. Receive/Send via the connection
> After the accept operation has completed, you can now receive or send data with that connection. "Receive" is also known as "read". "Send" is also referred to as "write".

4. Close the connection
> Either client or server can initiate an operation to close the connection.
Usually, the client would initiate that. Again, the lower level TCP/IP of the disconnect is handled by the Windows Operating System.
The connection can be closed using the Close method, which destroys the Socket and cleans up its managed and unmanaged resources.

With TCP, there is no guarantee that one send operation on the client will be equal to one receive operation on the server.
One send operation on the client might be equal to one, two, or more receive operations on the server.
And the same is true going back to the client from the server.
This peculiarity can be due to buffer size, network lag, and the way that the Operating System handles TCP to improve performance.
So you must have some way of determining where a TCP message begins and/or ends.

Three possible ways to handle TCP messages are:

1. Prefix every message with an integer that tells the length of the message.
2. All messages be fixed length. And both client and server must know the length before the message is sent.
3. Append every message with a delimiter to show where it ends. And both client and server must know what the delimiter is before the message is sent.

#### I/O Completion Ports (IOCP)

Microsoft created the SocketAsyncEventArgs class to help you write scalable, high performance socket server code.
SocketAsyncEventArgs uses I/O Completion Ports via the asynchronous methods in the .NET Socket class.

Why create a pool of the SocketAsyncEventArgs class?
The pool for receive/send operations should probably be at least equal to the maximum number of concurrent connections allowed.

#### Communication Protocol

What is our communication protocol in this code?

1. We will prefix every message with an integer that tells the length of the message.
2. One message from client will correspond with one message from the server.
3. After a connection is made, the client will send a message first, and then post a receive op to wait for the response from the server. And for each message that the client sends, the server will respond with a message to the client.

#### TCP Buffers

Buffers in TCP are unmanaged, that is, not controlled by the .NET Framework, but by the Windows system.
So the buffer gets "pinned" to one place in memory, thereby causing memory fragmentation, since the .NET Garbage Collector 
will not be able to collect that space. This situation is improved by putting all the buffers together in one block of memory, 
and just reusing that same space over and over. Also from what I remember is that the .NET Garbage Collector 
is a compacting collector and memory fragmentation will hurt any GC operations.

The theoretical maximum size for the buffer block is (2^31 = 2.147 GB), since it uses an integer data type.
For example, if you use a buffer size of 50,000 bytes, and have a separate buffer for send and receive,  then that is 100,000 bytes per connection.
2.147 GB divided by 100,000 bytes = 21,470, which would be the maximum number of concurrent connections that could use this 
buffer block with this buffer size and design.

#### Items still to discuss

- Framing
- What does "NoDelay" do?
- What does "LingerState" do?
- What does [SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true)] do?

## References & Acknowledgements

- http://www.codeproject.com/Articles/83102/C-SocketAsyncEventArgs-High-Performance-Socket-Cod
