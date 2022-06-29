using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Description;
using East_Vantage.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using NLog;

namespace East_Vantage.Controllers
{
    public class WorkOrderController : ApiController
    {
        #region DB context and Logging
        private EastContext db = new EastContext();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region GET Methods

        //Get all work order
        [HttpGet]
        [Route("GetWorkOrders")]
        public List<tbl_WorkOrder> GetWorkOrders()
        {
            try
            {
                return db.tbl_WorkOrders.ToList();
            }
            catch (Exception ex)
            {
                logger.Error($"WorkOrderController_GetWorkOrders : An exception has been generated {ex}");
                return new List<tbl_WorkOrder>();
            }
        }

        //Get work orders by date as input
        [HttpGet]
        [Route("GetWorkOrdersByDate")]
        public List<tbl_WorkOrder> GetWorkOrdersByDate(DateTime date)
        {
            try
            {
                return db.tbl_WorkOrders.Where(x => x.WorkOrderDate == date)?.ToList();
            }
            catch (Exception ex)
            {
                logger.Error($"WorkOrderController_GetWorkOrdersByDate : An exception has been generated {ex} for date :" + date);
                return new List<tbl_WorkOrder>();
            }
        }

        //Get work orders by reference id
        [HttpGet]
        [Route("GetWorkOrdersByRefId")]
        public List<tbl_WorkOrder> GetWorkOrdersByRefId(string refId)
        {
            try
            {
                return db.tbl_WorkOrders.Where(x => x.TechnicianId == refId)?.ToList();
            }
            catch (Exception ex)
            {
                logger.Error($"WorkOrderController_GetWorkOrdersByRefId : An exception has been generated {ex} for refId : " + refId);
                return new List<tbl_WorkOrder>();
            }
        }

        //Get work order by work order id
        [HttpGet]
        [Route("GetWorkOrderById")]
        [ResponseType(typeof(tbl_WorkOrder))]
        public IHttpActionResult GetWorkOrderById(int id)
        {
            try
            {
                tbl_WorkOrder tbl_WorkOrder = db.tbl_WorkOrders.Find(id);
                if (tbl_WorkOrder == null)
                {
                    return NotFound();
                }

                return Ok(tbl_WorkOrder);
            }
            catch (Exception ex)
            {
                logger.Error($"WorkOrderController_GetWorkOrderById : An exception has been generated {ex} for id : " + id);
                return Ok(new tbl_WorkOrder());
            }
        }
        #endregion

        #region POST Method

        //Add work order
        [HttpPost]
        [Route("AddWorkOrder")]
        [ResponseType(typeof(tbl_WorkOrder))]
        public IHttpActionResult AddWorkOrder(tbl_WorkOrder tbl_WorkOrder)
        {
            try
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
                return Ok(tbl_WorkOrder);
            }
            catch (Exception ex)
            {
                logger.Error($"WorkOrderController_AddWorkOrder : An exception has been generated {ex} for model :" + JsonConvert.SerializeObject(tbl_WorkOrder));
                return BadRequest(message: "An error has occured");
            }
        }

        #endregion

        #region PUT Methods

        //Assign technician to the work order using work order id and technician id
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
            catch (DbUpdateConcurrencyException ex)
            {
                if (!WorkOrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    logger.Error($"WorkOrderController_AssignTechnician : An exception has been generated : {ex}");
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
            catch (DbUpdateConcurrencyException ex)
            {
                if (!WorkOrderExists(tbl_WorkOrder.WorkOrderId))
                {
                    return NotFound();
                }
                else
                {
                    logger.Error($"WorkOrderController_EditWorkOrder : An exception has been generated : {ex}");
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
        #endregion

        #region DELETE Method
        [ResponseType(typeof(tbl_WorkOrder))]
        public IHttpActionResult DeleteWorkOrder(int id)
        {
            try
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
            catch (Exception ex)
            {
                logger.Error($"WorkOrderController_DeleteWorkOrder: An exception has been generated : {ex} for id : {id}");
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

        private bool WorkOrderExists(int id)
        {
            try
            {
                return db.tbl_WorkOrders.Count(e => e.WorkOrderId == id) > 0;
            }
            catch (Exception ex)
            {
                logger.Error($"WorkOrderController_WorkOrderExists: An exception has been generated : {ex} for id : {id}");
                return false;
            }
        }
    }
}