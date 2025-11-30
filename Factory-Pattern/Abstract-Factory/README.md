# Abstract Factory Pattern

## Overview

Abstract Factory is part of the Factory Pattern family. See the main [Factory Pattern README](../README.md) for a complete comparison of Simple Factory, Factory Method, and Abstract Factory.

---

## The Problem

Your payment system needs more than just payment processing. For each payment provider, you need:

1. **Payment Processor** - Processes the payment
2. **Receipt Generator** - Generates receipts in provider's format
3. **Refund Handler** - Handles refunds through provider's API

### The Critical Issue: Compatibility

These components **must work together**. You can't mix and match:

❌ **Inconsistent**: Stripe payment + PayPal receipt + Bank refund  
✅ **Consistent**: Stripe payment + Stripe receipt + Stripe refund

Each provider has its own:
- Fee structure
- Receipt format (PDF, JSON, HTML)
- Refund policies
- API endpoints

You need a way to ensure that all components for a transaction come from the **same provider family**.

---

## Solution: Abstract Factory Pattern

**Intent**: Provide an interface for creating **families of related objects** without specifying their concrete classes.

**Key Idea**: Each factory creates a complete **family** of compatible objects.

### Structure:
```
IPaymentServiceFactory (interface)
    ↓ creates family of 3 products
Processor + Receipt + Refund
    ↑
    |
┌───┴────┬─────────┬──────────────┐
|        |         |              |
Stripe  PayPal  BankTransfer   (each creates compatible family)
```

---

## How It Works

### Step 1: Define Product Interfaces

```csharp
public interface IPaymentProcessor
{
    PaymentResult ProcessPayment(decimal amount, PaymentDetails details);
}

public interface IReceiptGenerator
{
    Receipt GenerateReceipt(PaymentResult result);
}

public interface IRefundHandler
{
    RefundResult HandleRefund(string transactionId, decimal amount);
}
```

### Step 2: Define Abstract Factory Interface

```csharp
public interface IPaymentServiceFactory
{
    IPaymentProcessor CreatePaymentProcessor();
    IReceiptGenerator CreateReceiptGenerator();
    IRefundHandler CreateRefundHandler();
    string ProviderName { get; }
}
```

This interface **guarantees** that whoever implements it must provide **all three components** as a cohesive family.

### Step 3: Create Concrete Factories (One per Provider)

```csharp
// Stripe family
public class StripeServiceFactory : IPaymentServiceFactory
{
    public string ProviderName => "Stripe";
    
    public IPaymentProcessor CreatePaymentProcessor() 
        => new StripePaymentProcessor();
    
    public IReceiptGenerator CreateReceiptGenerator() 
        => new StripeReceiptGenerator();  // PDF format, Stripe branding
    
    public IRefundHandler CreateRefundHandler() 
        => new StripeRefundHandler();  // Uses Stripe API
}

// PayPal family  
public class PayPalServiceFactory : IPaymentServiceFactory
{
    public string ProviderName => "PayPal";
    
    public IPaymentProcessor CreatePaymentProcessor() 
        => new PayPalPaymentProcessor();
    
    public IReceiptGenerator CreateReceiptGenerator() 
        => new PayPalReceiptGenerator();  // JSON format
    
    public IRefundHandler CreateRefundHandler() 
        => new PayPalRefundHandler();  // Charges refund fee
}
```

### Step 4: Client Uses Entire Family

```csharp
public class PaymentService
{
    private readonly IPaymentServiceFactory _factory;
    
    public PaymentService(IPaymentServiceFactory factory)
    {
        _factory = factory;
    }
    
    public void ProcessTransaction(decimal amount, PaymentDetails details)
    {
        // Create entire family from one factory
        var processor = _factory.CreatePaymentProcessor();
        var receiptGenerator = _factory.CreateReceiptGenerator();
        var refundHandler = _factory.CreateRefundHandler();
        
        // All components guaranteed to work together
        var paymentResult = processor.ProcessPayment(amount, details);
        
        if (paymentResult.Success)
        {
            var receipt = receiptGenerator.GenerateReceipt(paymentResult);
            // Later, if refund needed:
            var refund = refundHandler.HandleRefund(paymentResult.TransactionId, amount);
        }
    }
}

// Usage
var stripeFactory = new StripeServiceFactory();
var paymentService = new PaymentService(stripeFactory);
paymentService.ProcessTransaction(100m, details);
```

