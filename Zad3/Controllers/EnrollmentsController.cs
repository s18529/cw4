using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zad3.Models;

namespace Zad3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private string connectionString = "Data Source=db-mssql;Initial Catalog=s18529;Integrated Security=True";
        [HttpPost]
        public IActionResult AddStudent(Student student)
        {
            if(student.IndexNumber.Equals(null) || student.FirstName.Equals(null) || student.LastName.Equals(null) || student.Bdate==null || student.Studies.Equals(null))
            {
                return BadRequest();
            }
            using(var client = new SqlConnection(connectionString))
                using(var command = new SqlCommand())
            {
                command.Connection = client;
                client.Open();
                var transaction = client.BeginTransaction();
                command.Transaction = transaction;
                command.CommandText = "select IndexNumber from Student";
                var dr = command.ExecuteReader();
                while (dr.Read())
                {
                    if (dr[0].Equals(student.IndexNumber))
                    {
                        dr.Close();
                        transaction.Rollback();
                        return BadRequest("niepoprawny index studenta");
                    }
                }
                dr.Close();
                int idStudy;
                command.CommandText = "select idStudy from Studies where name = @name";
                command.Parameters.AddWithValue("name", student.Studies);
                dr = command.ExecuteReader();
                if (!dr.Read())
                {
                    dr.Close();
                    transaction.Rollback();
                    return BadRequest("Niepoprawne studia");
                }
                else
                {
                    idStudy = int.Parse(dr[0].ToString());
                }
                dr.Close();
                command.CommandText = "select * from enrollment where semester=1 and idStudy = @idStudy";
                command.Parameters.AddWithValue("idStudy", idStudy);
                
                dr = command.ExecuteReader();
                int id=0;
                Enrollment enrollment = new Enrollment();
                if (!dr.Read())
                {
                    command.CommandText = "select max(idEnrollment)+1 from Enrollment";
                    dr.Close();
                    dr = command.ExecuteReader();
                    dr.Read();
                    id = int.Parse(dr[0].ToString());
                    dr.Close();
                    command.CommandText = "insert into Enrollment values (@id, 1, @idStudy, @date)";
                    command.Parameters.AddWithValue("date", DateTime.Now);
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteNonQuery();
                    enrollment.IdEnrollment = id;
                    enrollment.IdStudy = idStudy;
                    enrollment.StartDate = DateTime.Now;
                    enrollment.Semester = 1;
                }
                else
                {
                    id = int.Parse(dr[0].ToString());
                    enrollment.IdEnrollment = id;
                    enrollment.IdStudy = int.Parse(dr[2].ToString());
                    enrollment.StartDate = DateTime.Parse(dr[3].ToString());
                    enrollment.Semester = int.Parse(dr[1].ToString());
                    command.Parameters.AddWithValue("id", id);
                }
                dr.Close();
                command.CommandText = "insert into student values (@Inumber, @Fname, @Lname, @Bdate, @id)";
                command.Parameters.AddWithValue("Inumber", student.IndexNumber);
                command.Parameters.AddWithValue("Fname", student.FirstName);
                command.Parameters.AddWithValue("Lname", student.LastName);
                command.Parameters.AddWithValue("Bdate", student.Bdate);
                command.ExecuteNonQuery();
                transaction.Commit();
                return StatusCode((int)HttpStatusCode.Created, enrollment);

            }
        }
    }
}
