# Event Planner Workflow - execution rules

Agentic event-planning workflow built only from Claude Code features: one slash command (coordinator), subagents, skills, hooks and an MCP server. All helper tooling is C# (`tools/EventTools`). Read this file before doing anything in a run.

## Architecture (hub-and-spoke)

- **Coordinator** = `/plan-event` (`.claude/commands/plan-event.md`). It produces NO event content. It manages state, asks the user, plans the execution, launches subagents, runs checks, maps failures to agents, enforces retries and approval.
- **Subagents** (`.claude/agents/`) each have ONE responsibility and OWN exactly one artifact file. They never call each other; they communicate only through files in the run directory.
- **Skills**: `artifact-validator` (structural + citation check via `check-artifact`), `event-html-theme-builder` (HTML rules + `template.html`), `source-citation-rules`.
- **Hooks** (`.claude/settings.json` -> built `EventTools.dll`) enforce approval, no-leak and state updates deterministically.
- **MCP**: `open-meteo` (`.mcp.json`) is available only to `weather-outlook`.

## Agents and artifacts

| Agent | Artifact (in `runs/<run-id>/artifacts/` unless noted) | Tools |
|---|---|---|
| requirements-formalizer | `requirements.md` | Read, Write |
| venue-planner | `venue.md` | Read, Write, WebSearch, WebFetch |
| weather-outlook | `weather.md` | Read, Write, Open-Meteo MCP tools |
| catering-planner | `catering.md` | Read, Write, WebSearch, WebFetch |
| program-planner | `program.md` | Read, Write, WebSearch, WebFetch |
| decor-av-planner | `decor-av.md` | Read, Write, WebSearch, WebFetch |
| logistics-planner | `logistics.md` | Read, Write, WebSearch, WebFetch |
| budget-aggregator | `budget.md` | Read, Write |
| validator | `validation-report.md` | Read, Write, Bash (check-artifact only), WebFetch |
| event-plan-builder | `runs/<run-id>/event-plan.md` | Read, Write |
| html-builder | `runs/<run-id>/event-guide.html` | Read, Write |

## Dependency graph (what must exist before an agent runs, and what goes stale)

```
requirements-formalizer
  -> Group A (parallel): venue-planner, weather-outlook
  -> Group B (parallel): catering-planner, program-planner, decor-av-planner   (need requirements + venue + weather)
  -> Group C1: logistics-planner            (needs requirements, venue, weather, catering, program, decor-av)
  -> Group C2: budget-aggregator            (needs requirements, venue, catering, program, decor-av, logistics)
  -> validator                              (needs every artifact)
  -> event-plan-builder                     (needs every artifact + validation PASS)
  -> human approval
  -> html-builder                           (needs approved event-plan.md)
```

Note on Group C: the brief asks for logistics-planner and budget-aggregator in parallel, but the budget must include logistics costs, so budget-aggregator runs right after logistics-planner. Everything else in A and B runs in parallel.

Upstream sets (also stored in `workflow-state.json` -> `dependencies`):

- venue, weather <- requirements
- catering, program, decor-av <- requirements, venue, weather
- logistics <- requirements, venue, weather, catering, program, decor-av
- budget <- requirements, venue, catering, program, decor-av, logistics
- validator <- all of the above; event-plan-builder <- all + validator; html-builder <- event-plan-builder

**Invalidation rule:** if an upstream artifact's content changes, every transitive downstream agent becomes `stale` and must be re-run. The `post-write-state` hook marks completed downstream agents stale automatically when an upstream hash changes.

## Quality gates (checked by the validator)

| Gate | Check |
|---|---|
| G1 | Budget total <= user limit (including contingency) |
| G2 | Venue capacity >= guest count |
| G3 | Venue accessibility matches requirements |
| G4 | Dietary constraints fully covered by catering |
| G5 | Program fits within event duration, no overlapping segments |
| G6 | No duplicate vendors/venues across artifacts |
| G7 | Outdoor elements have a weather-based backup plan |
| G8 | Every recommendation links to a real source page (URL actually fetched/searched, never invented) |
| G9 | Every artifact follows its template (required headings present) |

### Gate -> responsible agent map