---

## Why This Solves the Problem

1. **Enforces Compatibility** - All components come from the same factory, ensuring they work together
2. **Prevents Mixing** - Can't create Stripe processor with PayPal receipt
3. **Consistency** - Receipt format matches provider, refund uses correct API
4. **Easy to Swap** - Change factory → get entire new compatible family

---

## Benefits

✅ **Guarantees product compatibility** - Family members designed to work together  
✅ **Isolates concrete classes** - Client doesn't know about StripePaymentProcessor, only IPaymentProcessor  
✅ **Easy to swap entire families** - Change one line (factory selection), everything changes  
✅ **Single Responsibility** - Each factory creates one coherent family  
✅ **Open/Closed** - Add new provider families without modifying existing code

---

## When to Use

✅ **Use Abstract Factory when:**
- System needs **multiple related products** that must work together
- You need to enforce that related products are from the **same family**
- System should be independent of how its products are created
- You want to provide a library of products and reveal only interfaces

### Real-World Examples:

**UI Themes:**
- Light Theme Factory → Light Button + Light TextBox + Light Menu
- Dark Theme Factory → Dark Button + Dark TextBox + Dark Menu

**Cross-Platform UI:**
- Windows Factory → WinButton + WinTextBox + WinMenu
- Mac Factory → MacButton + MacTextBox + MacMenu

**Database Providers:**
- SQL Server Factory → SqlConnection + SqlCommand + SqlDataReader
- PostgreSQL Factory → PgConnection + PgCommand + PgDataReader

**Payment Providers:**
- Stripe Factory → StripeProcessor + StripeReceipt + StripeRefund
- PayPal Factory → PayPalProcessor + PayPalReceipt + PayPalRefund

---

## When NOT to Use

❌ **Don't use Abstract Factory when:**

1. **Products aren't related or don't need to be compatible**
   ```csharp
   // ❌ Bad: Word, Excel, PowerPoint don't "work together"
   public interface IOfficeFactory
   {
       IWordProcessor CreateWord();
       ISpreadsheet CreateExcel();
       IPresentation CreatePowerPoint();
   }
   ```

2. **You only have one product type**
   - Use Simple Factory or Factory Method instead

3. **Objects are independent**
   - If you can mix and match without issues, you don't need Abstract Factory
   - Use Simple Factory or DI container

4. **Only configuration differs, not compatibility**
   ```csharp
   // ❌ Bad: Just different connection strings
   public class DevDatabaseFactory { }
   public class ProdDatabaseFactory { }
   
   // ✅ Good: Use configuration
   var connString = config.GetConnectionString("Default");
   ```

---

## Pattern Comparison

| Pattern       | Simple Factory           | Factory Method              | Abstract Factory                                 |
| ------------- | ------------------------ | --------------------------- | ------------------------------------------------ |
| **Products**  | One type                 | One type                    | Multiple related types (family)                  |
| **Factories** | One                      | Multiple (subclasses)       | Multiple (implementations)                       |
| **Use Case**  | Create payment processor | Regional payment processors | Payment ecosystem (processor + receipt + refund) |
| **Guarantee** | Correct product          | Region-specific product     | Compatible product family                        |

---

## Abstract Factory vs Factory Method

**Factory Method**:
- Creates **one product**, but factories vary
- Example: USPaymentFactory creates Stripe/Venmo; EUPaymentFactory creates Stripe/Sofort

**Abstract Factory**:
- Creates **multiple related products** (families)
- Example: StripeFactory creates Processor + Receipt + Refund; PayPalFactory creates Processor + Receipt + Refund

**Can be combined**:
```csharp
// US Stripe Factory creates US-specific Stripe family
// EU Stripe Factory creates EU-specific Stripe family
```
---

## Key Takeaways

> **Abstract Factory is about creating families of related products that are guaranteed to work together.**

> **Use when you need multiple product types that must be compatible.**

> **Don't use for unrelated products or single product types.**

> **The "abstract" part means the client works with interfaces, never concrete classes.**

---

For a complete understanding of when to use Abstract Factory vs. Simple Factory vs. Factory Method, see the [main Factory Pattern README](../README.md#decision-tree).
