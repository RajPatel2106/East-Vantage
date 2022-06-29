Hello, there.
I've created an API for assigning the Work order to a Technician.
I've added swagger to communicate with API methods.
This API has been created with the code-first approach of entity framework.
If you clone this API to your pc make sure to change the connection string in the web.config file and provide your DB's access and do the migration to add tables.
There are two tables in DB, "tbl_WorkOrder" and "tbl_Technician".
In tbl_WorkOrder "WorkOrderId" is a primary key plus auto incrementd and has the "TechnicianId" as the foreign key.
In tbl_Technician "TechnicianId" is a primary key but not auto incremented.
This API can handle the exception and also log the data in "Log" folder.
Thank you for reading.
