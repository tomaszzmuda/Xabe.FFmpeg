#!/usr/bin/env python3
"""Checks links in the documentation surface this repo maintains.

Scope (pinned on purpose): README.md, CONTRIBUTING.md, SECURITY.md.
Rules:
- Markdown images, inline links and autolinks are collected; fenced code blocks are ignored.
- Repository-relative links must resolve to an existing file or directory.
- Same-file anchors must match a heading slug (GitHub rule).
- External http(s) links must respond with a status below 400.
Stdlib only: no packages, no unpinned tools.
"""

import concurrent.futures
import os
import re
import sys
import urllib.request
import urllib.parse
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
FILES = ["README.md", "CONTRIBUTING.md", "SECURITY.md"]
SELF_REPO = "tomaszzmuda/Xabe.FFmpeg"
DEFAULT_BRANCH = "master"
USER_AGENT = "xabe-ffmpeg-docs-link-check/1.0"
TIMEOUT = 30

FENCE_RE = re.compile(r"^(```|~~~)")
IMAGE_LINK_RE = re.compile(r"!\[[^\]]*\]\(([^)\s]+)(?:\s+\"[^\"]*\")?\)|\[([^\]]+)\]\(([^)\s]+)(?:\s+\"[^\"]*\")?\)|<(https?://[^>\s]+)>")
HEADING_RE = re.compile(r"^(#{1,6})\s+(.*)$", re.MULTILINE)


def slugify(text):
    text = text.strip().lower()
    text = re.sub(r"<[^>]+>", "", text)
    text = re.sub(r"[`*_]", "", text)
    text = re.sub(r"\[([^\]]*)\]\([^)]*\)", r"\1", text)
    text = re.sub(r"[^\w\- ]", "", text, flags=re.UNICODE)
    text = text.replace(" ", "-")
    return text


def active_lines(md_text):
    """Yields (lineno, line) pairs, skipping fenced code blocks."""
    in_fence = False
    fence_marker = None
    for lineno, line in enumerate(md_text.splitlines(), 1):
        stripped = line.lstrip()
        if not in_fence and FENCE_RE.match(stripped):
            in_fence = True
            fence_marker = FENCE_RE.match(stripped).group(1)
            continue
        if in_fence:
            if stripped.startswith(fence_marker):
                in_fence = False
                fence_marker = None
            continue
        yield lineno, line


def extract_targets(md_text):
    targets = []
    for lineno, line in active_lines(md_text):
        for match in IMAGE_LINK_RE.finditer(line):
            href = match.group(1) or match.group(3) or match.group(4)
            if href:
                targets.append((lineno, href))
    return targets


def iter_headings(md_text):
    return [slugify(m.group(2)) for _, line in active_lines(md_text) if (m := HEADING_RE.match(line))]


def check_local(file_path, href, slugs):
    clean = href.split("#", 1)[0]
    anchor = None
    if "#" in href:
        clean, anchor = href.split("#", 1)
    if not clean:
        if anchor is None:
            return None
        if anchor not in slugs:
            return f"anchor #{anchor} has no matching heading"
        return None
    target = (file_path.parent / clean)
    candidates = [target]
    if target.is_dir():
        candidates = [target / "index.html", target / "README.md"]
    else:
        no_ext = target.with_name(target.stem + "" + (".md" if target.suffix != ".md" else target.suffix))
        candidates.append(target.parent.joinpath(target.name[:-3] + ".md") if target.suffix != ".md" else no_ext)
    if target.exists() or any(c.exists() for c in candidates):
        return None
    return "does not exist in the repository"


def self_repo_file_path(href):
    """Returns the in-repo file path for own-repo blob/tree links on the default branch, else None.

    Such links 404 over HTTP until the branch is merged, so they are verified against the
    working tree instead.
    """
    parts = urllib.parse.urlsplit(href)
    if parts.netloc not in ("github.com", "www.github.com"):
        return None
    segments = [s for s in parts.path.split("/") if s]
    if len(segments) < 3 or "/".join(segments[:2]) != SELF_REPO:
        return None
    if segments[2] not in ("blob", "tree"):
        return None
    rest = segments[3:]
    if not rest or rest[0] not in (DEFAULT_BRANCH, "HEAD", ""):
        return None
    return "/".join(rest[1:])


def check_external(href):
    request = urllib.request.Request(href, headers={"User-Agent": USER_AGENT}, method="GET")
    last_error = None
    for _ in range(2):
        try:
            with urllib.request.urlopen(request, timeout=TIMEOUT) as response:
                if response.status < 400:
                    return None
                return f"unexpected status {response.status}"
        except Exception as exc:  # noqa: BLE001 - report any network failure
            last_error = exc
    return f"unreachable ({last_error})"


def main():
    failures = []
    external = []
    for rel in FILES:
        path = ROOT / rel
        if not path.exists():
            failures.append(f"{rel}: file not found")
            continue
        text = path.read_text(encoding="utf-8")
        slugs = iter_headings(text)
        for lineno, href in extract_targets(text):
            lowered = href.lower()
            if lowered.startswith(("mailto:", "tel:")) or href.startswith("#") or href.startswith("data:"):
                continue
            if lowered.startswith(("http://", "https://")):
                local_file = self_repo_file_path(href)
                if local_file is not None:
                    if not (ROOT / local_file).exists():
                        failures.append(f"{rel}:{lineno}: {href} - points at {local_file} which is missing from the tree")
                else:
                    external.append((f"{rel}:{lineno}", href))
            else:
                problem = check_local(path, href, slugs)
                if problem:
                    failures.append(f"{rel}:{lineno}: {href} - {problem}")
    with concurrent.futures.ThreadPoolExecutor(max_workers=8) as pool:
        futures = {pool.submit(check_external, href): (loc, href) for loc, href in external}
        for future in concurrent.futures.as_completed(futures):
            loc, href = futures[future]
            problem = future.result()
            if problem:
                failures.append(f"{loc}: {href} - {problem}")
    if failures:
        print("Broken or unreachable links:")
        for failure in failures:
            print(f"  {failure}")
        return 1
    print(f"Checked {len(FILES)} file(s); {len(external)} external link(s) reachable.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
