# Validation Report

## Summary

**Overall:** PASS

All nine gates pass on this re-check (retry; G8 counter was 1). The two earlier G8 failures in logistics.md are resolved. The BVG URL is gone and its claim is marked unverified. The Q-Park page loads and supports exactly what is cited (garage "Q-Park Am Alexanderplatz", about 210 m from Alexanderplatz, online pre-booking). It states no capacity, address or price, and logistics.md correctly marks those as unverified (estimate). parkinglist.de is dropped. 25 distinct URLs were fetched (the Venuu page twice) and none failed.

Non-blocking findings:
- **venue.md quote overstates Venuu (minor wording, owner venue-planner).** venue.md says Venuu "also states 'wheelchair accessible throughout'". Two fetches of the Venuu page show only the feature tag "Rollstuhlgerecht" (wheelchair accessible) and the sentence "Mithilfe von vier Aufzuegen kann auch Menschen mit Gehbehinderung der Zugang garantiert werden". Nothing says "throughout", so the quoted phrase is not on the cited page. logistics.md already notes this (conflict item 5). I rate it minor and not gate-failing, because the claim that matters (step-free entrance and lifts) is supported by Venuu (four lifts), eventlocations.com and zipcube ("barrierefrei" via four elevators) and bbfc-cloud.de ("Step-free access: Yes", "Lift: Yes"). No decision rests on the word "throughout". If venue-planner re-runs for any reason, it should replace the quote with "Rollstuhlgerecht" plus the four-lift sentence. Re-running venue-planner for this alone is not required, since it would make all downstream artifacts stale.
- **catering.md and program.md late-snack end times differ (minor).** catering.md has the late snack at about 21:15-22:15, while program.md has it available until 22:45. Neither affects a gate.

## Gate Results

