
using StoreConsoleApp.UI;

namespace StoreConsoleApp.App
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("[ Welcome to Stationery Shop ]\n");
            //OrderProcessService orderProcess = new(server);
            bool exitShop = false;
            while (!exitShop)
            {
                Store store = new();
                // USER LOGIN SECTION
                var login = await CustomerLoginAsync(store);
                int CustomerID = login.Item1;
                exitShop = login.Item2;
                if (exitShop) break;
                // STORE INFORMATION SECTION
            StoreLocation:
                var locationResults = await StoreLocationAsync(store);
                int LocationID = locationResults.Item1;
                exitShop = locationResults.Item2;
                if (exitShop) break;
                // MENU SECTION
            MenuSelection:
                Console.Write("\nMenu Options:\n" +
                    "--------------------------------------------------------------\n" +
                    "     1: Make an order\n" +
                    "     2: Display all order history of this store location\n" +
                    "     3: Display all order histroy of Stationery Shop\n" +
                    "     4: Display a specific order. (Most Recent or By Order#)\n" +
                    "  Exit: Exit this store location.\n" +
                    "--------------------------------------------------------------\n" +
                    "Select an option: ");
                string? menuSelection = Console.ReadLine();
                Console.WriteLine();
                if (CheckEmptyInput(menuSelection, out menuSelection)) goto MenuSelection;
                if(ExitShop(menuSelection)) goto StoreLocation; // if user want exit shop
                if (menuSelection.Trim() == "1")
                {
                    // ORDERING SECTION
                    Console.WriteLine();
                    var productsResult = await store.GetStoreProductsAsync(LocationID + "");
                    string storeProducts = productsResult.Item1;
                    Console.WriteLine(storeProducts);
                    exitShop = await Ordering(store, LocationID, CustomerID);
                    if (exitShop) goto MenuSelection;
                }
                else if(menuSelection.Trim() == "2")
                {
                    // display all order history of this store location
                    //Console.WriteLine(orderProcess.DisplayOrderHistory(CustomerID, out bool getHistoryFailed, LocationID));
                    //if(getHistoryFailed) goto MenuSelection;
                }
                else if(menuSelection.Trim() == "3")
                {
                    // display all order history of Stationery Shop
                    //Console.WriteLine(orderProcess.DisplayOrderHistory(CustomerID, out bool getHistoryFailed));
                    //if (getHistoryFailed) goto MenuSelection;
                }
                else if(menuSelection.Trim() == "4")
                {
                SearchOrder:
                    Console.Write("Enter [0] for most recent order OR Enter [Order #] for specific order: ");
                    string? searchOrder = Console.ReadLine();
                    Console.WriteLine();
                    if (CheckEmptyInput(searchOrder, out searchOrder)) goto SearchOrder;
                    if (ExitShop(searchOrder)) goto StoreLocation; // if user want exit shop
                    if (!int.TryParse(searchOrder, out int orderNum)) // must be integer
                    {
                        InvalidInputMsg();
                        goto SearchOrder; 
                    }
                    // display most recent/specific order of the customer depend on user input
                    //Console.WriteLine(orderProcess.DisplayOrderHistory(CustomerID, out bool getHistoryFailed, -1, orderNum));
                    //if (getHistoryFailed) goto MenuSelection;
                }
                else
                {
                    InvalidInputMsg();
                    goto MenuSelection;
                }
            ExitOrNot:
                Console.Write("Enter (1) Go back to Menu OR (exit) for exit the store location: ");
                string? goMenu = Console.ReadLine();
                if (CheckEmptyInput(goMenu, out goMenu)) goto ExitOrNot;
                if (goMenu.Trim() == "1") goto MenuSelection;
                else if (ExitShop(goMenu)) goto StoreLocation;
                else
                {
                    InvalidInputMsg();
                    goto ExitOrNot;
                }
            }
        }
        
        /// <summary>
        ///     Customer login section. 
        ///     If user is new customer, then create a new account and print the customer ID to user. 
        ///     If user has an account, then ask user to login and store the customer ID.
        /// </summary>
        /// <param name="store">A Store type class</param>
        /// <param name="exit">A boolean that checks if user want to exit the shop</param>
        /// <returns>int Customer ID</returns>
        public static async Task<(int, bool)> CustomerLoginAsync(Store store)
        {
        NewCustomer:
            bool exit;
            int CustomerID = -1;    // initialize
            Console.Write("Before you start shopping, are you a new customer? (y/n) ");
            string? input = Console.ReadLine();
            // check if input is null/empty
            if (CheckEmptyInput(input, out input)) goto NewCustomer;

            exit = ExitShop(input); // if user want exit shop
            if (exit) return (CustomerID, exit);
            // if not a new customer, login account
            if (input.ToLower() == "n")
            {
            Login:
                // Login User
                Console.Write("Please Enter [Your Customer ID#] OR Enter [forgot] for Forgot Customer ID: ");
                string? customerID = Console.ReadLine();
                if (CheckEmptyInput(customerID, out customerID)) goto Login;
                bool customerExist;
                if (customerID.ToLower() == "forgot")
                {
                    Console.Write("Enter Your First Name: ");
                    string? firstName = Console.ReadLine();
                    // check if firstname is null/empty
                    if (CheckEmptyInput(firstName, out firstName)) goto Login;

                    Console.Write("Enter Your Last Name: ");
                    string? lastName = Console.ReadLine();
                    // check if lastname is null/empty
                    if (CheckEmptyInput(lastName, out lastName)) goto Login;
                    var searchedCustomer = await store.SearchCustomerAsync(customerID, firstName, lastName);
                    customerExist = searchedCustomer.Item1;
                    CustomerID = searchedCustomer.Item2;
                    if (!customerExist) goto NewCustomer;
                }
                else
                {
                    var searchedCustomer = await store.SearchCustomerAsync(customerID);
                    customerExist = searchedCustomer.Item1;
                    if (!customerExist) goto NewCustomer;
                }
            }
            else if (input.ToLower() == "y")    // new customer, add to database
            {
            CreateAccount:
                Console.WriteLine("To Create An New Acount, Please Enter Your Name.");
                Console.Write("Enter Your First Name: ");
                string? firstName = Console.ReadLine();
                // check if firstname is null/empty
                if (CheckEmptyInput(firstName, out firstName)) goto CreateAccount;

                Console.Write("Enter Your Last Name: ");
                string? lastName = Console.ReadLine();
                // check if lastname is null/empty
                if (CheckEmptyInput(lastName, out lastName)) goto CreateAccount;

                // create account
                CustomerID = await store.CreateAccount(firstName, lastName);
                if (CustomerID < 0)
                {
                    Console.WriteLine("Something When Wrong... Please Try Again.\n");
                    goto CreateAccount;  // jump to line name CreateAccount
                }
                Console.Write($"\nYour Account is Created Successfully!\n\n" +
                    $"Welcome to Stationery Shop, {firstName} {lastName}.\n" +
                    $"-------------------------------------------------------\n" +
                    $"[ Your Customer ID#: {CustomerID} ]\n" +
                    $"[ Please Remember Your Customer ID For Later Login. ]\n\n");
            }
            else
            {
                InvalidInputMsg();
                goto NewCustomer;
            }
            return (CustomerID, exit);
        }
        
        /// <summary>
        ///     Store Information section.
        ///     Print all the store location and ask user to pick a choice.
        ///     After user select a store location, display all products in the store loaction.
        /// </summary>
        /// <param name="store">A Store type class</param>
        /// <param name="exit">A boolean that checks if user want to exit the shop</param>
        /// <returns>The location ID that User selected</returns>
        public static async Task<(int, bool)> StoreLocationAsync(Store store)
        {
        StoreLocations:
            bool exit;
            Console.WriteLine(await store.GetLocationsAsync());    // print store locations
            Console.WriteLine("Which store location do you want to shop today? ");
            Console.Write("Enter an ID number or Enter EXIT for Exit this Shop: ");
            string? locationID = Console.ReadLine();
            Console.WriteLine();
            if (CheckEmptyInput(locationID, out locationID)) goto StoreLocations;

            exit = ExitShop(locationID); // if user want exit shop
            if (exit) return (-1, exit);
            // Print store products and get product list
            var tmp = await store.GetStoreProductsAsync(locationID);
            bool validID = tmp.Item2;
            // invalidID go back to the top and try again
            if (!validID) goto StoreLocations;
            return (int.Parse(locationID), exit);
        }
        
        /// <summary>
        ///     Ordering ask user to select products and quantity repeatedly 
        ///     until user want to checkout.
        /// </summary>
        /// <param name="store">Store type class</param>
        /// <param name="locationID">Location ID</param>
        public static async Task<bool> Ordering(Store store, int locationID, int customerID)
        {
            bool exit;
            List<string> productNames = new();
            List<int> productQty = new();
            while (true)
            {
            Ordering:
                Console.Write("\nChoose the product you want to order.\nEnter Product ID#: ");
                string? productID = Console.ReadLine();
                // check null/empty input
                if (CheckEmptyInput(productID, out productID)) goto Ordering;
                // valid product id
                if (store.ValidProductID(productID, out string productName))
                {
                    Console.Write("Enter Product Quantity (Max Qty: 99): ");
                    string? orderQty = Console.ReadLine();
                    if (CheckEmptyInput(orderQty, out orderQty)) goto Ordering;
                    var validAmount = await store.validAmountAsync(productName, orderQty, locationID);
                    bool isValid = validAmount.Item1;
                    int orderAmount = validAmount.Item2;
                    // valid quantity
                    if (isValid)
                    {
                        // need to handle when select same product twice.
                        if (productNames.Contains(productName))
                        {
                            int idx = productNames.IndexOf(productName);
                            productQty[idx] += orderAmount;
                        }
                        else
                        {
                            productNames.Add(productName);  // add product Name to list
                            productQty.Add(orderAmount);
                        }
                    // continue ordering? or go checkout
                    AskAgain:
                        Console.Write("\nContinue shopping (1) or Go checkout (2): ");
                        string? input = Console.ReadLine();
                        if (CheckEmptyInput(input, out input)) goto AskAgain;
                        if (input == "1")
                        {
                            Console.WriteLine();
                            var productsResult = await store.GetStoreProductsAsync(locationID + "");
                            string storeProducts = productsResult.Item1;
                            Console.WriteLine(storeProducts);
                            goto Ordering;
                        }
                        else if (input == "2")
                        {
                            // display the shopping list
                            OrderProcess orderProcess = new();
                            var orderDetail = await orderProcess.DisplayOrderDetail(store, customerID, productNames, productQty, locationID);
                            string receipt = orderDetail.Item1;
                            bool Processfailed = orderDetail.Item2;
                            if (Processfailed) goto Ordering;
                            // thank you for your purchase ...
                            Console.WriteLine("\nThank you for your purchase!\n");
                            Console.WriteLine(receipt);
                        ContinueShopping:
                            Console.Write("Would you like another order? (y/n) ");
                            string? continueShopping = Console.ReadLine();
                            if (CheckEmptyInput(continueShopping, out continueShopping)) goto ContinueShopping;
                            if(continueShopping.ToLower() == "y")
                            {
                                productNames = new();
                                productQty = new();
                                Console.WriteLine();
                                var productsResult = await store.GetStoreProductsAsync(locationID + "");
                                string storeProducts = productsResult.Item1;
                                Console.WriteLine(storeProducts);
                                goto Ordering;
                            } 
                            else if(continueShopping.ToLower() == "n" || ExitShop(continueShopping))
                            {
                                exit = true;
                                break;
                            }
                        }
                        else
                        {
                            InvalidInputMsg();
                            goto AskAgain;
                        }
                    }
                    else
                    {
                        goto Ordering;
                    }
                }
                else
                {
                    InvalidInputMsg();
                    goto Ordering;
                }
            }
            return exit;
        }
        
        /// <summary>
        ///     Printing empty input error message.
        /// </summary>
        public static void InvalidInputMsg()
        {
            Console.WriteLine("\n--- Your Input is invalid, please try again. ---\n");
        }
        /// <summary>
        ///     Check if user input is null or empty
        /// </summary>
        /// <param name="userInput">User Input string, can be null</param>
        /// <param name="userNotNullInput">Not null User input string</param>
        /// <returns>true if input is null/empty, false otherwise.</returns>
        public static bool CheckEmptyInput(string? userInput, out string userNotNullInput)
        {
            if (userInput == null || userInput.Length <= 0)
            {
                InvalidInputMsg();
                userNotNullInput = "";
                return true;
            }
            userNotNullInput = userInput;
            return false;
        }

        /// <summary>
        ///     Check if user input is "exit".
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns>true if user want to exit store. false otherwise.</returns>
        public static bool ExitShop(string userInput)
        {
            if (userInput.ToLower().Trim() == "exit")
            {
                Console.WriteLine("\n--- Thank You and Bye. Looking forward your next visit! ---\n");
                return true;
            }
            return false;
        }
    }
}