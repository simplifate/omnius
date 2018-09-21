Omnius
======
A web based low-code application platform.

Installation instructions
-------------------------

1 - Clone the repository
2 - Open it in Visual Studio
3 - Set your database connection string as DefaultConnection on line 191 in Web.config (supported database engines are MySQL, MariaDB and MS SQL Server)
4 - Run migrations on the Modules project (this will create a default account and a sample application)
5 - Set Frontend as startup project and run it

### Default account credentials
User: `admin`  
Pass: `pass132`

Features
--------

Omnius allows you to define application definitions, and then build them and run them on the platform. If you log in under an admin account, you will see a gear icon in the top left. Clicking it will open the admin sections, where applications can be designed, deployed and configured. The application definitions have 3 parts: database scheme, UI page definitions, and workflow definitions. Database module is called Entitron, UI designer is under Mozaic, and workflow definitions can be found under Tapestry. You can edit parts of the application and build it from the Application manager.
