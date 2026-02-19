# Normyx AI Runbook

## Health checks

- API liveness: `GET /health/live`
- API readiness: `GET /health/ready`

## Database migration

Migrations are applied automatically at API startup.

## Seed data

The API seeds demo tenant/system if no tenant exists.

## Backup targets

- PostgreSQL volume: `normyx-db-data`
- Upload/export files: `normyx-api-data`

## Operational checks

1. Verify `/health/ready` returns 200.
2. Verify login for demo tenant.
3. Run one assessment for `LoanAssist`.
4. Confirm actions are generated.
5. Generate a PDF export and download it.
6. Confirm audit events are present in `/audit`.

## Incident quick response

1. Rotate `JWT_SIGNING_KEY`.
2. Revoke active refresh tokens by clearing `RefreshTokens` table.
3. Export audit logs for affected period.
4. Regenerate assessment after remediation.
