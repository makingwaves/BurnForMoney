> The project manages all operations related to athlete context.

### How it works?

Command handlers receive commands from external sources, commands are processed and data are persisted in the event store (Azure Table Storage). Then events are published to Event Grid topic.