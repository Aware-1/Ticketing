namespace QodraTick.Components.Admin
{
    public partial class SystemSettings

    {
        // General Settings
        public string OrganizationName { get; set; } = string.Empty;
        public int MaxActiveTicketsPerUser { get; set; } = 5;
        public int MaxTicketsPerSupport { get; set; } = 20;
        public bool AutoAssignTickets { get; set; } = true;
        public bool AllowTicketReopening { get; set; } = false;

        // SLA Settings
        public double CriticalSLAHours { get; set; } = 2;
        public double HighSLAHours { get; set; } = 4;
        public double NormalSLAHours { get; set; } = 24;
        public double LowSLAHours { get; set; } = 48;
        public bool EnableSLAAlerts { get; set; } = true;

        // Notification Settings
        public bool NotifyOnNewTicket { get; set; } = true;
        public bool NotifyOnTicketAssigned { get; set; } = true;
        public bool NotifyOnStatusChange { get; set; } = true;
        public bool NotifyOnNewMessage { get; set; } = true;
        public bool EnableEmailNotifications { get; set; } = false;
        public bool EnableSoundNotifications { get; set; } = true;

        // Security Settings
        public int MaxLoginAttempts { get; set; } = 5;
        public int LockoutDurationMinutes { get; set; } = 15;
        public int SessionTimeoutHours { get; set; } = 8;
        public bool RequirePasswordChange { get; set; } = false;
        public bool EnableTwoFactorAuth { get; set; } = false;
        public bool LogUserActivity { get; set; } = true;

        // File Upload Settings
        public int MaxFileSizeMB { get; set; } = 5;
        public string AllowedFileExtensions { get; set; } = ".jpg,.png,.pdf,.docx,.txt";
        public bool AllowFileUploads { get; set; } = true;
        public bool ScanUploadedFiles { get; set; } = false;
    }
}
