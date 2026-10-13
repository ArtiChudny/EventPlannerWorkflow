# Execution Plan

Run: 20261012-2337-barcelona-garden-wedding
Requirements confirmed by the user: yes

| Agent | Group | Mode | Decision reason |
|---|---|---|---|
| venue-planner | A | full | No fixed venue supplied; research 2-3 garden venues and recommend one |
| weather-outlook | A | full | Ceremony and dinner are outdoors; user asked for heat and rain likelihood and backup |
| catering-planner | B | full | Dinner for 60 with vegetarian and gluten-free needs; drinks included |
| program-planner | B | full | Timed run-of-show from 17:30 to midnight with band or DJ and photographer |
| decor-av-planner | B | full | User wants basic decor, lighting and sound for band or DJ; not skipped |
| logistics-planner | C1 | full | Step-free access, guest transport, setup and teardown, risks, weather contingency |
| budget-aggregator | C2 | full | Must include logistics costs; checks 20,000 EUR limit with 10% contingency |
| validator | after C2 | full | Gates G1-G9 across all artifacts |
| event-plan-builder | after validator PASS | initial | Merge into event-plan.md |
| (approval) | - | - | Human approval via scripts\approve.cmd |
| html-builder | after approval | initial | Render approved plan to event-guide.html |

Skipped agents: none.

Groups: A (venue-planner, weather-outlook) -> B (catering-planner, program-planner, decor-av-planner) -> C1 (logistics-planner) -> C2 (budget-aggregator) -> validator -> event-plan-builder -> approval -> html-builder.
