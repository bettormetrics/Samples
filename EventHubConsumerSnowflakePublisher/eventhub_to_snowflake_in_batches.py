import json
import logging
import os
import configparser
from azure.eventhub import EventHubConsumerClient
import datetime
import asyncio

import snowflake.connector

from azure.eventhub import EventHubConsumerClient
from azure.eventhub.extensions.checkpointstoreblob import BlobCheckpointStore
from azure.storage.blob import BlobServiceClient

# Read configuration from a file
config = configparser.ConfigParser()
config.read('config.ini')

connection_str = config.get('EventHub', 'EVENT_HUB_CONN_STR')
eventhub_name = config.get('EventHub', 'EVENT_HUB_NAME')
consumer_group = config.get('EventHub', 'EVENT_HUB_CONSUMER_GROUP')

# Read Azure Storage configuration from config.ini
storage_connection_str = config.get('AzureStorage', 'STORAGE_CONNECTION_STR')
storage_container_name = config.get('AzureStorage', 'STORAGE_CONTAINER_NAME')

# Create a BlobServiceClient object which will be used to create a container client
blob_service_client = BlobServiceClient.from_connection_string(storage_connection_str)

# Create a BlobCheckpointStore using the BlobServiceClient and the container name
checkpoint_store = BlobCheckpointStore.from_connection_string(storage_connection_str, storage_container_name)

# Read Snowflake configuration from config.ini
snowflake_user = config.get('Snowflake', 'SNOWFLAKE_USER')
snowflake_password = config.get('Snowflake', 'SNOWFLAKE_PASSWORD')
snowflake_account_url = config.get('Snowflake', 'SNOWFLAKE_ACCOUNT_URL')
snowflake_warehouse = config.get('Snowflake', 'SNOWFLAKE_WAREHOUSE')
snowflake_database = config.get('Snowflake', 'SNOWFLAKE_DATABASE')
snowflake_schema = config.get('Snowflake', 'SNOWFLAKE_SCHEMA')
sink_tbl = config.get('Snowflake_Database', 'SINK_TBL')
event_data_col = config.get('Snowflake_Database', 'EVENT_DATA_COL')

# Establish a connection to Snowflake
conn = snowflake.connector.connect(
    user=snowflake_user,
    password=snowflake_password,
    account=snowflake_account_url,
    warehouse=snowflake_warehouse,
    database=snowflake_database,
    schema=snowflake_schema
)

try:
    client = EventHubConsumerClient.from_connection_string(connection_str, consumer_group, eventhub_name=eventhub_name, checkpoint_store=checkpoint_store)
except Exception as e:
    logging.error("Failed to create EventHubConsumerClient: {}".format(str(e)))
    # Handle the error or raise it to be caught by higher-level code

logger = logging.getLogger("azure.eventhub")
logging.basicConfig(level=logging.INFO)

def on_event_batch(partition_context, events):
    try:
        logger.info("Received event batch from partition {}".format(partition_context.partition_id))

        # Check if conn is valid
        if conn is not None:
            # Get a cursor object from the connection
            cur = conn.cursor()

            # Initialize a list to hold the data for all events in the batch
            batch_data = []

            for event in events:
                # Get the event data
                data = event.body

                # Convert the generator to a list
                data_list = list(data)

                # Decode bytes to string
                data_str = data_list[0].decode('utf-8')

                # Convert the JSON object back to a string
                data_json = json.loads(data_str)

                # Convert the JSON object back to a string, replacing single quotes with two single quotes
                data_json_str = json.dumps(data_json).replace("'", "''")

                # Add the data for this event to the batch data
                batch_data.append(f"('{data_json_str}')")

            # Create the query to insert all events in the batch
            query = f"INSERT INTO {sink_tbl} ({event_data_col}) SELECT PARSE_JSON(Column1) FROM VALUES {', '.join(batch_data)}"

            # Execute the query
            cur.execute(query)

            # Commit the transaction
            conn.commit()

        partition_context.update_checkpoint()
    except Exception as e:
        logging.error("Error processing event: {}".format(str(e)))
        # Handle the error or raise it to be caught by higher-level code


with client:
    try:
        client.receive_batch(
            on_event_batch=on_event_batch,
            starting_position="-1",  # "-1" is from the beginning of the partition.
            max_batch_size=100
        )
        # receive events from specified partition:
        # client.receive(on_event=on_event, partition_id='0')
    except Exception as e:
        logging.error("Error receiving events: {}".format(str(e)))
        # Handle the error or raise it to be caught by higher-level code
