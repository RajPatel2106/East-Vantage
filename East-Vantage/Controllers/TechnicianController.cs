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
using Newtonsoft.Json;
using NLog;

namespace East_Vantage.Controllers
{
    public class TechnicianController : ApiController
    {
        #region DB context and Logging
        private EastContext db = new EastContext();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region GET Methods

        //Get all active technicians
        [HttpGet]
        [Route("GetActiveTechnicians")]
        public List<tbl_Technician> GetActiveTechnicians()
        {
            try
            {
                return db.tbl_Technicians.Where(x => x.Active == true).ToList();
            }
            catch (Exception ex)
            {
                logger.Error($"TechnicianController_GetActiveTechnicians : An exception has been generated {ex}");
                return new List<tbl_Technician>();
            }
        }

        //Get technician by reference/technician id
        [HttpGet]
        [Route("GetTechnicianByRefId")]
        [ResponseType(typeof(tbl_Technician))]
        public IHttpActionResult GetTechnicianByRefId(string refId)
        {
            try
            {
                tbl_Technician tbl_Technician = db.tbl_Technicians.Find(refId);
                if (tbl_Technician == null)
                {
                    return NotFound();
                }

                return Ok(tbl_Technician);
            }
            catch (Exception ex)
            {
                logger.Error($"TechnicianController_GetTechnicianByRefId : An exception has been generated {ex} with id : {refId}");
                return BadRequest(message: "An error has occured.");
            }
        }

        #endregion

        #region POST Method

        //Add technician
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
            catch (DbUpdateException ex)
            {
                if (TechnicianExists(tbl_Technician.TechnicianId))
                {
                    return Conflict();
                }
                else
                {
                    logger.Error($"TechnicianController_AddTechnician : An exception has been generated {ex} for model : " + JsonConvert.SerializeObject(tbl_Technician));
                    return BadRequest(message: "An error has occured.");
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = tbl_Technician.TechnicianId }, tbl_Technician);
        }

        #endregion

        #region PUT Methods

        //Deactivate technician by reference/technician id
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
            catch (DbUpdateConcurrencyException ex)
            {
                logger.Error($"TechnicianController_DeactivateTechnician : An exception has been generated {ex} for refId : {refId}");
                return BadRequest(message: "An error has occured.");
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        //Edit technician 
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
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TechnicianExists(tbl_Technician.TechnicianId))
                {
                    return NotFound();
                }
                else
                {
                    logger.Error($"TechnicianController_EditTechnician : An exception has been generated {ex} for model : " + JsonConvert.SerializeObject(tbl_Technician));
                    return BadRequest(message: "An error has occured.");
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        #endregion

        #region DELETE Method

        //Delete technician by reference/technician id
        [ResponseType(typeof(tbl_Technician))]
        public IHttpActionResult Delete(string id)
        {
            try
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
            catch (Exception ex)
            {
                logger.Error($"TechnicianController_DeactivateTechnician : An exception has been generated {ex} for id : {id}");
                return BadRequest(message: "An error has occured.");
            }
        }

        #endregion

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