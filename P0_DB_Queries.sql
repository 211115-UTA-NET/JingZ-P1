-- CREATE DATABASE P0_StoreApp;
/*
DROP TABLE Customer;
DROP TABLE StoreInventory;
DROP TABLE CustomerOrder;
DROP TABLE OrderProduct;
*/
-- create tables
CREATE TABLE Customer(
    ID INT NOT NULL IDENTITY(100, 1) PRIMARY KEY,
    FirstName NVARCHAR(16) NOT NULL,
    LastName NVARCHAR(16) NOT NULL
);

CREATE TABLE CustomerOrder(
    OrderNum INT NOT NULL IDENTITY PRIMARY KEY,
    CustomerID INT NOT NULL     --FK
);

CREATE TABLE Location(
    ID INT NOT NULL IDENTITY PRIMARY KEY,
    StoreLocation NVARCHAR(168) NOT NULL 
);

CREATE TABLE OrderProduct(
    OrderNum INT NOT NULL,                 --PK + FK
    ProductName NVARCHAR(30) NOT NULL,     --PK
    Amount INT NOT NULL,        -- need CHECK Constrain will added later
    LocationID INT NOT NULL,    -- FK
    OrderTime DATETIMEOFFSET NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    PRIMARY KEY (OrderNum, ProductName)
);

--DROP TABLE StoreInventory
CREATE TABLE StoreInventory(
    LocationID INT NOT NULL,   --PK + FK
    ProductName NVARCHAR(30) NOT NULL,      --PK
    Price DECIMAL(9, 2) NOT NULL,
    ProductAmount INT NOT NULL,
    PRIMARY KEY (LocationID, ProductName)
);

-- Add CHECK constrain
ALTER TABLE StoreInventory ADD CONSTRAINT CK_Price CHECK (Price >= 0);
ALTER TABLE StoreInventory ADD CONSTRAINT CK_ProductAmount CHECK (ProductAmount >= 0);
ALTER TABLE OrderProduct ADD CONSTRAINT CK_OrderAmount CHECK (Amount >= 0);
ALTER TABLE OrderProduct ADD CONSTRAINT CK_OrderAmount2 CHECK (Amount < 100);

ALTER TABLE StoreInventory DROP CONSTRAINT CK_Price
ALTER TABLE StoreInventory DROP CONSTRAINT CK_ProductAmount
ALTER TABLE OrderProduct DROP CONSTRAINT CK_OrderAmount

-- Add foreign key constraint
ALTER TABLE CustomerOrder ADD CONSTRAINT FK_Customer_ID 
    FOREIGN KEY (CustomerID) REFERENCES Customer(ID);

ALTER TABLE OrderProduct ADD CONSTRAINT FK_Order_Num 
    FOREIGN KEY (OrderNum) REFERENCES CustomerOrder(OrderNum);

ALTER TABLE OrderProduct ADD CONSTRAINT FK_Order_LocationID
    FOREIGN KEY (LocationID) REFERENCES Location(ID);
    
ALTER TABLE StoreInventory ADD CONSTRAINT FK_Inventory_LocationID 
    FOREIGN KEY (LocationID) REFERENCES Location(ID);
-----------------------------------
SELECT * FROM Location;
SELECT * FROM StoreInventory;
-- Insert Location
-- UPDATE Location SET StoreLocation='1551 3rd Ave, New York, NY 10128' WHERE ID =1;
INSERT Location VALUES ('1551 3rd Ave, New York, NY 10128');			-- ID=1
INSERT Location VALUES ('367 Russell St, Hadley, MA 01035');			-- ID=2
INSERT Location VALUES ('613 Washington Blvd, Jersey City, NJ 07310');  -- ID=3


-- Insert Inventory
-- delete from StoreInventory where LocationID=1;
INSERT StoreInventory VALUES 
(1, 'Gel Ink Cap Type Ballpoint Pen', 1.50, 100),
(1, 'Paper Sketch Book', 3.90, 300),
(1, 'Bind Plain Pocket Notebook', 1.90, 100),
(1, 'Double Ringed Plain Notebook', 1.90, 200),
(1, 'Standard File Box', 10.90, 100),
(1, 'Handy Shredder', 14.90, 90);

INSERT StoreInventory VALUES 
(2, 'Highlighter', 1.90, 200),
(2, 'Paper Sketch Book', 3.90, 100),
(2, 'Aluminum Fountain Pen', 15.90, 50),
(2, 'Double Ringed Plain Notebook', 1.90, 100),
(2, 'Eraser', 0.50, 300),
(2, 'Scissors', 4.90, 70),
(2, 'Pen Case', 8.90, 100);

INSERT StoreInventory VALUES 
(3, 'Stapler', 4.90, 200),
(3, 'Sticky Index', 2.90, 100),
(3, 'Standard File Box', 10.90, 100),
(3, 'Tape Dispenser', 9.90, 100),
(3, 'Masking Tape', 1.9, 250);

