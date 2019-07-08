using Business;
using Microsoft.AspNetCore.Hosting;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Infrastructure
{
    public class ExcelService : IExcelService
    {
        private readonly IEventBusiness _events;
        private readonly IEmailService _emailService;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IHostingEnvironment _env;
        private readonly IUserBusiness _userBusiness;

        public ExcelService(
            IEventBusiness events,
            IEmailService emailService,
            IRazorPartialToStringRenderer renderer,
            IHostingEnvironment env,
            IUserBusiness userBusiness)
        {
            _events = events;
            _emailService = emailService;
            _renderer = renderer;
            _env = env;
            _userBusiness = userBusiness;
        }

        public async Task GetEventSummaryInformation()
        {
            var eventFilePath = $@"{_env.WebRootPath}\Input\EventSummary.xlsx";
            var notAttendedFilePath = $@"{_env.WebRootPath}\Input\Volunteer_Enrollmentdetails_NotAttended.xlsx";
            var unregisteredFilePath = $@"{_env.WebRootPath}\Input\Volunteer_Enrollmentdetails_Unregistered.xlsx";
            var attendedFilePath = $@"{_env.WebRootPath}\Input\Volunteer_Enrollmentdetails_Attended.xlsx";

            if (File.Exists(eventFilePath) && File.Exists(notAttendedFilePath) && File.Exists(unregisteredFilePath) & File.Exists(attendedFilePath))
            {

                try
                {
                    //Create a new DataTable for Events.
                    var events = ExcelHelper.ToDataTable($@"{_env.WebRootPath}\Input\EventSummary.xlsx");

                    //Create a new DataTable for Not Attended.
                    var notAttended = ExcelHelper.ToDataTable($@"{_env.WebRootPath}\Input\Volunteer_Enrollmentdetails_NotAttended.xlsx");

                    //Create a new DataTable for Unregistered.
                    var unregistered = ExcelHelper.ToDataTable($@"{_env.WebRootPath}\Input\Volunteer_Enrollmentdetails_Unregistered.xlsx");

                    //Create a new DataTable for Attended.
                    var attended = ExcelHelper.ToDataTable($@"{_env.WebRootPath}\Input\Volunteer_Enrollmentdetails_Attended.xlsx");

                    var eventDetails = new List<Event>();

                    foreach (var x in events.AsEnumerable())
                    {
                        var eventDetail = new Event
                        {
                            Id = Guid.NewGuid(),
                            EventId = x["Event ID"].ToString(),
                            Month = x["Month"].ToString(),
                            BaseLocation = x["Base Location"].ToString(),
                            BeneficiaryName = x["Beneficiary Name"].ToString(),
                            VenueAddress = x["Venue Address"].ToString(),
                            CouncilName = x["Council Name"].ToString(),
                            Project = x["Project"].ToString(),
                            Category = x["Category"].ToString(),
                            EventName = x["Event Name"].ToString(),
                            EventDescription = x["Event Description"].ToString(),
                            EventDate = DateTime.ParseExact(x["Event Date (DD-MM-YY)"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                            TotalNoOfVolunteers = Convert.ToInt32(x["Total no. of volunteers"]),
                            TotalVolunteerHours = Convert.ToDecimal(x["Total Volunteer Hours"]),
                            TotalTravelHours = Convert.ToDecimal(x["Total Travel Hours"]),
                            OverallVolunteeringHours = Convert.ToDecimal(x["Overall Volunteering Hours"]),
                            LivesImpacted = Convert.ToInt32(x["Lives Impacted"]),
                            ActivityType = Convert.ToInt32(x["Activity Type"]),
                            BusinessUnit = x["Business Unit"].ToString(),
                            Status = x["Status"].ToString(),
                            Poc = new List<Poc>(),
                            Participant = new List<Participant>(),
                            CreatedAt = DateTime.Now,
                            ModifiedAt = DateTime.Now
                        };

                        var pocCount = x["POC ID"].ToString().Contains(';') ? x["POC ID"].ToString().Split(';').Count() : 1;

                        var poc = new List<Poc>();

                        for (var i = 0; i < pocCount; i++)
                        {
                            poc.Add(new Poc
                            {
                                Id = Guid.NewGuid(),
                                PocId = pocCount.Equals(1) ? Convert.ToInt64(x["POC ID"].ToString()) : Convert.ToInt64(x["POC ID"].ToString().Split(';')[i]),
                                PocName = pocCount.Equals(1) ? x["POC Name"].ToString() : x["POC Name"].ToString().Split(';')[i],
                                PocContactNumber = pocCount.Equals(1) ? x["POC Contact Number"].ToString() : x["POC Contact Number"].ToString().Split(';')[i],
                                CreatedAt = DateTime.Now,
                                ModifiedAt = DateTime.Now
                            });

                            var user = new RegisterForm
                            {
                                Email = pocCount.Equals(1) ? $"{Convert.ToInt64(x["POC ID"].ToString())}@cognizant.com" : $"{Convert.ToInt64(x["POC ID"].ToString().Split(';')[i])}@cognizant.com",
                                FirstName = pocCount.Equals(1) ? x["POC Name"].ToString() : x["POC Name"].ToString().Split(';')[i],
                                LastName = pocCount.Equals(1) ? x["POC Name"].ToString() : x["POC Name"].ToString().Split(';')[i],
                                Password = "Poc@123"
                            };

                            var (succeeded, errorMessage) = await _userBusiness.CreateUserAsync(user);

                            // Put the user in the poc role
                            if (succeeded)
                            {
                                await _userBusiness.AddToRoleAsync(Guid.Parse(errorMessage), "Poc");
                            }
                        }

                        var participants = new List<Participant>();

                        foreach (var participant in notAttended.AsEnumerable().Where(y => y["Event ID"].ToString().Equals(eventDetail.EventId)))
                        {
                            var guid = Guid.NewGuid();

                            participants.Add(
                                new Participant
                                {
                                    Id = guid,
                                    EmployeeId = participant["EmployeeID"].ToString(),
                                    EmployeeName = participant["EmployeeID"].ToString(),
                                    EventId = participant["Event ID"].ToString(),
                                    BeneficiaryName = participant["Beneficiary Name"].ToString(),
                                    Location = participant["Base Location"].ToString(),
                                    NotAttended = true,
                                    CreatedAt = DateTime.Now,
                                    ModifiedAt = DateTime.Now,
                                    IsEmailSent = true,
                                    MailSentAt = DateTime.Now
                                });
                        }

                        foreach (var participant in unregistered.AsEnumerable().Where(y => y["Event ID"].ToString().Equals(eventDetail.EventId)))
                        {
                            var guid = Guid.NewGuid();

                            participants.Add(
                                new Participant
                                {
                                    Id = guid,
                                    EmployeeId = participant["EmployeeID"].ToString(),
                                    EmployeeName = participant["EmployeeID"].ToString(),
                                    EventId = participant["Event ID"].ToString(),
                                    BeneficiaryName = participant["Beneficiary Name"].ToString(),
                                    Location = participant["Base Location"].ToString(),
                                    Unregistered = true,
                                    CreatedAt = DateTime.Now,
                                    ModifiedAt = DateTime.Now,
                                    IsEmailSent = true,
                                    MailSentAt = DateTime.Now
                                });
                        }

                        foreach (var participant in attended.AsEnumerable().Where(y => y["Event ID"].ToString().Equals(eventDetail.EventId)))
                        {
                            var guid = Guid.NewGuid();

                            participants.Add(
                                new Participant
                                {
                                    Id = guid,
                                    EmployeeId = participant["EmployeeID"].ToString(),
                                    EmployeeName = participant["EmployeeID"].ToString(),
                                    EventId = participant["Event ID"].ToString(),
                                    BeneficiaryName = participant["Beneficiary Name"].ToString(),
                                    Location = participant["Base Location"].ToString(),
                                    Attended = true,
                                    CreatedAt = DateTime.Now,
                                    ModifiedAt = DateTime.Now,
                                    IsEmailSent = true,
                                    MailSentAt = DateTime.Now
                                });

                        }


                        eventDetail.Poc = eventDetail.Poc.Concat(poc);
                        eventDetail.Participant = eventDetail.Participant.Concat(participants);
                        eventDetails.Add(eventDetail);
                    }

                    var addEventsSuccess = await _events.AddAsync(eventDetails);
                    if (addEventsSuccess)
                    {
                        var filePaths = new List<string>();
                        var unProcessedEventFilePath = $@"{_env.WebRootPath}\Input\EventSummary.xlsx";
                        filePaths.Add(unProcessedEventFilePath);
                        var unProcesseNotAttendedFilePath = $@"{_env.WebRootPath}\Input\Volunteer_Enrollmentdetails_NotAttended.xlsx";
                        filePaths.Add(unProcesseNotAttendedFilePath);
                        var unProcessedUnregisteredFilePath = $@"{_env.WebRootPath}\Input\Volunteer_Enrollmentdetails_Unregistered.xlsx";
                        filePaths.Add(unProcessedUnregisteredFilePath);
                        var unProcessedAttendedFilePath = $@"{_env.WebRootPath}\Input\Volunteer_Enrollmentdetails_Attended.xlsx";
                        filePaths.Add(unProcessedAttendedFilePath);
                        foreach (var filePath in filePaths)
                        {
                            MoveExcel(filePath);
                        }
                    }

                }
                catch (Exception e)
                {
                    var strLogText = "";

                    strLogText += "Message ---\n{0}" + e.Message;
                    strLogText += Environment.NewLine + "Source ---\n{0}" + e.Source;
                    strLogText += Environment.NewLine + "StackTrace ---\n{0}" + e.StackTrace;
                    strLogText += Environment.NewLine + "TargetSite ---\n{0}" + e.TargetSite;
                    if (e.InnerException != null)
                    {
                        strLogText += Environment.NewLine + "Inner Exception is {0}" + e.InnerException;
                        //error prone
                    }
                    if (e.HelpLink != null)
                    {
                        strLogText += Environment.NewLine + "HelpLink ---\n{0}" + e.HelpLink;//error prone
                    }

                    StreamWriter log;

                    var timestamp = DateTime.Now.ToString("d-MMMM-yyyy", new CultureInfo("en-GB"));

                    var errorFolder = Path.Combine(_env.WebRootPath, "ErrorLog");

                    if (!Directory.Exists(errorFolder))
                    {
                        Directory.CreateDirectory(errorFolder);
                    }

                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (!File.Exists($@"{errorFolder}\Log_{timestamp}.txt"))
                    {
                        log = new StreamWriter($@"{errorFolder}\Log_{timestamp}.txt");
                    }
                    else
                    {
                        log = File.AppendText($@"{errorFolder}\Log_{timestamp}.txt");
                    }

                    var controllerName = "Hangfire";
                    var actionName = "Excel Service";

                    // Write to the file:
                    log.WriteLine(Environment.NewLine + DateTime.Now);
                    log.WriteLine("------------------------------------------------------------------------------------------------");
                    log.WriteLine("Controller Name :- " + controllerName);
                    log.WriteLine("Action Method Name :- " + actionName);
                    log.WriteLine("------------------------------------------------------------------------------------------------");
                    log.WriteLine(strLogText);
                    log.WriteLine();

                    // Close the stream:
                    log.Close();
                }
            }
        }
        private void MoveExcel(string unProcessedFileNamePath)
        {
            var newFileName = Path.GetFileNameWithoutExtension(unProcessedFileNamePath);
            var newFolderPath = $@"{_env.WebRootPath}\Processed\{DateTime.Now.ToString("MM_dd_yyyy")}";
            if (!Directory.Exists(newFolderPath))
                Directory.CreateDirectory(newFolderPath);
            var processedEventFilePath = $@"{newFolderPath}\{newFileName}_{DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss")}.xlsx";
            File.Move(unProcessedFileNamePath, processedEventFilePath);
        }
    }
}
