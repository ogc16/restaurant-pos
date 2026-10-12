# Security Policy

## Supported Versions

We currently support the `master` branch of RestaurantPOS with security updates.
Older tags and branches are not actively patched.

| Version | Supported          |
|---------|--------------------|
| master  | :white_check_mark: |

## Reporting a Vulnerability

Please **do not** file a public issue for security-related problems. Instead:

- Open a private advisory via the repository's **Security → Report a vulnerability**
  flow if you are a collaborator, or
- Email the maintainers with the subject line `[Security] <short description>`.

Include the following details to help us triage quickly:

- Which component is affected (Api, Web, Application, Infrastructure, Auth)
- The version or commit where the issue occurs
- Steps to reproduce, including any payloads
- Potential impact

We will acknowledge receipt within **48 hours**, keep you informed of progress,
and coordinate the disclosure once a fix is ready.

## Security Best Practices

- The seeded default credentials (`admin@restaurant.com`) must be changed on first
  login in any real deployment.
- Do not commit `appsettings.Development.json` or any secrets; the `.gitignore`
  already excludes these.
- JWT signing keys live in configuration. Use a secure, environment-provided key
  in production and rotate it regularly.
- Keep your .NET SDK/runtime patched with current security fixes.