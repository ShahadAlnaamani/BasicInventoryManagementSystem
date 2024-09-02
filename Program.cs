using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using System.IO;
using System.Text.Json;
using System.Collections;
using System;
using System.Text;

namespace BasicInventoryManagementSystem
{
    internal class Program
    {

        static void Main(string[] args)
        {
            string ProdPath = "C:\\Users\\Codeline user\\Desktop\\Projects\\InventorySysV3\\ProductInfo.txt";
            string InvoicePath = "C:\\Users\\Codeline user\\Desktop\\Projects\\InventorySysV3\\Invoices.txt";
            int ProductCounter = 0;
            //var ProductInformation = new List<(int ProductIDs, string ProductNames, float ProductPrices, int ProductQuantity, bool LowStock)>();
            var AllProducts = new List<(int ProductIDs, string ProductNames, float ProductPrices, int ProductQuantity, bool LowStock)>();

            CreateFiles(ProdPath, InvoicePath);

            bool Running = true;
            Console.WriteLine("\n");
            Console.WriteLine("     WELCOME TO SHAHAD'S E-SHOP!");
            Console.WriteLine("\n");
            Console.WriteLine("Lets start the setup!");
            Console.WriteLine("How many products would you like to setup?");

            //GETTING THE INPUT FROM USER [NUMBER OF PRODUCTS TO BE INITIALLY SET UP]
            int NumProducts = 0;
            try
            {

                NumProducts = int.Parse(Console.ReadLine());
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            ProductGetInfo(NumProducts, ProductCounter, ProdPath, AllProducts);


            //MAIN PART OF THE PROGRAM [ALL THE SET MENUS {CUSTOMER/ CLERK}  {CUSTOMER SERVICES}  {CLERK SERVICES} ]
            while (Running)
            {
                Console.Clear();
                Console.WriteLine("     WELCOME TO SHAHAD'S E-SHOP! \n");
                Console.WriteLine("Please select one of the following numbers:  ");
                Console.WriteLine("1. Customer ");
                Console.WriteLine("2. Shop Clerk \n");
                Console.WriteLine("Enter option: ");


                //GETTING USER IDENTITY (CUSTOMER/CLERK)
                int UserIdentity = 0;
                try
                {
                    UserIdentity = int.Parse(Console.ReadLine());
                }
                catch (Exception ex) { Console.WriteLine("Sorry invalid input, please try again enter a number. \n More info: " + ex.Message); Console.Clear(); }

                if (UserIdentity == 1 || UserIdentity == 2) //Input validation
                {
                    //CUSTOMER FEATURES MENU
                    if (UserIdentity == 1)
                    {
                        Console.Clear();
                        Console.WriteLine("     SHAHAD'S E-SHOP \n");
                        Console.WriteLine("\n  SERVICES: ");
                        Console.WriteLine("1. View all products");
                        Console.WriteLine("2. Search for product ");
                        Console.WriteLine("3. Back \n");
                        Console.WriteLine("Enter option: ");
                        int Option = 0;
                        try
                        {
                            Option = int.Parse(Console.ReadLine());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message); Console.Clear();
                        }
                        switch (Option)
                        {
                            //DISPLAYS ALL PRODUCTS
                            case 1:
                                {
                                    Console.Clear();
                                    ViewProducts(InvoicePath, ProdPath, AllProducts);
                                    Console.WriteLine();
                                    break;
                                }

                            //ALLOWS USER TO SEARCH FOR PRODUCTS AND SEE AVAILABLE STOCK FOR EACH
                            case 2:
                                {
                                    Console.Clear();
                                    //Search(ProductInformation);
                                    break;
                                }

                            //RETURNS USER TO MAIN MENU
                            case 3:
                                {
                                    Console.Clear();
                                    break;
                                }

                        }
                    }



                    //SHOP OWNER FEATURES
                    else if (UserIdentity == 2)
                    {
                        Console.Clear();
                        Console.WriteLine("     SHAHAD'S E-SHOP \n");
                        Console.WriteLine("\n  SERVICES: ");
                        Console.WriteLine("1. Check inventory");
                        Console.WriteLine("2. Edit products");
                        Console.WriteLine("3. Add new product ");
                        Console.WriteLine("4. View invoices ");
                        Console.WriteLine("5. Back \n");
                        Console.WriteLine("Enter option: ");
                        int Option = 0;
                        try
                        {
                            Option = int.Parse(Console.ReadLine());
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); Console.Clear(); }
                        switch (Option)
                        {
                            //SHOWS ALL REPORT AND STOCK STATISTICS (BONUS GIVES A LOW STOCK WARNING)
                            case 1:
                                {
                                    Console.Clear();
                                    //CheckInventory(ProductInformation);
                                    break;
                                }

                            //ALLOWS USER TO EDIT PRODUCTS (NAME/ADD QUANTITY/PRICE)
                            case 2:
                                {
                                    Console.Clear();
                                    //EditProduct(ProductInformation);
                                    break;
                                }

                            //SETUP NEW PRODUCTS
                            case 3:
                                {
                                    Console.Clear();
                                    Console.WriteLine("How many products would you like to add?");
                                    int NumNewProds = 0;
                                    try
                                    {
                                        NumNewProds = int.Parse(Console.ReadLine());
                                        ProductGetInfo(NumNewProds, ProductCounter, ProdPath, AllProducts);

                                    }
                                    catch (Exception ex) { Console.WriteLine(ex.Message); Console.Clear(); }
                                    Console.WriteLine("Add more products? Enter yes or no.");
                                    string ContinueAdding = Console.ReadLine().ToLower();
                                    if (ContinueAdding == "yes")
                                    {
                                        int MoreProds = 0;
                                        Console.WriteLine("How many more products? ");
                                        try
                                        {
                                            MoreProds = int.Parse(Console.ReadLine());
                                            Console.Clear();
                                        }
                                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                                        ProductGetInfo(MoreProds, ProductCounter, ProdPath, AllProducts);
                                    }
                                    else { Console.WriteLine("Improper input :("); Console.Clear(); }
                                    break;
                                }

                            //PRINTS OUT INVOICES 
                            case 4:
                                {
                                    Console.Clear();
                                    PrintInvoice(InvoicePath, AllProducts);
                                    break;
                                }


                            case 5:
                                {
                                    Console.Clear();
                                    break;
                                }
                        }
                    }

                }
                else { Console.WriteLine("Invalid Input: please input either 1 or 2."); }

                Console.WriteLine("Would like to continue? Enter yes or no: ");
                string Continue = (Console.ReadLine()).ToLower();
                if (Continue == "yes") { Running = ContinueProgram(Continue); Console.Clear(); }
                else { }
            }
        }


