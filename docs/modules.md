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

Projects support hierarchical locations such as:

- Site
- Building
- Zone
- Floor
- Area
- Room
- Other

Locations support an optional parent location and soft deactivation.

## Planned modules

- Work Packages
- Activities
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
