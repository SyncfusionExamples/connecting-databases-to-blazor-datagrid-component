-- ============================================================================
-- Generate 1000 Realistic Ticket Records with Refined Datetime Logic
-- Business Rules:
--   - CreatedAt: 3 months before current date to current date
--   - ResponseDue: Current datetime + 12-36 hours (based on priority)
--   - DueDate: ResponseDue + 1-5 days (based on priority)
--   - UpdatedAt: CreatedAt to NOW (status-dependent logic)
-- PublicTicketId: NET-1001 to NET-2000
-- ============================================================================

SET NOCOUNT ON;

-- Get current datetime for all calculations
DECLARE @CurrentDateTime DATETIME2(7) = GETDATE();
DECLARE @CurrentDate DATE = CAST(@CurrentDateTime AS DATE);
DECLARE @CurrentMonth INT = MONTH(@CurrentDate);
DECLARE @CurrentYear INT = YEAR(@CurrentDate);

-- Calculate date range for CreatedAt
DECLARE @StartMonth INT = @CurrentMonth - 3;
DECLARE @StartYear INT = @CurrentYear;
DECLARE @StartDate DATE;
DECLARE @EndDate DATE;

-- Adjust year if start month is negative
IF @StartMonth <= 0
BEGIN
    SET @StartMonth = @StartMonth + 12;
    SET @StartYear = @StartYear - 1;
END;

SET @StartDate = DATEFROMPARTS(@StartYear, @StartMonth, 1);
SET @EndDate = @CurrentDate;

DECLARE @DaysDiff INT = DATEDIFF(DAY, @StartDate, @EndDate);

IF @DaysDiff <= 0
BEGIN
    SET @StartDate = DATEFROMPARTS(YEAR(@CurrentDate), MONTH(@CurrentDate) - 3, 1);
    SET @EndDate = @CurrentDate;
    SET @DaysDiff = DATEDIFF(DAY, @StartDate, @EndDate);
END;

PRINT 'Dynamic Date Range Calculation:';
PRINT 'Current DateTime: ' + CAST(@CurrentDateTime AS NVARCHAR(25));
PRINT 'CreatedAt Range: ' + CAST(@StartDate AS NVARCHAR(10)) + ' to ' + CAST(@EndDate AS NVARCHAR(10));
PRINT 'ResponseDue/DueDate: Future dates from current datetime';
PRINT 'Days in CreatedAt Range: ' + CAST(@DaysDiff AS NVARCHAR(10));
PRINT '';

-- ============================================================================
-- Lookup Tables with Real Names (FirstName LastName format)
-- ============================================================================

DECLARE @Statuses TABLE (
    Id INT IDENTITY(1,1),
    Status NVARCHAR(50)
);

INSERT INTO @Statuses VALUES
(N'Open'), (N'InProgress'), (N'WaitingForCustomer'), (N'WaitingForAgent'), (N'Resolved'), (N'Closed');

DECLARE @Priorities TABLE (
    Id INT IDENTITY(1,1),
    Priority NVARCHAR(50)
);

INSERT INTO @Priorities VALUES
(N'Critical'), (N'High'), (N'Medium'), (N'Low');

DECLARE @Categories TABLE (
    Id INT IDENTITY(1,1),
    Category NVARCHAR(50)
);

INSERT INTO @Categories VALUES
(N'Network Issue'), (N'Performance'), (N'VPN'), (N'Hardware'), (N'Server Issue'), 
(N'Security'), (N'Connectivity'), (N'Software'), (N'Access'), (N'Backup'), (N'Database');

DECLARE @Departments TABLE (
    Id INT IDENTITY(1,1),
    Department NVARCHAR(50)
);

INSERT INTO @Departments VALUES
(N'IT Support'), (N'Network Ops'), (N'IT Security'), (N'Infrastructure'), 
(N'Database Admin'), (N'Help Desk'), (N'Cloud Services'), (N'DevOps');

DECLARE @Titles TABLE (
    Id INT IDENTITY(1,1),
    Title NVARCHAR(250)
);

