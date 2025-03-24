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
If necessary, run the Docker command with sudo when executing it.

1. Create directory and files
   - Create the following directories and files.
      ```
      pleasanter/  
      ├── .env  
      └── compose.yaml
      ```
2. Edit files
   - Edit each file as follows. “{{ ... }}” should be modified accordingly.
     - .env
        ```
        POSTGRES_VERSION={{PostgreSQL Version}}
        POSTGRES_USER={{Sa User}}
        POSTGRES_PASSWORD={{Sa Password}}
        POSTGRES_DB={{System DB}}
        POSTGRES_HOST_AUTH_METHOD=scram-sha-256
        POSTGRES_INITDB_ARGS="--auth-host=scram-sha-256 --encoding=UTF-8"
        PLEASANTER_VERSION={{Pleasanter Version}}
        Implem_Pleasanter_Rds_PostgreSQL_SaConnectionString='Server=db;Database={{System DB}};UID={{Sa User}};PWD={{Sa password}}'
        Implem_Pleasanter_Rds_PostgreSQL_OwnerConnectionString='Server=db;Database=#ServiceName#;UID=#ServiceName#_Owner;PWD={{Owner password}}'
        Implem_Pleasanter_Rds_PostgreSQL_UserConnectionString='Server=db;Database=#ServiceName#;UID=#ServiceName#_User;PWD={{User password}}'
        ```

     - compose.yaml
         ```
         services:
           db:
             container_name: postgres
             image: postgres:${POSTGRES_VERSION}
             environment:
               - POSTGRES_USER
               - POSTGRES_PASSWORD
               - POSTGRES_DB
               - POSTGRES_HOST_AUTH_METHOD
               - POSTGRES_INITDB_ARGS
             volumes:
               - type: volume
                 source: pg_data
                 target: /var/lib/postgresql/data
           pleasanter:
             container_name: pleasanter
             image: implem/pleasanter:${PLEASANTER_VERSION}
             depends_on:
               - db
             ports:
               - '50001:8080'
             environment:
               Implem.Pleasanter_Rds_PostgreSQL_SaConnectionString: ${Implem_Pleasanter_Rds_PostgreSQL_SaConnectionString}
               Implem.Pleasanter_Rds_PostgreSQL_OwnerConnectionString: ${Implem_Pleasanter_Rds_PostgreSQL_OwnerConnectionString}
               Implem.Pleasanter_Rds_PostgreSQL_UserConnectionString: ${Implem_Pleasanter_Rds_PostgreSQL_UserConnectionString}
           codedefiner:
             container_name: codedefiner
             image: implem/pleasanter:codedefiner
             depends_on:
               - db
             environment:
               Implem.Pleasanter_Rds_PostgreSQL_SaConnectionString: ${Implem_Pleasanter_Rds_PostgreSQL_SaConnectionString}
               Implem.Pleasanter_Rds_PostgreSQL_OwnerConnectionString: ${Implem_Pleasanter_Rds_PostgreSQL_OwnerConnectionString}
               Implem.Pleasanter_Rds_PostgreSQL_UserConnectionString: ${Implem_Pleasanter_Rds_PostgreSQL_UserConnectionString}
         volumes:
           pg_data:
             name: ${COMPOSE_PROJECT_NAME:-default}_pg_data_volume
         ```
3. Get image
   ```
   docker compose pull
   ```

4. Run CodeDefiner

   ```shell
   docker compose run --rm codedefiner _rds /l "<language>" /z "<timezone>"
   ```

   - example
      ```
      docker compose run --rm codedefiner _rds /l "ja" /z "Asia/Tokyo"
      ```

> [!NOTE]
> In version 1.4.8.0, the behavior of the CodeDefiner's _rds command has been changed.
> It now prompts for the license and confirms execution.
> When running with Docker, the prompt can be skipped by adding the /y option.
> Note that this is equivalent to typing (Y)es at the prompt.

1. Start Pleasanter

   ```shell
   docker compose up -d pleasanter
   ```

   `50001` in `-p` is the port of the site when accessing. (Change it as necessary)
   Accessing the site at <http://localhost:50001/>.

   When you access the site, you will be asked to log in. Enter the initial user name: `Administrator` and initial password: `pleasanter` to log in.

1. Terminate

   ```shell
   docker compose down
   ```

### Demonstration

Click [here](https://demo.pleasanter.org) to enter your email address and start the demo.

## Requirements

Pleasanter work on it`.NET8`. A Database can be PostgreSQL or SQL Server or MySQL.

|item|choice|
|:----|:----|
|OS|Windows / Linux|
|Framework|.NET8|
|Database|PostgreSQL / SQL Server / MySQL|

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
