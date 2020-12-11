using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zad3.Helpers;
using Zad3.Models;
using Zad3.Services;

namespace Zad3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IStudentDbService service;

        public EnrollmentsController(IStudentDbService dbService)
        {
            service = dbService;
        }

        [HttpPost]
        public IActionResult AddStudent(Student student)
        {
            MyHelper helper = service.AddStudent(student);
            if (helper.Value != 0)
            {
                return BadRequest(helper.Message);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.Created, helper.enrollment);
            }
        }
        
        [HttpPost("promotions")]
        public IActionResult promote(Study study)
        {
            MyHelper helper = service.Promote(study);
            if (helper.Value != 0)
            {
                return BadRequest(helper.Message);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.Created, helper.enrollment);
            }
        }
        
    }
}
