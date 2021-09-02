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
using MakeSale.Models;

namespace MakeSale.Controllers
{
    public class PaymentMethodsController : ApiController
    {
        private DBS_POSableEntities db = new DBS_POSableEntities();

        [Route("api/GetPaymentMethods")] //gets list of categories
        [HttpGet]
        public List<PaymentMethod> GetPaymentMethods()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<PaymentMethod> paymentMethods = db.PaymentMethods.ToList();
            return paymentMethods;

        }
        // GET: api/PaymentMethods/5
        [ResponseType(typeof(PaymentMethod))]
        public IHttpActionResult GetPaymentMethod(int id)
        {
            PaymentMethod paymentMethod = db.PaymentMethods.Find(id);
            if (paymentMethod == null)
            {
                return NotFound();
            }

            return Ok(paymentMethod);
        }

        // PUT: api/PaymentMethods/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPaymentMethod(int id, PaymentMethod paymentMethod)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != paymentMethod.PaymentMethod_ID)
            {
                return BadRequest();
            }

            db.Entry(paymentMethod).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentMethodExists(id))
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

        // POST: api/PaymentMethods
        [ResponseType(typeof(PaymentMethod))]
        public IHttpActionResult PostPaymentMethod(PaymentMethod paymentMethod)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PaymentMethods.Add(paymentMethod);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = paymentMethod.PaymentMethod_ID }, paymentMethod);
        }

        // DELETE: api/PaymentMethods/5
        [ResponseType(typeof(PaymentMethod))]
        public IHttpActionResult DeletePaymentMethod(int id)
        {
            PaymentMethod paymentMethod = db.PaymentMethods.Find(id);
            if (paymentMethod == null)
            {
                return NotFound();
            }

            db.PaymentMethods.Remove(paymentMethod);
            db.SaveChanges();

            return Ok(paymentMethod);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PaymentMethodExists(int id)
        {
            return db.PaymentMethods.Count(e => e.PaymentMethod_ID == id) > 0;
        }
    }
}