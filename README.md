[![GitHub stars](https://img.shields.io/github/stars/implem/implem.pleasanter)](https://github.com/implem/implem.pleasanter/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/implem/implem.pleasanter)](https://github.com/implem/implem.pleasanter/network)
[![GitHub issues](https://img.shields.io/github/issues/implem/implem.pleasanter)](https://github.com/implem/implem.pleasanter/issues)
[![GitHub license](https://img.shields.io/github/license/implem/implem.pleasanter)](https://github.com/implem/implem.pleasanter/blob/master/LICENSE)
[![Release](https://img.shields.io/github/v/release/implem/implem.pleasanter?label=release&logo=github&style=flat-square)](https://github.com/implem/implem.pleasanter/releases/latest)
[![Twitter Follow](https://img.shields.io/twitter/follow/pleasanter_oss?style=social)](https://twitter.com/pleasanter_oss)

![Pleasanter logo](Implem.Pleasanter/wwwroot/images/logo-version.png)

## 1. Overview

Pleasanter is an open-source, .NET-based development platform
for building business applications
using **no-code and low-code approaches**.
Pleasanter allows rapid creation of internal systems such as:

* CRM
* SFA
* Issue tracking
* Project management
* Document management
* Image databases

The platform provides extensibility through **scripts, APIs, and SQL**,
enabling integration with existing enterprise systems.

## 2. Key Features

* Application development
  * No-code business application development
  * Low-code customization using scripts
  * REST API for integration
* Data management
  * Table-based data model
  * Table relationships
  * Data history tracking
  * Status management and workflow
* Visualization
  * Calendar
  * Gantt chart
  * Burndown chart
  * Timeseries
  * Crosstab
  * Kanban board
  * Image library
* Collaboration
  * Email notifications
  * Chat integration (Slack, Teams, etc.)
* Security
  * Access control at table / row / column level
  * LDAP authentication
  * SAML authentication
  * Two-factor authentication
  * Passkey authentication
* Internationalization (Supported languages)
  * English
  * Japanese
  * Chinese
  * German
  * Korean
  * Spanish
  * Vietnamese
* Extensibility
  * Front-end JavaScript / CSS extensions
  * Server-side JavaScript extensions
  * SQL extensions
  * API integration

## 3. Requirements

| Component | Supported                       |
| :-------- | :------------------------------ |
| OS        | Windows / Linux                 |
| Framework | .NET 8 (LTS) / .NET 10 (LTS)    |
| Database  | SQL Server / PostgreSQL / MySQL |

## 4. Getting Started

This section describes how to run Pleasanter locally for evaluation.

### 4.1. Start with Docker

Ensure Docker is installed and running before proceeding.  
If necessary, run Docker commands with sudo. We use PostgreSQL as a backend database.

1. Create a directory named "pleasanter" in any location and store two files there.

      ```text
      pleasanter/
      ├── .env
      └── compose.yaml
      ```

1. Configure `.env`

    `"{{ ... }}"` should be modified accordingly.

    ```conf
    POSTGRES_VERSION={{PostgreSQL Version}}
    POSTGRES_VOLUMES_TARGET=/var/lib/postgresql/data
    POSTGRES_USER={{Sa User}}
    POSTGRES_PASSWORD={{Sa Password}}
    POSTGRES_DB={{System DB}}
    POSTGRES_HOST_AUTH_METHOD=scram-sha-256
    POSTGRES_INITDB_ARGS=--encoding=UTF-8
    PLEASANTER_VERSION={{Pleasanter Version}}
    Implem_Pleasanter_Rds_PostgreSQL_SaConnectionString='Server=db;Database={{System DB}};UID={{Sa User}};PWD={{Sa password}}'
    Implem_Pleasanter_Rds_PostgreSQL_OwnerConnectionString='Server=db;Database=#ServiceName#;UID=#ServiceName#_Owner;PWD={{Owner password}}'
    Implem_Pleasanter_Rds_PostgreSQL_UserConnectionString='Server=db;Database=#ServiceName#;UID=#ServiceName#_User;PWD={{User password}}'
    ```

1. Configure `compose.yaml`

    ```yaml
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
            target: ${POSTGRES_VOLUMES_TARGET}
        healthcheck:
          test:
            [
              "CMD-SHELL",
              "pg_isready -U $${POSTGRES_USER:-postgres} -d $${POSTGRES_DB:-postgres} || exit 1",
            ]
          interval: 10s
          timeout: 5s
          retries: 5
          start_period: 30s
        restart: unless-stopped
      pleasanter:
        container_name: pleasanter
        image: implem/pleasanter:${PLEASANTER_VERSION}
        restart: unless-stopped
        depends_on:
          db:
            condition: service_healthy
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
          db:
            condition: service_healthy
        environment:
          Implem.Pleasanter_Rds_PostgreSQL_SaConnectionString: ${Implem_Pleasanter_Rds_PostgreSQL_SaConnectionString}
          Implem.Pleasanter_Rds_PostgreSQL_OwnerConnectionString: ${Implem_Pleasanter_Rds_PostgreSQL_OwnerConnectionString}
          Implem.Pleasanter_Rds_PostgreSQL_UserConnectionString: ${Implem_Pleasanter_Rds_PostgreSQL_UserConnectionString}
    volumes:
      pg_data:
        name: ${COMPOSE_PROJECT_NAME:-default}_pg_data_volume
    ```

1. Pull images

    ```shell
    docker compose pull
    ```

1. Initialize the database

    Run CodeDefiner to initialize the database schema.
    `"{{ ... }}"` should be modified accordingly.

    ```shell
    docker compose run --rm codedefiner _rds /l "{{Language}}" /z "{{Timezone}}"
    ```

    To suppress prompts, run with the `/y` option.

    ```shell
    docker compose run --rm codedefiner _rds /l "ja" /z "Asia/Tokyo" /y
    ```

1. Start Pleasanter

    ```shell
    docker compose up -d pleasanter
    ```

    Access Pleasanter:

    ```text
    http://localhost:50001
    ```

    Default login:

    | Login ID      | Password   |
    | :-----------: | :--------: |
    | Administrator | pleasanter |

    For security reasons, change the administrator password after the first login.

1. Stop the system

    ```shell
    docker compose down
    ```

### 4.2. Start with Demo

You can try the demo just by registering your email address:  
[Register Demo](https://pleasanter.org/demo) (Japanese only)

## 5. Documentation

Official documentation: [User Manual](https://pleasanter.org/manual) (Partially available in English)

## 6. Case studies

Examples of real deployments: [User case studies](https://pleasanter.org/cases) (Japanese only)

## 7. Contributing

Please see: [CONTRIBUTING.md](CONTRIBUTING.md) for development environment setup and contribution guidelines.

## 8. License

This project is licensed under the [AGPL-3.0 license.](https://github.com/Implem/Implem.Pleasanter/blob/main/LICENSE)

## 9. Authors

Developed by [IMPLEM Inc.](https://implem.co.jp)
