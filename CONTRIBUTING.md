# Contributing

"I don't use AI to work with people", Linus Torvalds ([reference](https://www.youtube.com/watch?v=3NSSGt9bZag))

The short version: We want to interact with **you**, the human being, not your coding agent. After all, OSS is about the community.

Please do

* write an issue in your own voice, imperfections included. Be concise and to the point. ([reference](https://simonwillison.net/2026/May/24/armin-ronacher/)) A "wall of text" created by an agent isn't helpful.
* provide a PR only - especially when using AI - when you are properly capable of steering the agent. Don't provide a decompiler fix if you have no idea about the design of our decompiler and have to take the AI output at face value. Instead, spend time writing the issue, providing repros and steps you tried for us to be able to narrow down the problem quicker. ([reference, "A substantial patch used to imply substantial effort, and that effort was a reasonable proxy for good faith. That assumption no longer holds. [...]"](https://simonwillison.net/2026/Jun/5/andreas-kling) - we still accept PRs though)
* answer our comments to your PRs yourself. We don't want to play "AI by proxy" (you copy our reply to your LLM and copy the LLMs response as reply to us). In that case it would be easier to steer the agent ourselves, see the first bullet point about providing great issues. ([reference, "Zig values contributors over their contributions."](https://simonwillison.net/2026/Apr/30/zig-anti-ai/))
* note that when you open a PR, we expect that you understood and vetted every line of code that was written for you by the agent.
* add at least a brief in-your-own voice tl;dr at the beginning of the PR description - yes, it is ok to have a detailed, agent-written description following that.

## Non-Goals of ILSpy

We decided that we won't add features from the following list to ILSpy
* deobfuscation
* debugging
* assembly editing

So please don't ask us to add those.