INSERT INTO @Titles VALUES
(N'Network Connectivity Issue'), (N'System Performance Degradation'), (N'Email Server Unavailable'),
(N'VPN Connection Failed'), (N'Database Query Timeout'), (N'Hardware Device Failure'),
(N'Software Installation Error'), (N'Access Permission Issue'), (N'Backup Failure Alert'),
(N'Printer Not Responding'), (N'Slow Internet Speed'), (N'Wi-Fi Disconnection'),
(N'Application Crash'), (N'File Corruption Detected'), (N'License Expiration Warning'),
(N'Server Down Alert'), (N'Data Sync Issue'), (N'Security Update Required'),
(N'Password Reset Request'), (N'New User Setup'), (N'Firewall Rule Update Needed'),
(N'Storage Space Low'), (N'Monitor Resolution Issue'), (N'Browser Compatibility Problem'),
(N'Mobile Device Sync Error'), (N'Cloud Service Access Denied'), (N'API Integration Failure'),
(N'Report Generation Failed'), (N'Document Upload Issue'), (N'Calendar Sync Problem'),
(N'DNS Resolution Failed'), (N'SSL Certificate Error'), (N'User Account Locked'),
(N'Disk Space Critical'), (N'Service Timeout'), (N'Authentication Failure');

-- ============================================================================
-- Generate 1000 Records with Business Logic
-- ============================================================================

DECLARE @Counter INT = 1;
DECLARE @TicketCounter INT = 1001;
DECLARE @TicketCount INT = 1000;

