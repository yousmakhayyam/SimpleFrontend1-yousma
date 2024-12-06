# Warehouse

### Technology

 - Blazor .Net 8
 - Bootstrap v5
 - MS Sql DB

### Setup dev env
 - Setup SQL database (for example in docker)
   
 > [!TIP]
 > You can easly deploy sql server to docker with azure data studio

 - Update connection string in appsettings.Development.json
   - If file doesn't exists, just create it
 - Startup Warehouse.Web project

### Create migration script
 - Run this command to create new migration script to update Database:
   - dotnet ef migrations add **NAME OF MIGRATION** --project Warehouse.Domain --startup-project Warehouse.Web --output-dir Data/Migrations
 - Run project to apply migration or run this command to manually apply migration:
   - dotnet ef database update --project Warehouse.Domain --startup-project Warehouse.Web

> [!IMPORTANT]
> Default admin user is created upon starting of project, login: admin@local password: zaq1@WSX


### Functionality
 - [x] Login
 - [x] Register new user
 - [x] Register new company
 - Home page:
   - [x] Show quick actions for authorized and non authorized users
   - [x] Show newest products
 - Admin user view
   - [x] list of users
   - [x] create/edit admin user
 - Products:
   - [x] Create new Product
   - [x] Edit Product
   - [x] List of all Products
   - [x] Show details
   - [x] Filter by products created by current user
 - Profile page:
   - [x] Show user informations
   - [x] Show newest added products by the user
   - [ ] Update user data 
 - Company dashboard:
   - [x] Company informations
   - [x] Company newest products
   - [x] Company user list
   - [x] create/edit company user

 #### Additionall Functionality on pages
 - [x] Pagination
 - [x] Searching by text
 - [ ] Support for multi-language
