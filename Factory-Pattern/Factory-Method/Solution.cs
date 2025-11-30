/*
    Factory Method Pattern solves this by making the factory itself polymorphic.

    Intent: Define an interface for creating objects, but let subclasses decide which class to instantiate.
    Structure:
    - Abstract Factory (base class) declares the factory method
    - Concrete Factories (subclasses) override the factory method to create specific products
    - Product Interface defines what all products must implement
    - Concrete Products are the objects being created

    Key Benefit: Adding new factory variants (e.g., LatinAmericaPaymentFactory) doesn't require modifying existing code—just create a new subclass.
    
    When to Use:
    - You have multiple variants of creation logic
    - Each variant creates different sets of related objects
    - You want to adhere to Open/Closed Principle (open for extension, closed for modification)
*/

// ===== Models =====

public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; }
    public string Message { get; set; }
    public decimal Fee { get; set; }
}

public class PaymentDetails
{
    public string CustomerEmail { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

// ===== Payment Interface =====

public interface IPaymentProcessor
{
    PaymentResult ProcessPayment(decimal amount, PaymentDetails details);
    bool SupportsRefunds { get; }
}

// ===== Abstract Factory Base Class =====

public abstract class PaymentProcessorFactory
{
    // Factory Method - subclasses override this
    public abstract IPaymentProcessor CreatePaymentProcessor(string paymentMethod);

    // Template method that uses the factory method
    public PaymentResult ProcessPayment(string paymentMethod, decimal amount, PaymentDetails details)
    {
        var processor = CreatePaymentProcessor(paymentMethod);
        return processor.ProcessPayment(amount, details);
    }

    public abstract IEnumerable<string> GetSupportedMethods();
}

// ===== Concrete Processors =====

public class StripeProcessor : IPaymentProcessor
{
    public bool SupportsRefunds => true;

    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        Console.WriteLine($"[Stripe] Processing ${amount}...");
        
        var fee = amount * 0.029m + 0.30m; // 2.9% + $0.30
        
        return new PaymentResult
        {
            Success = true,
            TransactionId = $"stripe_{Guid.NewGuid():N}",
            Message = "Payment processed via Stripe",
            Fee = fee
        };
    }
}

public class PayPalProcessor : IPaymentProcessor
{
    public bool SupportsRefunds => true;

    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        Console.WriteLine($"[PayPal] Processing ${amount}...");
        
        var fee = amount * 0.034m + 0.30m; // 3.4% + $0.30
        
        return new PaymentResult
        {
            Success = true,
            TransactionId = $"paypal_{Guid.NewGuid():N}",
            Message = "Payment processed via PayPal",
            Fee = fee
        };
    }
}

public class VenmoProcessor : IPaymentProcessor
{
    public bool SupportsRefunds => true;

    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        Console.WriteLine($"[Venmo] Processing ${amount}...");
        
        return new PaymentResult 
        { 
            Success = true, 
            TransactionId = $"venmo_{Guid.NewGuid():N}",
            Message = "Payment processed via Venmo",
            Fee = 0m // No fees for Venmo
        };
    }
}

public class SofortProcessor : IPaymentProcessor
{
    public bool SupportsRefunds => true;

    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        Console.WriteLine($"[Sofort] Processing €{amount}...");
        
        return new PaymentResult 
        { 
            Success = true, 
            TransactionId = $"sofort_{Guid.NewGuid():N}",
            Message = "Payment processed via Sofort",
            Fee = amount * 0.014m // 1.4%
        };
    }
}

public class AlipayProcessor : IPaymentProcessor
{
    public bool SupportsRefunds => true;

    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        Console.WriteLine($"[Alipay] Processing ¥{amount}...");
        
        return new PaymentResult 
        { 
            Success = true, 
            TransactionId = $"alipay_{Guid.NewGuid():N}",
            Message = "Payment processed via Alipay",
            Fee = amount * 0.006m // 0.6%
        };
    }
}

public class InteracProcessor : IPaymentProcessor
{
    public bool SupportsRefunds => true;

    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        Console.WriteLine($"[Interac] Processing ${amount}...");
        
        return new PaymentResult 
        { 
            Success = true, 
            TransactionId = $"interac_{Guid.NewGuid():N}",
            Message = "Payment processed via Interac",
            Fee = 1.00m // Flat $1 fee
        };
    }
}

// ===== Concrete Factories =====

public class USPaymentProcessorFactory : PaymentProcessorFactory
{
    public override IPaymentProcessor CreatePaymentProcessor(string paymentMethod)
    {
        return paymentMethod.ToLower() switch
        {
            "creditcard" => new StripeProcessor(),
            "paypal" => new PayPalProcessor(),
            "venmo" => new VenmoProcessor(),
            _ => throw new ArgumentException($"Payment method '{paymentMethod}' is not supported in the US.", nameof(paymentMethod))
        };
    }

