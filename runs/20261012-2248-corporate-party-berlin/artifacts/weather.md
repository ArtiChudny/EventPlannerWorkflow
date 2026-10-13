# Weather Outlook

## Event Date and Location

Saturday 2026-12-12, 18:00-23:00, Berlin, Germany (geocoded to 52.52437 N, 13.41053 E, timezone Europe/Berlin).

## Data Basis

The event is 61 days away (today is 2026-10-12), beyond the roughly 16-day forecast range, so this is NOT a forecast. It is climatology from one `weather_archive` call (ERA5 reanalysis) for 2025-12-10 to 2025-12-14 at the coordinates above, daily temperature_2m_max/min, precipitation_sum and wind_gusts_10m_max, timezone Europe/Berlin. This is a single-year sample only (brief mode), so treat it as indicative, not statistically representative.

## Outlook Summary

Same dates in 2025 (12-12 itself: max 6.2 C, min 2.7 C, 0 mm rain, gusts 24.8 km/h). Across 10-14 Dec 2025: highs 6.2-11.5 C, lows 2.7-8.5 C, rain only on 10 Dec (0.7 mm), gusts 24.8-40.7 km/h. Evenings in Berlin in mid-December are typically cold and near freezing at times (general expectation, unverified (estimate)). Guest-travel note: advise warm coats and sturdy footwear; allow extra transit time if frost or snow occurs. Re-check with `weather_forecast` from about 2026-11-26 (16 days out).

## Outdoor Risk Level

low (indoor event)

## Backup Recommendation

No outdoor segments, so no weather fallback is needed. Only guest-travel advice: share a note on public transport options and coat check, and re-check the forecast about 3 days before the event for ice or snow disruption.

## Sources

- [Open-Meteo Historical Weather API (ERA5 archive endpoint)](https://archive-api.open-meteo.com/v1/archive) - supports: Berlin 10-14 Dec 2025 temperature, precipitation and gust figures (called via the open-meteo MCP server, `weather_archive`; documented base URL of the API)
- [Open-Meteo Geocoding API](https://geocoding-api.open-meteo.com/v1/search) - supports: Berlin coordinates and timezone (called via the open-meteo MCP server, `geocoding`; documented base URL of the API)
