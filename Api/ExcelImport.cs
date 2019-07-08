using Api.Infrastructure;
using Business;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    //Test
    public class ExcelImport
    {
        private readonly EventBusiness _events;
        private readonly IEmailService _emailService;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly UserBusiness _userBusiness;

        public ExcelImport(EventBusiness events, IEmailService emailService, IRazorPartialToStringRenderer renderer, UserBusiness userBusiness)
        {
            _events = events;
            _emailService = emailService;
            _renderer = renderer;
            _userBusiness = userBusiness;
        }

        public async Task GetEventSummaryInformation(string[] filesPath)
        {
            //Create a new DataTable for Events.
            var events = ExcelHelper.ToDataTable(filesPath[0]);

            //Create a new DataTable for Not Attended.
            var notAttended = ExcelHelper.ToDataTable(filesPath[1]);

            //Create a new DataTable for Unregistered.
            var unregistered = ExcelHelper.ToDataTable(filesPath[2]);

            //Create a new DataTable for Attended.
            var attended = ExcelHelper.ToDataTable(filesPath[3]);

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

                    var created = await _userBusiness.CreateUserAsync(user);

                    // Put the user in the poc role
                    if (created.Succeeded)
                    {
                        await _userBusiness.AddToRoleAsync(Guid.Parse(created.ErrorMessage), "Poc");
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

                    //var emailModel = new FeedbackRequestEmailTemplateModel
                    //{
                    //    EventName = eventDetail.EventName,
                    //    EventDate = eventDetail.EventDate,
                    //    ParticipantName = participant["EmployeeID"].ToString(),
                    //    FeedbackUrl = $"http://localhost:4200/feedback/notparticipated/{eventDetail.Id}/{guid}"
                    //};

                    //var body = await _renderer.RenderPartialToStringAsync("_FeedbackRequestEmailPartial", emailModel);

                    //await _emailService.SendAsync("outreachadmin@cognizant.com", "admin", participant["EmployeeID"].ToString(), $"Feedback Requested for {eventDetail.EventName} at {eventDetail.EventDate}", body);
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

                    //var emailModel = new FeedbackRequestEmailTemplateModel
                    //{
                    //    EventName = eventDetail.EventName,
                    //    EventDate = eventDetail.EventDate,
                    //    ParticipantName = participant["EmployeeID"].ToString(),
                    //    FeedbackUrl = $"http://localhost:4200/feedback/unregistered/{eventDetail.Id}/{guid}"
                    //};

                    //var body = await _renderer.RenderPartialToStringAsync("_FeedbackRequestEmailPartial", emailModel);

                    //await _emailService.SendAsync("outreachadmin@cognizant.com", "admin", participant["EmployeeID"].ToString(), $"Feedback Requested for {eventDetail.EventName} at {eventDetail.EventDate}", body);
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

                    //var emailModel = new FeedbackRequestEmailTemplateModel
                    //{
                    //    EventName = eventDetail.EventName,
                    //    EventDate = eventDetail.EventDate,
                    //    ParticipantName = participant["EmployeeID"].ToString(),
                    //    FeedbackUrl = $"http://localhost:4200/feedback/participated/{eventDetail.Id}/{guid}"
                    //};

                    //var body = await _renderer.RenderPartialToStringAsync("_FeedbackRequestEmailPartial", emailModel);

                    //await _emailService.SendAsync("outreachadmin@cognizant.com", "admin", participant["EmployeeID"].ToString(), $"Feedback Requested for {eventDetail.EventName} at {eventDetail.EventDate}", body);
                }


                eventDetail.Poc = eventDetail.Poc.Concat(poc);
                eventDetail.Participant = eventDetail.Participant.Concat(participants);
                eventDetails.Add(eventDetail);
            }

            await _events.AddAsync(eventDetails);
        }
    }
}
