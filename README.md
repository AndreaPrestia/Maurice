# Maurice
An event ingestor and reader written in .NET

## How does it works?

It can be used for event ingestion and read. It is made of two parts:

- Writer
- Reader

It contains the following entities:

- EventTypeEntity
- EventEntity
- ErrorEntity
  
### Writer
This part exposes the **WriterProcessor** that gived an object of type **T** validates it, retrieves the **EventTypeEntity** configured and serialize it on the choosed storage as **EventEntity**.
In case of error an **ErrorEntity** will be written.

### Reader
This part exposes the **ReaderProcessor** that gived an object of type **T** validates it, retrieves the **EventTypeEntity** configured and reads from the choosed storage as **EventEntity** using the date range and order by timestamp ASC/DESC by the parameter featured.

### TODO

- Move the serialize logic to a message queue.
- Storage implementations for other service (as SqlServer, Cosmos, etc..)
- Implement an horizontal scalability mechanism for reader. What i want to do? I'd like to have one database that is for writings. And a lot of distributed copies read-only. I think that the read capability it's more important in this context instead of writing one.
