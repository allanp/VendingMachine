Readme.txt

Author: Tianhua Piao
Email: magicpiao@gmail.com
Date: 2012-03-27

=================================================================================
Table of Contents

Project structure;
The file structure;
Application description;

=================================================================================
Project structure:

The solution consists of two projects: 
  VendingMachine.Core: application library; and,
  VendingMachine.Core.Test. the unit test project.

  *** NOTE:
  ***   You cannot run the application directly, since there is no application 
  ***   "Main" entry method, instead, in the current solution, it runs in the test
  ***   project. However, you can always include the project and use the classes
  ***   in your own solution, and extents its the feature.

=================================================================================
The file structure:

+ VendingMachine.Core
  - Boxes.cs:     a generic abstract class, StoreBox<T>, and its children classes, 
                  ProductBox and CoinBox
  - Elements.cs:  the basic elements of the application, Product and Coin structs.
  - Machine.cs:   the main Machine class.
  - Messenger.cs: the messenger interface and a default implementation.
+ VendingMachine.Core.Test
  - BoxesTest.cs
  - MachineTest.cs
README.TXT: This file

=================================================================================
Application description:

A typical vending machine contains:
  PBox:      product box, which contains a number of different types of products.
  SBox:      store-coin box, which contains the coins were stored in the machine.
  RBox:      receive-coin box, which contains the coins were inserted by a customer.
  Messenger: displays messages to customers.

Each type of products has its name and price (read only) properties.
Each type of coins has value (read only) property.

All xBox(x = [Product|Coin]) classes are inherited from the StoreBox<T> abstract class, 
where T is the struct type of the items could be stored in the box, such as products, 
or coins. The struct type is used because it is valuetype, and ease to use them as the
key in a dictionary, which is used in the StoreBox<T> internally to contain items.
Each type of items is also associated with an counter which represents the "how many" 
are in the box.

When starting the application, a machine should be created with a PBox, a SBox, a RBox 
and a messenger.
  the PBox should be filled with products;
  the SBox should be filled with some initial coins; 
  the RBox should be empty; 
  the messenger should implement IMessenger interface.

A use can call the InsterCoin method to insert a coin into the RBox. 
And then, call DoPurchase method to purchase a product, the method returns true if succeed, 
otherwise returns false, and display an error message. If DoPurchase succeeds, the selected 
product and the changes, if any, are also delivered to the customer.

When making changes, 
1: the machine first sum all the coins in both RBox and SBox;
2: sort the coins from greatest value to lowest value;
3: make combination of coins for the changes, try from the greatest value coins to lowest value coins.
If one of the following occurs, add the coins with lower value:
 3.1 The value of current coin is greater than the rest of the changes;
 3.2 The number of current coin is zero.
*At here, we can use mod operation to calculate how many coins of each value, or adding one coin in each round.

The make changes method, "CoinBox OnMakeChanges(int changesValue)" could also be overrided if
better implementation of making changes is known.

This process of making combination of coins for changes continues until:
 - The changes has made, or
 - The changes cannot made because there is not enough coins left in the machine.

If the changes can be made, deliver the changes and the product.
If the changes cannot be made, display the reason and deliver the coins in the RBox, if any, and clear the RBox.

-- End of file --