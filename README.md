# Kanime Clothing - E-Commerce Platform

A modern ASP.NET Core web application for an anime-themed clothing store with shopping cart functionality, product filtering, and integrated payment processing via Paystack.

## 🚀 Quick Start

### Prerequisites

- **.NET 10.0** SDK or later
- **Visual Studio 2022** (or VS Code)
- **SQL Server** (configured in `appsettings.json`)
- **Git**

### Clone the Repository

```bash
git clone https://github.com/yourusername/kanimeclothing.git
cd kanimeclothing
```

### Setup & Run

1. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

2. **Update database connection** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Your-SQL-Server-Connection-String"
   }
   ```

3. **Configure Paystack credentials** in `appsettings.json`:
   ```json
   "Payment": {
     "PaystackSecretKey": "sk_test_your_key",
     "PaystackVerifyUrl": "https://api.paystack.co/transaction/verify/"
   }
   ```

4. **Run migrations** (if applicable):
   ```bash
   dotnet ef database update
   ```

5. **Build and run:**
   ```bash
   dotnet build
   dotnet run
   ```

6. **Access the app:**
   - Open browser to `https://localhost:5001` (or HTTP port shown in terminal)

---

## 📁 Project Structure

```
kanimeclothing/
├── Controllers/
│   └── HomeController.cs          # Main controller (Shop, Cart, Checkout, Payments)
├── Models/
│   ├── Cart.cs                    # Shopping cart model
│   ├── CartItem.cs                # Individual cart item
│   ├── Product.cs                 # Product data model
│   ├── ProductReview.cs           # Product review model
│   └── ErrorViewModel.cs          # Error page model
├── Services/
│   ├── CartService.cs             # Shopping cart session management
│   ├── ProductService.cs          # Product data retrieval
│   ├── JsonLocalizationService.cs # Multi-language support (EN, ES, FR)
│   ├── PaymentService.cs          # Paystack payment verification
│   └── OrderService.cs            # Order creation & management
├── Data/
│   └── ApplicationDbContext.cs    # Entity Framework database context
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml           # Homepage with new arrivals
│   │   ├── Shop.cshtml            # Product listing with client-side filtering
│   │   ├── Details.cshtml         # Product detail page with reviews
│   │   ├── Categories.cshtml      # Category listing
│   │   ├── ViewCart.cshtml        # Shopping cart with quantity adjustment
│   │   ├── Contact.cshtml         # Contact page
│   │   ├── PaymentSuccess.cshtml  # Order confirmation
│   │   ├── PaymentCancelled.cshtml # Payment failed/cancelled
│   │   ├── _CheckoutConfirmationModal.cshtml # Checkout modal partial
│   │   └── _LoadingOverlay.cshtml (deprecated)
│   ├── Shared/
│   │   ├── _Layout.cshtml         # Master layout
│   │   ├── _Layout.cshtml.css     # Layout styles
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   └── Error.cshtml           # Global error page
│   └── _ViewImports.cshtml        # Global view imports
├── wwwroot/
│   ├── api/
│   │   └── products.json          # Product seed data
│   ├── css/
│   │   ├── site.css               # Global styles
│   │   ├── cart.css               # Cart & modal styles
│   │   ├── details.css            # Product details styles
│   │   ├── payment.css            # Payment page styles
│   │   └── temp_footer.css        # Footer styles
│   ├── js/
│   │   ├── site.js                # Global scripts
│   │   └── product-navigation.js  # Product filter & navigation
│   └── lib/                       # Third-party libraries (Bootstrap, jQuery, etc.)
├── Resources/
│   ├── en.json                    # English localization
│   ├── es.json                    # Spanish localization
│   └── fr.json                    # French localization
├── Properties/
│   ├── launchSettings.json        # Development server settings
│   └── PublishProfiles/           # FTP deployment profiles
├── kanimeclothing.Tests/          # Unit tests
│   ├── Controllers/
│   │   └── HomeControllerTests.cs
│   └── Services/
│       ├── ProductServiceTests.cs
│       └── JsonLocalizationServiceTests.cs
├── Program.cs                     # Application startup configuration
├── Global.cs                      # Global application settings
├── appsettings.json              # Configuration (DB, Paystack, etc.)
├── appsettings.Development.json  # Development overrides
└── README.md                      # This file
```

---

## 📄 Key Pages & Features

