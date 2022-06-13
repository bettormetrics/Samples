# Link Consumer
This sample demonstrates a connection to your tenant links event hub, deserializing entities, and caching link state in-memory.

## Usage
You'll need to create an Azure Storage account if you want to use the blob storage checkpointer, then create a _config.json_ file in the base directory with the following settings:
```code
{
  "blobConnectionString": "DefaultEndpointsProtocol=https;AccountName={NAME};AccountKey={KEY};EndpointSuffix=core.windows.net",
  "blobContainerName": "linkconsumersample",
  "eventHubConnectionString": "Endpoint=sb://{TENANT}.servicebus.windows.net/;SharedAccessKeyName=Listen;SharedAccessKey={KEY}",
  "eventHubConsumerGroup": "linkconsumersample",
  "eventHubName": "links"
}
```

## Relational Entity Model
```mermaid
graph TD;
    Sport-->Competition;
    Competition-->Fixture;
    Fixture-->Market;
    Market-->Selection;
    Fixture-->BookmakersFixture;
    Market-->BookmakersMarket;
    Selection-->BookmakersSelection;
    style BookmakersFixture fill:#008844
    style BookmakersMarket fill:#008844
    style BookmakersSelection fill:#008844
```

## Denormalized Message Model (partitioned by fixture)
```mermaid
graph TD;
  subgraph fixture
    Fixture-->Competition;
    Fixture-->Participant;
  end
  subgraph market
    Market;
  end
  subgraph selection
    Selection;
  end
  subgraph bookmakersFixture
    BookmakersFixture;
  end
  subgraph bookmakersMarket
    BookmakersMarket;
  end
  subgraph bookmakersSelection
    BookmakersSelection;
  end
    style Fixture fill:#884400
    style Market fill:#884400
    style Selection fill:#884400
    style BookmakersFixture fill:#884400
    style BookmakersMarket fill:#884400
    style BookmakersSelection fill:#884400
```
