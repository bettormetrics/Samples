import json
import logging
import os
import configparser
from azure.eventhub import EventHubConsumerClient
import datetime

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

# Enable this to define variables with the connection string, event hub name, and consumer group
# connection_str = '<your_event_hub_connection_string>'
# eventhub_name = '<your_event_hub_name>'
# consumer_group = '<your_event_hub_consumer_group>'

# Enable this to get your variables from environment variables
# connection_str = os.getenv('EVENT_HUB_CONN_STR')
# eventhub_name = os.getenv('EVENT_HUB_NAME')
# consumer_group = os.getenv('EVENT_HUB_CONSUMER_GROUP')

# Enable this to define a connection to Snowflake
# conn = snowflake.connector.connect(
#     user='<username>',
#     password='<password>',
#     account='<account_url>',
#     warehouse='<warehouse>',
#     database='<database>',
#     schema='<schema>'
# )

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


def on_event(partition_context, event):
    try:
        logger.info("Received event from partition {}".format(partition_context.partition_id))

        # Check if conn is valid
        if conn is not None:
            # Get a cursor object from the connection
            cur = conn.cursor()

            # Get the event data
            data = event.body

            # Convert the generator to a list
            data_list = list(data)

            # Decode bytes to string
            data_str = data_list[0].decode('utf-8')
            # print("data_str:", data_str) # -> Getting a valid json up to this point

            # Convert the JSON object back to a string
            data_json = json.loads(data_str)
            # print("data_json:", data_json)

            data_json_str = json.dumps(data_json)
            # print("data_json_str:", data_json_str)

            # query = f"INSERT INTO BM_EH_SINK (BM_EVENT_DATA) VALUES (PARSE_JSON('{data_json_str}'))"
            # sink_tbl = 'BM_EH_SINK'
            # event_data_col = 'BM_EVENT_DATA'

            query = f"INSERT INTO {sink_tbl} ({event_data_col}) SELECT PARSE_JSON(Column1) FROM VALUES ('{data_json_str}')"

            # Execute the query
            cur.execute(query)

            # Commit the transaction
            conn.commit()

        partition_context.update_checkpoint(event)
    except Exception as e:
        logging.error("Error processing event: {}".format(str(e)))
        # Handle the error or raise it to be caught by higher-level code


with client:
    try:
        client.receive(
            on_event=on_event,
            starting_position="-1",  # "-1" is from the beginning of the partition.
        )
        # receive events from specified partition:
        # client.receive(on_event=on_event, partition_id='0')
    except Exception as e:
        logging.error("Error receiving events: {}".format(str(e)))
        # Handle the error or raise it to be caught by higher-level code
