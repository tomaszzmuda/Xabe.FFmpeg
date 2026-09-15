# Security policy

## Reporting a vulnerability

Please do **not** open a public issue for security problems. Report them privately:

- Preferred: use the **Security** tab of this repository ("Report a vulnerability") if it is enabled at the time of your report.
- Alternatively, write to [support@xabe.net](mailto:support@xabe.net) with a short reproduction and the affected package version(s).

We aim to acknowledge reports promptly and to ship a fix as a new tagged NuGet release before any public disclosure.

## Scope

- `Xabe.FFmpeg` and `Xabe.FFmpeg.Downloader` as published on NuGet and in this repository.
- The downloaded `ffmpeg`/`ffprobe` binaries are third-party products with their own security processes; report vulnerabilities in those upstream ([FFmpeg security contacts](https://ffmpeg.org/security.html)).