WHILE @Counter <= @TicketCount
BEGIN
    -- Random values for CreatedAt (3 months back to now)
    DECLARE @RandomDayOffset INT = CASE 
        WHEN @Counter <= @TicketCount * 0.10 THEN ABS(CHECKSUM(NEWID())) % 7  -- 10% very recent
        WHEN @Counter <= @TicketCount * 0.60 THEN ABS(CHECKSUM(NEWID())) % (@DaysDiff / 2)  -- 50% middle
        ELSE ABS(CHECKSUM(NEWID())) % @DaysDiff  -- 40% older
    END;
    
    DECLARE @CreatedAtDate DATETIME2(7) = DATEADD(DAY, @RandomDayOffset, @StartDate);
    DECLARE @RandomHour INT = ABS(CHECKSUM(NEWID())) % 24;
    DECLARE @RandomMinute INT = ABS(CHECKSUM(NEWID())) % 60;
    DECLARE @RandomSecond INT = ABS(CHECKSUM(NEWID())) % 60;
    
    SET @CreatedAtDate = DATEADD(SECOND, @RandomSecond, DATEADD(MINUTE, @RandomMinute, DATEADD(HOUR, @RandomHour, @CreatedAtDate)));
    
    -- Select random status
    DECLARE @StatusIndex INT = ABS(CHECKSUM(NEWID())) % 6;
    DECLARE @RandomStatus NVARCHAR(50) = CASE @StatusIndex
        WHEN 0 THEN N'Open'
        WHEN 1 THEN N'InProgress'
        WHEN 2 THEN N'WaitingForCustomer'
        WHEN 3 THEN N'WaitingForAgent'
        WHEN 4 THEN N'Resolved'
        ELSE N'Closed'
    END;
    
    -- Select random priority
    DECLARE @PriorityIndex INT = ABS(CHECKSUM(NEWID())) % 4;
    DECLARE @RandomPriority NVARCHAR(50) = CASE @PriorityIndex
        WHEN 0 THEN N'Critical'
        WHEN 1 THEN N'High'
        WHEN 2 THEN N'Medium'
        ELSE N'Low'
    END;
    
    -- Select random category
    DECLARE @CategoryIndex INT = ABS(CHECKSUM(NEWID())) % 11;
    DECLARE @RandomCategory NVARCHAR(50) = CASE @CategoryIndex
        WHEN 0 THEN N'Network Issue'
        WHEN 1 THEN N'Performance'
        WHEN 2 THEN N'VPN'
        WHEN 3 THEN N'Hardware'
        WHEN 4 THEN N'Server Issue'
        WHEN 5 THEN N'Security'
        WHEN 6 THEN N'Connectivity'
        WHEN 7 THEN N'Software'
        WHEN 8 THEN N'Access'
        WHEN 9 THEN N'Backup'
        ELSE N'Database'
    END;
    
    -- Select department based on category correlation
    DECLARE @RandomDepartment NVARCHAR(50) = CASE @RandomCategory
        WHEN N'Network Issue' THEN (CASE WHEN ABS(CHECKSUM(NEWID())) % 100 < 60 THEN N'Network Ops' ELSE N'IT Support' END)
        WHEN N'Performance' THEN N'Network Ops'
        WHEN N'VPN' THEN (CASE WHEN ABS(CHECKSUM(NEWID())) % 100 < 70 THEN N'IT Security' ELSE N'Network Ops' END)
        WHEN N'Hardware' THEN N'IT Support'
        WHEN N'Server Issue' THEN (CASE WHEN ABS(CHECKSUM(NEWID())) % 100 < 60 THEN N'Infrastructure' ELSE N'DevOps' END)
        WHEN N'Security' THEN N'IT Security'
        WHEN N'Connectivity' THEN N'Network Ops'
        WHEN N'Software' THEN (CASE WHEN ABS(CHECKSUM(NEWID())) % 100 < 50 THEN N'Help Desk' ELSE N'Cloud Services' END)
        WHEN N'Access' THEN N'Help Desk'
        WHEN N'Backup' THEN N'Infrastructure'
        ELSE N'Database Admin'
    END;
    
    -- Always assign a ticket based on department (no unassigned tickets)
    DECLARE @AssigneeSelector INT = ABS(CHECKSUM(NEWID())) % 100;
    DECLARE @RandomAssignee NVARCHAR(100) = CASE @RandomDepartment
        WHEN N'IT Support' THEN CASE WHEN @AssigneeSelector < 40 THEN N'John Doe' WHEN @AssigneeSelector < 70 THEN N'Emily White' ELSE N'James Miller' END
        WHEN N'Network Ops' THEN CASE WHEN @AssigneeSelector < 50 THEN N'Lisa Taylor' ELSE N'Susan Clark' END
        WHEN N'IT Security' THEN CASE WHEN @AssigneeSelector < 60 THEN N'Sarah Lee' ELSE N'Robert Harris' END
        WHEN N'Infrastructure' THEN CASE WHEN @AssigneeSelector < 50 THEN N'Mike Green' ELSE N'Nancy Wilson' END
        WHEN N'Database Admin' THEN N'Mike Green'
        WHEN N'Help Desk' THEN CASE WHEN @AssigneeSelector < 50 THEN N'John Doe' ELSE N'Emily White' END
        WHEN N'Cloud Services' THEN N'David Brown'
        ELSE CASE WHEN @AssigneeSelector < 50 THEN N'David Brown' ELSE N'Nancy Wilson' END
    END;
    
    -- Select CreatedBy
    DECLARE @CreatedByIndex INT = ABS(CHECKSUM(NEWID())) % 14;
    DECLARE @RandomCreatedBy NVARCHAR(100) = CASE @CreatedByIndex
        WHEN 0 THEN N'Alice Smith'
        WHEN 1 THEN N'Bob Johnson'
        WHEN 2 THEN N'David Brown'
        WHEN 3 THEN N'Emily White'
        WHEN 4 THEN N'Nancy Wilson'
        WHEN 5 THEN N'Robert Harris'
        WHEN 6 THEN N'System Monitor'
        WHEN 7 THEN N'Reception Desk'
        WHEN 8 THEN N'User Support'
        WHEN 9 THEN N'Admin User'
        WHEN 10 THEN N'John Doe'
        WHEN 11 THEN N'Sarah Lee'
        WHEN 12 THEN N'Help Desk'
        ELSE N'IT Operations'
    END;
    
    -- Select Title
    DECLARE @TitleIndex INT = ABS(CHECKSUM(NEWID())) % 36;
    DECLARE @RandomTitle NVARCHAR(250) = CASE @TitleIndex
        WHEN 0 THEN N'Network Connectivity Issue'
        WHEN 1 THEN N'System Performance Degradation'
        WHEN 2 THEN N'Email Server Unavailable'
        WHEN 3 THEN N'VPN Connection Failed'
        WHEN 4 THEN N'Database Query Timeout'
        WHEN 5 THEN N'Hardware Device Failure'
        WHEN 6 THEN N'Software Installation Error'
        WHEN 7 THEN N'Access Permission Issue'
        WHEN 8 THEN N'Backup Failure Alert'
        WHEN 9 THEN N'Printer Not Responding'
        WHEN 10 THEN N'Slow Internet Speed'
        WHEN 11 THEN N'Wi-Fi Disconnection'
        WHEN 12 THEN N'Application Crash'
        WHEN 13 THEN N'File Corruption Detected'
        WHEN 14 THEN N'License Expiration Warning'
        WHEN 15 THEN N'Server Down Alert'
        WHEN 16 THEN N'Data Sync Issue'
        WHEN 17 THEN N'Security Update Required'
        WHEN 18 THEN N'Password Reset Request'
        WHEN 19 THEN N'New User Setup'
        WHEN 20 THEN N'Firewall Rule Update Needed'
        WHEN 21 THEN N'Storage Space Low'
        WHEN 22 THEN N'Monitor Resolution Issue'
        WHEN 23 THEN N'Browser Compatibility Problem'
        WHEN 24 THEN N'Mobile Device Sync Error'
        WHEN 25 THEN N'Cloud Service Access Denied'
        WHEN 26 THEN N'API Integration Failure'
        WHEN 27 THEN N'Report Generation Failed'
        WHEN 28 THEN N'Document Upload Issue'
        WHEN 29 THEN N'Calendar Sync Problem'
        WHEN 30 THEN N'DNS Resolution Failed'
        WHEN 31 THEN N'SSL Certificate Error'
        WHEN 32 THEN N'User Account Locked'
        WHEN 33 THEN N'Disk Space Critical'
        WHEN 34 THEN N'Service Timeout'
        ELSE N'Authentication Failure'
    END;
    
    -- Select Description based on Status
    DECLARE @RandomDescription NVARCHAR(MAX);
    DECLARE @DescriptionIndex INT;
    
    IF @RandomStatus = N'Open'
    BEGIN
        SET @DescriptionIndex = ABS(CHECKSUM(NEWID())) % 5;
        SET @RandomDescription = CASE @DescriptionIndex
            WHEN 0 THEN N'User experiencing connectivity issues. Troubleshooting steps attempted without success.'
            WHEN 1 THEN N'System running slowly. Resource utilization appears high. Performance monitoring needed.'
            WHEN 2 THEN N'Server responding intermittently. Check server status and system logs for errors.'
            WHEN 3 THEN N'Unable to establish VPN connection. Authentication appears to be failing.'
            ELSE N'Database query taking longer than expected. Query optimization may be required.'
        END;
    END
    ELSE IF @RandomStatus = N'InProgress'
    BEGIN
        SET @DescriptionIndex = ABS(CHECKSUM(NEWID())) % 13;
        SET @RandomDescription = CASE @DescriptionIndex
            WHEN 0 THEN N'Hardware device not responding. Physical inspection and diagnostics in progress. Ordered replacement parts.'
            WHEN 1 THEN N'Software installation failed with error code. Reinstalling with latest version. Expected completion in 2 hours.'
            WHEN 2 THEN N'User lacks required permissions to access application. AD group membership being configured. Testing permissions.'
            WHEN 3 THEN N'Backup job completed with errors. Investigating storage issues and clearing space. Rerunning backup.'
            WHEN 4 THEN N'Network printer offline. Checking cables and network connectivity. Firmware update pending.'
            WHEN 5 THEN N'Internet connection speed abnormally low. Running diagnostics on ISP connection. Technician scheduled.'
            WHEN 6 THEN N'Wi-Fi dropping frequently. Interference analysis completed. Repositioning access point and reconfiguring channels.'
            WHEN 7 THEN N'Application crashing on specific operations. Debugging in progress. Isolated the problematic code module.'
            WHEN 8 THEN N'Corrupted file preventing access to documents. Data recovery process initiated. Recovery tools running.'
            WHEN 9 THEN N'Software license expiration imminent. Renewal request submitted. Awaiting vendor confirmation.'
            WHEN 10 THEN N'Server is unresponsive but monitoring system shows activity. Checking network connectivity and system logs.'
            WHEN 11 THEN N'Data not syncing across devices. Clearing cache and restarting sync services. Testing synchronization.'
            ELSE N'Security update available and critical. Testing patch in staging environment. Deployment scheduled for maintenance window.'
        END;
    END
    ELSE IF @RandomStatus = N'WaitingForCustomer'
    BEGIN
        SET @DescriptionIndex = ABS(CHECKSUM(NEWID())) % 10;
        SET @RandomDescription = CASE @DescriptionIndex
            WHEN 0 THEN N'Waiting for customer response on additional details needed for diagnosis. Awaiting customer feedback on error messages.'
            WHEN 1 THEN N'Waiting for customer to provide system logs and configuration details. Sent diagnostic script for customer to run.'
            WHEN 2 THEN N'Waiting for customer to perform requested troubleshooting steps. Provided step-by-step instructions.'
            WHEN 3 THEN N'Waiting for customer approval to proceed with system changes. Sent proposal for review.'
            WHEN 4 THEN N'Waiting for manager approval on resource allocation for this issue. Escalated for authorization.'
            WHEN 5 THEN N'Waiting for hardware vendor to respond with replacement availability. Case reference provided to vendor.'
            WHEN 6 THEN N'Waiting for software vendor support ticket resolution. Reference number: SUP-2027-001234.'
            WHEN 7 THEN N'Waiting for customer to schedule maintenance window for system update. Proposed three available time slots.'
            WHEN 8 THEN N'Waiting for additional budget approval for solution implementation. Cost estimate submitted to finance.'
            ELSE N'Waiting for customer to upgrade their system before we can implement the fix. Compatibility issue identified.'
        END;
    END
    ELSE IF @RandomStatus = N'WaitingForAgent'
    BEGIN
        SET @RandomDescription = N'Ticket assigned and queued for agent assignment. Awaiting available technician to begin work.';
    END
    ELSE IF @RandomStatus = N'Resolved'
    BEGIN
        SET @DescriptionIndex = ABS(CHECKSUM(NEWID())) % 17;
        SET @RandomDescription = CASE @DescriptionIndex
            WHEN 0 THEN N'Issue resolved. Problem was due to misconfigured network settings. Updated configuration and verified functionality.'
            WHEN 1 THEN N'Issue resolved. System performance restored after clearing cache and optimizing database queries.'
            WHEN 2 THEN N'Issue resolved. Server service restored after correcting configuration. All services operational and responding normally.'
            WHEN 3 THEN N'Issue resolved. VPN authentication issue fixed. Reset user credentials and verified connection from multiple locations.'
            WHEN 4 THEN N'Issue resolved. Database query optimized. Indexes rebuilt and query execution time reduced to acceptable levels.'
            WHEN 5 THEN N'Issue resolved. Hardware device replaced and reconfigured. All tests passed. User can now access device normally.'
            WHEN 6 THEN N'Issue resolved. Software installed successfully after clearing installation cache. Application working properly.'
            WHEN 7 THEN N'Issue resolved. User permissions configured correctly. User now has access to all required applications.'
            WHEN 8 THEN N'Issue resolved. Backup storage expanded and backup job re-run successfully. All backups current.'
            WHEN 9 THEN N'Issue resolved. Network printer reconfigured and back online. Users can print successfully.'
            WHEN 10 THEN N'Issue resolved. ISP connection upgraded. Internet speed now meeting requirements. User confirmed.'
            WHEN 11 THEN N'Issue resolved. Wi-Fi coverage improved after relocating access points. Signal strength now optimal.'
            WHEN 12 THEN N'Issue resolved. Application crashing issue fixed after patching. User tested and confirmed working.'
            WHEN 13 THEN N'Issue resolved. Corrupted files recovered successfully. User verified all data is accessible.'
            WHEN 14 THEN N'Issue resolved. Software license renewed. License keys updated in all systems. Service continues without interruption.'
            WHEN 15 THEN N'Issue resolved. Server connectivity restored. Network configuration corrected. All services operational.'
            ELSE N'Issue resolved. Data synchronization working correctly. User verified sync across all devices.'
        END;
    END
    ELSE -- Closed
    BEGIN
        SET @DescriptionIndex = ABS(CHECKSUM(NEWID())) % 12;
        SET @RandomDescription = CASE @DescriptionIndex
            WHEN 0 THEN N'Ticket closed. Issue resolved and verified by customer. User confirmed system working properly. Closed per customer request.'
            WHEN 1 THEN N'Ticket closed. Problem was resolved 15 days ago. Customer did not respond to follow-up. Closing due to inactivity.'
            WHEN 2 THEN N'Ticket closed. Duplicate of ticket NET-0856. Customer directed to existing ticket for updates.'
            WHEN 3 THEN N'Ticket closed. User request for feature enhancement. Submitted to product development team for future consideration.'
            WHEN 4 THEN N'Ticket closed. User report of intermittent issue has not reoccurred for 7 days. Monitoring continues.'
            WHEN 5 THEN N'Ticket closed. Issue was resolved through alternate workaround. Customer opted not to proceed with full solution.'
            WHEN 6 THEN N'Ticket closed. System maintenance completed successfully. All updates applied and tested thoroughly.'
            WHEN 7 THEN N'Ticket closed. Customer satisfied with resolution. Follow-up meeting scheduled in 30 days to verify.'
            WHEN 8 THEN N'Ticket closed. Workaround provided and documented for future reference. User trained on proper procedure.'
            WHEN 9 THEN N'Ticket closed. Issue was caused by external factor now resolved. No further action required.'
            WHEN 10 THEN N'Ticket closed. Customer decided to proceed with manual process. Automation no longer needed.'
            ELSE N'Ticket closed. Investigation complete. Issue was transient and has not reoccurred for 14 days.'
        END;
    END;
    
    -- ============================================================================
    -- NEW LOGIC: Priority-based ResponseDue and DueDate (FUTURE DATES)
    -- ============================================================================
    DECLARE @ResponseDueDate DATETIME2(7);
    DECLARE @DueDateVal DATETIME2(7);
    DECLARE @ResponseHours INT;
    DECLARE @DueDays INT;
    
    -- Critical: ResponseDue 12-16 hours from now, DueDate ResponseDue + 1 day
    IF @RandomPriority = N'Critical'
    BEGIN
        SET @ResponseHours = 12 + ABS(CHECKSUM(NEWID())) % 4;
        SET @DueDays = 1;
    END
    -- High: ResponseDue 16-24 hours from now, DueDate ResponseDue + 2 days
    ELSE IF @RandomPriority = N'High'
    BEGIN
        SET @ResponseHours = 16 + ABS(CHECKSUM(NEWID())) % 8;
        SET @DueDays = 2;
    END
    -- Medium: ResponseDue 24-30 hours from now, DueDate ResponseDue + 3 days
    ELSE IF @RandomPriority = N'Medium'
    BEGIN
        SET @ResponseHours = 24 + ABS(CHECKSUM(NEWID())) % 6;
        SET @DueDays = 3;
    END
    -- Low: ResponseDue 30-36 hours from now, DueDate ResponseDue + 4-5 days
    ELSE
    BEGIN
        SET @ResponseHours = 30 + ABS(CHECKSUM(NEWID())) % 6;
        SET @DueDays = 4 + ABS(CHECKSUM(NEWID())) % 2;
    END;
    
    SET @ResponseDueDate = DATEADD(HOUR, @ResponseHours, @CurrentDateTime);
    SET @DueDateVal = DATEADD(DAY, @DueDays, @ResponseDueDate);
    
    -- ============================================================================
    -- NEW LOGIC: UpdatedAt (between CreatedAt and NOW) - Status Dependent
    -- ============================================================================
    DECLARE @UpdatedAtDate DATETIME2(7);
    DECLARE @HoursFromCreated INT;
    DECLARE @RandomUpdateSelector INT;
    
    IF @RandomStatus = N'Open'
    BEGIN
        -- Open status: UpdatedAt = CreatedAt (no updates)
        SET @UpdatedAtDate = @CreatedAtDate;
    END
    ELSE IF @RandomStatus = N'InProgress'
    BEGIN
        -- InProgress: Can have multiple updates, distributed randomly from Created to NOW
        SET @HoursFromCreated = DATEDIFF(HOUR, @CreatedAtDate, @CurrentDateTime);
        IF @HoursFromCreated > 0
            SET @UpdatedAtDate = DATEADD(HOUR, ABS(CHECKSUM(NEWID())) % @HoursFromCreated, @CreatedAtDate)
        ELSE
            SET @UpdatedAtDate = @CreatedAtDate;
    END
    ELSE IF @RandomStatus = N'WaitingForCustomer'
    BEGIN
        -- WaitingForCustomer: Updates within past 24 hours preferred (or less)
        SET @HoursFromCreated = DATEDIFF(HOUR, @CreatedAtDate, @CurrentDateTime);
        IF @HoursFromCreated > 24
            SET @UpdatedAtDate = DATEADD(HOUR, ABS(CHECKSUM(NEWID())) % 24, DATEADD(HOUR, -24, @CurrentDateTime))
        ELSE IF @HoursFromCreated > 0
            SET @UpdatedAtDate = DATEADD(HOUR, ABS(CHECKSUM(NEWID())) % @HoursFromCreated, @CreatedAtDate)
        ELSE
            SET @UpdatedAtDate = @CreatedAtDate;
    END
    ELSE IF @RandomStatus = N'WaitingForAgent'
    BEGIN
        -- WaitingForAgent: Updates within past 24 hours preferred
        SET @HoursFromCreated = DATEDIFF(HOUR, @CreatedAtDate, @CurrentDateTime);
        IF @HoursFromCreated > 24
            SET @UpdatedAtDate = DATEADD(HOUR, ABS(CHECKSUM(NEWID())) % 24, DATEADD(HOUR, -24, @CurrentDateTime))
        ELSE IF @HoursFromCreated > 0
            SET @UpdatedAtDate = DATEADD(HOUR, ABS(CHECKSUM(NEWID())) % @HoursFromCreated, @CreatedAtDate)
        ELSE
            SET @UpdatedAtDate = @CreatedAtDate;
    END
    ELSE IF @RandomStatus = N'Resolved'
    BEGIN
        -- Resolved: Updates within past 24 hours (closer to NOW)
        SET @HoursFromCreated = DATEDIFF(HOUR, @CreatedAtDate, @CurrentDateTime);
        IF @HoursFromCreated > 24
            SET @UpdatedAtDate = DATEADD(HOUR, ABS(CHECKSUM(NEWID())) % 24, DATEADD(HOUR, -24, @CurrentDateTime))
        ELSE IF @HoursFromCreated > 0
            SET @UpdatedAtDate = DATEADD(HOUR, ABS(CHECKSUM(NEWID())) % @HoursFromCreated, @CreatedAtDate)
        ELSE
            SET @UpdatedAtDate = @CreatedAtDate;
    END
    ELSE -- Closed
    BEGIN
        -- Closed: Very recent updates (within past 24 hours, preferably recent)
        SET @UpdatedAtDate = DATEADD(MINUTE, -(ABS(CHECKSUM(NEWID())) % 1440), @CurrentDateTime);
    END;
    
    -- PublicTicketId
    DECLARE @PublicTicketId NVARCHAR(50) = 'NET-' + CAST(@TicketCounter AS NVARCHAR(10));
    
    -- Insert the record
    INSERT INTO dbo.Tickets 
    (PublicTicketId, Title, Description, Category, Department, Assignee, CreatedBy, Status, Priority, ResponseDue, DueDate, CreatedAt, UpdatedAt)
    VALUES
    (@PublicTicketId, @RandomTitle, @RandomDescription, @RandomCategory, @RandomDepartment, @RandomAssignee, @RandomCreatedBy, @RandomStatus, @RandomPriority, @ResponseDueDate, @DueDateVal, @CreatedAtDate, @UpdatedAtDate);
    
    SET @Counter = @Counter + 1;
    SET @TicketCounter = @TicketCounter + 1;
    
    -- Progress indicator every 100 records
    IF @Counter % 100 = 0
        PRINT 'Generated ' + CAST(@Counter AS NVARCHAR(10)) + ' records...';
