# Validation Report

## Summary

**Overall:** PASS

Cycle 4 re-validation (2026-10-13), after the budget wording fix. Gates G1 to G9 all pass. No agent must re-run.

- **G1:** grand total 22,119.90 EUR against the 22,200 EUR limit (including the 10% contingency), 80.10 EUR under. Recomputed from the line items; see the G1 row.
- **G8 scope this cycle:** only budget.md was rewritten in this cycle (11:41). Every other artifact was last written between 11:31 and 11:35, and budget.md contains no URL. There were therefore no new URLs to fetch, and no WebFetch calls were made in this cycle. The URLs in the other artifacts are carried forward from their recorded 2026-10-13 fetches (Hotel Edelweiss and La Barcelonesa from the earlier 2026-10-12 run).
- **G6 judgment call:** budget.md line 22 carries the 2,000 EUR covered-backup amount under "Covered backup, see venue.md". It does not name Premiumtents or link its page. I treated this as the allowed "see venue.md" reference, not a duplicate. The coordinator may overrule this.

Non-blocking observations (not gate failures, no retry required):

1. Budget headroom is only 80.10 EUR. Two sensitivities would each exceed the limit if confirmed: staffing at the high case (+404.00 subtotal, 22,564.30 EUR total) and VAT on the covered backup (+420.00 subtotal, 22,581.90 EUR total). These are user decisions if they materialise (budget.md, Comparison and Overrun Notes).
2. weather.md still contains stale text. Its Backup Recommendation says the tent is "not priced in this artifact" and its last Sources item asks the coordinator to resolve the open pricing item "before the budget step". venue.md and budget.md now carry the tent at 2,000 EUR "from" price. Owner: weather-outlook, on its next run.
3. The covered backup is a "from" starting price for a 68 m2 tent (74 banquet seats per the supplier page). program.md and logistics.md both say it may not hold a dance floor and band for 60 guests. This is an open item for venue-planner (written quote) and is not a G2 or G3 issue, because G2 and G3 concern the recommended venue.
4. Tent erection timing is inconsistent. program.md shows Saturday erection (09:00 to 15:00), while the venue.md supplier terms allow a Friday install. logistics.md records this as a conflict. No run-of-show segment moves, so G5 still passes.

## Gate Results

