# Mini Wallet Backend System

## Overview
Mini Wallet Backend System developed using ASP.NET Core Web API,
Entity Framework Core and SQL Server.

## Technologies
- .NET 8
- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger
- Postman

## Project Structure

MiniWalletBackendSystem
- Controllers
- Services
- DTOs
- Models
- Data

## Database Setup
1. Open SQL Server Management Studio
2. Execute:Database/MiniWalletDB.sql
3. Update connection string in:appsettings.json
   
## Run Project
Open solution:MiniWalletBackendSystem.sln

Run:F5

Swagger: https://localhost:xxxx/swagger

## APIs

### Wallet

POST
/api/wallet/create

GET
/api/wallet/{walletId}/balance

GET
/api/wallet/{walletId}/transactions

### Transactions

POST
/api/transaction/credit

POST
/api/transaction/debit

POST
/api/transaction/transfer

## Duplicate Transaction Handling

ReferenceId is unique.
Duplicate credit/debit/transfer requests are rejected.

## Negative Balance Prevention

Debit and transfer operations validate available balance
before updating wallet.

## Concurrent Transaction Handling

Serializable database transaction isolation is used
for debit and transfer operations.

## Performance Optimization

- Async database calls
- EF Core AsNoTracking for reads
- Database indexes
- Pagination
- Minimal database calls