        //CREATING PRODUCT AND INVOICE FILES 
        static public void CreateFiles(string ProdPath, string InvoicePath)
        {
            //CREATING PRODUCT INFO FILE
            //Contains: |ProductIDs|Product Names|Product Prices|ProductQuantity|If lowstock|
            if (!File.Exists(ProdPath))
            {
                File.Create(ProdPath).Close();
            }

            //CREATING INVOICE FILE
            //Record of: |Customer name| Date and time of purchase| Product purchased| Quantity purchased of each item| Individual item prices| Total price| --> product prices and names may change so detailed logs must be kept
            if (!File.Exists(InvoicePath))
            {
                File.Create(InvoicePath).Close();
            }

        }





        //THIS FUNCTION GATHERS INFORMATION NEEDED TO SET PRODUCTS UP - IT CALLS THE NEW_PRODUCT FUNCTION SO THAT IT CAN BE REPEATED FOR A NUMBER OF TIMES THE USER SPECIFIED 
        static public void ProductGetInfo(int NumProducts, int ProductCounter, string ProdPath, List<(int ProductIDs, string ProductNames, float ProductPrices, int ProductQuantity, bool LowStock)> AllProducts)
        {
            Console.Clear();
            for (int i = 1; i <= NumProducts; i++)
            {
                Console.WriteLine("     SHAHAD'S E-SHOP \n");
                Console.WriteLine("Setting Up Products: \n \n ");
                Console.WriteLine("Enter product " + i + "'s name: ");
                string ProdName = Console.ReadLine();

                Console.WriteLine("Enter product " + i + "'s price: ");
                float ProdPrice = 0;
                try
                {
                    ProdPrice = float.Parse(Console.ReadLine());
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); return; }

                if (ProdPrice <= 0)
                {
                    while (ProdPrice <= 0)
                    {
                        Console.WriteLine("The product's price must be more than 0: ");
                        try
                        {
                            ProdPrice = float.Parse(Console.ReadLine());
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); return; }
                    }
                }

