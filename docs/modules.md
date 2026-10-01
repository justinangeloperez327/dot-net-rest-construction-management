# Construction Management Modules

## Implemented foundation

### Companies

Companies represent organizations participating in construction projects.

Supported company types:

- Client
- Main Contractor
- Consultant
- Subcontractor
- Supplier
- Other

Companies use soft deactivation rather than destructive deletion.

### Projects

Projects have a stable unique project number, name, lifecycle status, schedule dates, and optional Client/Main Contractor/Consultant company references.

Supported lifecycle states:

- Planned
- Active
- On Hold
- Completed
- Archived

Project numbers are immutable after creation.

### Project members

Project membership is separate from global Identity roles.

A user may be a:

- Project Administrator
- Project Manager
- Engineer
- Supervisor
- Consultant
- Contractor
- Viewer

The project creator is automatically added as Project Administrator.

Global permission possession does not automatically grant access to every project. Project-scoped operations require active membership unless the user has the explicit `projects.access-all` permission.

### Project locations

Projects support hierarchical locations such as Site, Building, Zone, Floor, Area, Room, and Other.

Locations support an optional parent location and soft deactivation.

### Work packages

Work packages group construction activities within a project.

Each work package has:

- a project-unique immutable code;
- name and description;
- optional parent work package;
- Planned, Active, Completed, or Archived status.

Parent updates are checked for hierarchy cycles before persistence.

### Construction activities

Activities are project-scoped schedule/work records with:

- a project-unique immutable code;
- optional work package;
- optional project location;
- Low, Normal, High, or Critical priority;
- planned start/end dates;
- actual start/end dates;
- progress from 0 to 100 percent;
- Not Started, In Progress, On Hold, Completed, or Cancelled status.

Completed and cancelled activities cannot be edited as ordinary active work. Completing an activity sets progress to 100 percent.

### Activity assignments

An activity can have multiple assigned project members.

Assignment roles are:

- Owner
- Responsible
- Contributor
- Reviewer

Only active project members can be assigned. Each user can have at most one assignment record per activity.

### Activity dependencies

Activities support the standard scheduling dependency relationships:

- Finish-to-Start
- Start-to-Start
- Finish-to-Finish
- Start-to-Finish

Dependencies can include positive or negative lag days. Self-dependencies, duplicate dependencies, cross-project dependencies, and dependency cycles are rejected.

### Daily Progress

Daily progress reports are project/date records with a controlled workflow:

- Draft
- Submitted
- Approved
- Rejected

A project can have only one daily progress report for a given date.

Each report can record:

- weather and temperature;
- work summary and remarks;
- activity progress snapshots and completed quantities;
- manpower by trade and optional company;
- equipment utilization and idle hours;
- creator, submitter, reviewer, timestamps, and rejection reason.

Submitted reports are read-only while under review. Rejected reports can be revised and automatically return to Draft. Approved reports are immutable.

Daily progress preparation uses `daily-progress.manage`; approval/rejection requires the separate `daily-progress.approve` permission. Both remain subject to project access rules.

Daily Progress attachments now use the shared attachment subsystem described below.

### Documents and attachments

The project document register supports:

- project-unique document numbers;
- title, category, description, and Active/Archived lifecycle;
- immutable revision history;
- sequential internal version numbers;
- user-facing revision codes;
- one current revision at a time;
- uploader/timestamp metadata;
- file download without storing binary content in PostgreSQL.

Document categories include Drawing, Specification, Method Statement, Procedure, Report, Contract, Correspondence, and Other.

Generic attachments can currently target:

- Project
- Work Package
- Activity
- Daily Progress Report
- RFI
- Submittal
- Submittal Revision
- Inspection
- Issue
- Corrective Action

Attachment targets are validated against the project before upload.

File bytes are stored through the Application `IFileStorage` abstraction. Local development uses filesystem storage; cloud implementations can later replace it without changing Domain/Application code.

### RFIs

RFIs use project-unique numbers and a controlled workflow:

- Draft
- Open
- Answered
- Closed
- Cancelled

Each RFI records subject, question, optional due date, optional responsible project member, creator, current response, responder, comments, and immutable workflow history.

Opening and general maintenance use `rfis.manage`. Answering requires the separate `rfis.respond` permission.

RFI files use the shared attachment subsystem through the `Rfi` attachment target.

### Submittals

Submittals use project-unique numbers and support types including Shop Drawing, Material, Method Statement, Sample, Technical Data, Calculation, Procedure, and Other.

The workflow is:

- Draft
- Submitted
- Under Review
- Approved / Approved With Comments / Rejected
- Closed or revised and resubmitted

Each submittal contains immutable revision records with sequential version numbers, user-facing revision codes, submission/review metadata, review due dates, reviewer remarks, comments, and immutable workflow history.

Preparation/submission uses `submittals.manage`. Review decisions require `submittals.review`.

Files may be attached to the overall Submittal or directly to a Submittal Revision using the shared attachment subsystem.

### Inspections

Inspections support quality/site inspection requests tied to a project and optionally to a project location and construction activity.

Inspection types include General, Work, Material, Testing, Handover, and Other.

The workflow is:

- Draft
- Requested
- In Progress
- Passed or Failed
- Cancelled where allowed

Inspection preparation uses `inspections.manage`. Performing and completing an inspection requires the separate `inspections.perform` permission. When an inspector is explicitly assigned, only that project member may start or complete the inspection.

Each inspection records result notes, inspector identity, result timestamp, and immutable workflow history.

### Issues and corrective actions

Quality/site issues support:

- Defect
- Observation
- Non-Conformance

Severity is Low, Medium, High, or Critical.

Issues can link to a project location and/or construction activity and may be assigned to an active project member.

The lifecycle is:

- Open
- In Progress
- Pending Verification
- Closed
- Cancelled

Each issue may contain multiple corrective actions. Corrective actions have their own responsible project member, due date, Pending/In Progress/Completed/Cancelled state, completion notes, and completion metadata.

An issue cannot be submitted for verification while a corrective action remains Pending or In Progress.

Issue preparation and corrective work use `issues.manage`. Final close/reopen verification requires the separate `issues.verify` permission.

Inspection, Issue, and Corrective Action files use the shared attachment subsystem.

## Planned modules

- Equipment
- Suppliers
- Purchase Requests
- Purchase Orders
- Notifications
- Audit
- Reporting
