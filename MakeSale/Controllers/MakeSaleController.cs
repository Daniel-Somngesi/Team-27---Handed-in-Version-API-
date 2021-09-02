using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Net.Mail;
using System.Data.Entity;
using MakeSale.Models;


namespace MakeSale.Controllers
{
    
    [RoutePrefix("api/Sale")]
    public class MakeSaleController : ApiController
    {
        DBS_POSableEntities db = new DBS_POSableEntities();
        

        [Route("GetCategories")] //gets list of categories
        [HttpGet]
        public List<ProductCategory> GetCategories()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<ProductCategory> categories = db.ProductCategories.ToList();
            return categories;
        }

        [Route("GetItems")]  //gets list of Products for a specific Category
        [HttpGet]
        public List<dynamic> GetItems(int CategoryID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<C_Product_Size> items = db.C_Product_Size.Include(zz => zz.C_Price).Include(zz => zz.C_Size).Include(zz => zz.Product).Where(zz => zz.Product.ProductCategory_ID == CategoryID).ToList();
            return getAllItems(items);
        }

        private List<dynamic> getAllItems(List<C_Product_Size> items) //parsing method for GetItems  //puts everything into the specifc JSON format specified in vscode project under API Service/Sale/SaleData.ts, the JSON export class is ProductSizeData
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

        [Route("GetPrice")] 
        [HttpGet]
        public double GetPrice(int _Product_Size_ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return Convert.ToDouble(db.C_Price.Where(zz => zz.C_Product_Size_ID == _Product_Size_ID).OrderByDescending(zz => zz.C_Date).Select(zz => zz.Price).FirstOrDefault());
        }

        //[Route("GetSale")]
        //[HttpGet]
        //public List<Sale> GetSale(int SaleID)
        //{
        //    db.Configuration.ProxyCreationEnabled = false;
        //    List<Sale> salelist = db.Sales.Include(zz => zz.SaleLines).Where(zz => zz.Sale_ID).

        //}

        [Route("AddCartItem")]  //Method to add a product to the CartItem table
        [HttpPost]
        public dynamic AddCartItem(int ProdSizeID)
        {
            try
            {
                CartItem findItem = db.CartItems.Where(zz => zz.C_Product_Size_ID == ProdSizeID).FirstOrDefault();

                if (findItem == null)
                {
                    CartItem newItem = new CartItem();
                    newItem.C_Product_Size_ID = ProdSizeID;
                    newItem.Quantity = 1;
                    db.CartItems.Add(newItem);
                    db.SaveChanges();
                }
                else
                {
                    IncreaseQuantity(ProdSizeID);
                }

               
                return "success";
            }
            catch (Exception err)
            {
                return err;
            }
           
        }

        [Route("CancelSale")]  //
        [HttpDelete]
        public void CancelSale()
        {
           
                List<CartItem> getItems = db.CartItems.ToList();
                db.CartItems.RemoveRange(getItems);
                db.SaveChanges();             
           
        }

        [Route("IncreaseQuantity")]
        [HttpPost]
        public void IncreaseQuantity(int ProdSizeID)
        {
            CartItem updateItem = db.CartItems.Where(zz => zz.C_Product_Size_ID == ProdSizeID).FirstOrDefault();
            updateItem.Quantity++;
            db.SaveChanges(); 
        }

        [Route("DecreaseQuantity")]
        [HttpPost]
        public void DecreaseQuantity(int ProdSizeID)
        {
            CartItem updateItem = db.CartItems.Where(zz => zz.C_Product_Size_ID == ProdSizeID).FirstOrDefault();
            updateItem.Quantity--;
            db.SaveChanges();

            if (updateItem.Quantity == 0)
            {
                db.CartItems.Remove(updateItem);
                db.SaveChanges();
            }
            
        }

