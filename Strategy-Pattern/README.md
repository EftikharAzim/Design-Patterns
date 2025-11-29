# Strategy Pattern

## The Problem

Consider an e-commerce system that calculates shipping costs. Initially, you might write code like this:

```csharp
public class Order
{
    public decimal CalculateShipping(string shippingType, decimal weight, decimal orderValue)
    {
        if (shippingType == "Standard")
            return 5m;
        else if (shippingType == "Express")
            return 15m + (2m * weight);
        else if (shippingType == "International")
            return 30m + (5m * weight) + (orderValue * 0.1m);
        return 0m;
    }
}
```

### What's Wrong With This?

1. **Adding new shipping methods** → Requires modifying the `Order` class ❌ *Violates Open/Closed Principle*
2. **Testing in isolation** → Can't test International shipping logic separately ❌ *Tight coupling*
3. **Complex variations** → If Express shipping rules change based on carrier (FedEx vs UPS), you'll need nested if-else ❌ *Violates Single Responsibility Principle*

The `Order` class knows too much. Every time shipping logic changes, you must modify this class.

---

## What is the Strategy Pattern?

**Intent**: Define a family of algorithms, encapsulate each one, and make them interchangeable. Strategy lets the algorithm vary independently from clients that use it.

**Key Idea**: Instead of having multiple conditional statements, extract each algorithm into its own class.

---

## How It Solves the Problem

The Strategy Pattern solves this by:

1. **Defining a common interface** for all shipping strategies
2. **Implementing each strategy** in a separate class
3. **Allowing the client** to choose which strategy to use at runtime

### Structure

```
┌─────────────────┐
│     Order       │  (Client)
│  Uses strategy  │
└────────┬────────┘
         │ depends on
         ▼
┌──────────────────────┐
│  IShippingStrategy   │  (Interface)
│  CalculateShipping() │
└──────────────────────┘
         △
         │ implements
    ┌────┴────┬──────────────────┐
    │         │                  │
┌───┴────┐ ┌─┴────────┐ ┌──────┴───────────┐
│Standard│ │ Express  │ │ International    │
│Shipping│ │ Shipping │ │ Shipping         │
└────────┘ └──────────┘ └──────────────────┘
```

---

## Solutions Overview

### Solution-1: Basic Strategy Pattern
- Introduces `IShippingStrategy` interface
- Separates each shipping method into its own class
- `Order` delegates to the strategy
- **Issue**: Factory creation logic still uses if-else in `CheckoutProcessor`

### Solution-2: Strategy + Simple Factory ⭐ **Recommended**
- Adds `ShippingStrategyFactory` to eliminate if-else
- Uses switch expression for clean strategy creation
- Input validation with `TryParse`
- **This is the sweet spot** for most real-world scenarios

### Solution-3: Interface Segregation Experiment
- Explores separate interfaces for weight-based and value-based strategies
- **Correctly identified as over-engineering** for this use case
- Shipping cost calculation is a pure function—doesn't need stateful properties

### Solution-4: Dependency Injection
- Demonstrates strategies with external dependencies (`ICarrierApiClient`)
- Shows strategy caching for performance
- Illustrates how to inject dependencies into strategies

---

## When to Use Strategy Pattern

✅ **Use Strategy when:**

1. You have **multiple ways** to perform a task (3+ algorithms)
2. You want to **avoid conditional statements** that select behavior
3. Related classes differ **only in behavior**
4. You need to **change algorithms at runtime**
5. Algorithms have **different data requirements** but share a common interface

### Real-World Examples:
- Payment processing (Credit Card, PayPal, Bank Transfer)
- Compression algorithms (ZIP, RAR, 7Z)
- Sorting algorithms (QuickSort, MergeSort, BubbleSort)
- Authentication methods (OAuth, JWT, API Key)
- Pricing strategies (Regular, Member Discount, Seasonal Sale)

---

## When NOT to Use Strategy Pattern

❌ **Don't use Strategy when:**

1. **You have only 1-2 algorithms**
   - Simple if-else is clearer than creating unnecessary abstractions
   - *YAGNI Principle*: Don't create patterns for hypothetical future needs