END;

-- ============================================================================
-- Summary and Verification
-- ============================================================================

DECLARE @TotalRecords INT = (SELECT COUNT(*) FROM dbo.Tickets);
DECLARE @OpenCount INT = (SELECT COUNT(*) FROM dbo.Tickets WHERE Status = 'Open');
DECLARE @InProgressCount INT = (SELECT COUNT(*) FROM dbo.Tickets WHERE Status = 'InProgress');
DECLARE @WaitingCustomerCount INT = (SELECT COUNT(*) FROM dbo.Tickets WHERE Status = 'WaitingForCustomer');
DECLARE @WaitingAgentCount INT = (SELECT COUNT(*) FROM dbo.Tickets WHERE Status = 'WaitingForAgent');
DECLARE @ResolvedCount INT = (SELECT COUNT(*) FROM dbo.Tickets WHERE Status = 'Resolved');
DECLARE @ClosedCount INT = (SELECT COUNT(*) FROM dbo.Tickets WHERE Status = 'Closed');
DECLARE @AssignedCount INT = (SELECT COUNT(*) FROM dbo.Tickets WHERE Assignee IS NOT NULL);
DECLARE @UnassignedCount INT = (SELECT COUNT(*) FROM dbo.Tickets WHERE Assignee IS NULL);

