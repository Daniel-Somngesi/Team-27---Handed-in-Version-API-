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
    public class ClientsController : ApiController
    {
        private DBS_POSableEntities db = new DBS_POSableEntities();

        //POST (CreateClient)
        [Route("api/Client/PostClient")]
        [HttpPost]
        public void PostCountry(Client client)
        {
            db.Configuration.ProxyCreationEnabled = false;
            try
            {                
                db.Clients.Add(client);
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.Message);
            }

            db.SaveChanges();
        }

        //GET (ReadClient)
        [Route("api/Client/{id}")]
        [HttpGet]
        public Client GetClient(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            Client client = new Client();
            try
            {
                client = db.Clients.Where(zz => zz.Client_ID == id).FirstOrDefault();
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.Message);
            }
            return client;

        }

        //PUT (updateClient)
        [Route("api/Client/{id}")]
        [HttpPut]
        public void PutClient(int id, Client client)
        {
            db.Configuration.ProxyCreationEnabled = false;

            try
            {
                Client clientToUpdate = db.Clients.Where(zz => zz.Client_ID == id).FirstOrDefault();
                if (clientToUpdate != null)
                {
                    clientToUpdate.Client_Name = client.Client_Name;
                    clientToUpdate.Client_Surname = client.Client_Surname;
                    clientToUpdate.Client_Email = client.Client_Email;

                }
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.Message);
            }

            db.SaveChanges();
        }
    }
}