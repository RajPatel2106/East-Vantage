using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using East_Vantage.Models;

namespace East_Vantage.Controllers
{
    public class TechnicianController : ApiController
    {
        private EastContext db = new EastContext();

        [HttpGet]
        [Route("GetActiveTechnicians")]
        public List<tbl_Technician> GetActiveTechnicians()
        {
            return db.tbl_Technicians.Where(x => x.Active == true).ToList();
        }

        [HttpPost]
        [Route("AddTechnician")]
        [ResponseType(typeof(tbl_Technician))]
        public IHttpActionResult AddTechnician(tbl_Technician tbl_Technician)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(tbl_Technician.TechnicianId))
            {
                return BadRequest(message: "Please enter valid technician/reference id.");
            }
            else if (TechnicianExists(tbl_Technician.TechnicianId))
            {
                return BadRequest(message: "Technician is already exist.");
            }

            db.tbl_Technicians.Add(tbl_Technician);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TechnicianExists(tbl_Technician.TechnicianId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = tbl_Technician.TechnicianId }, tbl_Technician);
        }

        [HttpPut]
        [Route("DeactivateTechnician")]
        [ResponseType(typeof(void))]
        public IHttpActionResult DeactivateTechnician(string refId)
        {
            tbl_Technician tbl_Technician = db.tbl_Technicians.Find(refId);
            if (tbl_Technician == null)
            {
                return NotFound();
            }

            tbl_Technician.Active = false;
            db.Entry(tbl_Technician).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("GetTechnicianByRefId")]
        [ResponseType(typeof(tbl_Technician))]
        public IHttpActionResult GetTechnicianByRefId(string refId)
        {
            tbl_Technician tbl_Technician = db.tbl_Technicians.Find(refId);
            if (tbl_Technician == null)
            {
                return NotFound();
            }

            return Ok(tbl_Technician);
        }

        [HttpPut]
        [Route("EditTechnician")]
        [ResponseType(typeof(void))]
        public IHttpActionResult EditTechnician(tbl_Technician tbl_Technician)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var techData = db.tbl_Technicians.FirstOrDefault(x => x.TechnicianId.Trim().ToLower().Equals(tbl_Technician.TechnicianId.Trim().ToLower()));

            if (techData == null)
            {
                return BadRequest(message: "No technician has found.");
            }
            else if (techData.Active == false)
            {
                return BadRequest(message: "Technician is inactive.");
            }

            techData.TechnicianId = tbl_Technician.TechnicianId;
            techData.FirstName = tbl_Technician.FirstName;
            techData.LastName = tbl_Technician.LastName;
            techData.Active = tbl_Technician.Active;
            db.SaveChanges();

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TechnicianExists(tbl_Technician.TechnicianId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(tbl_Technician))]
        public IHttpActionResult Delete(string id)
        {
            tbl_Technician tbl_Technician = db.tbl_Technicians.Find(id);
            if (tbl_Technician == null)
            {
                return NotFound();
            }

            db.tbl_Technicians.Remove(tbl_Technician);
            db.SaveChanges();

            return Ok(tbl_Technician);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TechnicianExists(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }
            return db.tbl_Technicians.Count(e => e.TechnicianId.Trim().ToLower() == id.Trim().ToLower()) > 0;
        }
    }
}