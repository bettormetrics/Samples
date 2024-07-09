# Azure Event Hub Consumer with Snowflake Integration

This Python script consumes events from an Azure Event Hub and stores the event data in a Snowflake database.

## Dependencies

- `json`
- `logging`
- `os`
- `configparser`
- `azure.eventhub`
- `datetime`
- `snowflake.connector`

## Configuration

The script reads configuration details from a `config.ini` file. This includes connection details for the Azure Event Hub and Snowflake.

### Azure Event Hub Configuration

The following details are required for the Azure Event Hub:

- `EVENT_HUB_CONN_STR`: The connection string for the Event Hub.
- `EVENT_HUB_NAME`: The name of the Event Hub.
- `EVENT_HUB_CONSUMER_GROUP`: The consumer group of the Event Hub.

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

## Usage

Run the script using a Python interpreter. The script will start consuming events from the specified Azure Event Hub and store the event data in the specified Snowflake database.

## Error Handling

The script includes error handling for creating the EventHubConsumerClient and receiving events. If an error occurs, it will be logged and can be used for troubleshooting.
