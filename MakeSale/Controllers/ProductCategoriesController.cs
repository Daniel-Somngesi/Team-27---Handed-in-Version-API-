using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MakeSale.Models;

namespace MakeSale.Controllers
{
    public class ProductCategoriesController : ApiController
    {
        private DBS_POSableEntities db = new DBS_POSableEntities();

        //POST (CreateProductCategory)
        [Route("api/ProductCategories/PostProductCategory")]
        [HttpPost]
        public void PostProductCategory(ProductCategory productCategory)
        {
            db.Configuration.ProxyCreationEnabled = false;
            try
            {
                db.ProductCategories.Add(productCategory);
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.Message);
            }

            db.SaveChanges();
        }

        //GET (ReadProductCategory)
        [Route("api/ProductCategory/{id}")]
        [HttpGet]
        public ProductCategory GetProductCategory(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            ProductCategory productCategory = new ProductCategory();
            try
            {
                productCategory = db.ProductCategories.Where(zz => zz.ProductCategory_ID == id).FirstOrDefault();
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.Message);
            }
            return productCategory;

        }

        //DELETE (DeleteProductCategory)
        [Route("api/ProductCategories/{id}")]
        [HttpDelete]
        public void DeleteProductCategory(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            try
            {
                ProductCategory productCategory = db.ProductCategories.Find(id);
                db.ProductCategories.Remove(productCategory);
            }
            catch (Exception)
            {
                throw;
            }

            db.SaveChanges();

        }

        //gets list of categories
        [Route("GetCategories")] 
        [HttpGet]
        public List<ProductCategory> GetCategories()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<ProductCategory> categories = db.ProductCategories.ToList();
            return categories;
        }


        //gets list of Products for a specific Category
        [Route("GetItems")]  
        [HttpGet]
        public List<dynamic> GetItems(int CategoryID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<C_Product_Size> items = db.C_Product_Size.Include(zz => zz.C_Price).Include(zz => zz.C_Size).Include(zz => zz.Product).Where(zz => zz.Product.ProductCategory_ID == CategoryID).ToList();
            return getAllItems(items);
        }

        private List<dynamic> getAllItems(List<C_Product_Size> items) 
        {
            List<dynamic> ItemsList = new List<dynamic>();
            foreach (C_Product_Size item in items)
            {
                dynamic getSize = new ExpandoObject();
                getSize._Product_Size_ID = item.C_Product_Size_ID;
                getSize.Product_ID = item.Product_ID;
                getSize.Size_ID = item.C_Size_ID;
                getSize.Price = db.C_Price.Where(zz => zz.C_Product_Size_ID == item.C_Product_Size_ID).OrderByDescending(zz => zz.C_Date).Select(zz => zz.Price).FirstOrDefault();
                getSize.Size = db.C_Size.Where(zz => zz.C_Size_ID == item.C_Size_ID).Select(zz => zz.Size).FirstOrDefault();
                getSize.QuantityOnHand = item.QuantityOnHand;
                getSize.Barcode = item.Barcode;


                dynamic getProduct = new ExpandoObject();
                getProduct.Product_ID = item.Product_ID;
                getProduct.ProductCategory_ID = item.Product.ProductCategory_ID;
                getProduct.ProductType_Id = item.Product.ProductType_ID;
                getProduct.ProductName = item.Product.ProductName;


                getSize.Product = getProduct;


                ItemsList.Add(getSize);

            }

            return ItemsList;
        }
    }
}