# Code review

How to review a pull request, a branch, or a working-tree diff in this repository. Written for
both human and agent reviewers; the agent-specific rules are called out where they differ.

`CLAUDE.md` carries the two rules that have to hold before you have even decided you are
reviewing. Everything below applies once you are.

## Scope

**A PR review and exploratory testing are different jobs.** A review is scoped to the feature the
PR implements, and reports only genuine bugs *the diff caused*.

Exploratory testing is encouraged, including on a PR branch — corpus sweeps, `nugetfuzz`,
`decompdiff`, round-tripping real-world assemblies, probing adjacent language shapes. That is how
real bugs get found. But its results never go into the review verbatim: a sweep measures the whole
decompiler, not the diff, so most of what it surfaces is pre-existing, unrelated, or a known
limitation. Triage first — what the diff caused may become a review comment, everything else
becomes an issue.

## What earns a comment

- **Output that is wrong** — it doesn't compile, changes semantics, crashes, or trips a
  `Debug.Assert`. A failing assert counts: it names a violated invariant, Debug is what the test
  suite and every contributor runs, and in Release the same violation just proceeds silently into
  wrong output.
- **Not** consequences of a feature the decompiler doesn't implement — that is a limitation, not a
  defect. Ask whether the finding survives if the gap stays open forever.
- **Not** pre-existing behaviour the change merely makes visible. That gets its own issue.
- **Readability, naming, duplication and performance are fair topics, but only where the magnitude
  is material.** Judge them against what the surrounding code already does, and say what you
  compared against: a few hundred closure allocations mean nothing beside the millions the
  transform pipeline already makes, while the same finding on a hot path is worth raising. A number
  with no baseline is not a finding.
- **Coverage gaps in the PR's own feature are in scope, and sweeps are how you find them.** Name the
  uncovered case and the input that reaches it — the PR is the right moment to add the fixture. A
  bare "this branch is untested" doesn't qualify, and a gap in code the PR didn't touch is an issue.

## Evidence

- **Build both outputs before calling anything a regression.** "This used to work" is a guess until
  the baseline has actually been compiled or run. Decompiler output is code, so that check is cheap
  — do it rather than reasoning about it from the diff.
- **Every comment carries a minimal repro, what both builds actually printed, and the compiler's
  diagnostic** when the claim is that something no longer compiles. Never what you reason it would
  print.
- **Reproduce what you inherited** — sub-agent results, tool summaries, your own earlier
  conclusions are leads, not evidence.
- **Follow each mechanism to its consequence.** "This loop starts at 1" is not a finding until you
  show what breaks. Describing a mechanism correctly and never checking its effect is the most
  common way an automated review wastes the author's time.
- **Be honest about severity.** No failing case means the finding is latent, and the comment says so.

## Posting

- **One finding per comment, and prefer three solid comments to twelve.** A recall-maximising sweep
  is a search stage, not an output format — the filtering is the work. Bundling a verified claim
  with an unverified one loses you both.
- **The author's expertise sets your prior.** If they maintain or designed the area, assume their
  pushback is right: withdraw or rewrite rather than compose a defence. Take the correction at the
  scope they gave it, though: rejecting specific findings is not a ruling that the topic they were
  about is off limits.
- **Withdraw for a stated reason, not for a quiet life.** Cut a finding because it fails the bar,
  because the baseline disproved it, or because the author showed it was wrong — never because they
  were sharp about a different comment. A finding nobody has challenged, that still meets the bar,
  stays. Pre-emptively clearing unopposed comments to shrink your footprint reads as tidying and is
  really just losing the review.
- **Retract by editing; delete only a thread nobody has answered.** Deleting a comment that has
  replies orphans them — GitHub drops their `in_reply_to` and promotes them to standalone comments,
  so the other person is left arguing with nothing, and their words look worse than the comment you
  were removing. Edit yours into a short correction instead. Either way update the review summary so
  it doesn't advertise findings that no longer exist, and name what was wrong once, without an
  apology paragraph.
