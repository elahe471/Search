# Search Service

A lightweight catalog search microservice built with **.NET 10**, **Elasticsearch**, **RabbitMQ**, and **MassTransit**.

The service consumes catalog integration events, keeps Elasticsearch synchronized, and exposes search-focused APIs for an e-commerce catalog.

## Architecture

```text
Catalog Service
      |
      | Integration Events
      v
   RabbitMQ
      |
      v
 Search Service
      |
      v
 Elasticsearch
```

Catalog changes are published asynchronously through RabbitMQ.
The Search service consumes those events and creates, updates, or deletes the corresponding Elasticsearch documents.

## Features

* Full-text search across multiple catalog fields
* Field boosting for better relevance
* Automatic fuzziness for common typing mistakes
* Pagination
* Case-insensitive Brand and Category filters
* Autocomplete / search suggestions
* Brand and Category facets using Elasticsearch aggregations
* Highlighted search results
* Elasticsearch health checks
* Catalog item create/update/delete synchronization through integration events

## Search Strategy

The main search uses Elasticsearch `MultiMatch` with different relevance weights:

| Field       | Boost |
| ----------- | ----: |
| Name        |    4x |
| Brand       |    3x |
| Category    |    2x |
| Description |    1x |

```csharp
.MultiMatch(mm => mm
    .Query(request.Query)
    .Fields(new[]
    {
        "name^4",
        "catalogBrand^3",
        "catalogCategory^2",
        "description"
    })
    .Fuzziness(new Fuzziness("AUTO"))
)
```

This keeps product-name matches more relevant while still searching brand, category, and description.

## API Endpoints

| Endpoint                         | Purpose                                      |
| -------------------------------- | -------------------------------------------- |
| `GET /api/v1/search`             | Full-text search with pagination and filters |
| `GET /api/v1/search/suggestions` | Search-as-you-type autocomplete              |
| `GET /api/v1/search/facets`      | Brand and Category aggregation counts        |
| `GET /api/v1/search/highlight`   | Search results with highlighted matches      |
| `GET /health/live`               | API liveness                                 |
| `GET /health/ready`              | Dependency readiness                         |

## Search Filters

Brand and Category are applied as filters, so they restrict the result set without affecting relevance scoring.

```text
Bool Query
├── Must
│   └── MultiMatch
└── Filter
    ├── Brand
    └── Category
```

Filtering is case-insensitive.

## Autocomplete

Autocomplete uses `MatchBoolPrefix` on product names for search-as-you-type scenarios.

Suggestion responses include product name, category, brand, and URL.

## Facets

The facets endpoint uses Elasticsearch `terms` aggregations on keyword fields to return Brand and Category counts.

## Highlighting

Highlighting is exposed through a dedicated endpoint so the main search endpoint stays lightweight.

```html
Apple <mark>iPhone</mark> 16 Pro
```

## Event Synchronization

The Search service handles:

```text
CatalogItemAddedEvent
CatalogItemChangedEvent
CatalogItemDeletedEvent
```

Deleted catalog items are removed from Elasticsearch using the slug as the document ID.

## Tech Stack

* .NET 10
* Elasticsearch
* RabbitMQ
* MassTransit
* Docker
* ASP.NET Core Minimal APIs

## Next Steps

Future improvements may include sorting, custom analyzers, synonyms, and semantic or hybrid search.
::: 
