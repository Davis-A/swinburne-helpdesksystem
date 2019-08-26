﻿using System;
using System.Linq;
using System.Net;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests.Helpdesk;
using Helpdesk.Common.Responses.Helpdesk;
using Helpdesk.Common.Utilities;
using Helpdesk.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Helpdesk.Services.Test
{
    [TestClass]
    public class HelpdeskTests
    {
        /// <summary>
        /// Ensures that adding a helpdesk works
        /// </summary>
        [TestMethod]
        public void AddHelpdesk()
        {
            var request = new AddHelpdeskRequest()
            {
                HasCheckIn = false,
                HasQueue = true,
                Name = AlphaNumericStringGenerator.GetString(10)
            };

            var facade = new HelpdeskFacade();
            var response = facade.AddHelpdesk(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.HelpdeskID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var helpdesk = context.Helpdesksettings.FirstOrDefault(p => p.HelpdeskId == response.HelpdeskID);

                Assert.IsNotNull(helpdesk);
            }
        }

        /// <summary>
        /// Ensures that the validation requires a name and prevents a helpdesk being added
        /// </summary>
        [TestMethod]
        public void AddHelpdeskNoName()
        {
            var request = new AddHelpdeskRequest()
            {
                HasCheckIn = false,
                HasQueue = true
            };

            var facade = new HelpdeskFacade();
            var response = facade.AddHelpdesk(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }


        /// <summary>
        /// Ensures that updating the helpdesk works correctly
        /// </summary>
        [TestMethod]
        public void UpdateHelpdesk()
        {
            var request = new AddHelpdeskRequest()
            {
                HasCheckIn = false,
                HasQueue = true,
                Name = AlphaNumericStringGenerator.GetString(10)
            };

            var facade = new HelpdeskFacade();
            var response = facade.AddHelpdesk(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.HelpdeskID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var helpdesk = context.Helpdesksettings.FirstOrDefault(p => p.HelpdeskId == response.HelpdeskID);

                Assert.IsNotNull(helpdesk);
            }

            var updateRequest = new UpdateHelpdeskRequest()
            {
                HasCheckIn = true,
                HasQueue = true,
                Name = AlphaNumericStringGenerator.GetString(10)
            };

            var updateResponse = facade.UpdateHelpdesk(response.HelpdeskID, updateRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var helpdesk = context.Helpdesksettings.FirstOrDefault(p => p.HelpdeskId == response.HelpdeskID);

                Assert.IsNotNull(helpdesk);
                Assert.AreEqual(updateRequest.HasCheckIn, helpdesk.HasCheckIn);
                Assert.AreEqual(updateRequest.HasQueue, helpdesk.HasQueue);
                Assert.AreEqual(updateRequest.Name, helpdesk.Name);
            }
        }

        /// <summary>
        /// Used to ensure that updating a helpdesk that does not exist, errors in the correct way
        /// </summary>
        [TestMethod]
        public void UpdateNotFoundHelpdesk()
        {
            var facade = new HelpdeskFacade();
            var updateRequest = new UpdateHelpdeskRequest()
            {
                HasCheckIn = true,
                HasQueue = true,
                Name = AlphaNumericStringGenerator.GetString(10)
            };

            var updateResponse = facade.UpdateHelpdesk(-1, updateRequest);

            Assert.AreEqual(HttpStatusCode.NotFound, updateResponse.Status);
        }

        /// <summary>
        /// Tests adding a timespan to the database with a valid request.
        /// </summary>
        [TestMethod]
        public void AddTimespan()
        {
            AddHelpdeskRequest addHelpdeskRequest = new AddHelpdeskRequest
            {
                HasCheckIn = false,
                HasQueue = true,
                Name = AlphaNumericStringGenerator.GetString(10)
            };

            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();
            AddHelpdeskResponse addHelpdeskResponse = helpdeskFacade.AddHelpdesk(addHelpdeskRequest);

            Assert.AreEqual(HttpStatusCode.OK, addHelpdeskResponse.Status);

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest
            {
                HelpdeskId = addHelpdeskResponse.HelpdeskID,
                Name = AlphaNumericStringGenerator.GetString(10)
            };
            DateTime startDate = DateTime.Today;
            DateTime endDate = new DateTime(startDate.Year + 1, startDate.Month, startDate.Day, 0, 0, 0);
            addTimeSpanRequest.StartDate = startDate;
            addTimeSpanRequest.EndDate = endDate;

            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.OK, addTimeSpanResponse.Status);
        }

        /// <summary>
        /// Tests adding a timespan where the end date precedes the start date.
        /// </summary>
        [TestMethod]
        public void AddTimespanEndBeforeStart()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest
            {
                HelpdeskId = -1,
                Name = AlphaNumericStringGenerator.GetString(10)
            };
            DateTime startDate = DateTime.Today;
            DateTime endDate = new DateTime(startDate.Year - 1, startDate.Month, startDate.Day, 0, 0, 0);
            addTimeSpanRequest.StartDate = startDate;
            addTimeSpanRequest.EndDate = endDate;

            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, addTimeSpanResponse.Status);
        }

        /// <summary>
        /// Tests adding a timespan where the start date predates the year that this system became available (2019).
        /// </summary>
        [TestMethod]
        public void AddTimespanStartDatePredatesSystem()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest
            {
                HelpdeskId = -1,
                Name = AlphaNumericStringGenerator.GetString(10)
            };
            DateTime startDate = new DateTime(2018, 1, 1, 0, 0, 0);
            DateTime endDate = DateTime.Today;
            addTimeSpanRequest.StartDate = startDate;
            addTimeSpanRequest.EndDate = endDate;

            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, addTimeSpanResponse.Status);
        }

        /// <summary>
        /// Test updating a specific timespan's name, start date and end date
        /// </summary>
        [TestMethod]
        public void UpdateTimespanFound()
        {
            AddHelpdeskRequest addHelpdeskRequest = new AddHelpdeskRequest
            {
                HasCheckIn = false,
                HasQueue = true,
                Name = AlphaNumericStringGenerator.GetString(10)
            };

            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();
            AddHelpdeskResponse addHelpdeskResponse = helpdeskFacade.AddHelpdesk(addHelpdeskRequest);

            Assert.AreEqual(HttpStatusCode.OK, addHelpdeskResponse.Status);

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest
            {
                HelpdeskId = addHelpdeskResponse.HelpdeskID,
                Name = AlphaNumericStringGenerator.GetString(10)
            };
            DateTime startDate = DateTime.Today;
            DateTime endDate = new DateTime(startDate.Year + 1, startDate.Month, startDate.Day, 0, 0, 0);
            addTimeSpanRequest.StartDate = startDate;
            addTimeSpanRequest.EndDate = endDate;

            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.OK, addTimeSpanResponse.Status);

            UpdateTimeSpanRequest updateTimespanRequest = new UpdateTimeSpanRequest
            {
                Name = AlphaNumericStringGenerator.GetString(10),
                StartDate = new DateTime(2019, 01, 01),
                EndDate = new DateTime(2019, 06, 01),
            };

            UpdateTimeSpanResponse updateTimespanResponse = helpdeskFacade.UpdateTimeSpan(addTimeSpanResponse.SpanId, updateTimespanRequest);

            Assert.AreEqual(HttpStatusCode.OK, updateTimespanResponse.Status);
            Assert.IsTrue(updateTimespanResponse.result);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var timespan = context.Timespans.FirstOrDefault(u => u.SpanId == addTimeSpanResponse.SpanId);

                Assert.AreEqual(timespan.StartDate, updateTimespanRequest.StartDate);
                Assert.AreEqual(timespan.Name, updateTimespanRequest.Name);
                Assert.AreEqual(timespan.EndDate, updateTimespanRequest.EndDate);
            }
        }

        /// <summary>
        /// Test updating a timespan that doesn't exist is handled properly
        /// </summary>
        [TestMethod]
        public void UpdateTimespanNotFound()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            UpdateTimeSpanRequest updateTimespanRequest = new UpdateTimeSpanRequest
            {
                Name = AlphaNumericStringGenerator.GetString(10),
                StartDate = new DateTime(2019, 08, 01),
                EndDate = new DateTime(2019, 11, 01)
            };

            UpdateTimeSpanResponse updateTimespanResponse = helpdeskFacade.UpdateTimeSpan(0, updateTimespanRequest);

            Assert.AreEqual(HttpStatusCode.NotFound, updateTimespanResponse.Status);
        }

        /// <summary>
        /// Test getting every timespan from the database
        /// </summary>
        [TestMethod]
        public void GetTimespans()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            GetTimeSpansResponse getTimespansResponse = helpdeskFacade.GetTimeSpans();

            Assert.AreEqual(HttpStatusCode.OK, getTimespansResponse.Status);
            Assert.AreEqual(1, getTimespansResponse.Timespans[0].SpanId);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var timespans = context.Timespans.ToList();

                Assert.IsNotNull(timespans);
            }
        }

        /// <summary>
        /// Test getting a specific timespan from the database by their span id
        /// </summary>
        [TestMethod]
        public void GetTimespanFound()
        {
            AddHelpdeskRequest addHelpdeskRequest = new AddHelpdeskRequest
            {
                HasCheckIn = false,
                HasQueue = true,
                Name = AlphaNumericStringGenerator.GetString(10)
            };

            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();
            AddHelpdeskResponse addHelpdeskResponse = helpdeskFacade.AddHelpdesk(addHelpdeskRequest);

            Assert.AreEqual(HttpStatusCode.OK, addHelpdeskResponse.Status);

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest
            {
                HelpdeskId = addHelpdeskResponse.HelpdeskID,
                Name = "TestName"
            };
            DateTime startDate = DateTime.Today;
            DateTime endDate = new DateTime(startDate.Year + 1, startDate.Month, startDate.Day, 0, 0, 0);
            addTimeSpanRequest.StartDate = startDate;
            addTimeSpanRequest.EndDate = endDate;

            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.OK, addTimeSpanResponse.Status);

            GetTimeSpanResponse getTimespanResponse = helpdeskFacade.GetTimeSpan(addTimeSpanResponse.SpanId);

            Assert.AreEqual(HttpStatusCode.OK, getTimespanResponse.Status);
            Assert.AreEqual("TestName", getTimespanResponse.Timespan.Name);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var timespan = context.Timespans.FirstOrDefault(t => t.SpanId == addTimeSpanResponse.SpanId);

                Assert.IsNotNull(timespan);
                Assert.AreEqual(addHelpdeskResponse.HelpdeskID, timespan.HelpdeskId);
                Assert.AreEqual("TestName", timespan.Name);
            }
        }

        /// <summary>
        /// Test getting a timespan that doesn't exist is handled properly
        /// </summary>
        [TestMethod]
        public void GetTimespanNotFound()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            GetTimeSpanResponse getTimespanResponse = helpdeskFacade.GetTimeSpan(-1);

            Assert.AreEqual(HttpStatusCode.NotFound, getTimespanResponse.Status);
        }
    }
}