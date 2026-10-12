# Event Planner Workflow

An agentic event-planning workflow for Claude Code, built from a slash command, 11 subagents, 3 skills, hooks and one MCP server. You run `/plan-event <request>`; a coordinator gathers missing information, runs specialist agents in parallel where possible, validates the results against quality gates, asks for your explicit approval, and only then renders a standalone HTML guide.

Deliverables per run: `event-plan.md` (approved plan) and `event-guide.html` (standalone guide).

## Prerequisites (Windows)

- Git for Windows (Claude Code uses Git Bash to run hook commands)
- Node.js 18+ (for `npx` which starts the Open-Meteo MCP server)
- .NET 10 SDK (the helper tool and hooks are C#)
- Claude Code (CLI or VS Code extension)

No API keys or credentials are needed (see `.env.example`).

## Setup

```
git clone <this repo> && cd EventPlannerWorkflow
dotnet build tools/EventTools -c Release
```

The build must exist before you start Claude Code: hooks execute `tools/EventTools/bin/Release/net10.0/EventTools.dll`. Rebuild after any change in `tools/EventTools`.

Start `claude` in the repository root. On first start Claude Code asks you to approve the project MCP server `open-meteo` (from `.mcp.json`); approve it. Check with `claude mcp list` that `open-meteo` connects. `.mcp.json` runs it as `cmd /c npx -y open-meteo-mcp-server@2.5.2`; the `cmd /c` wrapper is needed on Windows and the version is pinned.

## Run

```
/plan-event Plan a corporate year-end party for 80 people in Berlin on December 12, 2026. Budget 12,000 EUR. Evening event, 5 hours, mix of vegetarian/vegan/gluten-free guests, wheelchair-accessible venue required, we want live music and a short award ceremony.
```

Sample requests are in `runs/_sample-inputs/` (corporate party, outdoor wedding exercising the weather agent and gate G7, kids birthday with a tight budget that is likely to trigger a gate G1 retry).

Flow: one batched question message -> confirmed requirements -> execution plan -> Group A (venue, weather) -> Group B (catering, program, decor/AV) -> logistics -> budget -> validator (gates G1-G9, targeted retries, max 3 per gate) -> event plan -> **your approval** -> HTML guide.

### Approve or reject (you, not the AI)

In your own terminal or in Claude Code with the `!` prefix:

```
scripts\approve.cmd <run-id>
scripts\reject.cmd <run-id> your feedback text
```

Then tell Claude "done". `approve` stores the SHA-256 of `event-plan.md` in `approval.json`; if the plan changes afterwards the approval is void and the HTML hook blocks. `reject` records your feedback and the coordinator revises only the affected agents.

### Resume

```
/plan-event --resume <run-id>
```

With no arguments and unfinished runs present, the coordinator asks which run to resume. Completed agents whose inputs did not change are skipped; pending, failed and stale work continues. Gate retry counters persist.

## Repository structure

```
CLAUDE.md                     workflow rules: graph, gates, gate->agent map, retries, approval, resume
.mcp.json                     Open-Meteo MCP server
.claude/settings.json         hooks + permissions
.claude/commands/plan-event.md   coordinator
.claude/agents/*.md           11 subagents (each owns one artifact)
.claude/skills/               artifact-validator (+ templates), event-html-theme-builder, source-citation-rules
tools/EventTools/             C# console tool: approval-gate-guard, no-leak-guard, post-write-state,
                              check-artifact, approve, reject, new-run
scripts/approve.cmd, reject.cmd
runs/_sample-inputs/          3 sample requests
runs/<run-id>/                per-run data (git-ignored)
```

## Hooks

| Hook | Event | Effect |
|---|---|---|
| approval-gate-guard | PreToolUse (Write, Edit, MultiEdit, Bash, PowerShell) | blocks `event-guide.html` unless approved and the plan hash matches; blocks agents from writing `approval.json` or running approve/reject |
| no-leak-guard | PreToolUse (Write, Edit, MultiEdit) | blocks `event-guide.html` / `event-plan.md` content that mentions internal file names, agent names or G1-G9 |
| post-write-state | PostToolUse (Write, Edit, MultiEdit) | records artifact hash/owner/time in `workflow-state.json`, marks downstream agents stale, keeps retry counters |

Known limit: the Bash/PowerShell check is pattern-based (it looks for write-like commands naming the guarded files); the primary protection is that agents are told to use Write and the permission deny rules.

## Troubleshooting

- **Hooks fail with "file not found"**: run `dotnet build tools/EventTools -c Release` and start Claude Code from the repository root.
- **MCP shows "Pending approval"**: start `claude` interactively once and approve `open-meteo`; or run `claude mcp reset-project-choices` and restart.
- **`open-meteo` fails to connect**: check `node --version` (18+) and that `npx -y open-meteo-mcp-server@2.5.2` starts in a terminal.
- **HTML write blocked**: the message names the cause (no plan, no approval, or plan changed after approval). Re-run `scripts\approve.cmd <run-id>`.
- **Run state looks wrong**: `workflow-state.json` is plain JSON; a malformed file is backed up as `workflow-state.json.corrupt-<time>` and rebuilt.
- **Hook JSON unreadable**: guards fail open with a warning on stderr (exit 0) rather than blocking all work; only a clear policy violation exits with code 2.