### 🏠 **Home / Index (`/Home/Index`)**
- Displays **new arrivals** (latest 3 products)
- Hero section with branding
- Quick navigation to shop

### 🛍️ **Shop (`/Home/Shop`)**
- **Product grid** with images and prices
- **Client-side filtering** by:
  - Category
  - Price range
  - Sort (price low-to-high, high-to-low, newest)
- No page reload; smooth UX
- Add-to-cart buttons with size/color selection

### 📋 **Product Details (`/Home/Details/{id}`)**
- Full product information
- Customer reviews (sample data)
- Related products (same category)
- Quantity and variant (size/color) selection
- Add-to-cart action

### 🛒 **Shopping Cart (`/Home/ViewCart`)**
- Display all cart items
- **Smooth quantity adjustment** (no flicker):
  - UI updates immediately
  - Syncs to server on checkout
- Remove item buttons
- Order summary (subtotal, shipping, tax, total)
- **Proceed to Checkout** button
- Confirmation modal before payment redirect

### 💳 **Checkout & Payment (`/Home/PaymentCallback`)**
- Checkout confirmation modal (Razor partial)
- Redirect to **Paystack payment gateway**
- Payment verification via API
- On success:
  - Order created in database
  - Cart cleared
  - **PaymentSuccess** page shown with order reference
- On failure:
  - **PaymentCancelled** page
  - Cart preserved for retry

### 📞 **Contact (`/Home/Contact`)**
- Contact form (placeholder)

### 🔤 **Categories (`/Home/Categories`)**
- Browse by category

---

## 🛠️ Technology Stack

| Component | Technology |
|-----------|-----------|
| Framework | ASP.NET Core 10.0 |
| UI | Razor Pages & Views |
| Styling | CSS3, Bootstrap (optional) |
| Database | SQL Server |
| ORM | Entity Framework Core 10 |
| Payment Gateway | Paystack |
| Localization | JSON-based (EN, ES, FR) |
| Testing | xUnit, Moq |
| Frontend | Vanilla JavaScript (no jQuery dependency) |

---

## 🔐 Configuration

### **appsettings.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=YOUR_SERVER;Initial Catalog=YOUR_DB;..."
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Payment": {
    "PaystackSecretKey": "sk_test_YOUR_KEY_HERE",
    "PaystackVerifyUrl": "https://api.paystack.co/transaction/verify/"
  },
  "AllowedHosts": "*"
}
```

---

## 🧪 Running Tests

```bash
dotnet test
```

Tests are in `kanimeclothing.Tests/`:
- `HomeControllerTests.cs` - Controller action tests
- `ProductServiceTests.cs` - Product service tests
- `JsonLocalizationServiceTests.cs` - Localization tests

---

## 📦 Deployment

### **FTP Deployment (Visual Studio)**

1. Right-click project → **Publish**
2. Choose **FTP/FTPS**
3. Enter hosting credentials:
   - **Server**: `win8119.site4now.net` (example)
   - **Username**: Your FTP username
   - **Password**: Your FTP password
   - **Destination URL**: Your live site URL
4. Click **Validate Connection**
5. Click **Publish**

### **GitHub Actions (CI/CD)**

A workflow file (`.github/workflows/deploy.yml`) can automate deployment on push.

---

## 🌐 Localization

The app supports **English (EN)**, **Spanish (ES)**, and **French (FR)**.

Language files located in `Resources/`:
- `en.json`
- `es.json`
- `fr.json`

Switch language via browser culture settings or query string.

---

## 🐛 Troubleshooting

### **Cart not clearing after payment**
- Check `PaymentCallback` is being called (check browser Network tab)
- Verify `PaystackSecretKey` is correct (test vs live)
- Ensure session hasn't expired

### **Products not showing**
- Verify `products.json` exists in `wwwroot/api/`
- Check `ProductService` is loading from correct path
- Check browser console for JavaScript errors

### **Paystack redirect failing**
- Confirm webhook URL is correct
- Test payment key manually in Postman
- Check firewall/network blocking Paystack API

---

## 📝 License

This project is proprietary. Contact the owner for usage rights.

---

## 👤 Author

**Your Name** - Project Owner

For questions or issues, open an issue on GitHub or contact via email.

---

## 🔄 Version History

- **v1.0** (Mar 2026) - Initial release with shop, cart, and Paystack integration
