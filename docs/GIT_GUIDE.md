# Git Guide

Quick reference for branching, commits, and PR conventions used in this repo.

---

## Trunk-Based Development

`main` is the single source of truth. All work happens in short-lived feature branches that merge back into `main` via pull request.

```
main ─────●────●────●────●────●──────
           \       /      \       /
            feat-1         feat-2
```

### Rules

- Branch from `main`, merge back to `main`.
- Keep branches short-lived (ideally < 2 days).
- Rebase on `main` before merging to keep history linear.
- Delete branches after merge.
- No long-lived `develop`, `staging`, or `release` branches.

---

## Branch Naming

```
<type>/<short-description>
```

| Type | Use |
|------|-----|
| `feat/` | New feature |
| `fix/` | Bug fix |
| `chore/` | Tooling, config, dependencies |
| `refactor/` | Code restructuring (no behavior change) |
| `docs/` | Documentation only |
| `test/` | Adding or updating tests |

Examples: `feat/booking-engine`, `fix/ruc-validation`, `chore/ci-pipeline`

---

## Conventional Commits

Every commit message follows the format:

```
<type>(<scope>): <description>

[optional body]
```

### Types

| Type | When |
|------|------|
| `feat` | New feature |
| `fix` | Bug fix |
| `chore` | Build, CI, dependencies, config |
| `refactor` | Restructure without behavior change |
| `docs` | Documentation |
| `test` | Tests |
| `style` | Formatting (no logic change) |
| `perf` | Performance improvement |

### Scopes

Use the app or package name: `web`, `api`, `mobile`, `ui`, `infra`.
Omit scope for cross-cutting changes.

### Examples

```
feat(api): add service catalog endpoints
fix(web): correct time zone offset in booking calendar
chore: upgrade turborepo to 2.9
refactor(api): extract availability calculation to domain service
docs: add git guide
test(api): add unit tests for RUC validation
```

### Guidelines

- Use imperative mood: "add", not "added" or "adds".
- Keep the first line under 72 characters.
- Use the body for *why*, not *what* (the diff shows what).
- Reference issue numbers in the body when applicable: `Closes #42`.

---

## Protected Branch: `main`

| Rule | Setting |
|------|---------|
| Direct push | Blocked |
| PR required | Yes |
| Approvals required | 1 |
| Status checks must pass | `build`, `lint`, `check-types` |
| Branch must be up to date | Yes |
| Force push | Blocked |
| Deletion | Blocked |

---

## Pull Request Process

1. Create a branch following the naming convention.
2. Make small, focused commits (conventional commit format).
3. Push and open a PR against `main`.
4. Fill in the PR template: summary (what + why) and test plan.
5. CI must pass (build, lint, type-check).
6. Get one approval.
7. Squash-merge into `main`.
8. Branch auto-deletes after merge.

### PR Title

Use the same conventional commit format for the PR title — this becomes the squash commit message.

```
feat(api): add service catalog endpoints
```

---

## Quick Reference

```bash
# Start work
git checkout main && git pull
git checkout -b feat/my-feature

# Commit
git add <files>
git commit -m "feat(web): add booking confirmation page"

# Stay current with main
git fetch origin
git rebase origin/main

# Push and open PR
git push -u origin feat/my-feature
gh pr create --title "feat(web): add booking confirmation page" --body "..."

# After merge — clean up
git checkout main && git pull
git branch -d feat/my-feature
```
