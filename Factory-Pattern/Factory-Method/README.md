# Factory Method Pattern

## Overview

Factory Method is part of the Factory Pattern family. See the main [Factory Pattern README](../README.md) for a complete comparison of Simple Factory, Factory Method, and Abstract Factory.

---

## The Problem

Your e-commerce platform operates in multiple regions. Each region supports different payment providers:

- **US Region**: Stripe, PayPal, Venmo
- **EU Region**: Stripe, PayPal, Sofort  
- **Asia Region**: Stripe, PayPal, Alipay
- **Canada Region**: Stripe, Interac

With Simple Factory, you'd need messy if-else chains checking both region AND payment method. Every new region requires modifying the factory class ❌

---

## Solution: Factory Method Pattern

**Intent**: Define an interface for creating objects, but let **subclasses decide** which class to instantiate.

### Structure:
```
PaymentProcessorFactory (abstract)
    ↑ CreatePaymentProcessor(method)*
    |
    ├─ USPaymentFactory
    ├─ EUPaymentFactory  
    ├─ AsiaPaymentFactory
    └─ CanadaPaymentFactory
```

Each regional factory knows which payment methods it supports.

---

## How It Works

### Step 1: Define Abstract Factory

```csharp
public abstract class PaymentProcessorFactory
{
    // Factory method - subclasses override this
    public abstract IPaymentProcessor CreatePaymentProcessor(string method);
    
    // Template method using the factory method
    public PaymentResult ProcessPayment(string method, decimal amount)
    {
        var processor = CreatePaymentProcessor(method);
        return processor.ProcessPayment(amount, details);
    }
}
```

### Step 2: Create Concrete Factories

```csharp
public class USPaymentFactory : PaymentProcessorFactory
{
    public override IPaymentProcessor CreatePaymentProcessor(string method)
    {
        return method.ToLower() switch
        {
            "stripe" => new StripeProcessor(),
            "paypal" => new PayPalProcessor(),
            "venmo" => new VenmoProcessor(),
            _ => throw new ArgumentException($"'{method}' not supported in US")
        };
    }
}

public class EUPaymentFactory : PaymentProcessorFactory
{
    public override IPaymentProcessor CreatePaymentProcessor(string method)
    {
        return method.ToLower() switch
        {
            "stripe" => new StripeProcessor(),
            "paypal" => new PayPalProcessor(),
            "sofort" => new SofortProcessor(),
            _ => throw new ArgumentException($"'{method}' not supported in EU")
        };
    }
}
```

### Step 3: Use Polymorphically

```csharp
// Select factory based on region
PaymentProcessorFactory factory = region switch
{
    "US" => new USPaymentFactory(),
    "EU" => new EUPaymentFactory(),
    "Asia" => new AsiaPaymentFactory(),
    _ => throw new ArgumentException("Unsupported region")
};

// Process payment - factory creates correct processor
var result = factory.ProcessPayment("Stripe", 100m, details);
```

---

## Benefits

✅ **Open/Closed Principle** - Add new regional factories without modifying existing code  
✅ **Single Responsibility** - Each factory handles one region's logic  
✅ **Polymorphism** - Client works with abstract factory, doesn't know concrete types  
✅ **Scalability** - Easy to add new regions (just create new factory subclass)

---

## When to Use

✅ **Use Factory Method when:**
- You need **multiple variants of factories** (regional, versioned, themed)
- Each factory variant creates different sets of products
- Creation logic differs significantly between variants
- You want to adhere to Open/Closed Principle

### Real-World Examples:
- Regional payment gateways (US, EU, Asia)
- Platform-specific UI (iOS, Android, Web)
- Environment-specific configs (Dev, Staging, Prod)
- Database providers (SQL Server, PostgreSQL, MySQL)

---

## When NOT to Use

❌ **Don't use Factory Method when:**
- You have only **one factory implementation** → Use Simple Factory instead
- Differences are only **configuration** (connection strings, URLs) → Use config files
- You have fewer than **3 factory variants** → Probably overkill

---

## Comparison with Simple Factory

| Aspect            | Simple Factory          | Factory Method                     |
| ----------------- | ----------------------- | ---------------------------------- |
| **Structure**     | One factory class       | Abstract factory + subclasses      |
| **Extensibility** | Modify factory          | Add new subclass                   |
| **When to use**   | 3-10 product types      | Multiple factory variants          |
| **Example**       | PaymentProcessorFactory | USPaymentFactory, EUPaymentFactory |

**Rule of Thumb**: Start with Simple Factory. Refactor to Factory Method when you need 3+ factory variants.
---
## Key Takeaways

> **Factory Method is about creating one type of product (payment processor) but with multiple factory variants (US, EU, Asia).**

> **Each subclass determines which concrete products to create.**

> **Use when you need polymorphic factories, not just polymorphic products.**

---

For a complete understanding of when to use Factory Method vs. Simple Factory vs. Abstract Factory, see the [main Factory Pattern README](../README.md#decision-tree).