| Gate | Result | Findings | Responsible agent |
|---|---|---|---|
| G1 Budget total <= user limit (including contingency) | PASS | Grand total **22,119.90 EUR** against the **22,200 EUR** limit (including the 10% contingency). Headroom 80.10 EUR. Recomputed: Venue 2800.00 + 2000.00 = 4800.00; Catering 2520.00 + 1500.00 + 252.00 + 315.00 = 4587.00; Entertainment and Program 3000.00 + 2000.00 = 5000.00; Decor and AV 150 + 300 + 600 + 500 + 400 + 250 + 462 = 2662.00; Logistics 959 + 800 + 120 + 150 + 300 + 200 + 0 + 531 = 3060.00. Subtotal 20109.00; contingency 10% = 2010.90; grand total 22119.90. VAT checks: 252 = 10% of 2520; 315 = 21% of 1500; 462 = 21% of 2200; 531 = 21% of 2529 (531.09, rounded). Staffing 959 recomputed from the 54.5 listed hours (rates 14.16 and 17.70 EUR, coordinator and audio at 25 EUR). **Unverified (estimate) lines:** covered backup 2000; drinks add-on 1500; VAT on food 252; VAT on drinks 315; live band 3000; photographer 2000; decor lines 150, 300, 600, 500, 400, 250 and decor VAT 462; logistics lines 800, 120, 150, 300, 200, 0 and logistics VAT 531. **Partly sourced:** venue rent 2800 and catering package 2520 are sourced "from" prices (basis and contents unverified); staffing 959 uses a sourced base rate (group and employer costs unverified). Only 5,320 EUR of the 20,109 EUR subtotal rests on a fetched price. Watch items (not in the base total): staffing high case and covered-backup VAT would each exceed the limit. | None (PASS). Budget-aggregator owns the figure. Watch items: logistics-planner (staffing), venue-planner (covered backup). |
| G2 Venue capacity >= guest count | PASS | Recommended venue Hostal Rural Mas Blanc: **220 seated and 220 standing**, verified on the venue listing (venue.md, Option 1, fetched 2026-10-13) against 60 guests. The exact ceremony layout is unverified (estimate), but the listed capacity covers 60 by a wide margin. | None (PASS). |
| G3 Venue accessibility matches requirements | PASS | Revised requirement (requirements.md, Venue Needs): wheelchair access and accessible toilets as listed on the venue page. Both are listed under "In the venue" on the Hostal Rural Mas Blanc page (venue.md, fetched 2026-10-13). The step-free route, accessible seating and toilet location were relaxed to a stated risk (requirements.md, Venue Needs and Open Assumptions). venue.md records them as NOT EVIDENCED and requires an on-site check. program.md and logistics.md carry that check, so the risk is handled. | None (PASS). |
| G4 Dietary constraints fully covered by catering | PASS | Vegetarian (some guests): vegetarian starter (heirloom tomato and burrata), vegetarian main (wild mushroom and saffron risotto) and vegetarian canapés, all in the catering.md Menu Proposal and Dietary Coverage Check. Gluten-free (2 guests): gluten-free canapés, starter, main, separate gluten-free bread roll, gluten-free dessert option, gluten-free beer option and cross-contamination controls. Condition noted, not a failure: the venue page does not mention vegetarian or gluten-free menus, so the in-house caterer must confirm in writing (catering.md, Recommended Pick, open issue 3). | None (PASS). |
| G5 Program fits within event duration, no overlapping segments | PASS | program.md starts 17:30 and ends 00:00, matching the confirmed window (requirements.md, Event Overview and Hard Limits). Nine segments run back to back with no overlap: 17:30-18:00, 18:00-19:30, 19:30-20:30, 20:30-21:40, 21:40-21:50, 21:50-22:40, 22:40-23:00, 23:00-23:50, 23:50-00:00 (390 minutes). Venue availability is 00:00 to 02:00 on Friday and Saturday (venue.md), with no curfew stated, so the 00:00 music end is within it. | None (PASS). |
| G6 No duplicate vendors/venues across artifacts | PASS | Premiumtents (bodas.net tent page) is named and linked only in venue.md. I checked budget.md, catering.md, program.md, decor-av.md, logistics.md and weather.md: none names Premiumtents, links its page or states its tent price as a recommendation. They use "covered backup, see venue.md". budget.md line 22 carries the 2,000 EUR amount under that reference, with no vendor name or link; judged allowed (see Summary). Hostal Rural Mas Blanc appears as the venue (venue.md) and as the in-house caterer (catering.md), which the venue page mandates and catering.md declares as in-house, so it is allowed. La sala dels llaurers (catering.md) is a warning reference, not a recommendation. Groove Garden (program.md), Lana Alekhina (program.md), Abanik (decor-av.md) and the three outside caterers (catering.md, comparison only) each appear in one artifact as a recommendation. | None (PASS). |
| G7 Outdoor elements have a weather-based backup plan | PASS | Outdoor elements: ceremony, dinner and dancing in the garden. weather.md gives risk level **Medium** and a covered-backup plan (a tent at the venue), with triggers at 2 July 12:00 and 3 July 15:00, 18:30 and 21:30. program.md has the "Outdoor Elements and Weather Backup" section with the same triggers per segment. logistics.md carries the matching checks in its Event Day Timeline and Risks and Contingency. | None (PASS). |
| G8 Every recommendation links to a real source page | PASS | Carried forward; no new URLs in this cycle (no WebFetch used). Only budget.md changed this cycle and it contains no URL. All other URLs are carried from their recorded 2026-10-13 fetches: venue (venuu Hostal Rural Mas Blanc, Premiumtents bodas.net, Hotel Edelweiss and La Barcelonesa from the earlier 2026-10-12 run, Attica 21, Abba Sants, Mystical Garden Haven, Masia Mas Coll, Tipi Bcn); catering (venuu sister room, Fundesplai PDF, Vil.la Minerva, Marc Blesa, Butter & Basil, Ecurcat); program (FixTheMusic, Wezoree); decor-av (Abanik, administracion.gob.es); logistics (Turnozo, Virail, Rome2rio, Vilaweb). Every recommended venue, caterer, vendor and tent has a URL on its line or in its own section. Weather figures come from the Open-Meteo archive, climate and geocoding tools with the data basis described in weather.md, which the rule accepts. | None (PASS). |
| G9 Every artifact follows its template (required headings present) | PASS | check-artifact returned PASS for all nine artifacts (requirements 8 headings, venue 8, weather 7, catering 8, program 8, decor-av 8, logistics 8, budget 6). No TODO, placeholder or empty field was reported. See Structural Check Output. | None (PASS). |

## Required Retries

None. All gates G1 to G9 pass, so no agent must re-run.

## Structural Check Output

```
=== requirements.md
PASS runs/20261012-2337-barcelona-garden-wedding/artifacts/requirements.md (8 required headings)
exit=0
=== venue.md
PASS runs/20261012-2337-barcelona-garden-wedding/artifacts/venue.md (8 required headings)
exit=0
=== weather.md
PASS runs/20261012-2337-barcelona-garden-wedding/artifacts/weather.md (7 required headings)
exit=0
=== catering.md
PASS runs/20261012-2337-barcelona-garden-wedding/artifacts/catering.md (8 required headings)
exit=0
=== program.md
PASS runs/20261012-2337-barcelona-garden-wedding/artifacts/program.md (8 required headings)
exit=0
=== decor-av.md
PASS runs/20261012-2337-barcelona-garden-wedding/artifacts/decor-av.md (8 required headings)
exit=0
=== logistics.md
PASS runs/20261012-2337-barcelona-garden-wedding/artifacts/logistics.md (8 required headings)
exit=0
=== budget.md
PASS runs/20261012-2337-barcelona-garden-wedding/artifacts/budget.md (6 required headings)
exit=0
=== validation-report.md (prior version, checked before this rewrite)
PASS runs/20261012-2337-barcelona-garden-wedding/artifacts/validation-report.md (5 required headings)
exit=0
```
