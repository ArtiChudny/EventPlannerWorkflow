---
name: event-html-theme-builder
description: Rules and a predefined standalone HTML/CSS template for rendering an approved event plan into a single-file, responsive, print-friendly event-guide.html with a fixed section order. Use only in the html-builder agent, after the plan has been approved.
---

# event-html-theme-builder

Renders the approved plan into `runs/<run-id>/event-guide.html` using the predefined template
`.claude/skills/event-html-theme-builder/template.html`.

## Procedure

1. Read `runs/<run-id>/event-plan.md` (the approved plan). It is the only content source; do not read other artifacts
   and do not add facts that are not in the plan.
2. Read `template.html` from this skill folder.
3. Fill every `{{PLACEHOLDER}}` and write the result with ONE Write call to `runs/<run-id>/event-guide.html`.
   The approval hook blocks the write unless the plan is approved and unchanged; if it blocks, stop and report the
   reason, do not work around it.
4. Re-read the written file and confirm that no `{{` remains and all twelve sections are present.

## Placeholders

| Placeholder | Source in the plan |
|---|---|
| `{{TITLE}}` | the plan's title (level-1 heading), used in `<title>` and the hero |
| `{{SUBTITLE}}` | one sentence from the Overview |
| `{{FACT_DATE}}`, `{{FACT_LOCATION}}`, `{{FACT_GUESTS}}`, `{{FACT_BUDGET}}` | Confirmed Requirements (budget as `12,000 EUR`) |
| `{{SECTION_OVERVIEW}}` ... `{{SECTION_SOURCES}}` | the plan section with the same name, converted to HTML |
| `{{FOOTER}}` | `Event guide prepared on <date>. Prices marked "unverified (estimate)" are estimates.` |

## Fixed section order (never reorder, never drop)

Overview, Confirmed Requirements, Venue, Weather & Contingency, Timeline / Run-of-Show, Catering,
Entertainment & Program, Decor & AV, Logistics & Staffing, Budget, Risks, Sources.
If a plan section is empty write `<p>Not applicable for this event.</p>`.

## Conversion rules

- Markdown tables become `<table>` with `<thead>`/`<tbody>`; lists become `<ul>`/`<ol>`; sub-headings become `<h3>`.
- Links become `<a href="..." target="_blank" rel="noopener">`. Keep every source URL from the plan.
- Escape `&`, `<`, `>` in text. Preserve the wording of the plan; do not summarise away figures.
- Weather risk text may be wrapped in `<span class="risk-low|risk-medium|risk-high">`. A key caveat may use `<div class="note">`.
- Keep "unverified (estimate)" labels exactly as written.

## Standalone and style constraints

- One file, inline CSS only (already in the template). No external fonts, scripts, images, CDNs or frameworks.
- Keep the template's responsive and `@media print` rules untouched.
- Do not add JavaScript.

## User-facing leakage rule

The guide is for the event owner. It must never mention internal workflow details: internal file names
(`*.md` artifact names, state or approval files), agent names, skill names, or quality-gate IDs (G1-G9). A hook blocks
the write if it finds any. Describe things in plain language ("the venue research"), never by internal name.
