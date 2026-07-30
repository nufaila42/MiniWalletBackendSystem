# Mini Wallet Backend System

## Project Overview

Mini Wallet Backend System is a REST API-based wallet management application developed using **ASP.NET Core Web API, C#, Entity Framework Core, and SQL Server**.

The objective of this project is to implement a secure and reliable wallet backend system that supports:

* Creating user wallets
* Adding money (Credit)
* Removing money (Debit)
* Wallet-to-wallet transfer
* Checking wallet balance
* Viewing transaction history

The system focuses on:

* Correct wallet balance management
* Transaction consistency
* Duplicate transaction prevention
* Concurrent transaction handling
* Database transaction safety
* Clean API design
* Performance optimization

---

# Technology Stack

| Component            | Technology                          |
| -------------------- | ----------------------------------- |
| Backend Framework    | ASP.NET Core Web API (.NET 8)       |
| Programming Language | C#                                  |
| ORM                  | Entity Framework Core               |
| Database             | SQL Server                          |
| Database Tool        | SQL Server Management Studio (SSMS) |
| API Testing          | Postman                             |
| API Documentation    | Swagger                             |
| IDE                  | Visual Studio 2022                  |
| Architecture         | Service Layer Pattern               |

---

# Development Environment Setup

## Required Software

Install the following tools:

1. Visual Studio 2022

2. .NET 8 SDK

3. SQL Server

4. SQL Server Management Studio (SSMS)

5. Postman

---

# Project Structure

```
MiniWalletBackendSystem

│
├── MiniWallet.API
│
│
├── Controllers
│   │
│   ├── WalletController.cs
│   └── TransactionController.cs
│
│
├── Services
│   │
│   ├── IWalletService.cs
│   └── WalletService.cs
│
│
├── DTOs
│
│   ├── CreateWalletRequest.cs
│   ├── CreditRequest.cs
│   ├── DebitRequest.cs
│   └── TransferRequest.cs
│
│
├── Models
│
│   ├── Wallet.cs
│   └── WalletTransaction.cs
│
│
├── Data
│
│   └── MiniWalletDbContext.cs
│
│
├── Database
│
│   └── MiniWalletDB.sql
│
│
├── Postman
│
│   └── MiniWalletBackendSystem.postman_collection.json
│
│
└── README.md
```

---

# How to Run the Project Locally

## Step 1: Clone Repository

Clone the repository:

```
git clone https://github.com/nufaila42/MiniWalletBackendSystem.git
```

Navigate:

```
cd MiniWalletBackendSystem
```

---

## Step 2: Open Project

Open:

```
MiniWalletBackendSystem.sln
```

using:

```
Visual Studio 2022
```

---

## Step 3: Restore Dependencies

Open Visual Studio terminal:

```
dotnet restore
```

or:

```
Build → Restore NuGet Packages
```

---

## Step 4: Configure Database Connection

Open:

```
appsettings.json
```

Update:

```json
{
  "ConnectionStrings": {
    "WalletDb":
    "Server=localhost;Database=MiniWalletDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Update SQL Server instance name if required.

---

# Database Setup Steps

## SQL Server Setup

1. Open SQL Server Management Studio.

2. Connect to SQL Server.

3. Open:

```
Database/MiniWalletDB.sql
```

4. Execute SQL script.

The script creates:

---

## Wallet Table

Stores wallet information:

| Column       | Description              |
| ------------ | ------------------------ |
| WalletId     | Unique wallet identifier |
| Name         | User name                |
| Email        | User email               |
| MobileNumber | User mobile              |
| Balance      | Current wallet balance   |
| CreatedAt    | Wallet creation date     |
| UpdatedAt    | Last update timestamp    |
| Row Version  |  Automatic version number used to detect conflicting updates |

---

## WalletTransactions Table

Stores transaction history:

| Column          | Description                  |
| --------------- | ---------------------------- |
| TransactionId   | Transaction identifier       |
| WalletId        | Wallet owner                 |
| TransactionType | Credit/Debit/Transfer        |
| Amount          | Transaction amount           |
| BalanceBefore   | Previous balance             |
| BalanceAfter    | Updated balance              |
| ReferenceId     | Unique transaction reference |
| Status          | Transaction status           |
| CreatedAt       | Transaction timestamp        |

---

# Database Constraints

## Unique Email

A user cannot create multiple wallets using the same email.

## Unique Mobile Number

A mobile number can belong to only one wallet.

## Unique Reference ID

Each transaction reference ID must be unique.

Example:

```
CREDIT001
```

cannot be processed twice.

---

# Running the Application

Run the project:

```
F5
```

or:

```
Ctrl + F5
```

The API will start:

Example:

```
https://localhost:7001
```

Swagger documentation:

```
https://localhost:7001/swagger
```

---

# API Documentation

# Wallet APIs

## 1. Create Wallet

Endpoint:

```
POST /api/wallet/create
```

Request:

```json
{
    "name":"John Smith",
    "email":"john@gmail.com",
    "mobileNumber":"9876543210",
    "initialBalance":1000
}
```

Response:

```json
{
    "walletId":"guid",
    "name":"John Smith",
    "email":"john@gmail.com",
    "mobileNumber":"9876543210",
    "balance":1000
}
```

---

## 2. Get Wallet Balance

Endpoint:

```
GET /api/wallet/{walletId}/balance
```

Response:

```json
{
    "walletId":"guid",
    "userName":"John Smith",
    "currentBalance":1000,
    "updatedTimestamp":"date"
}
```

---

## 3. Transaction History

Endpoint:

```
GET /api/wallet/{walletId}/transactions
```

Supports:

* Transaction Type
* From Date
* To Date
* Page Number
* Page Size

Example:

```
GET /api/wallet/{id}/transactions?pageNumber=1&pageSize=10
```

---

# Transaction APIs

## 4. Credit Wallet

Endpoint:

```
POST /api/transaction/credit
```

Request:

```json
{
    "walletId":"guid",
    "amount":500,
    "referenceId":"CREDIT001"
}
```

---

## 5. Debit Wallet

Endpoint:

```
POST /api/transaction/debit
```

Request:

```json
{
    "walletId":"guid",
    "amount":100,
    "referenceId":"DEBIT001"
}
```

---

## 6. Wallet Transfer

Endpoint:

```
POST /api/transaction/transfer
```

Request:

```json
{
    "fromWalletId":"guid",
    "toWalletId":"guid",
    "amount":200,
    "referenceId":"TRANSFER001"
}
```

---

# Duplicate Transaction Handling

Duplicate transactions are prevented using multiple layers.

## 1. Database Level Protection

A unique index is created:

```
WalletTransactions.ReferenceId
```

This prevents duplicate records.

---

## 2. Application Level Validation

Before processing:

```csharp
AnyAsync(x=>x.ReferenceId == referenceId)
```

checks whether the transaction already exists.

Example:

First request:

```
ReferenceId = CREDIT001
```

Success.

Second request:

```
ReferenceId = CREDIT001
```

Rejected.

Response:

```json
{
 "message":"Duplicate reference ID"
}
```

---

# Concurrent Debit and Transfer Handling

The system handles concurrent transactions using database transactions.

## Debit Protection

Debit uses:

```
Serializable Isolation Level
```

This prevents:

* Double spending
* Race conditions
* Incorrect balances

Example:

Wallet balance:

```
1000
```

Two requests:

```
Debit 800
Debit 800
```

Result:

First request:

```
Success
```

Second request:

```
Insufficient balance
```

---

# Transfer Transaction Safety

Wallet transfer uses database transaction:

Process:

```
Start Transaction