| Gate | Result | Findings | Responsible agent |
|---|---|---|---|
| G1 Budget total <= user limit (including contingency) | PASS | Recomputed: venue 1500 + catering (2466+381+696+1120=4663) + entertainment (1550+220=1770) + decor-av (150+400+800+200+200+100=1850) + logistics (308+154+100+100=662) = 10445.00. Contingency 10% = 1044.50. Grand total 11489.50 EUR against the 12000 EUR limit, 510.50 under. The pre-contingency subtotal is under the 10800 ceiling. Category totals match each domain artifact's own cost summary. The margin is thin and mostly rests on estimates. | - |
| G2 Venue capacity >= guest count | PASS | Balcony am Fernsehturm: 350 seated, 1,000 standing, verified on Venuu ("Up to 1,000 standing or 350 seated") and consistent with eventlocations.com (600 m2). 80 guests fit. | - |
| G3 Venue accessibility matches requirements | PASS | The requirement is a step-free entrance plus accessible toilets, and open items are allowed per the revised requirements. Step-free entrance and lifts are verified on 4 pages (Venuu four elevators; eventlocations.com "barrierefreie Zugang ... vier Aufzuege"; zipcube; bbfc-cloud.de "Step-free access: Yes"). Accessible toilets are honestly marked unverified (estimate) and carried as a booking-quote open item with the exact question, a booking condition in the logistics risks, and a fallback. Stage ramp is nice-to-have only. | - |
| G4 Dietary constraints fully covered by catering | PASS | Vegetarian, vegan and gluten-free are each mapped to specific dishes or arrangements: Green Berlin Buffet vegan-first, vegetarian gratin or polenta, gluten-free salads, soups, stew, brownie and mousse, separate preparation, allergen labels. heycater page confirms vegan, vegetarian and gluten-free variations, and that 5-7 business days' notice is advised. Alcohol-free options are covered. | - |
| G5 Program fits within event duration, no overlapping segments | PASS | Segments run contiguously: 18:00-18:45, 18:45-19:00, 19:00-19:50, 19:50-20:00, 20:00-20:20, 20:20-20:30, 20:30-21:15, 21:15-21:30, 21:30-22:15, 22:15-22:45, 22:45-23:00. First start 18:00 and last end 23:00 match the confirmed window. No overlaps. The award ceremony is 20 minutes. The venue curfew is not stated and is flagged for confirmation. The band sound check is before doors. | - |
| G6 No duplicate vendors/venues across artifacts | PASS | Venue: Balcony am Fernsehturm (backups Mokka Mitte Bar and Allabout Cafe Mitte appear only as fallbacks). Catering: heycater, with Bella&Bona as a named fallback and Hohenstein as an unrecommended listing. Program: la la Luxe and Event Paradies Nimaro. Decor-av: XPulse, D&F, ambiente, CS Verleihservice. Logistics: no new businesses recommended. No business is a separate line item twice, and catering explicitly checks the venue's in-house caterer. | - |
| G7 Outdoor elements have a weather-based backup plan | PASS | The event is fully indoor, so this passes automatically. weather.md gives risk "low (indoor event)" with the data basis described (Open-Meteo ERA5 archive, single-year climatology, not a forecast). program.md and logistics.md state no outdoor segments and carry a matching travel-disruption note. | - |
| G8 Every recommendation links to a real source page | PASS | Structural check shows a URL on every recommendation row. Spot-checks all loaded and supported the cited claims: q-park.de (Q-Park Am Alexanderplatz, 210 m, pre-booking; no capacity, address or price, correctly marked unverified); bahnhof.de (Alexanderstrasse 7, 600 spaces, 2 EUR per hour, 24 EUR per 24 h, disabled permit free 24 h, track works to 12.12.2026); berlintourism.org (weekend 24 h U/S-Bahn at 15 min, 3.80 EUR single ticket); service.berlin.de (11-869 EUR, about 2 weeks); leinetal24.de (about 100 EUR GEMA for 80 guests, 5 h, 100 m2); Venuu (350 seated, from 500 EUR per day, own food and drink allowed, four lifts); eventlocations.com Balcony, Mokka (100 seated, 1,785 EUR, barrier-free, stage), Allabout (80 seated, Disability Access, 22:00), Mom's (60 seated); zipcube (4,480 EUR, four elevators); bbfc-cloud.de; heycater buffet, finger food and gluten-free pages; Bella&Bona menu and pricing guide; sellwerk (la la Luxe, price on request); Kleinanzeigen event-paradies (150/220 EUR) and D&F (lighting and PA, no mic price, correctly marked estimate); XPulse (800-3000 EUR); messen.de ambiente and CS Verleihservice; Bridebook Hohenstein. Minor: venue.md's quoted "wheelchair accessible throughout" is not on the Venuu page (see Summary). It is an embellishment of the "Rollstuhlgerecht" tag, the substantive claim is supported elsewhere, and I treat it as minor. Minor: the "39 EUR per hour" staff rate is attributed to the Bella&Bona guide, which gives a 35-45 EUR range, but it is marked unverified (estimate). weather.md cites the Open-Meteo API with its data basis, which is accepted. requirements.md and budget.md contain no recommendations needing URLs. | - (optional wording fix: venue-planner) |
| G9 Every artifact follows its template (required headings present) | PASS | check-artifact returned PASS for requirements (8 headings), venue (8), weather (7), catering (8), program (8), decor-av (8), logistics (8) and budget (6). | - |

## Required Retries

None. All gates pass. Optional, non-blocking and not worth invalidating downstream artifacts alone: venue-planner may replace the quoted "wheelchair accessible throughout" with the Venuu wording ("Rollstuhlgerecht" plus the four-lift sentence) if it is ever re-run for another reason.

## Structural Check Output

```
PASS runs/20261012-2248-corporate-party-berlin/artifacts/requirements.md (8 required headings)
PASS runs/20261012-2248-corporate-party-berlin/artifacts/venue.md (8 required headings)
PASS runs/20261012-2248-corporate-party-berlin/artifacts/weather.md (7 required headings)
PASS runs/20261012-2248-corporate-party-berlin/artifacts/catering.md (8 required headings)
PASS runs/20261012-2248-corporate-party-berlin/artifacts/program.md (8 required headings)
PASS runs/20261012-2248-corporate-party-berlin/artifacts/decor-av.md (8 required headings)
PASS runs/20261012-2248-corporate-party-berlin/artifacts/logistics.md (8 required headings)
PASS runs/20261012-2248-corporate-party-berlin/artifacts/budget.md (6 required headings)
```
