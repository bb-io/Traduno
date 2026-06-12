# Blackbird.io Traduno [Beta]

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Traduno Customer Portal is a customer-facing translation portal API for managing projects, quotes, files, invoices, and reference data. This Blackbird app focuses on the customer portal workflow: create quote requests or projects with uploaded source files, search existing records, and download delivered or invoicing documents.

## Before setting up

Before you connect the app, make sure that:

- You have access to a Traduno Customer Portal tenant.
- You have a Traduno API token generated in the Traduno interface.
- You know your tenant host, for example `organization.traduno.com`.

## Connecting

1. Add the Traduno app to your Blackbird environment.
2. Create a connection.
3. Enter the Traduno host without any path suffix, for example `organization.traduno.com`.
4. Enter your Traduno API token.
5. Save the connection. The app validates it against `GET /account`.

## Actions

### Projects

- **Search projects** searches projects with optional status, service, translation area, PO number, created date, deadline date, and visibility filters.
- **Create project** creates a project from uploaded source files and one or more deliverables.
- **Get project** retrieves a project by ID.

### Quotes

- **Search quotes** searches quotes with optional status, service, translation area, PO number, created date, deadline date, and visibility filters.
- **Create quote request** creates a quote request with optional uploaded source files and one or more deliverables.
- **Get quote** retrieves a quote by ID.
- **Accept quote** accepts an estimated quote and returns the updated quote.
- **Reject quote** rejects a quote with a reject message and returns the updated quote.

### Files

- **Download delivered file** downloads a delivered project file.
- **Download invoice document** downloads an invoice PDF.

## Events

- **On project status changed** polls a specific project and triggers when its status changes.
- **On quote status changed** polls a specific quote and triggers when its status changes.

### Invoices

- **Search invoices** searches invoices with optional name, payment status, issue date, and payment deadline filters.
- **Get invoice** retrieves an invoice by ID.

## Input notes

- Project and quote creation support multiple deliverables. Deliverable list fields are index-based: item `1` in each deliverable input belongs to the first deliverable, item `2` to the second, and so on.
- `Currency` is required for both project and quote creation.
- `Translation area`, `delivery files format`, `deliverable service code groups`, `deliverable target language code groups`, and `deliverable scheduling modes` are required for both project and quote creation.
- `Source files` are required for project creation and optional for quote creation.
- `Deliverable source language codes` are required for quote creation and optional for project creation.
- `Deliverable service code groups` and `Deliverable target language code groups` accept comma-separated values inside each item, for example `T,TEP` or `de-DE,fr-FR`.
- `Deliverable deadlines` are date-time inputs used only for deliverables whose scheduling mode is `deadline`.
- `Deliverable turnaround times` are integer inputs used only for deliverables whose scheduling mode is `turnaround`.
- Source files are uploaded automatically inside **Create project** and **Create quote request**.

## Rate limits

Traduno returns `429 Too Many Requests` when the rate limit is exceeded. This app retries automatically and respects the `Retry-After` response header when it is present.

## Notes

- Polling events watch a single project or quote by ID.
- Polling memory stores the last checked timestamp, the entity ID, and the last seen status.
- When a polling status filter is provided, the event triggers only when the new current status matches that filter.

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
