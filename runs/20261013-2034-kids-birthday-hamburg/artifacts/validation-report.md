# Validation Report

## Summary

**Overall:** PASS

Run 20261013-2034-kids-birthday-hamburg, revision cycle 14, re-validation on 2026-10-13 after the venue capacity wording fix. All nine gates pass. No agent must re-run.

- G8 was the only failing gate in the previous validation of this cycle: venue.md stated that the "40 Sitzplätze im ganzen Haus" passage was not on the Papijo page. venue.md now quotes both published figures ("Partyraum in Hamburg für bis zu 35 Personen" and "Bis zu 40 Sitzplätze im ganzen Haus", plus the FAQ "bis zu 35") and the false sentence is gone. The Papijo page was re-fetched this cycle and supports all of these quotes.
- Fetch scope: only the Papijo page was fetched in this cycle, because it is the only URL whose cited wording changed. The other nine page URLs were fetched on 2026-10-13 in the previous validation of this cycle, where all of them loaded and supported their claims. They are carried forward, not re-fetched. The Open-Meteo weather source is a tool-based source with the data basis described in weather.md, so it is accepted without a fetch.
- G1 recomputed: subtotal 553.34 EUR, contingency 55.33 EUR, grand total 608.67 EUR against the 1,300.00 EUR limit (including 10% contingency), headroom 691.33 EUR.
- G2: the recommended venue publishes capacity of up to 35 persons, so 25 guests fit on paper. Written confirmation of capacity for 25 is still open. It is a precondition for booking (requirements.md, cycle 11), not a gate failure.
- Non-gate observations (no retry required): (1) budget.md Totals, conflict 1, still describes the old venue.md statement that the 40-seat passage is not on the page. That statement is no longer true and the note is stale. (2) budget.md conflict 2 (napkins and cups possibly counted in both decor-av.md and catering.md) is unresolved. It does not change any gate. (3) logistics.md states that it did not use weather.md, although the dependency graph lists weather-outlook upstream of logistics-planner. G7 passes because the event is fully indoor. The coordinator decides whether that is worth a re-run.
- Open before booking, not gate failures: written confirmation of capacity for 25, the 14:00 to 17:00 slot inside the 4-hour minimum block, and the own-cake rule (venue.md, requirements.md cycle 12).

## Gate Results

