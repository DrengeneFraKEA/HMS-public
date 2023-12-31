﻿using HMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JournalController : ControllerBase
    {
        private readonly JournalService journalService;
        public JournalController(JournalService service)
        {
            journalService = service;
        }

        [Authorize]
        [HttpGet("doctor/{doctorId}")]
        public string Journal(int doctorId) 
        {
            return JsonSerializer.Serialize(journalService.GetJournals(doctorId));
        }

        [Authorize]
        [HttpGet("text/{journaltext}/cpr/{cpr}/doctor/{doctorid}")]
        public bool CreateJournal(string journaltext, string cpr, string doctorid)
        {
            return journalService.CreateJournal(journaltext, cpr, doctorid, out _);
        }

        [Authorize]
        [HttpDelete("{journalid}")]
        public bool DeleteJournal(string journalid) 
        {
            return journalService.DeleteJournal(journalid);
        }

        [Authorize]
        [HttpGet("journalid/{journalid}/newjournaltext/{newjournaltext}")]
        public bool UpdateJournal(string journalid, string newjournaltext) 
        {
            return journalService.UpdateJournal(journalid, newjournaltext);
        }
    }
}