-- Verify date ranges
DECLARE @MinCreatedAt DATE = (SELECT MIN(CAST(CreatedAt AS DATE)) FROM dbo.Tickets);
DECLARE @MaxCreatedAt DATE = (SELECT MAX(CAST(CreatedAt AS DATE)) FROM dbo.Tickets);
DECLARE @MinResponseDue DATE = (SELECT MIN(CAST(ResponseDue AS DATE)) FROM dbo.Tickets);
DECLARE @MaxResponseDue DATE = (SELECT MAX(CAST(ResponseDue AS DATE)) FROM dbo.Tickets);
DECLARE @MinUpdatedAt DATE = (SELECT MIN(CAST(UpdatedAt AS DATE)) FROM dbo.Tickets);
DECLARE @MaxUpdatedAt DATE = (SELECT MAX(CAST(UpdatedAt AS DATE)) FROM dbo.Tickets);

PRINT '';
PRINT '========================================';
PRINT 'Successfully generated 1000 ticket records!';
PRINT '========================================';
PRINT 'Total Records: ' + CAST(@TotalRecords AS NVARCHAR(10));
PRINT '';
PRINT 'Status Distribution:';
PRINT '  Open: ' + CAST(@OpenCount AS NVARCHAR(10)) + ' (' + CAST(CAST(@OpenCount * 100 / @TotalRecords AS INT) AS NVARCHAR(10)) + '%)';
PRINT '  InProgress: ' + CAST(@InProgressCount AS NVARCHAR(10)) + ' (' + CAST(CAST(@InProgressCount * 100 / @TotalRecords AS INT) AS NVARCHAR(10)) + '%)';
PRINT '  WaitingForCustomer: ' + CAST(@WaitingCustomerCount AS NVARCHAR(10)) + ' (' + CAST(CAST(@WaitingCustomerCount * 100 / @TotalRecords AS INT) AS NVARCHAR(10)) + '%)';
PRINT '  WaitingForAgent: ' + CAST(@WaitingAgentCount AS NVARCHAR(10)) + ' (' + CAST(CAST(@WaitingAgentCount * 100 / @TotalRecords AS INT) AS NVARCHAR(10)) + '%)';
PRINT '  Resolved: ' + CAST(@ResolvedCount AS NVARCHAR(10)) + ' (' + CAST(CAST(@ResolvedCount * 100 / @TotalRecords AS INT) AS NVARCHAR(10)) + '%)';
PRINT '  Closed: ' + CAST(@ClosedCount AS NVARCHAR(10)) + ' (' + CAST(CAST(@ClosedCount * 100 / @TotalRecords AS INT) AS NVARCHAR(10)) + '%)';
PRINT '';
PRINT 'Assignment Status:';
PRINT '  Assigned: ' + CAST(@AssignedCount AS NVARCHAR(10)) + ' (100%)';
PRINT '  Unassigned: ' + CAST(@UnassignedCount AS NVARCHAR(10)) + ' (0%)';
PRINT '';
PRINT 'DateTime Ranges:';
PRINT '  CreatedAt: ' + CAST(@MinCreatedAt AS NVARCHAR(10)) + ' to ' + CAST(@MaxCreatedAt AS NVARCHAR(10));
PRINT '  ResponseDue: ' + CAST(@MinResponseDue AS NVARCHAR(10)) + ' to ' + CAST(@MaxResponseDue AS NVARCHAR(10)) + ' (FUTURE dates)';
PRINT '  UpdatedAt: ' + CAST(@MinUpdatedAt AS NVARCHAR(10)) + ' to ' + CAST(@MaxUpdatedAt AS NVARCHAR(10)) + ' (Past or Today)';
PRINT '';
PRINT 'PublicTicketId Range: NET-1001 to NET-2000';
PRINT 'Names Format: FirstName LastName (e.g., John Doe)';
PRINT '';
PRINT 'Priority-Based DateTime Rules Applied:';
PRINT '  Critical: ResponseDue 12-16hrs from now, DueDate ResponseDue+1day';
PRINT '  High: ResponseDue 16-24hrs from now, DueDate ResponseDue+2days';
PRINT '  Medium: ResponseDue 24-30hrs from now, DueDate ResponseDue+3days';
PRINT '  Low: ResponseDue 30-36hrs from now, DueDate ResponseDue+4-5days';
PRINT '';
PRINT 'UpdatedAt Status Logic:';
PRINT '  Open: Same as CreatedAt (no updates)';
PRINT '  InProgress: Random between CreatedAt and NOW';
PRINT '  WaitingForCustomer: Preferably past 24 hours';
PRINT '  WaitingForAgent: Preferably past 24 hours';
PRINT '  Resolved: Preferably past 24 hours (closer to NOW)';
PRINT '  Closed: Very recent (within past 24 hours)';
PRINT '========================================';

SET NOCOUNT OFF;
