# Maurice
An event ingestor and reader written in .NET

## How does it works?

It can be used for event ingestion and read. It is made of 3 parts:

- Writer
- Reader
- Scheduler

It contains the following entities:

- EventTypeEntity
- EventEntity
- ErrorEntity
- ScheduledEntity
  
### Writer
This part exposes the **WriterProcessor** that gived an object of type **T** validates it, retrieves the **EventTypeEntity** configured and serialize it on the choosed storage as **EventEntity**.
In case of error an **ErrorEntity** will be written.

### Reader
This part exposes the **ReaderProcessor** that gived an object of type **T** validates it, retrieves the **EventTypeEntity** configured and reads from the choosed storage as **EventEntity** using the date range and order by timestamp ASC/DESC by the parameter featured.

### Scheduler
This part exposes the **SchedulerProcessor**.
The scheduler processor retrieves from every **EventTypeEntity** the schedule rules and runs the implementations of **IScheduleProcessor** as cron jobs.
The result of these implementations will be serialized it on the choosed storage as **ScheduledEntity**.

###TODO

- Scheduler.
- Storage implementations for other service (as SqlServer, Cosmos, etc..)
- Unit tests.
- Move the serialize logic to a message queue.
- Auth.
- Implement an horizontal scalability mechanism for reader. What i want to do? I'd like to have one database that is for writings. And a lot of distributed copies read-only. I think that the read capability it's more important in this context instead of writing one.
