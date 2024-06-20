# Medicine System Design

## How to use

1. Open `WebDesign.sql` and then create a database named `Drug System`

2. Create four Table which is called `Customer`、`Supplier`、`Medicine` and `Order`

3. Create two Trigger which is called `update_medicine_quantity` and `restore_medicine_quantity`

4. Go into the `main_code` directory, and then click on `MVC_Test.sln` to open the project with Visual Studio

5. Connect to the database on your local machine using the command below.

   ``` 
   Scaffold-DbContext "persist security info=True;data source=localhost;port=3306;initial catalog=drug_system;user id=root;password=123456;character set=utf8;allow zero datetime=true;convert zero datetime=true;pooling=true;maximumpoolsize=3000" Pomelo.EntityFrameworkCore.MySql -OutputDir Models -f
   ```

   NOTICE:  `user_id` is your database username and `password` is your database password.  

6. Final step is to launch the project.