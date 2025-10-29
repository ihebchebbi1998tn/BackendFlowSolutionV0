# Planning Board Module - Detailed Technical Specification

## 📋 Table of Contents
1. [Overview](#overview)
2. [Current State Analysis](#current-state-analysis)
3. [Required APIs](#required-apis)
4. [Database Schema](#database-schema)
5. [DTOs & Models](#dtos--models)
6. [Services & Controllers](#services--controllers)
7. [Frontend Integration](#frontend-integration)
8. [Implementation Roadmap](#implementation-roadmap)

---

## 🎯 Overview

**Module Purpose**: Planning Board is the central scheduling interface for dispatching service order jobs to field technicians. It transforms unassigned jobs into scheduled dispatches with proper time allocation, technician assignment, and route optimization.

**Core Workflow**:
```
Service Order → Jobs → Planning Board → Dispatch Assignment → Field Execution
```

---

## 📊 Current State Analysis

### ✅ What We Have (Backend)

#### 1. **Dispatches Module** (`backend/Modules/Dispatches/`)
- **Models**:
  - `Dispatch.cs` - Main dispatch entity with status workflow
  - `DispatchTechnician.cs` - Many-to-many technician assignment
  - `TimeEntry.cs` - Time tracking
  - `Expense.cs` - Expense tracking
  - `MaterialUsage.cs` - Materials used
  - `Attachment.cs` - Files/photos
  - `Note.cs` - Dispatch notes

- **DTOs**:
  - `DispatchDto`, `DispatchListItemDto` - Response DTOs
  - `UpdateDispatchDto` - Update payload
  - `UpdateDispatchStatusDto` - Status transitions
  - `StartDispatchDto`, `CompleteDispatchDto` - Workflow DTOs
  - `TimeEntryDto`, `ExpenseDto`, `MaterialDto` - Related entities

- **Service** (`DispatchService.cs`):
  - ✅ `CreateFromJobAsync()` - Create dispatch from job
  - ✅ `GetAllAsync()` - List with filtering
  - ✅ `GetByIdAsync()` - Get single dispatch
  - ✅ `UpdateAsync()` - Update dispatch
  - ✅ `UpdateStatusAsync()` - Status workflow
  - ✅ `StartDispatchAsync()` - Start work
  - ✅ `CompleteDispatchAsync()` - Complete work
  - ✅ `DeleteAsync()` - Soft delete
  - ✅ Time entry CRUD
  - ✅ Expense CRUD
  - ✅ Material usage CRUD
  - ✅ Attachment upload
  - ✅ Notes CRUD

- **Controller** (`DispatchesController.cs`):
  - ✅ `POST /api/dispatches/create-from-job/{jobId}`
  - ✅ `GET /api/dispatches`
  - ✅ `GET /api/dispatches/{id}`
  - ✅ `PUT /api/dispatches/{id}`
  - ✅ `DELETE /api/dispatches/{id}`
  - ✅ `PATCH /api/dispatches/{id}/status`
  - ✅ `POST /api/dispatches/{id}/start`
  - ✅ `POST /api/dispatches/{id}/complete`
  - ✅ Time entries endpoints
  - ✅ Expenses endpoints
  - ✅ Materials endpoints
  - ✅ Attachments endpoints
  - ✅ Notes endpoints
  - ✅ Statistics endpoint

#### 2. **Service Orders Module** (`backend/Modules/ServiceOrders/`)
- **Models**:
  - `ServiceOrder.cs` - Parent work order
  - `ServiceOrderJob.cs` - Individual jobs within service order
    - Has `AssignedTechnicianIds[]` field
    - Has `Status` field (unscheduled, scheduled, in_progress, completed, cancelled)
    - Has `EstimatedDuration` field
    - Has `CompletionPercentage` field

- **Controller**: `ServiceOrdersController.cs`
  - ✅ Service order CRUD operations
  - ✅ Job management within service orders

#### 3. **Database Tables**
- ✅ `dispatches` - Core dispatch table
- ✅ `dispatch_technicians` - Technician assignments
- ✅ `dispatch_time_entries` - Time tracking
- ✅ `dispatch_expenses` - Expenses
- ✅ `dispatch_materials` - Materials used
- ✅ `dispatch_attachments` - File attachments
- ✅ `dispatch_notes` - Notes
- ✅ `service_orders` - Service orders
- ✅ `service_order_jobs` - Jobs to be dispatched

### ✅ What We Have (Frontend)

#### 1. **Dispatcher Module** (`src/modules/dispatcher/`)

**Pages**:
- `DispatcherPage.tsx` - Main list view of dispatches
  - Stats cards (Total, Urgent, In Progress, Pending)
  - Search & filters (status, priority)
  - Table/List view toggle
  - Navigation to dispatch details
  - Mock data currently

- `DispatchingInterface.tsx` - Main planning board
  - Drag & drop interface
  - Unassigned jobs list
  - Calendar grid view
  - Technician assignment
  - Uses `DispatcherService` (mock data)

**Components**:
- `CustomCalendar.tsx` - Week view calendar
  - Drag & drop jobs to time slots
  - Fixed 3-day window
  - Zoom levels for grid
  - Job resizing
  - Job locking/confirmation
  - Preview on drag

- `TechnicianList.tsx` - Sidebar with technicians
  - Avatar with status indicator
  - Working hours display
  - Status badges (available, busy, offline, etc.)

- `UnassignedJobsList.tsx` - List of jobs to assign
  - Drag source for jobs
  - Priority badges
  - Search/filter jobs
  - Service order grouping

- `DispatcherHeader.tsx` - Header with actions
- `DispatcherSearchControls.tsx` - Search and filter controls
- `JobConfirmationModal.tsx` - Confirm/lock jobs

**Services**:
- `dispatcher.service.ts` - **MOCK SERVICE**
  - `getUnassignedJobs()` - Returns mock jobs
  - `getServiceOrders()` - Returns mock service orders
  - `getTechnicians()` - Returns mock technicians
  - `assignJob()` - Mock assignment
  - `lockJob()` - Mock lock
  - `resizeJob()` - Mock resize
  - `unassignJob()` - Mock unassign
  - `getAssignedJobs()` - Mock assigned jobs
  - `setTechnicianMeta()` - Mock metadata storage
  - `getTechnicianMeta()` - Mock metadata retrieval

**Types** (`dispatcher/types/index.ts`):
```typescript
interface Job {
  id: string;
  serviceOrderId: string;
  title: string;
  status: 'unassigned' | 'assigned' | 'in_progress' | 'completed' | 'cancelled';
  priority: 'low' | 'medium' | 'high' | 'urgent';
  estimatedDuration: number; // minutes
  requiredSkills: string[];
  assignedTechnicianId?: string;
  scheduledStart?: Date;
  scheduledEnd?: Date;
  isLocked?: boolean;
  location: { address: string; lat?: number; lng?: number };
  customerName: string;
  createdAt: Date;
}

interface Technician {
  id: string;
  firstName: string;
  lastName: string;
  skills: string[];
  status: 'available' | 'busy' | 'offline' | 'on_leave' | 'not_working' | 'over_capacity';
  workingHours: { start: string; end: string };
}

interface ServiceOrder {
  id: string;
  title: string;
  customerName: string;
  status: 'pending' | 'scheduled' | 'in_progress' | 'completed' | 'cancelled';
  priority: 'low' | 'medium' | 'high' | 'urgent';
  jobs: Job[];
}
```

#### 2. **Scheduling Module** (`src/modules/scheduling/`)

**Pages**:
- `SchedulerManager.tsx` - Manage technician schedules
  - List technicians
  - Edit working hours
  - Manage leaves
  - Status management

- `ScheduleEditorPage.tsx` - Edit individual technician schedule
  - Working hours per day
  - Leave periods
  - Status changes
  - Schedule notes

**Services**:
- `scheduling.service.ts` - Delegates to `DispatcherService`
  - `getTechnicians()`
  - `getUnassignedJobs()`
  - `setTechnicianMeta()`
  - `getTechnicianMeta()`

#### 3. **Field Dispatches Module** (`src/modules/field/dispatches/`)
- `DispatchModule.tsx` - Routes for dispatch views
- `DispatchJobDetail.tsx` - Detailed dispatch view (referenced but not shown)

### 🔴 What's Missing (Critical Gaps)

#### Backend Missing APIs

1. **Planning/Scheduling APIs**
   - ❌ `GET /api/planning/unassigned-jobs` - Get jobs ready for scheduling
   - ❌ `POST /api/planning/assign-job` - Assign job to technician with schedule
   - ❌ `POST /api/planning/batch-assign` - Bulk assign multiple jobs
   - ❌ `GET /api/planning/technician-schedule` - Get technician's schedule for date range
   - ❌ `POST /api/planning/optimize-route` - Optimize technician route
   - ❌ `POST /api/planning/validate-assignment` - Validate assignment constraints

2. **Technician Management APIs**
   - ❌ `GET /api/technicians` - List all technicians
   - ❌ `GET /api/technicians/{id}` - Get technician details
   - ❌ `GET /api/technicians/{id}/availability` - Check availability for date range
   - ❌ `GET /api/technicians/{id}/workload` - Get workload metrics
   - ❌ `PUT /api/technicians/{id}/status` - Update technician status
   - ❌ `GET /api/technicians/{id}/schedule` - Get technician schedule
   - ❌ `PUT /api/technicians/{id}/working-hours` - Update working hours
   - ❌ `POST /api/technicians/{id}/leaves` - Add leave period
   - ❌ `GET /api/technicians/{id}/leaves` - Get leave periods
   - ❌ `DELETE /api/technicians/{id}/leaves/{leaveId}` - Remove leave

3. **Service Order Integration**
   - ❌ `GET /api/service-orders/{id}/jobs` - Get jobs for service order
   - ❌ `POST /api/service-orders/{id}/jobs` - Create job in service order
   - ❌ `PUT /api/service-orders/jobs/{jobId}` - Update job
   - ❌ `PUT /api/service-orders/jobs/{jobId}/status` - Update job status
   - ❌ `POST /api/service-orders/jobs/{jobId}/dispatch` - Create dispatch from job

4. **Dispatch Workflow Enhancements**
   - ❌ `POST /api/dispatches/{id}/reassign` - Reassign to different technician
   - ❌ `POST /api/dispatches/{id}/reschedule` - Reschedule dispatch time
   - ❌ `GET /api/dispatches/{id}/history` - Get status change history
   - ❌ `POST /api/dispatches/bulk-update-status` - Bulk status updates

5. **Route Optimization**
   - ❌ `POST /api/routes/calculate` - Calculate optimal route
   - ❌ `GET /api/routes/technician/{id}` - Get technician's routes
   - ❌ `POST /api/routes/optimize` - Optimize existing route

#### Frontend Integration Gaps

1. **API Integration**
   - ❌ Replace `DispatcherService` mock with real API calls
   - ❌ Implement API client for planning endpoints
   - ❌ Add error handling and loading states
   - ❌ Add optimistic updates for better UX

2. **Real-time Features**
   - ❌ Real-time dispatch status updates
   - ❌ Technician status changes
   - ❌ Job assignment notifications

3. **Enhanced Features**
   - ❌ Route optimization visualization
   - ❌ Conflict detection (double booking)
   - ❌ Skill matching suggestions
   - ❌ Travel time calculation
   - ❌ Workload balancing indicators

---

## 🔧 Required APIs (Detailed)

### 1. Planning Module APIs

#### 1.1 Get Unassigned Jobs
```csharp
// GET /api/planning/unassigned-jobs
// Query params: ?status=unscheduled&priority=all&skillFilter=electrical&page=1&pageSize=20
public async Task<PagedResult<JobDto>> GetUnassignedJobs(PlanningQueryParams query)
```

**Response**:
```json
{
  "data": [
    {
      "id": "job-001",
      "serviceOrderId": "so-001",
      "serviceOrderNumber": "SO-2024-001",
      "title": "Server Maintenance",
      "description": "Replace faulty components",
      "status": "unscheduled",
      "priority": "high",
      "estimatedDuration": 180,
      "requiredSkills": ["server_maintenance", "hardware_repair"],
      "location": {
        "address": "123 Main St, City",
        "latitude": 40.7128,
        "longitude": -74.0060
      },
      "customer": {
        "id": "contact-001",
        "name": "Acme Corp",
        "phone": "+216 72 285 123"
      },
      "createdAt": "2024-01-20T16:45:00Z",
      "dueDate": "2024-01-25T17:00:00Z"
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalItems": 45,
  "totalPages": 3
}
```

#### 1.2 Assign Job to Technician
```csharp
// POST /api/planning/assign-job
public async Task<AssignmentResultDto> AssignJob(AssignJobDto dto)

public class AssignJobDto {
  public string JobId { get; set; }
  public string TechnicianId { get; set; }
  public DateTime ScheduledDate { get; set; }
  public TimeSpan ScheduledStartTime { get; set; }
  public TimeSpan ScheduledEndTime { get; set; }
  public string? Notes { get; set; }
  public bool CreateDispatch { get; set; } = true;
}
```

**Response**:
```json
{
  "success": true,
  "dispatchId": "disp-001",
  "dispatchNumber": "DISP-2024-001",
  "jobId": "job-001",
  "technicianId": "tech-001",
  "scheduledStart": "2024-01-25T09:00:00Z",
  "scheduledEnd": "2024-01-25T12:00:00Z",
  "conflicts": [],
  "warnings": [
    {
      "type": "travel_time",
      "message": "30 minutes travel time from previous job"
    }
  ]
}
```

#### 1.3 Get Technician Schedule
```csharp
// GET /api/planning/technician-schedule/{technicianId}
// Query: ?startDate=2024-01-22&endDate=2024-01-26
public async Task<TechnicianScheduleDto> GetTechnicianSchedule(string technicianId, DateTime startDate, DateTime endDate)
```

**Response**:
```json
{
  "technicianId": "tech-001",
  "technicianName": "Jean Dupont",
  "dateRange": {
    "start": "2024-01-22",
    "end": "2024-01-26"
  },
  "workingHours": {
    "monday": { "start": "08:00", "end": "17:00" },
    "tuesday": { "start": "08:00", "end": "17:00" },
    "wednesday": { "start": "08:00", "end": "17:00" },
    "thursday": { "start": "08:00", "end": "17:00" },
    "friday": { "start": "08:00", "end": "17:00" },
    "saturday": null,
    "sunday": null
  },
  "dispatches": [
    {
      "id": "disp-001",
      "jobId": "job-001",
      "title": "Server Maintenance",
      "scheduledStart": "2024-01-22T09:00:00Z",
      "scheduledEnd": "2024-01-22T12:00:00Z",
      "status": "assigned",
      "isLocked": false,
      "location": {...}
    }
  ],
  "leaves": [
    {
      "id": "leave-001",
      "startDate": "2024-01-24",
      "endDate": "2024-01-24",
      "reason": "Personal leave"
    }
  ],
  "availability": {
    "totalHours": 40,
    "scheduledHours": 12,
    "availableHours": 28,
    "utilizationPercentage": 30
  }
}
```

#### 1.4 Batch Assign Jobs
```csharp
// POST /api/planning/batch-assign
public async Task<BatchAssignmentResultDto> BatchAssignJobs(BatchAssignJobsDto dto)

public class BatchAssignJobsDto {
  public List<AssignJobDto> Assignments { get; set; }
  public bool ValidateConflicts { get; set; } = true;
}
```

#### 1.5 Validate Assignment
```csharp
// POST /api/planning/validate-assignment
public async Task<ValidationResultDto> ValidateAssignment(AssignJobDto dto)
```

**Response**:
```json
{
  "isValid": false,
  "errors": [
    {
      "type": "skill_mismatch",
      "field": "technicianId",
      "message": "Technician lacks required skill: 'server_maintenance'"
    }
  ],
  "warnings": [
    {
      "type": "high_utilization",
      "message": "Technician will be at 95% capacity"
    }
  ],
  "suggestions": [
    {
      "technicianId": "tech-002",
      "technicianName": "Ahmed Ben Ali",
      "matchScore": 0.85,
      "reason": "Has all required skills and 50% available capacity"
    }
  ]
}
```

### 2. Technician Management APIs

#### 2.1 List Technicians
```csharp
// GET /api/technicians
// Query: ?status=available&skills=server_maintenance&page=1&pageSize=50
public async Task<PagedResult<TechnicianDto>> GetTechnicians(TechnicianQueryParams query)
```

**Response**:
```json
{
  "data": [
    {
      "id": "tech-001",
      "firstName": "Jean",
      "lastName": "Dupont",
      "email": "jean.dupont@company.com",
      "phone": "+216 20 123 456",
      "position": "Senior Server Technician",
      "department": "IT Infrastructure",
      "skills": ["Server Maintenance", "Network Diagnostics", "Windows Server"],
      "certifications": ["Microsoft Certified Solutions Expert", "VMware Certified Professional"],
      "status": "available",
      "workingHours": {
        "start": "08:00",
        "end": "17:00"
      },
      "location": "Tunis Office",
      "avatar": null,
      "hourlyRate": 125.00,
      "currentWorkload": {
        "hoursThisWeek": 24,
        "capacity": 40,
        "utilizationPercentage": 60
      }
    }
  ],
  "pageNumber": 1,
  "pageSize": 50,
  "totalItems": 4,
  "totalPages": 1
}
```

#### 2.2 Get Technician Availability
```csharp
// GET /api/technicians/{id}/availability
// Query: ?startDate=2024-01-22&endDate=2024-01-26
public async Task<TechnicianAvailabilityDto> GetTechnicianAvailability(string id, DateTime startDate, DateTime endDate)
```

**Response**:
```json
{
  "technicianId": "tech-001",
  "dateRange": { "start": "2024-01-22", "end": "2024-01-26" },
  "dailyAvailability": [
    {
      "date": "2024-01-22",
      "isWorkingDay": true,
      "workingHours": { "start": "08:00", "end": "17:00" },
      "totalAvailableMinutes": 480,
      "scheduledMinutes": 180,
      "availableMinutes": 300,
      "slots": [
        { "start": "08:00", "end": "09:00", "isAvailable": true },
        { "start": "09:00", "end": "12:00", "isAvailable": false, "dispatchId": "disp-001" },
        { "start": "12:00", "end": "17:00", "isAvailable": true }
      ]
    },
    {
      "date": "2024-01-24",
      "isWorkingDay": false,
      "reason": "On leave"
    }
  ]
}
```

#### 2.3 Update Technician Status
```csharp
// PUT /api/technicians/{id}/status
public async Task<TechnicianDto> UpdateTechnicianStatus(string id, UpdateTechnicianStatusDto dto)

public class UpdateTechnicianStatusDto {
  public string Status { get; set; } // available, busy, offline, on_leave, not_working
  public string? Reason { get; set; }
  public DateTime? Until { get; set; }
}
```

#### 2.4 Manage Working Hours
```csharp
// PUT /api/technicians/{id}/working-hours
public async Task<TechnicianDto> UpdateWorkingHours(string id, UpdateWorkingHoursDto dto)

public class UpdateWorkingHoursDto {
  public Dictionary<DayOfWeek, WorkingHoursDto?> Schedule { get; set; }
}

public class WorkingHoursDto {
  public TimeSpan Start { get; set; }
  public TimeSpan End { get; set; }
}
```

#### 2.5 Manage Leaves
```csharp
// POST /api/technicians/{id}/leaves
public async Task<LeaveDto> AddLeave(string id, CreateLeaveDto dto)

// GET /api/technicians/{id}/leaves
public async Task<List<LeaveDto>> GetLeaves(string id, DateTime? startDate, DateTime? endDate)

// DELETE /api/technicians/{id}/leaves/{leaveId}
public async Task DeleteLeave(string id, string leaveId)

public class CreateLeaveDto {
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public string Type { get; set; } // vacation, sick, personal
  public string? Reason { get; set; }
}
```

### 3. Service Order Job APIs

#### 3.1 Get Jobs for Service Order
```csharp
// GET /api/service-orders/{id}/jobs
public async Task<List<JobDto>> GetServiceOrderJobs(string id)
```

#### 3.2 Update Job Status
```csharp
// PUT /api/service-orders/jobs/{jobId}/status
public async Task<JobDto> UpdateJobStatus(string jobId, UpdateJobStatusDto dto)

public class UpdateJobStatusDto {
  public string Status { get; set; } // unscheduled, scheduled, in_progress, completed, cancelled
  public string? Notes { get; set; }
}
```

#### 3.3 Create Dispatch from Job
```csharp
// POST /api/service-orders/jobs/{jobId}/dispatch
public async Task<DispatchDto> CreateDispatchFromJob(string jobId, CreateDispatchFromJobDto dto)
```

### 4. Dispatch Enhancement APIs

#### 4.1 Reassign Dispatch
```csharp
// POST /api/dispatches/{id}/reassign
public async Task<DispatchDto> ReassignDispatch(string id, ReassignDispatchDto dto)

public class ReassignDispatchDto {
  public string NewTechnicianId { get; set; }
  public string Reason { get; set; }
  public bool KeepSchedule { get; set; } = false;
  public DateTime? NewScheduledDate { get; set; }
  public TimeSpan? NewScheduledStartTime { get; set; }
  public TimeSpan? NewScheduledEndTime { get; set; }
}
```

#### 4.2 Reschedule Dispatch
```csharp
// POST /api/dispatches/{id}/reschedule
public async Task<DispatchDto> RescheduleDispatch(string id, RescheduleDispatchDto dto)

public class RescheduleDispatchDto {
  public DateTime NewScheduledDate { get; set; }
  public TimeSpan NewScheduledStartTime { get; set; }
  public TimeSpan NewScheduledEndTime { get; set; }
  public string Reason { get; set; }
}
```

#### 4.3 Get Dispatch History
```csharp
// GET /api/dispatches/{id}/history
public async Task<List<DispatchHistoryDto>> GetDispatchHistory(string id)
```

**Response**:
```json
[
  {
    "id": "hist-001",
    "dispatchId": "disp-001",
    "changeType": "status_change",
    "oldValue": "pending",
    "newValue": "assigned",
    "changedBy": {
      "id": "user-001",
      "name": "Admin User"
    },
    "changedAt": "2024-01-20T10:30:00Z",
    "notes": "Assigned to technician Jean Dupont"
  },
  {
    "id": "hist-002",
    "dispatchId": "disp-001",
    "changeType": "reassignment",
    "oldValue": "tech-002",
    "newValue": "tech-001",
    "changedBy": {...},
    "changedAt": "2024-01-21T09:15:00Z",
    "notes": "Reassigned due to skill requirements"
  }
]
```

---

## 🗄️ Database Schema

### New Tables Required

#### 1. `technicians` Table
```sql
CREATE TABLE IF NOT EXISTS technicians (
  id VARCHAR(50) PRIMARY KEY,
  first_name VARCHAR(100) NOT NULL,
  last_name VARCHAR(100) NOT NULL,
  email VARCHAR(255) UNIQUE NOT NULL,
  phone VARCHAR(50),
  position VARCHAR(100),
  department VARCHAR(100),
  skills TEXT[], -- Array of skill names
  certifications TEXT[], -- Array of certifications
  status VARCHAR(20) NOT NULL DEFAULT 'available', -- available, busy, offline, on_leave, not_working, over_capacity
  location VARCHAR(255),
  avatar VARCHAR(500),
  hourly_rate DECIMAL(10,2),
  
  -- Working hours (default schedule)
  working_hours_start TIME DEFAULT '08:00',
  working_hours_end TIME DEFAULT '17:00',
  
  -- Metadata
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  is_deleted BOOLEAN DEFAULT FALSE
);

CREATE INDEX idx_technicians_status ON technicians(status) WHERE NOT is_deleted;
CREATE INDEX idx_technicians_email ON technicians(email) WHERE NOT is_deleted;
```

#### 2. `technician_working_hours` Table
```sql
CREATE TABLE IF NOT EXISTS technician_working_hours (
  id VARCHAR(50) PRIMARY KEY,
  technician_id VARCHAR(50) NOT NULL REFERENCES technicians(id),
  day_of_week INT NOT NULL, -- 0=Sunday, 1=Monday, ..., 6=Saturday
  start_time TIME NOT NULL,
  end_time TIME NOT NULL,
  is_working_day BOOLEAN DEFAULT TRUE,
  
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  
  UNIQUE(technician_id, day_of_week)
);

CREATE INDEX idx_tech_hours_technician ON technician_working_hours(technician_id);
```

#### 3. `technician_leaves` Table
```sql
CREATE TABLE IF NOT EXISTS technician_leaves (
  id VARCHAR(50) PRIMARY KEY,
  technician_id VARCHAR(50) NOT NULL REFERENCES technicians(id),
  start_date DATE NOT NULL,
  end_date DATE NOT NULL,
  leave_type VARCHAR(20) NOT NULL, -- vacation, sick, personal, other
  reason TEXT,
  status VARCHAR(20) DEFAULT 'approved', -- pending, approved, rejected
  
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  created_by VARCHAR(50),
  
  CHECK (end_date >= start_date)
);

CREATE INDEX idx_tech_leaves_technician ON technician_leaves(technician_id);
CREATE INDEX idx_tech_leaves_dates ON technician_leaves(start_date, end_date);
```

#### 4. `technician_status_history` Table
```sql
CREATE TABLE IF NOT EXISTS technician_status_history (
  id VARCHAR(50) PRIMARY KEY,
  technician_id VARCHAR(50) NOT NULL REFERENCES technicians(id),
  old_status VARCHAR(20),
  new_status VARCHAR(20) NOT NULL,
  reason TEXT,
  changed_by VARCHAR(50),
  changed_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_tech_status_history_technician ON technician_status_history(technician_id);
CREATE INDEX idx_tech_status_history_date ON technician_status_history(changed_at);
```

#### 5. `dispatch_history` Table
```sql
CREATE TABLE IF NOT EXISTS dispatch_history (
  id VARCHAR(50) PRIMARY KEY,
  dispatch_id VARCHAR(50) NOT NULL REFERENCES dispatches(id),
  change_type VARCHAR(50) NOT NULL, -- status_change, reassignment, reschedule, note_added
  old_value TEXT,
  new_value TEXT,
  changed_by VARCHAR(50),
  changed_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  notes TEXT
);

CREATE INDEX idx_dispatch_history_dispatch ON dispatch_history(dispatch_id);
CREATE INDEX idx_dispatch_history_date ON dispatch_history(changed_at);
```

#### 6. `dispatch_routes` Table (Optional - for route optimization)
```sql
CREATE TABLE IF NOT EXISTS dispatch_routes (
  id VARCHAR(50) PRIMARY KEY,
  route_name VARCHAR(255) NOT NULL,
  technician_id VARCHAR(50) NOT NULL REFERENCES technicians(id),
  route_date DATE NOT NULL,
  dispatch_ids TEXT[], -- Array of dispatch IDs in optimized order
  total_estimated_time INT, -- minutes
  total_estimated_distance DECIMAL(10,2), -- km
  actual_time INT,
  actual_distance DECIMAL(10,2),
  status VARCHAR(20) DEFAULT 'planned', -- planned, in_progress, completed
  
  start_location JSONB,
  end_location JSONB,
  
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_routes_technician_date ON dispatch_routes(technician_id, route_date);
```

### Modified Tables

#### Modify `service_order_jobs` Table
```sql
-- Add fields if not present
ALTER TABLE service_order_jobs 
  ADD COLUMN IF NOT EXISTS location JSONB,
  ADD COLUMN IF NOT EXISTS customer_name VARCHAR(255),
  ADD COLUMN IF NOT EXISTS customer_phone VARCHAR(50),
  ADD COLUMN IF NOT EXISTS required_skills TEXT[],
  ADD COLUMN IF NOT EXISTS due_date TIMESTAMP,
  ADD COLUMN IF NOT EXISTS dispatch_id VARCHAR(50) REFERENCES dispatches(id);

CREATE INDEX IF NOT EXISTS idx_jobs_status ON service_order_jobs(status);
CREATE INDEX IF NOT EXISTS idx_jobs_dispatch ON service_order_jobs(dispatch_id);
```

---

## 📦 DTOs & Models

### 1. Planning DTOs

```csharp
// Query Parameters
public class PlanningQueryParams
{
    public string? Status { get; set; } = "unscheduled";
    public string? Priority { get; set; }
    public string? SkillFilter { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// Job DTO
public class JobDto
{
    public string Id { get; set; }
    public string ServiceOrderId { get; set; }
    public string ServiceOrderNumber { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public int EstimatedDuration { get; set; }
    public List<string> RequiredSkills { get; set; } = new();
    public LocationDto? Location { get; set; }
    public CustomerLightDto? Customer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public string? DispatchId { get; set; }
}

// Assignment DTOs
public class AssignJobDto
{
    public string JobId { get; set; }
    public string TechnicianId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeSpan ScheduledStartTime { get; set; }
    public TimeSpan ScheduledEndTime { get; set; }
    public string? Notes { get; set; }
    public bool CreateDispatch { get; set; } = true;
}

public class AssignmentResultDto
{
    public bool Success { get; set; }
    public string? DispatchId { get; set; }
    public string? DispatchNumber { get; set; }
    public string JobId { get; set; }
    public string TechnicianId { get; set; }
    public DateTime ScheduledStart { get; set; }
    public DateTime ScheduledEnd { get; set; }
    public List<AssignmentConflictDto> Conflicts { get; set; } = new();
    public List<AssignmentWarningDto> Warnings { get; set; } = new();
}

public class AssignmentConflictDto
{
    public string Type { get; set; } // time_overlap, skill_mismatch, unavailable
    public string Message { get; set; }
    public string? ConflictingDispatchId { get; set; }
}

public class AssignmentWarningDto
{
    public string Type { get; set; } // travel_time, high_utilization, overtime
    public string Message { get; set; }
}

// Validation DTOs
public class ValidationResultDto
{
    public bool IsValid { get; set; }
    public List<ValidationErrorDto> Errors { get; set; } = new();
    public List<AssignmentWarningDto> Warnings { get; set; } = new();
    public List<TechnicianSuggestionDto> Suggestions { get; set; } = new();
}

public class ValidationErrorDto
{
    public string Type { get; set; }
    public string Field { get; set; }
    public string Message { get; set; }
}

public class TechnicianSuggestionDto
{
    public string TechnicianId { get; set; }
    public string TechnicianName { get; set; }
    public double MatchScore { get; set; }
    public string Reason { get; set; }
}

// Schedule DTOs
public class TechnicianScheduleDto
{
    public string TechnicianId { get; set; }
    public string TechnicianName { get; set; }
    public DateRangeDto DateRange { get; set; }
    public Dictionary<string, WorkingHoursDto?> WorkingHours { get; set; } = new();
    public List<DispatchListItemDto> Dispatches { get; set; } = new();
    public List<LeaveDto> Leaves { get; set; } = new();
    public AvailabilityMetricsDto Availability { get; set; }
}

public class DateRangeDto
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

public class AvailabilityMetricsDto
{
    public int TotalHours { get; set; }
    public int ScheduledHours { get; set; }
    public int AvailableHours { get; set; }
    public int UtilizationPercentage { get; set; }
}
```

### 2. Technician DTOs

```csharp
// Technician DTO
public class TechnicianDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public string? Department { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<string> Certifications { get; set; } = new();
    public string Status { get; set; }
    public WorkingHoursDto WorkingHours { get; set; }
    public string? Location { get; set; }
    public string? Avatar { get; set; }
    public decimal? HourlyRate { get; set; }
    public TechnicianWorkloadDto? CurrentWorkload { get; set; }
}

public class WorkingHoursDto
{
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
}

public class TechnicianWorkloadDto
{
    public int HoursThisWeek { get; set; }
    public int Capacity { get; set; }
    public int UtilizationPercentage { get; set; }
}

// Query
public class TechnicianQueryParams
{
    public string? Status { get; set; }
    public string? Skills { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

// Availability
public class TechnicianAvailabilityDto
{
    public string TechnicianId { get; set; }
    public DateRangeDto DateRange { get; set; }
    public List<DailyAvailabilityDto> DailyAvailability { get; set; } = new();
}

public class DailyAvailabilityDto
{
    public DateTime Date { get; set; }
    public bool IsWorkingDay { get; set; }
    public WorkingHoursDto? WorkingHours { get; set; }
    public int TotalAvailableMinutes { get; set; }
    public int ScheduledMinutes { get; set; }
    public int AvailableMinutes { get; set; }
    public List<TimeSlotDto> Slots { get; set; } = new();
    public string? Reason { get; set; }
}

public class TimeSlotDto
{
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public bool IsAvailable { get; set; }
    public string? DispatchId { get; set; }
}

// Status Update
public class UpdateTechnicianStatusDto
{
    public string Status { get; set; }
    public string? Reason { get; set; }
    public DateTime? Until { get; set; }
}

// Working Hours
public class UpdateWorkingHoursDto
{
    public Dictionary<DayOfWeek, WorkingHoursDto?> Schedule { get; set; }
}

// Leaves
public class LeaveDto
{
    public string Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Type { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateLeaveDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Type { get; set; }
    public string? Reason { get; set; }
}
```

### 3. Dispatch Enhancement DTOs

```csharp
// Reassignment
public class ReassignDispatchDto
{
    public string NewTechnicianId { get; set; }
    public string Reason { get; set; }
    public bool KeepSchedule { get; set; } = false;
    public DateTime? NewScheduledDate { get; set; }
    public TimeSpan? NewScheduledStartTime { get; set; }
    public TimeSpan? NewScheduledEndTime { get; set; }
}

// Rescheduling
public class RescheduleDispatchDto
{
    public DateTime NewScheduledDate { get; set; }
    public TimeSpan NewScheduledStartTime { get; set; }
    public TimeSpan NewScheduledEndTime { get; set; }
    public string Reason { get; set; }
}

// History
public class DispatchHistoryDto
{
    public string Id { get; set; }
    public string DispatchId { get; set; }
    public string ChangeType { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public UserLightDto ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? Notes { get; set; }
}
```

### 4. Common DTOs

```csharp
public class LocationDto
{
    public string Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class CustomerLightDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Phone { get; set; }
}
```

---

## 🛠️ Services & Controllers

### 1. Planning Service

```csharp
// backend/Modules/Planning/Services/IPlanningService.cs
public interface IPlanningService
{
    // Jobs
    Task<PagedResult<JobDto>> GetUnassignedJobsAsync(PlanningQueryParams query);
    Task<AssignmentResultDto> AssignJobAsync(AssignJobDto dto, string userId);
    Task<List<AssignmentResultDto>> BatchAssignJobsAsync(List<AssignJobDto> assignments, string userId);
    Task<ValidationResultDto> ValidateAssignmentAsync(AssignJobDto dto);
    
    // Schedule
    Task<TechnicianScheduleDto> GetTechnicianScheduleAsync(string technicianId, DateTime startDate, DateTime endDate);
}
```

```csharp
// backend/Modules/Planning/Services/PlanningService.cs
public class PlanningService : IPlanningService
{
    private readonly ApplicationDbContext _db;
    private readonly IDispatchService _dispatchService;
    private readonly ILogger<PlanningService> _logger;

    public async Task<PagedResult<JobDto>> GetUnassignedJobsAsync(PlanningQueryParams query)
    {
        var q = _db.ServiceOrderJobs
            .Include(j => j.ServiceOrder)
            .Where(j => j.Status == "unscheduled" && j.DispatchId == null)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(query.Status))
            q = q.Where(j => j.Status == query.Status);

        if (!string.IsNullOrEmpty(query.Priority))
            q = q.Where(j => j.ServiceOrder.Priority == query.Priority);

        if (!string.IsNullOrEmpty(query.SkillFilter))
        {
            var skills = query.SkillFilter.Split(',');
            q = q.Where(j => j.RequiredSkills != null && j.RequiredSkills.Any(s => skills.Contains(s)));
        }

        // Pagination
        var total = await q.CountAsync();
        var items = await q
            .OrderBy(j => j.DueDate)
            .ThenByDescending(j => j.ServiceOrder.Priority)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(j => JobMapping.ToDto(j))
            .ToListAsync();

        return new PagedResult<JobDto>
        {
            Data = items,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)query.PageSize)
        };
    }

    public async Task<AssignmentResultDto> AssignJobAsync(AssignJobDto dto, string userId)
    {
        // 1. Validate assignment
        var validation = await ValidateAssignmentAsync(dto);
        if (!validation.IsValid)
        {
            return new AssignmentResultDto
            {
                Success = false,
                Conflicts = validation.Errors.Select(e => new AssignmentConflictDto
                {
                    Type = e.Type,
                    Message = e.Message
                }).ToList()
            };
        }

        // 2. Create dispatch
        if (dto.CreateDispatch)
        {
            var createDispatchDto = new CreateDispatchFromJobDto
            {
                AssignedTechnicianIds = new List<string> { dto.TechnicianId },
                ScheduledDate = dto.ScheduledDate,
                ScheduledStartTime = dto.ScheduledStartTime,
                ScheduledEndTime = dto.ScheduledEndTime,
                Priority = null // Inherit from job
            };

            var dispatch = await _dispatchService.CreateFromJobAsync(dto.JobId, createDispatchDto, userId);

            // 3. Update job status
            var job = await _db.ServiceOrderJobs.FindAsync(dto.JobId);
            if (job != null)
            {
                job.Status = "scheduled";
                job.DispatchId = dispatch.Id;
                job.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            return new AssignmentResultDto
            {
                Success = true,
                DispatchId = dispatch.Id,
                DispatchNumber = dispatch.DispatchNumber,
                JobId = dto.JobId,
                TechnicianId = dto.TechnicianId,
                ScheduledStart = dto.ScheduledDate.Add(dto.ScheduledStartTime),
                ScheduledEnd = dto.ScheduledDate.Add(dto.ScheduledEndTime),
                Warnings = validation.Warnings
            };
        }

        return new AssignmentResultDto { Success = false };
    }

    public async Task<ValidationResultDto> ValidateAssignmentAsync(AssignJobDto dto)
    {
        var result = new ValidationResultDto { IsValid = true };

        // 1. Check technician exists
        var technician = await _db.Technicians.FindAsync(dto.TechnicianId);
        if (technician == null)
        {
            result.IsValid = false;
            result.Errors.Add(new ValidationErrorDto
            {
                Type = "technician_not_found",
                Field = "technicianId",
                Message = "Technician not found"
            });
            return result;
        }

        // 2. Check skill match
        var job = await _db.ServiceOrderJobs.FindAsync(dto.JobId);
        if (job?.RequiredSkills != null && job.RequiredSkills.Length > 0)
        {
            var missingSkills = job.RequiredSkills.Except(technician.Skills ?? new string[0]).ToList();
            if (missingSkills.Any())
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationErrorDto
                {
                    Type = "skill_mismatch",
                    Field = "technicianId",
                    Message = $"Technician lacks required skills: {string.Join(", ", missingSkills)}"
                });
            }
        }

        // 3. Check time conflicts
        var scheduledStart = dto.ScheduledDate.Add(dto.ScheduledStartTime);
        var scheduledEnd = dto.ScheduledDate.Add(dto.ScheduledEndTime);

        var conflicts = await _db.Dispatches
            .Where(d => d.AssignedTechnicians.Any(t => t.TechnicianId == dto.TechnicianId) &&
                        d.ScheduledDate == dto.ScheduledDate &&
                        d.Status != "cancelled" &&
                        !d.IsDeleted)
            .ToListAsync();

        foreach (var conflict in conflicts)
        {
            if (conflict.ScheduledStartTime.HasValue && conflict.ScheduledEndTime.HasValue)
            {
                var conflictStart = dto.ScheduledDate.Add(conflict.ScheduledStartTime.Value);
                var conflictEnd = dto.ScheduledDate.Add(conflict.ScheduledEndTime.Value);

                if (scheduledStart < conflictEnd && scheduledEnd > conflictStart)
                {
                    result.IsValid = false;
                    result.Errors.Add(new ValidationErrorDto
                    {
                        Type = "time_overlap",
                        Field = "scheduledTime",
                        Message = $"Time overlaps with dispatch {conflict.DispatchNumber}"
                    });
                }
            }
        }

        // 4. Check availability (leaves, working hours)
        var leaves = await _db.TechnicianLeaves
            .Where(l => l.TechnicianId == dto.TechnicianId &&
                        l.StartDate <= dto.ScheduledDate &&
                        l.EndDate >= dto.ScheduledDate &&
                        l.Status == "approved")
            .AnyAsync();

        if (leaves)
        {
            result.IsValid = false;
            result.Errors.Add(new ValidationErrorDto
            {
                Type = "unavailable",
                Field = "scheduledDate",
                Message = "Technician is on leave on this date"
            });
        }

        // 5. Calculate utilization warning
        var totalScheduled = conflicts.Sum(c => c.EstimatedDuration ?? 0);
        var newDuration = (int)(scheduledEnd - scheduledStart).TotalMinutes;
        var totalWithNew = totalScheduled + newDuration;
        var dailyCapacity = 480; // 8 hours

        if (totalWithNew > dailyCapacity * 0.9)
        {
            result.Warnings.Add(new AssignmentWarningDto
            {
                Type = "high_utilization",
                Message = $"Technician will be at {(totalWithNew * 100 / dailyCapacity)}% capacity"
            });
        }

        return result;
    }

    public async Task<TechnicianScheduleDto> GetTechnicianScheduleAsync(string technicianId, DateTime startDate, DateTime endDate)
    {
        var technician = await _db.Technicians.FindAsync(technicianId);
        if (technician == null)
            throw new KeyNotFoundException("Technician not found");

        // Get dispatches
        var dispatches = await _db.Dispatches
            .Include(d => d.AssignedTechnicians)
            .Where(d => d.AssignedTechnicians.Any(t => t.TechnicianId == technicianId) &&
                        d.ScheduledDate >= startDate &&
                        d.ScheduledDate <= endDate &&
                        !d.IsDeleted)
            .ToListAsync();

        // Get leaves
        var leaves = await _db.TechnicianLeaves
            .Where(l => l.TechnicianId == technicianId &&
                        l.StartDate <= endDate &&
                        l.EndDate >= startDate)
            .ToListAsync();

        // Calculate availability
        var totalHours = (int)(endDate - startDate).TotalDays * 8;
        var scheduledHours = dispatches.Sum(d => (d.EstimatedDuration ?? 0) / 60);
        var availableHours = totalHours - scheduledHours;

        return new TechnicianScheduleDto
        {
            TechnicianId = technicianId,
            TechnicianName = $"{technician.FirstName} {technician.LastName}",
            DateRange = new DateRangeDto { Start = startDate, End = endDate },
            Dispatches = dispatches.Select(DispatchMapping.ToListItemDto).ToList(),
            Leaves = leaves.Select(LeaveMapping.ToDto).ToList(),
            Availability = new AvailabilityMetricsDto
            {
                TotalHours = totalHours,
                ScheduledHours = scheduledHours,
                AvailableHours = availableHours,
                UtilizationPercentage = totalHours > 0 ? (scheduledHours * 100 / totalHours) : 0
            }
        };
    }
}
```

```csharp
// backend/Modules/Planning/Controllers/PlanningController.cs
[ApiController]
[Route("api/planning")]
[Authorize]
public class PlanningController : ControllerBase
{
    private readonly IPlanningService _planningService;
    private readonly ILogger<PlanningController> _logger;

    [HttpGet("unassigned-jobs")]
    public async Task<IActionResult> GetUnassignedJobs([FromQuery] PlanningQueryParams query)
    {
        var result = await _planningService.GetUnassignedJobsAsync(query);
        return Ok(result);
    }

    [HttpPost("assign-job")]
    public async Task<IActionResult> AssignJob([FromBody] AssignJobDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        var result = await _planningService.AssignJobAsync(dto, userId);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpPost("batch-assign")]
    public async Task<IActionResult> BatchAssignJobs([FromBody] List<AssignJobDto> assignments)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        var results = await _planningService.BatchAssignJobsAsync(assignments, userId);
        return Ok(results);
    }

    [HttpPost("validate-assignment")]
    public async Task<IActionResult> ValidateAssignment([FromBody] AssignJobDto dto)
    {
        var result = await _planningService.ValidateAssignmentAsync(dto);
        return Ok(result);
    }

    [HttpGet("technician-schedule/{technicianId}")]
    public async Task<IActionResult> GetTechnicianSchedule(
        string technicianId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var result = await _planningService.GetTechnicianScheduleAsync(technicianId, startDate, endDate);
        return Ok(result);
    }
}
```

### 2. Technician Service

```csharp
// backend/Modules/Technicians/Services/ITechnicianService.cs
public interface ITechnicianService
{
    Task<PagedResult<TechnicianDto>> GetTechniciansAsync(TechnicianQueryParams query);
    Task<TechnicianDto> GetByIdAsync(string id);
    Task<TechnicianAvailabilityDto> GetAvailabilityAsync(string id, DateTime startDate, DateTime endDate);
    Task<TechnicianDto> UpdateStatusAsync(string id, UpdateTechnicianStatusDto dto, string userId);
    Task<TechnicianDto> UpdateWorkingHoursAsync(string id, UpdateWorkingHoursDto dto);
    Task<LeaveDto> AddLeaveAsync(string id, CreateLeaveDto dto, string userId);
    Task<List<LeaveDto>> GetLeavesAsync(string id, DateTime? startDate, DateTime? endDate);
    Task DeleteLeaveAsync(string id, string leaveId);
}
```

```csharp
// backend/Modules/Technicians/Controllers/TechniciansController.cs
[ApiController]
[Route("api/technicians")]
[Authorize]
public class TechniciansController : ControllerBase
{
    private readonly ITechnicianService _technicianService;

    [HttpGet]
    public async Task<IActionResult> GetTechnicians([FromQuery] TechnicianQueryParams query)
    {
        var result = await _technicianService.GetTechniciansAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _technicianService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("{id}/availability")]
    public async Task<IActionResult> GetAvailability(
        string id,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var result = await _technicianService.GetAvailabilityAsync(id, startDate, endDate);
        return Ok(result);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateTechnicianStatusDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        var result = await _technicianService.UpdateStatusAsync(id, dto, userId);
        return Ok(result);
    }

    [HttpPut("{id}/working-hours")]
    public async Task<IActionResult> UpdateWorkingHours(string id, [FromBody] UpdateWorkingHoursDto dto)
    {
        var result = await _technicianService.UpdateWorkingHoursAsync(id, dto);
        return Ok(result);
    }

    [HttpPost("{id}/leaves")]
    public async Task<IActionResult> AddLeave(string id, [FromBody] CreateLeaveDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        var result = await _technicianService.AddLeaveAsync(id, dto, userId);
        return Created($"/api/technicians/{id}/leaves/{result.Id}", result);
    }

    [HttpGet("{id}/leaves")]
    public async Task<IActionResult> GetLeaves(
        string id,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var result = await _technicianService.GetLeavesAsync(id, startDate, endDate);
        return Ok(result);
    }

    [HttpDelete("{id}/leaves/{leaveId}")]
    public async Task<IActionResult> DeleteLeave(string id, string leaveId)
    {
        await _technicianService.DeleteLeaveAsync(id, leaveId);
        return NoContent();
    }
}
```

---

## 🎨 Frontend Integration

### Update `dispatcher.service.ts` to Call Real APIs

```typescript
// src/modules/dispatcher/services/dispatcher.service.ts
import axios from 'axios';
import type { Job, Technician, ServiceOrder } from '../types';

const API_BASE = '/api';

export class DispatcherService {
  // Replace mock with real API
  static async getUnassignedJobs(filters?: {
    status?: string;
    priority?: string;
    skillFilter?: string;
  }): Promise<Job[]> {
    const params = new URLSearchParams();
    if (filters?.status) params.append('status', filters.status);
    if (filters?.priority) params.append('priority', filters.priority);
    if (filters?.skillFilter) params.append('skillFilter', filters.skillFilter);
    
    const response = await axios.get(`${API_BASE}/planning/unassigned-jobs?${params}`);
    return response.data.data.map(mapJobDto);
  }

  static async getTechnicians(filters?: {
    status?: string;
    skills?: string;
  }): Promise<Technician[]> {
    const params = new URLSearchParams();
    if (filters?.status) params.append('status', filters.status);
    if (filters?.skills) params.append('skills', filters.skills);
    
    const response = await axios.get(`${API_BASE}/technicians?${params}`);
    return response.data.data.map(mapTechnicianDto);
  }

  static async assignJob(
    jobId: string,
    technicianId: string,
    scheduledStart: Date,
    scheduledEnd: Date
  ): Promise<void> {
    const scheduledDate = new Date(scheduledStart);
    scheduledDate.setHours(0, 0, 0, 0);
    
    const payload = {
      jobId,
      technicianId,
      scheduledDate: scheduledDate.toISOString(),
      scheduledStartTime: formatTime(scheduledStart),
      scheduledEndTime: formatTime(scheduledEnd),
      createDispatch: true
    };

    const response = await axios.post(`${API_BASE}/planning/assign-job`, payload);
    
    if (!response.data.success) {
      throw new Error(response.data.conflicts?.[0]?.message || 'Assignment failed');
    }
  }

  static async getAssignedJobs(technicianId: string, date: Date): Promise<Job[]> {
    const startDate = new Date(date);
    startDate.setHours(0, 0, 0, 0);
    const endDate = new Date(date);
    endDate.setHours(23, 59, 59, 999);

    const response = await axios.get(
      `${API_BASE}/planning/technician-schedule/${technicianId}`,
      {
        params: {
          startDate: startDate.toISOString(),
          endDate: endDate.toISOString()
        }
      }
    );

    return response.data.dispatches.map(mapDispatchToJob);
  }

  static async lockJob(jobId: string): Promise<void> {
    // Lock via dispatch update
    const dispatch = await this.getDispatchByJobId(jobId);
    if (dispatch) {
      await axios.patch(`${API_BASE}/dispatches/${dispatch.id}/status`, {
        status: 'assigned',
        notes: 'Locked from planning board'
      });
    }
  }

  static async unassignJob(jobId: string): Promise<void> {
    const dispatch = await this.getDispatchByJobId(jobId);
    if (dispatch) {
      await axios.delete(`${API_BASE}/dispatches/${dispatch.id}`);
    }
  }

  static async resizeJob(jobId: string, newEnd: Date): Promise<void> {
    const dispatch = await this.getDispatchByJobId(jobId);
    if (dispatch) {
      await axios.put(`${API_BASE}/dispatches/${dispatch.id}`, {
        scheduledEndTime: formatTime(newEnd)
      });
    }
  }

  // Helper methods
  private static async getDispatchByJobId(jobId: string): Promise<any> {
    const response = await axios.get(`${API_BASE}/dispatches`, {
      params: { jobId }
    });
    return response.data.data[0];
  }
}

// Mapping functions
function mapJobDto(dto: any): Job {
  return {
    id: dto.id,
    serviceOrderId: dto.serviceOrderId,
    title: dto.title,
    description: dto.description,
    status: dto.status,
    priority: dto.priority,
    estimatedDuration: dto.estimatedDuration,
    requiredSkills: dto.requiredSkills,
    location: dto.location,
    customerName: dto.customer?.name,
    customerPhone: dto.customer?.phone,
    createdAt: new Date(dto.createdAt),
    updatedAt: new Date(dto.createdAt)
  };
}

function mapTechnicianDto(dto: any): Technician {
  return {
    id: dto.id,
    firstName: dto.firstName,
    lastName: dto.lastName,
    email: dto.email,
    phone: dto.phone,
    skills: dto.skills,
    status: dto.status,
    workingHours: dto.workingHours,
    avatar: dto.avatar
  };
}

function formatTime(date: Date): string {
  const hours = date.getHours().toString().padStart(2, '0');
  const minutes = date.getMinutes().toString().padStart(2, '0');
  return `${hours}:${minutes}:00`;
}
```

---

## 🗺️ Implementation Roadmap

### Phase 1: Database & Models (Week 1)
- [ ] Create migration for new tables:
  - `technicians`
  - `technician_working_hours`
  - `technician_leaves`
  - `technician_status_history`
  - `dispatch_history`
- [ ] Modify `service_order_jobs` table
- [ ] Create C# models for all entities
- [ ] Configure EF Core mappings

### Phase 2: Technician Module (Week 2)
- [ ] Create `ITechnicianService` and `TechnicianService`
- [ ] Implement `TechniciansController`
- [ ] Add DTOs for technician operations
- [ ] Test all technician endpoints
- [ ] Seed technician data from `src/data/mock/technicians.json`

### Phase 3: Planning Module - Core (Week 3)
- [ ] Create `IPlanningService` and `PlanningService`
- [ ] Implement `PlanningController`
- [ ] Add DTOs for planning operations
- [ ] Implement `GetUnassignedJobs()`
- [ ] Implement `AssignJob()` with validation
- [ ] Implement `ValidateAssignment()`
- [ ] Test assignment workflow

### Phase 4: Planning Module - Schedule (Week 4)
- [ ] Implement `GetTechnicianSchedule()`
- [ ] Implement availability calculation
- [ ] Add conflict detection logic
- [ ] Add skill matching validation
- [ ] Test schedule retrieval

### Phase 5: Dispatch Enhancements (Week 5)
- [ ] Add reassignment endpoint
- [ ] Add rescheduling endpoint
- [ ] Implement dispatch history tracking
- [ ] Add bulk operations
- [ ] Test workflow transitions

### Phase 6: Frontend Integration (Week 6)
- [ ] Replace mock `DispatcherService` with API calls
- [ ] Add loading states and error handling
- [ ] Implement real-time updates
- [ ] Add conflict warnings in UI
- [ ] Test end-to-end workflow

### Phase 7: Advanced Features (Week 7-8)
- [ ] Route optimization (optional)
- [ ] Travel time calculation
- [ ] Workload balancing
- [ ] Smart technician suggestions
- [ ] Performance optimization

### Phase 8: Testing & Polish (Week 9)
- [ ] Integration tests
- [ ] E2E tests
- [ ] Performance testing
- [ ] Bug fixes
- [ ] Documentation

---

## ✅ Success Criteria

1. **Functional**:
   - ✅ Jobs can be assigned to technicians via drag & drop
   - ✅ Conflicts are detected and prevented
   - ✅ Skills are validated before assignment
   - ✅ Technician schedules display accurately
   - ✅ Dispatches are created from assignments

2. **Performance**:
   - ✅ API responses < 500ms
   - ✅ UI remains responsive during drag operations
   - ✅ No data loss during assignment

3. **UX**:
   - ✅ Clear error messages for conflicts
   - ✅ Visual feedback during operations
   - ✅ Intuitive drag & drop interface

---

## 📝 Notes

- Mock data from `src/data/mock/technicians.json` should be migrated to database
- Consider adding WebSocket for real-time updates
- Route optimization can be added later as enhancement
- May need background job for calculating technician utilization metrics
