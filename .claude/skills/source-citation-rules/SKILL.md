---
name: source-citation-rules
description: Rules for citing sources in event-planning artifacts - what counts as a real source, how to format citations, and how to mark unverifiable figures as "unverified (estimate)". Use whenever an agent researches venues, vendors, prices or weather.
---

# source-citation-rules

## Hard rules

1. **Never invent a URL.** Only cite a URL that a WebSearch result returned, that you fetched with WebFetch, or that
   an MCP tool response provided. If you cannot reach it, do not cite it.
2. **Every recommendation has a source.** A recommendation is any named venue, caterer, vendor, performer, rental,
   or price quote. It needs an `http://` or `https://` URL on the same line (table row or bullet) or in its own
   `**Source:**` line.
3. **Link the specific page** (the venue's capacity page, the menu page), not just a homepage, when such a page exists.
4. **Unverified data is labelled.** A figure not found on a fetched page (price, capacity, availability, a typical
   market rate) is written exactly as `unverified (estimate)` right next to the number, with a one-line basis, for example
   `~45 EUR per head (unverified (estimate), based on typical Berlin catering ranges)`.
   An unverified figure never replaces a sourced one, and a vendor with no source at all is not recommended.
5. **Record access.** In the `Sources` section list each URL once, with a short title and what it supports.
   Weather data from an MCP tool is cited by naming the tool and the API endpoint it reports (for example
   `Open-Meteo forecast API via the open-meteo MCP server`) and, when the tool returns one, its URL.
6. **Prices carry currency and basis** (per head, per hour, flat) and the date they were retrieved if the page shows one.

## Source quality order

Official venue/vendor page > reputable listing or marketplace > news or review article > general estimate (label it).

## Sources section format

```
## Sources

- [Venue name - capacity and rates](https://example.com/page) - supports: capacity 120, hire price
- Open-Meteo forecast API (via open-meteo MCP, weather_forecast) - supports: temperature and rain outlook
```

## What the checker does and does not do

`check-artifact` verifies that URLs are present. Whether they are real is the validator agent's WebFetch spot-check,
so cite only what you actually saw.
