[![GitHub stars](https://img.shields.io/github/stars/implem/implem.pleasanter)](https://github.com/implem/implem.pleasanter/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/implem/implem.pleasanter)](https://github.com/implem/implem.pleasanter/network)
[![GitHub issues](https://img.shields.io/github/issues/implem/implem.pleasanter)](https://github.com/implem/implem.pleasanter/issues)
[![GitHub license](https://img.shields.io/github/license/implem/implem.pleasanter)](https://github.com/implem/implem.pleasanter/blob/master/LICENSE)
[![Release](https://img.shields.io/github/v/release/implem/implem.pleasanter?label=release&logo=github&style=flat-square)](https://github.com/implem/implem.pleasanter/releases/latest)
[![Twitter Follow](https://img.shields.io/twitter/follow/pleasanter_oss?style=social)](https://twitter.com/pleasanter_oss)

![image](Implem.Pleasanter/wwwroot/images/logo-version.png)

## Overview

Pleasanter is a development platform that utilizes both no-code and low-code approaches, operating on the .NET. With its simple operations, it allows for the swift creation of business applications, enabling prompt responses to any variations within the business. It boasts an abundance of features for developers, enabling seamless integration with existing systems and robust scalability through its powerful script functions and APIs. The desired business application can be constructed with greater ease and speed, as opposed to starting from scratch.

## Features
- Develop business applications with no-code
- Fast and stress-free UI
- Can be used as CRM, SFA, project management, etc
- Can be used as an image database, document database, etc
- Searching, Sorting, and Aggregating Table Records
- Relations between tables
- Data history management
- Status management and workflow
- CSV import and export
- Calendar and data chart(Gantt/Burndown/Timeseries/Crosstab/Kanban)
- Email and chat notifications(Slack/Teams/etc)
- Granular access control by table, row, and column
- LDAP and SAML authentication
- Multilingual(English/Chinese/Japanese/German/Korean/Spanish/Vietnamese)
- Extension with add-on JavaScript/CSS that works on the front end
- Extension with add-on JavaScript that works on the server side
- Extend with add-on SQL
- API

## Quick Start

### Run with Docker

First, please make sure that Docker is available :)

1. Environment variables

   Set the required environment variables. It is easy to write them in your `.env` file.
   Example:

   ```shell
    POSTGRES_USER=postgres
    POSTGRES_PASSWORD=<Any Sa password>
    POSTGRES_DB=postgres
    Implem_Pleasanter_Rds_PostgreSQL_SaConnectionString='Server=db;Database=postgres;UID=postgres;PWD=<Any Sa password>'
    Implem_Pleasanter_Rds_PostgreSQL_OwnerConnectionString='Server=db;Database=#ServiceName#;UID=#ServiceName#_Owner;PWD=SetAdminsPWD'
    Implem_Pleasanter_Rds_PostgreSQL_UserConnectionString='Server=db;Database=#ServiceName#;UID=#ServiceName#_User;PWD=SetUsersPWD'
    DOCKER_BUILDKIT=1
    COMPOSE_DOCKER_CLI_BUILD=true
   ```

2. Build

   ```shell
   docker compose build
   ```

3. Run CodeDefiner

   ```shell
   docker compose run --rm --name codedefiner Implem.CodeDefiner _rds
   ```

4. Start Pleasanter

   ```shell
   docker compose run --rm -d -p 50001:80 --name pleasanter Implem.Pleasanter
   ```

   `50001` in `-p` is the port of the site when accessing. (Change it as necessary)
   Accessing the site at <http://localhost:50001/>

5. Terminate

   ```shell
   docker compose down
   ```

### Demonstration

Click [here](https://demo.pleasanter.org) to enter your email address and start the demo.

## Requirements

Pleasanter works on it`.NET6`. A Database can be PostgreSQL or SQL Server.

|item|choice|
|:----|:----|
|OS|Windows / Linux|
|Framework|.NET6|
|Database|PostgreSQL / SQL Server|

## Documentation
You can learn installation procedures, parameter settings, operation methods, etc.
[User's guide](https://pleasanter.org/manual) (Japanese only)

## Case study
Introduction to large companies, local governments, financial institutions, etc. is increasing.
[User case studies](https://pleasanter.org/cases) (Japanese only)

## License
This program is under the terms of the [AGPL-3.0 license.](https://github.com/Implem/Implem.Pleasanter/blob/main/LICENSE)

## Authors
Pleasanter is developed by [IMPLEM Inc.](https://implem.co.jp)