2. **Algorithms rarely change**
   - If shipping calculation has been the same for 5 years, adding strategy is premature optimization

3. **The "strategy" is just configuration**
   - Don't use Strategy for data differences:
   ```csharp
   // ❌ Bad: Using Strategy for config
   public class DevDatabaseStrategy { }
   public class ProdDatabaseStrategy { }
   
   // ✅ Good: Use configuration
   var connectionString = config.GetConnectionString("Default");
   ```

4. **Simple conditional is more readable**
   ```csharp
   // ❌ Overkill: Creating strategy for simple logic
   var discount = customer.IsPremium ? 0.2m : 0.1m;
   
   // ✅ Better: Keep it simple
   ```

---

## Decision Tree: Should You Use Strategy?

```
Do you have multiple algorithms doing the same thing?
│
├─ No → Don't use Strategy
│
└─ Yes → Do they change frequently or differ significantly?
    │
    ├─ No → Use simple if-else or switch
    │
    └─ Yes → How many algorithms?
        │
        ├─ 1-2 → Probably overkill
        │
        └─ 3+ → Use Strategy Pattern ✅
            │
            └─ Do you need to create strategies dynamically?
                │
                ├─ No → Strategy alone (Solution-1)
                │
                └─ Yes → Strategy + Factory (Solution-2) ⭐
```

---

## How to Implement

### Step 1: Define the Strategy Interface
```csharp
public interface IShippingStrategy
{
    decimal CalculateShipping(decimal weight, decimal orderValue);
}
```

### Step 2: Implement Concrete Strategies
```csharp
public class ExpressShipping : IShippingStrategy
{
    public decimal CalculateShipping(decimal weight, decimal orderValue)
    {
        return 15m + (2m * weight);
    }
}
```

### Step 3: Use Strategy in Client
```csharp
public class Order
{
    private readonly IShippingStrategy _shippingStrategy;

    public Order(IShippingStrategy shippingStrategy)
    {
        _shippingStrategy = shippingStrategy;
    }

    public decimal CalculateShipping(decimal weight, decimal orderValue)
    {
        return _shippingStrategy.CalculateShipping(weight, orderValue);
    }
}
```

### Step 4: Choose Strategy at Runtime
```csharp
// Without Factory (Solution-1)
IShippingStrategy strategy = shippingType == "Express" 
    ? new ExpressShipping() 
    : new StandardShipping();

// With Factory (Solution-2) ⭐ Cleaner
var factory = new ShippingStrategyFactory();
var strategy = factory.GetShippingStrategy(shippingType);
```

---

## Benefits

✅ **Open/Closed Principle** - Add new strategies without modifying existing code  
✅ **Single Responsibility** - Each strategy has one job  
✅ **Testability** - Test strategies in isolation  
✅ **Runtime Flexibility** - Switch strategies dynamically  
✅ **Eliminates Conditionals** - Replaces if-else chains with polymorphism

---

## Trade-offs

⚠️ **Increased Number of Classes** - Each algorithm becomes a separate class  
⚠️ **Client Must Know Strategies** - Client needs to understand which strategy to use (mitigated by Factory)  
⚠️ **Overhead for Simple Cases** - Don't use for 1-2 simple conditions

---

## Related Patterns

- **Factory Pattern** - Often used together to create strategies (see Solution-2)
- **State Pattern** - Similar structure, but State changes behavior based on internal state; Strategy is chosen by client
- **Template Method** - Defines algorithm skeleton in base class; Strategy encapsulates entire algorithm
- **Command Pattern** - Encapsulates requests as objects; Strategy encapsulates algorithms

---

## Key Takeaways

> **Strategy is about algorithms, not data.**  
> If you're only changing values (rates, thresholds), use configuration.  
> If you're changing logic (formulas, rules), use Strategy.

> **Don't use Strategy for everything.**  
> Simple if-else is fine for 1-2 conditions.  
> Use Strategy when you have 3+ algorithms that change independently.

> **Combine with Factory for cleaner code.**  
> Factory eliminates strategy selection logic from business code.
