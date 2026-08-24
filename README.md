# Catalog Search Service

This project provides asynchronous catalog indexing and full-text product search using **RabbitMQ**, **MassTransit**, and **Elasticsearch**.

## Architecture

```text
Catalog Service
      |
      | Integration Event
      v
RabbitMQ
      |
      v
Search Consumer
      |
      v
Elasticsearch
```

When a catalog item is created or updated, the Catalog service publishes an integration event to RabbitMQ.  
The Search service consumes the event and stores the searchable document in Elasticsearch.

This keeps catalog operations independent from search indexing and allows indexing to run asynchronously.

---

## Search Strategy

The product search uses Elasticsearch `MultiMatch` across multiple fields:

```csharp
var response = await elasticsearch.SearchAsync<CatalogItemIndex>(s => s
    .Indices(CatalogItemIndex.IndexName)
    .From(0)
    .Size(10)
    .Query(q => q
        .MultiMatch(mm => mm
            .Query(searchText)
            .Fields(new[]
            {
                "name^4",
                "catalogBrand^3",
                "catalogCategory^2",
                "description"
            })
            .Fuzziness(new Fuzziness("AUTO"))
        )
    )
);
```

### Field Priority

| Field | Boost |
|---|---:|
| Name | 4x |
| Brand | 3x |
| Category | 2x |
| Description | 1x |

A match in the product name is considered more relevant than the same match in the description.

For example, when searching for `iphone`:

```text
Apple iPhone 16 Pro
```

should rank higher than:

```text
USB-C Charger
Description: Compatible with iPhone
```

---

## Why MultiMatch?

Several Elasticsearch query types were considered:

- `Term` — exact values and filters
- `Match` — full-text search on one field
- `Fuzzy` — typo-tolerant term matching
- `Prefix` — autocomplete scenarios
- `Range` — price/date filtering
- `Bool` — combines search and filters
- `MultiMatch` — full-text search across multiple fields

`MultiMatch` was selected because the search box needs to search **Name, Brand, Category, and Description at the same time** while still ranking the most relevant products first.

`AUTO` fuzziness is enabled to tolerate common typing mistakes such as:

```text
iphone -> ipone
samsung -> samsng
```

---

## Search vs Filter

Full-text search is responsible for relevance:

```text
Name
Brand
Category
Description
```

Structured constraints should be added as filters:

```text
Brand
Category
Price
Availability
```

The future query structure will therefore be:

```text
Bool Query
├── MultiMatch
└── Filters
    ├── Brand
    ├── Category
    ├── Price
    └── Availability
```

This keeps relevance scoring separate from filtering.

---

## Future Improvements

Possible future additions:

- Autocomplete
- Highlighting
- Brand/category aggregations
- Price filters
- Synonyms
- Semantic or hybrid search

---

## Tech Stack

- .NET
- Elasticsearch
- RabbitMQ
- MassTransit
- Docker
