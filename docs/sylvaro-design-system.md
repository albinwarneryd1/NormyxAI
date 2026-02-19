# SYLVARO Enterprise Design System

## 1. Product Positioning
SYLVARO is a regulatory intelligence and governance infrastructure platform for regulated enterprises. The visual and interaction model prioritizes institutional trust, auditability, and decision clarity over marketing aesthetics.

Design intent:
- calm and authoritative
- information dense and review friendly
- evidence centric and regulator ready
- operationally stable for daily control workflows

## 2. Brand Foundation
Brand name: `SYLVARO`

Primary palette:
- Deep Bronze: `#5C3A21`
- Dark Brown: `#3A2414`
- Warm Gold Accent: `#B08D57`

Neutral palette:
- Warm Off-White: `#F7F5F2`
- Light Stone: `#ECE8E2`
- Deep Charcoal: `#1F1C18`

Rules:
- No blue-led identity palette
- No neon or glow
- No playful gradients
- Severity colors only where semantically justified

## 3. Typography System
Font stack:
- Brand + headings: `Cinzel` (`--font-brand`)
- UI + data: `Inter` (`--font-ui`)

Hierarchy:
- H1: 28px serif
- H2: 22px serif
- Section label: 12px uppercase small-caps style
- Metric label: 12px uppercase
- Data value: bold sans-serif
- Metadata/context: 11px muted

## 4. Layout Architecture
Application shell:
- Left fixed governance sidebar
- Top governance bar with context and session details
- Main content panels in a structured grid
- Footer with platform/version metadata

Sidebar navigation structure:
- Executive Overview
- Regulatory Exposure
- AI System Inventory
- Controls & Obligations
- Evidence Vault
- Audit Trail
- Governance Reports

Topbar includes:
- SYLVARO wordmark
- Current system context
- Role badge, tenant badge, session expiry

## 5. Information Density Standard
Density target: approximately 15% higher than default SaaS spacing.

Implementation principles:
- compact paddings and tighter vertical rhythm
- structured panel grouping
- short metadata lines above major data blocks
- dense tables with alternating row tone

## 6. Component Standards
Cards:
- subtle border + minimal elevation
- 4px radius max
- no soft-bubble visual language

Tables:
- compact headers
- uppercase header labels
- alternating muted row shading
- regulator-readable contrast

Buttons:
- primary: bronze
- secondary: charcoal/surface
- no rounded-pill buttons

Status pills:
- semantic severities only (`critical/high/medium/low/info/compliant`)

## 7. Executive Overview Specification
Sections:
1. Regulatory Posture Summary
- Compliance score orb
- AI Act classification
- GDPR exposure
- NIS2 applicability
- Control coverage with deltas

2. Risk Distribution
- muted donut chart
- textual severity breakdown with percentages

3. Regulatory Exposure Matrix
- row axis: components
- column axis: AI Act / GDPR / Security / Vendor / Operational
- hover details: issue, linked control, evidence status

4. Governance Status
- Outstanding obligations
- Critical control deficiencies
- Overdue remediations
- Evidence gaps

## 8. Error Handling UX
Formal error surface via `EnterpriseErrorPanel`:
- institutional heading and summary
- correlation ID visibility
- technical details in collapsible section
- retry action where supported

Required language style:
- factual and formal
- no raw stack trace in main message
- no developer-centric text in primary panel

## 9. Loading, Empty, and System States
Loading:
- skeleton placeholders
- fade-only motion

Empty:
- explicit state explanation
- next actionable path

Session:
- explicit secure session restore state
- non-authenticated state must use formal wording and clear CTA

## 10. Data Visualization Rules
Charts:
- muted bronze/brown/sand palette
- red reserved for elevated severity
- no bounce or playful transitions
- fade-in only

Heatmap/matrix conventions:
- clear axis labels
- semantic legend
- hover for detailed context

## 11. Dark Mode Institutional Variant
Dark mode remains institutional (not neon cyber style):
- deep charcoal surfaces
- warm text contrast
- preserved bronze accent semantics
- severity backgrounds tuned for low-glare review

## 12. Responsive Behavior
Desktop-first governance layout with controlled collapse:
- sidebar collapses into top flow under `1040px`
- topbar stacks into vertical sections on narrow widths
- panels reflow without losing data hierarchy

## 13. PDF Export Visual Standard
Exports must reflect platform authority:
- bronze/brown header band
- explicit assessment context
- policy pack reference
- sign-off and timestamp metadata
- clean legal/report-friendly typography

## 14. Accessibility Baseline
- contrast-safe palette choices
- clear focus states for inputs/actions
- semantic labels for forms and controls
- reduced cognitive noise through consistent visual hierarchy

## 15. File Mapping (Current Implementation)
Core files implementing this system:
- `src/Sylvaro.Web/wwwroot/app.css`
- `src/Sylvaro.Web/Components/Layout/MainLayout.razor`
- `src/Sylvaro.Web/Components/Layout/MainLayout.razor.css`
- `src/Sylvaro.Web/Components/Layout/NavMenu.razor`
- `src/Sylvaro.Web/Components/Layout/NavMenu.razor.css`
- `src/Sylvaro.Web/Components/Pages/Home.razor`
- `src/Sylvaro.Web/Components/Shared/EnterpriseErrorPanel.razor`
- `src/Sylvaro.Infrastructure/Exports/PdfExportService.cs`
- `src/Sylvaro.Web/wwwroot/brand/sylvaro-wordmark.svg`

## 16. Ongoing Refinement Backlog
Next high-impact refinement items:
- unify all system workspace tabs under shared data panel primitives
- add full regulator-view toggle across dashboard and audit timeline
- extend evidence completeness scoring into a dedicated evidence vault dashboard
- add compact iconography set for severity and control status