        [Route("GetCartItems")]
        [HttpGet]
        public List<dynamic> GetCartItems() //gets all CartItems and puts them in JSON Format specified in vscode project under API Service/Sale/SaleData.ts, the JSON export class is CartItemData
        {
            db.Configuration.ProxyCreationEnabled = false;

            List<dynamic> ItemsList = new List<dynamic>();
            List<CartItem> items = db.CartItems.Include(zz => zz.C_Product_Size).ToList(); //currently it just retrieves everything, but once logins are implemeted, it should only retrieve cart items related to the person logged in
                                                                                           // see line 261 for explanation
            foreach (CartItem item in items)
            {
                dynamic getItem = new ExpandoObject();
                getItem._Product_Size_ID = item.C_Product_Size_ID;
                getItem.Quantity = item.Quantity;
               

                dynamic ProdSize = new ExpandoObject();
                ProdSize.ProductName = db.Products.Where(zz => zz.Product_ID == item.C_Product_Size.Product_ID).Select(zz => zz.ProductName).FirstOrDefault();
                ProdSize.Price = db.C_Price.Where(zz => zz.C_Product_Size_ID == item.C_Product_Size_ID).OrderByDescending(zz => zz.C_Date).Select(zz => zz.Price).FirstOrDefault();
                ProdSize.Size = db.C_Size.Where(zz => zz.C_Size_ID == item.C_Product_Size.C_Size_ID).Select(zz => zz.Size).FirstOrDefault();
                ProdSize.Measurement = db.Measurements.Where(zz => zz.MeasurementID == item.C_Product_Size.MeasurementID).Select(zz => zz.Measurement1).FirstOrDefault();
                ProdSize.QuantityOnHand = item.Quantity;

                getItem._Product_Size = ProdSize;

                ItemsList.Add(getItem);

            }

            return ItemsList;
        }

        private readonly Random randomGen = new Random();

        [Route("MakeCashSale")] //this is when you click Pay then Cash, it saves data to Payment table, then Sale table, then Sale line table in that order
        [HttpPost]
        public dynamic MakeCashSale(float CashAmount)
        {
            try
            {
                string receiptTemplate = "this is the template";  //when you have a receipt template it can be used to generate that receipt

                Payment makePayment = new Payment();
                makePayment.Payment_Date = DateTime.Now.Date;
                makePayment.Payment_Amount = CashAmount;
                makePayment.Payment_Receipt = receiptTemplate;
                makePayment.Payment_Type = "In Store";
                makePayment.PaymentMethod_ID = 1;           //depends on data in the database

                db.Payments.Add(makePayment);
                db.SaveChanges();

                int PaymentID = makePayment.Payment_ID;
                string invoice = "this is the invoice template";
                float BalanceDue = 0;
                float Total = 0;
                float SaleVatAmount = 0;
                List<CartItem> getCartItems = db.CartItems.Include(zz => zz.C_Product_Size).ToList();

                foreach (CartItem item in getCartItems) //this for loop is to re-calculate the balances due from the cart table
                {
                    decimal Price = Convert.ToDecimal(db.C_Price.Where(zz => zz.C_Product_Size_ID == item.C_Product_Size_ID).Select(zz => zz.Price).FirstOrDefault());

                    BalanceDue = BalanceDue + (float)Price;
                    Total = Total + (float)Price;
                }

                SaleVatAmount = BalanceDue * (15 / 100);

                Sale createSale = new Sale();
                createSale.Client_ID = 1;                  //due to change when implemented logins
                createSale.SaleDate = DateTime.Now.Date;
                createSale.Payment_ID = PaymentID;
                createSale.Employee_ID = 3;                //due to change when implemented logins
                createSale.Invoice = invoice;              //not sure what the invoice is about

                int RandomNum = randomGen.Next(1000, 10000);

                createSale.ReceiptNumber = "#" + RandomNum.ToString();        //receipt number is varchar??  //ReceiptNumber is a primary Key??
                createSale.BalanceDue = BalanceDue;
                createSale.Total = Total;
                createSale.SaleVatAmount = SaleVatAmount;
                createSale.AmountPaid = CashAmount;

                db.Sales.Add(createSale);
                db.SaveChanges();

                int SaleID = createSale.Sale_ID;

                List<SaleLine> newSaleLine = new List<SaleLine>();

                foreach (CartItem item in getCartItems)
                {
                    SaleLine newLine = new SaleLine();
                    newLine.Sale_ID = SaleID;
                    newLine.C_Product_Size_ID = item.C_Product_Size_ID;
                    newLine.Quantity = item.Quantity;

                    newSaleLine.Add(newLine);
                }

                db.SaleLines.AddRange(newSaleLine);
                db.SaveChanges();

                db.CartItems.RemoveRange(getCartItems); //this is to delete everything in CartItem once moved to SaleLine  //SaleLine table can also be modified to use a SessionID as a primary key once you've implemented logins, just to control duplicate data
                db.SaveChanges();

                return "success"; //success message it to control the subscribe response in vscode project
            }
            catch(Exception err)
            {
                dynamic toReturn = new ExpandoObject();
                toReturn.Error = "Failed";
                toReturn.Message = err.Message;
                return toReturn;
            }




        }


    }

   
}
