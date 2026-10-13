# Weather Outlook

## Event Date and Location

- **Date:** Saturday 3 July 2027
- **Time window:** 17:30 (ceremony) to 00:00 (end), dinner about 20:30, dancing outdoors until midnight
- **Location:** Barcelona, Catalonia, Spain (geocoded as 41.38879 N, 2.15899 E, elevation 15 m; timezone Europe/Madrid)
- **Lead time from today (2026-10-13):** about 263 days, so the date is beyond the forecast range

## Data Basis

- **Forecast range:** Not applicable for this date: the event is about 263 days away, beyond the roughly 16-day forecast range and beyond the roughly 217-day seasonal forecast horizon. No forecast figures are given in this artifact.
- **Climatology (main basis):** `weather_archive` via the open-meteo MCP server, ERA5 reanalysis, `models=era5`, `timezone=Europe/Madrid`, daily variables `temperature_2m_max`, `temperature_2m_min`, `precipitation_sum`, `precipitation_hours`, `wind_speed_10m_max`, `wind_gusts_10m_max`. Window: 30 June to 6 July (plus or minus 3 days around 3 July) for each of 2021, 2022, 2023, 2024 and 2025, giving 35 days. The archive grid cell returned was 41.5 N, 2.25 E (elevation 44 m), roughly 10 to 15 km from the requested point.
- **Climate projection (context only):** `climate_projection` via the open-meteo MCP server, default CMIP6 models, daily `temperature_2m_max`, `temperature_2m_min`, `precipitation_sum`, `wind_speed_10m_max` for 27 June to 9 July 2027. Grid cell returned 41.40 N, 2.10 E. This is a single model-based scenario for one specific date range, not a forecast, and one daily value from a climate model is not a reliable prediction for 3 July 2027. Its daily maxima (24.6 to 30.6 C in the window) are in line with the climatology below.
- **Seasonal forecast:** Not used. The target date (about 263 days ahead) is beyond the tool's maximum horizon of 217 days.
- **Revision check (2026-10-13, budget revision):** The current budget limit is 22200 EUR (requirements.md). The budget change does not affect the climatology, the risk level or the backup timing, so the data basis and the risk level are unchanged. The step-free route relaxation is reflected in Backup Recommendation.
- **Important caveat:** These figures describe what early July in Barcelona has typically looked like in recent years. They are not a forecast for 3 July 2027, and the actual weather may differ.

### Five-year window summary (30 June to 6 July, 35 days)

| Year | Tmax mean (C) | Tmin mean (C) | Days with rain 1 mm or more | Largest daily rain (mm) | Max daily gust (km/h) |
|---|---|---|---|---|---|
| 2021 | 27.5 | 20.7 | 1 | 1.0 | 44.6 |
| 2022 | 28.4 | 21.6 | 1 | 10.4 | 36.4 |
| 2023 | 26.5 | 21.1 | 3 | 14.6 | 35.6 |
| 2024 | 25.5 | 20.5 | 3 | 9.2 | 35.3 |
| 2025 | 31.4 | 24.1 | 2 | 1.0 | 36.0 |
| All 35 days | 27.9 | 21.6 | 10 (about 29%) | 14.6 | 44.6 |

Additional facts from the same data:
- Daily maximum temperature ranged from 24.1 C to 32.5 C. Two days reached 32 C or more (both in 2025), and 9 of 35 days (about 26%) reached 30 C or more (2 in 2022, 7 in 2025).
- Daily minimum temperature ranged from 19.8 C to 24.5 C, so nights stayed warm in every year.
- Rain was light in most years: 20 of 35 days recorded some rain (any amount above zero), but only 10 days reached 1 mm or more, and 4 days reached 5 mm or more. The wettest single day was 14.6 mm (30 June 2023). Several rainy days were short showers with few hours of precipitation.
- Daily maximum wind speed stayed below about 34 km/h in all years (highest 33.3 km/h, 6 July 2022). Daily maximum gusts ranged from about 25 to 45 km/h.

## Outlook Summary

- **Heat:** Likely warm for the whole event. Daytime highs are typically 25 to 31 C, with a meaningful chance (about 1 in 4 days in this record) of 30 C or more. The ceremony at 17:30 falls in the hottest part of the day, so guests seated in full sun will feel it most. Warm evenings are expected: daily minima of 20 to 24 C suggest late-evening temperatures in the low-to-mid 20s C (unverified (estimate), inferred from the daily minima above).
- **Rain:** Lower-probability but not negligible. About 3 in 10 days in this record had 1 mm or more of rain, and the rain was usually brief. Heavy rain is uncommon but possible: three of the five years had a day with 9 mm or more. For an outdoor dinner and dance, a short shower is the main disruption to plan for.
- **Wind:** Usually light. Daily maximum gusts were at most 44.6 km/h (4 July 2021), so anchored lightweight structures such as a tent, marquee and decor should still be checked.
- **Darkness:** Sunset in early July in Barcelona is around 21:30 local time (unverified (estimate), not from a tool response). Outdoor dancing from about 22:00 to midnight will take place in darkness, so lighting for the garden and dance area is required.
- **Trend note:** The 2025 window was the hottest of the five years, with a daily-high mean of 31.4 C. This is one data point and does not establish a trend.

