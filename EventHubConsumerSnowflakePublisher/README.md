# Azure Event Hub Consumer with Snowflake Integration

These Python scripts consume events from an Azure Event Hub and store the event data in a Snowflake database. One script processes the events in batches (`eventhub_to_snowflake_in_batches.py`), while the other (`eventhub_to_snowflake.py`) processes the events individually.

## Dependencies

- `json`
- `logging`
- `os`
- `configparser`
- `azure.eventhub`
- `datetime`
- `snowflake.connector`

## Configuration

The scripts read configuration details from a `config.ini` file. This includes connection details for the Azure Event Hub, Azure Storage, and Snowflake.

### Azure Event Hub Configuration

The following details are required for the Azure Event Hub:

- `EVENT_HUB_CONN_STR`: The connection string for the Event Hub.
- `EVENT_HUB_NAME`: The name of the Event Hub.
- `EVENT_HUB_CONSUMER_GROUP`: The consumer group of the Event Hub.

### Azure Storage Configuration

The following details are required for Azure Storage (used for checkpointing):

- `STORAGE_CONNECTION_STR`: The connection string for the Azure Storage account.
- `STORAGE_CONTAINER_NAME`: The name of the blob container in the Azure Storage account.

### Snowflake Configuration

The following details are required for Snowflake:

- `SNOWFLAKE_USER`: The username for the Snowflake account.
- `SNOWFLAKE_PASSWORD`: The password for the Snowflake account.
- `SNOWFLAKE_ACCOUNT_URL`: The URL for the Snowflake account.
- `SNOWFLAKE_WAREHOUSE`: The warehouse to use in Snowflake.
- `SNOWFLAKE_DATABASE`: The database to use in Snowflake.
- `SNOWFLAKE_SCHEMA`: The schema to use in Snowflake.
- `SINK_TBL`: The table in Snowflake where the event data will be stored.
- `EVENT_DATA_COL`: The column in the Snowflake table where the event data will be stored.

## Checkpoint Store

The scripts use Azure Blob Storage as a checkpoint store. Checkpointing is a process by which readers mark or commit their position within a partition event sequence. Checkpointing is the responsibility of the consumer and occurs on a per-partition basis within a consumer group. This responsibility means that for each consumer group, each partition reader must keep track of its current position in the event stream, and can inform the service when it considers the data stream complete.

If a reader disconnects from a partition, when it reconnects it begins reading at the checkpoint that was previously submitted by the last reader of that partition in the consumer group. When the reader connects, it passes the offset to the event hub to specify the location at which to start reading. In this way, you can use checkpointing to both mark events as "complete" by downstream applications, and to provide resiliency if a failover between readers running on different machines occurs. It can also be used to provide a recovery mechanism from failures that happen downstream from the event hub.

## Usage

Run the scripts using a Python interpreter. The scripts will start consuming events from the specified Azure Event Hub and store the event data in the specified Snowflake database.

## Error Handling

The scripts include error handling for creating the EventHubConsumerClient and receiving events. If an error occurs, it will be logged and can be used for troubleshooting.
