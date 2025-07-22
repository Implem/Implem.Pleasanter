[日本語版はこちら](CONTRIBUTING_JA.md)

# Development Environment Setup Guide

## Development on Windows with Visual Studio

This guide explains how to set up your environment for development using Visual Studio on Windows.

### 1. Tool Installation

#### Install Visual Studio 2022

Download and install Visual Studio 2022 from the following link:  
https://visualstudio.microsoft.com/downloads/

#### Install .NET 8 SDK

Download and install the latest .NET SDK 8.0 from the following link:
https://dotnet.microsoft.com/download/dotnet/8.0

#### Install Node.js

##### Install via the version management tool "VOLTA" (Recommended)
- If you have already installed the Node.js version management tool "VOLTA", the required version will be installed automatically during build.  
- If you are installing VOLTA for the first time, install it from the link below and get the latest Node.js.  
https://volta.sh/  
`volta install node@latest`  
**Note: If you already have Node.js installed or are using another version management tool, you need to uninstall them first.**

##### Install Node.js with a specific version
If you want to install Node.js directly, download it from the following link:  
https://nodejs.org/

Check the required version in the `volta` field of the following file:
```
Implem.PleasanterFrontend\wwwroot\package.json
```
#### Install VSCode (Optional)
If you are developing the frontend, please install VSCode:  
https://code.visualstudio.com/

### 2. Database Preparation
Pleasanter works with one of the following databases: SQLServer, PostgreSQL, or MySQL.
Choose the database you want to use in your development environment and follow the instructions below to install it.

- SQLServer: https://pleasanter.org/manual/install-sql-server2022-express
- PostgreSQL: https://pleasanter.org/manual/install-postgresql-on-windows
- MySQL: https://pleasanter.org/manual/install-mysql-on-windows

#### Set Connection Strings as Environment Variables
Set the database connection strings as environment variables, referring to the examples below.
- For {SA password}, set the administrator (superuser) password you specified during database installation.
- For {Owner password} and {User password}, set any password you like. These will be used to create each user.

Example for SQLServer:

|Environment Variable Name|Value|
|--|--|
|Implem.Pleasanter_Rds_SQLServer_SaConnectionString|Server=(local);Database=master;UID=sa;PWD={SA password};Connection Timeout=30;|
|Implem.Pleasanter_Rds_SQLServer_OwnerConnectionString|Server=(local);Database=#ServiceName#;UID=#ServiceName#_Owner;PWD={Owner password};Connection Timeout=30;|
|Implem.Pleasanter_Rds_SQLServer_UserConnectionString|Server=(local);Database=#ServiceName#;UID=#ServiceName#_User;PWD={User password};Connection Timeout=30;|

Example for PostgreSQL:

|Environment Variable Name|Value|
|--|--|
|Implem.Pleasanter_Rds_PostgreSQL_SaConnectionString|Server=localhost;Database=postgres;UID=postgres;PWD={SA password}|
|Implem.Pleasanter_Rds_PostgreSQL_OwnerConnectionString|Server=localhost;Database=#ServiceName#;UID=#ServiceName#_Owner;PWD={Owner password}|
|Implem.Pleasanter_Rds_PostgreSQL_UserConnectionString|Server=localhost;Database=#ServiceName#;UID=#ServiceName#_User;PWD={User password}|

Example for MySQL:

|Environment Variable Name|Value|
|--|--|
|Implem.Pleasanter_Rds_MySQL_SaConnectionString|Server=localhost;Database=mysql;UID=root;PWD={SA password}|
|Implem.Pleasanter_Rds_MySQL_OwnerConnectionString|Server=localhost;Database=#ServiceName#;UID=#ServiceName#_Owner;PWD={Owner password}|
|Implem.Pleasanter_Rds_MySQL_UserConnectionString|Server=localhost;Database=#ServiceName#;UID=#ServiceName#_User;PWD={User password}|

For more details on how to set connection strings, see the following link:  
https://pleasanter.org/manual/rds-json

### 3. Clone the Repository

Clone the [Implem.Pleasanter](https://github.com/Implem/Implem.Pleasanter) repository:

```bash
git clone https://github.com/Implem/Implem.Pleasanter
```

### 4. Install Frontend Development Environment
Run the following command in the `Implem.PleasanterFrontend\wwwroot` directory:
```
npm install
```

### 5. Build the Source Code

Open `Implem.Pleasanter.sln` in the root directory of the cloned repository with Visual Studio and build the solution.

### 6. Initialize the Database

1. In Solution Explorer, right-click the project "Implem.CodeDefiner" and select "Set as Startup Project".
2. In the debug profile, select "Implem.CodeDefiner_rds".
3. Start debugging the Implem.CodeDefiner project.
4. Follow the instructions displayed in the console to initialize the database.

### 7. Debug Run Pleasanter

1. In Solution Explorer, right-click the project "Implem.Pleasanter" and select "Set as Startup Project".
2. Start debugging the Implem.Pleasanter project.

### 8. Debug Run the Frontend

1. Open the working folder: `Implem.PleasanterFrontend\wwwroot` in VSCode.  
After opening the folder, a popup will appear asking to install VSCode extensions. Please install them as prompted.  
2. Start debugging by running `npm run dev` in the terminal.  
If necessary, select "Lunch" from the VSCode debug menu to start debugging.