| Gate | Result | Findings | Responsible agent |
|---|---|---|---|
| G1 Budget total <= user limit (including contingency) | PASS | Recomputed from budget.md. Venue 250.00 + Catering 150.00 + Entertainment and Program 0.00 + Decor and AV 91.94 + Logistics and Staffing 61.40 = subtotal 553.34 EUR. Contingency 10% = 55.334, shown as 55.33 EUR. Grand total 553.34 + 55.33 = 608.67 EUR, at or below the 1,300.00 EUR limit that includes contingency (requirements.md, Budget). Headroom 691.33 EUR. Row checks: decor-av 17.95 + 9.99 + 59.00 + 5.00 = 91.94; catering 85.00 + 33.00 + 20.00 + 12.00 = 150.00; logistics 30.00 + 15.00 + 16.40 = 61.40; venue 4 h x 62.50 EUR = 250.00 (venue.md). Most figures are marked unverified (estimate) by their owners. | None (PASS) |
| G2 Venue capacity >= guest count | PASS | Recommended venue: Papijo Eventhaus, https://www.papijo.de/partyraum-eimsbuettel-mieten/ (re-fetched 2026-10-13). The page states "Partyraum in Hamburg für bis zu 35 Personen" (header), "Bis zu 40 Sitzplätze im ganzen Haus" (feature list) and the FAQ "Insgesamt haben wir Platz für bis zu 35 Sitzplätze im Haus". The smallest published total is 35, which is at least 25 guests (15 children, 10 parents). venue.md Option 1 now quotes all three. Per-room maxima are Glow 14, Türkis 8, Werkraum 12 and Kids' Club 10 children, so the venue must confirm in writing which rooms are used for 25 (the 4-hour exclusive booking makes the whole house available). Written confirmation of 25 is still open and is a booking precondition, not a gate failure. | None (PASS) |
| G3 Venue accessibility matches requirements | PASS | requirements.md (Venue Needs): "No wheelchair access needed, as stated by the user. No other accessibility needs were stated." No accessibility need is assigned to the venue, so none is unmet. Papijo does not state step-free access or an accessible toilet. venue.md and logistics.md (R12) record this as an open question to ask in writing, not as an unmet requirement. | None (PASS) |
| G4 Dietary constraints fully covered by catering | PASS | requirements.md: 2 lactose-free children (cake needs a lactose-free option) and 1 nut-allergic child (cake home-baked and nut-free; goodie bags nut-free). catering.md Dietary Coverage Check covers each row. One vegan, dairy-free, nut-free home-baked sheet cake covers both the nut-allergy child and the lactose-free children. Nut-free drinks and finger food are covered by label checks. The goodie bag nut-free requirement is covered by logistics.md (label check, R4) and is outside catering scope. The base recipe (utopia.de, fetched 2026-10-13) lists no nuts, no nut oil, no chocolate and no egg; nut variants are not used. | None (PASS) |
| G5 Program fits within event duration, no overlapping segments | PASS | program.md run-of-show: 14:00-14:15 (15 min), 14:15-14:40 (25), 14:40-15:05 (25), 15:05-15:35 (30), 15:35-15:50 (15), 15:50-16:20 (30), 16:20-16:45 (25), 16:45-17:00 (15). The first start (14:00) and last end (17:00) match the confirmed window (requirements.md). Segments are sequential with no overlaps or gaps; durations sum to 180 minutes. Food 15:05-15:35 and cake 15:35-15:50 agree in catering.md, program.md and logistics.md. The 13:30-17:30 block is proposed and unconfirmed. It sits outside the party window and is the same in venue.md, decor-av.md, logistics.md and budget.md. Venue curfew is not stated on the page, so no breach can be shown. | None (PASS) |
| G6 No duplicate vendors/venues across artifacts | PASS | The only recommended venue is Papijo Eventhaus. It appears in venue.md, catering.md, program.md, decor-av.md and logistics.md as the same venue, not as a second line item. Catering is self-prepared with no vendor. Heycater and MYMY appear only as not-selected comparisons in catering.md. The Papijo Bluetooth music box is venue equipment, costed at 0.00 EUR and declared as included in venue hire. The eBay seller (decor-av.md) is the only other vendor. The Dasoertliche listing in logistics.md is used only for the address. | None (PASS) |
| G7 Outdoor elements have a weather-based backup plan | PASS | Fully indoor event (requirements.md; venue.md, Indoor). weather.md gives "Outdoor Risk Level: low (indoor event)" and "Backup Recommendation: Not applicable". program.md, Outdoor Elements and Weather Backup: "Not applicable: all segments are indoors". Fully indoor events pass automatically. | None (PASS) |
| G8 Every recommendation links to a real source page | PASS | This cycle: https://www.papijo.de/partyraum-eimsbuettel-mieten/ fetched with WebFetch on 2026-10-13. It loads and supports the venue.md claims: capacity "bis zu 35 Personen", "Bis zu 40 Sitzplätze im ganzen Haus" and FAQ "bis zu 35 Sitzplätze im Haus"; room capacities; "ab 62,50 €/h"; "mindestens 4 Stunden ganz für euch"; FAQ "Ihr könnt Deko, Kuchen, Snacks und Getränke selbst mitbringen"; "Schuhfreies Haus"; "Discolicht, Schwarzlicht, Bluetooth-Musikbox"; "Zentrale Lage mit guter U-Bahn-Anbindung"; "Eimsbüttel ist sehr gut an U-Bahn und Bus angebunden"; FAQ on public paid street parking and no own parking. The page gives no street address, curfew, opening hours or Saturday slot, and venue.md and logistics.md say so. Carried forward from the 2026-10-13 fetches of the previous validation of this cycle, all loaded and supported their claims: Loft 25 (eventlocations.com), Glückspilze (glueckspilze-hamburg.de), Utopia recipe, Supermarktcheck, MYMY Catering, Heycater, eBay listing 1339967625, Dasoertliche Papijo listing, aviation.direct night-bus article. Weather: Open-Meteo weather_archive and geocoding via the open-meteo MCP, with the data basis described in weather.md; accepted under the weather rule. The earlier G8 failure in venue.md is resolved. | None (PASS) |
| G9 Every artifact follows its template (required headings present) | PASS | check-artifact returned PASS for all eight existing artifacts: requirements.md (8 headings), venue.md (8), weather.md (7), catering.md (8), program.md (8), decor-av.md (8), logistics.md (8), budget.md (6). event-plan.md and event-guide.html do not exist yet and are not expected at this stage. The validation-report.md check is listed in Structural Check Output. | None (PASS) |

## Required Retries

None. All nine gates pass, so no agent must re-run.

## Structural Check Output

```
PASS runs/20261013-2034-kids-birthday-hamburg/artifacts/requirements.md (8 required headings)
exit=0
PASS runs/20261013-2034-kids-birthday-hamburg/artifacts/venue.md (8 required headings)
exit=0
PASS runs/20261013-2034-kids-birthday-hamburg/artifacts/weather.md (7 required headings)
exit=0
PASS runs/20261013-2034-kids-birthday-hamburg/artifacts/catering.md (8 required headings)
exit=0
PASS runs/20261013-2034-kids-birthday-hamburg/artifacts/program.md (8 required headings)
exit=0
PASS runs/20261013-2034-kids-birthday-hamburg/artifacts/decor-av.md (8 required headings)
exit=0
PASS runs/20261013-2034-kids-birthday-hamburg/artifacts/logistics.md (8 required headings)
exit=0
PASS runs/20261013-2034-kids-birthday-hamburg/artifacts/budget.md (6 required headings)
exit=0
```

event-plan.md: not present in the run directory. It is produced after validation, so it was not checked.

validation-report.md: this file is written in this run, so it cannot be checked before it exists. The check on the file itself is run after the write, and its result is reported in the validator's handback message. Its headings follow validation-report.template.md in the same order.