                Console.WriteLine("Enter product " + i + "'s quantity: ");
                int ProdQuantity = 0;
                try
                {
                    ProdQuantity = int.Parse(Console.ReadLine());
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); return; }
                //Checker(ProdName, ProdPrice, ProdQuantity, ProductCounter, AllProducts, ProdPath);
                NewProduct(ProdName, ProdPrice, ProdQuantity, ProductCounter, AllProducts, ProdPath);

                Console.WriteLine("Product " + i + "'s setup is complete. Press any key to continue");
                Console.ReadKey();
                Console.Clear();


            }


        }


        static public void Checker(string ProdName, float ProdPrice, int ProdQuantity, int ProductCounter, List<(int ProductIDs, string ProductNames, float ProductPrices, int ProductQuantity, bool LowStock)> AllProducts, string ProdPath)
        {/*
            int ProdID = 0;
            bool IsLowStock = false; //=> LowStock = false
            AllProducts.Add((ProdID, ProdName, ProdPrice, ProdQuantity, IsLowStock));

            using StreamReader reader = new StreamReader(ProdPath);
            {
                string Line;
                while ((Line = reader.ReadLine()) != null)
                {
                    string[] Product = Line.Split(','); //Sections will be arranged -> Prod ID, Prod Name, Prod Price, Prod Quantity, IfLowStock
                    int ID = int.Parse(Product[0]);
                    string Name = Product[1];
                    float Price = float.Parse(Product[2]);
                    int Quantity = int.Parse(Product[3]);
                    bool Stock = bool.Parse(Product[4]);
                    AllProducts.Add((ID, Name, Price, Quantity, Stock));
                }


                //Deconstructing the tuple list so we can see the last product ID available 
                List<int> LocationList = new List<int>();

                for (int i = 0; i < AllProducts.Count; i++)
                {
                    var (productIDs, productNames, productPrices, productQuantity, lowStock) = AllProducts[i];
                    LocationList.Add(productIDs);
                }

                int x = LocationList.Count();

                if (x != 0)
                {
                    int IndexOfMax = LocationList.Count;
                    ProdID = (IndexOfMax + 11);
                }

                else
                {
                    try
                    {
                        ProdID = (AllProducts.Count() + 10);
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); return; }
                }

                foreach (var name in AllProducts)
                {
                    if (name.ProductNames == ProdName)
                    {
                        Console.WriteLine("Invalid input: name must be unique, no repeated product names");
                        while (name.ProductNames == ProdName) //Ensures product name is unique 
                        {
                            Console.WriteLine("The products name must be unique please renter: ");
                            ProdName = Console.ReadLine();
                        }
                    }
                    else { break; }
                }

                if (ProdQuantity <= 5) { IsLowStock = true; }
                else { }
            }
                NewProduct(ProdName, ProdPrice, ProdQuantity, ProductCounter, AllProducts, IsLowStock, ProdPath);
           */

        }


        //THIS FUNCTION IS CALLED BY THE PRODUCT_GET_INFO AND SUPPLIES THE INFORMATION NEEDED TO SET THE PRODUCT UP
        static public void NewProduct(string ProdName, float ProdPrice, int ProdQuantity, int ProductCounter, List<(int ProductIDs, string ProductNames, float ProductPrices, int ProductQuantity, bool LowStock)> AllProducts, string ProdPath)
        {
            int ProdID = 0;
            bool IsLowStock = false; //=> LowStock = false

            AllProducts.Clear();
            ReadFile(ProdPath, AllProducts);
            //Getting information from product file  
            //AllProducts.Add((ProdID, ProdName, ProdPrice, ProdQuantity, IsLowStock));

            /* using StreamReader reader = new StreamReader(ProdPath);
             {
                 string Line;
                 while ((Line = reader.ReadLine()) != null)
                 {
                     string[] Product = Line.Split(','); //Sections will be arranged -> Prod ID, Prod Name, Prod Price, Prod Quantity, IfLowStock
                     int ID = int.Parse(Product[0]);
                     string Name = Product[1];
                     float Price = float.Parse(Product[2]);
                     int Quantity = int.Parse(Product[3]);
                     bool Stock = bool.Parse(Product[4]);
                     AllProducts.Add((ID, Name, Price, Quantity, Stock));
                 }


                 //Deconstructing the tuple list so we can see the last product ID available 
                 List<int> LocationList = new List<int>();

                 for (int i = 0; i < AllProducts.Count; i++)
                 {
                     var (productIDs, productNames, productPrices, productQuantity, lowStock) = AllProducts[i];
                     LocationList.Add(productIDs);
                 }

                 int x = LocationList.Count();
                 */
            // if (x != 0)
            //   {
            // int IndexOfMax = LocationList.Count;
            //   ProdID = (LocationList[IndexOfMax] + 11);
            //  }

            //  else
            //{
            //  try
            // {
            //   ProdID = (AllProducts.Count() + 10);
            //}
            //catch (Exception ex) { Console.WriteLine(ex.Message); return; }
            //}

            /* foreach (var name in AllProducts)
             {
                 if (name.ProductNames == ProdName)
                 {
                     Console.WriteLine("Invalid input: name must be unique, no repeated product names");
                     while (name.ProductNames == ProdName) //Ensures product name is unique 
                     {
                         Console.WriteLine("The products name must be unique please renter: ");
                         ProdName = Console.ReadLine();
                     }
                 }
                 else { break; }
             }
            */
            if (ProdQuantity <= 5) { IsLowStock = true; }
            else { }
            // }

            //Appending product to product| Structure -> ProdID, ProdName, ProdPrice, ProdQuantity, IsLowStock
            using (StreamWriter writer = new StreamWriter(ProdPath, true))
                {
                    writer.WriteLine($"{ProdID}, {ProdName}, {ProdPrice}, {ProdQuantity}, {IsLowStock}");
                }

                Console.WriteLine("Product: " + ProdName + " has been added successfully :) \n");
            

        }





        //THIS ALLOWS THE PROGRAM TO LOOP
        static public bool ContinueProgram(string Continue)
        {
            if (Continue == "no") { Console.WriteLine("Thank you for visiting Shahad's e-shop. \n Come again soon!"); return false; }
            else if (Continue == "yes") { return true; }
            else { return true; }
        }





        //LISTS OUT PRODUCTS - ALSO ALLOWS USER TO BUY ITEMS TO MAKE PROCESS EASIER FOR THE USER 
        static public void ViewProducts( string InvoicePath, string ProdPath, List<(int ProductIDs, string ProductNames, float ProductPrices, int ProductQuantity, bool LowStock)> AllProducts)
        {
            //Getting information from product file  
            /*using StreamReader reader = new StreamReader(ProdPath);
            {
                string Line;
                while ((Line = reader.ReadLine()) != null)
                {
                    string[] Product = Line.Split(','); //Sections will be arranged -> Prod ID, Prod Name, Prod Price, Prod Quantity, IfLowStock
                    int ID = int.Parse(Product[0]);
                    string Name = Product[1];
                    float Price = float.Parse(Product[2]);
                    int Quantity = int.Parse(Product[3]);
                    bool Stock = bool.Parse(Product[4]);
                    AllProducts.Add((ID, Name, Price, Quantity, Stock));
                }
            */
            AllProducts.Clear();
            ReadFile(ProdPath, AllProducts);
            //Deconstructing the tuple list so we can see the last product ID available 
            List<int> LocationList = new List<int>();

            for (int i = 0; i < AllProducts.Count; i++)
            {
                var (productIDs, productNames, productPrices, productQuantity, lowStock) = AllProducts[i];
                LocationList.Add(productIDs);
            }

            Console.WriteLine("     SHAHAD'S E-SHOP \n");
            Console.WriteLine("VIEW ALL PRODUCTS");

            Console.WriteLine("ID:\t Name:\t Prices: ");
            foreach (var product in AllProducts)
            {
               Console.WriteLine($"{product.ProductIDs}\t   {product.ProductNames}\t    {product.ProductPrices}");

            }

            Console.WriteLine("\n");
            Console.WriteLine("Would you like to buy anything? \n");
            Console.WriteLine("Enter Yes or No:");
            string Answer = Console.ReadLine();

            if (Answer.ToLower() == "yes")
            {
                //Deconstructing the tuple list to search for specific products (allows us to look through IDs, names etc )
                List<int> ProdIDList = new List<int>();
                for (int i = 0; i < AllProducts.Count; i++)
                {
                    var (productIDs, productNames, productPrices, productQuantity, lowStock) = AllProducts[i];
                    ProdIDList.Add(productIDs);
                }

                Console.WriteLine("Enter product ID");
                int ProdID = 0;
                int IndexOfProd = 0;
                try
                {
                    ProdID = int.Parse(Console.ReadLine());
                    if (!ProdIDList.Contains(ProdID))
                    {
                        Console.WriteLine("ID not found :(");
                    }

                    else { IndexOfProd = ProdIDList.IndexOf(ProdID); }

                }
                catch (Exception ex) { Console.WriteLine(ex.Message); return; }
                int AvailableQuantity = AllProducts[IndexOfProd].ProductQuantity;

                if (AllProducts[IndexOfProd].ProductIDs == ProdID)
                {
                    Console.WriteLine("How many would you like?");
                    int NoItems = 0;
                    try
                    {
                        NoItems = int.Parse(Console.ReadLine());
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); return; }

                    if (NoItems <= AvailableQuantity)
                    {
                        Console.WriteLine("We have enough for your order!");
                        Buy(NoItems, ProdID, InvoicePath, ProdPath, AllProducts);
                    }
                    else
                    {
                        if (AvailableQuantity > 0) // to find if there is any stock left (less than required)
                        {
                             Console.WriteLine("Sorry we don't have enough stock for your order. we currently have " + AvailableQuantity + " in stock.");
                             Console.WriteLine("Do you want to buy the remaining stock? \nEnter yes or no. ");
                             string GetRemaining = Console.ReadLine().ToLower();
                             if (GetRemaining == "yes") { NoItems = AvailableQuantity; Buy(NoItems, ProdID, InvoicePath, ProdPath, AllProducts); }
                             else { Console.WriteLine("Exiting..."); };
                        }
                        else { Console.WriteLine("Sorry this product is out of stock :("); }
                    }
                    Buy(NoItems, ProdID, InvoicePath, ProdPath, AllProducts);
                }
                else { Console.WriteLine("Sorry we don't have this product :("); }
            }
            else { }
            //}
        }





        //ALLOWS USER TO SEARCH FOR SPECIFIC ITEM - DISPLAYS ALL ITEMS FOR EASIER USE, LOOPS TO ALLOW MULTIPLE SEARCHES, ALLOWS USER TO BUY ITEM
        public static void Search( string InvoicePath, string ProdPath, List<(int ProductIDs, string ProductNames, float ProductPrices, int ProductQuantity, bool LowStock)> AllProducts)
        {
            AllProducts.Clear();
            ReadFile(ProdPath, AllProducts);

            /* //Getting information from product file  
             using StreamReader reader = new StreamReader(ProdPath);
             {
                 string Line;
                 while ((Line = reader.ReadLine()) != null)
                 {
                     string[] Product = Line.Split(','); //Sections will be arranged -> Prod ID, Prod Name, Prod Price, Prod Quantity, IfLowStock
                     int ID = int.Parse(Product[0]);
                     string Name = Product[1];
                     float Price = float.Parse(Product[2]);
                     int Quantity = int.Parse(Product[3]);
                     bool Stock = bool.Parse(Product[4]);
                     AllProducts.Add((ID, Name, Price, Quantity, Stock));
                 }*/

            //Deconstructing the tuple list to search for specific products (allows us to look through IDs, names etc )
            List<int> LocationList = new List<int>();

                for (int i = 0; i < AllProducts.Count; i++)
                {
                    var (productIDs, productNames, productPrices, productQuantity, lowStock) = AllProducts[i];
                    LocationList.Add(productIDs);
                }


                Console.WriteLine("List of products: ");
                Console.WriteLine("ID:\t Name:\t Prices: ");
                foreach (var product in AllProducts)
                {
                    Console.WriteLine($"{product.ProductIDs}\t {product.ProductNames}\t {product.ProductPrices}");

                }
                Console.WriteLine("\n");
                Console.WriteLine("Enter product ID: ");
                int SearchID = 0;
                int IndexOfProd = 0;

                //Deconstructing the tuple list to search for specific products (allows us to look through IDs, names etc )
                List<int> SearchIDList = new List<int>();
                for (int i = 0; i < AllProducts.Count; i++)
                {
                    var (productIDs, productNames, productPrices, productQuantity, lowStock) = AllProducts[i];
                    SearchIDList.Add(productIDs);
                }


                try
                {
                    SearchID = int.Parse(Console.ReadLine());
                    IndexOfProd = SearchIDList.IndexOf(SearchID);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); return; }
                int AvailableQuantity = AllProducts[IndexOfProd].ProductQuantity;

                if (AllProducts[IndexOfProd].ProductIDs == SearchID)
                {
                    if (AvailableQuantity > 0)
                    {
                        Console.WriteLine("This product is available :)\n We have: " + AvailableQuantity + " in stock.");
                        Console.WriteLine("Would you like to buy this product? Enter Yes or No.");
                        string GoToBuyNow = Console.ReadLine().ToLower();
                        if (GoToBuyNow == "yes")
                        {
                            Console.WriteLine("How many of this item would you like?");
                            int NoItems = 0;
                            try
                            {
                                NoItems = int.Parse(Console.ReadLine());
                            }
                            catch (Exception ex) { Console.WriteLine(ex.Message); return; }


                            if (NoItems <= AvailableQuantity)
                            {
                                Console.WriteLine("We have enough for your order!");
                                Buy(NoItems, SearchID, InvoicePath, ProdPath, AllProducts);
                            }
                            else
                            {
                                Console.WriteLine("Sorry we don't have enough stock for your order. we currently have " + AvailableQuantity + " in stock.");
                                Console.WriteLine("Do you want to buy the remaining stock? \nEnter yes or no. ");
                                string GetRemaining = Console.ReadLine().ToLower();
                                if (GetRemaining == "yes")
                                { NoItems = AvailableQuantity; Buy(NoItems, SearchID, InvoicePath, ProdPath, AllProducts); }
                                else { Console.WriteLine("Exiting..."); };

                            }
                        }
                    }
                    else { Console.WriteLine("Sorry this product is out of stock :("); }
                }
                else { Console.WriteLine("Sorry we don't have this product :("); }
                Console.WriteLine("Would you like to continue searching? Enter Yes or No.");
                string ContinueSearching = Console.ReadLine().ToLower();
                if (ContinueSearching == "yes")
                {
                    ViewProducts(InvoicePath, ProdPath, AllProducts);
                }
                else { }
           // }
        }





        //ALLOWS USER TO PURCHASE PRODUCT - ALSO PRINTS RECIPT, DEALS WITH STOCK ISSUES (NO STOCK/ NOT ENOUGH STOCK), IS CALLED BY OTHER FUNCTIONS FOR USERS EASE OF USE 
        public static void Buy(int NoItems, int ProdID, string InvoicePath, string ProdPath, List<(int ProductIDs, string ProductNames, float ProductPrices, int ProductQuantity, bool LowStock)> AllProducts)
        {
            AllProducts.Clear();
            ReadFile(ProdPath, AllProducts);
            /*
            //Getting information from product file  
            using StreamReader reader = new StreamReader(ProdPath);
            {
                string Line;
                while ((Line = reader.ReadLine()) != null)
                {
                    string[] Product = Line.Split(','); //Sections will be arranged -> Prod ID, Prod Name, Prod Price, Prod Quantity, IfLowStock
                    int ID = int.Parse(Product[0]);
                    string Name = Product[1];
                    float Price = float.Parse(Product[2]);
                    int Quantity = int.Parse(Product[3]);
                    bool Stock = bool.Parse(Product[4]);
                    AllProducts.Add((ID, Name, Price, Quantity, Stock));
                }*/

            //Deconstructing the tuple list to search for specific products (allows us to look through IDs, names etc )
            List<int> LocationList = new List<int>();

                for (int i = 0; i < AllProducts.Count; i++)
                {
                    var (productIDs, productNames, productPrices, productQuantity, lowStock) = AllProducts[i];
                    LocationList.Add(productIDs);
                }


                int Location = LocationList.IndexOf(ProdID);
                string ProductName = AllProducts[Location].ProductNames;
                float ProdPrice = AllProducts[Location].ProductPrices;
            

                Console.Clear();
                Console.WriteLine("     SHAHAD'S E-SHOP \n");


                if (NoItems > 0)
                {
                    if (NoItems <= AllProducts[Location].ProductQuantity)
                    {
                        Console.Clear();
                        Console.WriteLine("     SHAHAD'S E-SHOP \n");
                        Console.WriteLine("Your order is: x" + NoItems + " " + ProductName + " $" + ProdPrice);
                        Console.WriteLine("Enter Yes to proceed to checkout (Enter No to discard basket): ");
                        string CheckoutNow = (Console.ReadLine()).ToLower();

                    if (CheckoutNow == "yes")
                    {
                        StringBuilder Name = new StringBuilder();
                        Console.WriteLine("Please enter your name: ");
                        Name.Append(Console.ReadLine());

                        float Total = NoItems * ProdPrice;
                        DateTime Now = DateTime.Now;

                        Console.Clear();
                        Console.WriteLine("     SHAHAD'S E-SHOP \n");
                        Console.WriteLine("Checkout: \n \n ");
                        Console.WriteLine("Your order is: x" + NoItems + " " + ProductName + " $" + ProdPrice);
                        Console.WriteLine("Total: $" + Total);
                        Console.WriteLine("Press any key to print your recipt");
                        Console.ReadKey();



                        //Updating product quantity and changing low stock flag if required 
                        AllProducts[Location] = (AllProducts[Location].ProductIDs, AllProducts[Location].ProductNames, AllProducts[Location].ProductPrices, (AllProducts[Location].ProductQuantity - NoItems), AllProducts[Location].LowStock); //Minusing from stock 

                        if (AllProducts[Location].ProductQuantity < 5) //seeing if product needs to be added to the low stock flag
                        {
                            if (AllProducts[Location].LowStock == false) { AllProducts[Location] = (AllProducts[Location].ProductIDs, AllProducts[Location].ProductNames, AllProducts[Location].ProductPrices, AllProducts[Location].ProductQuantity, LowStock: true); } //change lowstock to true meaning less than 5 available 
                            else { }
                        }


                        //Appending updated product information to file 
                        StreamWriter writer = new StreamWriter(ProdPath); //rewrite all products to update
                        foreach (var prod in AllProducts)
                        {
                            writer.WriteLine($"{prod.ProductIDs}, {prod.ProductNames},  {prod.ProductPrices}, {prod.ProductQuantity},  {prod.LowStock}");
                        }

                        Console.Clear();
                        Console.WriteLine("*  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *"); //could make a function to print recipt instead of repeating code 
                        Console.WriteLine("\n\n                         SHAHAD'S E-SHOP \n \n");
                        Console.WriteLine("*  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  * \n");
                        Console.WriteLine(Now.ToString("dd-MM-yyyy HH:mm:ss"));
                        Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                        Console.WriteLine("Order: x" + NoItems + " " + ProductName + " $" + ProdPrice);
                        Console.WriteLine("Total: $" + Total);
                        Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                        Console.WriteLine("Thank you for shopping at Shahad's E-Shop\nCome again soon  " + Name.ToString() + " :)");
                        Console.WriteLine("\n\n\n\n");
                        Console.WriteLine("*  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *");
                        Name.Clear(); //Clearing name so it won't affect other functions 

                        //Appending invoice| Structure -> ProdID, ProdName, ProdPrice, ProdQuantity, IsLowStock
                        using StreamWriter InvWriter = new StreamWriter(InvoicePath, true)
                        {
                            InvWriter.WriteLine($" {Now}, {Name}, {ProdID}, {ProductName}, {ProdPrice}, {NoItems}, {Total}");
                        }
                    }

                    else
                    { Console.ReadKey(); Console.WriteLine("Exiting..."); }

                }
                    else
                    {
                        Console.WriteLine("Sorry it looks like we don't have enough stock to fulfil the whole order :(");
                        Console.WriteLine("We currently have " + AllProducts[Location].ProductQuantity + " of the product you want.");
                        Console.WriteLine("Would you like to purchase the available stock? Enter yes to continue and any other key to exit.");
                        string Response = (Console.ReadLine()).ToLower();

                        if (Response == "yes")
                        {
                            StringBuilder Name = new StringBuilder();
                            Console.WriteLine("Please enter your name: ");
                            Name.Append(Console.ReadLine());

                            NoItems = AllProducts[Location].ProductQuantity;
                            float Total = NoItems * ProdPrice;
                            DateTime Now = DateTime.Now;
                            Console.Clear();
                            Console.WriteLine("     SHAHAD'S E-SHOP \n");
                            Console.WriteLine("Checkout: \n \n ");
                            Console.WriteLine("Your order is: x" + NoItems + " " + ProductName + " $" + ProdPrice);
                            Console.WriteLine("Total: $" + Total);
                            Console.WriteLine("Press any key to print your recipt");


                            //Updating product quantity and changing low stock flag if required 
                            AllProducts[Location] = (AllProducts[Location].ProductIDs, AllProducts[Location].ProductNames, AllProducts[Location].ProductPrices, (AllProducts[Location].ProductQuantity - NoItems), AllProducts[Location].LowStock); //Minusing from stock

                            if (AllProducts[Location].ProductQuantity < 5) //seing if product needs to be added to the low stock flag
                            {
                                if (AllProducts[Location].LowStock == false) { AllProducts[Location] = (AllProducts[Location].ProductIDs, AllProducts[Location].ProductNames, AllProducts[Location].ProductPrices, AllProducts[Location].ProductQuantity, LowStock: true); } //change lowstock to true meaning less than 5 available 
                                else { }
                            }
                            //Appending updated product information to file 
                            StreamWriter writer = new StreamWriter(ProdPath); //rewrite all products to update
                           foreach (var prod in AllProducts)
                           {
                                writer.WriteLine($"{prod.ProductIDs}, {prod.ProductNames},  {prod.ProductPrices}, {prod.ProductQuantity},  {prod.LowStock}");
                           }


                            //Appending invoice | Structure -> ProdID, ProdName, ProdPrice, ProdQuantity, IsLowStock
                            StreamWriter WriteInvoice = new StreamWriter(InvoicePath, true);
                            WriteInvoice.WriteLine($" {Now}, {Name.ToString()}, {ProdID}, {ProductName}, {ProdPrice}, {NoItems}, {Total}");

                            Console.Clear();
                            Console.WriteLine("*  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *");
                            Console.WriteLine("\n\n                         SHAHAD'S E-SHOP \n \n");
                            Console.WriteLine("*  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  * \n");
                            Console.WriteLine(Now.ToString("dd-MM-yyyy HH:mm:ss"));
                            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                            Console.WriteLine("Order: x" + NoItems + " " + ProductName + " $" + ProdPrice);
                            Console.WriteLine("Total: $" + Total);
                            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                            Console.WriteLine("Thank you for shopping at Shahad's E-Shop\nCome again soon" + Name.ToString() + " :)");
                            Console.WriteLine("\n\n\n\n");
                            Console.WriteLine("*  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *");
                        }
                        else { Console.ReadKey();Console.WriteLine("Exiting..."); }
                    }
               // }

                else
                {
                    Console.WriteLine("Sorry but it looks like you have entered an invalid number of items :( \nPlease try again, enter a number larger than 0. ");
                }
                Console.WriteLine("Would you like to continue shopping? Enter Yes or No.");


                AllProducts.Clear(); //Clearning tuple list so it doesn't interfere with other functions 


                string ContinueShopping = Console.ReadLine().ToLower();
                if (ContinueShopping == "yes")
                {
                    ViewProducts(InvoicePath, ProdPath, AllProducts);
                }
                else { }
            //}
        }




        //ALLOWS STORE STAFF TO CHECK INVENTORY - LISTS OUT ITEMS WITH STOCK LESS THAN 5 ITEMS - LINKS TO ADDING STOCK FOR USERS EASE OF USE
        public static void CheckInventory(string ProdPath, List<(int ProductIDs, string ProductNames, float ProductPrices, int ProductQuantity, bool LowStock)> AllProducts)
        {
            float Total = 0;
            float SumTotal = 0;

            AllProducts.Clear();
            ReadFile(ProdPath, AllProducts);

            /*
            //Getting information from product file  
            using StreamReader reader = new StreamReader(ProdPath);
            {
                string Line;
                while ((Line = reader.ReadLine()) != null)
                {
                    string[] Product = Line.Split(','); //Sections will be arranged -> Prod ID, Prod Name, Prod Price, Prod Quantity, IfLowStock
                    int ID = int.Parse(Product[0]);
                    string Name = Product[1];
                    float Price = float.Parse(Product[2]);
                    int Quantity = int.Parse(Product[3]);
                    bool Stock = bool.Parse(Product[4]);
                    AllProducts.Add((ID, Name, Price, Quantity, Stock));
                }*/

                //Deconstructing the tuple list to search for specific products (allows us to look through IDs, names etc )
                List<int> LocationList = new List<int>();

                for (int i = 0; i < AllProducts.Count; i++)
                {
                    var (productIDs, productNames, productPrices, productQuantity, lowStock) = AllProducts[i];
                    LocationList.Add(productIDs);
                }

                Console.WriteLine("     SHAHAD'S E-SHOP \n");
                Console.WriteLine("Inventory: \n \n");
                Console.WriteLine("!!!Low Stock Warning!!!");
                foreach (var product in AllProducts)
                {
                    if (product.LowStock == true)
                    { Console.WriteLine("Product: ID " + product.ProductIDs + " " + product.ProductNames + " $" + product.ProductPrices + " x" + product.ProductQuantity); }
                    else { Console.WriteLine("No low stock items :)"); }
                }

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("All products: ");
                foreach (var product in AllProducts)
                {
                    Total = product.ProductQuantity * product.ProductPrices;
                    Console.WriteLine("Product: ID " + product.ProductIDs + " " + product.ProductNames + " $" + product.ProductPrices + " x" + product.ProductQuantity + " total stock value -> $" + Total);
                    SumTotal = +Total; //Calculates the total value of all stock

                }
                Console.WriteLine();
                Console.WriteLine("The total value of stock is: " + SumTotal + "\n");

                Console.WriteLine("Would you like to add stock?");
                string AddStock = Console.ReadLine().ToLower();

                if (AddStock == "yes") { EditProduct( ProdPath, AllProducts); }
                else { Console.Clear(); }
            //}
        }




        //ALLOWS USER TO EDIT EXISTING PRODUCTS 
        public static void EditProduct( string ProdPath, List<(int ProductIDs, string ProductNames, float ProductPrices, int ProductQuantity, bool LowStock)> AllProducts)
        {
            AllProducts.Clear();
            ReadFile(ProdPath, AllProducts);
            /*
            //Getting information from product file  
            using StreamReader reader = new StreamReader(ProdPath);
            {
                string Line;
                while ((Line = reader.ReadLine()) != null)
                {
                    string[] Product = Line.Split(','); //Sections will be arranged -> Prod ID, Prod Name, Prod Price, Prod Quantity, IfLowStock
                    int ID = int.Parse(Product[0]);
                    string Name = Product[1];
                    float Price = float.Parse(Product[2]);
                    int Quantity = int.Parse(Product[3]);
                    bool Stock = bool.Parse(Product[4]);
                    AllProducts.Add((ID, Name, Price, Quantity, Stock));
                }*/

            //Deconstructing the tuple list to search for specific products (allows us to look through IDs, names etc )
            List<int> LocationList = new List<int>();

                for (int i = 0; i < AllProducts.Count; i++)
                {
                    var (productIDs, productNames, productPrices, productQuantity, lowStock) = AllProducts[i];
                    LocationList.Add(productIDs);
                }

                bool Editing = true;
                while (Editing)
                {
                    Console.Clear();
                    Console.WriteLine("     SHAHAD'S E-SHOP \n");
                    Console.WriteLine("\n  Edit Products: ");
                    Console.WriteLine("Choose from the following");
                    Console.WriteLine("1. Edit product quantity");
                    Console.WriteLine("2. Edit product name");
                    Console.WriteLine("3. Edit product price");
                    Console.WriteLine("Enter number: ");
                    int EditOption = 0;
                    try
                    {
                        EditOption = int.Parse(Console.ReadLine());
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); return; }

                    Console.Clear();
                    Console.WriteLine("     SHAHAD'S E-SHOP \n");
                    Console.WriteLine("\n  Edit Products: ");
                    Console.WriteLine("Choose from the following products: ");
                    for (int i = 0; i < AllProducts.Count(); i++)
                    {
                        Console.WriteLine("Product: ID " + AllProducts[i].ProductIDs + " " + AllProducts[i].ProductNames + " $" + AllProducts[i].ProductPrices);

                    }
                    Console.WriteLine("Enter Product ID: ");
                    int EditProd = 0;
                    try
                    {
                        EditProd = int.Parse(Console.ReadLine());
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); return; }

                    int Location = LocationList.IndexOf(EditProd);
                    if (AllProducts[Location].ProductIDs == EditProd) //check if product id is found 
                    {
                        Console.Clear();
                        Console.WriteLine("     SHAHAD'S E-SHOP \n");
                        //int IndexOfEdit = ProductIDs.IndexOf(EditProd);
                        Console.WriteLine($"Current details:  ID:{AllProducts[Location].ProductIDs} {AllProducts[Location].ProductNames} ${AllProducts[Location].ProductPrices}\n ");
                        switch (EditOption)
                        {
                            case 1:
                                Console.WriteLine("Edit Product Quantity \n");
                                Console.WriteLine("Number of products to be added: ");
                                int NewQuantity = 0;
                                try
                                {
                                    NewQuantity = int.Parse(Console.ReadLine());
                                }
                                catch (Exception ex) { Console.WriteLine(ex.Message); return; }
                                if (NewQuantity > 0)
                                {
                                    AllProducts[Location] = (AllProducts[Location].ProductIDs, AllProducts[Location].ProductNames, AllProducts[Location].ProductPrices, ProductQuantity: AllProducts[Location].ProductQuantity + NewQuantity, AllProducts[Location].LowStock);
                                    Console.WriteLine($"Updated details:  ID:{AllProducts[Location].ProductIDs} {AllProducts[Location].ProductNames} ${AllProducts[Location].ProductPrices}\n ");
                                    if (AllProducts[Location].ProductQuantity >= 5) //seing if product needs to be added to the low stock flag
                                    {
                                        if (AllProducts[Location].LowStock == true) { AllProducts[Location] = (AllProducts[Location].ProductIDs, AllProducts[Location].ProductNames, AllProducts[Location].ProductPrices, AllProducts[Location].ProductQuantity, LowStock: false); } //remove low stock flag if total stock of prod is more than 5 
                                        else { }
                                    }


                                    //Appending updated product information to file 
                                    using (StreamWriter writer = new StreamWriter(ProdPath)) //rewrite all products to update
                                    {
                                        foreach (var prod in AllProducts)
                                        {
                                            writer.WriteLine($"{prod.ProductIDs}, {prod.ProductNames},  {prod.ProductPrices}, {prod.ProductQuantity},  {prod.LowStock}");
                                        }
                                    }
                                }
                                else { Console.WriteLine("Invalid input: value must be greater than 0"); }
                                break;

                            case 2:
                                Console.WriteLine("Edit Product Name \n");
                                Console.WriteLine("New product name: ");
                                string NewProdName = Console.ReadLine();
                                foreach (var name in AllProducts)
                                {
                                    if (name.ProductNames == NewProdName)
                                    {
                                        Console.WriteLine("Invalid input: name must be unique, no repeated product names");
                                        while (name.ProductNames == NewProdName) //ensures product name is unique 
                                        {
                                            Console.WriteLine("The products name must be unique please renter: ");
                                            NewProdName = Console.ReadLine();
                                        }
                                    }
                                    else
                                    {
                                        AllProducts[Location] = (AllProducts[Location].ProductIDs, ProductNames: NewProdName, AllProducts[Location].ProductPrices, AllProducts[Location].ProductQuantity, AllProducts[Location].LowStock);

                                        //Appending updated product information to file 
                                        using (StreamWriter writer = new StreamWriter(ProdPath)) //rewrite all products to update
                                        {
                                            foreach (var prod in AllProducts)
                                            {
                                                writer.WriteLine($"{prod.ProductIDs}, {prod.ProductNames},  {prod.ProductPrices}, {prod.ProductQuantity},  {prod.LowStock}");
                                            }
                                        }

                                        Console.WriteLine($"Updated details:  ID:{AllProducts[Location].ProductIDs} {AllProducts[Location].ProductNames} ${AllProducts[Location].ProductPrices}\n ");
                                    }
                                }
                                break;

                            case 3:
                                Console.WriteLine("Edit Product Price \n");
                                Console.WriteLine("New product price: ");
                                float NewPrice = float.Parse(Console.ReadLine());
                                if (NewPrice > 0)
                                {
                                    AllProducts[Location] = (AllProducts[Location].ProductIDs, AllProducts[Location].ProductNames, ProductPrices: NewPrice, AllProducts[Location].ProductQuantity, AllProducts[Location].LowStock);

                                    using (StreamWriter writer = new StreamWriter(ProdPath)) //rewrite all products to update
                                    {
                                        foreach (var prod in AllProducts)
                                        {
                                            writer.WriteLine($"{prod.ProductIDs}, {prod.ProductNames},  {prod.ProductPrices}, {prod.ProductQuantity},  {prod.LowStock}");
                                        }
                                    }

                                    Console.WriteLine($"Updated details:  ID:{AllProducts[Location].ProductIDs} {AllProducts[Location].ProductNames} ${AllProducts[Location].ProductPrices}\n ");
                                }
                                else { Console.WriteLine("Invalid input: price must be more than 0"); }
                                break;


                            case 4:
                                Console.WriteLine("Invalid input: please choose one of the available options");
                                break;
                        }

                        AllProducts.Clear(); //Clearning tuple list so it doesn't interfere with other functions 
                    }


                    else { Console.WriteLine("The product ID does not exist :("); }
                    Console.WriteLine("Would you like to continue editing? Enter Yes or No.");
                    string EditingResponse = Console.ReadLine().ToLower();
                    if (EditingResponse == "yes") { }
                    else { Editing = false; }
                }
            //}

        }


        //PRINTS OUT ALL PURCHASE INFO 
        public static void PrintInvoice(string InvoicePath, List<(int ProductIDs, string ProductNames, float ProductPrices, int ProductQuantity, bool LowStock)> AllProducts)
        {
            using StreamReader reader = new StreamReader(InvoicePath);
            {
                string Line;
                while ((Line = reader.ReadLine()) != null)
                {
                    string[] Product = Line.Split(','); //Sections will be arranged -> Prod ID, Prod Name, Prod Price, Prod Quantity, IfLowStock
                    int ID = int.Parse(Product[2]);
                    string CustomerName = Product[1];   
                    string Name = Product[3];
                    float Price = float.Parse(Product[4]);
                    int Quantity = int.Parse(Product[5]);
                    float Total = float.Parse(Product[6]);
                    Console.WriteLine($"Customer Name: {Name} | Product ID: {ID} | Product Name: {Name} | Quantity: {Quantity} | Total Paid: ${Total}");
                }

            }

        }


        //READING DATA FROM PRODUCT FILE 
        public static void ReadFile (string ProdPath, List<(int ProductIDs, string ProductNames, float ProductPrices, int ProductQuantity, bool LowStock)> AllProducts)
        {
            if (File.Exists(ProdPath))
            {
                using StreamReader reader = new StreamReader(ProdPath);
                {
                    string Line;
                    while ((Line = reader.ReadLine()) != null)
                    {
                        string[] Product = Line.Split(','); //Sections will be arranged -> Prod ID, Prod Name, Prod Price, Prod Quantity, IfLowStock
                        int ID = int.Parse(Product[0]);
                        string Name = Product[1];
                        float Price = float.Parse(Product[2]);
                        int Quantity = int.Parse(Product[3]);
                        bool Stock = bool.Parse(Product[4]);
                        AllProducts.Add((ID, Name, Price, Quantity, Stock));
                    }

                }
            }
        }

        //REWRITING DATA ONTO PRODUCT FILE 
        public static void WriteFile()
        {
            using (StreamWriter writer = new StreamWriter(ProdPath, true))
            {
                writer.WriteLine($"{ProdID}, {ProdName}, {ProdPrice}, {ProdQuantity}, {IsLowStock}");
            }
        }
    }
}

