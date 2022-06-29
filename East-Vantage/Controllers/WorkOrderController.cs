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
    public class WorkOrderController : ApiController
    {
        private EastContext db = new EastContext();

        [HttpGet]
        [Route("GetWorkOrders")]
        public List<tbl_WorkOrder> GetWorkOrders()
        {
            return db.tbl_WorkOrders.ToList();
        }

        [HttpGet]
        [Route("GetWorkOrdersByDate")]
        public List<tbl_WorkOrder> GetWorkOrdersByDate(DateTime date)
        {
            return db.tbl_WorkOrders.Where(x => x.WorkOrderDate == date)?.ToList();
        }

        [HttpGet]
        [Route("GetWorkOrdersByRefId")]
        public List<tbl_WorkOrder> GetWorkOrdersByRefId(string refId)
        {
            return db.tbl_WorkOrders.Where(x => x.TechnicianId == refId)?.ToList();
        }

        [HttpPost]
        [Route("AddWorkOrder")]
        [ResponseType(typeof(tbl_WorkOrder))]
        public IHttpActionResult AddWorkOrder(tbl_WorkOrder tbl_WorkOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var techData = db.tbl_Technicians.FirstOrDefault(e => e.TechnicianId.Trim().Equals(tbl_WorkOrder.TechnicianId.Trim(), StringComparison.OrdinalIgnoreCase));
            if (techData == null)
            {
                return BadRequest(message: "Technician is not exist.");
            }
            else if (techData != null && techData.Active == false)
            {
                return BadRequest(message: "Technician is inactive.");
            } 

            db.tbl_WorkOrders.Add(tbl_WorkOrder);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = tbl_WorkOrder.WorkOrderId }, tbl_WorkOrder);
        }

        [ResponseType(typeof(tbl_WorkOrder))]
        public IHttpActionResult DeleteWorkOrder(int id)
        {
            tbl_WorkOrder tbl_WorkOrder = db.tbl_WorkOrders.Find(id);
            if (tbl_WorkOrder == null)
            {
                return NotFound();
            }

            db.tbl_WorkOrders.Remove(tbl_WorkOrder);
            db.SaveChanges();

            return Ok(tbl_WorkOrder);
        }

        [HttpPut]
        [Route("AssignTechnician")]
        [ResponseType(typeof(void))]
        public IHttpActionResult AssignTechnician(int id, string refId)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(refId))
            {
                return BadRequest(message: "Please add valid data.");
            }

            var techData = db.tbl_Technicians.FirstOrDefault(x => x.TechnicianId.Trim().ToLower().Equals(refId.Trim().ToLower()));
            if (techData == null)
            {
                return BadRequest(message: "Technician does not exist.");
            }
            else if (techData.Active == false)
            {
                return BadRequest(message: "Technician is inactive.");
            }

            var workOrderData = db.tbl_WorkOrders.FirstOrDefault(x => x.WorkOrderId == id);
            if (workOrderData == null)
            {
                return BadRequest(message: "There is no work order contains this id.");
            }

            workOrderData.TechnicianId = refId;
            db.Entry(workOrderData).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkOrderExists(id))
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

        [HttpPut]
        [Route("EditWorkOrder")]
        [ResponseType(typeof(void))]
        public IHttpActionResult EditWorkOrder(tbl_WorkOrder tbl_WorkOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var techData = db.tbl_Technicians.FirstOrDefault(x => x.TechnicianId.Trim().ToLower().Equals(tbl_WorkOrder.TechnicianId.Trim().ToLower()));
            if (techData == null)
            {
                return BadRequest(message: "Technician does not exist.");
            }
            else if (techData.Active == false)
            {
                return BadRequest(message: "Technician is inactive.");
            }

            var workOrderData = db.tbl_WorkOrders.FirstOrDefault(x => x.WorkOrderId == tbl_WorkOrder.WorkOrderId);
            if (workOrderData == null)
            {
                return BadRequest(message: "There is no work order contains this id.");
            }

            workOrderData.Address = tbl_WorkOrder.Address;
            workOrderData.WorkOrderDate = tbl_WorkOrder.WorkOrderDate;
            workOrderData.TechnicianId = tbl_WorkOrder.TechnicianId;
            db.SaveChanges();

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkOrderExists(tbl_WorkOrder.WorkOrderId))
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

        [HttpGet]
        [Route("GetWorkOrderById")]
        [ResponseType(typeof(tbl_WorkOrder))]
        public IHttpActionResult GetWorkOrderById(int id)
        {
            tbl_WorkOrder tbl_WorkOrder = db.tbl_WorkOrders.Find(id);
            if (tbl_WorkOrder == null)
            {
                return NotFound();
            }

            return Ok(tbl_WorkOrder);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WorkOrderExists(int id)
        {
            return db.tbl_WorkOrders.Count(e => e.WorkOrderId == id) > 0;
        }
    }
}