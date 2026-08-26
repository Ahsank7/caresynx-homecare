using Scheduler.API.Models.Email;
using Scheduler.API.Models.ServicesTask;

namespace Scheduler.API.Services.Email
{
    public static class TaskEmailTemplates
    {
        public static EmailMessage CreateTaskStatusChangeEmail(
            ServicesTaskDetail task,
            string recipientEmail,
            string recipientName,
            string recipientType, // "Client" or "Service Provider"
            string previousStatus,
            string newStatus)
        {
            var taskDate = task.Date.ToString("MMMM dd, yyyy");
            var startTime = task.StartTime.ToString("hh:mm tt");
            var endTime = task.EndTime.ToString("hh:mm tt");
            
            var statusChangeDescription = $"The status of your task has been changed from <strong>{previousStatus}</strong> to <strong>{newStatus}</strong>.";
            
            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4a90e2; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .task-details {{ background-color: white; padding: 15px; margin: 15px 0; border-radius: 5px; border-left: 4px solid #4a90e2; }}
        .status-box {{ background-color: #e3f2fd; padding: 15px; text-align: center; border-radius: 5px; margin: 15px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
        table {{ width: 100%; border-collapse: collapse; margin: 15px 0; }}
        td {{ padding: 8px; border-bottom: 1px solid #eee; }}
        td:first-child {{ font-weight: bold; width: 40%; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Task Status Update</h1>
        </div>
        <div class=""content"">
            <p>Dear {recipientName},</p>
            <p>{statusChangeDescription}</p>
            
            <div class=""status-box"">
                <h3 style=""margin: 0; color: #4a90e2;"">New Status: {newStatus}</h3>
            </div>
            
            <div class=""task-details"">
                <h3>Task Information</h3>
                <table>
                    <tr>
                        <td>Task ID:</td>
                        <td>#{task.TaskId}</td>
                    </tr>
                    <tr>
                        <td>Schedule ID:</td>
                        <td>#{task.ScheduleId}</td>
                    </tr>
                    <tr>
                        <td>Task Date:</td>
                        <td>{taskDate}</td>
                    </tr>
                    <tr>
                        <td>Time:</td>
                        <td>{startTime} - {endTime}</td>
                    </tr>
                    <tr>
                        <td>Service Type:</td>
                        <td>{task.ServiceType ?? "N/A"}</td>
                    </tr>
                    <tr>
                        <td>Service Name:</td>
                        <td>{task.ServiceName ?? "N/A"}</td>
                    </tr>
                    <tr>
                        <td>Client:</td>
                        <td>{task.ClientName ?? "N/A"}</td>
                    </tr>
                    <tr>
                        <td>Service Provider:</td>
                        <td>{task.ServiceProviderName ?? "N/A"}</td>
                    </tr>
                    <tr>
                        <td>Location:</td>
                        <td>{task.ClientAddress ?? "N/A"}</td>
                    </tr>
                </table>
            </div>

            <p>If you have any questions about this task or its status change, please contact our support team.</p>
            
            <p>Best regards,<br>eXtremeScheduler Team</p>
        </div>
        <div class=""footer"">
            <p>This is an automated email. Please do not reply to this message.</p>
            <p>&copy; {DateTime.Now.Year} eXtremeScheduler. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

            return new EmailMessage
            {
                To = recipientEmail,
                Subject = $"Task Status Update - Task #{task.TaskId} is now {newStatus}",
                Body = body,
                IsHtml = true
            };
        }

        public static EmailMessage CreateScheduleCreatedEmail(
            string recipientEmail,
            string recipientName,
            string recipientType, // "Client" or "Service Provider"
            int scheduleId,
            DateTime startTime,
            DateTime endTime,
            string recurrencePattern,
            string serviceType,
            string serviceName,
            string clientName,
            string serviceProviderName,
            string description)
        {
            var startDate = startTime.ToString("MMMM dd, yyyy hh:mm tt");
            var endDate = endTime.ToString("MMMM dd, yyyy hh:mm tt");
            
            string roleSpecificMessage;
            if (recipientType == "Client")
            {
                roleSpecificMessage = $"<p>You will be receiving services from <strong>{serviceProviderName}</strong> as per the schedule below.</p>";
            }
            else
            {
                roleSpecificMessage = $"<p>You will be providing services to <strong>{clientName}</strong> as per the schedule below.</p>";
            }

            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #28a745; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .schedule-details {{ background-color: white; padding: 15px; margin: 15px 0; border-radius: 5px; border-left: 4px solid #28a745; }}
        .info-box {{ background-color: #d4edda; padding: 15px; border-radius: 5px; margin: 15px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
        table {{ width: 100%; border-collapse: collapse; margin: 15px 0; }}
        td {{ padding: 8px; border-bottom: 1px solid #eee; }}
        td:first-child {{ font-weight: bold; width: 40%; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>✓ New Schedule Created</h1>
        </div>
        <div class=""content"">
            <p>Dear {recipientName},</p>
            
            <div class=""info-box"">
                <h3 style=""margin-top: 0;"">A new schedule has been created for you!</h3>
                <p style=""margin-bottom: 0;"">Schedule ID: <strong>#{scheduleId}</strong></p>
            </div>
            
            {roleSpecificMessage}
            
            <div class=""schedule-details"">
                <h3>Schedule Details</h3>
                <table>
                    <tr>
                        <td>Schedule ID:</td>
                        <td>#{scheduleId}</td>
                    </tr>
                    <tr>
                        <td>Description:</td>
                        <td>{description}</td>
                    </tr>
                    <tr>
                        <td>Start Date/Time:</td>
                        <td>{startDate}</td>
                    </tr>
                    <tr>
                        <td>End Date/Time:</td>
                        <td>{endDate}</td>
                    </tr>
                    <tr>
                        <td>Recurrence:</td>
                        <td>{recurrencePattern}</td>
                    </tr>
                    <tr>
                        <td>Service Type:</td>
                        <td>{serviceType}</td>
                    </tr>
                    <tr>
                        <td>Service:</td>
                        <td>{serviceName}</td>
                    </tr>
                    <tr>
                        <td>Client:</td>
                        <td>{clientName}</td>
                    </tr>
                    <tr>
                        <td>Service Provider:</td>
                        <td>{serviceProviderName}</td>
                    </tr>
                </table>
            </div>

            <p><strong>Note:</strong> Individual tasks will be created based on this schedule. You will receive notifications as task statuses change.</p>

            <p>If you have any questions about this schedule, please contact our support team.</p>
            
            <p>Best regards,<br>eXtremeScheduler Team</p>
        </div>
        <div class=""footer"">
            <p>This is an automated email. Please do not reply to this message.</p>
            <p>&copy; {DateTime.Now.Year} eXtremeScheduler. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

            return new EmailMessage
            {
                To = recipientEmail,
                Subject = $"New Schedule Created - Schedule #{scheduleId}",
                Body = body,
                IsHtml = true
            };
        }
    }
}