↓


Validate Sender

↓

Validate Receiver

↓

Check Balance

↓

Debit Sender

↓

Credit Receiver

↓

Save Transaction History

↓

Commit Transaction
```

If any step fails:

```
Rollback Entire Transaction
```

Therefore:

* Sender debit
* Receiver credit

always happen together.

---

# Negative Balance Prevention

Negative wallet balance is prevented by validation.

Before debit:

```csharp
if(wallet.Balance < amount)
{
    throw exception;
}
```

Rules:

* Amount must be greater than zero
* Wallet must have sufficient balance
* Debit cannot exceed available balance

Wallet balance always remains:

```
Balance >= 0
```

---

# Performance Optimizations Applied

## 1. Async Database Operations

All database operations use:

```
async / await
```

Benefits:

* Better API scalability
* Non-blocking operations

---

## 2. No Tracking Queries

Read operations use:

```csharp
.AsNoTracking()
```

Used for:

* Balance checking
* Transaction history

Improves query performance.

---

## 3. Database Indexing

Indexes added:

```
ReferenceId

WalletId

TransactionType

CreatedAt
```

Improves:

* Transaction lookup
* History filtering
* Duplicate checking

---

## 4. Pagination

Transaction history supports:

```
pageNumber

pageSize
```

Prevents loading thousands of records.

---

## 5. Optimized Database Transactions

Only critical operations use database transactions:

* Debit
* Transfer
* Credit

---

# Expected API Performance

Local testing results:

| API                 | Expected Time |
| ------------------- | ------------- |
| Balance Check       | <50 ms        |
| Credit              | 100-150 ms    |
| Debit               | 100-150 ms    |
| Transfer            | 150-200 ms    |
| Transaction History | <200 ms       |

---

# Assumptions

1. One user can have only one wallet.

2. Wallet deletion is not supported.

3. Currency conversion is not required.

4. Reference ID is globally unique.

5. All successful transactions are permanently stored.

6. Balance is stored in Wallet table for faster retrieval.

---

# Improvements Possible With More Time

## Security Improvements

Add:

* JWT Authentication
* User authorization
* API rate limiting
* Encryption

---

## Architecture Improvements

Add:

* Repository Pattern
* CQRS
* MediatR
* Unit Testing

---

## Production Improvements

Add:

* Docker deployment
* Kubernetes
* Redis caching
* Message Queue
* Distributed logging
* Monitoring

---

## Database Improvements

Add:

* Read replicas
* Database partitioning
* Stored procedures
* Backup strategy

---

# Postman Collection

Postman collection location:

```
Postman/MiniWalletBackendSystem.postman_collection.json
```

Includes:

* Create Wallet
* Credit Wallet
* Debit Wallet
* Transfer Wallet
* Balance Check
* Transaction History
* Duplicate Transaction Tests
* Validation Tests
---

**Mini Wallet Backend System completed using ASP.NET Core Web API, C#, Entity Framework Core, and SQL Server.**
