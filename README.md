# FlapKap Vending Machine API

A **.NET 8 Web API** implementing the FlapKap Backend Challenge using **Clean Architecture Lite** with:
- Buyer & Seller roles
- JWT authentication
- EF Core + SQLite
- Swagger UI with JWT support
- Unit & Integration tests
- Exception handling & logging

---

## **📌 Requirements**
- .NET 8 SDK  
- EF Core CLI tools (`dotnet tool install --global dotnet-ef`)

---

## **🚀 Setup**

### 1️⃣ Clone the repository
```bash
git clone https://github.com/YOUR_USERNAME/FlapKapVendingMachine.git
cd FlapKapVendingMachine
2️⃣ Restore packages
bash
Copy
Edit
dotnet restore
3️⃣ Apply migrations & seed data
bash
Copy
Edit
dotnet ef database update --project src/FlapKapVendingMachine.Infrastructure --startup-project src/FlapKapVendingMachine.WebApi
4️⃣ Run the API
bash
Copy
Edit
dotnet run --project src/FlapKapVendingMachine.WebApi
Swagger UI will be available at:

bash
Copy
Edit
https://localhost:5001/swagger
🧪 Seeded Data
Role	Username	Password
Seller	seller1	123
Buyer	buyer1	123

📦 Features
Auth
POST /api/auth/register → Register a new user (no auth required)

POST /api/auth/login → Login & get JWT

Buyer Actions
POST /api/buyer/deposit?coin=VALUE → Deposit coins (5, 10, 20, 50, 100 cents)

POST /api/buyer/buy?productId=ID&quantity=QTY → Buy product

POST /api/buyer/reset → Reset deposit to 0

Seller Actions
GET /api/product → View all products (public)

POST /api/product → Create a product (Seller only)

PUT /api/product/{id} → Update own product

DELETE /api/product/{id} → Delete own product

🔑 Using JWT in Swagger
Login with:

pgsql
Copy
Edit
POST /api/auth/login?username=buyer1&password=123
Copy the token from the response.

Click Authorize in Swagger (top right).

Paste:

nginx
Copy
Edit
Bearer YOUR_TOKEN
Click Authorize → Close.

Now all protected endpoints will work from Swagger.

📜 Example Buyer Flow
Login as buyer → get JWT.

Authorize in Swagger.

Deposit:

bash
Copy
Edit
POST /api/buyer/deposit?coin=50
Buy product:

bash
Copy
Edit
POST /api/buyer/buy?productId=1&quantity=1
Reset deposit:

bash
Copy
Edit
POST /api/buyer/reset
🧪 Running Tests
Integration tests use WebApplicationFactory to run the API in-memory.

bash
Copy
Edit
dotnet test
Tests include:

Auth (valid & invalid login)

Buyer deposit/buy/reset

Seller CRUD with ownership checks

🏗 Project Structure
bash
Copy
Edit
src/
  FlapKapVendingMachine.Domain/        # Entities & Enums
  FlapKapVendingMachine.Application/   # Interfaces & Contracts
  FlapKapVendingMachine.Infrastructure/# EF Core, JWT, Persistence
  FlapKapVendingMachine.WebApi/        # Controllers, Auth, Swagger

tests/
  FlapKapVendingMachine.WebApi.Tests/  # Integration tests
💡 Bonus
Swagger configured for JWT Bearer authentication

Change calculation in allowed coin denominations

Role-based access control

Initial seeding for quick testing

Exception handling middleware with logging

Development Chat Log: https://chatgpt.com/share/68939404-1da8-8003-a541-9770ba84bac8

📩 Submission
Zip the repository:

bash
Copy
Edit
zip -r FlapKapVendingMachine.zip .
Send the .zip to FlapKap as requested.

yaml
Copy
Edit





