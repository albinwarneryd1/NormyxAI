# Normyx AI

Normyx AI is a compliance operations platform for AI systems with deterministic assessment logic, evidence mapping, human sign-off, and export workflows.

## What is included

- `src/Normyx.Api` ASP.NET Core API with JWT auth, RBAC, tenant isolation, audit log, policy engine, assessments, findings/actions, and PDF exports.
- `src/Normyx.Web` Blazor web app with login/register, dashboard, tenant settings, AI systems, assessment execution, and audit views.
- `src/Normyx.Infrastructure` EF Core/PostgreSQL persistence, seeded demo data, AI draft service, policy pack evaluation.
- `policy-packs/` versioned compliance-as-code JSON packs.
- `tests/Normyx.Tests` integration test for multi-tenant isolation using Testcontainers.

## One command start

```bash
cp .env.example .env
docker compose up --build
```

After startup:

- Web UI: [http://localhost:8080](http://localhost:8080)
- API Swagger: [http://localhost:8081/swagger](http://localhost:8081/swagger)
- MinIO console: [http://localhost:9001](http://localhost:9001)

## Demo credentials (seeded)

- Tenant: `NordicFin AB`
- Email: `admin@nordicfin.example`
- Password: `ChangeMe123!`

## Happy path demo

1. Login in the web app with seeded credentials.
2. Open `AI Systems` and enter `LoanAssist`.
3. Add/update architecture components and questionnaire answers.
4. Click `Run Assessment`.
5. Review generated findings/actions.
6. Approve actions through API or UI flow.
7. Generate `DPIA_Draft` export (PDF) and download it.
8. Open `Audit Log` to verify traceability.

## Local dev (without Docker)

```bash
dotnet restore
dotnet build NormyxAI.slnx
dotnet test NormyxAI.slnx
```

Run API:

```bash
dotnet run --project src/Normyx.Api/Normyx.Api.csproj
```

Run Web:

```bash
dotnet run --project src/Normyx.Web/Normyx.Web.csproj
```

## Key endpoints

- Auth: `/auth/login`, `/auth/register`, `/auth/refresh`, `/auth/logout`
- Tenant/users: `/tenants/me`, `/tenants/users`
- AI systems/versions: `/aisystems`, `/aisystems/{id}/versions`
- Architecture: `/versions/{versionId}/architecture`
- Inventory: `/versions/{versionId}/inventory`
- Evidence: `/documents/upload`, `/documents/{id}/download`
- Assessments: `/versions/{versionId}/assessments/run`
- Findings: `/findings/assessment/{assessmentId}`
- Actions: `/actions/version/{versionId}`, `/actions/{actionId}/approve`
- Exports: `/exports/versions/{versionId}/generate`, `/exports/{artifactId}/download`
- Audit: `/audit`

## Compliance-as-code and AI schemas

- Policy pack format: `docs/policy-pack-format.md`
- AI output JSON schemas: `docs/ai-json-schemas.md`
- Runbook: `docs/runbook.md`
