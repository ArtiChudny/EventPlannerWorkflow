---
name: artifact-validator
description: Structural and citation check for event-planning artifacts. Verifies required headings (from a template) and that recommendations carry http/https source URLs, using the EventTools check-artifact command. Use after each agent group completes and inside the validator agent (gate G9 and the structural half of G8).
---

# artifact-validator

Reusable check that an artifact follows its template and cites sources. It is deterministic: the C# tool does the
work, you only run it and interpret the output.

## Command

```
dotnet tools/EventTools/bin/Release/net10.0/EventTools.dll check-artifact <artifact-path> <template-path>
```

- Exit `0` and a `PASS` line: the artifact is structurally valid.
- Exit `1` and a `FAIL` line followed by `  - <problem>` lines: fix needed. Problems can be: missing heading,
  heading out of order, unfilled placeholder/`TODO`/`{{`, a section without a URL, a recommendation row without a URL.
- Exit `64`: usage error or unreadable template (a tooling problem, not an artifact problem).

## Templates

Templates live in `.claude/skills/artifact-validator/templates/` and are the single source of truth for the required headings.

| Artifact (under `runs/<run-id>/artifacts/` unless noted) | Template |
|---|---|
| `requirements.md` | `templates/requirements.template.md` |
| `venue.md` | `templates/venue.template.md` |
| `weather.md` | `templates/weather.template.md` |
| `catering.md` | `templates/catering.template.md` |
| `program.md` | `templates/program.template.md` |
| `decor-av.md` | `templates/decor-av.template.md` |
| `logistics.md` | `templates/logistics.template.md` |
| `budget.md` | `templates/budget.template.md` |
| `validation-report.md` | `templates/validation-report.template.md` |
| `runs/<run-id>/event-plan.md` | `templates/event-plan.template.md` |

Template rules: every markdown heading is required (same level, same text, same order). A heading containing `<...>`
is matched by the text before `<` (for example `### Option 1: <venue name>`). HTML-comment directives add citation rules:

- `<!-- require-url: Heading -->` the section contains at least one http(s) URL.
- `<!-- require-url-items: Heading -->` every bullet or table data row in the section has an http(s) URL.
- `<!-- require-url-sections: Prefix -->` every heading starting with the prefix has a URL in its own body.

## How to use it

1. After a group finishes, run the command once per artifact the group produced.
2. Collect all `FAIL` outputs. A structural failure is a G9 finding; a missing URL is a G8 finding. The responsible
   agent is the one that owns the artifact (see CLAUDE.md for the owner map).
3. Never edit the artifact yourself to make it pass. Report the failure so the owning agent can be re-run with the
   problem list as targeted feedback.

## Limits (be honest in reports)

The tool proves a URL is present and well-formed, not that it is real. Real-source verification (G8) additionally
requires the validator agent to spot-check URLs with WebFetch/WebSearch. Follow the `source-citation-rules` skill for
what counts as a valid source and how unverified data must be marked.