SELECT ProductName, Price FROM StoreInventory WHERE LocationID = 1 ORDER BY Price;

-- INSERT Customer VALUES (__, __);
SELECT * FROM Customer;
DELETE FROM Customer WHERE ID=104;

SELECT * FROM Customer WHERE ID = 106;

-- when make a order
INSERT INTO CustomerOrder VALUES (106)
SELECT MAX(OrderNum) AS OrderNum From CustomerOrder WHERE CustomerID = 106;
DELETE FROM CustomerOrder WHERE OrderNum = 8
SELECT * FROM CustomerOrder;
SELECT OrderNum FROM CustomerOrder where CustomerID = 106; -- get order number

-- inser order info
-- amount must < 100
INSERT OrderProduct (OrderNum, ProductName, Amount, LocationID) VALUES (9,'Masking Tape', 5, 3);
UPDATE OrderProduct set ProductName='stapler' WHERE OrderNum = 2;

-- get price from inventory
SELECT OrderNum, OrderProduct.ProductName, Amount, StoreInventory.LocationID
FROM OrderProduct
INNER JOIN StoreInventory ON OrderProduct.LocationID = StoreInventory.LocationID
WHERE StoreInventory.LocationID = 3 
AND OrderProduct.OrderNum = 2 AND StoreInventory.ProductName = 'Stapler'

SELECT * FROM OrderProduct WHERE OrderNum = 2;


SELECT * FROM CustomerOrder
SELECT * FROM OrderProduct
SELECT * FROM StoreInventory

-- check amount in the inventory vs order amount
SELECT Amount FROM OrderProduct WHERE OrderNum=2 AND ProductName = 'Stapler';
SELECT ProductAmount From StoreInventory Where LocationID = 3 AND ProductName='Stapler';

-- update inventory
UPDATE StoreInventory 
SET 
ProductAmount 
= ProductAmount - (SELECT Amount FROM OrderProduct WHERE OrderNum=2 AND ProductName = 'Stapler')
WHERE LocationID = 3 AND ProductName='Stapler';
SELECT * FROM StoreInventory;

-- get price
SELECT Price FROM StoreInventory WHERE LocationID = 2 AND ProductName='Eraser'

SELECT * FROM Customer WHERE FirstName='John' AND LastName='Doe';

-- display all
SELECT * FROM Customer
SELECT * FROM Location
SELECT * FROM CustomerOrder
SELECT * FROM OrderProduct
SELECT * FROM StoreInventory where locationID=3 order by price;

SELECT OrderNum, ProductName, Amount, LocationID, OrderTime, StoreLocation FROM OrderProduct, Location WHERE LocationID = Location.ID AND OrderNum =35


-- diaplay all orders from the location of the customer
SELECT OrderProduct.OrderNum, ProductName, Amount, LocationID, Location.StoreLocation, OrderTime FROM CustomerOrder 
INNER JOIN OrderProduct ON CustomerOrder.OrderNum = OrderProduct.OrderNum 
INNER JOIN Location ON LocationID = Location.ID
WHERE CustomerID = 107 AND LocationID=2 ORDER BY OrderProduct.OrderNum;

-- display all orders of this customer
SELECT * FROM CustomerOrder
SELECT * FROM OrderProduct
SELECT OrderProduct.OrderNum, ProductName, Amount, Location.ID, Location.StoreLocation, OrderTime FROM CustomerOrder 
INNER JOIN OrderProduct ON CustomerOrder.OrderNum = OrderProduct.OrderNum 
INNER JOIN Location ON LocationID = Location.ID
WHERE CustomerID = 107 ORDER BY OrderProduct.OrderNum;

-- display a specific order of a customer by order#
SELECT * FROM CustomerOrder
SELECT * FROM OrderProduct
SELECT OrderProduct.OrderNum, ProductName, Amount, Location.ID, Location.StoreLocation, OrderTime FROM CustomerOrder 
INNER JOIN OrderProduct ON CustomerOrder.OrderNum = OrderProduct.OrderNum 
INNER JOIN Location ON LocationID = Location.ID
WHERE CustomerID = 106 AND CustomerOrder.OrderNum=29;

-- by most recent
SELECT MAX(OrderNum) AS OrderNum From CustomerOrder WHERE CustomerID = 106;

SELECT OrderProduct.OrderNum, ProductName, Amount, Location.ID, Location.StoreLocation, OrderTime FROM CustomerOrder 
INNER JOIN OrderProduct ON CustomerOrder.OrderNum = OrderProduct.OrderNum 
INNER JOIN Location ON LocationID = Location.ID
WHERE CustomerID = 106 AND 
CustomerOrder.OrderNum
=(SELECT MAX(OrderNum) AS OrderNum From CustomerOrder WHERE CustomerID = 106);

-- empty up tables
DELETE FROM CustomerOrder WHERE CustomerID = 109
DELETE FROM OrderProduct WHERE OrderNum = 35
DELETE FROM Customer WHERE ID=110