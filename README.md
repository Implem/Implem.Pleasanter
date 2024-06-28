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

1. Create docker network

   ```shell
   docker network create pleasanter-net
   ```

1. Run PostgreSQL

   ```shell
   docker run --rm -d \
       --network pleasanter-net \
       --name db \
       --env POSTGRES_USER=postgres \
       --env POSTGRES_PASSWORD=<Any Sa password> \
       --env POSTGRES_DB=postgres \
       postgres:15
   ```

1. Environment variables

   Set the required environment variables. It is easy to write them in your `env-list` file.
   Example:

   ```text
    Implem.Pleasanter_Rds_PostgreSQL_SaConnectionString=Server=db;Database=postgres;UID=postgres;PWD=<Any Sa password>
    Implem.Pleasanter_Rds_PostgreSQL_OwnerConnectionString=Server=db;Database=#ServiceName#;UID=#ServiceName#_Owner;PWD=<Any Owner password>
    Implem.Pleasanter_Rds_PostgreSQL_UserConnectionString=Server=db;Database=#ServiceName#;UID=#ServiceName#_User;PWD=<Any User password>
   ```

1. Run CodeDefiner

   ```shell
   docker run --rm --network pleasanter-net \
       --name codedefiner \
       --env-file env-list \
       implem/pleasanter:codedefiner _rds
   ```

1. Start Pleasanter

   ```shell
   docker run --rm --network pleasanter-net \
       --name pleasanter \
       --env-file env-list \
       -p 50001:8080 \
       implem/pleasanter
   ```

   `50001` in `-p` is the port of the site when accessing. (Change it as necessary)
   Accessing the site at <http://localhost:50001/>.

   When you access the site, you will be asked to log in. Enter the initial user name: `Administrator` and initial password: `pleasanter` to log in.

   If you want to stop pleasanter container, press Ctrl-C.

1. Terminate

   ```shell
   docker stop db
   docker network rm pleasanter-net
   ```

### Demonstration

Click [here](https://demo.pleasanter.org) to enter your email address and start the demo.

## Requirements

Pleasanter work on it`.NET8`. A Database can be PostgreSQL or SQL Server.

|item|choice|
|:----|:----|
|OS|Windows / Linux|
|Framework|.NET8|
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
