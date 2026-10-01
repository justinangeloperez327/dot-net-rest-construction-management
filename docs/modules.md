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

## Planned modules

- Daily Progress
- Documents and Attachments
- RFIs
- Submittals
- Inspections
- Issues
- Equipment
- Suppliers
- Purchase Requests
- Purchase Orders
- Notifications
- Audit
- Reporting
