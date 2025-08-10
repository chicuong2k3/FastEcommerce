# Ecommerce

## Modules 

### Catalog 

### Inventory


# Bugs

- Update category not reflecting the updated info in the UI.
- Does not notify when add attribute fails.
- 

# Paging challenges in CQRS

## Client-side vs Server-side paging

When should we use client-side paging and when should we use server-side paging?

Client-side paging is suitable for small datasets because all data is loaded at once and paginated locally (on the client).
This approach is fast and simple since it avoids round-trips to the server when switching pages. 
Additionally, when a new item is created, it can be immediately appended to the existing list on the client, resulting in a responsive UI update.

However, for large datasets, client-side paging becomes inefficient due to high memory usage and slow initial load times. 
In such cases, server-side paging is more appropriate as it only loads a subset of data per request. 
This reduces client memory consumption and improves scalability, although it introduces latency due to additional requests for each page change or item creation.

One caveat with server-side paging is the uncertainty of where a newly created item will appear, especially when the data is sorted.
Since the client has no context of the overall sorting and paging rules, it cannot determine the correct position of the new item. 
As a result, displaying it immediately without re-fetching data may lead to inconsistencies in the user interface.

For example, suppose the items are sorted in descending order by price:
```json
[
    { "id": 1, "name": "item1", "price": 30000 },
    { "id": 2, "name": "item2", "price": 25000 },
    { "id": 3, "name": "item3", "price": 20000 },
    { "id": 4, "name": "item4", "price": 15000 },
    { "id": 5, "name": "item5", "price": 10000 }, 
    { "id": 6, "name": "item6", "price": 5000 }, 
]
```

Each page contains 3 items. 
If the user is currently viewing page 2, the displayed items are:
```json
[
    { "id": 4, "name": "item4", "price": 15000 },
    { "id": 5, "name": "item5", "price": 10000 }, 
    { "id": 6, "name": "item6", "price": 5000 }, 
]
```

Now, if a new item is created with a price of 22000, it logically belongs to the first page due to the descending sort order.
However, if the server simply returns the created item to the client and the client blindly appends it to the current list, 
the UI will display incorrect data, the new item will appear on the wrong page.

## CQRS and the server-side paging challenge

This issue becomes more complex when using a CQRS architecture, where the write database is updated immediately, but the read database is synchronized asynchronously.
In the case of client-side paging, handling the addition of a new item is relatively straightforward.
However, with server-side paging, re-fetching the current page right after item creation may not reflect the newly added item, as it may not yet be available in the read database due to replication lag.
This creates a temporary inconsistency between the expected and actual data shown to the user.

Here are a few potential solutions I've considered to address this issue:

# Inventory Problem

There are two main challenges in the inventory management system:
- The data consistency: The system does not allow sell over the quantity of a product.
This is difficult to achieve because:
	- There are millions products and each product may be in multitle warehouses.
	- The inventory is updated frequently, especially during peak times.
- The complexity when integrating with other services such as checkout, delivery.

To tackle these challenges, here are some approaches:
- Using Local Memory and Non-Blocking Processing
	- The inventory is stored in local memory, which allows for fast access and updates.
	- The system uses non-blocking processing to handle the inventory updates, which allows for high throughput and low latency.

- Using a Single Command Queue and Single Master Model
- Saga pattern



# Checkout process

![Checkout process](./docs/images/checkout_process_diagram.svg)