| Gate | Re-run |
|---|---|
| G1 | budget-aggregator and the domain agent that is over budget, then budget-aggregator again |
| G2, G3 | venue-planner, then all downstream: catering-planner, program-planner, decor-av-planner, logistics-planner, budget-aggregator |
| G4 | catering-planner, then budget-aggregator |
| G5 | program-planner, then logistics-planner, budget-aggregator |
| G6 | the later agent producing the duplicate (order: venue, catering, program, decor-av, logistics), then its downstream |
| G7 | weather-outlook / program-planner, then logistics-planner (and budget-aggregator) |
| G8, G9 | the agent that owns the offending artifact (and its downstream if the content changes) |

### Retry rules

- Re-run ONLY the affected agents plus every downstream artifact that depends on them.
- Maximum **3 retries per gate**; counters live in `workflow-state.json` -> `gate_retries` and persist across resumes. A revision after human rejection starts a new cycle (counters reset).
- If a gate still fails after 3 retries: STOP. Do not run event-plan-builder. Print a failure report (gate, findings, responsible agent, counters, suggested user action).
- The `artifact-validator` skill is run after EACH group completes, before the next group starts. Structural/citation failures count against G9 (headings) or G8 (URLs).

## Approval rules (deterministic, not chat-based)

- The human runs `scripts\approve.cmd <run-id>` or `scripts\reject.cmd <run-id> "<feedback>"` (in their terminal, or in Claude Code with the `!` prefix). `approve` writes `approval.json` = `{status, plan_sha256, timestamp}`. `reject` writes `revision-feedback.md`, removes any approval and sets state `revision_requested`.
- Agents NEVER write `approval.json` and NEVER run approve/reject (hook-enforced; permissions also deny it).
- `event-guide.html` may be created only if `approval.json` exists, `status == "approved"`, and its `plan_sha256` equals the current SHA-256 of `event-plan.md`. Any later edit of the plan invalidates the approval. HTML before the plan exists is blocked.
- After presenting the plan the coordinator waits for the user to say "done", then reads the files; it never creates approval itself.
- Rejection: coordinator reads feedback, picks the agents to revise, re-runs them + downstream + validator + event-plan-builder, and asks for approval again.

## State and resume rules

`runs/<run-id>/` layout: `input.md`, `execution-plan.md`, `workflow-state.json`, `artifacts/*.md`, `event-plan.md`, `approval.json`, `revision-feedback.md`, `event-guide.html`. Run id format `YYYYMMDD-HHMM-<slug>`, created by `EventTools new-run`.

State holds: run status and stage, `requirements_confirmed`, per-agent status (`pending|running|completed|failed|stale|skipped`) with `sha256` and `input_hashes`, per-artifact hashes, `dependencies`, `gate_retries`, `approval`, `decisions`.

Resume (`/plan-event --resume <run-id>`): skip agents that are `completed` and whose recorded `input_hashes` still equal the upstream artifacts' current hashes; re-run `pending`, `running`, `failed`, `stale` or hash-mismatched agents; keep retry counters; continue from the earliest incomplete group (or the approval step).

## Conventions

- Artifacts are human-readable markdown following the heading templates in `.claude/skills/artifact-validator/templates/` (single source of truth; the agent prompts embed the same text). Do not change a template without updating the agent that owns it.
- **The coordinator never writes content.** It writes only `input.md`, `execution-plan.md` and `workflow-state.json`.
- **Never fabricate URLs.** Cite only what was searched, fetched or returned by a tool. Unverifiable figures are marked `unverified (estimate)` (see `source-citation-rules`).
- User-facing outputs (`event-plan.md`, `event-guide.html`) never mention internal file names, agent names or gate IDs (hook-enforced).
- Least privilege: only `weather-outlook` has MCP tools; only the five research agents (venue, catering, program, decor-av, logistics) and the validator (spot-checks) have web tools.
- Hooks call the built DLL (`dotnet tools/EventTools/bin/Release/net10.0/EventTools.dll <subcommand>`). Rebuild after changing C#: `dotnet build tools/EventTools -c Release`. Exit code 2 plus stderr blocks a tool call; stdout stays clean.
- Do not commit secrets, `.env`, `bin/`, `obj/`, or `runs/<run-id>` data.