    public override IEnumerable<string> GetSupportedMethods()
    {
        return new[] { "CreditCard", "PayPal", "Venmo" };
    }
}

public class EUPaymentProcessorFactory : PaymentProcessorFactory
{
    public override IPaymentProcessor CreatePaymentProcessor(string paymentMethod)
    {
        return paymentMethod.ToLower() switch
        {
            "creditcard" => new StripeProcessor(),
            "paypal" => new PayPalProcessor(),
            "sofort" => new SofortProcessor(),
            _ => throw new ArgumentException($"Payment method '{paymentMethod}' is not supported in the EU.", nameof(paymentMethod))
        };
    }

    public override IEnumerable<string> GetSupportedMethods()
    {
        return new[] { "CreditCard", "PayPal", "Sofort" };
    }
}

public class AsiaPaymentProcessorFactory : PaymentProcessorFactory
{
    public override IPaymentProcessor CreatePaymentProcessor(string paymentMethod)
    {
        return paymentMethod.ToLower() switch
        {
            "creditcard" => new StripeProcessor(),
            "paypal" => new PayPalProcessor(),
            "alipay" => new AlipayProcessor(),
            _ => throw new ArgumentException($"Payment method '{paymentMethod}' is not supported in Asia.", nameof(paymentMethod))
        };
    }

    public override IEnumerable<string> GetSupportedMethods()
    {
        return new[] { "CreditCard", "PayPal", "Alipay" };
    }
}

public class CanadaPaymentProcessorFactory : PaymentProcessorFactory
{
    public override IPaymentProcessor CreatePaymentProcessor(string paymentMethod)
    {
        return paymentMethod.ToLower() switch
        {
            "creditcard" => new StripeProcessor(),
            "interac" => new InteracProcessor(),
            _ => throw new ArgumentException($"Payment method '{paymentMethod}' is not supported in Canada.", nameof(paymentMethod))
        };
    }

    public override IEnumerable<string> GetSupportedMethods()
    {
        return new[] { "CreditCard", "Interac" };
    }
}

// ===== Factory Selector =====

public class RegionalFactorySelector
{
    private readonly Dictionary<string, PaymentProcessorFactory> _factories;
    
    public RegionalFactorySelector()
    {
        _factories = new Dictionary<string, PaymentProcessorFactory>(StringComparer.OrdinalIgnoreCase)
        {
            { "US", new USPaymentProcessorFactory() },
            { "EU", new EUPaymentProcessorFactory() },
            { "Asia", new AsiaPaymentProcessorFactory() },
            { "Canada", new CanadaPaymentProcessorFactory() }
        };
    }

    public PaymentProcessorFactory GetFactory(string region)
    {
        if (_factories.TryGetValue(region, out var factory))
        {
            return factory;
        }
        throw new ArgumentException($"Region '{region}' is not supported.", nameof(region));
    }
}

// ===== Usage =====

public class Program
{
    public static void Main()
    {
        var selector = new RegionalFactorySelector();
        var details = new PaymentDetails { CustomerEmail = "customer@example.com" };

        // Customer in US
        Console.WriteLine("========== US Customer ==========");
        var usFactory = selector.GetFactory("US");
        Console.WriteLine($"Supported: {string.Join(", ", usFactory.GetSupportedMethods())}");
        
        var usResult = usFactory.ProcessPayment("Venmo", 150.00m, details);
        DisplayResult(usResult);

        // Customer in EU
        Console.WriteLine("\n========== EU Customer ==========");
        var euFactory = selector.GetFactory("EU");
        Console.WriteLine($"Supported: {string.Join(", ", euFactory.GetSupportedMethods())}");
        
        var euResult = euFactory.ProcessPayment("Sofort", 200.00m, details);
        DisplayResult(euResult);

        // Customer in Asia
        Console.WriteLine("\n========== Asia Customer ==========");
        var asiaFactory = selector.GetFactory("Asia");
        Console.WriteLine($"Supported: {string.Join(", ", asiaFactory.GetSupportedMethods())}");
        
        var asiaResult = asiaFactory.ProcessPayment("Alipay", 300.00m, details);
        DisplayResult(asiaResult);

        // Customer in Canada
        Console.WriteLine("\n========== Canada Customer ==========");
        var canadaFactory = selector.GetFactory("Canada");
        Console.WriteLine($"Supported: {string.Join(", ", canadaFactory.GetSupportedMethods())}");
        
        var canadaResult = canadaFactory.ProcessPayment("Interac", 250.00m, details);
        DisplayResult(canadaResult);
    }

    private static void DisplayResult(PaymentResult result)
    {
        if (result.Success)
        {
            Console.WriteLine($" {result.Message}");
            Console.WriteLine($"  Transaction ID: {result.TransactionId}");
            Console.WriteLine($"  Fee: ${result.Fee:F2}");
        }
        else
        {
            Console.WriteLine($" Payment failed: {result.Message}");
        }
    }
}