## Outdoor Risk Level

**Medium**

Reasoning:
- **Heat (main factor):** A ceremony at 17:30 in a garden in a typically warm July, with an approximately 1 in 4 chance of 30 C or more on a given day in this record. Guests with walking aids may need shade and rest points.
- **Rain (moderate factor):** About 29% of days in the record had 1 mm or more, and 4 of 35 days (about 11%) reached 5 mm or more. Short showers during the dinner or dancing would require a fast move under the covered backup.
- **Wind (low factor):** Gusts were moderate in every year of the record.
- **Darkness (planning factor):** Dancing after about 22:00 will be in darkness, so lighting is required but not a risk on its own.

## Backup Recommendation

- **Covered backup (must be used in the plan):** The covered backup space is a tent at Hostal Rural Mas Blanc, the recommended venue. It must hold all 60 guests seated for dinner and leave room for dancing. It is the backup for rain and for extreme heat during the day. The tent is not priced in this artifact; pricing belongs to the budget and venue artifacts. The confirmed requirements say the backup must be priced within the 22200 EUR total, so this gap is open (see Sources notes).
- **Step-free route to the backup:** The step-free route from arrival to the tent is not confirmed (the accessibility requirement was relaxed on 2026-10-13, and the route to the garden is recorded as a stated risk). The on-site route check must cover the route to the tent as well as the route to the garden. Confirm accessible toilets and step-free access to the tent with the venue before signing.
- **Heat measures for the outdoor ceremony:** Plan shaded seating (a tree canopy, shade sail or parasol) for the ceremony, shaded or cooled seating for dinner, cold water at each seat, and a shaded rest area on the step-free route for the two guests who use walking aids. Confirm with the venue that the garden has shade and an available cooling option, such as fans or misting, before signing.
- **Rain and heat decision timeline:**
  - Around 17 June 2027 (about 16 days before the event): first forecast check of the event date. This is a placeholder: the forecast horizon will not reach 3 July until approximately that date.
  - Friday 2 July 2027 at 12:00: final go or no-go decision on the outdoor setting for the dinner and dancing, based on the latest forecast. If a rain probability of 50% or more for the window 20:00 to 00:00 is forecast, or if a heat warning is issued for Barcelona, move the dinner and dancing to the covered tent at Hostal Rural Mas Blanc. Thresholds are proposed by this agent and are not from a source (unverified (estimate)).
  - Saturday 3 July 2027 at 15:00: last check; if rain starts or is forecast within 2 hours, activate the tent for the dinner.
- **Communication to guests:** Tell guests in advance that the ceremony is outdoors in the afternoon and that a covered backup tent is reserved, and ask them to bring sun protection. Notify guests of any change at least 24 hours before the event.

## Sources

- Open-Meteo Archive API (ERA5 reanalysis) via the open-meteo MCP server, `weather_archive` tool, models=era5, daily variables listed in Data Basis. Documented base URL: https://archive-api.open-meteo.com/v1/archive - supports: the five-year climatology for 30 June to 6 July (temperature, rain, wind, gusts).
- Open-Meteo Geocoding API via the open-meteo MCP server, `geocoding` tool. Documented base URL: https://geocoding-api.open-meteo.com/v1/search - supports: coordinates 41.38879 N, 2.15899 E, elevation 15 m, timezone Europe/Madrid for Barcelona.
- Open-Meteo Climate API via the open-meteo MCP server, `climate_projection` tool, default CMIP6 models. Documented base URL: https://climate-api.open-meteo.com/v1/climate - supports: context only for 27 June to 9 July 2027 (single model scenario, not a forecast).
- Sunset time of about 21:30 local time: unverified (estimate), not from a tool response or fetched page.
- Proposed decision thresholds (50% rain probability, 2 hour rain trigger) and dates: planning proposals from this agent, not sourced data.
- Hostal Rural Mas Blanc (covered backup tent): named from the revision decisions in input.md. No venue page URL was searched or fetched by this agent; the venue page is cited in the venue artifact, not here. Tent price: not sourced (unpriced; open item).
- Open item: the confirmed requirements say the covered backup must be priced within the 22200 EUR total, while this artifact keeps the tent unpriced. Coordinator to resolve before the budget step.
</content>
